using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telepuz.Helpers;

namespace Telepuz.Models.Business.Model.User
{
    public class ChatUser
    {
        readonly Regex chatUserNicknameRegex = new Regex("^[A-zА-яЁё0-9\\s]{1,30}$");

        readonly Random rand = new Random();

        public ChatUser(string name, string status)
        {
            AvatarBackgroundColor = Constants.avatarColors[rand.Next(0, Constants.avatarColors.Length)];
            Nickname = name;
            FirstLetter = Nickname[0].ToString().ToUpper();
            Status = status;
        }

        public string AvatarBackgroundColor { get; set; }

        string _nickname;
        public string Nickname
        {
            get => _nickname;
            set
            {
                if (value == null || !chatUserNicknameRegex.IsMatch(value))
                {
                    throw new ArgumentException("Неправильный формат никнейма");
                }

                _nickname = value;
            }
        }

        public string FirstLetter { get; private set; }

        public string Status { get; set; }
    }
}
