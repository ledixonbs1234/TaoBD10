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
using TaoBD10.Manager;
using TaoBD10.Model;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TaoBD10.ViewModels
{
    public class AddressViewModel : ObservableObject
    {
        private ObservableCollection<HangHoaDetailModel> _HangHoas;
        readonly MqttClient client;

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

        public ICommand SendDataCommand { get; }


        void SendData()
        {
            string dataSend = "";
            foreach (var item in HangHoas)
            {
                if (item.IsTamQuan)
                {
                    dataSend += item.TuiHangHoa.SHTui + "\n";
                }
            }
            client.Publish("tamquanget", Encoding.UTF8.GetBytes(dataSend), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
        }
        string _clientId;




        public AddressViewModel()
        {
            HangHoas = new ObservableCollection<HangHoaDetailModel>();

            LayDanhSachCommand = new RelayCommand(LayDanhSach);
            LocCommand = new RelayCommand(Loc);
            LayDiaChiCommand = new RelayCommand(LayDiaChi);
            SendDataCommand = new RelayCommand(SendData);

            client = new MqttClient("broker.hivemq.com");
            _clientId = Guid.NewGuid().ToString();
            client.Connect(_clientId);

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
            string[] fillTamQuan = { "tam quan", "hoai son", "hoai chau", "hoai hao", "hoai phu", "hoai thanh" };
            WeakReferenceMessenger.Default.Register<WebContentModel>(this, (r, m) =>
            {
                if (m.Key != "AddressTamQuan")
                    return;
                HangHoaDetailModel hangHoa = HangHoas.FirstOrDefault(c => m.Code.ToUpper().IndexOf(c.Code.ToUpper()) != -1);
                if (hangHoa != null)
                {
                    hangHoa.Address = m.AddressReiceive.Trim();
                    //thuc hien kiem tra tam quan
                    if (!string.IsNullOrEmpty(hangHoa.Address))
                    {
                        foreach (var fill in fillTamQuan)
                        {
                            if (APIManager.convertToUnSign3(hangHoa.Address).ToLower().IndexOf(fill) != -1)
                            {
                                hangHoa.IsTamQuan = true;
                                SetCountTamQuan();
                                break;
                            }
                        }
                    }
                    LayDiaChi();
                }
            });
        }

        void SetCountTamQuan()
        {
            var data = HangHoas.Where(m => m.IsTamQuan);
            if (data != null)
            {
                CountTamQuan = data.Count();
            }
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
