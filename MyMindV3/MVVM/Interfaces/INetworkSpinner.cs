using System;
namespace MvvmFramework
{
    public interface INetworkSpinner
    {
        void NetworkSpinner(bool on, string title, string message);
    }
}

