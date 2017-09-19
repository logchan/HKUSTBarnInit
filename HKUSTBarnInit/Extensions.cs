using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKUSTBarnInit
{
    public static class Extensions
    {
        public static bool IsTrue(this System.Collections.Specialized.NameValueCollection settings, string key)
        {
            return (settings[key] ?? String.Empty).ToLower() == "true";
        }
    }
}
