using System;

namespace Telepuz.Models.Network
{
    public class Callback
    {
        public Type Type { get; set; }

        public Action<object> Action { get; set; }
    }
}
