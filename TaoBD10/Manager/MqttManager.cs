using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Text;
using System.Windows;
using TaoBD10.Model;
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
            if (e.Topic == FileManager.FirebaseKey)
            {
                IsConnected = true;
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "CreateListKeyMQTT" });
                Pulish(FileManager.FirebaseKey + "_phone", data);
                Subcribe(FileManager.FirebaseKey + "_datatopc");
            }
            //   }else if (datas[0] == "chinhsualai")
            //    {
            //        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "ToDiNgoai_ChinhSuaLai", Content = datas[1] });
            //    }
            //    else if (datas[0] == "sapxepdingoai")
            //    {
            //        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "ToDiNgoai_SapXepDiNgoai", Content = datas[1] });
            //    }
            //}
        }

        private static void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
        }

        public static void SendMessageToPhone(string message)
        {
            Pulish(FileManager.FirebaseKey + "_message", message);
        }

        public static void Pulish(string topic, string message, bool isRetain = false)
        {
            if (!isRetain)
                client.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
            else
                client.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
        }

        public static void Subcribe(string topic)
        {
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        public static void Subcribe(string[] topics)
        {
            client.Subscribe(topics, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        public static void checkConnect()
        {
            var connected = client.IsConnected;
            if (!connected)
            {
                try
                {
                    client.Connect(_clientId);
                }
                catch
                {
                }
            }
        }
    }
}