using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The emission shape (Shuriken).</para>
	/// </summary>
	public enum ParticleSystemShapeType
	{
		/// <summary>
		///   <para>Emit from a sphere.</para>
		/// </summary>
		Sphere,
		/// <summary>
		///   <para>Emit from the surface of a sphere.</para>
		/// </summary>
		[Obsolete("SphereShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
		SphereShell,
		/// <summary>
		///   <para>Emit from a half-sphere.</para>
		/// </summary>
		Hemisphere,
		/// <summary>
		///   <para>Emit from the surface of a half-sphere.</para>
		/// </summary>
		[Obsolete("HemisphereShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
		HemisphereShell,
		/// <summary>
		///   <para>Emit from the base of a cone.</para>
		/// </summary>
		Cone,
		/// <summary>
		///   <para>Emit from the volume of a box.</para>
		/// </summary>
		Box,
		/// <summary>
		///   <para>Emit from a mesh.</para>
		/// </summary>
		Mesh,
		/// <summary>
		///   <para>Emit from the base surface of a cone.</para>
		/// </summary>
		[Obsolete("ConeShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
		ConeShell,
		/// <summary>
		///   <para>Emit from a cone.</para>
		/// </summary>
		ConeVolume,
		/// <summary>
		///   <para>Emit from the surface of a cone.</para>
		/// </summary>
		[Obsolete("ConeVolumeShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
		ConeVolumeShell,
		/// <summary>
		///   <para>Emit from a circle.</para>
		/// </summary>
		Circle,
		/// <summary>
		///   <para>Emit from the edge of a circle.</para>
		/// </summary>
		[Obsolete("CircleEdge is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
		CircleEdge,
		/// <summary>
		///   <para>Emit from an edge.</para>
		/// </summary>
		SingleSidedEdge,
		/// <summary>
		///   <para>Emit from a mesh renderer.</para>
		/// </summary>
		MeshRenderer,
		/// <summary>
		///   <para>Emit from a skinned mesh renderer.</para>
		/// </summary>
		SkinnedMeshRenderer,
		/// <summary>
		///   <para>Emit from the surface of a box.</para>
		/// </summary>
		BoxShell,
		/// <summary>
		///   <para>Emit from the edges of a box.</para>
		/// </summary>
		BoxEdge,
		/// <summary>
		///   <para>Emit from a Donut.</para>
		/// </summary>
		Donut,
		/// <summary>
		///   <para>Emit from a rectangle.</para>
		/// </summary>
		Rectangle,
		/// <summary>
		///   <para>Emit from a sprite.</para>
		/// </summary>
		Sprite,
		/// <summary>
		///   <para>Emit from a sprite renderer.</para>
		/// </summary>
		SpriteRenderer
	}
}
