using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	public sealed class EncoderParameters : IDisposable
	{
		public EncoderParameters()
		{
			this.parameters = new EncoderParameter[1];
		}

		public EncoderParameters(int count)
		{
			this.parameters = new EncoderParameter[count];
		}

		public EncoderParameter[] Param
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		internal IntPtr ToNativePtr()
		{
			IntPtr intPtr = Marshal.AllocHGlobal(4 + this.parameters.Length * EncoderParameter.NativeSize());
			IntPtr intPtr2 = intPtr;
			Marshal.WriteInt32(intPtr2, this.parameters.Length);
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + 4L);
			for (int i = 0; i < this.parameters.Length; i++)
			{
				this.parameters[i].ToNativePtr(intPtr2);
				intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)EncoderParameter.NativeSize());
			}
			return intPtr;
		}

		internal static EncoderParameters FromNativePtr(IntPtr epPtr)
		{
			if (epPtr == IntPtr.Zero)
			{
				return null;
			}
			IntPtr intPtr = epPtr;
			int num = Marshal.ReadInt32(intPtr);
			intPtr = (IntPtr)(intPtr.ToInt64() + 4L);
			if (num == 0)
			{
				return null;
			}
			EncoderParameters encoderParameters = new EncoderParameters(num);
			for (int i = 0; i < num; i++)
			{
				encoderParameters.parameters[i] = EncoderParameter.FromNativePtr(intPtr);
				intPtr = (IntPtr)(intPtr.ToInt64() + (long)EncoderParameter.NativeSize());
			}
			return encoderParameters;
		}

		private EncoderParameter[] parameters;
	}
}
