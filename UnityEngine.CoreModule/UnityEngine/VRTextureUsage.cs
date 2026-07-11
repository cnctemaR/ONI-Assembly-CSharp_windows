using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>This enum describes how the RenderTexture is used as a VR eye texture. Instead of using the values of this enum manually, use the value returned by XR.XRSettings.eyeTextureDesc|eyeTextureDesc or other VR functions returning a RenderTextureDescriptor.</para>
	/// </summary>
	public enum VRTextureUsage
	{
		/// <summary>
		///   <para>The RenderTexture is not a VR eye texture. No special rendering behavior will occur.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>This texture corresponds to a single eye on a stereoscopic display.</para>
		/// </summary>
		OneEye,
		/// <summary>
		///   <para>This texture corresponds to two eyes on a stereoscopic display. This will be taken into account when using Graphics.Blit and other rendering functions.</para>
		/// </summary>
		TwoEyes
	}
}
