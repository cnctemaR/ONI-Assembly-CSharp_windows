using System;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	/// <summary>
	///   <para>DeallocateOnJobCompletionAttribute.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class DeallocateOnJobCompletionAttribute : Attribute
	{
	}
}
