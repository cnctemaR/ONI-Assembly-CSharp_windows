using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace System.Drawing
{
	internal sealed class ComIStreamWrapper : IStream
	{
		internal ComIStreamWrapper(Stream stream)
		{
			this.baseStream = stream;
		}

		private void SetSizeToPosition()
		{
			if (this.position != -1L)
			{
				if (this.position > this.baseStream.Length)
				{
					this.baseStream.SetLength(this.position);
				}
				this.baseStream.Position = this.position;
				this.position = -1L;
			}
		}

		public void Read(byte[] pv, int cb, IntPtr pcbRead)
		{
			int num = 0;
			if (cb != 0)
			{
				this.SetSizeToPosition();
				num = this.baseStream.Read(pv, 0, cb);
			}
			if (pcbRead != IntPtr.Zero)
			{
				Marshal.WriteInt32(pcbRead, num);
			}
		}

		public void Write(byte[] pv, int cb, IntPtr pcbWritten)
		{
			if (cb != 0)
			{
				this.SetSizeToPosition();
				this.baseStream.Write(pv, 0, cb);
			}
			if (pcbWritten != IntPtr.Zero)
			{
				Marshal.WriteInt32(pcbWritten, cb);
			}
		}

		public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
		{
			long length = this.baseStream.Length;
			long num;
			switch (dwOrigin)
			{
			case 0:
				num = dlibMove;
				break;
			case 1:
				if (this.position == -1L)
				{
					num = this.baseStream.Position + dlibMove;
				}
				else
				{
					num = this.position + dlibMove;
				}
				break;
			case 2:
				num = length + dlibMove;
				break;
			default:
				throw new ExternalException(null, -2147287039);
			}
			if (num > length)
			{
				this.position = num;
			}
			else
			{
				this.baseStream.Position = num;
				this.position = -1L;
			}
			if (plibNewPosition != IntPtr.Zero)
			{
				Marshal.WriteInt64(plibNewPosition, num);
			}
		}

		public void SetSize(long libNewSize)
		{
			this.baseStream.SetLength(libNewSize);
		}

		public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
		{
			long num = 0L;
			if (cb != 0L)
			{
				int num2;
				if (cb < 4096L)
				{
					num2 = (int)cb;
				}
				else
				{
					num2 = 4096;
				}
				byte[] array = new byte[num2];
				this.SetSizeToPosition();
				int num3;
				while ((num3 = this.baseStream.Read(array, 0, num2)) != 0)
				{
					pstm.Write(array, num3, IntPtr.Zero);
					num += (long)num3;
					if (num >= cb)
					{
						break;
					}
					if (cb - num < 4096L)
					{
						num2 = (int)(cb - num);
					}
				}
			}
			if (pcbRead != IntPtr.Zero)
			{
				Marshal.WriteInt64(pcbRead, num);
			}
			if (pcbWritten != IntPtr.Zero)
			{
				Marshal.WriteInt64(pcbWritten, num);
			}
		}

		public void Commit(int grfCommitFlags)
		{
			this.baseStream.Flush();
			this.SetSizeToPosition();
		}

		public void Revert()
		{
			throw new ExternalException(null, -2147287039);
		}

		public void LockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new ExternalException(null, -2147287039);
		}

		public void UnlockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new ExternalException(null, -2147287039);
		}

		public void Stat(out global::System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
		{
			pstatstg = default(global::System.Runtime.InteropServices.ComTypes.STATSTG);
			pstatstg.cbSize = this.baseStream.Length;
		}

		public void Clone(out IStream ppstm)
		{
			ppstm = null;
			throw new ExternalException(null, -2147287039);
		}

		private const int STG_E_INVALIDFUNCTION = -2147287039;

		private readonly Stream baseStream;

		private long position = -1L;
	}
}
