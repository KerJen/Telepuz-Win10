using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.Toolkit.Uwp.Notifications;
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
                else if (_typing && !value)
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

        readonly Timer timer = new Timer(2000);

        string _user_id;

        private bool isWindowActive;

        public ChatViewModel(INavigationService navigationService)
        {
            MessengerInstance.Register<string>(this, data =>
            {
                _user_id = data;
            });


            SendClick = new RelayCommand(SendMessage, InputMessageCheck);

            Users = new ObservableCollection<User>();
            Messages = new ObservableCollection<Message>();

            _client = TelepuzWebSocketService.Client;

            timer.AutoReset = false;
        }

        public void LoadData()
        {
            Users.Clear();
            Messages.Clear();

            GetAllUsers();
            ListenApplicationEvents();
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
                    Users.First(x => x.Id == update.UserId).Status = update.Status;
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

                var nickname = Users.First(x => x.Id == update.Message.UserId).Nickname;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if(!isWindowActive)
                        MakeNotification(nickname, update.Message.Text);
                    Messages.Add(update.Message);
                });
            });
        }

        void MakeNotification(string nickname, string message)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = $"{nickname} пишет"
                            },
                            new AdaptiveText()
                            {
                                Text = message
                            }
                        }
                    }
                }
            };

            var toastNotif = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
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
                    Id = "1",
                    Text = messageText,
                    User = null,
                    UserId = _user_id,
                    Yours = true
                };



                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Messages.Add(message);

                    timer.Stop();
                    Typing = false;
                });
            });

            _client.Request("messages.create", new MessageSendRequestDTO()
            {
                Text = messageText
            });
        }

        void UpdateUserStatus(UserStatus status)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { Users.First(x => x.Id == _user_id).Status = status; });
            _client.Request("users.updateStatus", new UserUpdateStatusRequestDTO()
            {
                UserStatus = status
            });
        }

        void ListenApplicationEvents()
        {
            Window.Current.CoreWindow.Activated += (sender, args) =>
            {
                if (args.WindowActivationState == CoreWindowActivationState.Deactivated)
                {
                    isWindowActive = false;
                    UpdateUserStatus(UserStatus.AFK);
                }
                else
                {
                    isWindowActive = true;
                    UpdateUserStatus(UserStatus.Online);
                }
            };
        }
    }
}
