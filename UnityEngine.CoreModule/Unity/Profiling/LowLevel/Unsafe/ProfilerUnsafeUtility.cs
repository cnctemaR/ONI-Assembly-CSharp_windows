using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Profiling.LowLevel.Unsafe
{
	[NativeHeader("Runtime/Profiler/ScriptBindings/ProfilerUnsafeUtility.bindings.h")]
	[UsedByNativeCode]
	[IgnoredByDeepProfiler]
	public static class ProfilerUnsafeUtility
	{
		[ThreadSafe]
		internal unsafe static ushort CreateCategory(string name, ProfilerCategoryColor colorIndex)
		{
			ushort num;
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
				num = ProfilerUnsafeUtility.CreateCategory_Injected(ref managedSpanWrapper, colorIndex);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		[ThreadSafe]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern ushort CreateCategory__Unmanaged(byte* name, int nameLen, ProfilerCategoryColor colorIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ushort CreateCategory(char* name, int nameLen, ProfilerCategoryColor colorIndex)
		{
			return ProfilerUnsafeUtility.CreateCategory_Unsafe(name, nameLen, colorIndex);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern ushort CreateCategory_Unsafe(char* name, int nameLen, ProfilerCategoryColor colorIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ushort GetCategoryByName(char* name, int nameLen)
		{
			return ProfilerUnsafeUtility.GetCategoryByName_Unsafe(name, nameLen);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern ushort GetCategoryByName_Unsafe(char* name, int nameLen);

		[ThreadSafe]
		public static ProfilerCategoryDescription GetCategoryDescription(ushort categoryId)
		{
			ProfilerCategoryDescription profilerCategoryDescription;
			ProfilerUnsafeUtility.GetCategoryDescription_Injected(categoryId, out profilerCategoryDescription);
			return profilerCategoryDescription;
		}

		[ThreadSafe]
		internal static Color32 GetCategoryColor(ProfilerCategoryColor colorIndex)
		{
			Color32 color;
			ProfilerUnsafeUtility.GetCategoryColor_Injected(colorIndex, out color);
			return color;
		}

		[ThreadSafe]
		public unsafe static IntPtr CreateMarker(string name, ushort categoryId, MarkerFlags flags, int metadataCount)
		{
			IntPtr intPtr;
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
				intPtr = ProfilerUnsafeUtility.CreateMarker_Injected(ref managedSpanWrapper, categoryId, flags, metadataCount);
			}
			finally
			{
				char* ptr = null;
			}
			return intPtr;
		}

		[ThreadSafe]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern IntPtr CreateMarker__Unmanaged(byte* name, int nameLen, ushort categoryId, MarkerFlags flags, int metadataCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static IntPtr CreateMarker(char* name, int nameLen, ushort categoryId, MarkerFlags flags, int metadataCount)
		{
			return ProfilerUnsafeUtility.CreateMarker_Unsafe(name, nameLen, categoryId, flags, metadataCount);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr CreateMarker_Unsafe(char* name, int nameLen, ushort categoryId, MarkerFlags flags, int metadataCount);

		[ThreadSafe]
		internal unsafe static IntPtr GetMarker(string name)
		{
			IntPtr marker_Injected;
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
				marker_Injected = ProfilerUnsafeUtility.GetMarker_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return marker_Injected;
		}

		[ThreadSafe]
		public unsafe static void SetMarkerMetadata(IntPtr markerPtr, int index, string name, byte type, byte unit)
		{
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
				ProfilerUnsafeUtility.SetMarkerMetadata_Injected(markerPtr, index, ref managedSpanWrapper, type, unit);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[RequiredMember]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void SetMarkerMetadata__Unmanaged(IntPtr markerPtr, int index, byte* name, int nameLen, byte type, byte unit);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void SetMarkerMetadata(IntPtr markerPtr, int index, char* name, int nameLen, byte type, byte unit)
		{
			ProfilerUnsafeUtility.SetMarkerMetadata_Unsafe(markerPtr, index, name, nameLen, type, unit);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetMarkerMetadata_Unsafe(IntPtr markerPtr, int index, char* name, int nameLen, byte type, byte unit);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginSample(IntPtr markerPtr);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void BeginSampleWithMetadata(IntPtr markerPtr, int metadataCount, void* metadata);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndSample(IntPtr markerPtr);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void SingleSampleWithMetadata(IntPtr markerPtr, int metadataCount, void* metadata);

		[ThreadSafe]
		public unsafe static void* CreateCounterValue(out IntPtr counterPtr, string name, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions)
		{
			void* ptr2;
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
				ptr2 = ProfilerUnsafeUtility.CreateCounterValue_Injected(out counterPtr, ref managedSpanWrapper, categoryId, flags, dataType, dataUnit, dataSize, counterOptions);
			}
			finally
			{
				char* ptr = null;
			}
			return ptr2;
		}

		[RequiredMember]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void* CreateCounterValue__Unmanaged(out IntPtr counterPtr, byte* name, int nameLen, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* CreateCounterValue(out IntPtr counterPtr, char* name, int nameLen, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions)
		{
			return ProfilerUnsafeUtility.CreateCounterValue_Unsafe(out counterPtr, name, nameLen, categoryId, flags, dataType, dataUnit, dataSize, counterOptions);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* CreateCounterValue_Unsafe(out IntPtr counterPtr, char* name, int nameLen, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void FlushCounterValue(void* counterValuePtr);

		internal unsafe static string Utf8ToString(byte* chars, int charsLen)
		{
			bool flag = chars == null;
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				byte[] array = new byte[charsLen];
				Marshal.Copy((IntPtr)((void*)chars), array, 0, charsLen);
				text = Encoding.UTF8.GetString(array, 0, charsLen);
			}
			return text;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint CreateFlow(ushort categoryId);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FlowEvent(uint flowId, ProfilerFlowEventType flowEventType);

		[ThreadSafe]
		internal static void Internal_BeginWithObject(IntPtr markerPtr, Object contextUnityObject)
		{
			ProfilerUnsafeUtility.Internal_BeginWithObject_Injected(markerPtr, Object.MarshalledUnityObject.Marshal<Object>(contextUnityObject));
		}

		[NativeConditional("ENABLE_PROFILER")]
		internal static string Internal_GetName(IntPtr markerPtr)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				ProfilerUnsafeUtility.Internal_GetName_Injected(markerPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static extern long Timestamp
		{
			[ThreadSafe]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static ProfilerUnsafeUtility.TimestampConversionRatio TimestampToNanosecondsConversionRatio
		{
			[ThreadSafe]
			get
			{
				ProfilerUnsafeUtility.TimestampConversionRatio timestampConversionRatio;
				ProfilerUnsafeUtility.get_TimestampToNanosecondsConversionRatio_Injected(out timestampConversionRatio);
				return timestampConversionRatio;
			}
		}

		[ThreadSafe(ThrowsException = false)]
		[NativeConditional("ENABLE_MEM_PROFILER")]
		internal unsafe static IntPtr GetOrCreateMemLabel(string areaName, string objectName)
		{
			IntPtr orCreateMemLabel_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(areaName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = areaName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(objectName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = objectName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				orCreateMemLabel_Injected = ProfilerUnsafeUtility.GetOrCreateMemLabel_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return orCreateMemLabel_Injected;
		}

		[NativeConditional("ENABLE_MEM_PROFILER")]
		[RequiredMember]
		[ThreadSafe(ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern IntPtr GetOrCreateMemLabel__Unmanaged(byte* areaName, int areaNameLen, byte* objectName, int objectNameLen);

		[ThreadSafe(ThrowsException = true)]
		[NativeConditional("ENABLE_MEM_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetMemLabelRelatedMemorySize(IntPtr label);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort CreateCategory_Injected(ref ManagedSpanWrapper name, ProfilerCategoryColor colorIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCategoryDescription_Injected(ushort categoryId, out ProfilerCategoryDescription ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCategoryColor_Injected(ProfilerCategoryColor colorIndex, out Color32 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateMarker_Injected(ref ManagedSpanWrapper name, ushort categoryId, MarkerFlags flags, int metadataCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetMarker_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMarkerMetadata_Injected(IntPtr markerPtr, int index, ref ManagedSpanWrapper name, byte type, byte unit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* CreateCounterValue_Injected(out IntPtr counterPtr, ref ManagedSpanWrapper name, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BeginWithObject_Injected(IntPtr markerPtr, IntPtr contextUnityObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetName_Injected(IntPtr markerPtr, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_TimestampToNanosecondsConversionRatio_Injected(out ProfilerUnsafeUtility.TimestampConversionRatio ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetOrCreateMemLabel_Injected(ref ManagedSpanWrapper areaName, ref ManagedSpanWrapper objectName);

		public const ushort CategoryRender = 0;

		public const ushort CategoryScripts = 1;

		public const ushort CategoryGUI = 4;

		public const ushort CategoryPhysics = 5;

		public const ushort CategoryAnimation = 6;

		public const ushort CategoryAi = 7;

		public const ushort CategoryAudio = 8;

		public const ushort CategoryVideo = 11;

		public const ushort CategoryParticles = 12;

		public const ushort CategoryLighting = 13;

		[Obsolete("CategoryLightning has been renamed. Use CategoryLighting instead (UnityUpgradable) -> CategoryLighting", false)]
		public const ushort CategoryLightning = 13;

		public const ushort CategoryNetwork = 14;

		public const ushort CategoryLoading = 15;

		public const ushort CategoryOther = 16;

		public const ushort CategoryVr = 22;

		public const ushort CategoryAllocation = 23;

		public const ushort CategoryInternal = 24;

		public const ushort CategoryFileIO = 25;

		public const ushort CategoryInput = 30;

		public const ushort CategoryVirtualTexturing = 31;

		internal const ushort CategoryGPU = 32;

		public const ushort CategoryPhysics2D = 33;

		internal const ushort CategoryAny = 65535;

		public struct TimestampConversionRatio
		{
			public long Numerator;

			public long Denominator;
		}
	}
}
