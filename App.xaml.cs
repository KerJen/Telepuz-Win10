using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Threading;
using Telepuz.Models.Business.Model;
using Telepuz.Models.Business.Model.DTO;
using Telepuz.Models.Network;

namespace Telepuz
{
    /// <summary>
    /// Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
    /// </summary>
    sealed partial class App : Application
    {

        TelepuzWebSocketService _client;

        /// <summary>
        /// Инициализирует одноэлементный объект приложения. Это первая выполняемая строка разрабатываемого
        /// кода, поэтому она является логическим эквивалентом main() или WinMain().
        /// </summary> 
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;
        }

        /// <summary>
        /// Вызывается при обычном запуске приложения пользователем. Будут использоваться другие точки входа,
        /// например, если приложение запускается для открытия конкретного файла.
        /// </summary>
        /// <param name="e">Сведения о запросе и обработке запуска.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            DispatcherHelper.Initialize();
            // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
            // только обеспечьте активность окна
            if (rootFrame == null)
            {
                // Создание фрейма, который станет контекстом навигации, и переход к первой странице
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Загрузить состояние из ранее приостановленного приложения
                }

                // Размещение фрейма в текущем окне
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    _client = TelepuzWebSocketService.Client;


                    _client.OnServiceResponse((wbResult) =>
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            if (wbResult == WebSocketResults.CONNECTION_SUCCESS)
                            {
                                rootFrame.Navigate(typeof(LoginPage), e.Arguments);
                            }
                            else if (wbResult == WebSocketResults.CONNECTION_CLOSED &&
                                     rootFrame.CurrentSourcePageType != typeof(NoInternetConnectionPage))
                            {
                                rootFrame.Navigate(typeof(NoInternetConnectionPage));
                            }
                        });
                    });

                    _client.Connect();
                }
                // Обеспечение активности текущего окна
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Вызывается в случае сбоя навигации на определенную страницу
        /// </summary>
        /// <param name="sender">Фрейм, для которого произошел сбой навигации</param>
        /// <param name="e">Сведения о сбое навигации</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        void UpdateUserStatus(UserStatus status)
        {
            _client.Request("users.updateStatus", new UserUpdateStatusRequestDTO()
            {
                UserStatus = UserStatus.AFK
            });
        }

        /// <summary>
        /// Вызывается при приостановке выполнения приложения.  Состояние приложения сохраняется
        /// без учета информации о том, будет ли оно завершено или возобновлено с неизменным
        /// содержимым памяти.
        /// </summary>
        /// <param name="sender">Источник запроса приостановки.</param>
        /// <param name="e">Сведения о запросе приостановки.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            UpdateUserStatus(UserStatus.AFK);
            deferral.Complete();
        }

        /// <summary>
        /// Вызывается при возобновлении приложения
        /// </summary>
        /// <param name="sender">Источник запроса возобновления.</param>
        /// <param name="e">Сведения о запросе возобновления.</param>
        private void OnResuming(object sender, object e)
        {
            UpdateUserStatus(UserStatus.Online);
        }

    }
}
