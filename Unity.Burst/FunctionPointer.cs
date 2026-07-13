using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Burst
{
	public readonly struct FunctionPointer<T> : IFunctionPointer
	{
		public FunctionPointer(IntPtr ptr)
		{
			this._ptr = ptr;
		}

		public IntPtr Value
		{
			get
			{
				return this._ptr;
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckIsCreated()
		{
			if (!this.IsCreated)
			{
				throw new NullReferenceException("Object reference not set to an instance of an object");
			}
		}

		public T Invoke
		{
			get
			{
				return Marshal.GetDelegateForFunctionPointer<T>(this._ptr);
			}
		}

		public bool IsCreated
		{
			get
			{
				return this._ptr != IntPtr.Zero;
			}
		}

		IFunctionPointer IFunctionPointer.FromIntPtr(IntPtr ptr)
		{
			return new FunctionPointer<T>(ptr);
		}

		[NativeDisableUnsafePtrRestriction]
		private readonly IntPtr _ptr;
	}
}
