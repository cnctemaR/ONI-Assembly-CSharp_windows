using System;
using System.Runtime.InteropServices;

namespace Mono.Audio
{
	internal class AlsaDevice : AudioDevice, IDisposable
	{
		public AlsaDevice(string name)
		{
			if (name == null)
			{
				name = "default";
			}
			int num = AlsaDevice.snd_pcm_open(ref this.handle, name, 0, 0);
			if (num < 0)
			{
				throw new Exception("no open " + num);
			}
		}

		[DllImport("libasound.so.2")]
		private static extern int snd_pcm_open(ref IntPtr handle, string pcm_name, int stream, int mode);

		[DllImport("libasound.so.2")]
		private static extern int snd_pcm_close(IntPtr handle);

		[DllImport("libasound.so.2")]
		private static extern int snd_pcm_drain(IntPtr handle);

		[DllImport("libasound.so.2")]
		private static extern int snd_pcm_writei(IntPtr handle, byte[] buf, int size);

		[DllImport("libasound.so.2")]
		private static extern int snd_pcm_set_params(IntPtr handle, int format, int access, int channels, int rate, int soft_resample, int latency);

		~AlsaDevice()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			if (this.handle != IntPtr.Zero)
			{
				AlsaDevice.snd_pcm_close(this.handle);
			}
			this.handle = IntPtr.Zero;
		}

		public override bool SetFormat(AudioFormat format, int channels, int rate)
		{
			int num = AlsaDevice.snd_pcm_set_params(this.handle, (int)format, 3, channels, rate, 1, 500000);
			return num == 0;
		}

		public override int PlaySample(byte[] buffer, int num_frames)
		{
			return AlsaDevice.snd_pcm_writei(this.handle, buffer, num_frames);
		}

		public override void Wait()
		{
			AlsaDevice.snd_pcm_drain(this.handle);
		}

		private IntPtr handle;
	}
}
