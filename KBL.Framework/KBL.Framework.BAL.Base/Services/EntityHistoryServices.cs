using KBL.ExceptionManager.Model.Exceptions;
using KBL.Framework.BAL.Base.Entities;
using KBL.Framework.BAL.Base.Infrastructures;
using KBL.Framework.DAL.Base.Entities;
using KBL.Framework.DAL.Base.Repositories.Implementations;
using KBL.Framework.DAL.Interfaces.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KBL.Framework.BAL.Base.Services
{
    public class EntityHistoryServices
    {
        #region Fields
        private readonly AuditEntitiesQueryRepository _queryRepository;
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public EntityHistoryServices(IConfiguration configuration)
        {
            _queryRepository = new AuditEntitiesQueryRepository(configuration);
        }
        #endregion

        #region Public methods
        public IEnumerable<EntityHistoryDto<T>> GetHistory<T>(long id) where T : IEntity
        {
            var type = typeof(T).FullName;
            var data = _queryRepository.GetHistory(id, type);
            if (data.IsSuccess != DAL.Interfaces.Infrastructure.ResultType.OK)
            {
                throw new GetEntityException<AuditEntity>();
            }
            List<EntityHistoryDto<T>> result = new List<EntityHistoryDto<T>>();
            foreach (var item in data.ResultList)
            {
                EntityHistoryDto<T> dto = new EntityHistoryDto<T>
                {
                    EntityId = item.EntityId,
                    Timestamp = item.CreatedDateTime,
                    OldValue = JsonConvert.DeserializeObject<T>(item.OldValue, new[] { new JsonKblConverter() }),
                    NewValue = JsonConvert.DeserializeObject<T>(item.NewValue, new[] { new JsonKblConverter() })
                };
                dto.ChangedValues = GetChangedProperties<T>(dto.OldValue, dto.NewValue);
                result.Add(dto);
            }

            return result;
        }
        #endregion

        #region Private methods
        private static IDictionary<string, HistoryValueDto> GetChangedProperties<T>(T oldValue, T newValue)
        {
            var result = new Dictionary<string, HistoryValueDto>();
            var fields = typeof(T).GetProperties();
            foreach (var field in fields)
            {
                if (field?.GetValue(oldValue)?.Equals(field?.GetValue(newValue)) == false)
                {
                    result.Add(field.Name, new HistoryValueDto() { NewValue = field.GetValue(newValue)?.ToString(), OldValue = field.GetValue(oldValue)?.ToString() });
                }
            }
            return result;
        }
        #endregion
    }
}
