using MessagePack;

namespace Telepuz.Models.Network.Model
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
