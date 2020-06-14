using MessagePack;

namespace Telepuz.Models.Network.Model
{
    [MessagePackObject]
    public class Request<T>
    {
        [Key("data")]
        public T Data { get; set; }
    }
}
