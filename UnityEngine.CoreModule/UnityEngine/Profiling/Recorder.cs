using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	/// <summary>
	///   <para>Records profiling data produced by a specific Sampler.</para>
	/// </summary>
	[UsedByNativeCode]
	public sealed class Recorder
	{
		internal Recorder()
		{
		}

		~Recorder()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				this.DisposeNative();
			}
		}

		/// <summary>
		///   <para>Use this function to get a Recorder for the specific Profiler label.</para>
		/// </summary>
		/// <param name="samplerName">Sampler name.</param>
		/// <returns>
		///   <para>Recorder object for the specified Sampler.</para>
		/// </returns>
		public static Recorder Get(string samplerName)
		{
			Recorder @internal = Recorder.GetInternal(samplerName);
			Recorder recorder;
			if (@internal == null)
			{
				recorder = Recorder.s_InvalidRecorder;
			}
			else
			{
				recorder = @internal;
			}
			return recorder;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Recorder GetInternal(string samplerName);

		/// <summary>
		///   <para>Returns true if Recorder is valid and can collect data. (Read Only)</para>
		/// </summary>
		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DisposeNative();

		/// <summary>
		///   <para>Enables recording.</para>
		/// </summary>
		[ThreadAndSerializationSafe]
		public extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Accumulated time of Begin/End pairs for the previous frame in nanoseconds. (Read Only)</para>
		/// </summary>
		[ThreadAndSerializationSafe]
		public extern long elapsedNanoseconds
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Number of time Begin/End pairs was called during the previous frame. (Read Only)</para>
		/// </summary>
		[ThreadAndSerializationSafe]
		public extern int sampleBlockCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal IntPtr m_Ptr;

		internal static Recorder s_InvalidRecorder = new Recorder();
	}
}
