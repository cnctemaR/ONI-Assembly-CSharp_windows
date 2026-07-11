using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling.Memory.Experimental
{
	[NativeHeader("Modules/Profiler/Public/ProfilerConnection.h")]
	public sealed class MemoryProfiler
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event Action<string, bool> snapshotFinished;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<MetaData> createMetaData;

		[StaticAccessor("ProfilerConnection::Get()", StaticAccessorType.Dot)]
		[NativeMethod("TakeMemorySnapshot")]
		[NativeConditional("ENABLE_PLAYERCONNECTION")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TakeSnapshotInternal(string path, uint captureFlag);

		public static void TakeSnapshot(string path, Action<string, bool> finishCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
		{
			if (MemoryProfiler.snapshotFinished != null)
			{
				Debug.LogWarning("Canceling taking the snapshot. There is already ongoing capture.");
				finishCallback(path, false);
			}
			else
			{
				MemoryProfiler.snapshotFinished += finishCallback;
				MemoryProfiler.TakeSnapshotInternal(path, (uint)captureFlags);
			}
		}

		public static void TakeTempSnapshot(Action<string, bool> finishCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
		{
			string[] array = Application.dataPath.Split(new char[] { '/' });
			string text = array[array.Length - 2];
			string text2 = Application.temporaryCachePath + "/" + text + ".snap";
			MemoryProfiler.TakeSnapshot(text2, finishCallback, captureFlags);
		}

		[RequiredByNativeCode]
		private static byte[] PrepareMetadata()
		{
			byte[] array;
			if (MemoryProfiler.createMetaData == null)
			{
				array = new byte[0];
			}
			else
			{
				MetaData metaData = new MetaData();
				MemoryProfiler.createMetaData(metaData);
				if (metaData.content == null)
				{
					metaData.content = "";
				}
				if (metaData.platform == null)
				{
					metaData.platform = "";
				}
				int num = 2 * metaData.content.Length;
				int num2 = 2 * metaData.platform.Length;
				int num3 = num + num2 + 12;
				byte[] array2 = null;
				if (metaData.screenshot != null)
				{
					array2 = metaData.screenshot.GetRawTextureData();
					num3 += array2.Length + 12;
				}
				byte[] array3 = new byte[num3];
				int num4 = 0;
				num4 = MemoryProfiler.WriteIntToByteArray(array3, num4, metaData.content.Length);
				num4 = MemoryProfiler.WriteStringToByteArray(array3, num4, metaData.content);
				num4 = MemoryProfiler.WriteIntToByteArray(array3, num4, metaData.platform.Length);
				num4 = MemoryProfiler.WriteStringToByteArray(array3, num4, metaData.platform);
				if (metaData.screenshot != null)
				{
					num4 = MemoryProfiler.WriteIntToByteArray(array3, num4, array2.Length);
					Array.Copy(array2, 0, array3, num4, array2.Length);
					num4 += array2.Length;
					num4 = MemoryProfiler.WriteIntToByteArray(array3, num4, metaData.screenshot.width);
					num4 = MemoryProfiler.WriteIntToByteArray(array3, num4, metaData.screenshot.height);
					num4 = MemoryProfiler.WriteIntToByteArray(array3, num4, (int)metaData.screenshot.format);
				}
				else
				{
					num4 = MemoryProfiler.WriteIntToByteArray(array3, num4, 0);
				}
				Assert.AreEqual(array3.Length, num4);
				array = array3;
			}
			return array;
		}

		internal unsafe static int WriteIntToByteArray(byte[] array, int offset, int value)
		{
			array[offset++] = (byte)value;
			array[offset++] = *((ref value) + 1);
			array[offset++] = *((ref value) + 2);
			array[offset++] = *((ref value) + 3);
			return offset;
		}

		internal unsafe static int WriteStringToByteArray(byte[] array, int offset, string value)
		{
			if (value.Length != 0)
			{
				fixed (string text = value)
				{
					char* ptr = text + RuntimeHelpers.OffsetToStringData / 2;
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
			if (MemoryProfiler.snapshotFinished != null)
			{
				Action<string, bool> action = MemoryProfiler.snapshotFinished;
				MemoryProfiler.snapshotFinished = null;
				action(path, result);
			}
		}
	}
}
