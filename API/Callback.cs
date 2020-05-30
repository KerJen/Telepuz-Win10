using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telepuz.API
{
    public class Callback
    {
        public Type Type { get; set; }

        public Action<object> Action { get; set; }
    }
}
