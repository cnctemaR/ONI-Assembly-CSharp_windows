using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables
{
	[NativeHeader("Runtime/Graphics/Director/TextureMixerPlayable.h")]
	[NativeHeader("Runtime/Export/Director/TextureMixerPlayable.bindings.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[RequiredByNativeCode]
	[StaticAccessor("TextureMixerPlayableBindings", StaticAccessorType.DoubleColon)]
	public struct TextureMixerPlayable : IPlayable, IEquatable<TextureMixerPlayable>
	{
		internal TextureMixerPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<TextureMixerPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an TextureMixerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static TextureMixerPlayable Create(PlayableGraph graph)
		{
			PlayableHandle playableHandle = TextureMixerPlayable.CreateHandle(graph);
			return new TextureMixerPlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!TextureMixerPlayable.CreateTextureMixerPlayableInternal(ref graph, ref @null))
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

		public static implicit operator Playable(TextureMixerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator TextureMixerPlayable(Playable playable)
		{
			return new TextureMixerPlayable(playable.GetHandle());
		}

		public bool Equals(TextureMixerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateTextureMixerPlayableInternal(ref PlayableGraph graph, ref PlayableHandle handle);

		private PlayableHandle m_Handle;
	}
}
