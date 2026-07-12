using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe
{
	public struct ReadHandle : IDisposable
	{
		public bool IsValid()
		{
			return ReadHandle.IsReadHandleValid(this);
		}

		public void Dispose()
		{
			bool flag = !ReadHandle.IsReadHandleValid(this);
			if (flag)
			{
				throw new InvalidOperationException("ReadHandle.Dispose cannot be called twice on the same ReadHandle");
			}
			bool flag2 = this.Status == ReadStatus.InProgress;
			if (flag2)
			{
				throw new InvalidOperationException("ReadHandle.Dispose cannot be called until the read operation completes");
			}
			ReadHandle.ReleaseReadHandle(this);
		}

		public void Cancel()
		{
			bool flag = !ReadHandle.IsReadHandleValid(this);
			if (flag)
			{
				throw new InvalidOperationException("ReadHandle.Cancel cannot be called on a disposed ReadHandle");
			}
			ReadHandle.CancelInternal(this);
		}

		[FreeFunction("AsyncReadManagerManaged::CancelReadRequest")]
		private static void CancelInternal(ReadHandle handle)
		{
			ReadHandle.CancelInternal_Injected(ref handle);
		}

		public JobHandle JobHandle
		{
			get
			{
				bool flag = !ReadHandle.IsReadHandleValid(this);
				if (flag)
				{
					throw new InvalidOperationException("ReadHandle.JobHandle cannot be called after the ReadHandle has been disposed");
				}
				return ReadHandle.GetJobHandle(this);
			}
		}

		public ReadStatus Status
		{
			get
			{
				bool flag = !ReadHandle.IsReadHandleValid(this);
				if (flag)
				{
					throw new InvalidOperationException("Cannot use a ReadHandle that has been disposed");
				}
				return ReadHandle.GetReadStatus(this);
			}
		}

		public long ReadCount
		{
			get
			{
				bool flag = !ReadHandle.IsReadHandleValid(this);
				if (flag)
				{
					throw new InvalidOperationException("Cannot use a ReadHandle that has been disposed");
				}
				return ReadHandle.GetReadCount(this);
			}
		}

		public long GetBytesRead()
		{
			bool flag = !ReadHandle.IsReadHandleValid(this);
			if (flag)
			{
				throw new InvalidOperationException("ReadHandle.GetBytesRead cannot be called after the ReadHandle has been disposed");
			}
			return ReadHandle.GetBytesRead(this);
		}

		public long GetBytesRead(uint readCommandIndex)
		{
			bool flag = !ReadHandle.IsReadHandleValid(this);
			if (flag)
			{
				throw new InvalidOperationException("ReadHandle.GetBytesRead cannot be called after the ReadHandle has been disposed");
			}
			return ReadHandle.GetBytesReadForCommand(this, readCommandIndex);
		}

		public unsafe ulong* GetBytesReadArray()
		{
			bool flag = !ReadHandle.IsReadHandleValid(this);
			if (flag)
			{
				throw new InvalidOperationException("ReadHandle.GetBytesReadArray cannot be called after the ReadHandle has been disposed");
			}
			return ReadHandle.GetBytesReadArray(this);
		}

		[FreeFunction("AsyncReadManagerManaged::GetReadStatus", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private static ReadStatus GetReadStatus(ReadHandle handle)
		{
			return ReadHandle.GetReadStatus_Injected(ref handle);
		}

		[FreeFunction("AsyncReadManagerManaged::GetReadCount", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private static long GetReadCount(ReadHandle handle)
		{
			return ReadHandle.GetReadCount_Injected(ref handle);
		}

		[FreeFunction("AsyncReadManagerManaged::GetBytesRead", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private static long GetBytesRead(ReadHandle handle)
		{
			return ReadHandle.GetBytesRead_Injected(ref handle);
		}

		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::GetBytesReadForCommand", IsThreadSafe = true)]
		private static long GetBytesReadForCommand(ReadHandle handle, uint readCommandIndex)
		{
			return ReadHandle.GetBytesReadForCommand_Injected(ref handle, readCommandIndex);
		}

		[FreeFunction("AsyncReadManagerManaged::GetBytesReadArray", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private unsafe static ulong* GetBytesReadArray(ReadHandle handle)
		{
			return ReadHandle.GetBytesReadArray_Injected(ref handle);
		}

		[FreeFunction("AsyncReadManagerManaged::ReleaseReadHandle", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private static void ReleaseReadHandle(ReadHandle handle)
		{
			ReadHandle.ReleaseReadHandle_Injected(ref handle);
		}

		[FreeFunction("AsyncReadManagerManaged::IsReadHandleValid", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private static bool IsReadHandleValid(ReadHandle handle)
		{
			return ReadHandle.IsReadHandleValid_Injected(ref handle);
		}

		[FreeFunction("AsyncReadManagerManaged::GetJobHandle", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private static JobHandle GetJobHandle(ReadHandle handle)
		{
			JobHandle jobHandle;
			ReadHandle.GetJobHandle_Injected(ref handle, out jobHandle);
			return jobHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CancelInternal_Injected(ref ReadHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReadStatus GetReadStatus_Injected(ref ReadHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetReadCount_Injected(ref ReadHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetBytesRead_Injected(ref ReadHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetBytesReadForCommand_Injected(ref ReadHandle handle, uint readCommandIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern ulong* GetBytesReadArray_Injected(ref ReadHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseReadHandle_Injected(ref ReadHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsReadHandleValid_Injected(ref ReadHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetJobHandle_Injected(ref ReadHandle handle, out JobHandle ret);

		[NativeDisableUnsafePtrRestriction]
		internal IntPtr ptr;

		internal int version;
	}
}
