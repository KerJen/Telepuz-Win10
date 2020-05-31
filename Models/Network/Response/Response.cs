using MessagePack;

namespace Telepuz.Models.Network.Response
{
    [MessagePackObject]
    public class Response
    {
        [Key("result")]
        public int Result { get; set; }

        [Key("data")]
        public object Data { get; set; }
    }
}
