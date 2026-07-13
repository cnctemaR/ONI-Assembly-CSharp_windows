using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsWaitingForSceneActivation();

		[NativeMethod("WaitForCompletion")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WaitForCompletion();

		[NativeMethod("Cancel")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Cancel();

		[StaticAccessor("GetAsyncInstantiateManager()", StaticAccessorType.Dot)]
		internal static extern float IntegrationTimeMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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

		internal Object[] m_Result;
	}
}
