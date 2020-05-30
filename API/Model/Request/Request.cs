using MessagePack;

namespace Telepuz.API.Model.Request
{
    [MessagePackObject]
    public class Request<T>
    {
        [Key("data")]
        public T Data { get; set; }
    }
}
