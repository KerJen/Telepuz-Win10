using MessagePack;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class MessageSendRequestDTO
    {
        [Key("message_text")]
        public string Text { get; set; }
    }
}
