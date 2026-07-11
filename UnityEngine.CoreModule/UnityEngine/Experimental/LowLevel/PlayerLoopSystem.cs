using System;

namespace UnityEngine.Experimental.LowLevel
{
	/// <summary>
	///   <para>The representation of a single system being updated by the player loop in Unity.</para>
	/// </summary>
	public struct PlayerLoopSystem
	{
		/// <summary>
		///   <para>This property is used to identify which native system this belongs to, or to get the name of the managed system to show in the profiler.</para>
		/// </summary>
		public Type type;

		/// <summary>
		///   <para>A list of sub systems which run as part of this item in the player loop.</para>
		/// </summary>
		public PlayerLoopSystem[] subSystemList;

		/// <summary>
		///   <para>A managed delegate. You can set this to create a new C# entrypoint in the player loop.</para>
		/// </summary>
		public PlayerLoopSystem.UpdateFunction updateDelegate;

		/// <summary>
		///   <para>A native engine system. To get a valid value for this, you must copy it from one of the PlayerLoopSystems returned by PlayerLoop.GetDefaultPlayerLoop.</para>
		/// </summary>
		public IntPtr updateFunction;

		/// <summary>
		///   <para>The loop condition for a native engine system. To get a valid value for this, you must copy it from one of the PlayerLoopSystems returned by PlayerLoop.GetDefaultPlayerLoop.</para>
		/// </summary>
		public IntPtr loopConditionFunction;

		public delegate void UpdateFunction();
	}
}
