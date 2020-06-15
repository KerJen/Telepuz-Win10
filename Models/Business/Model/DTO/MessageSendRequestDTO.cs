using MessagePack;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class MessageSendRequestDTO
    {
        [Key("text")]
        public string Text { get; set; }
    }
}
