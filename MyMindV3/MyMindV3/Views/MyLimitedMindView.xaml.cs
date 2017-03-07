using MvvmFramework.ViewModel;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class MyLimitedMindView : ContentPage
    {
        MyLimitedMindViewModel ViewModel => App.Locator.MyLimitedMind;

        public MyLimitedMindView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }
    }
}
