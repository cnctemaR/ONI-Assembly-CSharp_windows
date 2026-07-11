using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public enum ProcessorArchitecture
	{
		None,
		MSIL,
		X86,
		IA64,
		Amd64,
		Arm
	}
}
