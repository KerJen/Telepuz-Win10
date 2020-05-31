using MessagePack;

namespace Telepuz.Models.Business.Model.Nickname
{
    [MessagePackObject()]
    public class NicknameRequestDTO
    {
        [Key("nickname")]
        public string Nickname { get; set; }
    }
}
