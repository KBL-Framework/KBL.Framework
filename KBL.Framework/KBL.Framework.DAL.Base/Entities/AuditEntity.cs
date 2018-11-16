using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Base.Entities
{
    public class AuditEntity
    {
        #region Properties
        public long EntityId { get; set; }
        public string EntityType { get; set; }
        public string EntityTable { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }
        #endregion
    }
}
