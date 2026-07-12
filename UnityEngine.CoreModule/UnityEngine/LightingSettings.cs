using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[PreventReadOnlyInstanceModification]
	[NativeHeader("Runtime/Graphics/LightingSettings.h")]
	public sealed class LightingSettings : Object
	{
		[RequiredByNativeCode]
		internal void LightingSettingsDontStripMe()
		{
		}

		public LightingSettings()
		{
			LightingSettings.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] LightingSettings self);

		[NativeName("EnableBakedLightmaps")]
		public extern bool bakedGI
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("EnableRealtimeLightmaps")]
		public extern bool realtimeGI
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("RealtimeEnvironmentLighting")]
		public extern bool realtimeEnvironmentLighting
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
