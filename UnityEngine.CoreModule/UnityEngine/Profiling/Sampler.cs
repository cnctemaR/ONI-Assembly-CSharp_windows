using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[NativeHeader("Runtime/Profiler/ScriptBindings/Sampler.bindings.h")]
	[NativeHeader("Runtime/Profiler/Marker.h")]
	[UsedByNativeCode]
	public class Sampler
	{
		internal Sampler()
		{
		}

		internal Sampler(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		public Recorder GetRecorder()
		{
			IntPtr recorderInternal = Sampler.GetRecorderInternal(this.m_Ptr);
			bool flag = recorderInternal == IntPtr.Zero;
			Recorder recorder;
			if (flag)
			{
				recorder = Recorder.s_InvalidRecorder;
			}
			else
			{
				recorder = new Recorder(recorderInternal);
			}
			return recorder;
		}

		public static Sampler Get(string name)
		{
			IntPtr samplerInternal = Sampler.GetSamplerInternal(name);
			bool flag = samplerInternal == IntPtr.Zero;
			Sampler sampler;
			if (flag)
			{
				sampler = Sampler.s_InvalidSampler;
			}
			else
			{
				sampler = new Sampler(samplerInternal);
			}
			return sampler;
		}

		public static int GetNames(List<string> names)
		{
			return Sampler.GetSamplerNamesInternal(names);
		}

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "GetName", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetSamplerName();

		public string name
		{
			get
			{
				return this.isValid ? this.GetSamplerName() : null;
			}
		}

		[NativeMethod(Name = "ProfilerBindings::GetRecorderInternal", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRecorderInternal(IntPtr ptr);

		[NativeMethod(Name = "ProfilerBindings::GetSamplerInternal", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSamplerInternal([NotNull] string name);

		[NativeMethod(Name = "ProfilerBindings::GetSamplerNamesInternal", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSamplerNamesInternal(List<string> namesScriptingPtr);

		internal IntPtr m_Ptr;

		internal static Sampler s_InvalidSampler = new Sampler();
	}
}
