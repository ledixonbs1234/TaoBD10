using CefSharp;
using CefSharp.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExcelDataReader;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;
using System.Threading.Tasks;

namespace TaoBD10.ViewModels
{
    public class WebViewModel : ObservableObject
    {
        string keyPathCheckCode = "";
        public WebViewModel()
        {
            LoadPageCommand = new RelayCommand<ChromiumWebBrowser>(LoadPage);
            LoginCommand = new RelayCommand(Login);
            DefaultCommand = new RelayCommand(Default);
            FullCommand = new RelayCommand(Full);
            MinCommand = new RelayCommand(Min);
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
             {

                 if (m.Key == "LoadAddressWeb")
                 {
                     _LoadWebChoose = LoadWebChoose.DiNgoaiAddress;
                     LoadAddressDiNgoai(m.Content);
                 }
                 else if (m.Key == "WebInitializer")
                 {
                     WebBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
                     WebBrowser.DownloadHandler = new MyDownloadHandler();
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
                 else if (m.Key == "XacNhanMHCTDen")
                 {
                     _LoadWebChoose = LoadWebChoose.XacNhanMHCTDen;
                     LoadAddressDiNgoai(m.Content);
                 }
                 else if (m.Key == "ToWeb_CheckCode")
                 {
                     heapList.Add(m.Content);
                     if (isFirstLoginSuccess)
                     {
                         _LoadWebChoose = LoadWebChoose.CheckCode;
                         requestOnHeap();
                     }
                 }

                 else if (m.Key == "LoadAddressDong")
                 {
                     _LoadWebChoose = LoadWebChoose.DongChuyenThu;
                     LoadAddressDiNgoai(m.Content);
                 }
                 else if (m.Key == "ListAddress")
                 {
                     isClickWebBCCP = false;
                     APIManager.downLoadRoad = DownLoadRoad.XacNhanTui;
                     WebBrowser.LoadUrl(m.Content);
                 }
                 else if (m.Key == "ListAddressFull")
                 {
                     APIManager.downLoadRoad = DownLoadRoad.TamQuanAddress;
                     isClickWebBCCP = false;
                     WebBrowser.LoadUrl(m.Content);
                 }
                 else if (m.Key == "ListAddressChuyenThu")
                 {
                     APIManager.downLoadRoad = DownLoadRoad.ChuyenThuAddress;
                     isClickWebBCCP = false;
                     WebBrowser.LoadUrl(m.Content);
                 }
                 else if (m.Key == "ListAddressDiNgoai")
                 {
                     APIManager.downLoadRoad = DownLoadRoad.DiNgoai;
                     isClickWebBCCP = false;
                     WebBrowser.LoadUrl(m.Content);
                 }
                 else if (m.Key == "OnlyCheck")
                 {
                     APIManager.downLoadRoad = DownLoadRoad.None;
                     string[] split = m.Content.Split('|');
                     keyPathCheckCode = split[1];
                     WebBrowser.LoadUrl(split[0]);
                 }
                 else if (m.Key == "ShowFullWeb")
                 {
                     showFullWeb();
                 }
                 else if (m.Key == "CaptureScreen")
                 {
                     captureAndUpdateScreen();
                 }
                 else if (m.Key == "KTChuaPhat")
                 {
                     if (m.Content == "LoadUrl")
                     {
                         isCheckingChuaPhat = true;
                         WebBrowser.LoadUrl("https://mps.vnpost.vn/default.aspx");
                     }
                     else if (m.Content == "Run230")
                     {
                         //thuc hien trong nay
                         string script = @"
                                document.getElementById('ctl00_ctl12_rcb_tp_gui_ClientState').value='" + FileManager.optionModel.CodeBCMPSKT + @"';
                                document.getElementById('ctl00_ctl12_rcb_status_ClientState').value = '{""logEntries"":[],""value"":"""",""text"":""Xác nhận đến"",""enabled"":true,""checkedIndices"":[2],""checkedItemsTextOverflows"":false}';

                         document.getElementById('ctl00_ctl12_btn_submit').click();
                         ";
                         IsRunningChuaPhat = true;
                         WebBrowser.ExecuteScriptAsync(script);
                     }
                     else if (m.Content == "Run280")
                     {
                         //thuc hien trong nay
                         string script = @"
                                document.getElementById('ctl00_ctl12_rcb_tp_gui_ClientState').value='" + FileManager.optionModel.CodeBCMPSBCP + @"';
                                document.getElementById('ctl00_ctl12_rcb_status_ClientState').value='{""logEntries"":[],""value"":"""",""text"":""Xác nhận đến"",""enabled"":true,""checkedIndices"":[2],""checkedItemsTextOverflows"":false}';

                                document.getElementById('ctl00_ctl12_btn_submit').click();
                ";
                         IsRunningChuaPhat = true;
                         WebBrowser.ExecuteScriptAsync(script);
                     }
                 }
                 else if (m.Key == "SendMaHieuPNS")
                 {
                     PNSName = m.Content;
                     WebBrowser.LoadUrl("https://pns.vnpost.vn/");
                 }
             });
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 20, 0);
            timer.Start();
        }
        bool IsRunningCheck = false;
        List<string> heapList = new List<string>();


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
                        IsRunningChuaPhat = true;
                        WebBrowser.ExecuteScriptAsync(script);
                        break;
                    }
                }
            }
        }

        void requestOnHeap()
        {
            if (!IsRunningCheck)
            {
                if (heapList.Count == 0)
                    return;

                IsRunningCheck = true;
                string content = heapList[0];
                var splitText = content.Split('|');
                APIManager.ShowSnackbar(splitText[0] + " đang được xử lý");
                _LoadWebChoose = LoadWebChoose.CheckCode;
                keyPathCheckCode = splitText[1];
                LoadAddressDiNgoai(splitText[0]);
                heapList.RemoveAt(0);

            }
        }

        private void Default()
        {
            WebBrowser.LoadUrl(defaultWeb);
        }

        private void Full()
        {
            WeakReferenceMessenger.Default.Send(new ContentModel() { Key = "Window", Content = "Full" });
        }

        private void LoadAddressDiNgoai(string code)
        {
            currentMaHieu = code;
            WebBrowser.LoadUrl("https://bccp.vnpost.vn/BCCP.aspx?act=Trace&id=" + code);
        }

        private void LoadPage(ChromiumWebBrowser web)
        {
        }

        void showFullWeb()
        {
            App.Current.Dispatcher.Invoke(delegate // <--- HERE
            {
                Default();
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Navigation", Content = "Web" });
                Full();
                Thread.Sleep(500);
                FileManager.SendMessageNotification("Đã hiện full web");
                //string pathImage = captureScreen();
                //PublishToWeb(new FileInfo(pathImage));
                //WeakReferenceMessenger.Default.Send(new ContentModel() { Key = "NormalWindow", Content = "Full" });
            });
        }

        void captureAndUpdateScreen()
        {
            string pathImage = captureScreen();
            PublishToWeb(new FileInfo(pathImage));
                FileManager.SendMessageNotification("Đã upload screen thành công");
        }

        private void Login()
        {
            string script = @"
                     document.getElementById('MainContent_txtUser').value='" + FileManager.optionModel.AccountDinhVi + @"';
            		document.getElementById('MainContent_txtPassword').value='" + FileManager.optionModel.PWDinhVi + @"';
            		document.getElementById('MainContent_btnLogin').click();
";

            WebBrowser.ExecuteScriptAsync(script);
        }
        private void PublishToWeb(FileInfo file)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("bd10", "ledixonbs123");
                    client.UploadFile("ftp://files.000webhost.com/public_html/" + file.Name, WebRequestMethods.Ftp.UploadFile, file.FullName);
                }
            }
            catch (Exception ex)
            {
                FileManager.SendMessageNotification(ex.Message);

            }

        }

        private void Min()
        {
            WeakReferenceMessenger.Default.Send(new ContentModel() { Key = "Window", Content = "Min" });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Default();
        }

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
                        if (isFirstLoginSuccess)
                        {
                            //SoundManager.playSound2(@"Number\tingting.wav");
                            isFirstLoginSuccess = false;
                            WebBrowser.LoadUrl("https://bccp.vnpost.vn/BCCP.aspx?act=Trace&id=" + currentMaHieu);

                            isFix = true;
                        }
                        else
                        {
                            WebBrowser.ExecuteScriptAsync(TextManager.SCRIPT_LOGIN);

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

                            HtmlNodeCollection Items = document.DocumentNode.SelectNodes(@"//*[@id=""Content""]/section/section[1]/table/tbody/tr");
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
                    else if (diachi.IndexOf("pns.vnpost.vn/dang-nhap") != -1)
                    {
                        string script = @"
                     document.getElementById('userid').value='" + FileManager.optionModel.AccountPNS + @"';
            		document.getElementById('password').value='" + FileManager.optionModel.PWPNS + @"';
document.querySelector('body>div.content>div>div>div>div>form>fieldset>div:nth-child(2)>div>div:nth-child(7)>button').click();                    ";
                        WebBrowser.ExecuteScriptAsync(script);
                    }
                    else if (diachi == "https://pns.vnpost.vn/")
                    {
                        string script = @"
document.querySelector('#menu-3 > li:nth-child(10) > a').click();";
                        WebBrowser.ExecuteScriptAsync(script);
                    }
                    else if (diachi.IndexOf("pns.vnpost.vn/van-don") != -1)
                    {
                        if (!string.IsNullOrEmpty(PNSName))
                        {
                            string script = @"
                                document.getElementById('LadingCode').value='" + PNSName + @"';
                                document.getElementById('search_key').click();
                ";
                            PNSName = "";
                            WebBrowser.ExecuteScriptAsync(script);
                            APIManager.downLoadRoad = DownLoadRoad.GetName;
                            script = @"
setTimeout(function (){  document.getElementById('export_excel').click();}, 2000); ";
                            WebBrowser.ExecuteScriptAsync(script);
                        }
                    }
                    //
                    else if (diachi.IndexOf("bccp.vnpost.vn/bccp.aspx?act=trace") != -1)
                    {
                        if (isFix)
                        {
                            isFix = false;
                        }
                        isFirstLoginSuccess = true;
                        //if (isFix)
                        //{
                        //    isFix = false;
                        //    APIManager.ShowSnackbar("Da vao Fix");
                        //}
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
                                _LoadWebChoose = LoadWebChoose.None;
                                APIManager.ShowSnackbar("Không có mã hiệu web");
                                return;
                            }
                            string barcodeWeb = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblBarcode']").InnerText;
                            if (barcodeWeb.Length < 13)
                            {
                                _LoadWebChoose = LoadWebChoose.None;
                                APIManager.ShowSnackbar("Ma hiệu nhỏ hơn 13");
                                return;
                            }

                            barcodeWeb = barcodeWeb.Substring(0, 13).ToUpper();
                            //if (string.IsNullOrEmpty(barcodeWeb))
                            //{
                            //    WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Không đúng Code" });
                            //    _LoadWebChoose = LoadWebChoose.None;
                            //    return;
                            //}
                            WebContentModel webContent = new WebContentModel
                            {
                                Code = barcodeWeb
                            };

                            //kiem tra null
                            var dd = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblReceiverAddr']");
                            if (dd == null)
                            {
                                _LoadWebChoose = LoadWebChoose.None;
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
                                    _LoadWebChoose = LoadWebChoose.None;

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

                            _LoadWebChoose = LoadWebChoose.None;
                            //Thuc hien send Web content qua do
                            _ = WeakReferenceMessenger.Default.Send(webContent);
                        }
                        else if (_LoadWebChoose == LoadWebChoose.CheckCode)
                        {
                            ThongTinCoBanModel thongTinCoBan = new ThongTinCoBanModel();

                            HtmlNodeCollection noteBarcode = document.DocumentNode.SelectNodes("//*[@id='MainContent_ctl00_lblBarcode']");
                            String thongTinJson;
                            if (noteBarcode == null)
                            {
                                _LoadWebChoose = LoadWebChoose.None;
                                IsRunningCheck = false;
                                thongTinCoBan.State = 2;
                                thongTinCoBan.id = keyPathCheckCode;
                                thongTinJson = JsonConvert.SerializeObject(thongTinCoBan);
                                //thuc hien cong viec update value
                                FileManager.client.Child(FileManager.FirebaseKey + "/danhsachmahieu/" + keyPathCheckCode).PatchAsync(thongTinJson).Wait();

                                requestOnHeap();
                                return;
                            }

                            string barcode = noteBarcode.First().InnerText;
                            if (string.IsNullOrEmpty(barcode))
                            {
                                _LoadWebChoose = LoadWebChoose.None;
                                IsRunningCheck = false;
                                thongTinCoBan.State = 2;
                                thongTinCoBan.id = keyPathCheckCode;
                                thongTinJson = JsonConvert.SerializeObject(thongTinCoBan);
                                //thuc hien cong viec update value
                                FileManager.client.Child(FileManager.FirebaseKey + "/danhsachmahieu/" + keyPathCheckCode).PatchAsync(@"{""State"":2}").Wait();
                                requestOnHeap();
                                return;
                            }
                            thongTinCoBan.MaHieu = barcode.Substring(0, 13);
                            //thuc hien lay barcode

                            HtmlNode addresss = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblReceiverAddr']");
                            if (addresss == null)
                                thongTinCoBan.DiaChiNhan = "";
                            else
                                thongTinCoBan.DiaChiNhan = addresss.InnerText;

                            HtmlNode buuCucChapNhan = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblFrPOS']");
                            if (buuCucChapNhan == null)
                                thongTinCoBan.BuuCucChapNhan = "";
                            else
                                thongTinCoBan.BuuCucChapNhan = buuCucChapNhan.InnerText;
                            HtmlNode nguoiGui = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblSenderName']");
                            if (nguoiGui == null)
                                thongTinCoBan.NguoiGui = "";
                            else
                                thongTinCoBan.NguoiGui = nguoiGui.InnerText;
                            HtmlNode diaChiGui = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblSenderAddr']");
                            if (diaChiGui == null)
                                thongTinCoBan.DiaChiGui = "";
                            else
                                thongTinCoBan.DiaChiGui = diaChiGui.InnerText;
                            HtmlNode nguoiNhan = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblReceiverName']");
                            if (nguoiNhan == null)
                                thongTinCoBan.NguoiNhan = "";
                            else
                                thongTinCoBan.NguoiNhan = nguoiNhan.InnerText;
                            HtmlNode khoiLuongThucTe = document.DocumentNode.SelectSingleNode("//*[@id='MainContent_ctl00_lblWeightthucte']");
                            if (khoiLuongThucTe == null)
                                thongTinCoBan.KhoiLuongThucTe = "";
                            else
                                thongTinCoBan.KhoiLuongThucTe = khoiLuongThucTe.InnerText;


                            //Danh sach chuyen thu
                            HtmlNode table = document.DocumentNode.SelectSingleNode("//table[@id='MainContent_ctl00_grvItemMailTrip']/tbody");
                            List<ThongTinChuyenThuModel> thongTinCTs = new List<ThongTinChuyenThuModel>();
                            if (table == null)
                            {
                                thongTinCoBan.ThongTinChuyenThus = thongTinCTs;
                            }
                            else
                            {

                                if (table.HasChildNodes)
                                {
                                    for (int i = 1; i < table.ChildNodes.Count; i++)
                                    {
                                        HtmlNode item = table.ChildNodes[i];
                                        HtmlNodeCollection listtd = item.SelectNodes("td");
                                        if (listtd == null)
                                            break;
                                        if (listtd.Count() >= 5)
                                        {
                                            ThongTinChuyenThuModel chuyenThu = new ThongTinChuyenThuModel(listtd[1].InnerText, listtd[2].InnerText, listtd[3].InnerText, listtd[4].InnerText, listtd[5].InnerText);
                                            thongTinCTs.Add(chuyenThu);
                                        }
                                    }
                                    thongTinCoBan.ThongTinChuyenThus = thongTinCTs;
                                }
                            }

                            HtmlNodeCollection tablesChiTiet = document.DocumentNode.SelectNodes("//table[@id='MainContent_ctl00_grvItemTrace']/tbody");

                            List<ThongTinTrangThaiModel> list = new List<ThongTinTrangThaiModel>();
                            if (tablesChiTiet == null)
                            {
                                thongTinCoBan.ThongTinTrangThais = list;
                            }
                            else
                            {
                                var tableChiTiet = tablesChiTiet.First();
                                if (tableChiTiet.HasChildNodes)
                                {
                                    for (int i = 1; i < tableChiTiet.ChildNodes.Count; i++)
                                    {
                                        HtmlNode item = tableChiTiet.ChildNodes[i];
                                        HtmlNodeCollection listTd = item.SelectNodes("td");
                                        if (listTd == null)
                                            break;
                                        if (listTd.Count() >= 5)
                                        {
                                            list.Add(new ThongTinTrangThaiModel(listTd[1].InnerText, listTd[2].InnerText, listTd[3].InnerText, listTd[4].InnerText));
                                        }
                                    }
                                    thongTinCoBan.ThongTinTrangThais = list;
                                }
                            }
                            HtmlNode tablesBd = document.DocumentNode.SelectSingleNode("//*[@id=\"MainContent_ctl00_grvBD10\"]/tbody");

                            List<ThongTinGiaoNhanBDModel> listGiaoNhan = new List<ThongTinGiaoNhanBDModel>();
                            if (tablesBd == null)
                            {
                                thongTinCoBan.ThongTinGiaoNhanBDs = listGiaoNhan;
                            }
                            else
                            {
                                if (tablesBd.HasChildNodes)
                                {
                                    for (int i = 1; i < tablesBd.ChildNodes.Count; i++)
                                    {
                                        HtmlNode item = tablesBd.ChildNodes[i];
                                        HtmlNodeCollection listTd = item.SelectNodes("td");
                                        if (listTd == null)
                                            break;
                                        if (listTd.Count() >= 5)
                                        {
                                            string[] temps = listTd[5].InnerText.Split('/');
                                            listGiaoNhan.Add(new ThongTinGiaoNhanBDModel(listTd[1].InnerText, listTd[2].InnerText, listTd[3].InnerText, listTd[4].InnerText, temps[0].Trim(), temps[1].Trim()));
                                        }
                                    }
                                    thongTinCoBan.ThongTinGiaoNhanBDs = listGiaoNhan;
                                }

                            }
                            _LoadWebChoose = LoadWebChoose.None;
                            thongTinCoBan.State = 1;
                            thongTinCoBan.id = keyPathCheckCode;
                            thongTinJson = JsonConvert.SerializeObject(thongTinCoBan);
                            //thuc hien cong viec update value
                            FileManager.client.Child(FileManager.FirebaseKey + "/danhsachmahieu/" + keyPathCheckCode).PatchAsync(thongTinJson).Wait();

                            //Thuc hien send data to web
                            //MqttManager.Pulish("ledixon1_checkcode", thongTinJson);
                            IsRunningCheck = false;
                            requestOnHeap();
                        }
                        else if (_LoadWebChoose == LoadWebChoose.CodeFromBD)
                        {
                            Regex regex = new Regex(@"MainContent_ctl00_lblBarcode"">((\w|\W)+?)<");
                            var match = regex.Match(html);
                            string barcodeWeb = match.Groups[1].Value;
                            if (string.IsNullOrEmpty(barcodeWeb))
                            {
                                //txtInfo.Text = "Lỗi ! Chạy Lại";
                                _LoadWebChoose = LoadWebChoose.None;
                                return;
                            }
                            _LoadWebChoose = LoadWebChoose.None;

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
                        else if (_LoadWebChoose == LoadWebChoose.KiemTraWeb || _LoadWebChoose == LoadWebChoose.XacNhanMH || _LoadWebChoose == LoadWebChoose.XacNhanMHCTDen)
                        {
                            KiemTraModel kiemTra = new KiemTraModel();

                            HtmlNodeCollection noteBarcode = document.DocumentNode.SelectNodes("//*[@id='MainContent_ctl00_lblBarcode']");
                            if (noteBarcode == null)
                            {
                                _LoadWebChoose = LoadWebChoose.None;
                                return;
                            }

                            string barcode = noteBarcode.First().InnerText;
                            if (string.IsNullOrEmpty(barcode))
                            {
                                _LoadWebChoose = LoadWebChoose.None;
                                return;
                            }
                            kiemTra.MaHieu = barcode.Substring(0, 13);
                            //thuc hien lay barcode

                            var addresss = document.DocumentNode.SelectNodes("//*[@id='MainContent_ctl00_lblReceiverAddr']");
                            if (addresss == null)
                            {
                                _LoadWebChoose = LoadWebChoose.None;
                                APIManager.ShowSnackbar("Error");
                                return;
                            }
                            kiemTra.Address = addresss.First().InnerText;

                            HtmlNodeCollection tables = document.DocumentNode.SelectNodes("//table[@id='MainContent_ctl00_grvItemMailTrip']/tbody");
                            if (tables == null)
                            {
                                _LoadWebChoose = LoadWebChoose.None;
                                return;
                            }
                            var table = tables.First();
                            HtmlNodeCollection aa = table.LastChild.PreviousSibling.SelectNodes("td");
                            kiemTra.Date = aa[2].InnerText;
                            kiemTra.BuuCucDong = aa[3].InnerText;
                            kiemTra.BuuCucNhan = aa[4].InnerText;
                            kiemTra.TTCT = aa[5].InnerText;

                            HtmlNodeCollection tablesChiTiet = document.DocumentNode.SelectNodes("//table[@id='MainContent_ctl00_grvItemTrace']/tbody");
                            if (tablesChiTiet == null)
                            {
                                _LoadWebChoose = LoadWebChoose.None;
                                return;
                            }
                            var tableChiTiet = tablesChiTiet.First();
                            if (tableChiTiet.HasChildNodes)
                            {
                                List<ThongTinTrangThaiModel> list = new List<ThongTinTrangThaiModel>();
                                for (int i = 1; i < tableChiTiet.ChildNodes.Count; i++)
                                {
                                    HtmlNode item = tableChiTiet.ChildNodes[i];
                                    HtmlNodeCollection listTd = item.SelectNodes("td");
                                    if (listTd == null)
                                        break;
                                    if (listTd.Count() >= 5)
                                    {
                                        list.Add(new ThongTinTrangThaiModel(listTd[1].InnerText, listTd[2].InnerText, listTd[3].InnerText, listTd[4].InnerText));
                                    }
                                }
                                kiemTra.ThongTins = list;
                            }
                            //int count = tableChiTiet.ChildNodes;
                            //kiemTra.Date = aa[2].InnerText;
                            //kiemTra.BuuCucDong = aa[3].InnerText;
                            //kiemTra.BuuCucNhan = aa[4].InnerText;
                            //kiemTra.TTCT = aa[5].InnerText;

                            //thuc hien send thong tin qua kiem tra model
                            if (_LoadWebChoose == LoadWebChoose.KiemTraWeb)
                            {
                                kiemTra.Key = "KiemTraWeb";
                                WeakReferenceMessenger.Default.Send(new KiemTraMessage(kiemTra));
                            }
                            else if (_LoadWebChoose == LoadWebChoose.XacNhanMH)
                            {
                                kiemTra.Key = "XacNhanMH";
                                WeakReferenceMessenger.Default.Send(new KiemTraMessage(kiemTra));
                            }
                            else if (_LoadWebChoose == LoadWebChoose.XacNhanMHCTDen)
                            {
                                kiemTra.Key = "XacNhanMHCTDen";
                                WeakReferenceMessenger.Default.Send(new KiemTraMessage(kiemTra));
                            }
                            _LoadWebChoose = LoadWebChoose.None;
                        }
                        //_LoadWebChoose = LoadWebChoose.None;
                    }
                    else if (diachi.IndexOf("bccp.vnpost.vn/bccp.aspx?act=multitrace") != -1)
                    {
                        isFirstLoginSuccess = true;
                        //kiem tra dieu kien url
                        //string html = await WebBrowser.GetSourceAsync();
                        //HtmlDocument document = new HtmlDocument();
                        //document.LoadHtml(html);
                        //APIManager.downLoadRoad = DownLoadRoad.XacNhanTui;
                        string script = @"
                     document.getElementById('MainContent_ctl00_btnExportV2').click();
";
                        if (!isClickWebBCCP)
                        {
                            isClickWebBCCP = true;
                            WebBrowser.ExecuteScriptAsync(script);
                        }
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
        string captureScreen()
        {
            Bitmap captureBitmap = new Bitmap(1024, 768, PixelFormat.Format32bppArgb);
            //Bitmap captureBitmap = new Bitmap(int width, int height, PixelFormat);
            //Creating a Rectangle object which will
            //capture our Current Screen
            Rectangle captureRectangle = System.Windows.Forms.Screen.AllScreens[0].Bounds;
            //Creating a New Graphics Object
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);
            //Copying Image from The Screen
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            string path = Environment.CurrentDirectory + @"\Capture.jpg";
            //Saving the Image File (I am here Saving it in My E drive).
            captureBitmap.Save(path, ImageFormat.Jpeg);
            return path;
        }

        public IRelayCommand<ChromiumWebBrowser> LoadPageCommand;
        private readonly string defaultWeb = "https://bccp.vnpost.vn/BCCP.aspx?act=Trace";
        private string _AddressWeb = "https://bccp.vnpost.vn/BCCP.aspx?act=Trace";
        private bool _IsExpanded;
        private LoadWebChoose _LoadWebChoose = LoadWebChoose.None;
        private ChromiumWebBrowser _WebBrowser;
        private string currentMaHieu = "";
        private bool isCheckingChuaPhat = false;
        private bool isClickWebBCCP = false;
        private bool _isFirstLoginSuccess = false;

        public bool isFirstLoginSuccess
        {
            get { return _isFirstLoginSuccess; }
            set
            {
                if (value)
                {
                    requestOnHeap();
                }
                SetProperty(ref _isFirstLoginSuccess, value);
            }
        }


        private bool isFix = false;
        private bool IsRunningChuaPhat = false;
        private string PNSName = "";
        private DispatcherTimer timer;
        public string AddressWeb
        {
            get { return _AddressWeb; }
            set
            {
                SetProperty(ref _AddressWeb, value);
            }
        }

        public ICommand DefaultCommand { get; }

        public ICommand FullCommand { get; }

        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                SetProperty(ref _IsExpanded, value);
                if (_IsExpanded == false)
                {
                    Min();
                }
                else
                {
                    Full();
                }
            }
        }
        //thuc hien lay du lieu tu danh sach da co
        public ICommand LoginCommand { get; }

        public ICommand MinCommand { get; }
        public ChromiumWebBrowser WebBrowser
        {
            get { return _WebBrowser; }
            set { SetProperty(ref _WebBrowser, value); }
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
                        switch (APIManager.downLoadRoad)
                        {
                            case DownLoadRoad.None:
                                break;

                            case DownLoadRoad.XacNhanTui:
                                GetListAddress(downloadItem.FullPath, "XacNhanTui");
                                break;

                            case DownLoadRoad.GetName:
                                GetNames(downloadItem.FullPath);
                                break;

                            case DownLoadRoad.TamQuanAddress:
                                GetListAddress(downloadItem.FullPath, "TamQuanAddress");
                                break;

                            case DownLoadRoad.ChuyenThuAddress:
                                GetListAddress(downloadItem.FullPath, "ChuyenThuAddress");
                                break;

                            case DownLoadRoad.DiNgoai:
                                GetListAddress(downloadItem.FullPath, "DiNgoai");
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            private void GetListAddress(string fullPath, string key)
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
                        using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            System.Data.DataSet tables = reader.AsDataSet();
                            List<ChiTietTuiModel> chiTietTui = new List<ChiTietTuiModel>();
                            for (int i = 2; i < tables.Tables[0].Rows.Count; i++)
                            {
                                chiTietTui.Add(new ChiTietTuiModel(tables.Tables[0].Rows[i][1].ToString(), tables.Tables[0].Rows[i][3].ToString(), tables.Tables[0].Rows[i][4].ToString()));
                            }
                            //thuc hien send data tra ve
                            if (chiTietTui.Count != 0)
                            {
                                WeakReferenceMessenger.Default.Send(new ChiTietTuiMessage(new ChiTietTuiInfo() { ChiTietTuis = chiTietTui, Key = key }));
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

            private void GetNames(string fullPath)
            {
                try
                {
                    Workbook workbook = new Workbook();
                    workbook.LoadFromFile(fullPath);
                    workbook.SaveToFile(@"C:\\test.xlsx", ExcelVersion.Version2013);

                    using (var stream = File.Open(@"C:\\test.xlsx", FileMode.Open, FileAccess.Read))
                    {
                        using (ExcelDataReader.IExcelDataReader reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                        {
                            System.Data.DataSet tables = reader.AsDataSet();
                            List<PNSNameModel> chiTietTui = new List<PNSNameModel>();
                            for (int i = 1; i < tables.Tables[0].Rows.Count; i++)
                            {
                                chiTietTui.Add(new PNSNameModel(tables.Tables[0].Rows[i][1].ToString(), tables.Tables[0].Rows[i][10].ToString(), tables.Tables[0].Rows[i][12].ToString(), tables.Tables[0].Rows[i][11].ToString()));
                            }
                            //thuc hien send data tra ve
                            if (chiTietTui.Count != 0)
                            {
                                WeakReferenceMessenger.Default.Send(new PNSNameMessage(chiTietTui));
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
    }
}