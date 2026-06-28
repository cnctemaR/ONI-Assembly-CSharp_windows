using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public enum CallingConvention
	{
		Winapi = 1,
		Cdecl,
		StdCall,
		ThisCall,
		FastCall
	}
}
