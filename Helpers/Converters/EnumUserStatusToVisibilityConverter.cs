using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Telepuz.Models.Business.Model;

namespace Telepuz.Helpers.Converters
{
    public class EnumUserStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var userStatus = (UserStatus) value;
            switch (userStatus)
            {
                case UserStatus.AFK:
                    return Visibility.Collapsed;
                case UserStatus.Online:
                    return Visibility.Collapsed;
                case UserStatus.Typing:
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var userStatus = (Visibility) value;
            switch (userStatus)
            {
                case Visibility.Collapsed:
                    return UserStatus.Online;
                case Visibility.Visible:
                    return UserStatus.Typing;
                default:
                    return UserStatus.AFK;
            }
        }
    }
}
