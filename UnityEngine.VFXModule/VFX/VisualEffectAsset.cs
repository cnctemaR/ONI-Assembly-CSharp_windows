using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[NativeHeader("VFXScriptingClasses.h")]
	[UsedByNativeCode]
	[NativeHeader("Modules/VFX/Public/VisualEffectAsset.h")]
	public class VisualEffectAsset : VisualEffectObject
	{
		[FreeFunction(Name = "VisualEffectAssetBindings::GetTextureDimension", HasExplicitThis = true)]
		public TextureDimension GetTextureDimension(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffectAsset.GetTextureDimension_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectAssetBindings::GetExposedSpace", HasExplicitThis = true)]
		public VFXSpace GetExposedSpace(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffectAsset.GetExposedSpace_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectAssetBindings::GetExposedProperties", HasExplicitThis = true)]
		public void GetExposedProperties([NotNull] List<VFXExposedProperty> exposedProperties)
		{
			if (exposedProperties == null)
			{
				ThrowHelper.ThrowArgumentNullException(exposedProperties, "exposedProperties");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffectAsset.GetExposedProperties_Injected(intPtr, exposedProperties);
		}

		[FreeFunction(Name = "VisualEffectAssetBindings::GetEvents", HasExplicitThis = true)]
		public void GetEvents([NotNull] List<string> names)
		{
			if (names == null)
			{
				ThrowHelper.ThrowArgumentNullException(names, "names");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffectAsset.GetEvents_Injected(intPtr, names);
		}

		[FreeFunction(Name = "VisualEffectAssetBindings::HasSystemFromScript", HasExplicitThis = true)]
		internal bool HasSystem(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffectAsset.HasSystem_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectAssetBindings::GetSystemNamesFromScript", HasExplicitThis = true)]
		internal void GetSystemNames([NotNull] List<string> names)
		{
			if (names == null)
			{
				ThrowHelper.ThrowArgumentNullException(names, "names");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffectAsset.GetSystemNames_Injected(intPtr, names);
		}

		[FreeFunction(Name = "VisualEffectAssetBindings::GetParticleSystemNamesFromScript", HasExplicitThis = true)]
		internal void GetParticleSystemNames([NotNull] List<string> names)
		{
			if (names == null)
			{
				ThrowHelper.ThrowArgumentNullException(names, "names");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffectAsset.GetParticleSystemNames_Injected(intPtr, names);
		}

		[FreeFunction(Name = "VisualEffectAssetBindings::GetOutputEventNamesFromScript", HasExplicitThis = true)]
		internal void GetOutputEventNames([NotNull] List<string> names)
		{
			if (names == null)
			{
				ThrowHelper.ThrowArgumentNullException(names, "names");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffectAsset.GetOutputEventNames_Injected(intPtr, names);
		}

		[FreeFunction(Name = "VisualEffectAssetBindings::GetSpawnSystemNamesFromScript", HasExplicitThis = true)]
		internal void GetSpawnSystemNames([NotNull] List<string> names)
		{
			if (names == null)
			{
				ThrowHelper.ThrowArgumentNullException(names, "names");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffectAsset.GetSpawnSystemNames_Injected(intPtr, names);
		}

		public TextureDimension GetTextureDimension(string name)
		{
			return this.GetTextureDimension(Shader.PropertyToID(name));
		}

		public VFXSpace GetExposedSpace(string name)
		{
			return this.GetExposedSpace(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureDimension GetTextureDimension_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VFXSpace GetExposedSpace_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetExposedProperties_Injected(IntPtr _unity_self, List<VFXExposedProperty> exposedProperties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetEvents_Injected(IntPtr _unity_self, List<string> names);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasSystem_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSystemNames_Injected(IntPtr _unity_self, List<string> names);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetParticleSystemNames_Injected(IntPtr _unity_self, List<string> names);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOutputEventNames_Injected(IntPtr _unity_self, List<string> names);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSpawnSystemNames_Injected(IntPtr _unity_self, List<string> names);

		public const string PlayEventName = "OnPlay";

		public const string StopEventName = "OnStop";

		public static readonly int PlayEventID = Shader.PropertyToID("OnPlay");

		public static readonly int StopEventID = Shader.PropertyToID("OnStop");
	}
}
