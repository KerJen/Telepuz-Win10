using MessagePack;

namespace Telepuz.Models.Business.Model
{
    [MessagePackObject]
    public class Message
    {
        [Key("id")]
        public string Id { get; set; }

        [Key("user_id")]
        public string UserId { get; set; }
        
        [Key("text")]
        public string Text { get; set; }

        [IgnoreMember]
        public bool Yours { get; set; }

        [IgnoreMember] 
        public User User { get; set; }

        [IgnoreMember]
        public bool UserInfoVisible { get; set; }
    }
}
