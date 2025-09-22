using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using NAudio.Wave;

namespace Collection.Model
{
    class Player
    {
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;
        public static bool IsPlaying = false;

        static public void init()
        {
            outputDevice = new WaveOutEvent();

            outputDevice.PlaybackStopped += (_, _) =>
            {
                if(outputDevice.PlaybackState != PlaybackState.Playing)
                    Collection.Global_var.CollectorPageInstance.IsPlayerWorking = false;
            };
        }

        static public void Play(string Path, float Volum = 1)
        {

            outputDevice?.Stop();
            audioFile?.Dispose();

            Collection.Global_var.CollectorPageInstance.IsPlayerWorking = true;

            audioFile = new AudioFileReader(Path);

            audioFile.Volume = Volum;

            outputDevice.Init(audioFile);
            outputDevice.Play();
        }
        static public void stop()
        {
            outputDevice?.Stop();
            audioFile?.Dispose();
        }

        static public void Dispose() 
        {
            outputDevice?.Stop();
            audioFile?.Dispose();
            outputDevice?.Dispose();
        }

        static public void Stop()
        {
            outputDevice?.Stop();
            audioFile?.Dispose();
        }
        static public void Suspend()
        {
            outputDevice?.Pause();
        }

        static public void Continue()
        {
            outputDevice?.Play();
        }
        public static void SetVolume(float volume) // 0.0 到 1.0
        {
            if (audioFile != null)
                audioFile.Volume = volume;
        }
    }
}
