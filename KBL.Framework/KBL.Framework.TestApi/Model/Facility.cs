using KBL.Framework.DAL.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.TestApi.Model
{
    public class Facility : AuditableEntity
    {
        public string Name { get; set; }
        public string Shortcut { get; set; }
    }
}
