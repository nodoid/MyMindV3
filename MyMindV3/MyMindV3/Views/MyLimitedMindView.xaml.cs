using MvvmFramework.ViewModel;
using System;
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

        private async void LoadMyProfileBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyProfileView(), true);
        }

        private async void LoadResourcesBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyResourcesView(), true);
        }

    }
}
