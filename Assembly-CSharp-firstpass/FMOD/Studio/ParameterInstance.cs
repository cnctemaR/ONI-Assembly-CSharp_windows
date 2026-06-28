using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public class ParameterInstance : HandleBase
	{
		public ParameterInstance(IntPtr raw)
			: base(raw)
		{
		}

		public RESULT getDescription(out PARAMETER_DESCRIPTION description)
		{
			description = default(PARAMETER_DESCRIPTION);
			PARAMETER_DESCRIPTION_INTERNAL parameter_DESCRIPTION_INTERNAL;
			RESULT result = ParameterInstance.FMOD_Studio_ParameterInstance_GetDescription(this.rawPtr, out parameter_DESCRIPTION_INTERNAL);
			if (result != RESULT.OK)
			{
				return result;
			}
			parameter_DESCRIPTION_INTERNAL.assign(out description);
			return result;
		}

		public RESULT getValue(out float value)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_GetValue(this.rawPtr, out value);
		}

		public RESULT setValue(float value)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_SetValue(this.rawPtr, value);
		}

		[DllImport("fmodstudiol")]
		private static extern bool FMOD_Studio_ParameterInstance_IsValid(IntPtr parameter);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetDescription(IntPtr parameter, out PARAMETER_DESCRIPTION_INTERNAL description);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetValue(IntPtr parameter, out float value);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD_Studio_ParameterInstance_SetValue(IntPtr parameter, float value);

		protected override bool isValidInternal()
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_IsValid(this.rawPtr);
		}
	}
}
