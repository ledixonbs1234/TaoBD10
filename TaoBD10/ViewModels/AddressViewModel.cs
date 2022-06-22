using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TaoBD10.ViewModels
{
    public class AddressViewModel : ObservableObject
    {
        public AddressViewModel()
        {
            HangHoas = new ObservableCollection<HangHoaDetailModel>();
            LoaiAddress = new ObservableCollection<string>();
            LoaiAddress.Add("None");
            LoaiAddress.Add("TamQuan");
            LoaiAddress.Add("ChuaXacDinh");

            LayDanhSachCommand = new RelayCommand(LayDanhSach);
            LocCommand = new RelayCommand(Loc);
            LayDiaChiCommand = new RelayCommand(LayDiaChi);
            SendDataCommand = new RelayCommand(SendData);

            try
            {
                client = new MqttClient("broker.hivemq.com");
                _clientId = Guid.NewGuid().ToString();
                client.Connect(_clientId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

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
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "IsTamQuan")
                {
                    if (m.Content == "Update")
                    {
                        SetCountTamQuan();
                    }
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
                    hangHoa.AddressSend = m.AddressSend.Trim();
                    //thuc hien kiem tra tam quan
                    if (!string.IsNullOrEmpty(hangHoa.Address))
                    {
                        foreach (var fill in fillTamQuan)
                        {
                            if (APIManager.ConvertToUnSign3(hangHoa.Address).ToLower().IndexOf(fill) != -1)
                            {
                                hangHoa.IsTamQuan = "TamQuan";
                                SetCountTamQuan();
                                break;
                            }
                        }
                    }
                    LayDiaChi();
                }
            });
        }

        private void LayDanhSach()
        {
            //thuc hien lay danh sach trong nay
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "LayHangHoa" });
        }

        private void LayDiaChi()
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

        private void Loc()
        {
            //thuc hien loc danh sach tu bd8
            if (HangHoas.Count == 0)
                return;
            bool isLast = false;

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

        private void SendData()
        {
            string dataSend = "";
            foreach (var item in HangHoas)
            {
                if (item.IsTamQuan != "None")
                {
                    if (item.IsTamQuan == "TamQuan")
                    {
                        dataSend += item.TuiHangHoa.SHTui + "|" + item.TuiHangHoa.ToBC + "\n";
                    }
                    else if (item.IsTamQuan == "ChuaXacDinh")
                    {
                        dataSend += item.TuiHangHoa.SHTui + "|chuaxacdinh\n";
                    }
                }
            }
            client.Publish("tamquanget1", Encoding.UTF8.GetBytes(dataSend), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
        }

        private void SetCountTamQuan()
        {
            var data = HangHoas.Where(m => m.IsTamQuan != "None");
            if (data != null)
            {
                CountTamQuan = data.Count();
            }
        }

        public int CountTamQuan
        {
            get { return _CountTamQuan; }
            set { SetProperty(ref _CountTamQuan, value); }
        }

        public ObservableCollection<HangHoaDetailModel> HangHoas
        {
            get { return _HangHoas; }
            set { SetProperty(ref _HangHoas, value); }
        }

        public ICommand LayDanhSachCommand { get; }
        public ICommand LayDiaChiCommand { get; }
        public ObservableCollection<string> LoaiAddress
        {
            get { return _LoaiAddress; }
            set { SetProperty(ref _LoaiAddress, value); }
        }

        public ICommand LocCommand { get; }
        public ICommand SendDataCommand { get; }
        private readonly MqttClient client;
        private string _clientId;
        private int _CountTamQuan;
        private ObservableCollection<HangHoaDetailModel> _HangHoas;
        private ObservableCollection<string> _LoaiAddress;
    }
}