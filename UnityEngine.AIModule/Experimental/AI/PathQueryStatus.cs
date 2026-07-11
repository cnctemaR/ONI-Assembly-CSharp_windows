using System;

namespace UnityEngine.Experimental.AI
{
	/// <summary>
	///   <para>Bit flags representing the resulting state of NavMeshQuery operations.</para>
	/// </summary>
	[Flags]
	public enum PathQueryStatus
	{
		/// <summary>
		///   <para>The operation has failed.</para>
		/// </summary>
		Failure = -2147483648,
		/// <summary>
		///   <para>The operation was successful.</para>
		/// </summary>
		Success = 1073741824,
		/// <summary>
		///   <para>The operation is in progress.</para>
		/// </summary>
		InProgress = 536870912,
		/// <summary>
		///   <para>Bitmask that has 0 set for the Success, Failure and InProgress bits and 1 set for all the other flags.</para>
		/// </summary>
		StatusDetailMask = 16777215,
		/// <summary>
		///   <para>Data in the NavMesh cannot be recognized and used.</para>
		/// </summary>
		WrongMagic = 1,
		/// <summary>
		///   <para>Data in the NavMesh world has a wrong version.</para>
		/// </summary>
		WrongVersion = 2,
		/// <summary>
		///   <para>Operation ran out of memory.</para>
		/// </summary>
		OutOfMemory = 4,
		/// <summary>
		///   <para>A parameter did not contain valid information, useful for carring out the NavMesh query.</para>
		/// </summary>
		InvalidParam = 8,
		/// <summary>
		///   <para>The node buffer of the query was too small to store all results.</para>
		/// </summary>
		BufferTooSmall = 16,
		/// <summary>
		///   <para>Query ran out of node stack space during a search.</para>
		/// </summary>
		OutOfNodes = 32,
		/// <summary>
		///   <para>Query did not reach the end location, returning best guess.</para>
		/// </summary>
		PartialResult = 64
	}
}
