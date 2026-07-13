using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioPlayableOutput.bindings.h")]
	[NativeHeader("Modules/Audio/Public/Director/AudioPlayableOutput.h")]
	[RequiredByNativeCode]
	[NativeHeader("Modules/Audio/Public/AudioSource.h")]
	[StaticAccessor("AudioPlayableOutputBindings", StaticAccessorType.DoubleColon)]
	public struct AudioPlayableOutput : IPlayableOutput
	{
		public static AudioPlayableOutput Create(PlayableGraph graph, string name, AudioSource target)
		{
			PlayableOutputHandle playableOutputHandle;
			bool flag = !AudioPlayableGraphExtensions.InternalCreateAudioOutput(ref graph, name, out playableOutputHandle);
			AudioPlayableOutput audioPlayableOutput;
			if (flag)
			{
				audioPlayableOutput = AudioPlayableOutput.Null;
			}
			else
			{
				AudioPlayableOutput audioPlayableOutput2 = new AudioPlayableOutput(playableOutputHandle);
				audioPlayableOutput2.SetTarget(target);
				audioPlayableOutput = audioPlayableOutput2;
			}
			return audioPlayableOutput;
		}

		internal AudioPlayableOutput(PlayableOutputHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOutputOfType<AudioPlayableOutput>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AudioPlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		public static AudioPlayableOutput Null
		{
			get
			{
				return new AudioPlayableOutput(PlayableOutputHandle.Null);
			}
		}

		public PlayableOutputHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator PlayableOutput(AudioPlayableOutput output)
		{
			return new PlayableOutput(output.GetHandle());
		}

		public static explicit operator AudioPlayableOutput(PlayableOutput output)
		{
			return new AudioPlayableOutput(output.GetHandle());
		}

		public AudioSource GetTarget()
		{
			return AudioPlayableOutput.InternalGetTarget(ref this.m_Handle);
		}

		public void SetTarget(AudioSource value)
		{
			AudioPlayableOutput.InternalSetTarget(ref this.m_Handle, value);
		}

		public bool GetEvaluateOnSeek()
		{
			return AudioPlayableOutput.InternalGetEvaluateOnSeek(ref this.m_Handle);
		}

		public void SetEvaluateOnSeek(bool value)
		{
			AudioPlayableOutput.InternalSetEvaluateOnSeek(ref this.m_Handle, value);
		}

		[NativeThrows]
		private static AudioSource InternalGetTarget(ref PlayableOutputHandle output)
		{
			return Unmarshal.UnmarshalUnityObject<AudioSource>(AudioPlayableOutput.InternalGetTarget_Injected(ref output));
		}

		[NativeThrows]
		private static void InternalSetTarget(ref PlayableOutputHandle output, AudioSource target)
		{
			AudioPlayableOutput.InternalSetTarget_Injected(ref output, Object.MarshalledUnityObject.Marshal<AudioSource>(target));
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetEvaluateOnSeek(ref PlayableOutputHandle output);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetEvaluateOnSeek(ref PlayableOutputHandle output, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetTarget_Injected(ref PlayableOutputHandle output);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetTarget_Injected(ref PlayableOutputHandle output, IntPtr target);

		private PlayableOutputHandle m_Handle;
	}
}
