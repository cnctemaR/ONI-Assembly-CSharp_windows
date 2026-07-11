using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Allows precise control over which shadow map passes to execute Rendering.CommandBuffer objects attached using Light.AddCommandBuffer.</para>
	/// </summary>
	[Flags]
	public enum ShadowMapPass
	{
		/// <summary>
		///   <para>+X point light shadow cubemap face.</para>
		/// </summary>
		PointlightPositiveX = 1,
		/// <summary>
		///   <para>-X point light shadow cubemap face.</para>
		/// </summary>
		PointlightNegativeX = 2,
		/// <summary>
		///   <para>+Y point light shadow cubemap face.</para>
		/// </summary>
		PointlightPositiveY = 4,
		/// <summary>
		///   <para>-Y point light shadow cubemap face.</para>
		/// </summary>
		PointlightNegativeY = 8,
		/// <summary>
		///   <para>+Z point light shadow cubemap face.</para>
		/// </summary>
		PointlightPositiveZ = 16,
		/// <summary>
		///   <para>-Z point light shadow cubemap face.</para>
		/// </summary>
		PointlightNegativeZ = 32,
		/// <summary>
		///   <para>First directional shadow map cascade.</para>
		/// </summary>
		DirectionalCascade0 = 64,
		/// <summary>
		///   <para>Second directional shadow map cascade.</para>
		/// </summary>
		DirectionalCascade1 = 128,
		/// <summary>
		///   <para>Third directional shadow map cascade.</para>
		/// </summary>
		DirectionalCascade2 = 256,
		/// <summary>
		///   <para>Fourth directional shadow map cascade.</para>
		/// </summary>
		DirectionalCascade3 = 512,
		/// <summary>
		///   <para>Spotlight shadow pass.</para>
		/// </summary>
		Spotlight = 1024,
		/// <summary>
		///   <para>All point light shadow passes.</para>
		/// </summary>
		Pointlight = 63,
		/// <summary>
		///   <para>All directional shadow map passes.</para>
		/// </summary>
		Directional = 960,
		/// <summary>
		///   <para>All shadow map passes.</para>
		/// </summary>
		All = 2047
	}
}
