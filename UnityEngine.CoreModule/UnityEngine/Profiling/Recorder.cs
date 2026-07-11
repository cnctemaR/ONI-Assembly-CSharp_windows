using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Profiler/ScriptBindings/Recorder.bindings.h")]
	[NativeHeader("Runtime/Profiler/Recorder.h")]
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

		protected override void Finalize()
		{
			try
			{
				bool flag = this.m_Ptr != IntPtr.Zero;
				if (flag)
				{
					Recorder.DisposeNative(this.m_Ptr);
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		public static Recorder Get(string samplerName)
		{
			IntPtr @internal = Recorder.GetInternal(samplerName);
			bool flag = @internal == IntPtr.Zero;
			Recorder recorder;
			if (flag)
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
				bool isValid = this.isValid;
				if (isValid)
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
				return this.isValid ? this.GetElapsedNanoseconds() : 0L;
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetElapsedNanoseconds();

		public int sampleBlockCount
		{
			get
			{
				return this.isValid ? this.GetSampleBlockCount() : 0;
			}
		}

		[NativeMethod(IsThreadSafe = true)]
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
