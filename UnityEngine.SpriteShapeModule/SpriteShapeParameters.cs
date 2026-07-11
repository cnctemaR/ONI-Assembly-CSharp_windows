using System;

namespace UnityEngine.Experimental.U2D
{
	/// <summary>
	///   <para>Input parameters for the SpriteShape tessellator.</para>
	/// </summary>
	public struct SpriteShapeParameters
	{
		/// <summary>
		///   <para>The world space transform of the game object used for calculating the UVs of the fill texture.</para>
		/// </summary>
		public Matrix4x4 transform;

		/// <summary>
		///   <para>The texture to be used for the fill of the SpriteShape.</para>
		/// </summary>
		public Texture2D fillTexture;

		/// <summary>
		///   <para>The scale to be used to calculate the UVs of the fill texture.</para>
		/// </summary>
		public uint fillScale;

		/// <summary>
		///   <para>The tessellation quality of the input Spline that determines the complexity of the mesh.</para>
		/// </summary>
		public uint splineDetail;

		/// <summary>
		///   <para>The threshold of the angle that indicates whether it is a corner or not.</para>
		/// </summary>
		public float angleThreshold;

		/// <summary>
		///   <para>The local displacement of the Sprite when tessellated.</para>
		/// </summary>
		public float borderPivot;

		/// <summary>
		///   <para>The threshold of the angle that decides if it should be tessellated as a curve or a corner.</para>
		/// </summary>
		public float bevelCutoff;

		/// <summary>
		///   <para>The radius of the curve to be tessellated.</para>
		/// </summary>
		public float bevelSize;

		/// <summary>
		///   <para>If true, the Shape will be tessellated as a closed form.</para>
		/// </summary>
		public bool carpet;

		/// <summary>
		///   <para>If enabled the tessellator will consider creating corners based on the various input parameters.</para>
		/// </summary>
		public bool smartSprite;

		/// <summary>
		///   <para>If enabled, the tessellator will adapt the size of the quads based on the height of the edge.</para>
		/// </summary>
		public bool adaptiveUV;

		/// <summary>
		///   <para>The borders to be used for calculating the uv of the edges based on the border info found in Sprites.</para>
		/// </summary>
		public bool spriteBorders;
	}
}
