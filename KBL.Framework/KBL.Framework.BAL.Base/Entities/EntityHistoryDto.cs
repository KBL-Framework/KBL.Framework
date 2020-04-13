using KBL.Framework.DAL.Interfaces.Entities;
using System;
using System.Collections.Generic;

namespace KBL.Framework.BAL.Base.Entities
{
    public class EntityHistoryDto<T> where T : IEntity
    {
        public long EntityId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public T OldValue { get; set; }
        public T NewValue { get; set; }
        public IDictionary<string, HistoryValueDto> ChangedValues { get; set; }
    }
}
