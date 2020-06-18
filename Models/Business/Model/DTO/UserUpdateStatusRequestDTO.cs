using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class UserUpdateStatusRequestDTO
    {
        [Key("user_status")]
        public UserStatus UserStatus { get; set; }
    }
}
