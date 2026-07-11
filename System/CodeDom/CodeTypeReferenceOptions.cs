using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum CodeTypeReferenceOptions
	{
		GlobalReference = 1,
		GenericTypeParameter = 2
	}
}
