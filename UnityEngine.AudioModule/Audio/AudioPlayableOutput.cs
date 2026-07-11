using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[RequiredByNativeCode]
	[StaticAccessor("AudioPlayableOutputBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Audio/Public/Director/AudioPlayableOutput.h")]
	[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioPlayableOutput.bindings.h")]
	[NativeHeader("Modules/Audio/Public/AudioSource.h")]
	public struct AudioPlayableOutput : IPlayableOutput
	{
		internal AudioPlayableOutput(PlayableOutputHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOutputOfType<AudioPlayableOutput>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AudioPlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		public static AudioPlayableOutput Create(PlayableGraph graph, string name, AudioSource target)
		{
			PlayableOutputHandle playableOutputHandle;
			AudioPlayableOutput audioPlayableOutput;
			if (!AudioPlayableGraphExtensions.InternalCreateAudioOutput(ref graph, name, out playableOutputHandle))
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioSource InternalGetTarget(ref PlayableOutputHandle output);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetTarget(ref PlayableOutputHandle output, AudioSource target);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetEvaluateOnSeek(ref PlayableOutputHandle output);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetEvaluateOnSeek(ref PlayableOutputHandle output, bool value);

		private PlayableOutputHandle m_Handle;
	}
}
