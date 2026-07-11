using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Asynchronous operation coroutine.</para>
	/// </summary>
	[RequiredByNativeCode]
	[ThreadAndSerializationSafe]
	[NativeHeader("Runtime/Export/AsyncOperation.bindings.h")]
	[NativeHeader("Runtime/Misc/AsyncOperation.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AsyncOperation : YieldInstruction
	{
		[NativeMethod(IsThreadSafe = true)]
		[StaticAccessor("AsyncOperationBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalDestroy(IntPtr ptr);

		/// <summary>
		///   <para>Has the operation finished? (Read Only)</para>
		/// </summary>
		public extern bool isDone
		{
			[NativeMethod("IsDone")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>What's the operation's progress. (Read Only)</para>
		/// </summary>
		public extern float progress
		{
			[NativeMethod("GetProgress")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Priority lets you tweak in which order async operation calls will be performed.</para>
		/// </summary>
		public extern int priority
		{
			[NativeMethod("GetPriority")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetPriority")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Allow scenes to be activated as soon as it is ready.</para>
		/// </summary>
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
