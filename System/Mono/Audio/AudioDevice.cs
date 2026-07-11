using System;

namespace Mono.Audio
{
	internal class AudioDevice
	{
		private static AudioDevice TryAlsa(string name)
		{
			AudioDevice audioDevice2;
			try
			{
				AudioDevice audioDevice = new AlsaDevice(name);
				audioDevice2 = audioDevice;
			}
			catch
			{
				audioDevice2 = null;
			}
			return audioDevice2;
		}

		public static AudioDevice CreateDevice(string name)
		{
			AudioDevice audioDevice = AudioDevice.TryAlsa(name);
			if (audioDevice == null)
			{
				audioDevice = new AudioDevice();
			}
			return audioDevice;
		}

		public virtual bool SetFormat(AudioFormat format, int channels, int rate)
		{
			return true;
		}

		public virtual int PlaySample(byte[] buffer, int num_frames)
		{
			return num_frames;
		}

		public virtual void Wait()
		{
		}
	}
}
