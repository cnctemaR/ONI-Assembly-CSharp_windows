using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	[StructLayout(LayoutKind.Sequential)]
	internal class MarshallingTestObject : Object
	{
		public MarshallingTestObject()
		{
			MarshallingTestObject.Internal_CreateMarshallingTestObject(this);
		}

		public int MemberFunction(int a)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MarshallingTestObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MarshallingTestObject.MemberFunction_Injected(intPtr, a);
		}

		public int MemberProperty
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MarshallingTestObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return MarshallingTestObject.get_MemberProperty_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MarshallingTestObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				MarshallingTestObject.set_MemberProperty_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_fieldBoundProp", false, TargetType.Field)]
		public int FieldBoundMemberProperty
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MarshallingTestObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return MarshallingTestObject.get_FieldBoundMemberProperty_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MarshallingTestObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				MarshallingTestObject.set_FieldBoundMemberProperty_Injected(intPtr, value);
			}
		}

		public static MarshallingTestObject Create()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(MarshallingTestObject.Create_Injected());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateMarshallingTestObject([Writable] MarshallingTestObject notSelf);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int MemberFunction_Injected(IntPtr _unity_self, int a);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_MemberProperty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_MemberProperty_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_FieldBoundMemberProperty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_FieldBoundMemberProperty_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected();

		[RequiredByNativeCode(Optional = true)]
		[RequiredMember]
		private int TestField;
	}
}
