using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[RequiredByNativeCode]
	[NativeHeader("Modules/Audio/Public/Director/AudioClipPlayable.h")]
	[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioClipPlayable.bindings.h")]
	[StaticAccessor("AudioClipPlayableBindings", StaticAccessorType.DoubleColon)]
	public struct AudioClipPlayable : IPlayable, IEquatable<AudioClipPlayable>
	{
		internal AudioClipPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AudioClipPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AudioClipPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AudioClipPlayable Create(PlayableGraph graph, AudioClip clip, bool looping)
		{
			PlayableHandle playableHandle = AudioClipPlayable.CreateHandle(graph, clip, looping);
			AudioClipPlayable audioClipPlayable = new AudioClipPlayable(playableHandle);
			if (clip != null)
			{
				audioClipPlayable.SetDuration((double)clip.length);
			}
			return audioClipPlayable;
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, AudioClip clip, bool looping)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AudioClipPlayable.InternalCreateAudioClipPlayable(ref graph, clip, looping, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				playableHandle = @null;
			}
			return playableHandle;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator Playable(AudioClipPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AudioClipPlayable(Playable playable)
		{
			return new AudioClipPlayable(playable.GetHandle());
		}

		public bool Equals(AudioClipPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		public AudioClip GetClip()
		{
			return AudioClipPlayable.GetClipInternal(ref this.m_Handle);
		}

		public void SetClip(AudioClip value)
		{
			AudioClipPlayable.SetClipInternal(ref this.m_Handle, value);
		}

		public bool GetLooped()
		{
			return AudioClipPlayable.GetLoopedInternal(ref this.m_Handle);
		}

		public void SetLooped(bool value)
		{
			AudioClipPlayable.SetLoopedInternal(ref this.m_Handle, value);
		}

		[Obsolete("IsPlaying() has been deprecated. Use IsChannelPlaying() instead (UnityUpgradable) -> IsChannelPlaying()", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsPlaying()
		{
			return this.IsChannelPlaying();
		}

		public bool IsChannelPlaying()
		{
			return AudioClipPlayable.GetIsChannelPlayingInternal(ref this.m_Handle);
		}

		public double GetStartDelay()
		{
			return AudioClipPlayable.GetStartDelayInternal(ref this.m_Handle);
		}

		internal void SetStartDelay(double value)
		{
			AudioClipPlayable.SetStartDelayInternal(ref this.m_Handle, value);
		}

		public double GetPauseDelay()
		{
			return AudioClipPlayable.GetPauseDelayInternal(ref this.m_Handle);
		}

		internal void GetPauseDelay(double value)
		{
			double pauseDelayInternal = AudioClipPlayable.GetPauseDelayInternal(ref this.m_Handle);
			if (this.m_Handle.GetPlayState() == PlayState.Playing && (value < 0.05 || (pauseDelayInternal != 0.0 && pauseDelayInternal < 0.05)))
			{
				throw new ArgumentException("AudioClipPlayable.pauseDelay: Setting new delay when existing delay is too small or 0.0 (" + pauseDelayInternal + "), audio system will not be able to change in time");
			}
			AudioClipPlayable.SetPauseDelayInternal(ref this.m_Handle, value);
		}

		public void Seek(double startTime, double startDelay)
		{
			this.Seek(startTime, startDelay, 0.0);
		}

		public void Seek(double startTime, double startDelay, [DefaultValue("0")] double duration)
		{
			AudioClipPlayable.SetStartDelayInternal(ref this.m_Handle, startDelay);
			if (duration > 0.0)
			{
				this.m_Handle.SetDuration(duration + startTime);
				AudioClipPlayable.SetPauseDelayInternal(ref this.m_Handle, startDelay + duration);
			}
			else
			{
				this.m_Handle.SetDuration(double.MaxValue);
				AudioClipPlayable.SetPauseDelayInternal(ref this.m_Handle, 0.0);
			}
			this.m_Handle.SetTime(startTime);
			this.m_Handle.Play();
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioClip GetClipInternal(ref PlayableHandle hdl);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetClipInternal(ref PlayableHandle hdl, AudioClip clip);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetLoopedInternal(ref PlayableHandle hdl);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLoopedInternal(ref PlayableHandle hdl, bool looped);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsChannelPlayingInternal(ref PlayableHandle hdl);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double GetStartDelayInternal(ref PlayableHandle hdl);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStartDelayInternal(ref PlayableHandle hdl, double delay);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double GetPauseDelayInternal(ref PlayableHandle hdl);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPauseDelayInternal(ref PlayableHandle hdl, double delay);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalCreateAudioClipPlayable(ref PlayableGraph graph, AudioClip clip, bool looping, ref PlayableHandle handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ValidateType(ref PlayableHandle hdl);

		private PlayableHandle m_Handle;
	}
}
