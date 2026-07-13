using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Camera/Skybox.h")]
	public sealed class Skybox : Behaviour
	{
		public Material material
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Skybox>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Material>(Skybox.get_material_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Skybox>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Skybox.set_material_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(value));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_material_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_material_Injected(IntPtr _unity_self, IntPtr value);
	}
}
