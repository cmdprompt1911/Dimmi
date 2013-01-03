using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Data
{
    public static class DataUtil
    {
        public static object CheckForEmptyStringVal(String value)
        {
            if (value.Trim().Length == 0)
            {
                return DBNull.Value;
            }
            else
            {
                return value.Trim();
            }
        }
        public static object CheckForNullValReturnEmptyString(Object value)
        {
            if (value is DBNull)
            {
                return "";
            }
            else
            {
                return value;
            }
        }
    }
}