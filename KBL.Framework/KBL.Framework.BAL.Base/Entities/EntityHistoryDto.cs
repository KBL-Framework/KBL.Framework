using KBL.Framework.BAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.BAL.Base.Entities
{
    public class EntityHistoryDto<T> where T : IEntity
    {
        public long EntityId { get; set; }
        public DateTime Timestamp { get; set; }
        public T OldValue { get; set; }
        public T NewValue { get; set; }
        public IDictionary<string, HistoryValueDto> ChangedValues { get; set; }
    }
}
