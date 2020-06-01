using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Telepuz.Helpers;
using Telepuz.Models.Business;
using Telepuz.Models.Business.Model.Nickname;
using Telepuz.Models.Network;
using WebSocketSharp;

namespace Telepuz.ViewModels
{
    // TODO: Внедрить репозитории с DI
    public class LoginViewModel : ViewModelBase
    {
        readonly Regex _nicknameRegex = new Regex("^[A-zА-яЁё0-9\\s]{1,30}$");

        readonly Random _rand = new Random();

        readonly MediaPlayer _player = new MediaPlayer();

        readonly TelepuzWebSocketService _client;

        public LoginViewModel()
        {
            // Инициализация делегатов нажатий
            SloganClick = new RelayCommand(PlayAle);
            EnterButtonClick = new RelayCommand(SendNickname, NicknameCheck);

            _player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/ale.mp3"));

            SetPhrases();

            _client = TelepuzWebSocketService.Client;

            _client.Connect();
        }

        public RelayCommand EnterButtonClick { get; private set; }
        public RelayCommand SloganClick { get; private set; }

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
                foreach (var phrase in StringHelper.aleStrings)
                {
                    Phrase = phrase;
                    await Task.Delay(_rand.Next(1000,1300));
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

        /// <summary>
        /// Проверка формата имени
        /// </summary>
        /// <returns></returns>
        bool NicknameCheck()
        {
            return Nickname != null && _nicknameRegex.IsMatch(Nickname);
        }

        // TODO: Перенести в репозитории
        /// <summary>
        /// Отправка запроса с никнеймом на сервер
        /// </summary>
        void SendNickname()
        {
            // Прослушивание ответа с сервера
            _client.Once("auth.login", (response) =>
            {
                if (response.Result == (int)Results.OK)
                {

                }
            });

            // Подготовка объекта запроса
            var nicknameRequestDTO = new NicknameRequestDTO()
            {
                Nickname = Nickname
            };

            _client.Request<NicknameRequestDTO>("auth.login", nicknameRequestDTO);
        }
    }
}
