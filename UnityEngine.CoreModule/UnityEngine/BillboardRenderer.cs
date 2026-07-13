using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Billboard/BillboardRenderer.h")]
	public sealed class BillboardRenderer : Renderer
	{
		public BillboardAsset billboard
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<BillboardAsset>(BillboardRenderer.get_billboard_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BillboardRenderer.set_billboard_Injected(intPtr, Object.MarshalledUnityObject.Marshal<BillboardAsset>(value));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_billboard_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_billboard_Injected(IntPtr _unity_self, IntPtr value);
	}
}
