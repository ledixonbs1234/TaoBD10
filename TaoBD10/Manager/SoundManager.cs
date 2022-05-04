using System.IO;
using System.Media;

namespace TaoBD10.Manager
{
    public static class SoundManager
    {
        private static SoundPlayer player;
        private static SoundPlayer playerSync;
        private static string dir = "";
        static System.Windows.Media.MediaPlayer p1;

        public static void SetUpDirectory()
        {
            dir = Directory.GetCurrentDirectory();
            player = new SoundPlayer();
            playerSync = new SoundPlayer();
            p1 = new System.Windows.Media.MediaPlayer();
            
        }

        public static void playSound(string path)
        {
            //player.SoundLocation = dir + @"\" + path;
            //player.Play();
            p1.Open(new System.Uri(dir + @"\" + path));
            p1.Play();
        }

        public static void playSync(string path)
        {
            playerSync.SoundLocation = dir + @"\" + path;
            playerSync.PlaySync();
        }
    }
}