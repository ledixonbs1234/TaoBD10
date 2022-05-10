using System.IO;
using System.Media;
using System.Windows;

namespace TaoBD10.Manager
{
    public static class SoundManager
    {
        private static SoundPlayer player;
        private static SoundPlayer playerSync;
        private static string dir = "";
        private static System.Windows.Media.MediaPlayer p1;
        private static System.Windows.Media.MediaPlayer p2;

        public static void SetUpDirectory()
        {
            dir = Directory.GetCurrentDirectory();
            player = new SoundPlayer();
            playerSync = new SoundPlayer();
            p1 = new System.Windows.Media.MediaPlayer();
            p2 = new System.Windows.Media.MediaPlayer();
        }

        public static void playSound(string path)
        {
            //player.SoundLocation = dir + @"\" + path;
            //player.Play();
            try
            {
                p1.Open(new System.Uri(dir + @"\" + path));
                p1.Play();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message + " " + path);
            }
        }

        public static void playSound2(string path)
        {
            //player.SoundLocation = dir + @"\" + path;
            //player.Play();
            try
            {
                p2.Open(new System.Uri(dir + @"\" + path));
                p2.Play();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message + " " + path);
            }
        }

        public static void playSync(string path)
        {
            playerSync.SoundLocation = dir + @"\" + path;
            playerSync.PlaySync();
        }
    }
}