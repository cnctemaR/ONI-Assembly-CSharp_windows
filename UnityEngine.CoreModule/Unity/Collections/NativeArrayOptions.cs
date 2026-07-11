using System;

namespace Unity.Collections
{
	/// <summary>
	///   <para>NativeArrayOptions lets you control if memory should be cleared on allocation or left uninitialized.</para>
	/// </summary>
	public enum NativeArrayOptions
	{
		/// <summary>
		///   <para>Uninitialized memory can improve performance, but results in the contents of the array elements being undefined.
		/// In performance sensitive code it can make sense to use NativeArrayOptions.Uninitialized, if you are writing to the entire array right after creating it without reading any of the elements first.
		/// </para>
		/// </summary>
		UninitializedMemory,
		/// <summary>
		///   <para>Clear NativeArray memory on allocation.</para>
		/// </summary>
		ClearMemory
	}
}
