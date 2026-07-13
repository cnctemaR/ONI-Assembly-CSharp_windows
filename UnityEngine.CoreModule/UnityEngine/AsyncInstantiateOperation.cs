using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/GameCode/AsyncInstantiate/AsyncInstantiateOperation.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AsyncInstantiateOperation : AsyncOperation
	{
		public Object[] Result
		{
			get
			{
				return this.m_Result;
			}
		}

		[NativeMethod("IsWaitingForSceneActivation")]
		public bool IsWaitingForSceneActivation()
		{
			IntPtr intPtr = AsyncInstantiateOperation.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AsyncInstantiateOperation.IsWaitingForSceneActivation_Injected(intPtr);
		}

		[NativeMethod("WaitForCompletion")]
		public void WaitForCompletion()
		{
			IntPtr intPtr = AsyncInstantiateOperation.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AsyncInstantiateOperation.WaitForCompletion_Injected(intPtr);
		}

		[NativeMethod("Cancel")]
		public void Cancel()
		{
			IntPtr intPtr = AsyncInstantiateOperation.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AsyncInstantiateOperation.Cancel_Injected(intPtr);
		}

		[StaticAccessor("GetAsyncInstantiateManager()", StaticAccessorType.Dot)]
		internal static extern float IntegrationTimeMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public AsyncInstantiateOperation()
			: this(IntPtr.Zero, default(CancellationToken))
		{
		}

		protected AsyncInstantiateOperation(IntPtr ptr, CancellationToken cancellationToken)
			: base(ptr)
		{
			this.m_CancellationToken = CancellationTokenSource.CreateLinkedTokenSource(AsyncInstantiateOperation.s_GlobalCancellation.Token, cancellationToken).Token;
		}

		public static float GetIntegrationTimeMS()
		{
			return AsyncInstantiateOperation.IntegrationTimeMS;
		}

		public static void SetIntegrationTimeMS(float integrationTimeMS)
		{
			bool flag = integrationTimeMS <= 0f;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("integrationTimeMS", "integrationTimeMS was out of range. Must be greater than zero.");
			}
			AsyncInstantiateOperation.IntegrationTimeMS = integrationTimeMS;
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private bool IsCancellationRequested()
		{
			return this.m_CancellationToken.IsCancellationRequested;
		}

		internal virtual Object[] CreateResultArray(int size)
		{
			this.m_Result = new Object[size];
			return this.m_Result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsWaitingForSceneActivation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WaitForCompletion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Cancel_Injected(IntPtr _unity_self);

		internal static CancellationTokenSource s_GlobalCancellation = new CancellationTokenSource();

		internal Object[] m_Result;

		private CancellationToken m_CancellationToken;

		internal new static class BindingsMarshaller
		{
			public static AsyncInstantiateOperation ConvertToManaged(IntPtr ptr)
			{
				return new AsyncInstantiateOperation(ptr, CancellationToken.None);
			}

			public static IntPtr ConvertToNative(AsyncInstantiateOperation obj)
			{
				return obj.m_Ptr;
			}
		}
	}
}
