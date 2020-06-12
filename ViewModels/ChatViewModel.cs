using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Telepuz.Models.Business.Model.User;
using Telepuz.Models.Network;

namespace Telepuz.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        readonly TelepuzWebSocketService _client;

        ObservableCollection<ChatUser> _users;
        public ObservableCollection<ChatUser> Users
        {
            get => _users;
            set
            {
                _users = value;
                RaisePropertyChanged("Users");
            }
        }

        string _message;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        public ChatViewModel(INavigationService navigationService)
        {
            Users = new ObservableCollection<ChatUser>();

            _client = TelepuzWebSocketService.Client;

            AddChatUsers();
        }


        async void AddChatUsers()
        {
            while (true)
            {
                Users.Add(new ChatUser("ddd", "Dd"));
                await Task.Delay(1000);
            }
        }


    }
}
