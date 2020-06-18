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
    public class EnumUserStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var userStatus = (UserStatus) value;
            switch (userStatus)
            {
                case UserStatus.AFK:
                    return "#FF6D00";
                case UserStatus.Online:
                    return "#00BCD4";
                case UserStatus.Typing:
                    return "#00BCD4";
                default:
                    return "онлайн";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var userStatus = value.ToString();
            switch (userStatus)
            {
                case "#FF6D00":
                    return UserStatus.AFK;
                case "#00BCD4":
                    return UserStatus.Typing;
                default:
                    return UserStatus.Online;
            }
        }
    }
}
