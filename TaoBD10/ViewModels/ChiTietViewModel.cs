﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    public class ChiTietViewModel : ObservableObject
    {
        public ChiTietViewModel()
        {
            ConLai = new LocBDInfoModel();
            ConLai.TenBD = "Còn Lại";
            bwChiTiet = new BackgroundWorker();
            bwChiTiet.DoWork += BwChiTiet_DoWork;
            bwChiTiet.RunWorkerCompleted += BwChiTiet_RunWorkerCompleted;
            taoBDWorker = new BackgroundWorker();
            taoBDWorker.DoWork += TaoBDWorker_DoWork;
            bwLayMaHieu = new BackgroundWorker();
            bwLayMaHieu.DoWork += BwLayMaHieu_DoWork;
            bwLayMaHieu.WorkerSupportsCancellation = true;
            bwLayMaHieu.RunWorkerCompleted += BwLayMaHieu_RunWorkerCompleted;
            LocBDs = new ObservableCollection<LocBDInfoModel>();
            AddBDTinhCommand = new RelayCommand<string>(AddBDTinh);
            ShowTinhs = new ObservableCollection<TinhHuyenModel>();
            TuDongXacNhanCTCommand = new RelayCommand(TuDongXacNhanCT);
            UpdateBuuCucChuyenThuCommand = new RelayCommand(UpdateBuuCucChuyenThu);
            SaveLocBDCommand = new RelayCommand(SaveLocBD);
            XuongLocCommand = new RelayCommand(XuongLoc);
            LenLocCommand = new RelayCommand(LenLoc);
            SaveTinhToSelectedLocBDCommand = new RelayCommand(SaveTinhToSelectedLocBD);
            TuDongXuLyCTCommand = new RelayCommand(TuDongXuLyCT);
            DeleteTinhCommand = new RelayCommand(DeleteTinh);
            HiddenCommand = new RelayCommand(Hidden);
            GetDataFromCloudCommand = new RelayCommand(GetDataFromCloud);
            PublishCommand = new RelayCommand(Publish);
            XeXaHoiCommand = new RelayCommand(XeXaHoi);
            SelectedTinhCommand = new RelayCommand<string>(SelectedTinh);
            LayCodeFromSHTuiCommand = new RelayCommand(LayCodeFromSHTui);

            SelectionCommand = new RelayCommand<HangHoaDetailModel>(ChonCT);

            CopySHTuiCommand = new RelayCommand(CopySHTui, () =>
            {
                if (SelectedTui != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            LoadLoc();

            WeakReferenceMessenger.Default.Register<BD10Message>(this, (r, m) =>
            {
                //Thuc Hien Trong ngay
                if (m.Value != null)
                {
                    currentListHangHoa = new List<HangHoaDetailModel>();
                    ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();
                    foreach (var BD10 in m.Value)
                    {
                        foreach (TuiHangHoa tuiHangHoa in BD10.TuiHangHoas)
                        {
                            currentListHangHoa.Add(new HangHoaDetailModel(tuiHangHoa));
                        }
                    }
                    //FillData();
                    FillLocBD();
                    //ResetAndCount();

                    UpdateEnableButtonLoc();
                    //ShowTest();
                }
            });
            WeakReferenceMessenger.Default.Register<ChuyenTamQuanMessage>(this, (r, m) =>
            {
                //Thu\c Hien Trong ngay
                if (m.Value != null)
                {
                    if (LocBCP == null && LocKhaiThac == null)
                        return;
                    foreach (var maHangHoa in m.Value)
                    {
                        var haveItem = LocBCP.HangHoas.FirstOrDefault(a => maHangHoa.TuiHangHoa.SHTui.ToUpper() == a.TuiHangHoa.SHTui.ToUpper());
                        if (haveItem != null)
                        {
                            haveItem.IsTamQuan = "TamQuan";
                            haveItem.TrangThaiBD = TrangThaiBD.TamQuan;
                        }
                        var haveItem1 = LocKhaiThac.HangHoas.FirstOrDefault(a => maHangHoa.TuiHangHoa.SHTui.ToUpper() == a.TuiHangHoa.SHTui.ToUpper());
                        if (haveItem1 != null)
                        {
                            haveItem1.IsTamQuan = "TamQuan";
                            haveItem1.TrangThaiBD = TrangThaiBD.TamQuan;
                        }
                    }
                }
            });

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Navigation" && m.Content == "PrintDiNgoai")
                {
                    PrintDiNgoai();
                }
                else if (m.Key == "LayHangHoa")
                {
                    //thuc hien lay hang hoa trong nay
                    if (currentListHangHoa == null)
                        return;
                    List<HangHoaDetailModel> tempData = new List<HangHoaDetailModel>();
                    tempData.AddRange(LocKhaiThac.HangHoas);
                    tempData.AddRange(LocBCP.HangHoas);
                    if (tempData == null)
                        return;
                    List<HangHoaDetailModel> data = new List<HangHoaDetailModel>();

                    foreach (HangHoaDetailModel hangHoa in tempData)
                    {
                        string temp = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.DichVu).ToLower();
                        string temp1 = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower();
                        if (temp.IndexOf("phat hanh") != -1 || temp1.IndexOf("tui") != -1)
                        {
                            continue;
                        }
                        data.Add(hangHoa);
                    }

                    WeakReferenceMessenger.Default.Send<TuiHangHoaMessage>(new TuiHangHoaMessage(data));
                }
                else if (m.Key == "GuiTrucTiep")
                {
                    GuiTrucTiep();
                }
            });

            timerTaoBD = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 200)
            };
            timerTaoBD.Tick += TimerTaoBD_Tick;

            var listTemp = FileManager.LoadTinhThanhOffline();
            if (listTemp != null)
            {
                ShowTinhs = new ObservableCollection<TinhHuyenModel>();
                foreach (TinhHuyenModel item in listTemp)
                {
                    ShowTinhs.Add(item);
                }
            }

            //Lay Du Lieu LocBD10
            var listLoc = FileManager.LoadLocBD10sOffline();
            if (listLoc != null)
            {
                LocBDs = new ObservableCollection<LocBDInfoModel>();
                foreach (LocBDInfoModel item in listLoc)
                {
                    if (item.TaoBDs.Count == 0)
                    {
                        item.TaoBDs.Add(new TaoBdInfoModel());
                    }
                    LocBDs.Add(item);
                }
            }

            WeakReferenceMessenger.Default.Register<ChiTietTuiMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "ChuyenThuAddress")
                    {
                        List<ChiTietTuiModel> chiTietTuis = m.Value.ChiTietTuis;

                        foreach (ChiTietTuiModel chiTietTui in chiTietTuis)
                        {
                            var HaveHangHoa = ListShowHangHoa.FirstOrDefault(s => s.TuiHangHoa.SHTui.ToUpper() == chiTietTui.MaHieu.ToUpper());
                            if (HaveHangHoa != null)
                            {
                                HaveHangHoa.Address = chiTietTui.Address.Trim();
                                HaveHangHoa.BuuCucChapNhan = chiTietTui.BCChapNhan;
                            }
                        }
                    }
                }
            }
          );

            //thuc hien viec xoa Thong Tin tu Tinh Thanh;
        }

        private void AddBDTinh(string Name)
        {
            if (string.IsNullOrEmpty(Name))
                return;
            LocBDInfoModel loc = LocBDs.FirstOrDefault(m => m.TenBD == Name);
            if (loc == null) return;
            _taoBDAdd = loc.TaoBDs[0];
            if (string.IsNullOrEmpty(_taoBDAdd.DuongThu)) return;

            taoBDWorker.RunWorkerAsync();
        }

        private void BwChiTiet_DoWork(object sender, DoWorkEventArgs e)
        {
            var window = VaoChiTietChuyenThu(CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui);
            if (window == null)
                return;
            if (window.text.IndexOf("xac nhan chi tiet tui thu") != -1)
            {
                if (CurrentSelectedHangHoaDetail.TrangThaiBD == TrangThaiBD.TamQuan)
                {
                    APIManager.ClickButton(window.hwnd, "Đối kiểm", isExactly: true);
                    Thread.Sleep(500);
                }
                //kiemtra thu cho nay co sh tui la bao nhieu neu vn thi lay dia chi
                //de ra phan xem chuyen thu chieu den thi in ra luon
                //con neu khong co lam cach nao do de lay duoc cai ma hieu va in ra
                //if (CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui.Length == 13)
                //{
                //    //thuc hien lay dia chi cho nay
                //    currentMH = CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui;
                //}♦
                //else
                //{
                //    SendKeys.SendWait("{TAB}{TAB}");
                //    Thread.Sleep(50);

                //    string copyed = APIManager.GetCopyData();
                //    if (copyed != null)
                //    {
                //        string[] enterText = copyed.Split('\n');
                //        if (enterText.Length == 2)
                //        {
                //            copyed = enterText[1];
                //        }
                //        string[] data = copyed.Split('\t');

                //        currentMH = data[1];
                //    }
                //}

                //if (currentMH.Length == 13)
                //{
                //    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanMHCTDen", Content = currentMH });
                //}
                SendKeys.SendWait("{ESC}");
            }
        }

        private void BwChiTiet_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //thuc hien cong viec trong nay
            if (!IsTuDongXuLy)
                return;
            var window = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
            if (window == null)
                return;

            bool daTimThay = checkDanhSachTrongDongCT(CurrentSelectedHangHoaDetail);
            if (!daTimThay)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }

            //bool daTimThay = checkDanhSachTrongDongCT(CurrentSelectedHangHoaDetail);
            //if (daTimThay)
            //{
            //    APIManager.ShowSnackbar("da tim thay");
            //}
            //else
            //{
            //    APIManager.ShowSnackbar("Khong tim thay");
            //}
            //return;

            TamQuanModel maHieu = layMaHieuTrongDongCT();
            if (maHieu == null)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }
            if (!string.IsNullOrEmpty(CurrentSelectedHangHoaDetail.Code))
            {
                if (maHieu.MaHieu.ToUpper() != CurrentSelectedHangHoaDetail.Code.ToUpper())
                {
                    SoundManager.playSound3(@"Number\error_sound.wav");
                    return;
                }
            }
            else
            if (maHieu.MaHieu != CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }

            if (CurrentSelectedHangHoaDetail.TrangThaiBD == TrangThaiBD.TamQuan)
            {
                APIManager.ShowSnackbar("Tam Quan");
                //thuc hien chuyen ma qua tam quan
                // co 2 loai nen lam thu
                WeakReferenceMessenger.Default.Send(new ChuyenTamQuanMHMessage(maHieu));
                Thread.Sleep(200);
                if (ChuyenThuTiepTheo())
                    TuDongXuLyCT();
            }
            else
            {
                //thuc hien nhan nut de in
                SendKeys.SendWait("{F9}");
                PrintDiNgoai();
            }
        }

        private void BwLayMaHieu_DoWork(object sender, DoWorkEventArgs e)
        {
            var window = VaoChiTietChuyenThu(CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui);
            string currentMH = "";
            if (window == null)
            {
                e.Cancel = true;
                return;
            }
            if (window.text.IndexOf("xac nhan chi tiet tui thu") != -1)
            {
                //kiemtra thu cho nay co sh tui la bao nhieu neu vn thi lay dia chi
                //de ra phan xem chuyen thu chieu den thi in ra luon
                //con neu khong co lam cach nao do de lay duoc cai ma hieu va in ra
                //if (CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui.Length == 13)
                //{
                //    //thuc hien lay dia chi cho nay
                //    currentMH = CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui;
                //}♦
                //else
                //{
                SendKeys.SendWait("{TAB}{TAB}");
                Thread.Sleep(50);

                string copyed = APIManager.GetCopyData();
                if (copyed != null)
                {
                    string[] enterText = copyed.Split('\n');
                    if (enterText.Length == 2)
                    {
                        copyed = enterText[1];
                    }
                    string[] data = copyed.Split('\t');

                    currentMH = data[1];
                    CurrentSelectedHangHoaDetail.Code = currentMH.ToUpper();
                }
                else { e.Cancel = true; }

                //}

                //if (currentMH.Length == 13)
                //{
                //    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanMHCTDen", Content = currentMH });
                //}
            }
            else if (window.text.IndexOf("xem chuyen thu chieu den") != -1)
            {
                window = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
                if (window == null)
                {
                    e.Cancel = true;
                    return;
                }

                bool daTimThay = checkDanhSachTrongDongCT(CurrentSelectedHangHoaDetail);
                if (!daTimThay)
                {
                    SoundManager.playSound3(@"Number\error_sound.wav");
                    e.Cancel = true;
                    return;
                }

                //bool daTimThay = checkDanhSachTrongDongCT(CurrentSelectedHangHoaDetail);
                //if (daTimThay)
                //{
                //    APIManager.ShowSnackbar("da tim thay");
                //}
                //else
                //{
                //    APIManager.ShowSnackbar("Khong tim thay");
                //}
                //return;

                TamQuanModel maHieu = layMaHieuTrongDongCT();
                if (maHieu == null)
                {
                    SoundManager.playSound3(@"Number\error_sound.wav");
                    e.Cancel = true;
                    return;
                }
                CurrentSelectedHangHoaDetail.Code = maHieu.MaHieu;
            }
        }

        private void BwLayMaHieu_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (ChuyenThuTiepTheo())
                    LayCodeFromSHTui();
            }
        }

        private bool checkDanhSachTrongDongCT(HangHoaDetailModel hangHoa)
        {
            SendKeys.SendWait("{F5}");
            SendKeys.SendWait("^{UP}");
            Thread.Sleep(100);
            string lastCopied = "";
            bool daTimThay = false;
            while (true)
            {
                string copiedText = APIManager.GetCopyData();
                if (string.IsNullOrEmpty(copiedText))
                    return false;
                if (lastCopied == copiedText)
                {
                    break;
                }
                else
                {
                    lastCopied = copiedText;
                }
                TuiInfo tui = xuLyTui(copiedText);
                if (tui.TuiSo == hangHoa.TuiHangHoa.TuiSo)
                    if (tui.KL == hangHoa.TuiHangHoa.KhoiLuong)
                    {
                        daTimThay = true;
                        break;
                    }
                SendKeys.SendWait("{DOWN}");
                Thread.Sleep(50);
            }
            if (daTimThay)
                return true;
            else return false;

            //False	1	6,5	Ði ngoài(BK)	False	Cleared
            //False	2	6,5	Ði ngoài(BK)	True	Cleared
            //	Túi số	KL (kg)	Loại túi	F	xác nhận
            // False   1   7,0 Ði ngoài(BK)    True Cleared
        }

        private void ChonCT(HangHoaDetailModel selected)
        {
            if (selected == null)
                return;
            CurrentSelectedHangHoaDetail = selected;
            if (selected.TrangThaiBD == TrangThaiBD.ChuaChon)
            {
                selected.TrangThaiBD = TrangThaiBD.DaChon;
            }
            if (IsBoQuaMaHieuSHTuiSai)
            {
                if (CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui.Length != 13)
                {
                }
            }

            if (!IsTuDongXuLy)
                if (!bwChiTiet.IsBusy)
                    bwChiTiet.RunWorkerAsync();
        }

        private bool ChuyenThuTiepTheo()
        {
            if (ListShowHangHoa.Count == 0)
            {
                APIManager.ShowSnackbar("Không có dữ liệu");
                return false;
            }
            //lay vi tri tiep theo
            //get index
            int index = ListShowHangHoa.IndexOf(CurrentSelectedHangHoaDetail);
            if (index == -1)
            {
                APIManager.ShowSnackbar("Chưa chọn mã hiệu");
                return false;
            }
            index++;
            if (index > ListShowHangHoa.Count - 1)
            {
                APIManager.ShowSnackbar("Đã tới vị trí cuối cùng");
                //txtInfo.Text = "Đã tới vị trí cuối cùng";
                return false;
            }

            //////xem thu no co chay cai gi khong

            SelectedTui = ListShowHangHoa[index];
            CurrentSelectedHangHoaDetail = ListShowHangHoa[index];
            return true;
        }

        private void ClearHangHoa()
        {
            foreach (var locBD in LocBDs)
            {
                locBD.IsEnabledButton = false;
                locBD.HangHoas.Clear();
            }
            LocKhaiThac.HangHoas.Clear();
            LocBCP.HangHoas.Clear();
            ConLai.HangHoas.Clear();
            ConLai.IsEnabledButton = false;
        }

        private void CopySHTui()
        {
            //thuc hien lenh trong nay
            if (SelectedTui != null)
            {
                System.Windows.Clipboard.SetDataObject(SelectedTui.TuiHangHoa.SHTui);
                //SendMessage
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = " Đã Copy" });
            }
        }

        private void DeleteTinh()
        {
            if (SelectedLocBD != null && SelectTinh != null)
            {
                SelectedLocBD.DanhSachTinh.Remove(SelectTinh);
            }
        }

        private void FillLocBD()
        {
            ClearHangHoa();
            //thuc hien viec clear Hang Hoa
            foreach (LocBDInfoModel locBD in LocBDs)
            {
                if (!string.IsNullOrEmpty(locBD.DanhSachHuyen))
                {
                    foreach (var item in FillThang(locBD.DanhSachHuyen))
                    {
                        IEnumerable<HangHoaDetailModel> listFilledHuyen = currentListHangHoa.Where(m => m.TuiHangHoa.ToBC.IndexOf(item) != -1);
                        foreach (HangHoaDetailModel temp in listFilledHuyen.ToList())
                        {
                            if (!string.IsNullOrEmpty(locBD.PhanLoais))
                            {
                                foreach (string phanLoai in FillThang(locBD.PhanLoais))
                                {
                                    if (temp.TuiHangHoa.PhanLoai.IndexOf(phanLoai) != -1)
                                    {
                                        temp.PhanLoai = locBD.TenBD;
                                        locBD.HangHoas.Add(temp);
                                        currentListHangHoa.Remove(temp);
                                        break;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(locBD.DichVus))
                            {
                                foreach (string phanLoai in FillThang(locBD.DichVus))
                                {
                                    if (temp.TuiHangHoa.DichVu.IndexOf(phanLoai) != -1)
                                    {
                                        temp.PhanLoai = locBD.TenBD;
                                        locBD.HangHoas.Add(temp);
                                        currentListHangHoa.Remove(temp);
                                        break;
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(locBD.PhanLoais) && string.IsNullOrEmpty(locBD.DichVus))
                            {
                                temp.PhanLoai = locBD.TenBD;
                                locBD.HangHoas.Add(temp);
                                currentListHangHoa.Remove(temp);
                            }
                            //temp.Key = locBD.TenBD;
                        }
                    }
                }
                if (locBD.DanhSachTinh.Count > 0)
                {
                    foreach (TinhHuyenModel item in locBD.DanhSachTinh)
                    {
                        IEnumerable<HangHoaDetailModel> listTinh = currentListHangHoa.Where(m => m.TuiHangHoa.ToBC.Substring(0, 2) == item.Ma);
                        foreach (HangHoaDetailModel temp in listTinh.ToList())
                        {
                            if (!string.IsNullOrEmpty(locBD.PhanLoais))
                            {
                                foreach (string phanLoai in FillThang(locBD.PhanLoais))
                                {
                                    if (temp.TuiHangHoa.PhanLoai.IndexOf(phanLoai) != -1)
                                    {
                                        temp.PhanLoai = locBD.TenBD;
                                        locBD.HangHoas.Add(temp);
                                        currentListHangHoa.Remove(temp);
                                        break;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(locBD.DichVus))
                            {
                                foreach (string phanLoai in FillThang(locBD.DichVus))
                                {
                                    if (temp.TuiHangHoa.DichVu.IndexOf(phanLoai) != -1)
                                    {
                                        temp.PhanLoai = locBD.TenBD;
                                        locBD.HangHoas.Add(temp);
                                        currentListHangHoa.Remove(temp);
                                        break;
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(locBD.PhanLoais) && string.IsNullOrEmpty(locBD.DichVus))
                            {
                                temp.PhanLoai = locBD.TenBD;
                                locBD.HangHoas.Add(temp);
                                currentListHangHoa.Remove(temp);
                            }
                        }
                    }
                }
            }

            //Xu ly ben kt bcp
            //KT
            if (!string.IsNullOrEmpty(LocKhaiThac.DanhSachHuyen))
            {
                IEnumerable<HangHoaDetailModel> listFill = currentListHangHoa.Where(m => m.TuiHangHoa.ToBC.IndexOf(LocKhaiThac.DanhSachHuyen) != -1);
                foreach (var item in listFill.ToList())
                {
                    item.PhanLoai = LocKhaiThac.TenBD;
                    LocKhaiThac.HangHoas.Add(item);
                    currentListHangHoa.Remove(item);
                }
            }

            //BCP
            if (!string.IsNullOrEmpty(LocBCP.DanhSachHuyen))
            {
                IEnumerable<HangHoaDetailModel> listFill = currentListHangHoa.Where(m => m.TuiHangHoa.ToBC.IndexOf(LocBCP.DanhSachHuyen) != -1);
                foreach (var item in listFill.ToList())
                {
                    item.PhanLoai = LocBCP.TenBD;
                    LocBCP.HangHoas.Add(item);
                    currentListHangHoa.Remove(item);
                }
            }

            //Con Lai
            if (currentListHangHoa.Count > 0)
            {
                foreach (HangHoaDetailModel item in currentListHangHoa.ToList())
                {
                    item.PhanLoai = ConLai.TenBD;
                    ConLai.HangHoas.Add(item);
                    currentListHangHoa.Remove(item);
                }
            }
        }

        private List<string> FillThang(string text)
        {
            List<string> temp = text.Split('|').ToList();

            return temp;
        }

        private void GetDataFromCloud()
        {
            var listLoc = FileManager.LoadLocBDOnFirebase();
            if (listLoc != null)
            {
                LocBDs = new ObservableCollection<LocBDInfoModel>();
                foreach (LocBDInfoModel item in listLoc)
                {
                    if (item.TaoBDs.Count == 0)
                    {
                        item.TaoBDs.Add(new TaoBdInfoModel());
                    }
                    LocBDs.Add(item);
                }
            }
        }

        private void GuiTrucTiep()
        {
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;
            _ = APIManager.ConvertToUnSign3(currentWindow.text).ToLower();
            if (_ListShowHangHoa.Count == 0)
            {
                APIManager.ShowSnackbar("Chưa có dữ liệu");
                return;
            }

            //thuc hien kiem tra thu hien tai dang dung cai nao
            var handles = APIManager.GetAllChildHandles(currentWindow.hwnd);
            string textHandleName = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
            string textSoLuongTui = "WindowsForms10.STATIC.app.0.1e6fa8e";
            int countTemp = 0;
            string classNameComBoBox = "";
            int countCurrentTui = 0;

            bool isGone = false;
            if (NameTinhCurrent != LocBDs[0].TenBD)
            {
                List<TestAPIModel> controlsHandle = APIManager.GetListControlText(currentWindow.hwnd);
                foreach (var item in controlsHandle)
                {
                    if (item.ClassName.IndexOf(textHandleName) != -1)
                    {
                        classNameComBoBox = item.Text;
                    }
                    else if (item.ClassName.IndexOf(textSoLuongTui) != -1)
                    {
                        if (countTemp == 10)
                        {
                            countCurrentTui = int.Parse(item.Text);
                        }
                        countTemp++;
                    }

                    //tim cai o cua sh tui
                    //focus no
                    //xong roi dien vao va nhan enter thoi
                }
            }
            else
            {
                isGone = true;
            }

            bool isRightBD10 = true;
            var currentLocBd = LocBDs.FirstOrDefault(m => m.TenBD == NameTinhCurrent);
            if (currentLocBd == null)
            {
                return;
            }
            if (currentLocBd.TenBD == LocBDs[0].TenBD)
            {
                isGone = true;
            }
            else if (classNameComBoBox.IndexOf(currentLocBd.TaoBDs[0].BCNhan.Substring(0, 6)) == -1)
            {
                isRightBD10 = false;
            }

            if (!isRightBD10)
            {
                //thuc hien doc roi thoat
                SoundManager.playSound(@"Number\nhapkhongdung.wav");
                return;
            }
            //thuc hien kiem tra ngay trong nay

            Thread.Sleep(200);
            WindowInfo suaBDInfo = APIManager.GetActiveWindowTitle();
            ///////////////////////
            TestAPIModel textBDHandle = null;

            var controls = APIManager.GetListControlText(suaBDInfo.hwnd);
            if (suaBDInfo.text.IndexOf("sua thong tin bd10") != -1 || suaBDInfo.text == "lap bd10")
            {
                int iShTuiHandle = controls.FindIndex(m => m.Text == "SH túi");
                textBDHandle = controls[iShTuiHandle + 1];
            }
            else if (suaBDInfo.text.IndexOf("lap bd10 theo") != -1)
            {
                int iShTuiHandle = controls.FindIndex(m => m.Text == "SH túi");
                textBDHandle = controls[iShTuiHandle - 1];
            }
            if (textBDHandle == null)
            {
                return;
            }

            ///////////////////////////////////////////
            //txtStateSend.Text = "Đang Gửi Trực Tiếp";
            double delayTime = Convert.ToDouble(SelectedTime);
            int lastNumber = 0;
            for (int i = IndexTaoBDItem; i < ListShowHangHoa.Count; i++)
            {
                HangHoaDetailModel hangHoa = ListShowHangHoa[i];
                CurrentIndexGui = (i + 1).ToString();
                //SendKeys.SendWait(hangHoa.TuiHangHoa.SHTui);
                //SendKeys.SendWait("{ENTER}");

                ////////////////////////////////

                APIManager.setTextControl(textBDHandle.Handle, hangHoa.TuiHangHoa.SHTui);
                SendKeys.SendWait("{ENTER}");
                //lenh la cho khi so thay doi hoac hien thong bao toi da la 1 s
                if (i == ListShowHangHoa.Count - 1)
                {
                    break;
                }

                /////////////////////////////////
                int countDown = 100;
                while (true)
                {
                    Thread.Sleep(50);
                    countDown--;
                    if (lastNumber != APIManager.currentNumberBD)
                    {
                        lastNumber = APIManager.currentNumberBD;
                        break;
                    }
                    var active = APIManager.GetActiveWindowTitle();
                    if (active.text.IndexOf("sua thong tin bd") == -1 && active.text.IndexOf("lap bd10") == -1)
                    {
                        IndexTaoBDItem = i;
                        APIManager.ShowSnackbar("Lỗi");
                        return;
                    }
                    if (countDown <= 0)
                    {
                        IndexTaoBDItem = 0;
                        APIManager.ShowSnackbar("Hết thời gian chờ.");
                        return;
                    }
                }

                //Thread.Sleep(500);
            }

            if (!isGone)
            {
                //kiem tra so luong tui hien tai co bang
                int lastCountTuiHienTai = 0;
                countTemp = 0;
                foreach (var item in handles)
                {
                    string classText = APIManager.GetWindowClass(item);

                    if (classText.IndexOf(textSoLuongTui) != -1)
                    {
                        if (countTemp == 10)
                        {
                            lastCountTuiHienTai = int.Parse(APIManager.GetControlText(item));
                        }
                        countTemp++;
                    }
                    //tim cai o cua sh tui
                    //focus no
                    //xong roi dien vao va nhan enter thoi
                }

                if (lastCountTuiHienTai - countCurrentTui != ListShowHangHoa.Count)
                {
                    SoundManager.playSound2(@"Number\khongkhopsolieu.wav");
                    return;
                }
                else
                {
                    SoundManager.playSound2(@"Number\dusoluong.wav");
                    APIManager.ShowSnackbar("OK");
                    return;
                }
            }
        }

        private void Hidden()
        {
            var currentWindow = APIManager.WaitingFindedWindow("lap bd10");
            if (currentWindow == null)
                return;
            List<TestAPIModel> controls = APIManager.GetListControlText(currentWindow.hwnd);
            TestAPIModel controlChuyen = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[1];
            TestAPIModel controlBCNhan = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[3];
            if (_taoBDAdd.IsSangChieu)
            {
                int cHour = DateTime.Now.Hour;
                if (cHour < _taoBDAdd.ThoiGianChia2LanDT)
                    APIManager.setTextControl(controlChuyen.Handle, _taoBDAdd.Chuyen1);
                else
                    APIManager.setTextControl(controlChuyen.Handle, _taoBDAdd.Chuyen2);
            }
            else
            {
                APIManager.setTextControl(controlChuyen.Handle, _taoBDAdd.Chuyen1);
            }
            SendKeys.SendWait("{TAB}");
            APIManager.setTextControl(controlBCNhan.Handle, _taoBDAdd.BCNhan);
            SendKeys.SendWait("{TAB}");
            if (_taoBDAdd.IsNextDay)
            {
                SendKeys.SendWait(@"{UP}");
            }
            SendKeys.SendWait(@"{ENTER}");
        }

        private void KhongInLanChanged()
        {
            if (ListShowHangHoa == null)
                return;
            if (KhongInLan)
            {
                foreach (HangHoaDetailModel hangHoa in ListShowHangHoa)
                {
                    if (!string.IsNullOrEmpty(hangHoa.BuuCucChapNhan))
                        if (hangHoa.BuuCucChapNhan.IndexOf("593200") != -1 && (hangHoa.TuiHangHoa.KhoiLuong == "1,0" || hangHoa.TuiHangHoa.KhoiLuong == "1,5" || hangHoa.TuiHangHoa.KhoiLuong == "2,0" || hangHoa.TuiHangHoa.KhoiLuong == "2,5"))
                            hangHoa.TrangThaiBD = TrangThaiBD.KhongIn;
                }
            }
            else
            {
                foreach (HangHoaDetailModel hangHoa in ListShowHangHoa)
                {
                    if (!string.IsNullOrEmpty(hangHoa.BuuCucChapNhan))
                    {
                        if (hangHoa.BuuCucChapNhan.IndexOf("593200") != -1 && (hangHoa.TuiHangHoa.KhoiLuong == "1,0" || hangHoa.TuiHangHoa.KhoiLuong == "1,5" || hangHoa.TuiHangHoa.KhoiLuong == "2,0" || hangHoa.TuiHangHoa.KhoiLuong == "2,5"))
                            hangHoa.TrangThaiBD = TrangThaiBD.ChuaChon;
                    }
                }
            }
        }

        private void LayCodeFromSHTui()
        {
            if (CurrentSelectedHangHoaDetail == null)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }
            if (CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui.Length == 13)
            {
                if (ChuyenThuTiepTheo())
                    LayCodeFromSHTui();
                else
                {
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "ToAddress_LayDanhSach" });
                }
            }
            if (IsTuDongXuLy)
                if (!bwLayMaHieu.IsBusy)
                    bwLayMaHieu.RunWorkerAsync();
        }

        private void LayDiaChi(string listCode)
        {
            string addressDefault = "https://bccp.vnpost.vn/BCCP.aspx?act=MultiTrace&id=";

            addressDefault += listCode;
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "ListAddressChuyenThu", Content = addressDefault });
        }

        private TamQuanModel layMaHieuTrongDongCT()
        {
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return null;

            if (currentWindow.text.IndexOf("xem chuyen thu chieu den") != -1)
            {
                SendKeys.SendWait("{F6}");
                Thread.Sleep(100);
                SendKeys.SendWait("{TAB}{TAB}");
                Thread.Sleep(100);

                SendKeys.SendWait("^(c)");
                string data = APIManager.GetCopyData();

                //loc du lieu trong nay
                //STT	Số hiệu	Tỉnh gốc	BC gốc	BC phát	Loại	KL (gr)	QĐ (gr)	Số hiệu lô	Ghi chú
                // 1   CH214294910VN   10  157870  593280  C * 10000   0
                var temp = data.Split('\n');
                if (temp.Length == 2)
                {
                    string code = temp[1].Split('\t')[1];
                    bool isDoubleRight = double.TryParse(temp[1].Split('\t')[6], out double klTemp);

                    //xu ly du lieu trong nay
                    if (code.Length == 13 && isDoubleRight)
                    {
                        TamQuanModel tamquan = new TamQuanModel(1, code, klTemp);
                        return tamquan;
                    }
                }
            }
            return null;
        }

        private void LenLoc()
        {
            int indexLoc = LocBDs.IndexOf(SelectedLocBD);
            if (indexLoc == -1)
                return;
            if (indexLoc == 0)
                return;
            LocBDInfoModel tempCT = LocBDs[indexLoc - 1];
            LocBDs[indexLoc - 1] = SelectedLocBD;
            LocBDs[indexLoc] = tempCT;
            SelectedLocBD = LocBDs[indexLoc - 1];
        }

        private void LoadLoc()
        {
            var list = FileManager.LoadLocKTBCPOffline();
            LocKhaiThac = list[0];
            LocBCP = list[1];
        }

        private void PrintDiNgoai()
        {
            WindowInfo info = APIManager.WaitingFindedWindow("thong tin buu gui");
            if (info == null)
                return;

            APIManager.SetPrintBD10();
            List<TestAPIModel> listControl = APIManager.GetListControlText(info.hwnd);
            List<TestAPIModel> listWindowForm = listControl.Where(m => m.ClassName.IndexOf("10.Window.8.ap") != -1).ToList();

            //thuc hien kiem tra ma trong nay
            bool laTimThayBD8 = false;
            string clipboard = "";
            for (int i = 0; i < 7; i++)
            {
                Clipboard.Clear();
                SendKeys.SendWait("+{TAB}");
                SendKeys.SendWait("^(a)");
                Thread.Sleep(50);
                clipboard = APIManager.GetCopyData();
                Thread.Sleep(50);
                if (clipboard.IndexOf("BĐ1 Bis") != -1)
                {
                    laTimThayBD8 = true;
                    break;
                }
            }
            if (!laTimThayBD8)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }
            foreach (string item in clipboard.Split('\n'))
            {
                var datas = item.Split('\t');
                if (datas[1].IndexOf("BĐ1 Bis") != -1)
                {
                    SendKeys.SendWait(" ");
                    break;
                }
                if (datas[4].IndexOf("BĐ1 Bis") != -1)
                {
                    SendKeys.SendWait("{RIGHT}");
                    Thread.Sleep(50);
                    SendKeys.SendWait("{RIGHT}");
                    Thread.Sleep(50);
                    SendKeys.SendWait("{RIGHT}");
                    Thread.Sleep(50);
                    SendKeys.SendWait(" ");
                    Thread.Sleep(100);
                    break;
                }
                SendKeys.SendWait("{DOWN}");
            }

            APIManager.ClickButton(info.hwnd, "in an pham", isExactly: false);
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("print document");
            if (currentWindow == null)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }
            Thread.Sleep(500);
            APIManager.ClickButton(currentWindow.hwnd, "in an pham", isExactly: false);

            WindowInfo infoPrint = APIManager.WaitingFindedWindow("Print", isExactly: true);
            if (infoPrint == null)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }

            SendKeys.SendWait("%(p)");
            Thread.Sleep(500);

            WindowInfo infoPrintDocument = APIManager.WaitingFindedWindow("Print Document", isExactly: true);
            if (infoPrintDocument == null)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }

            if (CurrentSelectedHangHoaDetail != null)
            {
                CurrentSelectedHangHoaDetail.TrangThaiBD = TrangThaiBD.DaIn;
            }
            APIManager.ClickButton(currentWindow.hwnd, "thoat", isExactly: false);

            WindowInfo infoThongTin = APIManager.WaitingFindedWindow("thong tin buu gui");
            if (infoThongTin == null)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }
            APIManager.ClickButton(infoThongTin.hwnd, "thoat", isExactly: false);
            if (IsTuDongXuLy)
            {
                if (ChuyenThuTiepTheo())
                    TuDongXuLyCT();
            }
        }

        private void Publish()
        {
            FileManager.SaveLocBD10Firebase(LocBDs.ToList());
        }

        private void SaveLocBD()
        {
            //DeleteTinhWhenUnCheck();
            FileManager.SaveLocBD10Offline(LocBDs.ToList());
        }

        private void SaveTinhToSelectedLocBD()
        {
            if (SelectedLocBD == null)
                return;

            if (SelectedLocBD.IsTinh)
            {
                foreach (TinhHuyenModel item in ShowTinhs)
                {
                    if (item.IsChecked)
                    {
                        TinhHuyenModel isHave = SelectedLocBD.DanhSachTinh.FirstOrDefault(m => m.Ma == item.Ma);
                        if (isHave == null)
                        {
                            SelectedLocBD.DanhSachTinh.Add(item);
                        }
                    }
                }
            }
        }

        private void SelectedTinh(string Name)
        {
            try
            {
                IndexContinueGuiTrucTiep = 0;
                if (string.IsNullOrEmpty(Name))
                    return;
                if (Name == ConLai.TenBD)
                {
                    if (ConLai.HangHoas.Count == 0)
                        return;

                    ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();

                    foreach (HangHoaDetailModel hangHoa in ConLai.HangHoas)
                    {
                        hangHoa.Index = ListShowHangHoa.Count + 1;
                        ListShowHangHoa.Add(hangHoa);
                    }
                    //thuc hien show Ten Tinh
                    NameTinhCurrent = ConLai.TenBD;
                }
                else if (Name == LocKhaiThac.TenBD || Name == LocBCP.TenBD)
                {
                    CurrentSelectedHangHoaDetail = null;
                    LocBDInfoModel currenLoc;
                    if (Name == LocKhaiThac.TenBD)
                    {
                        currenLoc = LocKhaiThac;
                        currentBuuCuc = BuuCuc.KT;
                    }
                    else
                    {
                        currenLoc = LocBCP;
                        currentBuuCuc = BuuCuc.BCP;
                    }

                    if (currenLoc.HangHoas.Count == 0)
                        return;

                    ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();
                    string SHTuiList = "";

                    foreach (HangHoaDetailModel hangHoa in currenLoc.HangHoas)
                    {
                        string temp = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.DichVu).ToLower();
                        string temp1 = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower();
                        if (temp.IndexOf("phat hanh") != -1 || temp1.IndexOf("tui") != -1)
                        {
                            continue;
                        }
                        hangHoa.Index = ListShowHangHoa.Count + 1;
                        ListShowHangHoa.Add(hangHoa);
                        if (hangHoa.TuiHangHoa.SHTui.Length == 13)
                        {
                            SHTuiList += hangHoa.TuiHangHoa.SHTui + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(SHTuiList))
                    {
                        LayDiaChi(SHTuiList);
                    }

                    //thuc hien show Ten Tinh
                    NameTinhCurrent = currenLoc.TenBD;
                }
                else
                {
                    LocBDInfoModel LocBD = LocBDs.FirstOrDefault(m => m.TenBD == Name);
                    if (LocBD == null)
                        return;
                    if (LocBD.HangHoas.Count == 0)
                        return;

                    ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();

                    foreach (HangHoaDetailModel hangHoa in LocBD.HangHoas)
                    {
                        hangHoa.Index = ListShowHangHoa.Count + 1;
                        ListShowHangHoa.Add(hangHoa);
                    }
                    //thuc hien show Ten Tinh
                    NameTinhCurrent = LocBD.TenBD;
                    //ShowNameTinh(LocBD.TenBD);
                }
                if (ListShowHangHoa.Count > 0)
                {
                    IndexTaoBDItem = 0;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "APIManager " + line + " Number Line ");
                throw;
            }
        }

        private void TaoBDWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //var temp = FileManager.optionModel.GoFastBD10Di.Split(',');
            //APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "danh sach bd10 di", temp[0], temp[1]);

            WindowInfo currentWindow = APIManager.WaitingFindedWindow("danh sach bd10 di");
            if (currentWindow == null)
                return;
            APIManager.ClickButton(currentWindow.hwnd, "f1", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("lap bd10");
            if (currentWindow == null)
                return;
            System.Collections.Generic.List<Model.TestAPIModel> controls = APIManager.GetListControlText(currentWindow.hwnd);
            Model.TestAPIModel controlDuongThu = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[2];
            Model.TestAPIModel controlChuyen = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[1];
            Model.TestAPIModel controlBCNhan = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[3];
            TestAPIModel editDuongThu = controls.Where(m => m.ClassName == "Edit").ToList()[2];
            TestAPIModel editChuyen = controls.Where(m => m.ClassName == "Edit").ToList()[1];
            TestAPIModel editBCNhan = controls.Where(m => m.ClassName == "Edit").ToList()[3];
            APIManager.FocusHandle(controlDuongThu.Handle);
            APIManager.setTextControl(editDuongThu.Handle, _taoBDAdd.DuongThu);

            SendKeys.SendWait("{TAB}");
            Thread.Sleep(200);
            HiddenCommand.Execute(null);
        }

        private void TimerTaoBD_Tick(object sender, EventArgs e)
        {
            if (isWaiting)
                return;
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;

            switch (stateTaoBd10)
            {
                case StateTaoBd10.DanhSachBD10:
                    if (currentWindow.text.IndexOf("danh sach bd10 di") != -1)
                    {
                        SendKeys.SendWait("{F1}");
                        stateTaoBd10 = StateTaoBd10.LapBD10;
                    }
                    break;

                case StateTaoBd10.LapBD10:
                    if (currentWindow.text.IndexOf("lap bd10") != -1)
                    {
                        isWaiting = true;
                        for (int i = 0; i < countDuongThu; i++)
                        {
                            SendKeys.SendWait("{DOWN}");
                            Thread.Sleep(50);
                        }
                        SendKeys.SendWait("^(c)");
                        Thread.Sleep(100);
                        string clip = Clipboard.GetText();
                        if (clip.IndexOf(tenDuongThu) == -1)
                        {
                            isWaiting = false;
                            timerTaoBD.Stop();
                        }
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        for (int i = 0; i < countChuyen; i++)
                        {
                            SendKeys.SendWait("{DOWN}");
                            Thread.Sleep(50);
                        }
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        SendKeys.SendWait(maBuuCuc);
                        Thread.Sleep(100);
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        isWaiting = false;
                        timerTaoBD.Stop();
                    }

                    break;

                default:
                    break;
            }
        }

        private void TuDongXacNhanCT()
        {
        }

        private void TuDongXuLyCT()
        {
            if (CurrentSelectedHangHoaDetail == null)
            {
                SoundManager.playSound3(@"Number\error_sound.wav");
                return;
            }
            if (IsBoQuaMaHieuSHTuiSai)
                if (CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui.Length != 13)
                {
                    if (ChuyenThuTiepTheo())
                        TuDongXuLyCT();
                }
            if (CurrentSelectedHangHoaDetail.TrangThaiBD == TrangThaiBD.KhongIn)
            {
                if (ChuyenThuTiepTheo())
                    TuDongXuLyCT();
            }

            if (!bwChiTiet.IsBusy)
                bwChiTiet.RunWorkerAsync();
        }

        private void UpdateBuuCucChuyenThu()
        {
            if (MaKT.Length == 6)
            {
                LocKhaiThac = new LocBDInfoModel
                {
                    DanhSachHuyen = MaKT,
                    TenBD = MaKT
                };
            }
            if (MaBCP.Length == 6)
            {
                LocBCP = new LocBDInfoModel
                {
                    DanhSachHuyen = MaBCP,
                    TenBD = MaBCP
                };
            }
            List<LocBDInfoModel> list = new List<LocBDInfoModel>();
            list.Add(LocKhaiThac);
            list.Add(LocBCP);
            FileManager.SaveLocKTBCPOffline(list);
        }

        private void UpdateEnableButtonLoc()
        {
            foreach (var locBD in LocBDs)
            {
                locBD.IsEnabledButton = false;
                if (locBD.HangHoas.Count > 0)
                {
                    locBD.IsEnabledButton = true;
                }
            }
            if (ConLai.HangHoas.Count > 0)
            {
                ConLai.IsEnabledButton = true;
            }
        }

        private WindowInfo VaoChiTietChuyenThu(string maSHTui)
        {
            switch (currentBuuCuc)
            {
                case BuuCuc.KT:
                    string[] temp = FileManager.optionModel.GoFastQLCTCDKT.Split(',');
                    APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "quan ly chuyen thu chieu den", temp[0], temp[1]);
                    break;

                case BuuCuc.BCP:
                    string[] temp1 = FileManager.optionModel.GoFastQLCTCDBCP.Split(',');
                    APIManager.GoToWindow(FileManager.optionModel.MaBuuCucPhat, "quan ly chuyen thu chieu den", temp1[0], temp1[1]);
                    break;

                default:
                    break;
            }
            Thread.Sleep(100);
            WindowInfo window = APIManager.WaitingFindedWindow("quan ly chuyen thu");
            if (window == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy quản lý chuyến thư");
                return null;
            }

            //List<TestAPIModel> controls = APIManager.GetListControlText(window.hwnd);
            //if (controls.Count == 0)
            //{
            //    APIManager.ShowSnackbar("Window lỗi");
            //    return;
            //}
            //List<TestAPIModel> listCombobox = controls.Where(m => m.ClassName.ToLower().IndexOf("combobox") != -1).ToList();
            //IntPtr comboHandle = listCombobox[3].Handle;

            //APIManager.SendMessage(comboHandle, 0x0007, 0, 0);
            //APIManager.SendMessage(comboHandle, 0x0007, 0, 0);

            SendKeys.SendWait("{F3}");
            SendKeys.SendWait(maSHTui);
            SendKeys.SendWait("{ENTER}");
            window = APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu", "xem chuyen thu chieu den");
            if (window == null)
                return null;
            Thread.Sleep(500);
            window = APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu", "xem chuyen thu chieu den");
            if (window == null)
                return null;
            return window;
        }

        private void XeXaHoi()
        {
            maBuuCuc = "590100";
            tenDuongThu = "Tam Quan - Quy Nhơn (Xe XH)";
            countDuongThu = 11;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        private TuiInfo xuLyTui(string copiedText)
        {
            try
            {
                string[] textEnter = copiedText.Split('\n');
                string textDaChia = "";
                if (textEnter.Length > 1)
                    textDaChia = textEnter[1];
                else
                    textDaChia = copiedText;
                var textedTab = textDaChia.Split('\t');
                TuiInfo tui = new TuiInfo();
                tui.IsDaChon = bool.Parse(textedTab[0].Trim());
                tui.TuiSo = textedTab[1].Trim();
                tui.KL = textedTab[2].Trim();
                tui.LoaiTui = textedTab[3].Trim();
                tui.FTui = bool.Parse(textedTab[4].Trim());
                tui.DaXacNhan = textedTab[5].Trim();
                return tui;
            }
            catch
            {
                APIManager.ShowSnackbar(copiedText);
                return new TuiInfo();
            }
        }

        private void XuongLoc()
        {
            int indexLoc = LocBDs.IndexOf(SelectedLocBD);
            if (indexLoc == -1)
                return;
            if (indexLoc == LocBDs.Count - 1)
                return;
            LocBDInfoModel tempCT = LocBDs[indexLoc + 1];
            LocBDs[indexLoc + 1] = SelectedLocBD;
            LocBDs[indexLoc] = tempCT;
            SelectedLocBD = LocBDs[indexLoc + 1];
        }

        private readonly BackgroundWorker bwChiTiet;
        private readonly BackgroundWorker taoBDWorker;
        private readonly DispatcherTimer timerTaoBD;
        private LocBDInfoModel _ConLai;
        private TaoBdInfoModel _CurrenTaoBD;
        private string _CurrentIndexGui = "0";
        private HangHoaDetailModel _CurrentSelectedHangHoaDetail;
        private int _IndexContinueGuiTrucTiep = 0;
        private int _IndexTaoBDItem;
        private bool _IsBoQuaMaHieuSHTuiSai;
        private bool _IsTuDongXuLy = true;
        private bool _KhongInLan;
        private ObservableCollection<HangHoaDetailModel> _ListShowHangHoa;
        private LocBDInfoModel _LocBCP;
        private ObservableCollection<LocBDInfoModel> _LocBDs;
        private LocBDInfoModel _LocKhaiThac;
        private string _MaBCP;
        private string _MaKT;
        private string _NameTinhCurrent;
        private LocBDInfoModel _SelectedLocBD;
        private string _SelectedTime = "0.5";
        private HangHoaDetailModel _SelectedTui;
        private TinhHuyenModel _SelectTinh;
        private ObservableCollection<TinhHuyenModel> _ShowTinhs;
        private TaoBdInfoModel _taoBDAdd;
        private string _TextCurrentChuyenThu;
        private BackgroundWorker bwLayMaHieu;
        private int countChuyen = 0;
        private int countDuongThu = 0;
        private BuuCuc currentBuuCuc = BuuCuc.None;
        private List<HangHoaDetailModel> currentListHangHoa;
        private bool isWaiting = false;
        private string maBuuCuc = "0";
        private StateTaoBd10 stateTaoBd10;
        private string tenDuongThu = "";
        public IRelayCommand<string> AddBDTinhCommand { get; }

        public LocBDInfoModel ConLai
        {
            get { return _ConLai; }
            set { SetProperty(ref _ConLai, value); }
        }

        public IRelayCommand CopySHTuiCommand { get; }

        public TaoBdInfoModel CurrenTaoBD
        {
            get { return _CurrenTaoBD; }
            set { SetProperty(ref _CurrenTaoBD, value); }
        }

        public string CurrentIndexGui
        {
            get { return _CurrentIndexGui; }
            set { SetProperty(ref _CurrentIndexGui, value); }
        }

        public HangHoaDetailModel CurrentSelectedHangHoaDetail
        {
            get { return _CurrentSelectedHangHoaDetail; }
            set { SetProperty(ref _CurrentSelectedHangHoaDetail, value); }
        }

        public ICommand DeleteTinhCommand { get; }
        public ICommand GetDataFromCloudCommand { get; }
        public ICommand GopBDCommand { get; }
        public ICommand HiddenCommand { get; }

        public int IndexContinueGuiTrucTiep
        {
            get { return _IndexContinueGuiTrucTiep; }
            set { SetProperty(ref _IndexContinueGuiTrucTiep, value); }
        }

        public int IndexTaoBDItem
        {
            get { return _IndexTaoBDItem; }
            set { SetProperty(ref _IndexTaoBDItem, value); }
        }

        public bool IsBoQuaMaHieuSHTuiSai
        {
            get { return _IsBoQuaMaHieuSHTuiSai; }
            set { SetProperty(ref _IsBoQuaMaHieuSHTuiSai, value); }
        }

        public bool IsTuDongXuLy
        {
            get { return _IsTuDongXuLy; }
            set { SetProperty(ref _IsTuDongXuLy, value); }
        }

        public bool KhongInLan
        {
            get { return _KhongInLan; }
            set
            {
                SetProperty(ref _KhongInLan, value);
                KhongInLanChanged();
            }
        }

        public ICommand LayCodeFromSHTuiCommand { get; }
        public ICommand LenLocCommand { get; }

        public ObservableCollection<HangHoaDetailModel> ListShowHangHoa
        {
            get { return _ListShowHangHoa; }
            set { SetProperty(ref _ListShowHangHoa, value); }
        }

        public LocBDInfoModel LocBCP
        {
            get { return _LocBCP; }
            set { SetProperty(ref _LocBCP, value); }
        }

        public ObservableCollection<LocBDInfoModel> LocBDs
        {
            get { return _LocBDs; }
            set { SetProperty(ref _LocBDs, value); }
        }

        public LocBDInfoModel LocKhaiThac
        {
            get { return _LocKhaiThac; }
            set { SetProperty(ref _LocKhaiThac, value); }
        }

        public string MaBCP
        {
            get { return _MaBCP; }
            set { SetProperty(ref _MaBCP, value); }
        }

        public string MaKT
        {
            get { return _MaKT; }
            set { SetProperty(ref _MaKT, value); }
        }

        public string NameTinhCurrent
        {
            get { return _NameTinhCurrent; }
            set { SetProperty(ref _NameTinhCurrent, value); }
        }

        public ICommand PublishCommand { get; }
        public ICommand SaveLocBDCommand { get; }
        public ICommand SaveTinhToSelectedLocBDCommand { get; }

        public LocBDInfoModel SelectedLocBD
        {
            get { return _SelectedLocBD; }
            set
            {
                SetProperty(ref _SelectedLocBD, value);
            }
        }

        public string SelectedTime
        {
            get { return _SelectedTime; }
            set { SetProperty(ref _SelectedTime, value); }
        }

        public ICommand SelectedTinhCommand { get; }

        public HangHoaDetailModel SelectedTui
        {
            get { return _SelectedTui; }
            set
            {
                SetProperty(ref _SelectedTui, value);
                CopySHTuiCommand.NotifyCanExecuteChanged();
            }
        }

        public IRelayCommand<HangHoaDetailModel> SelectionCommand { get; }

        public TinhHuyenModel SelectTinh
        {
            get { return _SelectTinh; }
            set { SetProperty(ref _SelectTinh, value); }
        }

        public ObservableCollection<TinhHuyenModel> ShowTinhs
        {
            get { return _ShowTinhs; }
            set { SetProperty(ref _ShowTinhs, value); }
        }

        public string TextCurrentChuyenThu
        {
            get { return _TextCurrentChuyenThu; }
            set { SetProperty(ref _TextCurrentChuyenThu, value); }
        }

        public ICommand TuDongXacNhanCTCommand { get; }
        public ICommand TuDongXuLyCTCommand { get; }
        public ICommand UpdateBuuCucChuyenThuCommand { get; }
        public ICommand XeXaHoiCommand { get; }
        public ICommand XuongLocCommand { get; }
    }
}