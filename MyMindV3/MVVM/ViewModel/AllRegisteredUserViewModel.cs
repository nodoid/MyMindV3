using GalaSoft.MvvmLight.Views;
using MvvmFramework.Models;
using System.Collections.Generic;
using System.Linq;

namespace MvvmFramework.ViewModel
{
    public class AllRegisteredUserViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlRepo;

        public AllRegisteredUserViewModel(INavigationService nav, IRepository repo)
        {
            navService = nav;
            sqlRepo = repo;
        }

        IEnumerable<SystemUser> getSystemUsers;
        public IEnumerable<SystemUser> GetSystemUsers
        {
            get { return sqlRepo.GetList<SystemUser>().AsEnumerable(); }
        }

        int allUsersTotal;
        public int AllUsersTotal
        {
            get { return allUsersTotal; }
            set { Set(() => AllUsersTotal, ref allUsersTotal, value); }
        }
    }
}
