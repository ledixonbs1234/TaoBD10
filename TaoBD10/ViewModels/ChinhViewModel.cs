using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;
using WindowsInput;
using WindowsInput.Native;

namespace TaoBD10.ViewModels
{
    public class ChinhViewModel : ObservableObject
    {
        public ChinhViewModel()
        {
            KTHNCommand = new RelayCommand(KTHN);

            TongHopCommand = new RelayCommand(TongHop);
            AnNhonCommand = new RelayCommand(AnNhon);

            PhuCatCommand = new RelayCommand(PhuCat);

            PhuMyCommand = new RelayCommand(PhuMy);

            QuyNhon2Command = new RelayCommand(QuyNhon2);

            QuiNhon1Command = new RelayCommand(QuiNhon1);

            EMSCommand = new RelayCommand(EMS);

            KienCommand = new RelayCommand(Kien);

            BCPHNCommand = new RelayCommand(BCPHN);

        }

        private ChuyenThuModel currentChuyenThu;

        public ICommand KTHNCommand { get; }
        public ICommand BCPHNCommand { get; }
        public ICommand KienCommand { get; }

        public ICommand EMSCommand { get; }
        public ICommand QuiNhon1Command { get; }

        public ICommand QuyNhon2Command { get; }

        public ICommand PhuMyCommand { get; }

        public ICommand PhuCatCommand { get; }

        public ICommand AnNhonCommand { get; }

        public ICommand TongHopCommand { get; }

        

        void TongHop()
        {

        }

        
        void AnNhon()
        {

        }

        void PhuCat()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Phù Cát Tổng Hợp";
            chuyenThu.NumberTinh = "592460";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "592460";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo"; 
            chuyenThu.NameMusic = "phucatth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();
        }

        void PhuMy()
        {

        }

        void QuyNhon2()
        {

        }

        void QuiNhon1()
        {

        }


        void EMS()
        {

        }

        void Kien()
        {

        }

        void BCPHN()
        {

        }



        void KTHN()
        {

        }


        void CreateChuyenThu()
        {
            //thuc hien cong viec trong nay
            WindowInfo currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
            {
                return;
            }
            int countTempReturn = 0;

            while (currentWindow.text.IndexOf("Khởi tạo chuyến thư") == -1)
            {
                currentWindow = APIManager.GetActiveWindowTitle();
                if (currentWindow == null)
                {
                    return;
                }
                countTempReturn++;
                if (countTempReturn > 50)
                {
                    return;
                }
                Thread.Sleep(100);
            }


            Thread.Sleep(100);

            var childsHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);
            //thuc hien lay vi tri nao do
            int indexTinh = 14;
            IntPtr Tinh = childsHandle[indexTinh];
            Thread.Sleep(100);


            APIManager.SendMessage(Tinh, 0x0007, 0, 0);
            APIManager.SendMessage(Tinh, 0x0007, 0, 0);            //thuc hien nhap vao
            var inputImulator = new InputSimulator();
            inputImulator.Keyboard.TextEntry(currentChuyenThu.NumberTinh);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);

            inputImulator.Keyboard.TextEntry(currentChuyenThu.TextLoai);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);
            countTempReturn = 0;

            while (currentWindow.text.IndexOf("Khởi tạo chuyến thư") != -1)
            {
                currentWindow = APIManager.GetActiveWindowTitle();
                countTempReturn++;
                if (countTempReturn > 30)
                {
                    return;
                }
                Thread.Sleep(100);
            }
            Thread.Sleep(200);


            currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
            {
                return;
            }
            if (currentWindow.text.IndexOf("Đóng chuyến thư") != -1)
            {

                int countInTui = 0;
                childsHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);
                string title = APIManager.GetControlText(childsHandle[3]);
                //kiem tra so luong la bao nhieu
                string resultString = Regex.Match(title, @"\d+").Value;
                bool isRight = int.TryParse(resultString, out countInTui);

                if (countInTui > 0)
                {

                }
                else
                {
                    //thuc hien cho tao tui 10 lan
                    countTempReturn = 0;
                    while (currentWindow.text.IndexOf("Tạo túi") == -1)
                    {
                        currentWindow = APIManager.GetActiveWindowTitle();
                        if (currentWindow == null)
                        {
                            return;
                        }
                        countTempReturn++;
                        if (countTempReturn > 10)
                        {
                            return;
                        }
                        Thread.Sleep(200);
                    }

                    //thuc hien co tui trong nay
                    var childTaoTuiHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);
                    IntPtr loai1 = IntPtr.Zero;

                    if (currentChuyenThu.Ten == "EMS Nam Trung Bộ" || currentChuyenThu.Ten == "Kiện Nam Trung Bộ")
                    { }
                    else
                    {
                        APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                        APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                        inputImulator.Keyboard.TextEntry(currentChuyenThu.TextTui);
                        inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    }
                    inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);
                }

            }
            else
            if (currentWindow.text.IndexOf("Tạo túi") != -1)
            {
                var childTaoTuiHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);
                IntPtr loai1 = IntPtr.Zero;

                if (currentChuyenThu.Ten == "EMS Nam Trung Bộ" || currentChuyenThu.Ten == "Kiện Nam Trung Bộ")
                { }
                else
                {
                    APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                    APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                    inputImulator.Keyboard.TextEntry(currentChuyenThu.TextTui);
                    inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
                }
                inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);
            }
            countTempReturn = 0;
            if (currentWindow == null)
            {
                return;
            }
            while (currentWindow.text.IndexOf("Đóng chuyến thư") == -1)
            {
                currentWindow = APIManager.GetActiveWindowTitle();
                countTempReturn++;
                if (countTempReturn > 30)
                {
                    return;
                }
                Thread.Sleep(100);
            }

           var childHandles = APIManager.GetAllChildHandles(currentWindow.hwnd);
            int countEdit = 0;
            string tempCheckTinh = "";
            string tempCheckLoai = "";
            string tempCheckThuyBo = "";
            foreach (var item in childHandles)
            {
                string className = APIManager.GetWindowClass(item);
                 string temp =APIManager.GetControlText(item);
                String text =APIManager.convertToUnSign3(temp).ToLower();

                string classDefault = "WindowsForms10.EDIT.app.0.1e6fa8e";
                if (className == classDefault)
                {
                    if (countEdit == 2)
                    {
                        tempCheckTinh = text;
                    }
                    else if (countEdit == 3)
                    {
                        tempCheckLoai = text;
                    }
                    else if (countEdit == 4)
                    {
                        tempCheckThuyBo = text;
                        break;
                    }
                    countEdit++;
                }
            }
            if (tempCheckTinh.IndexOf(currentChuyenThu.CheckTinh) != -1 && tempCheckLoai.IndexOf(currentChuyenThu.CheckLoai) != -1 && tempCheckThuyBo.IndexOf(currentChuyenThu.CheckThuyBo) != -1)
            {
                SendKeys.SendWait("A{BS}{BS}");
                Thread.Sleep(1000);
                SoundManager.playSound(@"\music\" + currentChuyenThu.NameMusic + ".wav");
            }
            else
            {
                //Kiem tra lai
            }
        }

    }
}
