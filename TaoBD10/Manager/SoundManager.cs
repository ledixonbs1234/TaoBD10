using System;
using System.IO;
using System.Media;

namespace TaoBD10.Manager
{
    public static class SoundManager
    {
        private static SoundPlayer player;
        private static SoundPlayer playerSync;
        private static string dir = "";
        private static System.Windows.Media.MediaPlayer p2;

        public static void SetUpDirectory()
        {
            dir = Directory.GetCurrentDirectory();
            player = new SoundPlayer();
            playerSync = new SoundPlayer();
            p2 = new System.Windows.Media.MediaPlayer();
        }

        public static void playSound(string path)
        {
            //player.SoundLocation = dir + @"\" + path;
            //player.Play();
            try
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    player.Stop();
                    player.SoundLocation = dir + @"\" + path;
                    player.Play();
                }));
            }
            catch (System.Exception e)
            {
                APIManager.ShowSnackbar(e.Message + " " + path);
            }
        }

        public static void playSound2(string path)
        {
            //player.SoundLocation = dir + @"\" + path;
            //player.Play();
            try
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    p2.Stop();
                    p2.Open(new Uri(dir + @"\" + path));
                    p2.Play();
                }));
            }
            catch (System.Exception e)
            {
                APIManager.ShowSnackbar(e.Message + " " + path);
            }
        }

        public static void playSync(string path)
        {
            playerSync.SoundLocation = dir + @"\" + path;
            playerSync.PlaySync();
        }
    }
}