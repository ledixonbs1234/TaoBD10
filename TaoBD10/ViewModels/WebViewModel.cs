﻿using CefSharp;
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
        public IRelayCommand<ChromiumWebBrowser> LoadPageCommand;

        private IWpfWebBrowser _WebBrowser;
        private LoadWebChoose _LoadWebChoose = LoadWebChoose.None;

        public IWpfWebBrowser WebBrowser
        {
            get { return _WebBrowser; }
            set { SetProperty(ref _WebBrowser, value); }
        }

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
            });


        }


        void LoadAddressDiNgoai(string code)
        {
            if (WebBrowser.IsBrowserInitialized)
            {

            }
            else
            {
                //thuc hien navigate
                return;
            }
            string script = @"
                                document.getElementById('MainContent_ctl00_txtID').value='" + code + @"';
                				document.getElementById('MainContent_ctl00_btnView').click();
                ";
            WebBrowser.ExecuteScriptAsync(script);
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
            set
            {
                SetProperty(ref _AddressWeb, value);
                if (!isInitializeWeb)
                {
                    WebBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
                }
            }
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
                    //kiem tra dieu kien url
                    string html = await WebBrowser.GetSourceAsync();

                    //kiem tra thu co no khong
                    if (_LoadWebChoose == LoadWebChoose.DiNgoaiAddress)
                    {
                        Regex regex = new Regex(@"MainContent_ctl00_lblBarcode"">((\w|\W)+?)<");
                        var match = regex.Match(html);
                        string barcodeWeb = match.Groups[1].Value;
                        if (string.IsNullOrEmpty(barcodeWeb))
                        {
                            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Không đúng Code" });
                            return;
                        }

                        if (barcodeWeb.ToLower().IndexOf(diNgoaiViewModel.diNgoais[iCurrentItemDiNgoai].Code.ToLower()) == -1)
                        {
                            txtInfo.Text = "Không trùng mã";
                            return;
                        }

                        Regex regexMaTinh = new Regex(@"MainContent_ctl00_lblDesPOS"">((\w|\W)+?) ");
                        var matchMaTinh = regexMaTinh.Match(html);
                        numberMaTinh = 0;
                        int.TryParse(matchMaTinh.Groups[1].Value, out numberMaTinh);
                        if (numberMaTinh == 0)
                        {
                            lblMaTInh.Text = "Lỗi";
                            txtInfo.Text = "Lỗi";
                            return;
                        }
                        else
                        {
                            Regex regex1 = new Regex(@"MainContent_ctl00_lblReceiverAddr(\W|\w)+?>((\W|\w)+?)<");
                            var mat = regex1.Match(html);
                            string data = mat.Groups[2].Value;
                            if (string.IsNullOrEmpty(data))
                            {
                                lblMaTInh.Text = "Lỗi";
                                txtInfo.Text = "Lỗi";
                                return;
                            }
                            diNgoaiViewModel.diNgoais[iCurrentItemDiNgoai].Address = data;
                            diNgoaiViewModel.diNgoais[iCurrentItemDiNgoai].MaTinh = numberMaTinh.ToString();

                            dgvAddress.Refresh();
                            BtnAddAddress_Click(null, null);

                        }

                    }
                }
            }

        public ICommand LoginCommand { get; }


        void LoadPage(ChromiumWebBrowser web)
        {
        }
    }
}
