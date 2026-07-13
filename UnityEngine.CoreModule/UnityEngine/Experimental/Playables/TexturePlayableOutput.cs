using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables
{
	[NativeHeader("Runtime/Export/Director/TexturePlayableOutput.bindings.h")]
	[NativeHeader("Runtime/Graphics/Director/TexturePlayableOutput.h")]
	[NativeHeader("Runtime/Graphics/RenderTexture.h")]
	[StaticAccessor("TexturePlayableOutputBindings", StaticAccessorType.DoubleColon)]
	[RequiredByNativeCode]
	public struct TexturePlayableOutput : IPlayableOutput
	{
		public static TexturePlayableOutput Create(PlayableGraph graph, string name, RenderTexture target)
		{
			PlayableOutputHandle playableOutputHandle;
			bool flag = !TexturePlayableGraphExtensions.InternalCreateTextureOutput(ref graph, name, out playableOutputHandle);
			TexturePlayableOutput texturePlayableOutput;
			if (flag)
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

		internal TexturePlayableOutput(PlayableOutputHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOutputOfType<TexturePlayableOutput>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an TexturePlayableOutput.");
				}
			}
			this.m_Handle = handle;
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
		private static RenderTexture InternalGetTarget(ref PlayableOutputHandle output)
		{
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(TexturePlayableOutput.InternalGetTarget_Injected(ref output));
		}

		[NativeThrows]
		private static void InternalSetTarget(ref PlayableOutputHandle output, RenderTexture target)
		{
			TexturePlayableOutput.InternalSetTarget_Injected(ref output, Object.MarshalledUnityObject.Marshal<RenderTexture>(target));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetTarget_Injected(ref PlayableOutputHandle output);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetTarget_Injected(ref PlayableOutputHandle output, IntPtr target);

		private PlayableOutputHandle m_Handle;
	}
}
