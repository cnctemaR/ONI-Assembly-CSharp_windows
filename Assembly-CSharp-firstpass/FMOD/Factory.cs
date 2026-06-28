using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public class Factory
	{
		public static RESULT System_Create(out FMOD.System system)
		{
			system = null;
			IntPtr intPtr = 0;
			RESULT result = Factory.FMOD5_System_Create(out intPtr);
			RESULT result2;
			if (result != RESULT.OK)
			{
				result2 = result;
			}
			else
			{
				system = new FMOD.System(intPtr);
				result2 = result;
			}
			return result2;
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Create(out IntPtr system);
	}
}
