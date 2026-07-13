using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[NativeHeader("Runtime/Export/Director/PlayableGraph.bindings.h")]
	public struct PlayableGraph
	{
		public Playable GetRootPlayable(int index)
		{
			PlayableHandle rootPlayableInternal = this.GetRootPlayableInternal(index);
			return new Playable(rootPlayableInternal);
		}

		public bool Connect<U, V>(U source, int sourceOutputPort, V destination, int destinationInputPort) where U : struct, IPlayable where V : struct, IPlayable
		{
			return this.ConnectInternal(source.GetHandle(), sourceOutputPort, destination.GetHandle(), destinationInputPort);
		}

		public void Disconnect<U>(U input, int inputPort) where U : struct, IPlayable
		{
			this.DisconnectInternal(input.GetHandle(), inputPort);
		}

		public void DestroyPlayable<U>(U playable) where U : struct, IPlayable
		{
			this.DestroyPlayableInternal(playable.GetHandle());
		}

		public void DestroySubgraph<U>(U playable) where U : struct, IPlayable
		{
			this.DestroySubgraphInternal(playable.GetHandle());
		}

		public void DestroyOutput<U>(U output) where U : struct, IPlayableOutput
		{
			this.DestroyOutputInternal(output.GetHandle());
		}

		public int GetOutputCountByType<T>() where T : struct, IPlayableOutput
		{
			return this.GetOutputCountByTypeInternal(typeof(T));
		}

		public PlayableOutput GetOutput(int index)
		{
			PlayableOutputHandle playableOutputHandle;
			bool flag = !this.GetOutputInternal(index, out playableOutputHandle);
			PlayableOutput playableOutput;
			if (flag)
			{
				playableOutput = PlayableOutput.Null;
			}
			else
			{
				playableOutput = new PlayableOutput(playableOutputHandle);
			}
			return playableOutput;
		}

		public PlayableOutput GetOutputByType<T>(int index) where T : struct, IPlayableOutput
		{
			PlayableOutputHandle playableOutputHandle;
			bool flag = !this.GetOutputByTypeInternal(typeof(T), index, out playableOutputHandle);
			PlayableOutput playableOutput;
			if (flag)
			{
				playableOutput = PlayableOutput.Null;
			}
			else
			{
				playableOutput = new PlayableOutput(playableOutputHandle);
			}
			return playableOutput;
		}

		public void Evaluate()
		{
			this.Evaluate(0f);
		}

		public static PlayableGraph Create()
		{
			return PlayableGraph.Create(null);
		}

		public unsafe static PlayableGraph Create(string name)
		{
			PlayableGraph playableGraph2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				PlayableGraph playableGraph;
				PlayableGraph.Create_Injected(ref managedSpanWrapper, out playableGraph);
			}
			finally
			{
				char* ptr = null;
				PlayableGraph playableGraph;
				playableGraph2 = playableGraph;
			}
			return playableGraph2;
		}

		[FreeFunction("PlayableGraphBindings::Destroy", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Destroy();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsValid();

		[FreeFunction("PlayableGraphBindings::IsPlaying", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsPlaying();

		[FreeFunction("PlayableGraphBindings::IsDone", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsDone();

		[FreeFunction("PlayableGraphBindings::Play", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Play();

		[FreeFunction("PlayableGraphBindings::Stop", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();

		[FreeFunction("PlayableGraphBindings::Evaluate", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Evaluate([DefaultValue("0")] float deltaTime);

		[FreeFunction("PlayableGraphBindings::GetTimeUpdateMode", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern DirectorUpdateMode GetTimeUpdateMode();

		[FreeFunction("PlayableGraphBindings::SetTimeUpdateMode", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTimeUpdateMode(DirectorUpdateMode value);

		[FreeFunction("PlayableGraphBindings::GetResolver", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IExposedPropertyTable GetResolver();

		[FreeFunction("PlayableGraphBindings::SetResolver", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetResolver(IExposedPropertyTable value);

		[FreeFunction("PlayableGraphBindings::GetPlayableCount", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetPlayableCount();

		[FreeFunction("PlayableGraphBindings::GetRootPlayableCount", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetRootPlayableCount();

		[FreeFunction("PlayableGraphBindings::SynchronizeEvaluation", HasExplicitThis = true, ThrowsException = true)]
		internal void SynchronizeEvaluation(PlayableGraph playable)
		{
			PlayableGraph.SynchronizeEvaluation_Injected(ref this, ref playable);
		}

		[FreeFunction("PlayableGraphBindings::GetOutputCount", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetOutputCount();

		[FreeFunction("PlayableGraphBindings::CreatePlayableHandle", HasExplicitThis = true, ThrowsException = true)]
		internal PlayableHandle CreatePlayableHandle()
		{
			PlayableHandle playableHandle;
			PlayableGraph.CreatePlayableHandle_Injected(ref this, out playableHandle);
			return playableHandle;
		}

		[FreeFunction("PlayableGraphBindings::CreateScriptOutputInternal", HasExplicitThis = true, ThrowsException = true)]
		internal unsafe bool CreateScriptOutputInternal(string name, out PlayableOutputHandle handle)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = PlayableGraph.CreateScriptOutputInternal_Injected(ref this, ref managedSpanWrapper, out handle);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction("PlayableGraphBindings::GetRootPlayableInternal", HasExplicitThis = true, ThrowsException = true)]
		internal PlayableHandle GetRootPlayableInternal(int index)
		{
			PlayableHandle playableHandle;
			PlayableGraph.GetRootPlayableInternal_Injected(ref this, index, out playableHandle);
			return playableHandle;
		}

		[FreeFunction("PlayableGraphBindings::DestroyOutputInternal", HasExplicitThis = true, ThrowsException = true)]
		internal void DestroyOutputInternal(PlayableOutputHandle handle)
		{
			PlayableGraph.DestroyOutputInternal_Injected(ref this, ref handle);
		}

		[FreeFunction("PlayableGraphBindings::IsMatchFrameRateEnabled", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsMatchFrameRateEnabled();

		[FreeFunction("PlayableGraphBindings::EnableMatchFrameRate", HasExplicitThis = true, ThrowsException = true)]
		internal void EnableMatchFrameRate(FrameRate frameRate)
		{
			PlayableGraph.EnableMatchFrameRate_Injected(ref this, ref frameRate);
		}

		[FreeFunction("PlayableGraphBindings::DisableMatchFrameRate", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void DisableMatchFrameRate();

		[FreeFunction("PlayableGraphBindings::GetFrameRate", HasExplicitThis = true, ThrowsException = true)]
		internal FrameRate GetFrameRate()
		{
			FrameRate frameRate;
			PlayableGraph.GetFrameRate_Injected(ref this, out frameRate);
			return frameRate;
		}

		[FreeFunction("PlayableGraphBindings::GetOutputInternal", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetOutputInternal(int index, out PlayableOutputHandle handle);

		[FreeFunction("PlayableGraphBindings::GetOutputCountByTypeInternal", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetOutputCountByTypeInternal(Type outputType);

		[FreeFunction("PlayableGraphBindings::GetOutputByTypeInternal", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetOutputByTypeInternal(Type outputType, int index, out PlayableOutputHandle handle);

		[FreeFunction("PlayableGraphBindings::ConnectInternal", HasExplicitThis = true, ThrowsException = true)]
		private bool ConnectInternal(PlayableHandle source, int sourceOutputPort, PlayableHandle destination, int destinationInputPort)
		{
			return PlayableGraph.ConnectInternal_Injected(ref this, ref source, sourceOutputPort, ref destination, destinationInputPort);
		}

		[FreeFunction("PlayableGraphBindings::DisconnectInternal", HasExplicitThis = true, ThrowsException = true)]
		private void DisconnectInternal(PlayableHandle playable, int inputPort)
		{
			PlayableGraph.DisconnectInternal_Injected(ref this, ref playable, inputPort);
		}

		[FreeFunction("PlayableGraphBindings::DestroyPlayableInternal", HasExplicitThis = true, ThrowsException = true)]
		private void DestroyPlayableInternal(PlayableHandle playable)
		{
			PlayableGraph.DestroyPlayableInternal_Injected(ref this, ref playable);
		}

		[FreeFunction("PlayableGraphBindings::DestroySubgraphInternal", HasExplicitThis = true, ThrowsException = true)]
		private void DestroySubgraphInternal(PlayableHandle playable)
		{
			PlayableGraph.DestroySubgraphInternal_Injected(ref this, ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Create_Injected(ref ManagedSpanWrapper name, out PlayableGraph ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SynchronizeEvaluation_Injected(ref PlayableGraph _unity_self, [In] ref PlayableGraph playable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreatePlayableHandle_Injected(ref PlayableGraph _unity_self, out PlayableHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateScriptOutputInternal_Injected(ref PlayableGraph _unity_self, ref ManagedSpanWrapper name, out PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootPlayableInternal_Injected(ref PlayableGraph _unity_self, int index, out PlayableHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyOutputInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableMatchFrameRate_Injected(ref PlayableGraph _unity_self, [In] ref FrameRate frameRate);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetFrameRate_Injected(ref PlayableGraph _unity_self, out FrameRate ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ConnectInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableHandle source, int sourceOutputPort, [In] ref PlayableHandle destination, int destinationInputPort);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisconnectInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableHandle playable, int inputPort);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyPlayableInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableHandle playable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroySubgraphInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableHandle playable);

		internal IntPtr m_Handle;

		internal uint m_Version;
	}
}
