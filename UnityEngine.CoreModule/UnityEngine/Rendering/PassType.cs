using System;

namespace UnityEngine.Rendering
{
	public enum PassType
	{
		Normal,
		Vertex,
		VertexLM,
		[Obsolete("VertexLMRGBM PassType is obsolete. Please use VertexLM PassType together with DecodeLightmap shader function.")]
		VertexLMRGBM,
		ForwardBase,
		ForwardAdd,
		[Obsolete("Deferred Lighting was removed, so LightPrePassBase pass type is never used anymore.")]
		LightPrePassBase,
		[Obsolete("Deferred Lighting was removed, so LightPrePassFinal pass type is never used anymore.")]
		LightPrePassFinal,
		ShadowCaster,
		Deferred = 10,
		Meta,
		MotionVectors,
		ScriptableRenderPipeline,
		ScriptableRenderPipelineDefaultUnlit,
		GrabPass
	}
}
