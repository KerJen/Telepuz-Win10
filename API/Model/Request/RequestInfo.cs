using MessagePack;

namespace Telepuz.API.Model.Request
{
    [MessagePackObject]
    public class RequestInfo
    {
        [Key("method_name")]
        public string MethodName { get; set; }
    }
}
