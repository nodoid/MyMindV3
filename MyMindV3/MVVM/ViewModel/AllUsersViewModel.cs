namespace MvvmFramework.ViewModel
{
    public class AllUsersViewModel : BaseViewModel
    {
        private int _allUsersTotal;

        public int AllUsersTotal
        {
            get { return _allUsersTotal; }
            set
            {
                if (value != _allUsersTotal)
                {
                    Set(() => AllUsersTotal, ref _allUsersTotal, value, true); 
                }
            }
        }
    }
}
