using MessagePack;

namespace Telepuz.API.Model.DTO.Send
{
    [MessagePackObject()]
    public class NicknameDataSend
    {
        [Key("nickname")]
        public string Nickname { get; set; }
    }
}
