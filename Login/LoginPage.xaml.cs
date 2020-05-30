using Telepuz.API;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Telepuz.API.Model.DTO.Send;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Telepuz
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            InitClient();
        }

        static void InitClient()
        {
            var client = TelepuzWebSocketService.Client;

            client.Connect();


            // Проверка методов

            client.Once("auth.login", (response) =>
            {
                var t = response.Result;
            });


            client.Request("auth.login", new NicknameDataSend()
            {
                Nickname = "KerJen"
            });

        }

        private void enterButton_Click(object sender, RoutedEventArgs e)
        {
            nicknameInput.IsEnabled = false;
        }
    }
}
