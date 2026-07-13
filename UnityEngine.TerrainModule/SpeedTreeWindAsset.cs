using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[ExcludeFromPreset]
	[NativeHeader("Modules/Terrain/Public/SpeedTreeWind.h")]
	public class SpeedTreeWindAsset : Object
	{
		public int Version
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpeedTreeWindAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpeedTreeWindAsset.get_Version_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpeedTreeWindAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpeedTreeWindAsset.set_Version_Injected(intPtr, value);
			}
		}

		internal SpeedTreeWindAsset(int version, SpeedTreeWindConfig9 config)
		{
			SpeedTreeWindAsset.Internal_Create(this, version, SpeedTreeWindConfig9.Serialize(config));
		}

		[NativeThrows]
		private unsafe static void Internal_Create([Writable] SpeedTreeWindAsset notSelf, int version, byte[] data)
		{
			Span<byte> span = new Span<byte>(data);
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				SpeedTreeWindAsset.Internal_Create_Injected(notSelf, version, ref managedSpanWrapper);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_Version_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_Version_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create_Injected([Writable] SpeedTreeWindAsset notSelf, int version, ref ManagedSpanWrapper data);
	}
}
