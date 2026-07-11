using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Shader pass type for Unity's lighting pipeline.</para>
	/// </summary>
	public enum PassType
	{
		/// <summary>
		///   <para>Regular shader pass that does not interact with lighting.</para>
		/// </summary>
		Normal,
		/// <summary>
		///   <para>Legacy vertex-lit shader pass.</para>
		/// </summary>
		Vertex,
		/// <summary>
		///   <para>Legacy vertex-lit shader pass, with mobile lightmaps.</para>
		/// </summary>
		VertexLM,
		/// <summary>
		///   <para>Legacy vertex-lit shader pass, with desktop (RGBM) lightmaps.</para>
		/// </summary>
		[Obsolete("VertexLMRGBM PassType is obsolete. Please use VertexLM PassType together with DecodeLightmap shader function.")]
		VertexLMRGBM,
		/// <summary>
		///   <para>Forward rendering base pass.</para>
		/// </summary>
		ForwardBase,
		/// <summary>
		///   <para>Forward rendering additive pixel light pass.</para>
		/// </summary>
		ForwardAdd,
		/// <summary>
		///   <para>Legacy deferred lighting (light pre-pass) base pass.</para>
		/// </summary>
		LightPrePassBase,
		/// <summary>
		///   <para>Legacy deferred lighting (light pre-pass) final pass.</para>
		/// </summary>
		LightPrePassFinal,
		/// <summary>
		///   <para>Shadow caster &amp; depth texure shader pass.</para>
		/// </summary>
		ShadowCaster,
		/// <summary>
		///   <para>Deferred Shading shader pass.</para>
		/// </summary>
		Deferred = 10,
		/// <summary>
		///   <para>Shader pass used to generate the albedo and emissive values used as input to lightmapping.</para>
		/// </summary>
		Meta,
		/// <summary>
		///   <para>Motion vector render pass.</para>
		/// </summary>
		MotionVectors,
		/// <summary>
		///   <para>Custom scriptable pipeline.</para>
		/// </summary>
		ScriptableRenderPipeline,
		/// <summary>
		///   <para>Custom scriptable pipeline when lightmode is set to default unlit or no light mode is set.</para>
		/// </summary>
		ScriptableRenderPipelineDefaultUnlit
	}
}
