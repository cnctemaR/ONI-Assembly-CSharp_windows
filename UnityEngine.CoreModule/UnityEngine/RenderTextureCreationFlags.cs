using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Set of flags that control the state of a newly-created RenderTexture.</para>
	/// </summary>
	[Flags]
	public enum RenderTextureCreationFlags
	{
		/// <summary>
		///   <para>Set this flag to allocate mipmaps in the RenderTexture. See RenderTexture.useMipMap for more details.</para>
		/// </summary>
		MipMap = 1,
		/// <summary>
		///   <para>Determines whether or not mipmaps are automatically generated when the RenderTexture is modified.
		/// This flag is set by default, and has no effect if the RenderTextureCreationFlags.MipMap flag is not also set.
		/// See RenderTexture.autoGenerateMips for more details.</para>
		/// </summary>
		AutoGenerateMips = 2,
		/// <summary>
		///   <para>When this flag is set, reads and writes to this texture are converted to SRGB color space. See RenderTexture.sRGB for more details.</para>
		/// </summary>
		SRGB = 4,
		/// <summary>
		///   <para>Set this flag when the Texture is to be used as a VR eye texture. This flag is cleared by default. This flag is set on a RenderTextureDesc when it is returned from GetDefaultVREyeTextureDesc or other VR functions returning a RenderTextureDesc.</para>
		/// </summary>
		EyeTexture = 8,
		/// <summary>
		///   <para>Set this flag to enable random access writes to the RenderTexture from shaders.
		/// Normally, pixel shaders only operate on pixels they are given. Compute shaders cannot write to textures without this flag. Random write enables shaders to write to arbitrary locations on a RenderTexture.  See RenderTexture.enableRandomWrite for more details, including supported platforms.</para>
		/// </summary>
		EnableRandomWrite = 16,
		/// <summary>
		///   <para>This flag is always set internally when a RenderTexture is created from script. It has no effect when set manually from script code.</para>
		/// </summary>
		CreatedFromScript = 32,
		/// <summary>
		///   <para>Clear this flag when a RenderTexture is a VR eye texture and the device does not automatically flip the texture when being displayed. This is platform specific and
		/// It is set by default. This flag is only cleared when part of a RenderTextureDesc that is returned from GetDefaultVREyeTextureDesc or other VR functions that return a RenderTextureDesc. Currently, only Hololens eye textures need to clear this flag.</para>
		/// </summary>
		AllowVerticalFlip = 128,
		/// <summary>
		///   <para>When this flag is set, the engine will not automatically resolve the color surface.</para>
		/// </summary>
		NoResolvedColorSurface = 256,
		/// <summary>
		///   <para>Set this flag to mark this RenderTexture for Dynamic Resolution should the target platform/graphics API support Dynamic Resolution. See ScalabeBufferManager for more details.</para>
		/// </summary>
		DynamicallyScalable = 1024,
		/// <summary>
		///   <para>Setting this flag causes the RenderTexture to be bound as a multisampled texture in a shader. The flag prevents the RenderTexture from being resolved by default when RenderTexture.antiAliasing is greater than 1.</para>
		/// </summary>
		BindMS = 2048
	}
}
