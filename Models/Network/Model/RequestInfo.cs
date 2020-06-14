using MessagePack;

namespace Telepuz.Models.Network.Model
{
    [MessagePackObject]
    public class RequestInfo
    {
        [Key("method_name")]
        public string MethodName { get; set; }
    }
}
