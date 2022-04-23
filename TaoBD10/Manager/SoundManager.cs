using System.IO;
using System.Media;

namespace TaoBD10.Manager
{
    public static class SoundManager
    {
        private static SoundPlayer player;
        private static SoundPlayer playerSync;
        private static string dir = "";

        public static void SetUpDirectory()
        {
            dir = Directory.GetCurrentDirectory();
            player = new SoundPlayer();
            playerSync = new SoundPlayer();
        }

        public static void playSound(string path)
        {
            player.SoundLocation = dir + @"\" + path;
            player.Play();
        }

        public static void playSync(string path)
        {
            playerSync.SoundLocation = dir + @"\" + path;
            playerSync.PlaySync();
        }
    }
}