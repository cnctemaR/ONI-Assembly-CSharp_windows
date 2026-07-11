using System;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[RequiredByNativeCode]
	public struct ScriptPlayableOutput : IPlayableOutput
	{
		internal ScriptPlayableOutput(PlayableOutputHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOutputOfType<ScriptPlayableOutput>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not a ScriptPlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		public static ScriptPlayableOutput Create(PlayableGraph graph, string name)
		{
			PlayableOutputHandle playableOutputHandle;
			ScriptPlayableOutput scriptPlayableOutput;
			if (!graph.CreateScriptOutputInternal(name, out playableOutputHandle))
			{
				scriptPlayableOutput = ScriptPlayableOutput.Null;
			}
			else
			{
				scriptPlayableOutput = new ScriptPlayableOutput(playableOutputHandle);
			}
			return scriptPlayableOutput;
		}

		public static ScriptPlayableOutput Null
		{
			get
			{
				return new ScriptPlayableOutput(PlayableOutputHandle.Null);
			}
		}

		public PlayableOutputHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator PlayableOutput(ScriptPlayableOutput output)
		{
			return new PlayableOutput(output.GetHandle());
		}

		public static explicit operator ScriptPlayableOutput(PlayableOutput output)
		{
			return new ScriptPlayableOutput(output.GetHandle());
		}

		private PlayableOutputHandle m_Handle;
	}
}
