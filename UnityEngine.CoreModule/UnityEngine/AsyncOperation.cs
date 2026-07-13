using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Misc/AsyncOperation.h")]
	[NativeHeader("Runtime/Export/Scripting/AsyncOperation.bindings.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class AsyncOperation : YieldInstruction
	{
		[StaticAccessor("AsyncOperationBindings", StaticAccessorType.DoubleColon)]
		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalDestroy(IntPtr ptr);

		[NativeMethod(IsThreadSafe = true)]
		[StaticAccessor("AsyncOperationBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetManagedObject(IntPtr ptr, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] AsyncOperation self);

		public AsyncOperation()
		{
		}

		protected AsyncOperation(IntPtr ptr)
		{
			bool flag = ptr == IntPtr.Zero;
			if (!flag)
			{
				AsyncOperation.InternalSetManagedObject(ptr, this);
				this.m_Ptr = ptr;
			}
		}

		public bool isDone
		{
			[NativeMethod("IsDone")]
			get
			{
				IntPtr intPtr = AsyncOperation.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AsyncOperation.get_isDone_Injected(intPtr);
			}
		}

		public float progress
		{
			[NativeMethod("GetProgress")]
			get
			{
				IntPtr intPtr = AsyncOperation.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AsyncOperation.get_progress_Injected(intPtr);
			}
		}

		public int priority
		{
			[NativeMethod("GetPriority")]
			get
			{
				IntPtr intPtr = AsyncOperation.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AsyncOperation.get_priority_Injected(intPtr);
			}
			[NativeMethod("SetPriority")]
			set
			{
				IntPtr intPtr = AsyncOperation.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AsyncOperation.set_priority_Injected(intPtr, value);
			}
		}

		public bool allowSceneActivation
		{
			[NativeMethod("GetAllowSceneActivation")]
			get
			{
				IntPtr intPtr = AsyncOperation.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AsyncOperation.get_allowSceneActivation_Injected(intPtr);
			}
			[NativeMethod("SetAllowSceneActivation")]
			set
			{
				IntPtr intPtr = AsyncOperation.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AsyncOperation.set_allowSceneActivation_Injected(intPtr, value);
			}
		}

		~AsyncOperation()
		{
			AsyncOperation.InternalDestroy(this.m_Ptr);
		}

		[RequiredByNativeCode]
		internal void InvokeCompletionEvent()
		{
			bool flag = this.m_completeCallback != null;
			if (flag)
			{
				this.m_completeCallback(this);
				this.m_completeCallback = null;
			}
		}

		public event Action<AsyncOperation> completed
		{
			add
			{
				bool isDone = this.isDone;
				if (isDone)
				{
					value(this);
				}
				else
				{
					this.m_completeCallback = (Action<AsyncOperation>)Delegate.Combine(this.m_completeCallback, value);
				}
			}
			remove
			{
				this.m_completeCallback = (Action<AsyncOperation>)Delegate.Remove(this.m_completeCallback, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isDone_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_progress_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_priority_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_priority_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowSceneActivation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowSceneActivation_Injected(IntPtr _unity_self, bool value);

		[VisibleToOtherModules]
		internal IntPtr m_Ptr;

		private Action<AsyncOperation> m_completeCallback;

		internal static class BindingsMarshaller
		{
			public static AsyncOperation ConvertToManaged(IntPtr ptr)
			{
				return new AsyncOperation(ptr);
			}

			public static IntPtr ConvertToNative(AsyncOperation asyncOperation)
			{
				return asyncOperation.m_Ptr;
			}
		}
	}
}
