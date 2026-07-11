using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enumeration of all the muscles in an arm.</para>
	/// </summary>
	public enum ArmDof
	{
		/// <summary>
		///   <para>The shoulder down-up muscle.</para>
		/// </summary>
		ShoulderDownUp,
		/// <summary>
		///   <para>The shoulder front-back muscle.</para>
		/// </summary>
		ShoulderFrontBack,
		/// <summary>
		///   <para>The arm down-up muscle.</para>
		/// </summary>
		ArmDownUp,
		/// <summary>
		///   <para>The arm front-back muscle.</para>
		/// </summary>
		ArmFrontBack,
		/// <summary>
		///   <para>The arm roll in-out muscle.</para>
		/// </summary>
		ArmRollInOut,
		/// <summary>
		///   <para>The forearm close-open muscle.</para>
		/// </summary>
		ForeArmCloseOpen,
		/// <summary>
		///   <para>The forearm roll in-out muscle.</para>
		/// </summary>
		ForeArmRollInOut,
		/// <summary>
		///   <para>The hand down-up muscle.</para>
		/// </summary>
		HandDownUp,
		/// <summary>
		///   <para>The hand in-out muscle.</para>
		/// </summary>
		HandInOut,
		/// <summary>
		///   <para>The last value of the ArmDof enum.</para>
		/// </summary>
		LastArmDof
	}
}
