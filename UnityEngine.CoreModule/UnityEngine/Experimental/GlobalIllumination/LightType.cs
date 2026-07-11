using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	/// <summary>
	///   <para>The light type.</para>
	/// </summary>
	public enum LightType : byte
	{
		/// <summary>
		///   <para>An infinite directional light.</para>
		/// </summary>
		Directional,
		/// <summary>
		///   <para>A point light emitting light in all directions.</para>
		/// </summary>
		Point,
		/// <summary>
		///   <para>A spot light emitting light in a direction with a cone shaped opening angle.</para>
		/// </summary>
		Spot,
		/// <summary>
		///   <para>A light shaped like a rectangle emitting light into the hemisphere that it is facing.</para>
		/// </summary>
		Rectangle
	}
}
