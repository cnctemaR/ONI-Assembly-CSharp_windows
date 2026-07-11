using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	/// <summary>
	///   <para>Provides control over a CPU Profiler label.</para>
	/// </summary>
	[UsedByNativeCode]
	public class Sampler
	{
		internal Sampler()
		{
		}

		/// <summary>
		///   <para>Returns true if Sampler is valid. (Read Only)</para>
		/// </summary>
		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		/// <summary>
		///   <para>Returns Recorder associated with the Sampler.</para>
		/// </summary>
		/// <returns>
		///   <para>Recorder object associated with the Sampler.</para>
		/// </returns>
		public Recorder GetRecorder()
		{
			Recorder recorderInternal = this.GetRecorderInternal();
			return recorderInternal ?? Recorder.s_InvalidRecorder;
		}

		/// <summary>
		///   <para>Returns Sampler object for the specific CPU Profiler label.</para>
		/// </summary>
		/// <param name="name">Profiler Sampler name.</param>
		/// <returns>
		///   <para>Sampler object which represents specific profiler label.</para>
		/// </returns>
		public static Sampler Get(string name)
		{
			Sampler samplerInternal = Sampler.GetSamplerInternal(name);
			return samplerInternal ?? Sampler.s_InvalidSampler;
		}

		public static int GetNames(List<string> names)
		{
			return Sampler.GetSamplerNamesInternal(names);
		}

		/// <summary>
		///   <para>Sampler name. (Read Only)</para>
		/// </summary>
		[ThreadAndSerializationSafe]
		public extern string name
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Recorder GetRecorderInternal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Sampler GetSamplerInternal(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSamplerNamesInternal(object namesScriptingPtr);

		internal IntPtr m_Ptr;

		internal static Sampler s_InvalidSampler = new Sampler();
	}
}
