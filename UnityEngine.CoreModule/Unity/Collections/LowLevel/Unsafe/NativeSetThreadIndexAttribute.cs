using System;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>This attribute can inject a worker thread index into an int on the job struct. This is usually used in the implementation of atomic containers. The index is guaranteed to be unique to any other job that might be running in parallel.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NativeSetThreadIndexAttribute : Attribute
	{
	}
}
