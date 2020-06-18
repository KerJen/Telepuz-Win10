using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class UserStatusUpdateDTO
    {
        [Key("user_id")]
        public string UserId { get; set; }

        [Key("status")]
        public UserStatus Status { get; set; }
    }
}
