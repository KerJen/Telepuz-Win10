using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telepuz.API
{
    public static class ApiOptions
    {
        public static readonly string IpAddress = "127.0.0.1";
        public static readonly string Port = "5000";

        public static string Url => $"ws://{IpAddress}:{Port}";
    }
}
