using KBL.Framework.DAL.Base.Infrastructure;
using KBL.Framework.DAL.Interfaces.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.BAL.Base.Extensions
{
    public static class AuditEntityValuesExtensions
    {
        #region Fields
        private static string _connectionString;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static IConfiguration _configuration;
        #endregion

        #region Public methods
        public static IApplicationBuilder UseAuditEntityValuesForMssql(this IApplicationBuilder builder, IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
            AuditingHelper.Instance.IsAuditingEnable = true;
            CreateDbObjects();
            
            return builder;
        }
        #endregion

        #region Private methods
        private static void CreateDbObjects()
        {
            if (!IsSchemaExists())
            {
                CreateSchema();
            }
            if (!IsTableExists())
            {
                CreateTable();
            }
            if (!IsFunctionExists())
            {
                CreateToJsonFunction();
            }
            CreateTriggers();
        }

        private static bool IsSchemaExists()
        {
            string query = $"SELECT schema_name FROM information_schema.schemata WHERE schema_name = @name;";
            var par = new SqlParameter("@name", AuditingHelper.Instance.AuditingTableSchema);
            return ExecuteReaderSQLCommand(query, new List<SqlParameter>() { par });
        }

        private static bool IsTableExists()
        {

            string query = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table";
            var pars = new List<SqlParameter>()
            {
                new SqlParameter("@schema", AuditingHelper.Instance.AuditingTableSchema),
                new SqlParameter("@table", AuditingHelper.Instance.AuditingTableName.Replace(AuditingHelper.Instance.AuditingTableSchema+".",""))
            };
            return ExecuteReaderSQLCommand(query, pars);
        }

        private static bool CreateSchema()
        {
            var query = $"CREATE SCHEMA {AuditingHelper.Instance.AuditingTableSchema}";
            return ExecuteSQLCommand(query);
        }

        private static bool CreateTable()
        {
            string sqlText = $@"CREATE TABLE {AuditingHelper.Instance.AuditingTableName}(
                                [ID] [bigint] IDENTITY(1,1) NOT NULL,
                                [EntityID] [bigint] NOT NULL,
                                [TableName] [nvarchar](50) NOT NULL,
                                [ClassFullName] [nvarchar](250) NOT NULL,
                                [OldValue] [nvarchar](max) NOT NULL,
                                [NewValue] [nvarchar](max) NOT NULL,
                                [CreatedDateTime] [datetime] NOT NULL,
                             CONSTRAINT [PK_AuditEntities] PRIMARY KEY CLUSTERED 
                            (
                                [ID] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            return ExecuteSQLCommand(sqlText);
        }

        private static bool ExecuteReaderSQLCommand(string query, List<SqlParameter> collection)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(query, connection);
                    collection.ForEach(x => command.Parameters.Add(x));
                    connection.Open();
                    var reader = command.ExecuteReader();
                    return reader.Read();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error in ExecuteCommand() for {nameof(AuditEntityValuesExtensions)}");
                return false;
            }
        }

        private static bool ExecuteSQLCommand(string query)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(query, connection);
                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error in ExecuteCommand() for {nameof(AuditEntityValuesExtensions)}");
                return false;
            }
        }

        private static bool CreateToJsonFunction()
        {
            var function = "CREATE FUNCTION [logs].[RowToJson] (@IncludeHead int,@ToLowerCase int,@XML xml)\r\nReturns varchar(max)\r\nAS\r\nBegin\r\n    Declare @Head varchar(max) = '',@JSON varchar(max) = ''\r\n    ; with cteEAV as (Select RowNr=Row_Number() over (Order By (Select NULL))\r\n                            ,Entity    = xRow.value('@*[1]','varchar(100)')\r\n                            ,Attribute = xAtt.value('local-name(.)','varchar(100)')\r\n                            ,Value     = xAtt.value('.','varchar(max)') \r\n                       From  @XML.nodes('/row') As R(xRow) \r\n                       Cross Apply R.xRow.nodes('./@*') As A(xAtt) )\r\n          ,cteSum as (Select Records=count(Distinct Entity)\r\n                            ,Head = IIF(@IncludeHead=0,IIF(count(Distinct Entity)<=1,'[getResults]','[[getResults]]'),Concat('{\""+"status"+"\":{\"successful\":\"true\",\"timestamp\":\"',Format(GetUTCDate(),'yyyy-MM-dd hh:mm:ss '),'GMT','\",\"rows\":\"',count(Distinct Entity),'\"},\"results\":[[getResults]]}') ) \r\n                       From  cteEAV)\r\n          ,cteBld as (Select *\r\n                            ,NewRow=IIF(Lag(Entity,1)  over (Partition By Entity Order By (Select NULL))=Entity,'',',{')\r\n                            ,EndRow=IIF(Lead(Entity,1) over (Partition By Entity Order By (Select NULL))=Entity,',','}')\r\n                            ,JSON=Concat('\"',IIF(@ToLowerCase=1,Lower(Attribute),Attribute),'\":','\"',Value,'\"') \r\n                       From  cteEAV )\r\n    Select @JSON = @JSON+NewRow+JSON+EndRow,@Head = Head From cteBld, cteSum\r\n    Return Replace(@Head,'[getResults]',Stuff(@JSON,1,1,''))\r\nEnd";
            return ExecuteSQLCommand(function);
        }

        private static bool CreateTriggerForClass(string namespaceName, string className, string tableName)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("CREATE TRIGGER ").Append(CreateFullTriggerName(className)).Append("\n ON [").Append(_configuration["KBL.Framework.DAL.Config:TableSchema"]).Append("].[");
            stringBuilder.Append(tableName).Append("]").Append("\n AFTER UPDATE \n AS \n");
            stringBuilder.Append("BEGIN DECLARE @id bigint;	DECLARE @tableName NVARCHAR(50); DECLARE @classFullName NVARCHAR(250); DECLARE @oldValue NVARCHAR(MAX); DECLARE @newValue NVARCHAR(MAX);\n");
            stringBuilder.Append("SELECT @oldValue=[logs].[RowToJson](0,1,(Select * From DELETED for XML RAW));\n");
            stringBuilder.Append("SELECT @newValue=[logs].[RowToJson](0,1,(Select * From INSERTED for XML RAW));\n");
            stringBuilder.Append("SELECT @ID=id From INSERTED;\n");
            stringBuilder.Append("SET @tableName = '").Append(tableName).Append("';\n");
            stringBuilder.Append("SET @classFullName = '").Append(namespaceName).Append(".").Append(className).Append("';\n");
            stringBuilder.Append("INSERT INTO [logs].[AuditEntities](EntityId, TableName, ClassFullName, OldValue, NewValue, CreatedDateTime)\n")
                .Append("VALUES(@id, @tableName, @classFullName, @oldValue, @newValue, GETUTCDATE());\n").Append("END");

            return ExecuteSQLCommand(stringBuilder.ToString());
        }

        private static void CreateTriggers()
        {
            var section = _configuration.GetSection("KBL.Framework.DAL.Config:Auditing").AsEnumerable();
            string namespaceName = "";
            IDictionary<string, IList<string>> pairs = new Dictionary<string, IList<string>>();
            foreach (var item in section)
            {
                if (item.Key.Contains("Namespace") && !string.IsNullOrEmpty(item.Value) && !pairs.TryGetValue(item.Value, out IList<string> tables))
                {
                    namespaceName = item.Value;
                    tables = new List<string>();
                    pairs.Add(item.Value, tables);
                    continue;
                }
                if (item.Key.Contains("Entities") && !string.IsNullOrEmpty(item.Value))
                {
                    pairs[namespaceName].Add(item.Value);
                }
            }

            foreach (var item in pairs)
            {
                namespaceName = item.Key;
                foreach (var name in item.Value)
                {
                    if (!IsTriggerExists(CreateTriggerName(name)))
                    {
                        string tableName = _configuration[$"KBL.Framework.DAL.Config:Tables:{name}"] ?? name + "s";
                        CreateTriggerForClass(namespaceName, name, tableName);
                    }
                }
            }
        }

        private static string CreateFullTriggerName(string className)
        {
            return $"[{_configuration["KBL.Framework.DAL.Config:TableSchema"]}].[Log{className}History]";
        }

        private static string CreateTriggerName(string className)
        {
            return $"Log{className}History";
        }

        private static bool IsTriggerExists(string triggerName)
        {
            var query = "SELECT * FROM sys.triggers  WHERE name = @triggerName";
            var param = new SqlParameter("@triggerName", triggerName);
            return ExecuteReaderSQLCommand(query, new List<SqlParameter> { param });
        }

        private static bool IsFunctionExists()
        {
            var query = "SELECT * FROM sys.objects WHERE name = @name AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )";
            var param = new SqlParameter("@name", "RowToJson");
            return ExecuteReaderSQLCommand(query, new List<SqlParameter> { param });
        }
        #endregion
    }
}