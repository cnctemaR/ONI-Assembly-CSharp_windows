using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Profiler/ScriptBindings/Sampler.bindings.h")]
	[NativeHeader("Runtime/Profiler/Marker.h")]
	public sealed class CustomSampler : Sampler
	{
		internal CustomSampler()
		{
		}

		internal CustomSampler(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public static CustomSampler Create(string name)
		{
			IntPtr intPtr = CustomSampler.CreateInternal(name);
			CustomSampler customSampler;
			if (intPtr == IntPtr.Zero)
			{
				customSampler = CustomSampler.s_InvalidCustomSampler;
			}
			else
			{
				customSampler = new CustomSampler(intPtr);
			}
			return customSampler;
		}

		[NativeMethod(Name = "ProfilerBindings::CreateCustomSamplerInternal", IsFreeFunction = true, ThrowsException = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateInternal([NotNull] string name);

		[Conditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "ProfilerBindings::CustomSampler_Begin", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Begin();

		[Conditional("ENABLE_PROFILER")]
		public void Begin(Object targetObject)
		{
			this.BeginWithObject(targetObject);
		}

		[NativeMethod(Name = "ProfilerBindings::CustomSampler_BeginWithObject", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void BeginWithObject(Object targetObject);

		[Conditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "ProfilerBindings::CustomSampler_End", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void End();

		internal static CustomSampler s_InvalidCustomSampler = new CustomSampler();
	}
}
