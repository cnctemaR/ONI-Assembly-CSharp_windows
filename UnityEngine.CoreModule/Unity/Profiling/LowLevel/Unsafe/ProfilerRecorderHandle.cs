using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Profiling.LowLevel.Unsafe
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public readonly struct ProfilerRecorderHandle
	{
		internal ProfilerRecorderHandle(ulong handle)
		{
			this.handle = handle;
		}

		public bool Valid
		{
			get
			{
				return this.handle != 0UL && this.handle != ulong.MaxValue;
			}
		}

		internal static ProfilerRecorderHandle Get(ProfilerMarker marker)
		{
			return new ProfilerRecorderHandle((ulong)marker.Handle.ToInt64());
		}

		internal static ProfilerRecorderHandle Get(ProfilerCategory category, string statName)
		{
			bool flag = string.IsNullOrEmpty(statName);
			if (flag)
			{
				throw new ArgumentException("String must be not null or empty", "statName");
			}
			return ProfilerRecorderHandle.GetByName(category, statName);
		}

		public static ProfilerRecorderDescription GetDescription(ProfilerRecorderHandle handle)
		{
			bool flag = !handle.Valid;
			if (flag)
			{
				throw new ArgumentException("ProfilerRecorderHandle is not initialized or is not available", "handle");
			}
			return ProfilerRecorderHandle.GetDescriptionInternal(handle);
		}

		[NativeMethod(IsThreadSafe = true)]
		public unsafe static void GetAvailable([NotNull] List<ProfilerRecorderHandle> outRecorderHandleList)
		{
			if (outRecorderHandleList == null)
			{
				ThrowHelper.ThrowArgumentNullException(outRecorderHandleList, "outRecorderHandleList");
			}
			try
			{
				fixed (ProfilerRecorderHandle[] array = NoAllocHelpers.ExtractArrayFromList<ProfilerRecorderHandle>(outRecorderHandleList))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, outRecorderHandleList.Count);
					ProfilerRecorderHandle.GetAvailable_Injected(ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ProfilerRecorderHandle>(outRecorderHandleList);
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		internal unsafe static ProfilerRecorderHandle GetByName(ProfilerCategory category, string name)
		{
			ProfilerRecorderHandle profilerRecorderHandle2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ProfilerRecorderHandle profilerRecorderHandle;
				ProfilerRecorderHandle.GetByName_Injected(ref category, ref managedSpanWrapper, out profilerRecorderHandle);
			}
			finally
			{
				char* ptr = null;
				ProfilerRecorderHandle profilerRecorderHandle;
				profilerRecorderHandle2 = profilerRecorderHandle;
			}
			return profilerRecorderHandle2;
		}

		[RequiredMember]
		[NativeMethod(IsThreadSafe = true)]
		internal unsafe static ProfilerRecorderHandle GetByName__Unmanaged(ProfilerCategory category, byte* name, int nameLen)
		{
			ProfilerRecorderHandle profilerRecorderHandle;
			ProfilerRecorderHandle.GetByName__Unmanaged_Injected(ref category, name, nameLen, out profilerRecorderHandle);
			return profilerRecorderHandle;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static ProfilerRecorderHandle GetByName(ProfilerCategory category, char* name, int nameLen)
		{
			return ProfilerRecorderHandle.GetByName_Unsafe(category, name, nameLen);
		}

		[NativeMethod(IsThreadSafe = true)]
		private unsafe static ProfilerRecorderHandle GetByName_Unsafe(ProfilerCategory category, char* name, int nameLen)
		{
			ProfilerRecorderHandle profilerRecorderHandle;
			ProfilerRecorderHandle.GetByName_Unsafe_Injected(ref category, name, nameLen, out profilerRecorderHandle);
			return profilerRecorderHandle;
		}

		[NativeMethod(IsThreadSafe = true)]
		private static ProfilerRecorderDescription GetDescriptionInternal(ProfilerRecorderHandle handle)
		{
			ProfilerRecorderDescription profilerRecorderDescription;
			ProfilerRecorderHandle.GetDescriptionInternal_Injected(ref handle, out profilerRecorderDescription);
			return profilerRecorderDescription;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAvailable_Injected(ref BlittableListWrapper outRecorderHandleList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetByName_Injected([In] ref ProfilerCategory category, ref ManagedSpanWrapper name, out ProfilerRecorderHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetByName__Unmanaged_Injected([In] ref ProfilerCategory category, byte* name, int nameLen, out ProfilerRecorderHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetByName_Unsafe_Injected([In] ref ProfilerCategory category, char* name, int nameLen, out ProfilerRecorderHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDescriptionInternal_Injected([In] ref ProfilerRecorderHandle handle, out ProfilerRecorderDescription ret);

		private const ulong k_InvalidHandle = 18446744073709551615UL;

		[FieldOffset(0)]
		internal readonly ulong handle;
	}
}
