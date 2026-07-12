using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("Use NativeSetThreadIndexAttribute instead")]
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class NativeContainerNeedsThreadIndexAttribute : Attribute
	{
	}
}
