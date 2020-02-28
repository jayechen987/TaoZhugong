using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models
{
    public static class EnumTool
    {
        public static T ConvertStrToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}