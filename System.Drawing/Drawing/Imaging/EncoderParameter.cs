using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class EncoderParameter : IDisposable
	{
		internal EncoderParameter()
		{
		}

		public EncoderParameter(Encoder encoder, byte value)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeByte;
			this.valuePtr = Marshal.AllocHGlobal(1);
			Marshal.WriteByte(this.valuePtr, value);
		}

		public EncoderParameter(Encoder encoder, byte[] value)
		{
			this.encoder = encoder;
			this.valuesCount = value.Length;
			this.type = EncoderParameterValueType.ValueTypeByte;
			this.valuePtr = Marshal.AllocHGlobal(1 * this.valuesCount);
			Marshal.Copy(value, 0, this.valuePtr, this.valuesCount);
		}

		public EncoderParameter(Encoder encoder, short value)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeShort;
			this.valuePtr = Marshal.AllocHGlobal(2);
			Marshal.WriteInt16(this.valuePtr, value);
		}

		public EncoderParameter(Encoder encoder, short[] value)
		{
			this.encoder = encoder;
			this.valuesCount = value.Length;
			this.type = EncoderParameterValueType.ValueTypeShort;
			this.valuePtr = Marshal.AllocHGlobal(2 * this.valuesCount);
			Marshal.Copy(value, 0, this.valuePtr, this.valuesCount);
		}

		public EncoderParameter(Encoder encoder, long value)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeLong;
			this.valuePtr = Marshal.AllocHGlobal(4);
			Marshal.WriteInt32(this.valuePtr, (int)value);
		}

		public EncoderParameter(Encoder encoder, long[] value)
		{
			this.encoder = encoder;
			this.valuesCount = value.Length;
			this.type = EncoderParameterValueType.ValueTypeLong;
			this.valuePtr = Marshal.AllocHGlobal(4 * this.valuesCount);
			int[] array = new int[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = (int)value[i];
			}
			Marshal.Copy(array, 0, this.valuePtr, this.valuesCount);
		}

		public EncoderParameter(Encoder encoder, string value)
		{
			this.encoder = encoder;
			ASCIIEncoding asciiencoding = new ASCIIEncoding();
			int byteCount = asciiencoding.GetByteCount(value);
			byte[] array = new byte[byteCount];
			asciiencoding.GetBytes(value, 0, value.Length, array, 0);
			this.valuesCount = array.Length;
			this.type = EncoderParameterValueType.ValueTypeAscii;
			this.valuePtr = Marshal.AllocHGlobal(this.valuesCount);
			Marshal.Copy(array, 0, this.valuePtr, this.valuesCount);
		}

		public EncoderParameter(Encoder encoder, byte value, bool undefined)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			if (undefined)
			{
				this.type = EncoderParameterValueType.ValueTypeUndefined;
			}
			else
			{
				this.type = EncoderParameterValueType.ValueTypeByte;
			}
			this.valuePtr = Marshal.AllocHGlobal(1);
			Marshal.WriteByte(this.valuePtr, value);
		}

		public EncoderParameter(Encoder encoder, byte[] value, bool undefined)
		{
			this.encoder = encoder;
			this.valuesCount = value.Length;
			if (undefined)
			{
				this.type = EncoderParameterValueType.ValueTypeUndefined;
			}
			else
			{
				this.type = EncoderParameterValueType.ValueTypeByte;
			}
			this.valuePtr = Marshal.AllocHGlobal(this.valuesCount);
			Marshal.Copy(value, 0, this.valuePtr, this.valuesCount);
		}

		public EncoderParameter(Encoder encoder, int numerator, int denominator)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeRational;
			this.valuePtr = Marshal.AllocHGlobal(8);
			int[] array = new int[] { numerator, denominator };
			Marshal.Copy(array, 0, this.valuePtr, array.Length);
		}

		public EncoderParameter(Encoder encoder, int[] numerator, int[] denominator)
		{
			if (numerator.Length != denominator.Length)
			{
				throw new ArgumentException("Invalid parameter used.");
			}
			this.encoder = encoder;
			this.valuesCount = numerator.Length;
			this.type = EncoderParameterValueType.ValueTypeRational;
			this.valuePtr = Marshal.AllocHGlobal(4 * this.valuesCount * 2);
			for (int i = 0; i < this.valuesCount; i++)
			{
				Marshal.WriteInt32(this.valuePtr, i * 4, numerator[i]);
				Marshal.WriteInt32(this.valuePtr, (i + 1) * 4, denominator[i]);
			}
		}

		public EncoderParameter(Encoder encoder, long rangebegin, long rangeend)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeLongRange;
			this.valuePtr = Marshal.AllocHGlobal(8);
			int[] array = new int[]
			{
				(int)rangebegin,
				(int)rangeend
			};
			Marshal.Copy(array, 0, this.valuePtr, array.Length);
		}

		public EncoderParameter(Encoder encoder, long[] rangebegin, long[] rangeend)
		{
			if (rangebegin.Length != rangeend.Length)
			{
				throw new ArgumentException("Invalid parameter used.");
			}
			this.encoder = encoder;
			this.valuesCount = rangebegin.Length;
			this.type = EncoderParameterValueType.ValueTypeLongRange;
			this.valuePtr = Marshal.AllocHGlobal(4 * this.valuesCount * 2);
			IntPtr intPtr = this.valuePtr;
			for (int i = 0; i < this.valuesCount; i++)
			{
				Marshal.WriteInt32(intPtr, i * 4, (int)rangebegin[i]);
				Marshal.WriteInt32(intPtr, (i + 1) * 4, (int)rangeend[i]);
			}
		}

		public EncoderParameter(Encoder encoder, int numberOfValues, int type, int value)
		{
			this.encoder = encoder;
			this.valuePtr = (IntPtr)value;
			this.valuesCount = numberOfValues;
			this.type = (EncoderParameterValueType)type;
		}

		public EncoderParameter(Encoder encoder, int numerator1, int denominator1, int numerator2, int denominator2)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeRationalRange;
			this.valuePtr = Marshal.AllocHGlobal(16);
			int[] array = new int[] { numerator1, denominator1, numerator2, denominator2 };
			Marshal.Copy(array, 0, this.valuePtr, 4);
		}

		public EncoderParameter(Encoder encoder, int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2)
		{
			if (numerator1.Length != denominator1.Length || numerator2.Length != denominator2.Length || numerator1.Length != numerator2.Length)
			{
				throw new ArgumentException("Invalid parameter used.");
			}
			this.encoder = encoder;
			this.valuesCount = numerator1.Length;
			this.type = EncoderParameterValueType.ValueTypeRationalRange;
			this.valuePtr = Marshal.AllocHGlobal(4 * this.valuesCount * 4);
			IntPtr intPtr = this.valuePtr;
			for (int i = 0; i < this.valuesCount; i++)
			{
				Marshal.WriteInt32(intPtr, i * 4, numerator1[i]);
				Marshal.WriteInt32(intPtr, (i + 1) * 4, denominator1[i]);
				Marshal.WriteInt32(intPtr, (i + 2) * 4, numerator2[i]);
				Marshal.WriteInt32(intPtr, (i + 3) * 4, denominator2[i]);
			}
		}

		public Encoder Encoder
		{
			get
			{
				return this.encoder;
			}
			set
			{
				this.encoder = value;
			}
		}

		public int NumberOfValues
		{
			get
			{
				return this.valuesCount;
			}
		}

		public EncoderParameterValueType Type
		{
			get
			{
				return this.type;
			}
		}

		public EncoderParameterValueType ValueType
		{
			get
			{
				return this.type;
			}
		}

		private void Dispose(bool disposing)
		{
			if (this.valuePtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.valuePtr);
				this.valuePtr = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~EncoderParameter()
		{
			this.Dispose(false);
		}

		internal static int NativeSize()
		{
			return Marshal.SizeOf(typeof(GdipEncoderParameter));
		}

		internal void ToNativePtr(IntPtr epPtr)
		{
			Marshal.StructureToPtr(new GdipEncoderParameter
			{
				guid = this.encoder.Guid,
				numberOfValues = (uint)this.valuesCount,
				type = this.type,
				value = this.valuePtr
			}, epPtr, false);
		}

		internal unsafe static EncoderParameter FromNativePtr(IntPtr epPtr)
		{
			GdipEncoderParameter gdipEncoderParameter = (GdipEncoderParameter)Marshal.PtrToStructure(epPtr, typeof(GdipEncoderParameter));
			Type type;
			uint num;
			switch (gdipEncoderParameter.type)
			{
			case EncoderParameterValueType.ValueTypeByte:
			case EncoderParameterValueType.ValueTypeAscii:
			case EncoderParameterValueType.ValueTypeUndefined:
				type = typeof(byte);
				num = gdipEncoderParameter.numberOfValues;
				break;
			case EncoderParameterValueType.ValueTypeShort:
				type = typeof(short);
				num = gdipEncoderParameter.numberOfValues;
				break;
			case EncoderParameterValueType.ValueTypeLong:
				type = typeof(int);
				num = gdipEncoderParameter.numberOfValues;
				break;
			case EncoderParameterValueType.ValueTypeRational:
			case EncoderParameterValueType.ValueTypeLongRange:
				type = typeof(int);
				num = gdipEncoderParameter.numberOfValues * 2U;
				break;
			case EncoderParameterValueType.ValueTypeRationalRange:
				type = typeof(int);
				num = gdipEncoderParameter.numberOfValues * 4U;
				break;
			default:
				return null;
			}
			EncoderParameter encoderParameter = new EncoderParameter();
			encoderParameter.encoder = new Encoder(gdipEncoderParameter.guid);
			encoderParameter.valuesCount = (int)gdipEncoderParameter.numberOfValues;
			encoderParameter.type = gdipEncoderParameter.type;
			encoderParameter.valuePtr = Marshal.AllocHGlobal((int)((ulong)num * (ulong)((long)Marshal.SizeOf(type))));
			byte* ptr = (byte*)(void*)gdipEncoderParameter.value;
			byte* ptr2 = (byte*)(void*)encoderParameter.valuePtr;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num * (ulong)((long)Marshal.SizeOf(type))))
			{
				*(ptr2++) = *(ptr++);
				num2++;
			}
			return encoderParameter;
		}

		private Encoder encoder;

		private int valuesCount;

		private EncoderParameterValueType type;

		private IntPtr valuePtr;
	}
}
