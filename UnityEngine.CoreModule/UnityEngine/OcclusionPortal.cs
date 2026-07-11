using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The portal for dynamically changing occlusion at runtime.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/OcclusionPortal.h")]
	public sealed class OcclusionPortal : Component
	{
		/// <summary>
		///   <para>Gets / sets the portal's open state.</para>
		/// </summary>
		[NativeProperty("IsOpen")]
		public extern bool open
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
