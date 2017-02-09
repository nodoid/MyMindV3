using Xamarin.Forms;
using MyMindV3.Languages;

namespace MyMindV3.Views
{
    public class ApptList : ViewCell
    {
        public ApptList()
        {
            var lblApptDT = new Label
            {
                FontSize = 15,
                TextColor = Color.White,
            };

            var lblLoc = new Label
            {
                FontSize = 15,
                TextColor = Color.White
            };

            var lblStatus = new Label
            {
                FontSize = 15,
                TextColor = Color.White
            };

            lblApptDT.SetBinding(Label.TextProperty, new Binding("AppointmentDateTime"));
            lblLoc.SetBinding(Label.TextProperty, new Binding("AppointmentLocation"));
            lblStatus.SetBinding(Label.TextProperty, new Binding("AppointmentStatus"));

            View = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label {Text = Langs.MyJourney_Date, FontSize=15, TextColor = Color.White, FontAttributes=FontAttributes.Italic},
                            lblApptDT
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label {Text = Langs.MyJourney_Loc, FontSize=15, TextColor = Color.White, FontAttributes=FontAttributes.Italic},
                            lblLoc
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label {Text = Langs.MyJourney_Outcome, FontSize=15, TextColor = Color.White, FontAttributes=FontAttributes.Italic},
                            lblStatus
                        }
                    }
                }
            };
        }
    }
}
