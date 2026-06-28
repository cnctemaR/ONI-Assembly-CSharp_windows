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
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				parameter_DESCRIPTION_INTERNAL.assign(out description);
				result2 = result;
			}
			return result2;
		}

		public RESULT getValue(out float value)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_GetValue(this.rawPtr, out value);
		}

		public RESULT setValue(float value)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_SetValue(this.rawPtr, value);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_ParameterInstance_IsValid(IntPtr parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetDescription(IntPtr parameter, out PARAMETER_DESCRIPTION_INTERNAL description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetValue(IntPtr parameter, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_SetValue(IntPtr parameter, float value);

		protected override bool isValidInternal()
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_IsValid(this.rawPtr);
		}
	}
}
