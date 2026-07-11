using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>NativeContainerSupportsMinMaxWriteRestrictionAttribute.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class NativeContainerSupportsMinMaxWriteRestrictionAttribute : Attribute
	{
	}
}
