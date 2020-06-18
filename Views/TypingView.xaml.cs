using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace Telepuz
{
    public sealed partial class TypingView : UserControl
    {
        public TypingView()
        {
            this.InitializeComponent();
            StartAnimation();
        }

        async void StartAnimation()
        {
            firstStoryboard.Begin();
            await Task.Delay(100);
            secondStoryboard.Begin();
            await Task.Delay(100);
            thirdStoryboard.Begin();
        }
    }
}
