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
			if (result != RESULT.OK)
			{
				return result;
			}
			system = new FMOD.System(intPtr);
			return result;
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Create(out IntPtr system);
	}
}
