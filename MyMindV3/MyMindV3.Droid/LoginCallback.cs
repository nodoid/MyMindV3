using System;
using Com.Inscripts.Interfaces;

namespace MyMindV3.Droid
{
    public class LoginCallback : Java.Lang.Object, ILoginCallbacks
    {
        Action<Org.Json.JSONObject> _onSuccess;
        Action<Org.Json.JSONObject> _onFail;
        Action<Org.Json.JSONObject> _onUserInfoCallback;
        Action<Org.Json.JSONObject> _onChatroomInfoCallback;
        Action<Org.Json.JSONObject> _onMessageReceive;
        Action<Org.Json.JSONObject> _onChatWindowListner;

        public LoginCallback(Action<Org.Json.JSONObject> onSuccess, Action<Org.Json.JSONObject> onFail, Action<Org.Json.JSONObject> onUserInfoCallback
            , Action<Org.Json.JSONObject> onChatroomInfoCallback, Action<Org.Json.JSONObject> onMessageReceive, Action<Org.Json.JSONObject> onChatWindowListner)
        {
            this._onFail = onFail;
            this._onSuccess = onSuccess;
            this._onUserInfoCallback = onUserInfoCallback;
            this._onChatroomInfoCallback = onChatroomInfoCallback;
            this._onMessageReceive = onMessageReceive;
            this._onChatWindowListner = onChatWindowListner;
        }

        public void FailCallback(Org.Json.JSONObject p0)
        {
            this._onFail?.Invoke(p0);
        }
        public void SuccessCallback(Org.Json.JSONObject p0)
        {
            this._onSuccess?.Invoke(p0);
        }

        public void UserInfoCallback(Org.Json.JSONObject p0)
        {
            this._onUserInfoCallback?.Invoke(p0);
        }

        public void ChatroomInfoCallback(Org.Json.JSONObject p0)
        {
            this._onChatroomInfoCallback?.Invoke(p0);
        }


        public void OnMessageReceive(Org.Json.JSONObject p0)
        {
            this._onMessageReceive?.Invoke(p0);
        }


        public void ChatWindowListner(Org.Json.JSONObject p0)
        {
            this._onChatWindowListner?.Invoke(p0);
        }
    }
}