using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Telepuz.Models.Business.Model.DTO
{
    [MessagePackObject]
    public class UserDeletedUpdateDTO
    {
        [Key("user_id")]
        public string UserId { get; set; }
    }
}
