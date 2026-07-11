using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Graphics device API type.</para>
	/// </summary>
	[UsedByNativeCode]
	public enum GraphicsDeviceType
	{
		/// <summary>
		///   <para>OpenGL 2.x graphics API. (deprecated, only available on Linux and MacOSX)</para>
		/// </summary>
		[Obsolete("OpenGL2 is no longer supported in Unity 5.5+")]
		OpenGL2,
		/// <summary>
		///   <para>Direct3D 9 graphics API.</para>
		/// </summary>
		[Obsolete("Direct3D 9 is no longer supported in Unity 2017.2+")]
		Direct3D9,
		/// <summary>
		///   <para>Direct3D 11 graphics API.</para>
		/// </summary>
		Direct3D11,
		/// <summary>
		///   <para>PlayStation 3 graphics API.</para>
		/// </summary>
		[Obsolete("PS3 is no longer supported in Unity 5.5+")]
		PlayStation3,
		/// <summary>
		///   <para>No graphics API.</para>
		/// </summary>
		Null,
		[Obsolete("Xbox360 is no longer supported in Unity 5.5+")]
		Xbox360 = 6,
		/// <summary>
		///   <para>OpenGL ES 2.0 graphics API.</para>
		/// </summary>
		OpenGLES2 = 8,
		/// <summary>
		///   <para>OpenGL ES 3.0 graphics API.</para>
		/// </summary>
		OpenGLES3 = 11,
		/// <summary>
		///   <para>PlayStation Vita graphics API.</para>
		/// </summary>
		PlayStationVita,
		/// <summary>
		///   <para>PlayStation 4 graphics API.</para>
		/// </summary>
		PlayStation4,
		/// <summary>
		///   <para>Xbox One graphics API using Direct3D 11.</para>
		/// </summary>
		XboxOne,
		/// <summary>
		///   <para>PlayStation Mobile (PSM) graphics API.</para>
		/// </summary>
		[Obsolete("PlayStationMobile is no longer supported in Unity 5.3+")]
		PlayStationMobile,
		/// <summary>
		///   <para>iOS Metal graphics API.</para>
		/// </summary>
		Metal,
		/// <summary>
		///   <para>OpenGL (Core profile - GL3 or later) graphics API.</para>
		/// </summary>
		OpenGLCore,
		/// <summary>
		///   <para>Direct3D 12 graphics API.</para>
		/// </summary>
		Direct3D12,
		/// <summary>
		///   <para>Nintendo 3DS graphics API.</para>
		/// </summary>
		N3DS,
		/// <summary>
		///   <para>Vulkan (EXPERIMENTAL).</para>
		/// </summary>
		Vulkan = 21,
		/// <summary>
		///   <para>Nintendo Switch graphics API.</para>
		/// </summary>
		Switch,
		/// <summary>
		///   <para>Xbox One graphics API using Direct3D 12.</para>
		/// </summary>
		XboxOneD3D12
	}
}
