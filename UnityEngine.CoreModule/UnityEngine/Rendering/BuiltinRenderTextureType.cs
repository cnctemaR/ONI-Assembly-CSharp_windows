using System;

namespace UnityEngine.Rendering
{
	public enum BuiltinRenderTextureType
	{
		PropertyName = -4,
		BufferPtr,
		RenderTexture,
		BindableTexture,
		None,
		CurrentActive,
		CameraTarget,
		Depth,
		DepthNormals,
		ResolvedDepth,
		[Obsolete("Deferred Lighting has been removed, so PrepassNormalsSpec built-in render texture type is never used now.", false)]
		PrepassNormalsSpec = 7,
		[Obsolete("Deferred Lighting has been removed, so PrepassLight built-in render texture type is never used now.", false)]
		PrepassLight,
		[Obsolete("Deferred Lighting has been removed, so PrepassLightSpec built-in render texture type is never used now.", false)]
		PrepassLightSpec,
		GBuffer0,
		GBuffer1,
		GBuffer2,
		GBuffer3,
		Reflections,
		MotionVectors,
		GBuffer4,
		GBuffer5,
		GBuffer6,
		GBuffer7
	}
}
