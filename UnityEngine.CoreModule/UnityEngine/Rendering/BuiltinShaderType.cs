using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Built-in shader types used by Rendering.GraphicsSettings.</para>
	/// </summary>
	public enum BuiltinShaderType
	{
		/// <summary>
		///   <para>Shader used for deferred shading calculations.</para>
		/// </summary>
		DeferredShading,
		/// <summary>
		///   <para>Shader used for deferred reflection probes.</para>
		/// </summary>
		DeferredReflections,
		/// <summary>
		///   <para>Shader used for legacy deferred lighting calculations.</para>
		/// </summary>
		LegacyDeferredLighting,
		/// <summary>
		///   <para>Shader used for screen-space cascaded shadows.</para>
		/// </summary>
		ScreenSpaceShadows,
		/// <summary>
		///   <para>Shader used for depth and normals texture when enabled on a Camera.</para>
		/// </summary>
		DepthNormals,
		/// <summary>
		///   <para>Shader used for Motion Vectors when enabled on a Camera.</para>
		/// </summary>
		MotionVectors,
		/// <summary>
		///   <para>Default shader used for light halos.</para>
		/// </summary>
		LightHalo,
		/// <summary>
		///   <para>Default shader used for lens flares.</para>
		/// </summary>
		LensFlare
	}
}
