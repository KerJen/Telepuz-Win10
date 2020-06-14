using MessagePack;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class NicknameRequestDTO
    {
        [Key("nickname")]
        public string Nickname { get; set; }
    }
}
