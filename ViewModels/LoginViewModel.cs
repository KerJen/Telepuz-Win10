using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Telepuz.Helpers;
using Telepuz.Models.Business;
using Telepuz.Models.Business.Model.DTO;
using Telepuz.Models.Network;

namespace Telepuz.ViewModels
{
    // TODO: Внедрить репозитории с DI
    public class LoginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        readonly Regex _nicknameRegex = new Regex("^[A-zА-яЁё0-9\\s]{1,30}$");

        readonly Random _rand = new Random();

        readonly MediaPlayer _player = new MediaPlayer();

        readonly TelepuzWebSocketService _client;

        string _phrase;
        public string Phrase
        {
            get => _phrase;
            set
            {
                _phrase = value;
                RaisePropertyChanged("Phrase");
            }
        }

        /// <summary>
        /// Меняет фразу слогана с помощью биндингов
        /// </summary>
        async void SetPhrases()
        {
            while (true)
            {
                foreach (var phrase in Constants.aleStrings)
                {
                    Phrase = phrase;
                    await Task.Delay(_rand.Next(1000, 1300));
                }
            }
        }

        /// <summary>
        /// Проигрывает звук алё)
        /// </summary>
        void PlayAle()
        {
            _player.Play();
        }

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

        bool _loading;
        public bool Loading
        {
            get => _loading;
            set
            {
                _loading = value;
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    RaisePropertyChanged("Loading");
                    EnterButtonClick.RaiseCanExecuteChanged();
                });
            }
        }

        public RelayCommand EnterButtonClick { get; }
        public RelayCommand SloganClick { get; }

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            // Инициализация делегатов нажатий
            SloganClick = new RelayCommand(PlayAle);
            EnterButtonClick = new RelayCommand(SendNickname, EnterButtonCheck);

            _player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/ale.mp3"));

            _client = TelepuzWebSocketService.Client;
        }


        public void LoadData()
        {
            //TODO: load
        }

        /// <summary>
        /// Проверка - может ли быть активна кнопка
        /// </summary>
        /// <returns></returns>
        bool EnterButtonCheck()
        {
            return !Loading && Nickname != null && _nicknameRegex.IsMatch(Nickname);
        }

        // TODO: Перенести в репозитории
        /// <summary>
        /// Отправка запроса с никнеймом на сервер
        /// </summary>
        void SendNickname()
        {
            Loading = true;
            // Прослушивание ответа с сервера
            _client.Once<NicknameResponseDTO>("users.create", (response) =>
            {
                if (response.Result == (int)Results.OK)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _navigationService.NavigateTo("Chat");
                        Messenger.Default.Send<string>(response.UserId);
                        Loading = false;
                    });
                }
            });

            // Подготовка объекта запроса
            var nicknameRequestDTO = new NicknameRequestDTO()
            {
                Nickname = Nickname
            };

            _client.Request("users.create", nicknameRequestDTO);
        }
    }
}
