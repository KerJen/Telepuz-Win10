using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MessagePack;
using Telepuz.Helpers;

namespace Telepuz.Models.Business.Model.User
{
    [MessagePackObject]
    public class ChatUser
    {

        readonly SHA256 sha = SHA256.Create();

        [Key("id")]
        public string Id { get; set; }

        [Key("nickname")]
        public string Nickname { get; set; }

        [IgnoreMember]
        public string Status => "Тест";

        [IgnoreMember]
        public string AvatarBackground
        {
            get
            {
                var nicknameHashBytes = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(Nickname));
                var hashNumber = BitConverter.ToInt32(nicknameHashBytes, 0);

                return Constants.avatarColors[Math.Abs(hashNumber) % Constants.avatarColors.Length]; 
            }
        }

        [IgnoreMember]
        public string FirstLetter => Nickname[0].ToString().ToUpper();
    }
}
