using System;
using System.Runtime.InteropServices;

namespace Mono.Audio
{
	internal class AlsaDevice : AudioDevice, IDisposable
	{
		[DllImport("libasound")]
		private static extern int snd_pcm_open(ref IntPtr handle, string pcm_name, int stream, int mode);

		[DllImport("libasound")]
		private static extern int snd_pcm_close(IntPtr handle);

		[DllImport("libasound")]
		private static extern int snd_pcm_drain(IntPtr handle);

		[DllImport("libasound")]
		private static extern int snd_pcm_writei(IntPtr handle, byte[] buf, int size);

		[DllImport("libasound")]
		private static extern int snd_pcm_set_params(IntPtr handle, int format, int access, int channels, int rate, int soft_resample, int latency);

		[DllImport("libasound")]
		private static extern int snd_pcm_state(IntPtr handle);

		[DllImport("libasound")]
		private static extern int snd_pcm_prepare(IntPtr handle);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params(IntPtr handle, IntPtr param);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_malloc(ref IntPtr param);

		[DllImport("libasound")]
		private static extern void snd_pcm_hw_params_free(IntPtr param);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_any(IntPtr handle, IntPtr param);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_set_access(IntPtr handle, IntPtr param, int access);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_set_format(IntPtr handle, IntPtr param, int format);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_set_channels(IntPtr handle, IntPtr param, uint channel);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_set_rate_near(IntPtr handle, IntPtr param, ref uint rate, ref int dir);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_set_period_time_near(IntPtr handle, IntPtr param, ref uint period, ref int dir);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_get_period_size(IntPtr param, ref uint period, ref int dir);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_set_buffer_size_near(IntPtr handle, IntPtr param, ref uint buff_size);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_get_buffer_time_max(IntPtr param, ref uint buffer_time, ref int dir);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_set_buffer_time_near(IntPtr handle, IntPtr param, ref uint BufferTime, ref int dir);

		[DllImport("libasound")]
		private static extern int snd_pcm_hw_params_get_buffer_size(IntPtr param, ref uint BufferSize);

		[DllImport("libasound")]
		private static extern int snd_pcm_sw_params(IntPtr handle, IntPtr param);

		[DllImport("libasound")]
		private static extern int snd_pcm_sw_params_malloc(ref IntPtr param);

		[DllImport("libasound")]
		private static extern void snd_pcm_sw_params_free(IntPtr param);

		[DllImport("libasound")]
		private static extern int snd_pcm_sw_params_current(IntPtr handle, IntPtr param);

		[DllImport("libasound")]
		private static extern int snd_pcm_sw_params_set_avail_min(IntPtr handle, IntPtr param, uint frames);

		[DllImport("libasound")]
		private static extern int snd_pcm_sw_params_set_start_threshold(IntPtr handle, IntPtr param, uint StartThreshold);

		public AlsaDevice(string name)
		{
			if (name == null)
			{
				name = "default";
			}
			int num = AlsaDevice.snd_pcm_open(ref this.handle, name, 0, 0);
			if (num < 0)
			{
				throw new Exception("no open " + num.ToString());
			}
		}

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
			if (this.sw_param != IntPtr.Zero)
			{
				AlsaDevice.snd_pcm_sw_params_free(this.sw_param);
			}
			if (this.hw_param != IntPtr.Zero)
			{
				AlsaDevice.snd_pcm_hw_params_free(this.hw_param);
			}
			if (this.handle != IntPtr.Zero)
			{
				AlsaDevice.snd_pcm_close(this.handle);
			}
			this.sw_param = IntPtr.Zero;
			this.hw_param = IntPtr.Zero;
			this.handle = IntPtr.Zero;
		}

		public override bool SetFormat(AudioFormat format, int channels, int rate)
		{
			uint num = 0U;
			uint num2 = 0U;
			uint num3 = 0U;
			uint num4 = 0U;
			int num5 = 0;
			uint num6 = (uint)rate;
			if (AlsaDevice.snd_pcm_hw_params_malloc(ref this.hw_param) == 0)
			{
				AlsaDevice.snd_pcm_hw_params_any(this.handle, this.hw_param);
				AlsaDevice.snd_pcm_hw_params_set_access(this.handle, this.hw_param, 3);
				AlsaDevice.snd_pcm_hw_params_set_format(this.handle, this.hw_param, (int)format);
				AlsaDevice.snd_pcm_hw_params_set_channels(this.handle, this.hw_param, (uint)channels);
				num5 = 0;
				AlsaDevice.snd_pcm_hw_params_set_rate_near(this.handle, this.hw_param, ref num6, ref num5);
				num5 = 0;
				AlsaDevice.snd_pcm_hw_params_get_buffer_time_max(this.hw_param, ref num4, ref num5);
				if (num4 > 500000U)
				{
					num4 = 500000U;
				}
				if (num4 > 0U)
				{
					num = num4 / 4U;
				}
				num5 = 0;
				AlsaDevice.snd_pcm_hw_params_set_period_time_near(this.handle, this.hw_param, ref num, ref num5);
				num5 = 0;
				AlsaDevice.snd_pcm_hw_params_set_buffer_time_near(this.handle, this.hw_param, ref num4, ref num5);
				AlsaDevice.snd_pcm_hw_params_get_period_size(this.hw_param, ref num2, ref num5);
				this.chunk_size = num2;
				AlsaDevice.snd_pcm_hw_params_get_buffer_size(this.hw_param, ref num3);
				AlsaDevice.snd_pcm_hw_params(this.handle, this.hw_param);
			}
			else
			{
				Console.WriteLine("failed to alloc Alsa hw param struct");
			}
			int num7 = AlsaDevice.snd_pcm_sw_params_malloc(ref this.sw_param);
			if (num7 == 0)
			{
				AlsaDevice.snd_pcm_sw_params_current(this.handle, this.sw_param);
				AlsaDevice.snd_pcm_sw_params_set_avail_min(this.handle, this.sw_param, this.chunk_size);
				AlsaDevice.snd_pcm_sw_params_set_start_threshold(this.handle, this.sw_param, num3);
				AlsaDevice.snd_pcm_sw_params(this.handle, this.sw_param);
			}
			else
			{
				Console.WriteLine("failed to alloc Alsa sw param struct");
			}
			if (this.hw_param != IntPtr.Zero)
			{
				AlsaDevice.snd_pcm_hw_params_free(this.hw_param);
				this.hw_param = IntPtr.Zero;
			}
			if (this.sw_param != IntPtr.Zero)
			{
				AlsaDevice.snd_pcm_sw_params_free(this.sw_param);
				this.sw_param = IntPtr.Zero;
			}
			return num7 == 0;
		}

		public override int PlaySample(byte[] buffer, int num_frames)
		{
			int num;
			do
			{
				num = AlsaDevice.snd_pcm_writei(this.handle, buffer, num_frames);
				if (num < 0)
				{
					this.XRunRecovery(num);
				}
			}
			while (num < 0);
			return num;
		}

		public override int XRunRecovery(int err)
		{
			int num = 0;
			if (-32 == err)
			{
				num = AlsaDevice.snd_pcm_prepare(this.handle);
			}
			return num;
		}

		public override void Wait()
		{
			AlsaDevice.snd_pcm_drain(this.handle);
		}

		private IntPtr handle;

		private IntPtr hw_param;

		private IntPtr sw_param;
	}
}
