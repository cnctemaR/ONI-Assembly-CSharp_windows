using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Struct, Inherited = true)]
	[Serializable]
	public sealed class NativeCppClassAttribute : Attribute
	{
	}
}
