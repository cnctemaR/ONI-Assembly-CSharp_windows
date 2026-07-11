using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>NativeContainerIsAtomicWriteOnlyAttribute.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class NativeContainerIsAtomicWriteOnlyAttribute : Attribute
	{
	}
}
