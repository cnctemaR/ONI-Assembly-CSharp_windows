using System;
using System.Runtime.InteropServices;

namespace System.Threading
{
	internal struct Win32ThreadPoolNativeOverlapped
	{
		static Win32ThreadPoolNativeOverlapped()
		{
			if (!Environment.IsRunningOnWindows)
			{
				throw new PlatformNotSupportedException();
			}
		}

		internal Win32ThreadPoolNativeOverlapped.OverlappedData Data
		{
			get
			{
				return Win32ThreadPoolNativeOverlapped.s_dataArray[this._dataIndex];
			}
		}

		internal unsafe static Win32ThreadPoolNativeOverlapped* Allocate(IOCompletionCallback callback, object state, object pinData, PreAllocatedOverlapped preAllocated)
		{
			Win32ThreadPoolNativeOverlapped* ptr = Win32ThreadPoolNativeOverlapped.AllocateNew();
			try
			{
				ptr->SetData(callback, state, pinData, preAllocated);
			}
			catch
			{
				Win32ThreadPoolNativeOverlapped.Free(ptr);
				throw;
			}
			return ptr;
		}

		private unsafe static Win32ThreadPoolNativeOverlapped* AllocateNew()
		{
			IntPtr intPtr;
			Win32ThreadPoolNativeOverlapped* ptr;
			while ((intPtr = Volatile.Read(ref Win32ThreadPoolNativeOverlapped.s_freeList)) != IntPtr.Zero)
			{
				ptr = (Win32ThreadPoolNativeOverlapped*)(void*)intPtr;
				if (!(Interlocked.CompareExchange(ref Win32ThreadPoolNativeOverlapped.s_freeList, ptr->_nextFree, intPtr) != intPtr))
				{
					ptr->_nextFree = IntPtr.Zero;
					return ptr;
				}
			}
			ptr = (Win32ThreadPoolNativeOverlapped*)(void*)Interop.MemAlloc((UIntPtr)((ulong)((long)sizeof(Win32ThreadPoolNativeOverlapped))));
			*ptr = default(Win32ThreadPoolNativeOverlapped);
			Win32ThreadPoolNativeOverlapped.OverlappedData overlappedData = new Win32ThreadPoolNativeOverlapped.OverlappedData();
			int num = Interlocked.Increment(ref Win32ThreadPoolNativeOverlapped.s_dataCount) - 1;
			if (num < 0)
			{
				Environment.FailFast("Too many outstanding Win32ThreadPoolNativeOverlapped instances");
			}
			for (;;)
			{
				Win32ThreadPoolNativeOverlapped.OverlappedData[] array = Volatile.Read<Win32ThreadPoolNativeOverlapped.OverlappedData[]>(ref Win32ThreadPoolNativeOverlapped.s_dataArray);
				int num2 = ((array == null) ? 0 : array.Length);
				if (num2 <= num)
				{
					int i = num2;
					if (i == 0)
					{
						i = 128;
					}
					while (i <= num)
					{
						i = i * 3 / 2;
					}
					Win32ThreadPoolNativeOverlapped.OverlappedData[] array2 = array;
					Array.Resize<Win32ThreadPoolNativeOverlapped.OverlappedData>(ref array2, i);
					if (Interlocked.CompareExchange<Win32ThreadPoolNativeOverlapped.OverlappedData[]>(ref Win32ThreadPoolNativeOverlapped.s_dataArray, array2, array) != array)
					{
						continue;
					}
					array = array2;
				}
				if (Win32ThreadPoolNativeOverlapped.s_dataArray[num] != null)
				{
					break;
				}
				Interlocked.Exchange<Win32ThreadPoolNativeOverlapped.OverlappedData>(ref array[num], overlappedData);
			}
			ptr->_dataIndex = num;
			return ptr;
		}

		private void SetData(IOCompletionCallback callback, object state, object pinData, PreAllocatedOverlapped preAllocated)
		{
			Win32ThreadPoolNativeOverlapped.OverlappedData data = this.Data;
			data._callback = callback;
			data._state = state;
			data._executionContext = ExecutionContext.Capture();
			data._preAllocated = preAllocated;
			if (pinData != null)
			{
				object[] array = pinData as object[];
				if (array != null && array.GetType() == typeof(object[]))
				{
					if (data._pinnedData == null || data._pinnedData.Length < array.Length)
					{
						Array.Resize<GCHandle>(ref data._pinnedData, array.Length);
					}
					for (int i = 0; i < array.Length; i++)
					{
						if (!data._pinnedData[i].IsAllocated)
						{
							data._pinnedData[i] = GCHandle.Alloc(array[i], GCHandleType.Pinned);
						}
						else
						{
							data._pinnedData[i].Target = array[i];
						}
					}
					return;
				}
				if (data._pinnedData == null)
				{
					data._pinnedData = new GCHandle[1];
				}
				if (!data._pinnedData[0].IsAllocated)
				{
					data._pinnedData[0] = GCHandle.Alloc(pinData, GCHandleType.Pinned);
					return;
				}
				data._pinnedData[0].Target = pinData;
			}
		}

