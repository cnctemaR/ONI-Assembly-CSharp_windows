using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>By default native containers are tracked by the safety system to avoid race conditions. The safety system encapsulates the best practices and catches many race condition bugs from the start.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NativeDisableContainerSafetyRestrictionAttribute : Attribute
	{
	}
}
