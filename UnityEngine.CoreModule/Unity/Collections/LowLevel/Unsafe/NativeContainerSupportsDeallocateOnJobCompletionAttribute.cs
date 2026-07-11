using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>NativeContainerSupportsDeallocateOnJobCompletionAttribute.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class NativeContainerSupportsDeallocateOnJobCompletionAttribute : Attribute
	{
	}
}
