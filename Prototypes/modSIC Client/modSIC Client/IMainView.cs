using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modSIC_Client
{
    interface IMainView
    {
        event EventHandler OnFormClose;
        event EventHandler OnFormLoad;
        event EventHandler<LoginEventArgs> OnLogin;

        void ShowErrorMessage(string message);
    }

    public class LoginEventArgs : EventArgs
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
