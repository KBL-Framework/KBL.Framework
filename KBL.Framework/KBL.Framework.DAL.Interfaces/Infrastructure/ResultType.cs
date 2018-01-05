using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Interfaces.Infrastructure
{
    public enum ResultType
    {
        OK = 1,
        TooMuchData = 2,
        NotExistingQueryParameter = 3,
        NotExistingQueryParameterValue = 4,
        SqlError = 5,
        Error = 6,
        NotExistingStoredQuery = 7
    }
}
