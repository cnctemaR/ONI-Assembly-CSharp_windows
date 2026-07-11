using System;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	/// <summary>
	///   <para>Used to specify allocation type for NativeArray.</para>
	/// </summary>
	[UsedByNativeCode]
	public enum Allocator
	{
		/// <summary>
		///   <para>Invalid allocation.</para>
		/// </summary>
		Invalid,
		/// <summary>
		///   <para>No allocation.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Temporary allocation.</para>
		/// </summary>
		Temp,
		/// <summary>
		///   <para>Temporary job allocation.</para>
		/// </summary>
		TempJob,
		/// <summary>
		///   <para>Persistent allocation.</para>
		/// </summary>
		Persistent
	}
}
