using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	public sealed class EncoderParameters : IDisposable
	{
		public EncoderParameters(int count)
		{
			this._param = new EncoderParameter[count];
		}

		public EncoderParameters()
		{
			this._param = new EncoderParameter[1];
		}

		public EncoderParameter[] Param
		{
			get
			{
				return this._param;
			}
			set
			{
				this._param = value;
			}
		}

		internal IntPtr ConvertToMemory()
		{
			int num = Marshal.SizeOf(typeof(EncoderParameter));
			int num2 = this._param.Length;
			IntPtr intPtr;
			long num3;
			checked
			{
				intPtr = Marshal.AllocHGlobal(num2 * num + Marshal.SizeOf(typeof(IntPtr)));
				if (intPtr == IntPtr.Zero)
				{
					throw SafeNativeMethods.Gdip.StatusException(3);
				}
				Marshal.WriteIntPtr(intPtr, (IntPtr)num2);
				num3 = (long)intPtr + unchecked((long)Marshal.SizeOf(typeof(IntPtr)));
			}
			for (int i = 0; i < num2; i++)
			{
				Marshal.StructureToPtr<EncoderParameter>(this._param[i], (IntPtr)(num3 + (long)(i * num)), false);
			}
			return intPtr;
		}

		internal static EncoderParameters ConvertFromMemory(IntPtr memory)
		{
			if (memory == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(2);
			}
			int num = Marshal.ReadIntPtr(memory).ToInt32();
			EncoderParameters encoderParameters = new EncoderParameters(num);
			int num2 = Marshal.SizeOf(typeof(EncoderParameter));
			long num3 = (long)memory + (long)Marshal.SizeOf(typeof(IntPtr));
			for (int i = 0; i < num; i++)
			{
				Guid guid = (Guid)Marshal.PtrToStructure((IntPtr)((long)(i * num2) + num3), typeof(Guid));
				int num4 = Marshal.ReadInt32((IntPtr)((long)(i * num2) + num3 + 16L));
				EncoderParameterValueType encoderParameterValueType = (EncoderParameterValueType)Marshal.ReadInt32((IntPtr)((long)(i * num2) + num3 + 20L));
				IntPtr intPtr = Marshal.ReadIntPtr((IntPtr)((long)(i * num2) + num3 + 24L));
				encoderParameters._param[i] = new EncoderParameter(new Encoder(guid), num4, encoderParameterValueType, intPtr);
			}
			return encoderParameters;
		}

		public void Dispose()
		{
			foreach (EncoderParameter encoderParameter in this._param)
			{
				if (encoderParameter != null)
				{
					encoderParameter.Dispose();
				}
			}
			this._param = null;
		}

		private EncoderParameter[] _param;
	}
}
