/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:MVVM"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using MvvmFramework.ViewModel;

namespace MvvmFramework
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        /// 
        public const string AllRegisteredUsersKey = "AllRegisteredUsers";
        public const string LoginPinCodeKey = "LoginPinCode";
        public const string LoginKey = "Login";
        public const string MyChatKey = "MyChat";
        public const string MyClinicianKey = "MyClinician";
        public const string MyFileKey = "MyFile";
        public const string MyJourneyKey = "MyJourney";
        public const string MyLimitedMindKey = "MyLimitedMind";
        public const string MyMindKey = "MyMind";
        public const string MyPatientKey = "MyPatient";
        public const string MyPlansKey = "MyPlans";
        public const string MyProfileKey = "MyProfile";
        public const string MyResourcesKey = "MyResources";
        public const string SignupKey = "SignUp";

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IRepository, SqLiteRepository>();
            SimpleIoc.Default.Register<AllRegisteredUserViewModel>();
            SimpleIoc.Default.Register<LoginPinCodeViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MyChatViewModel>();
            SimpleIoc.Default.Register<MyClinicianViewModel>();
            SimpleIoc.Default.Register<MyFileViewModel>();
            SimpleIoc.Default.Register<MyJourneyViewModel>();
            SimpleIoc.Default.Register<MyLimitedMindViewModel>();
            SimpleIoc.Default.Register<MyMindViewModel>();
            SimpleIoc.Default.Register<MyPatientViewModel>();
            SimpleIoc.Default.Register<MyPlansViewModel>();
            SimpleIoc.Default.Register<MyProfileViewModel>();
            SimpleIoc.Default.Register<MyResourcesViewModel>();
            SimpleIoc.Default.Register<SignUpViewModel>();
        }

        public AllRegisteredUserViewModel AllRegisteredUsers
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AllRegisteredUserViewModel>();
            }
        }

        public LoginPinCodeViewModel LoginPinCode
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginPinCodeViewModel>();
            }
        }
        public LoginViewModel Login
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }
        public MyChatViewModel MyChat
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyChatViewModel>();
            }
        }
        public MyClinicianViewModel MyClinician
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyClinicianViewModel>();
            }
        }
        public MyFileViewModel MyFile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyFileViewModel>();
            }
        }
        public MyJourneyViewModel MyJourney
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyJourneyViewModel>();
            }
        }
        public MyLimitedMindViewModel MyLimitedMind
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyLimitedMindViewModel>();
            }
        }
        public MyMindViewModel MyMind
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyMindViewModel>();
            }
        }
        public MyPatientViewModel MyPatient
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyPatientViewModel>();
            }
        }
        public MyPlansViewModel MyPlans
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyPlansViewModel>();
            }
        }
        public MyProfileViewModel MyProfile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyProfileViewModel>();
            }
        }
        public MyResourcesViewModel MyResources
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyResourcesViewModel>();
            }
        }
        public SignUpViewModel SignUp
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SignUpViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}