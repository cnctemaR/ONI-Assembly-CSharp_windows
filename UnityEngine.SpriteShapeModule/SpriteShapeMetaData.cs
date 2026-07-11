using System;

namespace UnityEngine.Experimental.U2D
{
	/// <summary>
	///   <para>Additional data about the shape's control point. This is useful during tessellation of the shape.</para>
	/// </summary>
	public struct SpriteShapeMetaData
	{
		/// <summary>
		///   <para>The height of the tessellated edge.</para>
		/// </summary>
		public float height;

		/// <summary>
		///   <para>The threshold of the angle that decides if it should be tessellated as a curve or a corner.</para>
		/// </summary>
		public float bevelCutoff;

		/// <summary>
		///   <para>The radius of the curve to be tessellated.</para>
		/// </summary>
		public float bevelSize;

		/// <summary>
		///   <para>The Sprite to be used for a particular edge.</para>
		/// </summary>
		public uint spriteIndex;

		/// <summary>
		///   <para>True will indicate that this point should be tessellated as a corner or a continuous line otherwise.</para>
		/// </summary>
		public bool corner;
	}
}
