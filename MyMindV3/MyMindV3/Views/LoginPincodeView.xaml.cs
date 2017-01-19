using MyMindV3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class LoginPincodeView : ContentPage
    {
        private LoginPinCodeViewModel _loginPinCodeVM;
        private RootViewModel _rootVM;
        private INavigation _navigation;
        //int restrictCount = 1;

        public LoginPincodeView(RootViewModel rootVM)
        {
            //InitializeComponent();
            RootVM = rootVM;
            _navigation = this.Navigation;

            _loginPinCodeVM = new LoginPinCodeViewModel(RootVM, _navigation);
            BindingContext = _loginPinCodeVM;

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
            await Navigation.PushAsync(new MyPlansView(RootVM), true);
        }


        // provide access to RootVM
        public RootViewModel RootVM
        {
            get { return _rootVM; }
            set
            {
                if (value != _rootVM)
                {
                    _rootVM = value;
                    OnPropertyChanged("RootVM");
                }
            }
        }
    }
}
