using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Telepuz.Models.Business.Model;

namespace Telepuz
{
    public class MessageItemSelector : DataTemplateSelector
    {
        public DataTemplate YourMessageTemplate { get; set; }
        public DataTemplate MateMessageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var message = (Message)item;

            return message.Yours ? YourMessageTemplate : MateMessageTemplate;
        }
    }
}
