using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	[AttributeUsage(AttributeTargets.Struct)]
	[RequiredByNativeCode]
	[Obsolete("Use NativeSetThreadIndexAttribute instead")]
	public sealed class NativeContainerNeedsThreadIndexAttribute : Attribute
	{
	}
}
