using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Telepuz.Models.Business;
using Telepuz.Models.Business.Model.Nickname;
using Telepuz.Models.Network;
using WebSocketSharp;

namespace Telepuz.ViewModels
{
    // TODO: Внедрить репозитории с DI
    public class LoginViewModel : ViewModelBase
    {
        readonly Regex nicknameRegex = new Regex("^[A-zА-яЁё0-9\\s]{1,30}$");

        readonly TelepuzWebSocketService _client;

        public LoginViewModel()
        {
            EnterButtonClick = new RelayCommand(SendNickname, NicknameCheck);

            _client = TelepuzWebSocketService.Client;

            _client.Connect();
        }

        public RelayCommand EnterButtonClick { get; private set; }

        // TODO: Посмотреть способ сокращения записи
        string _nickname;
        public string Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                RaisePropertyChanged("Nickname");
                EnterButtonClick.RaiseCanExecuteChanged();
            }
        }

        bool NicknameCheck()
        {
            return Nickname != null && nicknameRegex.IsMatch(Nickname);
        } 

        // TODO: Перенести в репозитории
        void SendNickname()
        {
            _client.Once("auth.login", (response) =>
            {
                if (response.Result == (int) Results.OK)
                {

                }
            });

            var nicknameRequestDTO = new NicknameRequestDTO()
            {
                Nickname = Nickname
            };

            _client.Request<NicknameRequestDTO>("auth.login", nicknameRequestDTO);
        }
    }
}
