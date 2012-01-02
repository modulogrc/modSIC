using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modSIC_Client
{
    class MainPresenter
    {
        private IMainView view;
        private IMainModel model;

        public MainPresenter(IMainView _view, IMainModel _model)
        {
            view = _view;
            model = _model;

            view.OnFormLoad += new EventHandler(view_OnFormLoad);
            view.OnFormClose += new EventHandler(view_OnFormClose);
            view.OnLogin += new EventHandler<LoginEventArgs>(view_OnLogin);
        }

        void view_OnLogin(object sender, LoginEventArgs e)
        {
            throw new NotImplementedException();
        }

        void view_OnFormClose(object sender, EventArgs e)
        {
            // ...
            view.OnFormLoad -= new EventHandler(view_OnFormLoad);
            view.OnFormClose -= new EventHandler(view_OnFormClose);
        }

        void view_OnFormLoad(object sender, EventArgs e)
        {
            // ...
        }
    }
}
