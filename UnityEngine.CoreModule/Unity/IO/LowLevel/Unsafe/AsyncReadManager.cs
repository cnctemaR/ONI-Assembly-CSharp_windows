using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
			ReadHandle readHandle2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filename.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = assetName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ReadHandle readHandle;
				AsyncReadManager.ReadInternal_Injected(ref managedSpanWrapper, cmds, cmdCount, ref managedSpanWrapper2, typeID, subsystem, out readHandle);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				ReadHandle readHandle;
				readHandle2 = readHandle;
			}
			return readHandle2;
		}

		public unsafe static ReadHandle Read(string filename, ReadCommand* readCmds, uint readCmdCount, string assetName = "", ulong typeID = 0UL, AssetLoadingSubsystem subsystem = AssetLoadingSubsystem.Scripts)
		{
			return AsyncReadManager.ReadInternal(filename, (void*)readCmds, readCmdCount, assetName, typeID, subsystem);
		}

		[FreeFunction("AsyncReadManagerManaged::GetFileInfo", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private unsafe static ReadHandle GetFileInfoInternal(string filename, void* cmd)
		{
			ReadHandle readHandle2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filename.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ReadHandle readHandle;
				AsyncReadManager.GetFileInfoInternal_Injected(ref managedSpanWrapper, cmd, out readHandle);
			}
			finally
			{
				char* ptr = null;
				ReadHandle readHandle;
				readHandle2 = readHandle;
			}
			return readHandle2;
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

		[FreeFunction("AsyncReadManagerManaged::ReadWithHandles_NativePtr", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private unsafe static ReadHandle ReadWithHandlesInternal_NativePtr(in FileHandle fileHandle, void* readCmdArray, JobHandle dependency)
		{
			ReadHandle readHandle;
			AsyncReadManager.ReadWithHandlesInternal_NativePtr_Injected(in fileHandle, readCmdArray, ref dependency, out readHandle);
			return readHandle;
		}

		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::ReadWithHandles_NativeCopy", IsThreadSafe = true)]
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

		[FreeFunction("AsyncReadManagerManaged::ScheduleOpenRequest", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private unsafe static FileHandle OpenFileAsync_Internal(string fileName)
		{
			FileHandle fileHandle2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fileName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				FileHandle fileHandle;
				AsyncReadManager.OpenFileAsync_Internal_Injected(ref managedSpanWrapper, out fileHandle);
			}
			finally
			{
				char* ptr = null;
				FileHandle fileHandle;
				fileHandle2 = fileHandle;
			}
			return fileHandle2;
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
		public unsafe static JobHandle CloseCachedFileAsync(string fileName, JobHandle dependency = default(JobHandle))
		{
			JobHandle jobHandle2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fileName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				JobHandle jobHandle;
				AsyncReadManager.CloseCachedFileAsync_Injected(ref managedSpanWrapper, ref dependency, out jobHandle);
			}
			finally
			{
				char* ptr = null;
				JobHandle jobHandle;
				jobHandle2 = jobHandle;
			}
			return jobHandle2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadInternal_Injected(ref ManagedSpanWrapper filename, void* cmds, uint cmdCount, ref ManagedSpanWrapper assetName, ulong typeID, AssetLoadingSubsystem subsystem, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetFileInfoInternal_Injected(ref ManagedSpanWrapper filename, void* cmd, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadWithHandlesInternal_NativePtr_Injected(in FileHandle fileHandle, void* readCmdArray, [In] ref JobHandle dependency, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadWithHandlesInternal_NativeCopy_Injected(in FileHandle fileHandle, void* readCmdArray, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OpenFileAsync_Internal_Injected(ref ManagedSpanWrapper fileName, out FileHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CloseFileAsync_Injected(in FileHandle fileHandle, [In] ref JobHandle dependency, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CloseCachedFileAsync_Injected(ref ManagedSpanWrapper fileName, [In] ref JobHandle dependency, out JobHandle ret);
	}
}
