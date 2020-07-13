using System.Media;

namespace SimpleEngine.Utils
{ 
    /// <summary>
    /// Class for making sound in a different thread.
    /// </summary>
    public static class Sounder
    { 
        /// <summary>
        /// Plays .wav file in an asynchronous thread
        /// </summary>
        /// <param name="soundFile">path to the .wav file</param>
        public static void PlaySound(string soundFile)
        {
            var player = new SoundPlayer(soundFile);
            player.Play();
            player.Dispose();
        }

    }
}
