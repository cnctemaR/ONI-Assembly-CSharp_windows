using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	internal static class StringHelper
	{
		public static StringHelper.ThreadSafeEncoding GetFreeHelper()
		{
			List<StringHelper.ThreadSafeEncoding> list = StringHelper.encoders;
			StringHelper.ThreadSafeEncoding threadSafeEncoding2;
			lock (list)
			{
				StringHelper.ThreadSafeEncoding threadSafeEncoding = null;
				for (int i = 0; i < StringHelper.encoders.Count; i++)
				{
					if (!StringHelper.encoders[i].InUse())
					{
						threadSafeEncoding = StringHelper.encoders[i];
						break;
					}
				}
				if (threadSafeEncoding == null)
				{
					threadSafeEncoding = new StringHelper.ThreadSafeEncoding();
					StringHelper.encoders.Add(threadSafeEncoding);
				}
				threadSafeEncoding.SetInUse();
				threadSafeEncoding2 = threadSafeEncoding;
			}
			return threadSafeEncoding2;
		}

		private static List<StringHelper.ThreadSafeEncoding> encoders = new List<StringHelper.ThreadSafeEncoding>(1);

		public class ThreadSafeEncoding : IDisposable
		{
			public bool InUse()
			{
				return this.inUse;
			}

			public void SetInUse()
			{
				this.inUse = true;
			}

			private int roundUpPowerTwo(int number)
			{
				int i;
				for (i = 1; i <= number; i *= 2)
				{
				}
				return i;
			}

			public byte[] byteFromStringUTF8(string s)
			{
				if (s == null)
				{
					return null;
				}
				if (this.encoding.GetMaxByteCount(s.Length) + 1 > this.encodedBuffer.Length)
				{
					int num = this.encoding.GetByteCount(s) + 1;
					if (num > this.encodedBuffer.Length)
					{
						this.encodedBuffer = new byte[this.roundUpPowerTwo(num)];
					}
				}
				int bytes = this.encoding.GetBytes(s, 0, s.Length, this.encodedBuffer, 0);
				this.encodedBuffer[bytes] = 0;
				return this.encodedBuffer;
			}

			public string stringFromNative(IntPtr nativePtr)
			{
				if (nativePtr == IntPtr.Zero)
				{
					return "";
				}
				int num = 0;
				while (Marshal.ReadByte(nativePtr, num) != 0)
				{
					num++;
				}
				if (num == 0)
				{
					return "";
				}
				if (num > this.encodedBuffer.Length)
				{
					this.encodedBuffer = new byte[this.roundUpPowerTwo(num)];
				}
				Marshal.Copy(nativePtr, this.encodedBuffer, 0, num);
				if (this.encoding.GetMaxCharCount(num) > this.decodedBuffer.Length)
				{
					int charCount = this.encoding.GetCharCount(this.encodedBuffer, 0, num);
					if (charCount > this.decodedBuffer.Length)
					{
						this.decodedBuffer = new char[this.roundUpPowerTwo(charCount)];
					}
				}
				int chars = this.encoding.GetChars(this.encodedBuffer, 0, num, this.decodedBuffer, 0);
				return new string(this.decodedBuffer, 0, chars);
			}

			public void Dispose()
			{
				List<StringHelper.ThreadSafeEncoding> encoders = StringHelper.encoders;
				lock (encoders)
				{
					this.inUse = false;
				}
			}

			private UTF8Encoding encoding = new UTF8Encoding();

			private byte[] encodedBuffer = new byte[128];

			private char[] decodedBuffer = new char[128];

			private bool inUse;
		}
	}
}
