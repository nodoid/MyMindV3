using Xamarin.Forms;

namespace MyMindV3.Views
{
    class CometChatView : ContentPage
    {
        protected override void OnAppearing()
        {
            App.Self.MessageEvents.Change += (s, ea) =>
            {
                if (ea.ModuleName == "Chat")
                {
                    DisplayAlert("CometChar error", ea.Message, "OK");
                    Navigation.PopAsync();
                }
            };
        }

        public CometChatView()
        {
            DependencyService.Get<ILaunchCometChat>().LaunchChat();
        }
    }
}
