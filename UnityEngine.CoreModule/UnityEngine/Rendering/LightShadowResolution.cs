using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Shadow resolution options for a Light.</para>
	/// </summary>
	public enum LightShadowResolution
	{
		/// <summary>
		///   <para>Use resolution from QualitySettings (default).</para>
		/// </summary>
		FromQualitySettings = -1,
		/// <summary>
		///   <para>Low shadow map resolution.</para>
		/// </summary>
		Low,
		/// <summary>
		///   <para>Medium shadow map resolution.</para>
		/// </summary>
		Medium,
		/// <summary>
		///   <para>High shadow map resolution.</para>
		/// </summary>
		High,
		/// <summary>
		///   <para>Very high shadow map resolution.</para>
		/// </summary>
		VeryHigh
	}
}
