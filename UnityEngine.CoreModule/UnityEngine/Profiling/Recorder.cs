using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[NativeHeader("Runtime/Profiler/ScriptBindings/Recorder.bindings.h")]
	[NativeHeader("Runtime/Profiler/Recorder.h")]
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Recorder
	{
		internal Recorder()
		{
		}

		internal Recorder(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		~Recorder()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				Recorder.DisposeNative(this.m_Ptr);
			}
		}

		public static Recorder Get(string samplerName)
		{
			IntPtr @internal = Recorder.GetInternal(samplerName);
			Recorder recorder;
			if (@internal == IntPtr.Zero)
			{
				recorder = Recorder.s_InvalidRecorder;
			}
			else
			{
				recorder = new Recorder(@internal);
			}
			return recorder;
		}

		[NativeMethod(Name = "ProfilerBindings::GetRecorderInternal", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetInternal(string samplerName);

		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		[NativeMethod(Name = "ProfilerBindings::DisposeNativeRecorder", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisposeNative(IntPtr ptr);

		public bool enabled
		{
			get
			{
				return this.isValid && this.IsEnabled();
			}
			set
			{
				if (this.isValid)
				{
					this.SetEnabled(value);
				}
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsEnabled();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetEnabled(bool enabled);

		public long elapsedNanoseconds
		{
			get
			{
				return (!this.isValid) ? 0L : this.GetElapsedNanoseconds();
			}
		}

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetElapsedNanoseconds();

		public int sampleBlockCount
		{
			get
			{
				return (!this.isValid) ? 0 : this.GetSampleBlockCount();
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetSampleBlockCount();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void FilterToCurrentThread();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CollectFromAllThreads();

		internal IntPtr m_Ptr;

		internal static Recorder s_InvalidRecorder = new Recorder();
	}
}
