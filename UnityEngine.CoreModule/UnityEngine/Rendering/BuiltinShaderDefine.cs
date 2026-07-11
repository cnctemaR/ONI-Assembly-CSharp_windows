using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Defines set by editor when compiling shaders, depending on target platform and tier.</para>
	/// </summary>
	public enum BuiltinShaderDefine
	{
		/// <summary>
		///   <para>UNITY_NO_DXT5nm is set when compiling shader for platform that do not support DXT5NM, meaning that normal maps will be encoded in RGB instead.</para>
		/// </summary>
		UNITY_NO_DXT5nm,
		/// <summary>
		///   <para>UNITY_NO_RGBM is set when compiling shader for platform that do not support RGBM, so dLDR will be used instead.</para>
		/// </summary>
		UNITY_NO_RGBM,
		UNITY_USE_NATIVE_HDR,
		/// <summary>
		///   <para>UNITY_ENABLE_REFLECTION_BUFFERS is set when deferred shading renders reflection probes in deferred mode. With this option set reflections are rendered into a per-pixel buffer. This is similar to the way lights are rendered into a per-pixel buffer. UNITY_ENABLE_REFLECTION_BUFFERS is on by default when using deferred shading, but you can turn it off by setting “No support” for the Deferred Reflections shader option in Graphics Settings. When the setting is off, reflection probes are rendered per-object, similar to the way forward rendering works.</para>
		/// </summary>
		UNITY_ENABLE_REFLECTION_BUFFERS,
		/// <summary>
		///   <para>UNITY_FRAMEBUFFER_FETCH_AVAILABLE is set when compiling shaders for platforms where framebuffer fetch is potentially available.</para>
		/// </summary>
		UNITY_FRAMEBUFFER_FETCH_AVAILABLE,
		/// <summary>
		///   <para>UNITY_ENABLE_NATIVE_SHADOW_LOOKUPS enables use of built-in shadow comparison samplers on OpenGL ES 2.0.</para>
		/// </summary>
		UNITY_ENABLE_NATIVE_SHADOW_LOOKUPS,
		/// <summary>
		///   <para>UNITY_METAL_SHADOWS_USE_POINT_FILTERING is set if shadow sampler should use point filtering on iOS Metal.</para>
		/// </summary>
		UNITY_METAL_SHADOWS_USE_POINT_FILTERING,
		UNITY_NO_CUBEMAP_ARRAY,
		/// <summary>
		///   <para>UNITY_NO_SCREENSPACE_SHADOWS is set when screenspace cascaded shadow maps are disabled.</para>
		/// </summary>
		UNITY_NO_SCREENSPACE_SHADOWS,
		/// <summary>
		///   <para>UNITY_USE_DITHER_MASK_FOR_ALPHABLENDED_SHADOWS is set when Semitransparent Shadows are enabled.</para>
		/// </summary>
		UNITY_USE_DITHER_MASK_FOR_ALPHABLENDED_SHADOWS,
		/// <summary>
		///   <para>UNITY_PBS_USE_BRDF1 is set if Standard Shader BRDF1 should be used.</para>
		/// </summary>
		UNITY_PBS_USE_BRDF1,
		/// <summary>
		///   <para>UNITY_PBS_USE_BRDF2 is set if Standard Shader BRDF2 should be used.</para>
		/// </summary>
		UNITY_PBS_USE_BRDF2,
		/// <summary>
		///   <para>UNITY_PBS_USE_BRDF3 is set if Standard Shader BRDF3 should be used.</para>
		/// </summary>
		UNITY_PBS_USE_BRDF3,
		/// <summary>
		///   <para>UNITY_NO_FULL_STANDARD_SHADER is set if Standard shader BRDF3 with extra simplifications should be used.</para>
		/// </summary>
		UNITY_NO_FULL_STANDARD_SHADER,
		/// <summary>
		///   <para>UNITY_SPECCUBE_BLENDING is set if Reflection Probes Box Projection is enabled.</para>
		/// </summary>
		UNITY_SPECCUBE_BOX_PROJECTION,
		/// <summary>
		///   <para>UNITY_SPECCUBE_BLENDING is set if Reflection Probes Blending is enabled.</para>
		/// </summary>
		UNITY_SPECCUBE_BLENDING,
		/// <summary>
		///   <para>UNITY_ENABLE_DETAIL_NORMALMAP is set if Detail Normal Map should be sampled if assigned.</para>
		/// </summary>
		UNITY_ENABLE_DETAIL_NORMALMAP,
		/// <summary>
		///   <para>SHADER_API_MOBILE is set when compiling shader for mobile platforms.</para>
		/// </summary>
		SHADER_API_MOBILE,
		/// <summary>
		///   <para>SHADER_API_DESKTOP is set when compiling shader for "desktop" platforms.</para>
		/// </summary>
		SHADER_API_DESKTOP,
		/// <summary>
		///   <para>UNITY_HARDWARE_TIER1 is set when compiling shaders for GraphicsTier.Tier1.</para>
		/// </summary>
		UNITY_HARDWARE_TIER1,
		/// <summary>
		///   <para>UNITY_HARDWARE_TIER2 is set when compiling shaders for GraphicsTier.Tier2.</para>
		/// </summary>
		UNITY_HARDWARE_TIER2,
		/// <summary>
		///   <para>UNITY_HARDWARE_TIER3 is set when compiling shaders for GraphicsTier.Tier3.</para>
		/// </summary>
		UNITY_HARDWARE_TIER3,
		/// <summary>
		///   <para>UNITY_COLORSPACE_GAMMA is set when compiling shaders for Gamma Color Space.</para>
		/// </summary>
		UNITY_COLORSPACE_GAMMA,
		/// <summary>
		///   <para>UNITY_LIGHT_PROBE_PROXY_VOLUME is set when Light Probe Proxy Volume feature is supported by the current graphics API and is enabled in the current Tier Settings(Graphics Settings).</para>
		/// </summary>
		UNITY_LIGHT_PROBE_PROXY_VOLUME,
		/// <summary>
		///   <para>UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS is set automatically for platforms that don't require full floating-point precision support in fragment shaders.</para>
		/// </summary>
		UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS,
		/// <summary>
		///   <para>UNITY_LIGHTMAP_DLDR_ENCODING is set when lightmap textures are using double LDR encoding to store the values in the texture.</para>
		/// </summary>
		UNITY_LIGHTMAP_DLDR_ENCODING,
		/// <summary>
		///   <para>UNITY_LIGHTMAP_RGBM_ENCODING is set when lightmap textures are using RGBM encoding to store the values in the texture.</para>
		/// </summary>
		UNITY_LIGHTMAP_RGBM_ENCODING,
		/// <summary>
		///   <para>UNITY_LIGHTMAP_FULL_HDR is set when lightmap textures are not using any encoding to store the values in the texture.</para>
		/// </summary>
		UNITY_LIGHTMAP_FULL_HDR
	}
}
