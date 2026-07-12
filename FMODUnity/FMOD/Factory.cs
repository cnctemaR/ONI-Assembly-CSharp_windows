using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct Factory
	{
		public static RESULT System_Create(out FMOD.System system)
		{
			return Factory.FMOD5_System_Create(out system.handle, 131591U);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Create(out IntPtr system, uint headerversion);
	}
}
