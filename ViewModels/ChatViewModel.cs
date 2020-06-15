using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Telepuz.Models.Business.Model;
using Telepuz.Models.Business.Model.DTO;
using Telepuz.Models.Network;

namespace Telepuz.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        readonly TelepuzWebSocketService _client;

        ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                DispatcherHelper.CheckBeginInvokeOnUI(() => { RaisePropertyChanged("Users"); });
            }
        }

        ObservableCollection<Message> _messages;
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                DispatcherHelper.CheckBeginInvokeOnUI(() => { RaisePropertyChanged("Messages"); });
            }
        }

        string _inputMessage;
        public string InputMessage
        {
            get => _inputMessage;
            set
            {
                _inputMessage = value;
                RaisePropertyChanged("InputMessage");
            }
        }

        public RelayCommand SendClick { get; }

        public ChatViewModel(INavigationService navigationService)
        {
            SendClick = new RelayCommand(SendMessage, InputMessageCheck);

            Users = new ObservableCollection<User>();
            Messages = new ObservableCollection<Message>();

            _client = TelepuzWebSocketService.Client;

            ListenUserChange();
            GetAllUsers();
            ListenNewMessage();
        }

        void ListenUserChange()
        {
            _client.On<UserNewUpdateDTO>("updates.user.new", (update) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { Users.Add(update.NewUser); });
            });

            _client.On<UserDeletedUpdateDTO>("updates.user.deleted", (update) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { Users.Remove(Users.First(x => x.Id == update.UserId)); });
            });
        }

        void GetAllUsers()
        {
            _client.Once<UsersResponseDTO>("users.getAll", (response) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { Users = new ObservableCollection<User>(response.Users); });
            });

            _client.Request("users.getAll");
        }

        void ListenNewMessage()
        {
            _client.On<MessageNewUpdateDTO>("updates.message.new", (update) =>
            {
                var message = update.Message;
                message.Yours = false;

                DispatcherHelper.CheckBeginInvokeOnUI(() => { Messages.Add(message); });
            });
        }

        bool InputMessageCheck()
        {
            return InputMessage != null;
        }

        void SendMessage()
        {
            var messageText = InputMessage;

            _client.Once<MessageSendReponseDTO>("messages.send", (response) =>
            {
                var message = new Message()
                {
                    Id = "e",
                    Text = messageText,
                    User = null,
                    UserId = "ff",
                    Yours = true
                };

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { Messages.Add(message); });
                });
            });

            _client.Request("messages.send", new MessageSendRequestDTO()
            {
                Text = InputMessage
            });
        }
    }
}
