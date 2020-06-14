using MessagePack;

namespace Telepuz.Models.Network.Model
{
    [MessagePackObject]
    public class ResponseInfo
    {
        [Key("method_name")]
        public string MethodName { get; set; }
    }
}
