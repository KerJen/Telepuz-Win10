using MessagePack;

namespace Telepuz.API.Model.Response
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
