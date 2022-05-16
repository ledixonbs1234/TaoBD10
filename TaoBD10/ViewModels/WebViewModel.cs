using CefSharp;
using CefSharp.Wpf;
using HtmlAgilityPack;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class WebViewModel : ObservableObject
    {

        string currentMaHieu = "";
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
                }
                else if (m.Key == "LoadAddressTQWeb")
                {
                    _LoadWebChoose = LoadWebChoose.AddressTamQuan;
                    LoadAddressDiNgoai(m.Content);
                }
                else if (m.Key == "LoadAddressChuaPhat")
                {
                    _LoadWebChoose = LoadWebChoose.AddressChuaPhat;
                    LoadAddressDiNgoai(m.Content);
                }
                else if (m.Key == "GetCodeFromBD")
                {
                    _LoadWebChoose = LoadWebChoose.CodeFromBD;
                    LoadAddressDiNgoai(m.Content);
                }
                else if (m.Key == "KiemTraWeb")
                {
                    _LoadWebChoose = LoadWebChoose.KiemTraWeb;
                    LoadAddressDiNgoai(m.Content);
                }else if (m.Key == "LoadAddressDong")
                {

                    _LoadWebChoose = LoadWebChoose.DongChuyenThu;
                    LoadAddressDiNgoai(m.Content);
                }
                else if (m.Key == "KTChuaPhat")
                {
                    if (m.Content == "LoadUrl")
                    {
                        isCheckingChuaPhat = true;
                        IsLoadedWeb = false;
                        WebBrowser.LoadUrl("https://mps.vnpost.vn/default.aspx");
                    }
                    else if (m.Content == "Run230")
                    {
                        //thuc hien trong nay
                        string script = @"
                                document.getElementById('ctl00_ctl12_rcb_Donvi_ClientState').value='{""logEntries"":[],""value"":""593230"",""text"":""----593230 - KT Hoài Nhơn -KT2"",""enabled"":true,""checkedIndices"":[],""checkedItemsTextOverflows"":false}';
                                document.getElementById('ctl00_ctl12_rcbTrangThai_ClientState').value='{""logEntries"":[],""value"":""3"",""text"":""Xác nhận đến"",""enabled"":true,""checkedIndices"":[],""checkedItemsTextOverflows"":false}';

                                document.getElementById('ctl00_ctl12_btn_submit').click();
                ";
                        IsLoadedWeb = false;
                        IsRunningChuaPhat = true;
                        WebBrowser.ExecuteScriptAsync(script);
                    }
                    else if (m.Content == "Run280")
                    {
                        //thuc hien trong nay
                        string script = @"
                                document.getElementById('ctl00_ctl12_rcb_Donvi_ClientState').value='{""logEntries"":[],""value"":""593280"",""text"":""----593280 - BCP Hoài Nhơn -PH2"",""enabled"":true,""checkedIndices"":[],""checkedItemsTextOverflows"":false}';
                                document.getElementById('ctl00_ctl12_rcbTrangThai_ClientState').value='{""logEntries"":[],""value"":""3"",""text"":""Xác nhận đến"",""enabled"":true,""checkedIndices"":[],""checkedItemsTextOverflows"":false}';

                                document.getElementById('ctl00_ctl12_btn_submit').click();
                ";
                        IsLoadedWeb = false;
                        IsRunningChuaPhat = true;
                        WebBrowser.ExecuteScriptAsync(script);
                    }
                }
            });
        }

        public string AddressWeb
        {
            get { return _AddressWeb; }
            set
            {
                SetProperty(ref _AddressWeb, value);
                WebBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
            }
        }

        public ICommand LoginCommand { get; }
        public ChromiumWebBrowser WebBrowser
        {
            get { return _WebBrowser; }
            set { SetProperty(ref _WebBrowser, value); }
        }

        private void LoadAddressDiNgoai(string code)
        {
            currentMaHieu = code;
            WebBrowser.LoadUrl("https://bccp.vnpost.vn/BCCP.aspx?act=Trace&id=" + code);
            IsLoadedWeb = false;
        }

        private void LoadPage(ChromiumWebBrowser web)
        {
        }

        private void Login()
        {
            string script = @"
                     document.getElementById('MainContent_txtUser').value='593280';
            		document.getElementById('MainContent_txtPassword').value='593280';
            		document.getElementById('MainContent_btnLogin').click();
";

            WebBrowser.ExecuteScriptAsync(script);
        }
        string scriptLogin = @"
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

        bool isFirstLoginSuccess = false;

        private async void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                //Da tai xong
                string diachi = AddressWeb.ToLower();

                if (diachi.IndexOf("bccp.vnpost.vn/login") != -1)
                {
                    if (!IsLoadedWeb)
                    {
                        IsLoadedWeb = true;
                    }
                    else
                        return;
                    if (isFirstLoginSuccess)
                    {
                        WebBrowser.BackCommand.Execute(null);
                        WebBrowser.LoadUrl("https://bccp.vnpost.vn/BCCP.aspx?act=Trace&id=" + currentMaHieu);
                    }
                    else
                    {
                        WebBrowser.ExecuteScriptAsync(scriptLogin);
                    }

                }
                else if (diachi.IndexOf("mps.vnpost.vn/login") != -1)
                {
                    //thuc hien dang nhap vao trang web
                    string script = @"
                     document.getElementById('tx_tname').value='bdh.005932';
            		document.getElementById('txt_password').value='abc.123';
            		document.getElementById('btx_login').click();
";

                    WebBrowser.ExecuteScriptAsync(script);
                }
                else if (diachi.IndexOf("mps.vnpost.vn/default") != -1)
                {
                    if (isCheckingChuaPhat)
                    {
                        isCheckingChuaPhat = false;
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "RChuaPhat", Content = "InfoOK" });
                    }
                    else
                    {
                        //thuc hien get content trong nay
                        if (!IsLoadedWeb)
                        {
                            IsLoadedWeb = true;
                        }
                        else
                            return;

                        if (!IsRunningChuaPhat)
                        {
                            return;
                        }
                        IsRunningChuaPhat = false;
                        string html = await WebBrowser.GetSourceAsync();
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(html);

                        //thuc hien xu ly du lieu trong nay
                        ////*[@id="root"]/section/section/section[1]/table/tbody/tr[1]/td[3]

                        HtmlNodeCollection Items = document.DocumentNode.SelectNodes(@"//*[@id=""root""]/section/section/section[1]/table/tbody/tr");
                        if (Items == null)
                        {
                            APIManager.ShowSnackbar("Lỗi NULL");
                            return;
                        }
                        List<HangTonModel> hangTons = new List<HangTonModel>();
                        foreach (HtmlNode item in Items)
                        {
                            HangTonModel hangTon = new HangTonModel();
                            HtmlNodeCollection data = item.SelectNodes("td");
                            hangTon.Index = data[0].InnerText.Trim();

                            hangTon.MaHieu = data[2].InnerText.Trim().ToUpper();
                            hangTon.TienThuHo = data[3].InnerText.Trim();
                            hangTon.TimeGui = data[5].InnerText.Trim();
                            hangTon.TimeCapNhat = data[6].InnerText.Trim();
                            hangTon.BuuCucLuuGiu = data[7].InnerText.Trim();
                            hangTon.ChuyenHoan = data[8].InnerText.Trim();
                            hangTon.KhoiLuong = data[4].InnerText.Trim();
                            hangTon.BuuCucPhatHanh = data[11].InnerText.Trim();
                            hangTons.Add(hangTon);
                        }
                        //thuc hien send Du Lieu sang Map
                        WeakReferenceMessenger.Default.Send<HangTonMessage>(new HangTonMessage(hangTons));
                    }
                }
                else if (diachi.IndexOf("bccp.vnpost.vn/bccp") != -1)
                {
                    if (!IsLoadedWeb)
                    {
                        IsLoadedWeb = true;
                    }
                    else
                        return;
                    isFirstLoginSuccess = true;
                    //kiem tra dieu kien url
                    string html = await WebBrowser.GetSourceAsync();
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(html);

                    //kiem tra thu co no khong
                    if (_LoadWebChoose == LoadWebChoose.DiNgoaiAddress || _LoadWebChoose == LoadWebChoose.AddressTamQuan || _LoadWebChoose == LoadWebChoose.AddressChuaPhat)
                    {
                        var check = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblBarcode']");
                        if (check == null)
                        {
                            APIManager.ShowSnackbar("Lỗi Web");
                            return;
                        }
                        string barcodeWeb = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblBarcode']").InnerText;

                        barcodeWeb = barcodeWeb.Substring(0, 13).ToUpper();
                        if (string.IsNullOrEmpty(barcodeWeb))
                        {
                            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Không đúng Code" });
                            return;
                        }
                        WebContentModel webContent = new WebContentModel
                        {
                            Code = barcodeWeb
                        };

                        //kiem tra null
                        var dd = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblReceiverAddr']");
                        if (dd == null)
                        {
                            return;
                        }

                        webContent.AddressReiceive = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblReceiverAddr']").InnerText;
                        if (string.IsNullOrEmpty(webContent.AddressReiceive))
                        {
                            webContent.AddressReiceive = "";
                        }
                        webContent.AddressSend = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblSenderAddr']").InnerText;
                        var ff = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblDesPOS']").InnerText;
                        if (!string.IsNullOrEmpty(ff))
                        {
                            webContent.BuuCucPhat = ff.Substring(0, 2);
                        }
                        webContent.BuuCucGui = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblFrPOS']").InnerText;
                        webContent.NguoiGui = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblSenderName']").InnerText;
                        if (_LoadWebChoose == LoadWebChoose.DiNgoaiAddress)
                            webContent.Key = "DiNgoaiAddress";
                        else if (_LoadWebChoose == LoadWebChoose.AddressTamQuan)
                            webContent.Key = "AddressTamQuan";
                        else if (_LoadWebChoose == LoadWebChoose.AddressChuaPhat)
                            webContent.Key = "AddressChuaPhat";
                        else if (_LoadWebChoose == LoadWebChoose.DongChuyenThu)
                        {
                            webContent.Key = "AddressDongChuyenThu";
                        }
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
                    else if (_LoadWebChoose == LoadWebChoose.KiemTraWeb)
                    {
                        KiemTraModel kiemTra = new KiemTraModel();

                        string barcode = document.DocumentNode.SelectNodes("//*[@id='MainContent_ctl00_lblBarcode']").First().InnerText;
                        if (string.IsNullOrEmpty(barcode))
                        {
                            return;
                        }
                        kiemTra.MaHieu = barcode.Substring(0, 13);
                        //thuc hien lay barcode

                        kiemTra.Address = document.DocumentNode.SelectNodes("//*[@id='MainContent_ctl00_lblReceiverAddr']").First().InnerText;

                        HtmlNode table = document.DocumentNode.SelectNodes("//table[@id='MainContent_ctl00_grvItemMailTrip']/tbody").First();
                        HtmlNodeCollection aa = table.LastChild.PreviousSibling.SelectNodes("td");
                        kiemTra.Date = aa[2].InnerText;
                        kiemTra.BuuCucDong = aa[3].InnerText;
                        kiemTra.BuuCucNhan = aa[4].InnerText;
                        kiemTra.TTCT = aa[5].InnerText;

                        //thuc hien send thong tin qua kiem tra model
                        WeakReferenceMessenger.Default.Send<KiemTraMessage>(new KiemTraMessage(kiemTra));
                    }
                }
            }
        }

        public IRelayCommand<ChromiumWebBrowser> LoadPageCommand;
        private string _AddressWeb = "https://bccp.vnpost.vn/BCCP.aspx?act=Trace";
        private LoadWebChoose _LoadWebChoose = LoadWebChoose.None;
        private ChromiumWebBrowser _WebBrowser;
        private bool isCheckingChuaPhat = false;
        /// <summary>
        /// Chi load web 1 lan
        /// </summary>
        private bool IsLoadedWeb = false;

        private bool IsRunningChuaPhat = false;
    }
}