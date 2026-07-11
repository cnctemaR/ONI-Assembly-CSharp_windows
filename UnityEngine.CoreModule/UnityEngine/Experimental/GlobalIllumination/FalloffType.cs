using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	/// <summary>
	///   <para>Available falloff models for baking.</para>
	/// </summary>
	public enum FalloffType : byte
	{
		/// <summary>
		///   <para>Inverse squared distance falloff model.</para>
		/// </summary>
		InverseSquared,
		/// <summary>
		///   <para>Inverse squared distance falloff model (without smooth range attenuation).</para>
		/// </summary>
		InverseSquaredNoRangeAttenuation,
		/// <summary>
		///   <para>Linear falloff model.</para>
		/// </summary>
		Linear,
		/// <summary>
		///   <para>Quadratic falloff model.</para>
		/// </summary>
		Legacy,
		/// <summary>
		///   <para>Falloff model is undefined.</para>
		/// </summary>
		Undefined
	}
}
