using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEditor.Experimental
{
	/// <summary>
	///   <para>Experimental render settings features.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/RenderSettings.h")]
	[StaticAccessor("GetRenderSettings()", StaticAccessorType.Dot)]
	public sealed class RenderSettings
	{
		/// <summary>
		///   <para>If enabled, ambient trilight will be sampled using the old radiance sampling method.</para>
		/// </summary>
		public static extern bool useRadianceAmbientProbe
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
