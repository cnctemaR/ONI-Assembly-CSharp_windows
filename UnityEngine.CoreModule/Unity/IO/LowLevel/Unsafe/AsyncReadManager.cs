using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe
{
	[NativeHeader("Runtime/File/AsyncReadManagerManagedApi.h")]
	public static class AsyncReadManager
	{
		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::Read", IsThreadSafe = true)]
		private unsafe static ReadHandle ReadInternal(string filename, void* cmds, uint cmdCount, string assetName, ulong typeID, AssetLoadingSubsystem subsystem)
		{
			ReadHandle readHandle;
			AsyncReadManager.ReadInternal_Injected(filename, cmds, cmdCount, assetName, typeID, subsystem, out readHandle);
			return readHandle;
		}

		public unsafe static ReadHandle Read(string filename, ReadCommand* readCmds, uint readCmdCount, string assetName = "", ulong typeID = 0UL, AssetLoadingSubsystem subsystem = AssetLoadingSubsystem.Scripts)
		{
			return AsyncReadManager.ReadInternal(filename, (void*)readCmds, readCmdCount, assetName, typeID, subsystem);
		}

		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::GetFileInfo", IsThreadSafe = true)]
		private unsafe static ReadHandle GetFileInfoInternal(string filename, void* cmd)
		{
			ReadHandle readHandle;
			AsyncReadManager.GetFileInfoInternal_Injected(filename, cmd, out readHandle);
			return readHandle;
		}

		public unsafe static ReadHandle GetFileInfo(string filename, FileInfoResult* result)
		{
			bool flag = result == null;
			if (flag)
			{
				throw new NullReferenceException("GetFileInfo must have a valid FileInfoResult to write into.");
			}
			return AsyncReadManager.GetFileInfoInternal(filename, (void*)result);
		}

		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::ReadWithHandles_NativePtr", IsThreadSafe = true)]
		private unsafe static ReadHandle ReadWithHandlesInternal_NativePtr(in FileHandle fileHandle, void* readCmdArray, JobHandle dependency)
		{
			ReadHandle readHandle;
			AsyncReadManager.ReadWithHandlesInternal_NativePtr_Injected(in fileHandle, readCmdArray, ref dependency, out readHandle);
			return readHandle;
		}

		[FreeFunction("AsyncReadManagerManaged::ReadWithHandles_NativeCopy", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private unsafe static ReadHandle ReadWithHandlesInternal_NativeCopy(in FileHandle fileHandle, void* readCmdArray)
		{
			ReadHandle readHandle;
			AsyncReadManager.ReadWithHandlesInternal_NativeCopy_Injected(in fileHandle, readCmdArray, out readHandle);
			return readHandle;
		}

		public unsafe static ReadHandle ReadDeferred(in FileHandle fileHandle, ReadCommandArray* readCmdArray, JobHandle dependency)
		{
			bool flag = !fileHandle.IsValid();
			if (flag)
			{
				throw new InvalidOperationException("FileHandle is invalid and may not be read from.");
			}
			return AsyncReadManager.ReadWithHandlesInternal_NativePtr(in fileHandle, (void*)readCmdArray, dependency);
		}

		public static ReadHandle Read(in FileHandle fileHandle, ReadCommandArray readCmdArray)
		{
			bool flag = !fileHandle.IsValid();
			if (flag)
			{
				throw new InvalidOperationException("FileHandle is invalid and may not be read from.");
			}
			return AsyncReadManager.ReadWithHandlesInternal_NativeCopy(in fileHandle, UnsafeUtility.AddressOf<ReadCommandArray>(ref readCmdArray));
		}

		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::ScheduleOpenRequest", IsThreadSafe = true)]
		private static FileHandle OpenFileAsync_Internal(string fileName)
		{
			FileHandle fileHandle;
			AsyncReadManager.OpenFileAsync_Internal_Injected(fileName, out fileHandle);
			return fileHandle;
		}

		public static FileHandle OpenFileAsync(string fileName)
		{
			bool flag = fileName.Length == 0;
			if (flag)
			{
				throw new InvalidOperationException("FileName is empty");
			}
			return AsyncReadManager.OpenFileAsync_Internal(fileName);
		}

		[FreeFunction("AsyncReadManagerManaged::ScheduleCloseRequest", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		internal static JobHandle CloseFileAsync(in FileHandle fileHandle, JobHandle dependency)
		{
			JobHandle jobHandle;
			AsyncReadManager.CloseFileAsync_Injected(in fileHandle, ref dependency, out jobHandle);
			return jobHandle;
		}

		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::ScheduleCloseCachedFileRequest", IsThreadSafe = true)]
		public static JobHandle CloseCachedFileAsync(string fileName, JobHandle dependency = default(JobHandle))
		{
			JobHandle jobHandle;
			AsyncReadManager.CloseCachedFileAsync_Injected(fileName, ref dependency, out jobHandle);
			return jobHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadInternal_Injected(string filename, void* cmds, uint cmdCount, string assetName, ulong typeID, AssetLoadingSubsystem subsystem, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetFileInfoInternal_Injected(string filename, void* cmd, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadWithHandlesInternal_NativePtr_Injected(in FileHandle fileHandle, void* readCmdArray, ref JobHandle dependency, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadWithHandlesInternal_NativeCopy_Injected(in FileHandle fileHandle, void* readCmdArray, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OpenFileAsync_Internal_Injected(string fileName, out FileHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CloseFileAsync_Injected(in FileHandle fileHandle, ref JobHandle dependency, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CloseCachedFileAsync_Injected(string fileName, ref JobHandle dependency = null, out JobHandle ret);
	}
}
