using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Telepuz.Models.Business.Model.User;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class ChatUsersDTO
    {
        [Key("users")]
        public List<ChatUser> Users { get; set; }
    }
}
