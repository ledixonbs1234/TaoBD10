using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class KTChuaPhatViewModel : ObservableObject
    {

        private ObservableCollection<HangTonModel> _HangTons;

        public ObservableCollection<HangTonModel> HangTons
        {
            get { return _HangTons; }
            set { SetProperty(ref _HangTons, value); }
        }

        public KTChuaPhatViewModel()
        {

            Run593280Command = new RelayCommand(Run593280);
            Run593230Command = new RelayCommand(Run593230);
            CheckCommand = new RelayCommand(Check);
            HangTons = new ObservableCollection<HangTonModel>();

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) => {
                if(m.Key == "RChuaPhat")
                {
                    if (m.Content == "InfoOK")
                    {
                        //neu thuc hien thanh cong thi vao trong nay
                        IsOk = "OK";
                    }
                }
            });

            WeakReferenceMessenger.Default.Register<HangTonMessage>(this, (r, m) => { 
                if(m.Value != null)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        HangTons.Clear();
                        foreach (HangTonModel item in m.Value)
                        {
                            HangTons.Add(item);
                        }

                    });
                   
                }

            
            });
        }

        //thuc hien lenh trong nay
        public ICommand Run593230Command { get; }


        public ICommand Run593280Command { get; }
        private ChuaPhat currentChuaPhat = ChuaPhat.C593280;

        public ICommand CheckCommand { get; }

        private string _IsOk;

        public string IsOk
        {
            get { return _IsOk; }
            set { SetProperty(ref _IsOk, value); }
        }



        void Check()
        {
            IsOk = "Checking...";
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "LoadUrl" });
        }


        void Run593230()
        {
            currentChuaPhat = ChuaPhat.C593230;
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "Run230" });

            /*
            Quy trinh thuc hien nhu sau
            vao trang web do
            //kiem tra thu ip address co thay doi gi khong
            //neu co thi tu dong dang nhap 
            sau do kiem tra thu phai default khong
            neu dung thi thuc hien cac cong viec binh thuong
            neu chua thi tu dang nhap

             */
        }

        void Run593280()
        {
            currentChuaPhat = ChuaPhat.C593280;
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "Run280" });

        }
    }
}
