using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables
{
	[NativeHeader("Runtime/Graphics/RenderTexture.h")]
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Export/Director/TexturePlayableOutput.bindings.h")]
	[NativeHeader("Runtime/Graphics/Director/TexturePlayableOutput.h")]
	[StaticAccessor("TexturePlayableOutputBindings", StaticAccessorType.DoubleColon)]
	public struct TexturePlayableOutput : IPlayableOutput
	{
		internal TexturePlayableOutput(PlayableOutputHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOutputOfType<TexturePlayableOutput>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an TexturePlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		public static TexturePlayableOutput Create(PlayableGraph graph, string name, RenderTexture target)
		{
			PlayableOutputHandle playableOutputHandle;
			TexturePlayableOutput texturePlayableOutput;
			if (!TexturePlayableGraphExtensions.InternalCreateTextureOutput(ref graph, name, out playableOutputHandle))
			{
				texturePlayableOutput = TexturePlayableOutput.Null;
			}
			else
			{
				TexturePlayableOutput texturePlayableOutput2 = new TexturePlayableOutput(playableOutputHandle);
				texturePlayableOutput2.SetTarget(target);
				texturePlayableOutput = texturePlayableOutput2;
			}
			return texturePlayableOutput;
		}

		public static TexturePlayableOutput Null
		{
			get
			{
				return new TexturePlayableOutput(PlayableOutputHandle.Null);
			}
		}

		public PlayableOutputHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator PlayableOutput(TexturePlayableOutput output)
		{
			return new PlayableOutput(output.GetHandle());
		}

		public static explicit operator TexturePlayableOutput(PlayableOutput output)
		{
			return new TexturePlayableOutput(output.GetHandle());
		}

		public RenderTexture GetTarget()
		{
			return TexturePlayableOutput.InternalGetTarget(ref this.m_Handle);
		}

		public void SetTarget(RenderTexture value)
		{
			TexturePlayableOutput.InternalSetTarget(ref this.m_Handle, value);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderTexture InternalGetTarget(ref PlayableOutputHandle output);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetTarget(ref PlayableOutputHandle output, RenderTexture target);

		private PlayableOutputHandle m_Handle;
	}
}
