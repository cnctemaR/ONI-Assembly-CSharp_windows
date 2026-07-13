using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType(Header = "Modules/RenderAs2D/Public/RenderAs2D.h")]
	[RequireComponent(typeof(Transform))]
	[AddComponentMenu("")]
	internal sealed class RenderAs2D : Renderer
	{
		internal void Init(Component owner)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderAs2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderAs2D.Init_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Component>(owner));
		}

		internal bool IsOwner(Component owner)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderAs2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RenderAs2D.IsOwner_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Component>(owner));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Init_Injected(IntPtr _unity_self, IntPtr owner);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsOwner_Injected(IntPtr _unity_self, IntPtr owner);
	}
}
