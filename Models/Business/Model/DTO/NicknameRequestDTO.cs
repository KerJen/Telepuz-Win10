using MessagePack;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class NicknameRequestDTO
    {
        [Key("user_nickname")]
        public string Nickname { get; set; }
    }
}
