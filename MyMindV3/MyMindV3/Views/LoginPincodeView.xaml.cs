using MvvmFramework.ViewModel;
using System;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class LoginPincodeView : ContentPage
    {
        MyClinicianViewModel ViewModel => App.Locator.MyClinician;
        //int restrictCount = 1;

        public LoginPincodeView()
        {
            //InitializeComponent();
            BindingContext = ViewModel;

            //Pin1.TextChanged += OnPinOneTextChanged;
            //Pin2.TextChanged += OnPinTwoTextChanged;
            //Pin3.TextChanged += OnPinThreeTextChanged;
            //Pin4.TextChanged += OnPinFourTextChanged;            
        }

        /*
        void OnPinOneTextChanged(object sender, EventArgs evtArgs)
        {
            Entry e = sender as Entry;
            String val = e.Text; //Get Current Text

            if (val.Length > restrictCount)//If it is more than your character restriction
            {
                val = val.Remove(val.Length - 1);// Remove Last character 
                e.Text = val; //Set the Old value
            }

            PinInputOne.Focus();
        }

        void OnPinTwoTextChanged(object sender, EventArgs evtArgs)
        {
            Entry e = sender as Entry;
            String val = e.Text; //Get Current Text

            if (val.Length > restrictCount)//If it is more than your character restriction
            {
                val = val.Remove(val.Length - 1);// Remove Last character 
                e.Text = val; //Set the Old value
            }

            Pin2 = val;

            PinInputTwo.Focus();
        }

        void OnPinThreeTextChanged(object sender, EventArgs evtArgs)
        {
            Entry e = sender as Entry;
            String val = e.Text; //Get Current Text

            if (val.Length > restrictCount)//If it is more than your character restriction
            {
                val = val.Remove(val.Length - 1);// Remove Last character 
                e.Text = val; //Set the Old value
            }

            Pin3 = val;

            PinInputThree.Focus();
        }

        void OnPinFourTextChanged(object sender, EventArgs evtArgs)
        {
            Entry e = sender as Entry;
            String val = e.Text; //Get Current Text

            if (val.Length > restrictCount)//If it is more than your character restriction
            {
                val = val.Remove(val.Length - 1);// Remove Last character 
                e.Text = val; //Set the Old value
            }
            
            Pin4 = val;

            PinInputFour.Focus();
        }
        */

        private async void PinClickSubmit(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyPlansView(), true);
        }
    }
}
