using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe
{
	public readonly struct FileHandle
	{
		public FileStatus Status
		{
			get
			{
				bool flag = !FileHandle.IsFileHandleValid(in this);
				if (flag)
				{
					throw new InvalidOperationException("FileHandle.Status cannot be called on a closed FileHandle");
				}
				return FileHandle.GetFileStatus_Internal(in this);
			}
		}

		public JobHandle JobHandle
		{
			get
			{
				bool flag = !FileHandle.IsFileHandleValid(in this);
				if (flag)
				{
					throw new InvalidOperationException("FileHandle.JobHandle cannot be called on a closed FileHandle");
				}
				return FileHandle.GetJobHandle_Internal(in this);
			}
		}

		public bool IsValid()
		{
			return FileHandle.IsFileHandleValid(in this);
		}

		public JobHandle Close(JobHandle dependency = default(JobHandle))
		{
			bool flag = !FileHandle.IsFileHandleValid(in this);
			if (flag)
			{
				throw new InvalidOperationException("FileHandle.Close cannot be called twice on the same FileHandle");
			}
			return AsyncReadManager.CloseFileAsync(in this, dependency);
		}

		[FreeFunction("AsyncReadManagerManaged::IsFileHandleValid")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsFileHandleValid(in FileHandle handle);

		[FreeFunction("AsyncReadManagerManaged::GetFileStatusFromManagedHandle")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern FileStatus GetFileStatus_Internal(in FileHandle handle);

		[FreeFunction("AsyncReadManagerManaged::GetJobFenceFromManagedHandle")]
		private static JobHandle GetJobHandle_Internal(in FileHandle handle)
		{
			JobHandle jobHandle;
			FileHandle.GetJobHandle_Internal_Injected(in handle, out jobHandle);
			return jobHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetJobHandle_Internal_Injected(in FileHandle handle, out JobHandle ret);

		[NativeDisableUnsafePtrRestriction]
		internal readonly IntPtr fileCommandPtr;

		internal readonly int version;
	}
}
