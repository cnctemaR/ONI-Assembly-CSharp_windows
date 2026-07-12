using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public static class MarshalHelper
	{
		public static int SizeOf(Type t)
		{
			return Marshal.SizeOf(t);
		}

		public static object PtrToStructure(IntPtr ptr, Type structureType)
		{
			return Marshal.PtrToStructure(ptr, structureType);
		}
	}
}
