using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public enum OpCodeType
	{
		[Obsolete("This API has been deprecated.")]
		Annotation,
		Macro,
		Nternal,
		Objmodel,
		Prefix,
		Primitive
	}
}
