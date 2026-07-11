using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct DSPConnection
	{
		public RESULT getInput(out DSP input)
		{
			return DSPConnection.FMOD5_DSPConnection_GetInput(this.handle, out input.handle);
		}

		public RESULT getOutput(out DSP output)
		{
			return DSPConnection.FMOD5_DSPConnection_GetOutput(this.handle, out output.handle);
		}

		public RESULT setMix(float volume)
		{
			return DSPConnection.FMOD5_DSPConnection_SetMix(this.handle, volume);
		}

		public RESULT getMix(out float volume)
		{
			return DSPConnection.FMOD5_DSPConnection_GetMix(this.handle, out volume);
		}

		public RESULT setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop)
		{
			return DSPConnection.FMOD5_DSPConnection_SetMixMatrix(this.handle, matrix, outchannels, inchannels, inchannel_hop);
		}

		public RESULT getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
		{
			return DSPConnection.FMOD5_DSPConnection_GetMixMatrix(this.handle, matrix, out outchannels, out inchannels, inchannel_hop);
		}

		public RESULT getType(out DSPCONNECTION_TYPE type)
		{
			return DSPConnection.FMOD5_DSPConnection_GetType(this.handle, out type);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return DSPConnection.FMOD5_DSPConnection_SetUserData(this.handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return DSPConnection.FMOD5_DSPConnection_GetUserData(this.handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_GetInput(IntPtr dspconnection, out IntPtr input);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_GetOutput(IntPtr dspconnection, out IntPtr output);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_SetMix(IntPtr dspconnection, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_GetMix(IntPtr dspconnection, out float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_SetMixMatrix(IntPtr dspconnection, float[] matrix, int outchannels, int inchannels, int inchannel_hop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_GetMixMatrix(IntPtr dspconnection, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_GetType(IntPtr dspconnection, out DSPCONNECTION_TYPE type);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSPConnection_GetUserData(IntPtr dspconnection, out IntPtr userdata);

		public bool hasHandle()
		{
			return this.handle != IntPtr.Zero;
		}

		public void clearHandle()
		{
			this.handle = IntPtr.Zero;
		}

		public IntPtr handle;
	}
}
