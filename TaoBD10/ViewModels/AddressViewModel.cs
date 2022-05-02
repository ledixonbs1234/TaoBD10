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

namespace TaoBD10.ViewModels
{
    public class AddressViewModel : ObservableObject
    {
        private ObservableCollection<HangHoaDetailModel> _HangHoas;

        public ObservableCollection<HangHoaDetailModel> HangHoas
        {
            get { return _HangHoas; }
            set { SetProperty(ref _HangHoas, value); }
        }

        private int _CountTamQuan;

        public int CountTamQuan
        {
            get { return _CountTamQuan; }
            set { SetProperty(ref _CountTamQuan, value); }
        }




        public AddressViewModel()
        {
            HangHoas = new ObservableCollection<HangHoaDetailModel>();

            LayDanhSachCommand = new RelayCommand(LayDanhSach);
            LocCommand = new RelayCommand(Loc);
            LayDiaChiCommand = new RelayCommand(LayDiaChi);
            WeakReferenceMessenger.Default.Register<TuiHangHoaMessage>(this, (r, m) =>
            {
                if (m.Value == null)
                    return;
                HangHoas.Clear();
                foreach (HangHoaDetailModel item in m.Value)
                {
                    HangHoas.Add(item);
                }
            });

            WeakReferenceMessenger.Default.Register<SHTuiMessage>(this, (r, m) =>
            {
                if (m.Value == null)
                    return;
                if (m.Value.Key == "ReturnSHTui")
                {
                    foreach (HangHoaDetailModel item in HangHoas)
                    {
                        if (m.Value.SHTui.ToLower() == item.TuiHangHoa.SHTui.ToLower())
                        {
                            item.Code = m.Value.Code.ToUpper();
                            break;
                        }
                    }
                    Loc();

                }
            });

            WeakReferenceMessenger.Default.Register<WebContentModel>(this, (r, m) =>
            {
                if (m.Key != "AddressTamQuan")
                    return;
                HangHoaDetailModel hangHoa = HangHoas.FirstOrDefault(c => m.Code.ToUpper().IndexOf(c.Code.ToUpper()) != -1);
                if (hangHoa != null)
                {
                    hangHoa.Address = m.AddressReiceive.Trim();
                    LayDiaChi();
                }
            });

        }

        public ICommand LayDanhSachCommand { get; }
        public ICommand LocCommand { get; }
        public ICommand LayDiaChiCommand { get; }

        void LayDiaChi()
        {
            if (HangHoas.Count == 0)
                return;

            foreach (HangHoaDetailModel diNgoaiItem in HangHoas)
            {
                if (!string.IsNullOrEmpty(diNgoaiItem.Code))
                {
                    if (diNgoaiItem.Code.Length != 13)
                        continue;
                    if (!string.IsNullOrEmpty(diNgoaiItem.Address))
                        continue;

                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "LoadAddressTQWeb", Content = diNgoaiItem.Code });
                    break;
                }
            }

        }
        void Loc()
        {
            //thuc hien loc danh sach tu bd8
            if (HangHoas.Count == 0)
                return;

            foreach (HangHoaDetailModel diNgoaiItem in HangHoas)
            {
                if (!string.IsNullOrEmpty(diNgoaiItem.TuiHangHoa.SHTui))
                {
                    if (!string.IsNullOrEmpty(diNgoaiItem.Code))
                        continue;
                    if (diNgoaiItem.TuiHangHoa.SHTui.Length != 13)
                    {
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "GetCodeFromBD", Content = diNgoaiItem.TuiHangHoa.SHTui });
                        break;
                    }
                    else
                    {
                        //thuc hien trong nay
                        diNgoaiItem.Code = diNgoaiItem.TuiHangHoa.SHTui;
                    }
                }
            }

        }



        void LayDanhSach()
        {
            //thuc hien lay danh sach trong nay
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "LayHangHoa" });

        }

    }
}
