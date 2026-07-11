using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[ThreadAndSerializationSafe]
	[NativeHeader("Runtime/Export/AsyncOperation.bindings.h")]
	[NativeHeader("Runtime/Misc/AsyncOperation.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AsyncOperation : YieldInstruction
	{
		[StaticAccessor("AsyncOperationBindings", StaticAccessorType.DoubleColon)]
		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalDestroy(IntPtr ptr);

		public extern bool isDone
		{
			[NativeMethod("IsDone")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float progress
		{
			[NativeMethod("GetProgress")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int priority
		{
			[NativeMethod("GetPriority")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetPriority")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool allowSceneActivation
		{
			[NativeMethod("GetAllowSceneActivation")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetAllowSceneActivation")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		~AsyncOperation()
		{
			AsyncOperation.InternalDestroy(this.m_Ptr);
		}

		[RequiredByNativeCode]
		internal void InvokeCompletionEvent()
		{
			if (this.m_completeCallback != null)
			{
				this.m_completeCallback(this);
				this.m_completeCallback = null;
			}
		}

		public event Action<AsyncOperation> completed
		{
			add
			{
				if (this.isDone)
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

		internal IntPtr m_Ptr;

		private Action<AsyncOperation> m_completeCallback;
	}
}
