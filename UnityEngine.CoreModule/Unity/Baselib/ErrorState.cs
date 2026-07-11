using System;
using System.Runtime.InteropServices;
using Unity.Baselib.LowLevel;

namespace Unity.Baselib
{
	internal struct ErrorState
	{
		public void ThrowIfFailed()
		{
			bool flag = this.ErrorCode > Binding.Baselib_ErrorCode.Success;
			if (flag)
			{
				throw new BaselibException(this);
			}
		}

		public Binding.Baselib_ErrorCode ErrorCode
		{
			get
			{
				return this.nativeErrorState.code;
			}
		}

		public unsafe Binding.Baselib_ErrorState* NativeErrorStatePtr
		{
			get
			{
				fixed (Binding.Baselib_ErrorState* ptr = &this.nativeErrorState)
				{
					return ptr;
				}
			}
		}

		public unsafe string Explain(Binding.Baselib_ErrorState_ExplainVerbosity verbosity = Binding.Baselib_ErrorState_ExplainVerbosity.ErrorType_SourceLocation_Explanation)
		{
			fixed (Binding.Baselib_ErrorState* ptr = &this.nativeErrorState)
			{
				Binding.Baselib_ErrorState* ptr2 = ptr;
				uint num = Binding.Baselib_ErrorState_Explain(ptr2, null, 0U, verbosity) + 1U;
				IntPtr intPtr = Binding.Baselib_Memory_Allocate(new UIntPtr(num));
				string text;
				try
				{
					Binding.Baselib_ErrorState_Explain(ptr2, (byte*)(void*)intPtr, num, verbosity);
					text = Marshal.PtrToStringAnsi(intPtr);
				}
				finally
				{
					Binding.Baselib_Memory_Free(intPtr);
				}
				return text;
			}
		}

		private Binding.Baselib_ErrorState nativeErrorState;
	}
}
