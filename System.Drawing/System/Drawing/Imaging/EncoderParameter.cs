using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class EncoderParameter : IDisposable
	{
		~EncoderParameter()
		{
			this.Dispose(false);
		}

		public Encoder Encoder
		{
			get
			{
				return new Encoder(this._parameterGuid);
			}
			set
			{
				this._parameterGuid = value.Guid;
			}
		}

		public EncoderParameterValueType Type
		{
			get
			{
				return this._parameterValueType;
			}
		}

		public EncoderParameterValueType ValueType
		{
			get
			{
				return this._parameterValueType;
			}
		}

		public int NumberOfValues
		{
			get
			{
				return this._numberOfValues;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.KeepAlive(this);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this._parameterValue != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this._parameterValue);
			}
			this._parameterValue = IntPtr.Zero;
		}

		public EncoderParameter(Encoder encoder, byte value)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeByte;
			this._numberOfValues = 1;
			this._parameterValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.WriteByte(this._parameterValue, value);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, byte value, bool undefined)
		{
			this._parameterGuid = encoder.Guid;
			if (undefined)
			{
				this._parameterValueType = EncoderParameterValueType.ValueTypeUndefined;
			}
			else
			{
				this._parameterValueType = EncoderParameterValueType.ValueTypeByte;
			}
			this._numberOfValues = 1;
			this._parameterValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.WriteByte(this._parameterValue, value);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, short value)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeShort;
			this._numberOfValues = 1;
			this._parameterValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(short)));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.WriteInt16(this._parameterValue, value);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, long value)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeLong;
			this._numberOfValues = 1;
			this._parameterValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.WriteInt32(this._parameterValue, (int)value);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, int numerator, int denominator)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeRational;
			this._numberOfValues = 1;
			int num = Marshal.SizeOf(typeof(int));
			this._parameterValue = Marshal.AllocHGlobal(2 * num);
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.WriteInt32(this._parameterValue, numerator);
			Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, num), denominator);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, long rangebegin, long rangeend)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeLongRange;
			this._numberOfValues = 1;
			int num = Marshal.SizeOf(typeof(int));
			this._parameterValue = Marshal.AllocHGlobal(2 * num);
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.WriteInt32(this._parameterValue, (int)rangebegin);
			Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, num), (int)rangeend);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, int numerator1, int demoninator1, int numerator2, int demoninator2)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeRationalRange;
			this._numberOfValues = 1;
			int num = Marshal.SizeOf(typeof(int));
			this._parameterValue = Marshal.AllocHGlobal(4 * num);
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.WriteInt32(this._parameterValue, numerator1);
			Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, num), demoninator1);
			Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, 2 * num), numerator2);
			Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, 3 * num), demoninator2);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, string value)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeAscii;
			this._numberOfValues = value.Length;
			this._parameterValue = Marshal.StringToHGlobalAnsi(value);
			GC.KeepAlive(this);
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
		}

		public EncoderParameter(Encoder encoder, byte[] value)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeByte;
			this._numberOfValues = value.Length;
			this._parameterValue = Marshal.AllocHGlobal(this._numberOfValues);
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.Copy(value, 0, this._parameterValue, this._numberOfValues);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, byte[] value, bool undefined)
		{
			this._parameterGuid = encoder.Guid;
			if (undefined)
			{
				this._parameterValueType = EncoderParameterValueType.ValueTypeUndefined;
			}
			else
			{
				this._parameterValueType = EncoderParameterValueType.ValueTypeByte;
			}
			this._numberOfValues = value.Length;
			this._parameterValue = Marshal.AllocHGlobal(this._numberOfValues);
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.Copy(value, 0, this._parameterValue, this._numberOfValues);
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, short[] value)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeShort;
			this._numberOfValues = value.Length;
			int num = Marshal.SizeOf(typeof(short));
			this._parameterValue = Marshal.AllocHGlobal(checked(this._numberOfValues * num));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.Copy(value, 0, this._parameterValue, this._numberOfValues);
			GC.KeepAlive(this);
		}

		public unsafe EncoderParameter(Encoder encoder, long[] value)
		{
			this._parameterGuid = encoder.Guid;
			this._parameterValueType = EncoderParameterValueType.ValueTypeLong;
			this._numberOfValues = value.Length;
			int num = Marshal.SizeOf(typeof(int));
			this._parameterValue = Marshal.AllocHGlobal(checked(this._numberOfValues * num));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			int* ptr = (int*)(void*)this._parameterValue;
			fixed (long[] array = value)
			{
				long* ptr2;
				if (value == null || array.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array[0];
				}
				for (int i = 0; i < value.Length; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
			}
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, int[] numerator, int[] denominator)
		{
			this._parameterGuid = encoder.Guid;
			if (numerator.Length != denominator.Length)
			{
				throw SafeNativeMethods.Gdip.StatusException(2);
			}
			this._parameterValueType = EncoderParameterValueType.ValueTypeRational;
			this._numberOfValues = numerator.Length;
			int num = Marshal.SizeOf(typeof(int));
			this._parameterValue = Marshal.AllocHGlobal(checked(this._numberOfValues * 2 * num));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			for (int i = 0; i < this._numberOfValues; i++)
			{
				Marshal.WriteInt32(EncoderParameter.Add(i * 2 * num, this._parameterValue), numerator[i]);
				Marshal.WriteInt32(EncoderParameter.Add((i * 2 + 1) * num, this._parameterValue), denominator[i]);
			}
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, long[] rangebegin, long[] rangeend)
		{
			this._parameterGuid = encoder.Guid;
			if (rangebegin.Length != rangeend.Length)
			{
				throw SafeNativeMethods.Gdip.StatusException(2);
			}
			this._parameterValueType = EncoderParameterValueType.ValueTypeLongRange;
			this._numberOfValues = rangebegin.Length;
			int num = Marshal.SizeOf(typeof(int));
			this._parameterValue = Marshal.AllocHGlobal(checked(this._numberOfValues * 2 * num));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			for (int i = 0; i < this._numberOfValues; i++)
			{
				Marshal.WriteInt32(EncoderParameter.Add(i * 2 * num, this._parameterValue), (int)rangebegin[i]);
				Marshal.WriteInt32(EncoderParameter.Add((i * 2 + 1) * num, this._parameterValue), (int)rangeend[i]);
			}
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2)
		{
			this._parameterGuid = encoder.Guid;
			if (numerator1.Length != denominator1.Length || numerator1.Length != denominator2.Length || denominator1.Length != denominator2.Length)
			{
				throw SafeNativeMethods.Gdip.StatusException(2);
			}
			this._parameterValueType = EncoderParameterValueType.ValueTypeRationalRange;
			this._numberOfValues = numerator1.Length;
			int num = Marshal.SizeOf(typeof(int));
			this._parameterValue = Marshal.AllocHGlobal(checked(this._numberOfValues * 4 * num));
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			for (int i = 0; i < this._numberOfValues; i++)
			{
				Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, 4 * i * num), numerator1[i]);
				Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, (4 * i + 1) * num), denominator1[i]);
				Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, (4 * i + 2) * num), numerator2[i]);
				Marshal.WriteInt32(EncoderParameter.Add(this._parameterValue, (4 * i + 3) * num), denominator2[i]);
			}
			GC.KeepAlive(this);
		}

		[Obsolete("This constructor has been deprecated. Use EncoderParameter(Encoder encoder, int numberValues, EncoderParameterValueType type, IntPtr value) instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
		public EncoderParameter(Encoder encoder, int NumberOfValues, int Type, int Value)
		{
			int num;
			switch (Type)
			{
			case 1:
			case 2:
				num = 1;
				break;
			case 3:
				num = 2;
				break;
			case 4:
				num = 4;
				break;
			case 5:
			case 6:
				num = 8;
				break;
			case 7:
				num = 1;
				break;
			case 8:
				num = 16;
				break;
			default:
				throw SafeNativeMethods.Gdip.StatusException(8);
			}
			int num2 = checked(num * NumberOfValues);
			this._parameterValue = Marshal.AllocHGlobal(num2);
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			for (int i = 0; i < num2; i++)
			{
				Marshal.WriteByte(EncoderParameter.Add(this._parameterValue, i), Marshal.ReadByte((IntPtr)(Value + i)));
			}
			this._parameterValueType = (EncoderParameterValueType)Type;
			this._numberOfValues = NumberOfValues;
			this._parameterGuid = encoder.Guid;
			GC.KeepAlive(this);
		}

		public EncoderParameter(Encoder encoder, int numberValues, EncoderParameterValueType type, IntPtr value)
		{
			int num;
			switch (type)
			{
			case EncoderParameterValueType.ValueTypeByte:
			case EncoderParameterValueType.ValueTypeAscii:
				num = 1;
				break;
			case EncoderParameterValueType.ValueTypeShort:
				num = 2;
				break;
			case EncoderParameterValueType.ValueTypeLong:
				num = 4;
				break;
			case EncoderParameterValueType.ValueTypeRational:
			case EncoderParameterValueType.ValueTypeLongRange:
				num = 8;
				break;
			case EncoderParameterValueType.ValueTypeUndefined:
				num = 1;
				break;
			case EncoderParameterValueType.ValueTypeRationalRange:
				num = 16;
				break;
			default:
				throw SafeNativeMethods.Gdip.StatusException(8);
			}
			int num2 = checked(num * numberValues);
			this._parameterValue = Marshal.AllocHGlobal(num2);
			if (this._parameterValue == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			for (int i = 0; i < num2; i++)
			{
				Marshal.WriteByte(EncoderParameter.Add(this._parameterValue, i), Marshal.ReadByte(value + i));
			}
			this._parameterValueType = type;
			this._numberOfValues = numberValues;
			this._parameterGuid = encoder.Guid;
			GC.KeepAlive(this);
		}

		private static IntPtr Add(IntPtr a, int b)
		{
			return (IntPtr)((long)a + (long)b);
		}

		private static IntPtr Add(int a, IntPtr b)
		{
			return (IntPtr)((long)a + (long)b);
		}

		[MarshalAs(UnmanagedType.Struct)]
		private Guid _parameterGuid;

		private int _numberOfValues;

		private EncoderParameterValueType _parameterValueType;

		private IntPtr _parameterValue;
	}
}
