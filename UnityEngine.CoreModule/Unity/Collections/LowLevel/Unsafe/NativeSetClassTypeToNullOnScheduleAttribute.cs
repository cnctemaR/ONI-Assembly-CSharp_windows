using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>When this attribute is applied to a field in a job struct, the managed reference to the class will be set to null on the copy of the job struct that is passed to the job.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NativeSetClassTypeToNullOnScheduleAttribute : Attribute
	{
	}
}
