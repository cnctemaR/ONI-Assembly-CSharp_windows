using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Profiling.Memory
{
	[NativeHeader("Modules/Profiler/Runtime/MemorySnapshotManager.h")]
	public static class MemoryProfiler
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event Action<string, bool> m_SnapshotFinished;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event Action<string, bool, DebugScreenCapture> m_SaveScreenshotToDisk;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<MemorySnapshotMetadata> CreatingMetadata;

		[StaticAccessor("profiling::memory::GetMemorySnapshotManager()", StaticAccessorType.Dot)]
		[NativeMethod("StartOperation")]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartOperation(uint captureFlag, bool requestScreenshot, string path, bool isRemote);

		public static void TakeSnapshot(string path, Action<string, bool> finishCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
		{
			MemoryProfiler.TakeSnapshot(path, finishCallback, null, captureFlags);
		}

		public static void TakeSnapshot(string path, Action<string, bool> finishCallback, Action<string, bool, DebugScreenCapture> screenshotCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
		{
			bool flag = MemoryProfiler.m_SnapshotFinished != null;
			if (flag)
			{
				Debug.LogWarning("Canceling snapshot, there is another snapshot in progress.");
				finishCallback(path, false);
			}
			else
			{
				MemoryProfiler.m_SnapshotFinished += finishCallback;
				MemoryProfiler.m_SaveScreenshotToDisk += screenshotCallback;
				MemoryProfiler.StartOperation((uint)captureFlags, MemoryProfiler.m_SaveScreenshotToDisk != null, path, false);
			}
		}

		public static void TakeTempSnapshot(Action<string, bool> finishCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
		{
			string[] array = Application.dataPath.Split('/', StringSplitOptions.None);
			string text = array[array.Length - 2];
			string text2 = Application.temporaryCachePath + "/" + text + ".snap";
			MemoryProfiler.TakeSnapshot(text2, finishCallback, captureFlags);
		}

		[RequiredByNativeCode]
		private unsafe static byte[] PrepareMetadata()
		{
			bool flag = MemoryProfiler.CreatingMetadata == null;
			byte[] array;
			if (flag)
			{
				array = new byte[0];
			}
			else
			{
				MemorySnapshotMetadata memorySnapshotMetadata = new MemorySnapshotMetadata();
				memorySnapshotMetadata.Description = string.Empty;
				MemoryProfiler.CreatingMetadata(memorySnapshotMetadata);
				bool flag2 = memorySnapshotMetadata.Description == null;
				if (flag2)
				{
					memorySnapshotMetadata.Description = "";
				}
				int num = 2 * memorySnapshotMetadata.Description.Length;
				int num2 = ((memorySnapshotMetadata.Data == null) ? 0 : memorySnapshotMetadata.Data.Length);
				int num3 = num + num2 + 12;
				byte[] array2 = new byte[num3];
				int num4 = 0;
				num4 = MemoryProfiler.WriteIntToByteArray(array2, num4, memorySnapshotMetadata.Description.Length);
				num4 = MemoryProfiler.WriteStringToByteArray(array2, num4, memorySnapshotMetadata.Description);
				num4 = MemoryProfiler.WriteIntToByteArray(array2, num4, num2);
				byte[] array3;
				byte* ptr;
				if ((array3 = memorySnapshotMetadata.Data) == null || array3.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array3[0];
				}
				byte[] array4;
				byte* ptr2;
				if ((array4 = array2) == null || array4.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array4[0];
				}
				byte* ptr3 = ptr2 + num4;
				UnsafeUtility.MemCpy((void*)ptr3, (void*)ptr, (long)num2);
				array3 = null;
				array4 = null;
				array = array2;
			}
			return array;
		}

		internal unsafe static int WriteIntToByteArray(byte[] array, int offset, int value)
		{
			byte* ptr = (byte*)(&value);
			array[offset++] = *ptr;
			array[offset++] = ptr[1];
			array[offset++] = ptr[2];
			array[offset++] = ptr[3];
			return offset;
		}

		internal unsafe static int WriteStringToByteArray(byte[] array, int offset, string value)
		{
			bool flag = value.Length != 0;
			if (flag)
			{
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr2 = ptr;
					char* ptr3 = ptr + value.Length;
					while (ptr2 != ptr3)
					{
						for (int i = 0; i < 2; i++)
						{
							array[offset++] = *(byte*)(ptr2 + i / 2);
						}
						ptr2++;
					}
				}
			}
			return offset;
		}

		[RequiredByNativeCode]
		private static void FinalizeSnapshot(string path, bool result)
		{
			bool flag = MemoryProfiler.m_SnapshotFinished != null;
			if (flag)
			{
				Action<string, bool> snapshotFinished = MemoryProfiler.m_SnapshotFinished;
				MemoryProfiler.m_SnapshotFinished = null;
				snapshotFinished(path, result);
			}
		}

		[RequiredByNativeCode]
		private static void SaveScreenshotToDisk(string path, bool result, IntPtr pixelsPtr, int pixelsCount, TextureFormat format, int width, int height)
		{
			bool flag = MemoryProfiler.m_SaveScreenshotToDisk != null;
			if (flag)
			{
				Action<string, bool, DebugScreenCapture> saveScreenshotToDisk = MemoryProfiler.m_SaveScreenshotToDisk;
				MemoryProfiler.m_SaveScreenshotToDisk = null;
				DebugScreenCapture debugScreenCapture = default(DebugScreenCapture);
				if (result)
				{
					NativeArray<byte> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(pixelsPtr.ToPointer(), pixelsCount, Allocator.Persistent);
					debugScreenCapture.RawImageDataReference = nativeArray;
					debugScreenCapture.Height = height;
					debugScreenCapture.Width = width;
					debugScreenCapture.ImageFormat = format;
				}
				saveScreenshotToDisk(path, result, debugScreenCapture);
			}
		}
	}
}
