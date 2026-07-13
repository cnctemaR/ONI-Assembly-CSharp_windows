using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/LightmapSettings.h")]
	[StaticAccessor("GetLightmapSettings()")]
	public sealed class LightmapSettings : Object
	{
		private LightmapSettings()
		{
		}

		public static extern LightmapData[] lightmaps
		{
			[FreeFunction]
			[MethodImpl(MethodImplOptions.InternalCall)]
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get;
			[FreeFunction(ThrowsException = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			[param: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			set;
		}

		public static extern LightmapsMode lightmapsMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(ThrowsException = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static LightProbes lightProbes
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<LightProbes>(LightmapSettings.get_lightProbes_Injected());
			}
			[FreeFunction]
			[NativeName("SetLightProbes")]
			set
			{
				LightmapSettings.set_lightProbes_Injected(Object.MarshalledUnityObject.Marshal<LightProbes>(value));
			}
		}

		[NativeName("ResetAndAwakeFromLoad")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Reset();

		[Obsolete("Use lightmapsMode instead.", false)]
		public static LightmapsModeLegacy lightmapsModeLegacy
		{
			get
			{
				return LightmapsModeLegacy.Single;
			}
			set
			{
			}
		}

		[Obsolete("Use QualitySettings.desiredColorSpace instead.", false)]
		public static ColorSpace bakedColorSpace
		{
			get
			{
				return QualitySettings.desiredColorSpace;
			}
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_lightProbes_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lightProbes_Injected(IntPtr value);
	}
}
