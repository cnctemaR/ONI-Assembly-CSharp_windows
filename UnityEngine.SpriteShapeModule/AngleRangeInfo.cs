using System;

namespace UnityEngine.Experimental.U2D
{
	/// <summary>
	///   <para>Describes the information about the edge and how to tessellate it.</para>
	/// </summary>
	public struct AngleRangeInfo
	{
		/// <summary>
		///   <para>The minimum angle to be considered within this range.</para>
		/// </summary>
		public float start;

		/// <summary>
		///   <para>The maximum angle to be considered within this range.</para>
		/// </summary>
		public float end;

		/// <summary>
		///   <para>The render order of the edges that belong in this range.</para>
		/// </summary>
		public uint order;

		/// <summary>
		///   <para>The list of Sprites that are associated with this range.</para>
		/// </summary>
		public int[] sprites;
	}
}
