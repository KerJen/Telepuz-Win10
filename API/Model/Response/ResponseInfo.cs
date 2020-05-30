using MessagePack;

namespace Telepuz.API.Model.Response
{
    [MessagePackObject]
    public class ResponseInfo
    {
        [Key("method_name")]
        public string MethodName { get; set; }
    }
}
