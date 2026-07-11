using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Profiling
{
	/// <summary>
	///   <para>Controls the from script.</para>
	/// </summary>
	[UsedByNativeCode]
	[MovedFrom("UnityEngine")]
	public sealed class Profiler
	{
		private Profiler()
		{
		}

		public static extern bool supported
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Specifies the file to use when writing profiling data.</para>
		/// </summary>
		public static extern string logFile
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enables the logging of profiling data to a file.</para>
		/// </summary>
		public static extern bool enableBinaryLog
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enables the Profiler.</para>
		/// </summary>
		public static extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Displays the recorded profile data in the profiler.</para>
		/// </summary>
		/// <param name="file">The name of the file containing the frame data, including extension.</param>
		[Conditional("UNITY_EDITOR")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddFramesFromFile(string file);

		/// <summary>
		///   <para>Enables profiling on the thread from which you call this method.</para>
		/// </summary>
		/// <param name="threadGroupName">The name of the thread group to which the thread belongs.</param>
		/// <param name="threadName">The name of the thread.</param>
		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginThreadProfiling(string threadGroupName, string threadName);

		/// <summary>
		///   <para>Frees the internal resources used by the Profiler for the thread.</para>
		/// </summary>
		[Conditional("ENABLE_PROFILER")]
		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndThreadProfiling();

		/// <summary>
		///   <para>Begin profiling a piece of code with a custom label.</para>
		/// </summary>
		/// <param name="name">A string to identify the sample in the Profiler window.</param>
		/// <param name="targetObject">An object that provides context to the sample,.</param>
		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string name)
		{
			Profiler.BeginSampleOnly(name);
		}

		/// <summary>
		///   <para>Begin profiling a piece of code with a custom label.</para>
		/// </summary>
		/// <param name="name">A string to identify the sample in the Profiler window.</param>
		/// <param name="targetObject">An object that provides context to the sample,.</param>
		[Conditional("ENABLE_PROFILER")]
		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginSample(string name, Object targetObject);

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSampleOnly(string name);

		/// <summary>
		///   <para>Ends the current profiling sample.</para>
		/// </summary>
		[Conditional("ENABLE_PROFILER")]
		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndSample();

		/// <summary>
		///   <para>Resize the profiler sample buffers to allow the desired amount of samples per thread.</para>
		/// </summary>
		[Obsolete("maxNumberOfSamplesPerFrame is no longer needed, as the profiler buffers auto expand as needed")]
		public static extern int maxNumberOfSamplesPerFrame
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Heap size used by the program.</para>
		/// </summary>
		/// <returns>
		///   <para>Size of the used heap in bytes, (or 0 if the profiler is disabled).</para>
		/// </returns>
		[Obsolete("usedHeapSize has been deprecated since it is limited to 4GB. Please use usedHeapSizeLong instead.")]
		public static extern uint usedHeapSize
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the number of bytes that Unity has allocated. This does not include bytes allocated by external libraries or drivers.</para>
		/// </summary>
		/// <returns>
		///   <para>Size of the memory allocated by Unity (or 0 if the profiler is disabled).</para>
		/// </returns>
		public static extern long usedHeapSizeLong
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the runtime memory usage of the resource.</para>
		/// </summary>
		/// <param name="o"></param>
		[Obsolete("GetRuntimeMemorySize has been deprecated since it is limited to 2GB. Please use GetRuntimeMemorySizeLong() instead.")]
		public static int GetRuntimeMemorySize(Object o)
		{
			return (int)Profiler.GetRuntimeMemorySizeLong(o);
		}

		/// <summary>
		///   <para>Gathers the native-memory used by a Unity object.</para>
		/// </summary>
		/// <param name="o">The target Unity object.</param>
		/// <returns>
		///   <para>The amount of native-memory used by a Unity object. This returns 0 if the Profiler is not available.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetRuntimeMemorySizeLong(Object o);

		/// <summary>
		///   <para>Returns the size of the mono heap.</para>
		/// </summary>
		[Obsolete("GetMonoHeapSize has been deprecated since it is limited to 4GB. Please use GetMonoHeapSizeLong() instead.")]
		public static uint GetMonoHeapSize()
		{
			return (uint)Profiler.GetMonoHeapSizeLong();
		}

		/// <summary>
		///   <para>Returns the size of the reserved space for managed-memory.</para>
		/// </summary>
		/// <returns>
		///   <para>The size of the managed heap. This returns 0 if the Profiler is not available.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetMonoHeapSizeLong();

		/// <summary>
		///   <para>Returns the used size from mono.</para>
		/// </summary>
		[Obsolete("GetMonoUsedSize has been deprecated since it is limited to 4GB. Please use GetMonoUsedSizeLong() instead.")]
		public static uint GetMonoUsedSize()
		{
			return (uint)Profiler.GetMonoUsedSizeLong();
		}

		/// <summary>
		///   <para>The allocated managed-memory for live objects and non-collected objects.</para>
		/// </summary>
		/// <returns>
		///   <para>A long integer value of the memory in use. This returns 0 if the Profiler is not available.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetMonoUsedSizeLong();

		/// <summary>
		///   <para>Sets the size of the temp allocator.</para>
		/// </summary>
		/// <param name="size">Size in bytes.</param>
		/// <returns>
		///   <para>Returns true if requested size was successfully set. Will return false if value is disallowed (too small).</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTempAllocatorRequestedSize(uint size);

		/// <summary>
		///   <para>Returns the size of the temp allocator.</para>
		/// </summary>
		/// <returns>
		///   <para>Size in bytes.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetTempAllocatorSize();

		/// <summary>
		///   <para>Returns the amount of allocated and used system memory.</para>
		/// </summary>
		[Obsolete("GetTotalAllocatedMemory has been deprecated since it is limited to 4GB. Please use GetTotalAllocatedMemoryLong() instead.")]
		public static uint GetTotalAllocatedMemory()
		{
			return (uint)Profiler.GetTotalAllocatedMemoryLong();
		}

		/// <summary>
		///   <para>The total memory allocated by the internal allocators in Unity. Unity reserves large pools of memory from the system. This function returns the amount of used memory in those pools.</para>
		/// </summary>
		/// <returns>
		///   <para>The amount of memory allocated by Unity. This returns 0 if the Profiler is not available.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalAllocatedMemoryLong();

		/// <summary>
		///   <para>Returns the amount of reserved but not used system memory.</para>
		/// </summary>
		[Obsolete("GetTotalUnusedReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalUnusedReservedMemoryLong() instead.")]
		public static uint GetTotalUnusedReservedMemory()
		{
			return (uint)Profiler.GetTotalUnusedReservedMemoryLong();
		}

		/// <summary>
		///   <para>Unity allocates memory in pools for usage when unity needs to allocate memory. This function returns the amount of unused memory in these pools.</para>
		/// </summary>
		/// <returns>
		///   <para>The amount of unused memory in the reserved pools. This returns 0 if the Profiler is not available.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalUnusedReservedMemoryLong();

		/// <summary>
		///   <para>Returns the amount of reserved system memory.</para>
		/// </summary>
		[Obsolete("GetTotalReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalReservedMemoryLong() instead.")]
		public static uint GetTotalReservedMemory()
		{
			return (uint)Profiler.GetTotalReservedMemoryLong();
		}

		/// <summary>
		///   <para>The total memory Unity has reserved.</para>
		/// </summary>
		/// <returns>
		///   <para>Memory reserved by Unity in bytes. This returns 0 if the Profiler is not available.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalReservedMemoryLong();

		/// <summary>
		///   <para>Returns the amount of allocated memory for the graphics driver, in bytes.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetAllocatedMemoryForGraphicsDriver();
	}
}
