using GalaSoft.MvvmLight.Views;
using MvvmFramework.Helpers;
using System.IO;

namespace MvvmFramework.ViewModel
{
    public class MyClinicianViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyClinicianViewModel(INavigationService nav)
        {
            navService = nav;
        }

        public Stream GetClinicianImage(string filename)
        {
            return new FileIO().LoadFile(filename).Result; 
        }
    }
}
