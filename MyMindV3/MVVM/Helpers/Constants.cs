﻿namespace MvvmFramework
{
    public class Constants
    {
        public static string BaseUrl { get; private set; } = "https://apps.nelft.nhs.uk/CareMapApi/api/";

        public static string BaseChatUrl { get; private set; } = "https://apps.nelft.nhs.uk/ChatSig/Account/";

        public static string BaseTestUrl { get; private set; } = "https://apps.nelft.nhs.uk/CareMapAPI-Test";

        public static string CometChatAPIKey { get; private set; }

        public static string CometChatURL { get; private set; } = "https://apps.nelft.nhs.uk/cometchat";

        public static string CometChatMyId { get; set; }

        public static string TestFairyKey { get; private set; } = "1761398db506414b977ca868067ae3179077d42a";

        public static string DESKey { get; private set; } = "yxO7qG5CK03M1fJ7fP6cuXwxz3H5t9pI";
    }
}
