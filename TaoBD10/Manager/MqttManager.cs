using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaoBD10.Model;
using TaoBD10.ViewModels;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TaoBD10.Manager
{
    public static class MqttManager
    {

        public static string _clientId;
        public static MqttClient client;
        public static bool IsConnected = false;
        public static void Connect()
        {
            try
            {
                client = new MqttClient("broker.hivemq.com");
                _clientId = Guid.NewGuid().ToString();
                client.Connect(_clientId);
                client.MqttMsgSubscribed += Client_MqttMsgSubscribed;
                client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string data = Encoding.UTF8.GetString(e.Message);
            if (e.Topic == FileManager.MQTTKEY)
            {
                APIManager.ShowSnackbar("Connected");
                IsConnected = true;
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "CreateListKeyMQTT" });
                Pulish(FileManager.MQTTKEY + "_phone", data);
                Subcribe(FileManager.MQTTKEY + "_datatopc");
            }
            else if (e.Topic == FileManager.MQTTKEY + "_control")
            {
                if (data == "anmy")
                {
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Button",Content="AnMy" });
                }
                else if (data == "hoaian")
                {
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Button",Content="HoaiAn" });
                }
                else if (data == "anlao")
                {
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Button",Content="AnLao" });
                }
                else if (data == "anhoa")
                {
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Button",Content="AnHoa" });
                }
            }
            else if (e.Topic == FileManager.MQTTKEY + "_datatopc")
            {
                string[] datas= data.Split('|');
                if (datas[0] == "laybd")
                {
                    //Thuc hien lay bd sang trua chieu toi
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "BD10BUOI", Content = "Toi" });
                    var window = APIManager.GetActiveWindowTitle();
                    if(window == null)
                    {
                        return;
                    }
                    if (window.text != "danh sach bd10 den")
                    {
                        return;
                    }
                    //var list = APIManager.GetListControlText(window.hwnd);
                    //var control = list.FirstOrDefault(m => m.Text.IndexOf("xac nhan")!= -1);
                    //if(control == null)
                    //{
                    //    return;
                    //}
                    APIManager.ClickButton(window.hwnd,"xac nhan",isExactly:false);
                    APIManager.WaitingFindedWindow("xac nhan bd10 den");
                }else if (datas[0] == "laydanhsachbd")
                {
                    //Thuc hien xu ly lay danh sach bd
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "ToLayBDHA_LayDanhSach" });

                }

            }
        }

        private static void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {

        }

        public static void Pulish(string topic, string message)
        {
            client.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        public static void Subcribe(string topic)
        {
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }
        public static void Subcribe(string[] topics)
        {
            client.Subscribe(topics, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }
    }
}
