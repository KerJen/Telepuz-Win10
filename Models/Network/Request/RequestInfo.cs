using MessagePack;

namespace Telepuz.Models.Network.Request
{
    [MessagePackObject]
    public class RequestInfo
    {
        [Key("method_name")]
        public string MethodName { get; set; }
    }
}
