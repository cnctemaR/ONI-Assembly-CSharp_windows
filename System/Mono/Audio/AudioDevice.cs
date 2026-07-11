using System;

namespace Mono.Audio
{
	internal class AudioDevice
	{
		private static AudioDevice TryAlsa(string name)
		{
			AudioDevice audioDevice;
			try
			{
				audioDevice = new AlsaDevice(name);
			}
			catch
			{
				audioDevice = null;
			}
			return audioDevice;
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

		public virtual int XRunRecovery(int err)
		{
			return err;
		}

		public virtual void Wait()
		{
		}

		public uint ChunkSize
		{
			get
			{
				return this.chunk_size;
			}
		}

		protected uint chunk_size;
	}
}
