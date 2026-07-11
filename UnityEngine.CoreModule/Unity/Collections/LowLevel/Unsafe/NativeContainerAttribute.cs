using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>Allows you to create your own custom native container.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class NativeContainerAttribute : Attribute
	{
	}
}
