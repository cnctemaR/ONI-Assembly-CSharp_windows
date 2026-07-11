using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>By default unsafe Pointers are not allowed to be used in a job since it is not possible for the Job Debugger to gurantee race condition free behaviour. This attribute lets you explicitly disable the restriction on a job.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NativeDisableUnsafePtrRestrictionAttribute : Attribute
	{
	}
}
