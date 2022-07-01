using CefSharp;
using CefSharp.Wpf;
using ExcelDataReader;
using HtmlAgilityPack;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
            DefaultCommand = new RelayCommand(Default);

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
                }
                else if (m.Key == "XacNhanMH")
                {
                    _LoadWebChoose = LoadWebChoose.XacNhanMH;
                    LoadAddressDiNgoai(m.Content);
                }
                else if (m.Key == "LoadAddressDong")
                {
                    _LoadWebChoose = LoadWebChoose.DongChuyenThu;
                    LoadAddressDiNgoai(m.Content);
                }
                else if (m.Key == "ListAddress")
                {
                    WebBrowser.LoadUrl(m.Content);
                    IsLoadedWeb = false;
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
        public ICommand DefaultCommand { get; }


        void Default()
        {
            WebBrowser.LoadUrl(defaultWeb);
        }

        public string AddressWeb
        {
            get { return _AddressWeb; }
            set
            {
                SetProperty(ref _AddressWeb, value);
                WebBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
                WebBrowser.DownloadHandler = new MyDownloadHandler();
            }
        }


        public class MyDownloadHandler : IDownloadHandler
        {
            public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
            {
                return true;
            }

            public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
            {
                if (!callback.IsDisposed)
                {
                    using (callback)
                    {
                        callback.Continue(System.IO.Path.Combine(@"c:\downloadFolder", downloadItem.SuggestedFileName), showDialog: false);
                    }
                }
            }

            public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
            {
                if (downloadItem.IsValid)
                {
                    if (downloadItem.IsComplete)
                    {
                        GetListAddress(downloadItem.FullPath);
                    }
                }

            }
            void GetListAddress(string fullPath)
            {
                try
                {
                    //Send list address to Data;
                    //thuc hien doc du lieu tu file nao do
                    //WorkBook workBook = new WorkBook(fullPath);
                    //WorkSheet sheet = workBook.WorkSheets.First();

                    //string cellValue = sheet["B5"].StringValue;
                    using (var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        using (ExcelDataReader.IExcelDataReader reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                        {
                            System.Data.DataSet tables = reader.AsDataSet();
                            List<ChiTietTuiModel> chiTietTui = new List<ChiTietTuiModel>();
                            for (int i = 2; i < tables.Tables[0].Rows.Count; i++)
                            {
                                chiTietTui.Add(new ChiTietTuiModel(tables.Tables[0].Rows[i][1].ToString(), tables.Tables[0].Rows[i][3].ToString()));
                            }
                            //thuc hien send data tra ve
                            if (chiTietTui.Count != 0)
                            {
                                WeakReferenceMessenger.Default.Send(new ChiTietTuiMessage(chiTietTui));
                            }
                        }

                    }


                }
                catch (Exception ex)
                {

                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    APIManager.OpenNotePad(ex.Message + '\n' + "loi Line WebViewModel " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                    throw;
                }

            }
        }

        //thuc hien lay du lieu tu danh sach da co
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

        readonly string scriptLogin = @"
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
            try
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
                            SoundManager.playSound2(@"Number\tingting.wav");
                            isFirstLoginSuccess = false;
                            WebBrowser.LoadUrl("https://bccp.vnpost.vn/BCCP.aspx?act=Trace&id=" + currentMaHieu);
                            WebBrowser.ExecuteScriptAsync(scriptLogin);
                            IsLoadedWeb = false;
                        }
                        else
                        {
                            WebBrowser.ExecuteScriptAsync(scriptLogin);
                            //App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                            //{
                            //    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Navigation", Content = "Web" });
                            //});

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

                            CheckPageMPS(document);
                        }
                    }
                    else if (diachi.IndexOf("bccp.vnpost.vn/bccp.aspx?act=trace") != -1)
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
                        if (_LoadWebChoose == LoadWebChoose.DiNgoaiAddress || _LoadWebChoose == LoadWebChoose.AddressTamQuan || _LoadWebChoose == LoadWebChoose.AddressChuaPhat || _LoadWebChoose == LoadWebChoose.DongChuyenThu)
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
                            else if (_LoadWebChoose == LoadWebChoose.DiNgoaiAddress)
                                webContent.Key = "DongChuyenThu";
                            else if (_LoadWebChoose == LoadWebChoose.AddressTamQuan)
                                webContent.Key = "AddressTamQuan";
                            else if (_LoadWebChoose == LoadWebChoose.AddressChuaPhat)
                            {


                                HtmlNodeCollection tables = document.DocumentNode.SelectNodes("//table[@id='MainContent_ctl00_grvItemMailTrip']/tbody");

                                if (tables == null)
                                {
                                    APIManager.ShowSnackbar("Error");
                                    return;
                                }
                                HtmlNode table = tables.First();
                                HtmlNodeCollection aa = table.LastChild.PreviousSibling.SelectNodes("td");

                                webContent.KiemTraWeb = new KiemTraModel
                                {
                                    Date = aa[2].InnerText,
                                    BuuCucDong = aa[3].InnerText,
                                    BuuCucNhan = aa[4].InnerText,
                                    TTCT = aa[5].InnerText
                                };
                                webContent.Key = "AddressChuaPhat";
                            }
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
                        else if (_LoadWebChoose == LoadWebChoose.KiemTraWeb || _LoadWebChoose == LoadWebChoose.XacNhanMH)
                        {
                            KiemTraModel kiemTra = new KiemTraModel();

                            string barcode = document.DocumentNode.SelectNodes("//*[@id='MainContent_ctl00_lblBarcode']").First().InnerText;
                            if (string.IsNullOrEmpty(barcode))
                            {
                                return;
                            }
                            kiemTra.MaHieu = barcode.Substring(0, 13);
                            //thuc hien lay barcode

                            var addresss = document.DocumentNode.SelectNodes("//*[@id='MainContent_ctl00_lblReceiverAddr']");
                            if (addresss == null)
                            {
                                APIManager.ShowSnackbar("Error");
                                return;

                            }
                            kiemTra.Address = addresss.First().InnerText;

                            HtmlNodeCollection tables = document.DocumentNode.SelectNodes("//table[@id='MainContent_ctl00_grvItemMailTrip']/tbody");
                            if (tables == null)
                            {
                                return;
                            }
                            var table = tables.First();
                            HtmlNodeCollection aa = table.LastChild.PreviousSibling.SelectNodes("td");
                            kiemTra.Date = aa[2].InnerText;
                            kiemTra.BuuCucDong = aa[3].InnerText;
                            kiemTra.BuuCucNhan = aa[4].InnerText;
                            kiemTra.TTCT = aa[5].InnerText;

                            //thuc hien send thong tin qua kiem tra model
                            if (_LoadWebChoose == LoadWebChoose.KiemTraWeb)
                            {
                                kiemTra.Key = "KiemTraWeb";
                                WeakReferenceMessenger.Default.Send<KiemTraMessage>(new KiemTraMessage(kiemTra));
                            }
                            else if (_LoadWebChoose == LoadWebChoose.XacNhanMH)
                            {
                                kiemTra.Key = "XacNhanMH";
                                WeakReferenceMessenger.Default.Send<KiemTraMessage>(new KiemTraMessage(kiemTra));
                            }
                        }
                    }
                    else if (diachi.IndexOf("bccp.vnpost.vn/bccp.aspx?act=multitrace") != -1)
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
                        string script = @"
                     document.getElementById('MainContent_ctl00_btnExportV2').click();
