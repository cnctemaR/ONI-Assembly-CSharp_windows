using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum AssemblyBuilderAccess
	{
		Run = 1,
		Save = 2,
		RunAndSave = 3,
		ReflectionOnly = 6,
		RunAndCollect = 9
	}
}
