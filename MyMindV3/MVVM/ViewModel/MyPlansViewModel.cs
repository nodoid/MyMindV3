using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvvmFramework.ViewModel
{
    public class MyPlansViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyPlansViewModel(INavigationService nav)
        {
            navService = nav;
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Plans_Page_View : ActionCodes.User_My_Plans_Page_View);
        }

        public string GuidToUse { get; set; }

        IEnumerable<ClientPlan> myPlan;
        public IEnumerable<ClientPlan> MyPlan
        {
            get { return myPlan; }
            set { Set(() => MyPlan, ref myPlan, value); }
        }

        public async Task GetClientPlans()
        {
            var url = string.Format("/api/MyMind/GetMyPlans/", new List<string> { "guid", GuidToUse, "authtoken", SystemUser.APIToken }.ToArray());
            var resources = await Send.GetPostListObject<ClientPlan>(url);
            MyPlan = resources.AsEnumerable();
        }
    }
}
