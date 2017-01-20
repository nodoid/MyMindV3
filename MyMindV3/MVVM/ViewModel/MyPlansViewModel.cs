using GalaSoft.MvvmLight.Views;
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
        }

        public string GuidToUse { get; set; }

        IEnumerable<ClientPlan> myPlan;
        public IEnumerable<ClientPlan> MyPlan
        {
            get { GetClientPlans().ConfigureAwait(true); return myPlan; }
            set { Set(() => MyPlan, ref myPlan, value); }
        }

        async Task GetClientPlans()
        {
            var url = string.Format("/api/MyMind/GetMyPlans/{0}", GuidToUse);
            var resources = await GetData.GetListObject<ClientPlan>(url);
            MyPlan = resources.AsEnumerable();
        }
    }
}
