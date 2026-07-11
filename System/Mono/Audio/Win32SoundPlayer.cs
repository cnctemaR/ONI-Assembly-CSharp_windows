using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mono.Audio
{
	internal class Win32SoundPlayer : IDisposable
	{
		public Win32SoundPlayer(Stream s)
		{
			if (s != null)
			{
				this._buffer = new byte[s.Length];
				s.Read(this._buffer, 0, this._buffer.Length);
				return;
			}
			this._buffer = new byte[0];
		}

		[DllImport("winmm.dll", SetLastError = true)]
		private static extern bool PlaySound(byte[] ptrToSound, UIntPtr hmod, Win32SoundPlayer.SoundFlags flags);

		public Stream Stream
		{
			set
			{
				this.Stop();
				if (value != null)
				{
					this._buffer = new byte[value.Length];
					value.Read(this._buffer, 0, this._buffer.Length);
					return;
				}
				this._buffer = new byte[0];
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Win32SoundPlayer()
		{
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				this.Stop();
				this._disposed = true;
			}
		}

		public void Play()
		{
			Win32SoundPlayer.PlaySound(this._buffer, UIntPtr.Zero, (Win32SoundPlayer.SoundFlags)5U);
		}

		public void PlayLooping()
		{
			Win32SoundPlayer.PlaySound(this._buffer, UIntPtr.Zero, (Win32SoundPlayer.SoundFlags)13U);
		}

		public void PlaySync()
		{
			Win32SoundPlayer.PlaySound(this._buffer, UIntPtr.Zero, (Win32SoundPlayer.SoundFlags)6U);
		}

		public void Stop()
		{
			Win32SoundPlayer.PlaySound(null, UIntPtr.Zero, Win32SoundPlayer.SoundFlags.SND_SYNC);
		}

		private byte[] _buffer;

		private bool _disposed;

		private enum SoundFlags : uint
		{
			SND_SYNC,
			SND_ASYNC,
			SND_NODEFAULT,
			SND_MEMORY = 4U,
			SND_LOOP = 8U,
			SND_FILENAME = 131072U
		}
	}
}
