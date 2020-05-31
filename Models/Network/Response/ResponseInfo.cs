using MessagePack;

namespace Telepuz.Models.Network.Response
{
    [MessagePackObject]
    public class ResponseInfo
    {
        [Key("method_name")]
        public string MethodName { get; set; }
    }
}
