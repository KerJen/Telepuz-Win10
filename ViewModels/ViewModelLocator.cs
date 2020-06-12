using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;

namespace Telepuz.ViewModels
{
    public class ViewModelLocator
    {
        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();
        public ChatViewModel Chat => ServiceLocator.Current.GetInstance<ChatViewModel>();

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigationService = CreateNavigationService();
            SimpleIoc.Default.Register(() => navigationService);

            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<ChatViewModel>();
        }

        private static INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure("Login", typeof(LoginPage));
            navigationService.Configure("Chat", typeof(ChatPage));
            return navigationService;
        }

    }
}
