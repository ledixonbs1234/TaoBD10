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

        private string _AddressWeb = "https://www.google.com";
        bool isInitilizeWeb = false;

        public string AddressWeb
        {
            get { return _AddressWeb; }
            set {SetProperty(ref _AddressWeb , value);

                if (!isInitilizeWeb)
                {
                    isInitilizeWeb = true;
                    WebBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
                    //AddressWeb = "https://bccp.vnpost.vn/BCCP.aspx?act=Trace";
                    WebBrowser.LoadUrlAsync("genk.vn");

                }
            }
        }

        private void WebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                //Da tai xong
                string diachi = WebBrowser.Address.ToLower();

                if (diachi.IndexOf("login") != -1)
                {
                    string scriptFirst = @"
Element.prototype.remove = function() {
    this.parentElement.removeChild(this);
}
NodeList.prototype.remove = HTMLCollection.prototype.remove = function() {
    for(var i = this.length - 1; i >= 0; i--) {
        if(this[i] && this[i].parentElement) {
            this[i].parentElement.removeChild(this[i]);
        }
    }
}
document.getElementById(""vnpNav"").remove();
document.getElementsByClassName("".mainHomepageTitle"").remove();
document.getElementsByClassName("".mainHomepageUserLogin"").remove();
document.getElementsByClassName("".footer"").remove();

                                    var textbox = document.getElementById(""MainContent_imgCaptcha"");
    textbox.focus();
                    textbox.scrollIntoView();
                    ";
                    //_WebBrowser.ex
                    //string html= await chrWeb.GetSourceAsync();
                    //Regex regexLogin = new Regex(@"MainContent_imgCaptcha"" src=""((\w|\W)+?)""");
                    //var matchLogin = regexLogin.Match(html);
                    //if (matchLogin != null)
                    //{

                    //    picImage.Load("https://bccp.vnpost.vn/" + matchLogin.Groups[1].Value);
                    //}

                    _WebBrowser.EvaluateScriptAsync(scriptFirst);
                }
                else
                {
                    //kiem tra dieu kien url

                }
            }
        }

        public ICommand LoginCommand { get; }


        void LoadPage(ChromiumWebBrowser web)
        {
        }
    }
}
