using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	internal class MyIntPtrObject : IDisposable
	{
		internal MyIntPtrObject(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public MyIntPtrObject()
		{
			this.m_Ptr = MyIntPtrObject.Internal_Create();
		}

		public void Dispose()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				MyIntPtrObject.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public static MyIntPtrObject Create()
		{
			IntPtr intPtr = MyIntPtrObject.Create_Injected();
			return (intPtr == 0) ? null : MyIntPtrObject.BindingsMarshaller.ConvertToManaged(intPtr);
		}

		public int MemberFunction(int a)
		{
			IntPtr intPtr = MyIntPtrObject.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MyIntPtrObject.MemberFunction_Injected(intPtr, a);
		}

		public int MemberProperty
		{
			get
			{
				IntPtr intPtr = MyIntPtrObject.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return MyIntPtrObject.get_MemberProperty_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = MyIntPtrObject.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				MyIntPtrObject.set_MemberProperty_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int MemberFunction_Injected(IntPtr _unity_self, int a);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_MemberProperty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_MemberProperty_Injected(IntPtr _unity_self, int value);

		public IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(MyIntPtrObject obj)
			{
				return obj.m_Ptr;
			}

			public static MyIntPtrObject ConvertToManaged(IntPtr ptr)
			{
				return new MyIntPtrObject(ptr);
			}
		}
	}
}
