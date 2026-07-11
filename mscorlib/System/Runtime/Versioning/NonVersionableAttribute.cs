using System;
using System.Diagnostics;

namespace System.Runtime.Versioning
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Conditional("FEATURE_READYTORUN")]
	internal sealed class NonVersionableAttribute : Attribute
	{
	}
}