		internal unsafe static void Free(Win32ThreadPoolNativeOverlapped* overlapped)
		{
			overlapped->Data.Reset();
			overlapped->_overlapped = default(NativeOverlapped);
			IntPtr intPtr;
			do
			{
				intPtr = Volatile.Read(ref Win32ThreadPoolNativeOverlapped.s_freeList);
				overlapped->_nextFree = intPtr;
			}
			while (!(Interlocked.CompareExchange(ref Win32ThreadPoolNativeOverlapped.s_freeList, (IntPtr)((void*)overlapped), intPtr) == intPtr));
		}

		internal unsafe static NativeOverlapped* ToNativeOverlapped(Win32ThreadPoolNativeOverlapped* overlapped)
		{
			return (NativeOverlapped*)overlapped;
		}

		internal unsafe static Win32ThreadPoolNativeOverlapped* FromNativeOverlapped(NativeOverlapped* overlapped)
		{
			return (Win32ThreadPoolNativeOverlapped*)overlapped;
		}

		internal unsafe static void CompleteWithCallback(uint errorCode, uint bytesWritten, Win32ThreadPoolNativeOverlapped* overlapped)
		{
			Win32ThreadPoolNativeOverlapped.OverlappedData data = overlapped->Data;
			data._completed = true;
			if (data._executionContext == null)
			{
				data._callback(errorCode, bytesWritten, Win32ThreadPoolNativeOverlapped.ToNativeOverlapped(overlapped));
				return;
			}
			ContextCallback contextCallback = Win32ThreadPoolNativeOverlapped.s_executionContextCallback;
			if (contextCallback == null)
			{
				contextCallback = (Win32ThreadPoolNativeOverlapped.s_executionContextCallback = new ContextCallback(Win32ThreadPoolNativeOverlapped.OnExecutionContextCallback));
			}
			Win32ThreadPoolNativeOverlapped.ExecutionContextCallbackArgs executionContextCallbackArgs = Win32ThreadPoolNativeOverlapped.t_executionContextCallbackArgs;
			if (executionContextCallbackArgs == null)
			{
				executionContextCallbackArgs = new Win32ThreadPoolNativeOverlapped.ExecutionContextCallbackArgs();
			}
			Win32ThreadPoolNativeOverlapped.t_executionContextCallbackArgs = null;
			executionContextCallbackArgs._errorCode = errorCode;
			executionContextCallbackArgs._bytesWritten = bytesWritten;
			executionContextCallbackArgs._overlapped = overlapped;
			executionContextCallbackArgs._data = data;
			ExecutionContext.Run(data._executionContext, contextCallback, executionContextCallbackArgs);
		}

		private unsafe static void OnExecutionContextCallback(object state)
		{
			Win32ThreadPoolNativeOverlapped.ExecutionContextCallbackArgs executionContextCallbackArgs = (Win32ThreadPoolNativeOverlapped.ExecutionContextCallbackArgs)state;
			uint errorCode = executionContextCallbackArgs._errorCode;
			uint bytesWritten = executionContextCallbackArgs._bytesWritten;
			Win32ThreadPoolNativeOverlapped* overlapped = executionContextCallbackArgs._overlapped;
			Win32ThreadPoolNativeOverlapped.OverlappedData data = executionContextCallbackArgs._data;
			executionContextCallbackArgs._data = null;
			Win32ThreadPoolNativeOverlapped.t_executionContextCallbackArgs = executionContextCallbackArgs;
			data._callback(errorCode, bytesWritten, Win32ThreadPoolNativeOverlapped.ToNativeOverlapped(overlapped));
		}

		[ThreadStatic]
		private static Win32ThreadPoolNativeOverlapped.ExecutionContextCallbackArgs t_executionContextCallbackArgs;

		private static ContextCallback s_executionContextCallback;

		private static Win32ThreadPoolNativeOverlapped.OverlappedData[] s_dataArray;

		private static int s_dataCount;

		private static IntPtr s_freeList;

		private NativeOverlapped _overlapped;

		private IntPtr _nextFree;

		private int _dataIndex;

		private class ExecutionContextCallbackArgs
		{
			internal uint _errorCode;

			internal uint _bytesWritten;

			internal unsafe Win32ThreadPoolNativeOverlapped* _overlapped;

			internal Win32ThreadPoolNativeOverlapped.OverlappedData _data;
		}

		internal class OverlappedData
		{
			internal void Reset()
			{
				if (this._pinnedData != null)
				{
					for (int i = 0; i < this._pinnedData.Length; i++)
					{
						if (this._pinnedData[i].IsAllocated && this._pinnedData[i].Target != null)
						{
							this._pinnedData[i].Target = null;
						}
					}
				}
				this._callback = null;
				this._state = null;
				this._executionContext = null;
				this._completed = false;
				this._preAllocated = null;
			}

			internal GCHandle[] _pinnedData;

			internal IOCompletionCallback _callback;

			internal object _state;

			internal ExecutionContext _executionContext;

			internal ThreadPoolBoundHandle _boundHandle;

			internal PreAllocatedOverlapped _preAllocated;

			internal bool _completed;
		}
	}
}
