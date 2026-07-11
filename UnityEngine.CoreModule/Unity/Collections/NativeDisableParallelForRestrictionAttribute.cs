using System;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	/// <summary>
	///   <para>NativeDisableParallelForRestrictionAttribute.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NativeDisableParallelForRestrictionAttribute : Attribute
	{
	}
}
