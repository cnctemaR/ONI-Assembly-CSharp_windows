using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct ParameterInstance
	{
		public RESULT getDescription(out PARAMETER_DESCRIPTION description)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_GetDescription(this.handle, out description);
		}

		public RESULT getValue(out float value)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_GetValue(this.handle, out value);
		}

		public RESULT setValue(float value)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_SetValue(this.handle, value);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_ParameterInstance_IsValid(IntPtr parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetDescription(IntPtr parameter, out PARAMETER_DESCRIPTION description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetValue(IntPtr parameter, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_SetValue(IntPtr parameter, float value);

		public bool hasHandle()
		{
			return this.handle != IntPtr.Zero;
		}

		public void clearHandle()
		{
			this.handle = IntPtr.Zero;
		}

		public bool isValid()
		{
			return this.hasHandle() && ParameterInstance.FMOD_Studio_ParameterInstance_IsValid(this.handle);
		}

		public IntPtr handle;
	}
}
