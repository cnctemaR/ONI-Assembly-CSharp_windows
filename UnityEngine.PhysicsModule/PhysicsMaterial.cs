using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/PhysicsMaterial.h")]
	public class PhysicsMaterial : Object
	{
		public PhysicsMaterial()
		{
			PhysicsMaterial.Internal_CreateDynamicsMaterial(this, "DynamicMaterial");
		}

		public PhysicsMaterial(string name)
		{
			PhysicsMaterial.Internal_CreateDynamicsMaterial(this, name);
		}

		private unsafe static void Internal_CreateDynamicsMaterial([Writable] PhysicsMaterial mat, string name)
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
				PhysicsMaterial.Internal_CreateDynamicsMaterial_Injected(mat, ref managedSpanWrapper);
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
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial.get_bounciness_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial.set_bounciness_Injected(intPtr, value);
			}
		}

		public float dynamicFriction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial.get_dynamicFriction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial.set_dynamicFriction_Injected(intPtr, value);
			}
		}

		public float staticFriction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial.get_staticFriction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial.set_staticFriction_Injected(intPtr, value);
			}
		}

		public PhysicsMaterialCombine frictionCombine
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial.get_frictionCombine_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial.set_frictionCombine_Injected(intPtr, value);
			}
		}

		public PhysicsMaterialCombine bounceCombine
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PhysicsMaterial.get_bounceCombine_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PhysicsMaterial>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PhysicsMaterial.set_bounceCombine_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateDynamicsMaterial_Injected([Writable] PhysicsMaterial mat, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_bounciness_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bounciness_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_dynamicFriction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_dynamicFriction_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_staticFriction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_staticFriction_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PhysicsMaterialCombine get_frictionCombine_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_frictionCombine_Injected(IntPtr _unity_self, PhysicsMaterialCombine value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PhysicsMaterialCombine get_bounceCombine_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bounceCombine_Injected(IntPtr _unity_self, PhysicsMaterialCombine value);
	}
}
