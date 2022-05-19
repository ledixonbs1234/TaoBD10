﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class LayChuyenThuViewModel : ObservableObject
    {

        BackgroundWorker bwLayCT;
        public LayChuyenThuViewModel()
        {
            bwLayCT = new BackgroundWorker();
            bwLayCT.DoWork += BwLayCT_DoWork;
            BongSonCommand = new RelayCommand(BongSon);
            HoaiXuanCommand = new RelayCommand(HoaiXuan);
            HoaiTanCommand = new RelayCommand(HoaiTan);
            HoaiDucCommand = new RelayCommand(HoaiDuc);
            GiaoDichCommand = new RelayCommand(GiaoDich);
            HoaiHuongCommand = new RelayCommand(HoaiHuong);
            HoaiHaiCommand = new RelayCommand(HoaiHai);
            BCPCommand = new RelayCommand(BCP);
            HoaiMyCommand = new RelayCommand(HoaiMy);
        }

        private void BwLayCT_DoWork(object sender, DoWorkEventArgs e)
        {

            WindowInfo window = APIManager.WaitingFindedWindow("quan ly chuyen thu");
            if (window == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy quản lý chuyến thư");
                return;
            }

            List<TestAPIModel> controls = APIManager.GetListControlText(window.hwnd);
            if (controls.Count == 0)
            {
                APIManager.ShowSnackbar("Window lỗi");
                return;
            }
            List<TestAPIModel> listCombobox = controls.Where(m => m.ClassName.ToLower().IndexOf("combobox") != -1).ToList();
            IntPtr comboHandle = listCombobox[3].Handle;

            APIManager.SendMessage(comboHandle, 0x0007, 0, 0);
            APIManager.SendMessage(comboHandle, 0x0007, 0, 0);

            ChuyenThuDen();
            Thread.Sleep(200);
            SendKeys.SendWait("{ENTER}");

            window = APIManager.WaitingFindedWindow("canh bao", "thong bao");
            if (window == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy cảnh báo");
                return;
            }

            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(500);
            SendKeys.SendWait("{F5}");

        }

        public ICommand BCPCommand { get; }
        public ICommand BongSonCommand { get; }
        public ICommand GiaoDichCommand { get; }
        public ICommand HoaiDucCommand { get; }
        public ICommand HoaiHaiCommand { get; }
        public ICommand HoaiHuongCommand { get; }
        public ICommand HoaiMyCommand { get; }
        public ICommand HoaiTanCommand { get; }
        public ICommand HoaiXuanCommand { get; }

        public void Btn593200()
        {
            maBuuCucChuyenThuDen = "593200";
            isBaoDamChuyenThuDen = false;

            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void BCP()
        {
            maBuuCucChuyenThuDen = "593280";
            isBaoDamChuyenThuDen = false;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void BongSon()
        {
            maBuuCucChuyenThuDen = "593522";
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void ChuyenThuDen()
        {
            if (isBaoDamChuyenThuDen)
            {
                SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                //buu pham bao dam
                SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
            }
            else
            {
                SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                //buu pham bao dam
                SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
            }
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(maBuuCucChuyenThuDen);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{F8}");
        }

        private void GiaoDich()
        {
            maBuuCucChuyenThuDen = "593200";
            isBaoDamChuyenThuDen = false;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void HoaiDuc()
        {
            maBuuCucChuyenThuDen = "593550";
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void HoaiHai()
        {
            maBuuCucChuyenThuDen = "593260";
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void HoaiHuong()
        {
            maBuuCucChuyenThuDen = "593270";
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void HoaiMy()
        {
            maBuuCucChuyenThuDen = "593240";
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void HoaiTan()
        {
            maBuuCucChuyenThuDen = "593370";
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void HoaiXuan()
        {
            maBuuCucChuyenThuDen = "593220";
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }
        private bool isBaoDamChuyenThuDen = false;
        private bool isWaitingChuyenThuChieuDen = false;
        private string maBuuCucChuyenThuDen = "";
    }
}