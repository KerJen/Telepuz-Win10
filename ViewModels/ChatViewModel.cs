using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Telepuz.Helpers;
using Telepuz.Models.Business.Model.DTO;
using Telepuz.Models.Business.Model.User;
using Telepuz.Models.Network;

namespace Telepuz.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        readonly Random rand = new Random();

        readonly TelepuzWebSocketService _client;

        ObservableCollection<ChatUser> _users;
        public ObservableCollection<ChatUser> Users
        {
            get => _users;
            set
            {
                _users = value;
                DispatcherHelper.CheckBeginInvokeOnUI(() => { RaisePropertyChanged("Users"); });
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

            GetAllUsers();
        }


        void GetAllUsers()
        {
           _client.Once<ChatUsersDTO>("users.getAll", (response) =>
           {
               var users = ((ChatUsersDTO) response.Data).Users;

               Users = new ObservableCollection<ChatUser>(users);
           });

           _client.Request("users.getAll");
        }
    }
}
