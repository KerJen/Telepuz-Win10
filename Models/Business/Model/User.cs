using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using MessagePack;
using Telepuz.Annotations;
using Telepuz.Helpers;

namespace Telepuz.Models.Business.Model
{
    [MessagePackObject]
    public class User : INotifyPropertyChanged
    {

        string _id;
        [Key("id")]
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        string _nickname;
        [Key("nickname")]
        public string Nickname {
            get => _nickname;
            set
            {
                _nickname = value;
                OnPropertyChanged();
            }
        }

        UserStatus _status;
        [Key("status")]
        public UserStatus Status {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

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


        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
