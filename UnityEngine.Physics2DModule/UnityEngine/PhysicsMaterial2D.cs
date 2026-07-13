using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/Public/PhysicsMaterial2D.h")]
	public sealed class PhysicsMaterial2D : Object
	{
		public PhysicsMaterial2D()
		{
			PhysicsMaterial2D.Create_Internal(this, null);
		}

		public PhysicsMaterial2D(string name)
		{
			PhysicsMaterial2D.Create_Internal(this, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetCombinedValues(float valueA, float valueB, PhysicsMaterialCombine2D materialCombineA, PhysicsMaterialCombine2D materialCombineB);

		[NativeMethod("Create_Binding")]
		private unsafe static void Create_Internal([Writable] PhysicsMaterial2D scriptMaterial, string name)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				PhysicsMaterial2D.Create_Internal_Injected(scriptMaterial, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public float bounciness
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial2D.get_bounciness_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial2D.set_bounciness_Injected(intPtr, value);
			}
		}

		public float friction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial2D.get_friction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial2D.set_friction_Injected(intPtr, value);
			}
		}

		public PhysicsMaterialCombine2D frictionCombine
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial2D.get_frictionCombine_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial2D.set_frictionCombine_Injected(intPtr, value);
			}
		}

		public PhysicsMaterialCombine2D bounceCombine
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial2D.get_bounceCombine_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial2D.set_bounceCombine_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Create_Internal_Injected([Writable] PhysicsMaterial2D scriptMaterial, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_bounciness_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bounciness_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_friction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_friction_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PhysicsMaterialCombine2D get_frictionCombine_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_frictionCombine_Injected(IntPtr _unity_self, PhysicsMaterialCombine2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PhysicsMaterialCombine2D get_bounceCombine_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bounceCombine_Injected(IntPtr _unity_self, PhysicsMaterialCombine2D value);
	}
}