";
                        WebBrowser.ExecuteScriptAsync(script);



                    }
                }
            }
            catch (System.Exception ex)
            {

                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "loi Line WebViewModel " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }

        }

        private void CheckPageMPS(HtmlDocument document)
        {
            IEnumerable<HtmlNode> pageHave = document.DocumentNode.Descendants("section").Where(d => d.Attributes["class"].Value.Contains("paging"));
            bool isHasPaging = document.DocumentNode.HasClass("paging");
            if (pageHave == null)
                return;
            HtmlNodeCollection childPage = document.DocumentNode.SelectNodes(@"//section[contains(@class,'paging')]/ul/li");
            //HtmlNodeCollection childPage = PagingClass.FirstChild.ChildNodes;
            for (int i = 0; i < childPage.Count; i++)
            {
                if (i != childPage.Count - 1)
                {
                    HtmlNode child = childPage[i];
                    if (child.InnerHtml.Contains("active"))
                    {
                        var child1 = childPage[i + 1];
                        string id = child1.SelectSingleNode("./input").Id;
                        string script = @"document.getElementById('" + id + "').click();";
                        IsLoadedWeb = false;
                        IsRunningChuaPhat = true;
                        WebBrowser.ExecuteScriptAsync(script);
                        break;
                    }





                }
            }

        }

        readonly string defaultWeb = "https://bccp.vnpost.vn/BCCP.aspx?act=Trace";

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