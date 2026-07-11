using System;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Enumeration describing the AR rendering mode used with XR.ARBackgroundRenderer.</para>
	/// </summary>
	public enum ARRenderMode
	{
		/// <summary>
		///   <para>The standard background is rendered. (Skybox, Solid Color, etc.)</para>
		/// </summary>
		StandardBackground,
		/// <summary>
		///   <para>The material associated with XR.ARBackgroundRenderer is being rendered as the background.</para>
		/// </summary>
		MaterialAsBackground
	}
}
