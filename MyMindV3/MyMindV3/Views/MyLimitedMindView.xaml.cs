using MyMindV3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class MyLimitedMindView : ContentPage
    {
        private RootViewModel _rootVM;

        public MyLimitedMindView(RootViewModel rootVM)
        {
            InitializeComponent();
            _rootVM = rootVM;
        }

        private async void LoadMyProfileBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyProfileView(_rootVM), true);
        }

        private async void LoadResourcesBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyResourcesView(), true);
        }

    }
}
