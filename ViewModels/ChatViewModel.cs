using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Telepuz.Helpers;
using Telepuz.Models.Business.Model;
using Telepuz.Models.Business.Model.DTO;
using Telepuz.Models.Network;
using Timer = System.Timers.Timer;

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

        bool _typing;
        private bool Typing
        {
            set
            {
                if (!_typing && value)
                {
                    UpdateUserStatus(UserStatus.Typing);
                }
                else if(_typing && !value)
                {
                    UpdateUserStatus(UserStatus.Online);
                }

                _typing = value;
            }
        }

        string _inputMessage;
        public string InputMessage
        {
            get => _inputMessage;
            set
            {
                Typing = true;
                timer.Stop();
                timer.Start();

                _inputMessage = value;
                RaisePropertyChanged("InputMessage");
                SendClick.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand SendClick { get; }

        readonly Timer timer = new Timer(5000);

        public ChatViewModel(INavigationService navigationService)
        {
            SendClick = new RelayCommand(SendMessage, InputMessageCheck);

            Users = new ObservableCollection<User>();
            Messages = new ObservableCollection<Message>();

            _client = TelepuzWebSocketService.Client;

            timer.AutoReset = false;

            GetAllUsers();
            ListenUserChange();
            ListenUserStatus();
            ListenNewMessage();
        }


        void ListenUserChange()
        {
            _client.On<UserNewUpdateDTO>("users.created", (update) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { Users.Add(update.NewUser); });
            });

            _client.On<UserDeletedUpdateDTO>("users.removed", (update) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { Users.Remove(Users.First(x => x.Id == update.UserId)); });
            });
        }

        void GetAllUsers()
        {
            _client.Once<UsersResponseDTO>("users.get", (response) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { Users = new ObservableCollection<User>(response.Users); });
            });

            _client.Request("users.get");
        }

        private void ListenUserStatus()
        {
            timer.Elapsed += (sender, args) =>
            {
                Typing = false;
            };

            _client.On<UserStatusUpdateDTO>("users.statusUpdated", (update) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var user = Users.First(x => x.Id == update.UserId);
                    user.Status = update.Status;

                    Users[Users.FindIndex(x => x.Id == update.UserId)] = user;
                });
            });
        }

        void ListenNewMessage()
        {
            _client.On<MessageNewUpdateDTO>("messages.created", (update) =>
            {
                update.Message.Yours = false;
                update.Message.User = Users.First(x => x.Id == update.Message.UserId);

                if (Messages.Count == 0 || update.Message.UserId != Messages.Last().UserId)
                {
                    update.Message.UserInfoVisible = true;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() => { Messages.Add(update.Message); });
            });
        }

        bool InputMessageCheck()
        {
            return !string.IsNullOrEmpty(InputMessage) && InputMessage.Length < 6000;
        }

        void SendMessage()
        {
            var messageText = InputMessage;
            InputMessage = "";

            _client.Once<MessageSendReponseDTO>("messages.create", response =>
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

            _client.Request("messages.create", new MessageSendRequestDTO()
            {
                Text = messageText
            });
        }

        void UpdateUserStatus(UserStatus status)
        {
            _client.Request("users.updateStatus", new UserUpdateStatusRequestDTO()
            {
                UserStatus = status
            });
        }
    }
}
