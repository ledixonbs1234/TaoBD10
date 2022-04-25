using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class LocTuiViewModel : ObservableObject
    {
        private ObservableCollection<TuiHangHoa> _ListHangHoa;

        public ObservableCollection<TuiHangHoa> ListHangHoa
        {
            get { return _ListHangHoa; }
            set { SetProperty(ref _ListHangHoa, value); }
        }

        private string _NameBD;

        public string NameBD
        {
            get { return _NameBD; }
            set { SetProperty(ref _NameBD, value);
                CapNhatCommand.NotifyCanExecuteChanged();
            }
        }

        public IRelayCommand CapNhatCommand { get; }


        void CapNhat()
        {
            FileManager.SaveData(new BD10InfoModel(NameBD, ListHangHoa.ToList(), DateTime.Now, EnumAll.TimeSet.Sang, "1"));
            WeakReferenceMessenger.Default.Send<string>("LoadBD10");
            MessageShow("Đã Tạo BĐ 10 với tên : " + NameBD);
        }

        void MessageShow(string content)
        {
            if (MessageQueue == null)
                MessageQueue = new SnackbarMessageQueue();
            MessageQueue.Enqueue(content, null, null, null, false, false, TimeSpan.FromSeconds(3));

        }

        private SnackbarMessageQueue _MessageQueue;

        public SnackbarMessageQueue MessageQueue
        {
            get { return _MessageQueue; }
            set { SetProperty(ref _MessageQueue, value); }
        }



        private string _TextBD;

        public string TextBD
        {
            get { return _TextBD; }
            set
            {
                if (value != _TextBD)
                {
                    SetProperty(ref _TextBD, value);
                    //Kiem tra thu Text trong nay
                    CheckEnterKey();
                }
            }
        }


        private void CheckEnterKey()
        {
            if (TextBD.IndexOf('\n') != -1)
            {
                TextBD = TextBD.Trim();
                if (TextBD.Length != 29)
                {
                    TextBD = "";
                    return;
                }
                //kiem tra trung khong
                if (ListHangHoa.Count == 0)
                {
                    ListHangHoa.Add(new TuiHangHoa((ListHangHoa.Count + 1).ToString(), TextBD));
                    TextBD = "";
                }
                else
                {
                    foreach (TuiHangHoa item in ListHangHoa)
                    {
                        if (item.SHTui == TextBD)
                        {
                            TextBD = "";
                            return;
                        }
                    }
                    ListHangHoa.Add(new TuiHangHoa((ListHangHoa.Count + 1).ToString(), TextBD));
                    TextBD = "";
                }
            }
        }

        public LocTuiViewModel()
        {
            ListHangHoa = new ObservableCollection<TuiHangHoa>();
            CapNhatCommand = new RelayCommand(CapNhat,()=> {
                if (!string.IsNullOrEmpty(NameBD)&& ListHangHoa.Count != 0)
                {
                    return true;
                }else
                {
                    return false;
                }
            });

        }


    }
}
