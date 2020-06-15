using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class MessageSendReponseDTO
    {
        [Key("message_id")]
        public string MessageId { get; set; }
    }
}
