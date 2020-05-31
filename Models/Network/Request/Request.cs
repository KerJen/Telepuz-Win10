using MessagePack;

namespace Telepuz.Models.Network.Request
{
    [MessagePackObject]
    public class Request<T>
    {
        [Key("data")]
        public T Data { get; set; }
    }
}
