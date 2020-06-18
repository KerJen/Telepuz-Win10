using System;
using System.Security.Cryptography;
using System.Text;
using MessagePack;
using Telepuz.Helpers;

namespace Telepuz.Models.Business.Model
{
    [MessagePackObject]
    public class User
    {
        [Key("id")]
        public string Id { get; set; }

        [Key("nickname")]
        public string Nickname { get; set; }

        [Key("status")]
        public UserStatus Status { get; set; }

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
