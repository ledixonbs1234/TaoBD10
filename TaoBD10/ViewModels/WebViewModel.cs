using CefSharp;
using CefSharp.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaoBD10.ViewModels
{
    public class WebViewModel :ObservableObject
    {
        public IRelayCommand<ChromiumWebBrowser> LoadPageCommand;

        private IWpfWebBrowser _WebBrowser;

        public IWpfWebBrowser WebBrowser
        {
            get { return _WebBrowser; }
            set { SetProperty(ref _WebBrowser, value); }
        }

        public WebViewModel()
        {
            LoadPageCommand = new RelayCommand<ChromiumWebBrowser>(LoadPage);
            LoginCommand = new RelayCommand(Login);
            
        }

        void Login()
        {
            AddressWeb = "https://bccp.vnpost.vn/BCCP.aspx?act=Trace";
        }
        bool isInitializeWeb = false;

        private string _AddressWeb = "https://www.google.com";
        bool isInitilizeWeb = false;

        public string AddressWeb
        {
            get { return _AddressWeb; }
            set {SetProperty(ref _AddressWeb , value); }
        }

        public ICommand LoginCommand;


        void LoadPage(ChromiumWebBrowser web)
        {
        }
    }
}
