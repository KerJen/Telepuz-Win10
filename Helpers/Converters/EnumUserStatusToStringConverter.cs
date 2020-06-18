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
    public class EnumUserStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var userStatus = (UserStatus) value;
            switch (userStatus)
            {
                case UserStatus.AFK:
                    return "отошел";
                case UserStatus.Online:
                    return "онлайн";
                case UserStatus.Typing:
                    return "печатает...";
                default:
                    return "онлайн";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var userStatus = value.ToString();
            switch (userStatus)
            {
                case "отошел":
                    return UserStatus.AFK;
                case "онлайн":
                    return UserStatus.Online;
                case "печатает...":
                    return UserStatus.Typing;
                default:
                    return UserStatus.Online;
            }
        }
    }
}
