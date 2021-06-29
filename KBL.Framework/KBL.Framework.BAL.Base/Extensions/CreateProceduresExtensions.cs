using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KBL.Framework.BAL.Base.Extensions
{
    public static class CreateProceduresExtensions
    {
        #region Fields
        private static string _connectionString;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static IConfiguration _configuration;

        private static readonly string ROOT_CONFIG_PATH = "KBL.Framework.DAL.Config";
        private static string _procedureSchema;
        private static string _tableSchema;
        private static string _procedureNamePattern;
        private static string _primaryKeyColumn;

        private static List<string> _ignoreForAdd = new List<string>() { "modifieddatetime", "modifiedby", "deleteddatetime", "deletedby" };
        private static List<string> _ignoreForUpdate = new List<string>() { "createddatetime", "createdby", "deleteddatetime", "deletedby" };
        private static List<string> _forDelete = new List<string>() { "deleteddatetime", "deletedby" };
        #endregion

        #region Properties
        #endregion

        #region Cstors

        #endregion

        #region Public methods
        public static IApplicationBuilder UseCreateProceduresForMssql(this IApplicationBuilder builder, IConfiguration configuration)
        {
            AssignFields(configuration);
            CreateProcedures();
            return builder;
        }
        #endregion

        #region Private methods
        private static void AssignFields(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
            _procedureSchema = _configuration[$"{ROOT_CONFIG_PATH}:ProcedureSchema"];
            _procedureNamePattern = _configuration[$"{ROOT_CONFIG_PATH}:ProcedurePattern"];
            _tableSchema = _configuration[$"{ROOT_CONFIG_PATH}:TableSchema"];
            _primaryKeyColumn = _configuration[$"{ROOT_CONFIG_PATH}:PrimaryKeyColumn"];

            //_ignoreForAdd.Add(_primaryKeyColumn.ToLower());
            //_ignoreForUpdate.Add(_primaryKeyColumn.ToLower());
            _forDelete.Add(_primaryKeyColumn.ToLower());
        }

        private static void CreateProcedures()
        {
            var scs = CreateScripts();
            if (scs?.Any() == true)
            {
                ExecuteSQLCommands(scs);
            }
        }

        private static List<string> CreateScripts()
        {
            var flag = _configuration[$"{ROOT_CONFIG_PATH}:CreateProcedure"];
            bool recreate = false;
            if (string.IsNullOrEmpty(flag) || flag.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            else
            {
                recreate = flag.Equals("Always", StringComparison.OrdinalIgnoreCase);
            }
            CreateSchema();
            List<string> scripts = new List<string>();
            foreach (var item in GetClasses())
            {
                string[] procs = new string[]
                {
                    CreateProcedureFullName("Create", item),
                    CreateProcedureFullName("Update", item),
                    CreateProcedureFullName("Delete", item)
                };
                var tableName = CreateTableName(item);
                var metas = GetTableMetainfo(tableName).ToList();
                for (int i = 0; i < procs.Length; i++)
                {
                    string proc = procs[i];
                    var isExits = IsProcedureExists(proc.Replace("[", "").Replace("]", "").Replace(".", "").Replace(_procedureSchema, ""));
                    if (recreate || !isExits)
                    {
                        switch (i)
                        {
                            case 0:
                                scripts.Add(CreateAddProcedure(proc, tableName, isExits ? "ALTER" : "CREATE", metas));
                                break;
                            case 1:
                                scripts.Add(CreateUpdateProcedure(proc, tableName, isExits ? "ALTER" : "CREATE", metas));
                                break;
                            case 2:
                                scripts.Add(CreateDeleteProcedure(proc, tableName, isExits ? "ALTER" : "CREATE", metas));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return scripts;
        }

        private static string CreateAddProcedure(string procedureName, string table, string dbOperation, List<(string column, string type, int? len)> metas)
        {
            metas = metas.Where(x => !_ignoreForAdd.Contains(x.column.ToLower())).ToList();
            StringBuilder builder = CreateProcedureHead(procedureName, dbOperation, metas);
            builder
                .Append("INSERT INTO ")
                .Append("[")
                .Append(_tableSchema)
                .Append("].[")
                .Append(table)
                .Append("]\n(\n");
            metas = metas.Where(x => !x.column.Equals(_primaryKeyColumn, StringComparison.OrdinalIgnoreCase)).ToList();
            metas.ForEach(x => builder.Append("[").Append(x.column).Append("],"));
            builder.Length--; // posledni carka pryc
            builder.Append(")\n VALUES \n(");
            metas.ForEach(x => builder.Append("@").Append(x.column).Append(","));
            builder.Length--; // posledni carka pryc
            builder.Append(");\n SET @Id = cast(scope_identity() as int); \n END;");
            return builder.ToString();
        }

        private static StringBuilder CreateProcedureHead(string procedureName, string dbOperation, List<(string column, string type, int? len)> metas)
        {
            var builder = new StringBuilder();
            builder.Append(dbOperation).Append(" PROCEDURE ").Append(procedureName).Append(" \n");
            foreach (var (column, type, len) in metas)
            {
                builder.Append("@").Append(column).Append(" ").Append(type);
                if (len != null && len != 0)
                {
                    builder.Append("(").Append(len).Append(") ");
                }
                if (column.ToUpper().Equals(_primaryKeyColumn))
                {
                    builder.Append(" output");
                }
                builder.Append(",\n");
            }
            builder.Length -= 2;
            builder.Append("\nAS \nBEGIN\n");
            return builder;
        }

        private static string CreateUpdateProcedure(string procedureName, string table, string dbOperation, List<(string column, string type, int? len)> metas)
        {
            metas = metas.Where(x => !_ignoreForUpdate.Contains(x.column.ToLower())).ToList();
            return CreateDmlUpdate(procedureName, table, dbOperation, metas);
        }

        private static string CreateDmlUpdate(string procedureName, string table, string dbOperation, List<(string column, string type, int? len)> metas)
        {
            var builder = CreateProcedureHead(procedureName, dbOperation, metas);
            CreateSqlCommandPart(table, "UPDATE", builder);
            builder.Append("\n SET ");
            metas = metas.Where(x => !x.column.Equals(_primaryKeyColumn, StringComparison.OrdinalIgnoreCase)).ToList();
            metas.ForEach(x => builder.Append("\n[").Append(x.column).Append("]").Append(" = @").Append(x.column).Append(","));
            builder.Length--;
            builder.Append("\n WHERE [").Append(_primaryKeyColumn).Append("] = @").Append(_primaryKeyColumn).Append(";\n").Append("END;");
            return builder.ToString();
        }

        private static void CreateSqlCommandPart(string table, string dmlCommand, StringBuilder builder)
        {
            builder.Append(" ").Append(dmlCommand).Append(" ").Append("[").Append(_tableSchema).Append("].[").Append(table).Append("]");
        }

        private static string CreateDeleteProcedure(string procedureName, string table, string dbOperation, List<(string column, string type, int? len)> metas)
        {
            metas = metas.Where(x => _forDelete.Contains(x.column.ToLower())).ToList();
            return CreateDmlUpdate(procedureName, table, dbOperation, metas);
        }

        private static string CreateTableName(string className)
        {
            string tableName = _configuration[$"{ROOT_CONFIG_PATH}:Tables:{className}"];
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = className + "s";
            }
            return tableName;
        }

        private static string CreateProcedureFullName(string operation, string className)
        {
            return $"[{_procedureSchema}].[{_procedureNamePattern.Replace("<Action>", operation).Replace("<Entity>", className)}]";
        }

        private static string CreateProcedureName(string operation, string className)
        {
            return $"{_procedureNamePattern.Replace("<Action>", operation).Replace("<Entity>", className)}";
        }

        private static IEnumerable<(string column, string type, int? len)> GetTableMetainfo(string tableName)
        {
            string query = @"SELECT COLUMN_NAME, DATA_TYPE, ISNULL(CHARACTER_MAXIMUM_LENGTH,0)
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_NAME = @tableName
                            AND TABLE_SCHEMA = @schema";
            List<(string column, string type, int? len)> list = new List<(string, string, int?)>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(query, connection);
                    command.Parameters.Add(new SqlParameter("@tableName", tableName));
                    command.Parameters.Add(new SqlParameter("@schema", _tableSchema));
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add((reader.GetString(0), reader.GetString(1), reader.GetInt32(2)));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error in GetTableMetainfo() for {tableName}");
                return null;
            }
            return list;
        }

        private static bool CreateSchema()
        {
            var query = $"IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = '{_procedureSchema}') EXEC('CREATE SCHEMA {_procedureSchema}')";
            return ExecuteSQLCommands(new List<string> { query });
        }

        private static bool IsProcedureExists(string procedureName)
        {
            string sql = $"SELECT * FROM sys.objects WHERE name = @procedureName AND type IN ( N'P', N'PC ' )";
            return ExecuteSQLReader(sql, new List<SqlParameter> { new SqlParameter("@procedureName", procedureName) });
        }

        private static bool ExecuteSQLCommands(List<string> scripts)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    foreach (var item in scripts)
                    {
                        var command = new SqlCommand(item, connection);
                        var result = command.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error in ExecuteCommand() for {nameof(CreateProceduresExtensions)}");
                return false;
            }
        }

        private static bool ExecuteSQLReader(string query, List<SqlParameter> parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(query, connection);
                    if (parameters?.Any() == true)
                    {
                        parameters.ForEach(x => command.Parameters.Add(x));
                    }
                    connection.Open();
                    var reader = command.ExecuteReader();
                    return reader.Read();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error in ExecuteCommand() for {nameof(CreateProceduresExtensions)}");
                return false;
            }
        }

        private static List<string> GetClasses()
        {
            List<string> classes = new List<string>();
            string assemblyName = _configuration[$"{ROOT_CONFIG_PATH}:ModelAssembly"];
            string modelNs = _configuration[$"{ROOT_CONFIG_PATH}:ModelNamespace"];
            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(modelNs))
            {
                return classes;
            }
            if (!assemblyName.EndsWith(".dll"))
            {
                assemblyName += ".dll";
            }
            var path = Assembly.GetEntryAssembly().Location;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, assemblyName);
            var assembly = Assembly.LoadFrom(path);
            return assembly.GetTypes()
              .Where(t => String.Equals(t.Namespace, modelNs, StringComparison.Ordinal))
              .Select(x => x.Name)
              .ToList();
        }
        #endregion
    }
}
