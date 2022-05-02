using CefSharp;
using CefSharp.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class WebViewModel : ObservableObject
    {
        public WebViewModel()
        {
            LoadPageCommand = new RelayCommand<ChromiumWebBrowser>(LoadPage);
            LoginCommand = new RelayCommand(Login);

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "LoadAddressWeb")
                {
                    _LoadWebChoose = LoadWebChoose.DiNgoaiAddress;
                    LoadAddressDiNgoai(m.Content);
                }else if (m.Key == "LoadAddressTQWeb")
                {
                    _LoadWebChoose = LoadWebChoose.AddressTamQuan;
                    LoadAddressDiNgoai(m.Content);
                }
                else if (m.Key == "GetCodeFromBD")
                {
                    _LoadWebChoose = LoadWebChoose.CodeFromBD;
                    LoadAddressDiNgoai(m.Content);
                }
            });


        }

        public string AddressWeb
        {
            get { return _AddressWeb; }
            set
            {
                SetProperty(ref _AddressWeb, value);
                if (!isInitializeWeb)
                {
                    WebBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
                }
            }
        }

        public ICommand LoginCommand { get; }
        public ChromiumWebBrowser WebBrowser
        {
            get { return _WebBrowser; }
            set { SetProperty(ref _WebBrowser, value); }
        }

        void LoadAddressDiNgoai(string code)
        {

            string script = @"
                                document.getElementById('MainContent_ctl00_txtID').value='" + code + @"';
                				document.getElementById('MainContent_ctl00_btnView').click();
                ";
            IsLoadedWeb = false;
            WebBrowser.ExecuteScriptAsync(script);
        }

        void LoadPage(ChromiumWebBrowser web)
        {
        }

        void Login()
        {
            string script = @"
                     document.getElementById('MainContent_txtUser').value='593280';
            		document.getElementById('MainContent_txtPassword').value='593280';
            		document.getElementById('MainContent_btnLogin').click();
";

            WebBrowser.ExecuteScriptAsync(script);
        }

        private async void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                //Da tai xong
                string diachi = AddressWeb.ToLower();

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
                    WebBrowser.ExecuteScriptAsync(scriptFirst);
                    //string html= await chrWeb.GetSourceAsync();
                    //Regex regexLogin = new Regex(@"MainContent_imgCaptcha"" src=""((\w|\W)+?)""");
                    //var matchLogin = regexLogin.Match(html);
                    //if (matchLogin != null)
                    //{

                    //    picImage.Load("https://bccp.vnpost.vn/" + matchLogin.Groups[1].Value);
                    //}

                }
                else
                {
                    if (!IsLoadedWeb)
                    {
                        IsLoadedWeb = true;
                    }
                    else
                        return;
                    //kiem tra dieu kien url
                    string html = await WebBrowser.GetSourceAsync();

                    //kiem tra thu co no khong
                    if (_LoadWebChoose == LoadWebChoose.DiNgoaiAddress|| _LoadWebChoose == LoadWebChoose.AddressTamQuan)
                    {
                        Regex regex = new Regex(@"MainContent_ctl00_lblBarcode"">((\w|\W)+?)<");
                        var match = regex.Match(html);
                        string barcodeWeb = match.Groups[1].Value.ToUpper();
                        if (string.IsNullOrEmpty(barcodeWeb))
                        {
                            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Không đúng Code" });
                            return;
                        }
                        WebContentModel webContent = new WebContentModel
                        {
                            Code = barcodeWeb
                        };
                        Regex regexMaTinh = new Regex(@"MainContent_ctl00_lblDesPOS"">((\w|\W)+?) ");
                        string matchMaTinh = regexMaTinh.Match(html).Groups[1].Value;
                        if (String.IsNullOrEmpty(matchMaTinh))
                        {
                            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Chưa có mã Tỉnh" });
                            return;
                        }

                        string addressR = new Regex(@"MainContent_ctl00_lblReceiverAddr(\W|\w)+?>((\W|\w)+?)<").Match(html).Groups[2].Value;
                        string addressS = new Regex(@"MainContent_ctl00_lblSenderAddr(\W|\w)+?>((\W|\w)+?)<").Match(html).Groups[2].Value;
                        string buuCucGui = new Regex(@"MainContent_ctl00_lblFrPOS(\W|\w)+?>((\W|\w)+?)<").Match(html).Groups[2].Value;
                        if (string.IsNullOrEmpty(addressR))
                        {
                            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Không có địa chỉ" });
                            return;
                        }
                        webContent.AddressReiceive = addressR;
                        webContent.AddressSend = addressS;
                        webContent.BuuCucPhat = matchMaTinh;
                        webContent.BuuCucGui = buuCucGui;
                        if (_LoadWebChoose == LoadWebChoose.DiNgoaiAddress)
                            webContent.Key = "DiNgoaiAddress";
                        else if (_LoadWebChoose == LoadWebChoose.AddressTamQuan)
                            webContent.Key = "AddressTamQuan";
                        //Thuc hien send Web content qua do
                        WeakReferenceMessenger.Default.Send<WebContentModel>(webContent);
                    }
                    else if (_LoadWebChoose == LoadWebChoose.CodeFromBD)
                    {
                        Regex regex = new Regex(@"MainContent_ctl00_lblBarcode"">((\w|\W)+?)<");
                        var match = regex.Match(html);
                        string barcodeWeb = match.Groups[1].Value;
                        if (string.IsNullOrEmpty(barcodeWeb))
                        {
                            //txtInfo.Text = "Lỗi ! Chạy Lại";
                            return;
                        }


                        Regex regexMaTinh = new Regex(@"<a href=""\/BCCP.aspx\?act=Trace(\w|\W)+?"" title=""Xem chi tiết"">((\w|\W)+?)<\/a>");
                        var matchMaTinh = regexMaTinh.Matches(html);
                        if (matchMaTinh.Count != 1)
                        {
                            //gui ve la khong co tui trong nay
                            WeakReferenceMessenger.Default.Send<SHTuiMessage>(new SHTuiMessage(new SHTuiCodeModel { Key = "ReturnSHTui", Code = "NULL", SHTui = barcodeWeb }));
                        }
                        else
                        {
                            var giatri = matchMaTinh[0].Groups[2].Value;
                            //thuc hien viec luu gia tri hien tai

                            WeakReferenceMessenger.Default.Send<SHTuiMessage>(new SHTuiMessage(new SHTuiCodeModel { Key = "ReturnSHTui", Code = giatri, SHTui = barcodeWeb }));
                        }
                    }
                }
            }
        }

        public IRelayCommand<ChromiumWebBrowser> LoadPageCommand;

        private string _AddressWeb = "https://bccp.vnpost.vn/BCCP.aspx?act=Trace";
        private LoadWebChoose _LoadWebChoose = LoadWebChoose.None;
        private ChromiumWebBrowser _WebBrowser;
        bool isInitializeWeb = false;

        bool isInitilizeWeb = false;

        /// <summary>
        /// Chi load web 1 lan
        /// </summary>
        bool IsLoadedWeb = false;
    }
}
