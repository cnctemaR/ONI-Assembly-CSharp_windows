using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct DSP
	{
		public RESULT release()
		{
			return DSP.FMOD5_DSP_Release(this.handle);
		}

		public RESULT getSystemObject(out FMOD.System system)
		{
			return DSP.FMOD5_DSP_GetSystemObject(this.handle, out system.handle);
		}

		public RESULT addInput(DSP target, out DSPConnection connection, DSPCONNECTION_TYPE type)
		{
			return DSP.FMOD5_DSP_AddInput(this.handle, target.handle, out connection.handle, type);
		}

		public RESULT disconnectFrom(DSP target, DSPConnection connection)
		{
			return DSP.FMOD5_DSP_DisconnectFrom(this.handle, target.handle, connection.handle);
		}

		public RESULT disconnectAll(bool inputs, bool outputs)
		{
			return DSP.FMOD5_DSP_DisconnectAll(this.handle, inputs, outputs);
		}

		public RESULT getNumInputs(out int numinputs)
		{
			return DSP.FMOD5_DSP_GetNumInputs(this.handle, out numinputs);
		}

		public RESULT getNumOutputs(out int numoutputs)
		{
			return DSP.FMOD5_DSP_GetNumOutputs(this.handle, out numoutputs);
		}

		public RESULT getInput(int index, out DSP input, out DSPConnection inputconnection)
		{
			return DSP.FMOD5_DSP_GetInput(this.handle, index, out input.handle, out inputconnection.handle);
		}

		public RESULT getOutput(int index, out DSP output, out DSPConnection outputconnection)
		{
			return DSP.FMOD5_DSP_GetOutput(this.handle, index, out output.handle, out outputconnection.handle);
		}

		public RESULT setActive(bool active)
		{
			return DSP.FMOD5_DSP_SetActive(this.handle, active);
		}

		public RESULT getActive(out bool active)
		{
			return DSP.FMOD5_DSP_GetActive(this.handle, out active);
		}

		public RESULT setBypass(bool bypass)
		{
			return DSP.FMOD5_DSP_SetBypass(this.handle, bypass);
		}

		public RESULT getBypass(out bool bypass)
		{
			return DSP.FMOD5_DSP_GetBypass(this.handle, out bypass);
		}

		public RESULT setWetDryMix(float prewet, float postwet, float dry)
		{
			return DSP.FMOD5_DSP_SetWetDryMix(this.handle, prewet, postwet, dry);
		}

		public RESULT getWetDryMix(out float prewet, out float postwet, out float dry)
		{
			return DSP.FMOD5_DSP_GetWetDryMix(this.handle, out prewet, out postwet, out dry);
		}

		public RESULT setChannelFormat(CHANNELMASK channelmask, int numchannels, SPEAKERMODE source_speakermode)
		{
			return DSP.FMOD5_DSP_SetChannelFormat(this.handle, channelmask, numchannels, source_speakermode);
		}

		public RESULT getChannelFormat(out CHANNELMASK channelmask, out int numchannels, out SPEAKERMODE source_speakermode)
		{
			return DSP.FMOD5_DSP_GetChannelFormat(this.handle, out channelmask, out numchannels, out source_speakermode);
		}

		public RESULT getOutputChannelFormat(CHANNELMASK inmask, int inchannels, SPEAKERMODE inspeakermode, out CHANNELMASK outmask, out int outchannels, out SPEAKERMODE outspeakermode)
		{
			return DSP.FMOD5_DSP_GetOutputChannelFormat(this.handle, inmask, inchannels, inspeakermode, out outmask, out outchannels, out outspeakermode);
		}

		public RESULT reset()
		{
			return DSP.FMOD5_DSP_Reset(this.handle);
		}

		public RESULT setParameterFloat(int index, float value)
		{
			return DSP.FMOD5_DSP_SetParameterFloat(this.handle, index, value);
		}

		public RESULT setParameterInt(int index, int value)
		{
			return DSP.FMOD5_DSP_SetParameterInt(this.handle, index, value);
		}

		public RESULT setParameterBool(int index, bool value)
		{
			return DSP.FMOD5_DSP_SetParameterBool(this.handle, index, value);
		}

		public RESULT setParameterData(int index, byte[] data)
		{
			return DSP.FMOD5_DSP_SetParameterData(this.handle, index, Marshal.UnsafeAddrOfPinnedArrayElement<byte>(data, 0), (uint)data.Length);
		}

		public RESULT getParameterFloat(int index, out float value)
		{
			return DSP.FMOD5_DSP_GetParameterFloat(this.handle, index, out value, IntPtr.Zero, 0);
		}

		public RESULT getParameterInt(int index, out int value)
		{
			return DSP.FMOD5_DSP_GetParameterInt(this.handle, index, out value, IntPtr.Zero, 0);
		}

		public RESULT getParameterBool(int index, out bool value)
		{
			return DSP.FMOD5_DSP_GetParameterBool(this.handle, index, out value, IntPtr.Zero, 0);
		}

		public RESULT getParameterData(int index, out IntPtr data, out uint length)
		{
			return DSP.FMOD5_DSP_GetParameterData(this.handle, index, out data, out length, IntPtr.Zero, 0);
		}

		public RESULT getNumParameters(out int numparams)
		{
			return DSP.FMOD5_DSP_GetNumParameters(this.handle, out numparams);
		}

		public RESULT getParameterInfo(int index, out DSP_PARAMETER_DESC desc)
		{
			return DSP.FMOD5_DSP_GetParameterInfo(this.handle, index, out desc);
		}

		public RESULT getDataParameterIndex(int datatype, out int index)
		{
			return DSP.FMOD5_DSP_GetDataParameterIndex(this.handle, datatype, out index);
		}

		public RESULT showConfigDialog(IntPtr hwnd, bool show)
		{
			return DSP.FMOD5_DSP_ShowConfigDialog(this.handle, hwnd, show);
		}

		public RESULT getInfo(out string name, out uint version, out int channels, out int configwidth, out int configheight)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(32);
			RESULT result = DSP.FMOD5_DSP_GetInfo(this.handle, intPtr, out version, out channels, out configwidth, out configheight);
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				name = freeHelper.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getInfo(out uint version, out int channels, out int configwidth, out int configheight)
		{
			return DSP.FMOD5_DSP_GetInfo(this.handle, IntPtr.Zero, out version, out channels, out configwidth, out configheight);
		}

		public RESULT getType(out DSP_TYPE type)
		{
			return DSP.FMOD5_DSP_GetType(this.handle, out type);
		}

		public RESULT getIdle(out bool idle)
		{
			return DSP.FMOD5_DSP_GetIdle(this.handle, out idle);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return DSP.FMOD5_DSP_SetUserData(this.handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return DSP.FMOD5_DSP_GetUserData(this.handle, out userdata);
		}

		public RESULT setMeteringEnabled(bool inputEnabled, bool outputEnabled)
		{
			return DSP.FMOD5_DSP_SetMeteringEnabled(this.handle, inputEnabled, outputEnabled);
		}

		public RESULT getMeteringEnabled(out bool inputEnabled, out bool outputEnabled)
		{
			return DSP.FMOD5_DSP_GetMeteringEnabled(this.handle, out inputEnabled, out outputEnabled);
		}

		public RESULT getMeteringInfo(IntPtr zero, out DSP_METERING_INFO outputInfo)
		{
			return DSP.FMOD5_DSP_GetMeteringInfo(this.handle, zero, out outputInfo);
		}

		public RESULT getMeteringInfo(out DSP_METERING_INFO inputInfo, IntPtr zero)
		{
			return DSP.FMOD5_DSP_GetMeteringInfo(this.handle, out inputInfo, zero);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_Release(IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetSystemObject(IntPtr dsp, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_AddInput(IntPtr dsp, IntPtr target, out IntPtr connection, DSPCONNECTION_TYPE type);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_DisconnectAll(IntPtr dsp, bool inputs, bool outputs);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetNumInputs(IntPtr dsp, out int numinputs);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetNumOutputs(IntPtr dsp, out int numoutputs);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetInput(IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetOutput(IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetActive(IntPtr dsp, bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetActive(IntPtr dsp, out bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetBypass(IntPtr dsp, bool bypass);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetBypass(IntPtr dsp, out bool bypass);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetWetDryMix(IntPtr dsp, out float prewet, out float postwet, out float dry);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetChannelFormat(IntPtr dsp, CHANNELMASK channelmask, int numchannels, SPEAKERMODE source_speakermode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetChannelFormat(IntPtr dsp, out CHANNELMASK channelmask, out int numchannels, out SPEAKERMODE source_speakermode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetOutputChannelFormat(IntPtr dsp, CHANNELMASK inmask, int inchannels, SPEAKERMODE inspeakermode, out CHANNELMASK outmask, out int outchannels, out SPEAKERMODE outspeakermode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_Reset(IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetParameterFloat(IntPtr dsp, int index, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetParameterInt(IntPtr dsp, int index, int value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetParameterBool(IntPtr dsp, int index, bool value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetParameterData(IntPtr dsp, int index, IntPtr data, uint length);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetParameterFloat(IntPtr dsp, int index, out float value, IntPtr valuestr, int valuestrlen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetParameterInt(IntPtr dsp, int index, out int value, IntPtr valuestr, int valuestrlen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetParameterBool(IntPtr dsp, int index, out bool value, IntPtr valuestr, int valuestrlen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetParameterData(IntPtr dsp, int index, out IntPtr data, out uint length, IntPtr valuestr, int valuestrlen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetNumParameters(IntPtr dsp, out int numparams);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetParameterInfo(IntPtr dsp, int index, out DSP_PARAMETER_DESC desc);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetDataParameterIndex(IntPtr dsp, int datatype, out int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, bool show);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetInfo(IntPtr dsp, IntPtr name, out uint version, out int channels, out int configwidth, out int configheight);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetType(IntPtr dsp, out DSP_TYPE type);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetIdle(IntPtr dsp, out bool idle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_SetUserData(IntPtr dsp, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_DSP_GetUserData(IntPtr dsp, out IntPtr userdata);

		[DllImport("fmodstudio")]
		public static extern RESULT FMOD5_DSP_SetMeteringEnabled(IntPtr dsp, bool inputEnabled, bool outputEnabled);

		[DllImport("fmodstudio")]
		public static extern RESULT FMOD5_DSP_GetMeteringEnabled(IntPtr dsp, out bool inputEnabled, out bool outputEnabled);

		[DllImport("fmodstudio")]
		public static extern RESULT FMOD5_DSP_GetMeteringInfo(IntPtr dsp, IntPtr zero, out DSP_METERING_INFO outputInfo);

		[DllImport("fmodstudio")]
		public static extern RESULT FMOD5_DSP_GetMeteringInfo(IntPtr dsp, out DSP_METERING_INFO inputInfo, IntPtr zero);

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
