using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Telepuz.ViewModels;

namespace Telepuz
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((LoginViewModel)DataContext).LoadData();
        }

        private void LoginPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            nicknameInput.Focus(FocusState.Programmatic);
        }

        private void OnEnterDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ((LoginViewModel)DataContext).EnterButtonClick.Execute(null);
            }
        }
    }
}
