using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Audio
{
	[NativeType(Header = "Modules/Audio/Public/ScriptBindings/AudioSampleProvider.bindings.h")]
	[StaticAccessor("AudioSampleProviderBindings", StaticAccessorType.DoubleColon)]
	public class AudioSampleProvider : IDisposable
	{
		private AudioSampleProvider(uint providerId, Object ownerObj, ushort trackIdx)
		{
			this.owner = ownerObj;
			this.id = providerId;
			this.trackIndex = trackIdx;
			this.m_ConsumeSampleFramesNativeFunction = (AudioSampleProvider.ConsumeSampleFramesNativeFunction)Marshal.GetDelegateForFunctionPointer(AudioSampleProvider.InternalGetConsumeSampleFramesNativeFunctionPtr(), typeof(AudioSampleProvider.ConsumeSampleFramesNativeFunction));
			ushort num = 0;
			uint num2 = 0U;
			AudioSampleProvider.InternalGetFormatInfo(providerId, out num, out num2);
			this.channelCount = num;
			this.sampleRate = num2;
			AudioSampleProvider.InternalSetScriptingPtr(providerId, this);
		}

		[VisibleToOtherModules]
		internal static AudioSampleProvider Lookup(uint providerId, Object ownerObj, ushort trackIndex)
		{
			AudioSampleProvider audioSampleProvider = AudioSampleProvider.InternalGetScriptingPtr(providerId);
			AudioSampleProvider audioSampleProvider2;
			if (audioSampleProvider != null || !AudioSampleProvider.InternalIsValid(providerId))
			{
				audioSampleProvider2 = audioSampleProvider;
			}
			else
			{
				audioSampleProvider2 = new AudioSampleProvider(providerId, ownerObj, trackIndex);
			}
			return audioSampleProvider2;
		}

		~AudioSampleProvider()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.id != 0U)
			{
				AudioSampleProvider.InternalSetScriptingPtr(this.id, null);
				this.id = 0U;
			}
			GC.SuppressFinalize(this);
		}

		public uint id { get; private set; }

		public ushort trackIndex { get; private set; }

		public Object owner { get; private set; }

		public bool valid
		{
			get
			{
				return AudioSampleProvider.InternalIsValid(this.id);
			}
		}

		public ushort channelCount { get; private set; }

		public uint sampleRate { get; private set; }

		public uint maxSampleFrameCount
		{
			get
			{
				return AudioSampleProvider.InternalGetMaxSampleFrameCount(this.id);
			}
		}

		public uint availableSampleFrameCount
		{
			get
			{
				return AudioSampleProvider.InternalGetAvailableSampleFrameCount(this.id);
			}
		}

		public uint freeSampleFrameCount
		{
			get
			{
				return AudioSampleProvider.InternalGetFreeSampleFrameCount(this.id);
			}
		}

		public uint freeSampleFrameCountLowThreshold
		{
			get
			{
				return AudioSampleProvider.InternalGetFreeSampleFrameCountLowThreshold(this.id);
			}
			set
			{
				AudioSampleProvider.InternalSetFreeSampleFrameCountLowThreshold(this.id, value);
			}
		}

		public bool enableSampleFramesAvailableEvents
		{
			get
			{
				return AudioSampleProvider.InternalGetEnableSampleFramesAvailableEvents(this.id);
			}
			set
			{
				AudioSampleProvider.InternalSetEnableSampleFramesAvailableEvents(this.id, value);
			}
		}

		public bool enableSilencePadding
		{
			get
			{
				return AudioSampleProvider.InternalGetEnableSilencePadding(this.id);
			}
			set
			{
				AudioSampleProvider.InternalSetEnableSilencePadding(this.id, value);
			}
		}

		public uint ConsumeSampleFrames(NativeArray<float> sampleFrames)
		{
			uint num;
			if (this.channelCount == 0)
			{
				num = 0U;
			}
			else
			{
				num = this.m_ConsumeSampleFramesNativeFunction(this.id, (IntPtr)sampleFrames.GetUnsafePtr<float>(), (uint)(sampleFrames.Length / (int)this.channelCount));
			}
			return num;
		}

		public static AudioSampleProvider.ConsumeSampleFramesNativeFunction consumeSampleFramesNativeFunction
		{
			get
			{
				return (AudioSampleProvider.ConsumeSampleFramesNativeFunction)Marshal.GetDelegateForFunctionPointer(AudioSampleProvider.InternalGetConsumeSampleFramesNativeFunctionPtr(), typeof(AudioSampleProvider.ConsumeSampleFramesNativeFunction));
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event AudioSampleProvider.SampleFramesHandler sampleFramesAvailable;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event AudioSampleProvider.SampleFramesHandler sampleFramesOverflow;

		public void SetSampleFramesAvailableNativeHandler(AudioSampleProvider.SampleFramesEventNativeFunction handler, IntPtr userData)
		{
			AudioSampleProvider.InternalSetSampleFramesAvailableNativeHandler(this.id, Marshal.GetFunctionPointerForDelegate(handler), userData);
		}

		public void ClearSampleFramesAvailableNativeHandler()
		{
			AudioSampleProvider.InternalClearSampleFramesAvailableNativeHandler(this.id);
		}

		public void SetSampleFramesOverflowNativeHandler(AudioSampleProvider.SampleFramesEventNativeFunction handler, IntPtr userData)
		{
			AudioSampleProvider.InternalSetSampleFramesOverflowNativeHandler(this.id, Marshal.GetFunctionPointerForDelegate(handler), userData);
		}

		public void ClearSampleFramesOverflowNativeHandler()
		{
			AudioSampleProvider.InternalClearSampleFramesOverflowNativeHandler(this.id);
		}

		[RequiredByNativeCode]
		private void InvokeSampleFramesAvailable(int sampleFrameCount)
		{
			if (this.sampleFramesAvailable != null)
			{
				this.sampleFramesAvailable(this, (uint)sampleFrameCount);
			}
		}

		[RequiredByNativeCode]
		private void InvokeSampleFramesOverflow(int droppedSampleFrameCount)
		{
			if (this.sampleFramesOverflow != null)
			{
				this.sampleFramesOverflow(this, (uint)droppedSampleFrameCount);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetFormatInfo(uint providerId, out ushort chCount, out uint sRate);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioSampleProvider InternalGetScriptingPtr(uint providerId);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetScriptingPtr(uint providerId, AudioSampleProvider provider);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalIsValid(uint providerId);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint InternalGetMaxSampleFrameCount(uint providerId);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint InternalGetAvailableSampleFrameCount(uint providerId);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint InternalGetFreeSampleFrameCount(uint providerId);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint InternalGetFreeSampleFrameCountLowThreshold(uint providerId);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetFreeSampleFrameCountLowThreshold(uint providerId, uint sampleFrameCount);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetEnableSampleFramesAvailableEvents(uint providerId);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetEnableSampleFramesAvailableEvents(uint providerId, bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetSampleFramesAvailableNativeHandler(uint providerId, IntPtr handler, IntPtr userData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalClearSampleFramesAvailableNativeHandler(uint providerId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetSampleFramesOverflowNativeHandler(uint providerId, IntPtr handler, IntPtr userData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalClearSampleFramesOverflowNativeHandler(uint providerId);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetEnableSilencePadding(uint id);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetEnableSilencePadding(uint id, bool enabled);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetConsumeSampleFramesNativeFunctionPtr();

		private AudioSampleProvider.ConsumeSampleFramesNativeFunction m_ConsumeSampleFramesNativeFunction;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate uint ConsumeSampleFramesNativeFunction(uint providerId, IntPtr interleavedSampleFrames, uint sampleFrameCount);

		public delegate void SampleFramesHandler(AudioSampleProvider provider, uint sampleFrameCount);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SampleFramesEventNativeFunction(IntPtr userData, uint providerId, uint sampleFrameCount);
	}
}
