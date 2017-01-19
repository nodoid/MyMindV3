using System.Collections.Generic;

namespace MyMindV3
{
    public interface ICometChat
    {
        void SetupMessenger();

        void LoginUser(string url, string username, string password);

        void LogoutUser();
    }
}

