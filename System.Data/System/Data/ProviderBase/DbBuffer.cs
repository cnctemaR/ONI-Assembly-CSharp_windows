using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Data.ProviderBase
{
	internal abstract class DbBuffer : SafeHandle
	{
		protected DbBuffer(int initialSize)
			: base(IntPtr.Zero, true)
		{
			if (0 < initialSize)
			{
				this._bufferLength = initialSize;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					this.handle = SafeNativeMethods.LocalAlloc((IntPtr)initialSize);
				}
				if (IntPtr.Zero == this.handle)
				{
					throw new OutOfMemoryException();
				}
			}
		}

		protected DbBuffer(IntPtr invalidHandleValue, bool ownsHandle)
			: base(invalidHandleValue, ownsHandle)
		{
		}

		private int BaseOffset
		{
			get
			{
				return 0;
			}
		}

		public override bool IsInvalid
		{
			get
			{
				return IntPtr.Zero == this.handle;
			}
		}

		internal int Length
		{
			get
			{
				return this._bufferLength;
			}
		}

		internal string PtrToStringUni(int offset)
		{
			offset += this.BaseOffset;
			this.Validate(offset, 2);
			string text = null;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				text = Marshal.PtrToStringUni(ADP.IntPtrOffset(base.DangerousGetHandle(), offset));
				this.Validate(offset, 2 * (text.Length + 1));
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return text;
		}

		internal string PtrToStringUni(int offset, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, 2 * length);
			string text = null;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				text = Marshal.PtrToStringUni(ADP.IntPtrOffset(base.DangerousGetHandle(), offset), length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return text;
		}

		internal byte ReadByte(int offset)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			byte b;
			try
			{
				base.DangerousAddRef(ref flag);
				b = Marshal.ReadByte(base.DangerousGetHandle(), offset);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return b;
		}

		internal byte[] ReadBytes(int offset, int length)
		{
			byte[] array = new byte[length];
			return this.ReadBytes(offset, array, 0, length);
		}

		internal byte[] ReadBytes(int offset, byte[] destination, int startIndex, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, length);
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.Copy(ADP.IntPtrOffset(base.DangerousGetHandle(), offset), destination, startIndex, length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return destination;
		}

		internal char ReadChar(int offset)
		{
			return (char)this.ReadInt16(offset);
		}

		internal char[] ReadChars(int offset, char[] destination, int startIndex, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, 2 * length);
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.Copy(ADP.IntPtrOffset(base.DangerousGetHandle(), offset), destination, startIndex, length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return destination;
		}

		internal double ReadDouble(int offset)
		{
			return BitConverter.Int64BitsToDouble(this.ReadInt64(offset));
		}

		internal short ReadInt16(int offset)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			short num;
			try
			{
				base.DangerousAddRef(ref flag);
				num = Marshal.ReadInt16(base.DangerousGetHandle(), offset);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return num;
		}

		internal void ReadInt16Array(int offset, short[] destination, int startIndex, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, 2 * length);
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.Copy(ADP.IntPtrOffset(base.DangerousGetHandle(), offset), destination, startIndex, length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal int ReadInt32(int offset)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			int num;
			try
			{
				base.DangerousAddRef(ref flag);
				num = Marshal.ReadInt32(base.DangerousGetHandle(), offset);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return num;
		}

		internal void ReadInt32Array(int offset, int[] destination, int startIndex, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, 4 * length);
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.Copy(ADP.IntPtrOffset(base.DangerousGetHandle(), offset), destination, startIndex, length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal long ReadInt64(int offset)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			long num;
			try
			{
				base.DangerousAddRef(ref flag);
				num = Marshal.ReadInt64(base.DangerousGetHandle(), offset);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return num;
		}

		internal IntPtr ReadIntPtr(int offset)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			IntPtr intPtr;
			try
			{
				base.DangerousAddRef(ref flag);
				intPtr = Marshal.ReadIntPtr(base.DangerousGetHandle(), offset);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return intPtr;
		}

		internal unsafe float ReadSingle(int offset)
		{
			int num = this.ReadInt32(offset);
			return *(float*)(&num);
		}

		protected override bool ReleaseHandle()
		{
			IntPtr handle = this.handle;
			this.handle = IntPtr.Zero;
			if (IntPtr.Zero != handle)
			{
				SafeNativeMethods.LocalFree(handle);
			}
			return true;
		}

		private void StructureToPtr(int offset, object structure)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				IntPtr intPtr = ADP.IntPtrOffset(base.DangerousGetHandle(), offset);
				Marshal.StructureToPtr(structure, intPtr, false);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteByte(int offset, byte value)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.WriteByte(base.DangerousGetHandle(), offset, value);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteBytes(int offset, byte[] source, int startIndex, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, length);
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				IntPtr intPtr = ADP.IntPtrOffset(base.DangerousGetHandle(), offset);
				Marshal.Copy(source, startIndex, intPtr, length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteCharArray(int offset, char[] source, int startIndex, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, 2 * length);
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				IntPtr intPtr = ADP.IntPtrOffset(base.DangerousGetHandle(), offset);
				Marshal.Copy(source, startIndex, intPtr, length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteDouble(int offset, double value)
		{
			this.WriteInt64(offset, BitConverter.DoubleToInt64Bits(value));
		}

		internal void WriteInt16(int offset, short value)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.WriteInt16(base.DangerousGetHandle(), offset, value);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteInt16Array(int offset, short[] source, int startIndex, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, 2 * length);
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				IntPtr intPtr = ADP.IntPtrOffset(base.DangerousGetHandle(), offset);
				Marshal.Copy(source, startIndex, intPtr, length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteInt32(int offset, int value)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.WriteInt32(base.DangerousGetHandle(), offset, value);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteInt32Array(int offset, int[] source, int startIndex, int length)
		{
			offset += this.BaseOffset;
			this.Validate(offset, 4 * length);
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				IntPtr intPtr = ADP.IntPtrOffset(base.DangerousGetHandle(), offset);
				Marshal.Copy(source, startIndex, intPtr, length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteInt64(int offset, long value)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.WriteInt64(base.DangerousGetHandle(), offset, value);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal void WriteIntPtr(int offset, IntPtr value)
		{
			offset += this.BaseOffset;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				Marshal.WriteIntPtr(base.DangerousGetHandle(), offset, value);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal unsafe void WriteSingle(int offset, float value)
		{
			this.WriteInt32(offset, *(int*)(&value));
		}

		internal void ZeroMemory()
		{
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.DangerousAddRef(ref flag);
				SafeNativeMethods.ZeroMemory(base.DangerousGetHandle(), this.Length);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		internal Guid ReadGuid(int offset)
		{
			byte[] array = new byte[16];
			this.ReadBytes(offset, array, 0, 16);
			return new Guid(array);
		}

		internal void WriteGuid(int offset, Guid value)
		{
			this.StructureToPtr(offset, value);
		}

		internal DateTime ReadDate(int offset)
		{
			short[] array = new short[3];
			this.ReadInt16Array(offset, array, 0, 3);
			return new DateTime((int)((ushort)array[0]), (int)((ushort)array[1]), (int)((ushort)array[2]));
		}

		internal void WriteDate(int offset, DateTime value)
		{
			short[] array = new short[]
			{
				(short)value.Year,
				(short)value.Month,
				(short)value.Day
			};
			this.WriteInt16Array(offset, array, 0, 3);
		}

		internal TimeSpan ReadTime(int offset)
		{
			short[] array = new short[3];
			this.ReadInt16Array(offset, array, 0, 3);
			return new TimeSpan((int)((ushort)array[0]), (int)((ushort)array[1]), (int)((ushort)array[2]));
		}

		internal void WriteTime(int offset, TimeSpan value)
		{
			short[] array = new short[]
			{
				(short)value.Hours,
				(short)value.Minutes,
				(short)value.Seconds
			};
			this.WriteInt16Array(offset, array, 0, 3);
		}

		internal DateTime ReadDateTime(int offset)
		{
			short[] array = new short[6];
			this.ReadInt16Array(offset, array, 0, 6);
			int num = this.ReadInt32(offset + 12);
			DateTime dateTime = new DateTime((int)((ushort)array[0]), (int)((ushort)array[1]), (int)((ushort)array[2]), (int)((ushort)array[3]), (int)((ushort)array[4]), (int)((ushort)array[5]));
			return dateTime.AddTicks((long)(num / 100));
		}

		internal void WriteDateTime(int offset, DateTime value)
		{
			int num = (int)(value.Ticks % 10000000L) * 100;
			short[] array = new short[]
			{
				(short)value.Year,
				(short)value.Month,
				(short)value.Day,
				(short)value.Hour,
				(short)value.Minute,
				(short)value.Second
			};
			this.WriteInt16Array(offset, array, 0, 6);
			this.WriteInt32(offset + 12, num);
		}

		internal decimal ReadNumeric(int offset)
		{
			byte[] array = new byte[20];
			this.ReadBytes(offset, array, 1, 19);
			int[] array2 = new int[]
			{
				0,
				0,
				0,
				(int)array[2] << 16
			};
			if (array[3] == 0)
			{
				array2[3] |= int.MinValue;
			}
			array2[0] = BitConverter.ToInt32(array, 4);
			array2[1] = BitConverter.ToInt32(array, 8);
			array2[2] = BitConverter.ToInt32(array, 12);
			if (BitConverter.ToInt32(array, 16) != 0)
			{
				throw ADP.NumericToDecimalOverflow();
			}
			return new decimal(array2);
		}

		internal void WriteNumeric(int offset, decimal value, byte precision)
		{
			int[] bits = decimal.GetBits(value);
			byte[] array = new byte[20];
			array[1] = precision;
			Buffer.BlockCopy(bits, 14, array, 2, 2);
			array[3] = ((array[3] == 0) ? 1 : 0);
			Buffer.BlockCopy(bits, 0, array, 4, 12);
			array[16] = 0;
			array[17] = 0;
			array[18] = 0;
			array[19] = 0;
			this.WriteBytes(offset, array, 1, 19);
		}

		[Conditional("DEBUG")]
		protected void ValidateCheck(int offset, int count)
		{
			this.Validate(offset, count);
		}

		protected void Validate(int offset, int count)
		{
			if (offset < 0 || count < 0 || this.Length < checked(offset + count))
			{
				throw ADP.InternalError(ADP.InternalErrorCode.InvalidBuffer);
			}
		}

		private readonly int _bufferLength;
	}
}
