using KBL.Framework.BAL.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.BAL.Base.Entities
{
    public class BaseDto : IDto
    {
        #region Properties
        public long ID { get; set; }
        #endregion
    }
}
