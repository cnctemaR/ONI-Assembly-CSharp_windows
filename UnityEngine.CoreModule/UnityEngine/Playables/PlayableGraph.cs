using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>Use the PlayableGraph to manage Playable creations and destructions.</para>
	/// </summary>
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
	[UsedByNativeCode]
	public struct PlayableGraph
	{
		/// <summary>
		///   <para>Returns the Playable with no output connections at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the root Playable.</param>
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

		/// <summary>
		///   <para>Get PlayableOutput at the given index in the graph.</para>
		/// </summary>
		/// <param name="index">The output index.</param>
		/// <returns>
		///   <para>The PlayableOutput at this given index, otherwise null.</para>
		/// </returns>
		public PlayableOutput GetOutput(int index)
		{
			PlayableOutputHandle playableOutputHandle;
			PlayableOutput playableOutput;
			if (!this.GetOutputInternal(index, out playableOutputHandle))
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
			PlayableOutput playableOutput;
			if (!this.GetOutputByTypeInternal(typeof(T), index, out playableOutputHandle))
			{
				playableOutput = PlayableOutput.Null;
			}
			else
			{
				playableOutput = new PlayableOutput(playableOutputHandle);
			}
			return playableOutput;
		}

		/// <summary>
		///   <para>Evaluates all the PlayableOutputs in the graph, and updates all the connected Playables in the graph.</para>
		/// </summary>
		/// <param name="deltaTime">The time in seconds by which to advance each Playable in the graph.</param>
		public void Evaluate()
		{
			this.Evaluate(0f);
		}

		/// <summary>
		///   <para>Creates a PlayableGraph.</para>
		/// </summary>
		/// <param name="name">The name of the graph.</param>
		/// <returns>
		///   <para>The newly created PlayableGraph.</para>
		/// </returns>
		public static PlayableGraph Create()
		{
			return PlayableGraph.Create(null);
		}

		/// <summary>
		///   <para>Creates a PlayableGraph.</para>
		/// </summary>
		/// <param name="name">The name of the graph.</param>
		/// <returns>
		///   <para>The newly created PlayableGraph.</para>
		/// </returns>
		public static PlayableGraph Create(string name)
		{
			PlayableGraph playableGraph;
			PlayableGraph.Create_Injected(name, out playableGraph);
			return playableGraph;
		}

		/// <summary>
		///   <para>Destroys the graph.</para>
		/// </summary>
		public void Destroy()
		{
			PlayableGraph.Destroy_Injected(ref this);
		}

		/// <summary>
		///   <para>Returns true if the PlayableGraph has been properly constructed using PlayableGraph.CreateGraph and is not deleted.</para>
		/// </summary>
		/// <returns>
		///   <para>A boolean indicating if the graph is invalid or not.</para>
		/// </returns>
		public bool IsValid()
		{
			return PlayableGraph.IsValid_Injected(ref this);
		}

		/// <summary>
		///   <para>Indicates that a graph is presently running.</para>
		/// </summary>
		/// <returns>
		///   <para>A boolean indicating if the graph is playing or not.</para>
		/// </returns>
		public bool IsPlaying()
		{
			return PlayableGraph.IsPlaying_Injected(ref this);
		}

		/// <summary>
		///   <para>Indicates that a graph has completed its operations.</para>
		/// </summary>
		/// <returns>
		///   <para>A boolean indicating if the graph is done playing or not.</para>
		/// </returns>
		public bool IsDone()
		{
			return PlayableGraph.IsDone_Injected(ref this);
		}

		/// <summary>
		///   <para>Plays the graph.</para>
		/// </summary>
		public void Play()
		{
			PlayableGraph.Play_Injected(ref this);
		}

		/// <summary>
		///   <para>Stops the graph, if it is playing.</para>
		/// </summary>
		public void Stop()
		{
			PlayableGraph.Stop_Injected(ref this);
		}

		/// <summary>
		///   <para>Evaluates all the PlayableOutputs in the graph, and updates all the connected Playables in the graph.</para>
		/// </summary>
		/// <param name="deltaTime">The time in seconds by which to advance each Playable in the graph.</param>
		public void Evaluate([DefaultValue("0")] float deltaTime)
		{
			PlayableGraph.Evaluate_Injected(ref this, deltaTime);
		}

		/// <summary>
		///   <para>Returns how time is incremented when playing back.</para>
		/// </summary>
		public DirectorUpdateMode GetTimeUpdateMode()
		{
			return PlayableGraph.GetTimeUpdateMode_Injected(ref this);
		}

		/// <summary>
		///   <para>Changes how time is incremented when playing back.</para>
		/// </summary>
		/// <param name="value">The new DirectorUpdateMode.</param>
		public void SetTimeUpdateMode(DirectorUpdateMode value)
		{
			PlayableGraph.SetTimeUpdateMode_Injected(ref this, value);
		}

		/// <summary>
		///   <para>Returns the table used by the graph to resolve ExposedReferences.</para>
		/// </summary>
		public IExposedPropertyTable GetResolver()
		{
			return PlayableGraph.GetResolver_Injected(ref this);
		}

		/// <summary>
		///   <para>Changes the table used by the graph to resolve ExposedReferences.</para>
		/// </summary>
		/// <param name="value"></param>
		public void SetResolver(IExposedPropertyTable value)
		{
			PlayableGraph.SetResolver_Injected(ref this, value);
		}

		/// <summary>
		///   <para>Returns the number of Playable owned by the Graph.</para>
		/// </summary>
		public int GetPlayableCount()
		{
			return PlayableGraph.GetPlayableCount_Injected(ref this);
		}

		/// <summary>
		///   <para>Returns the number of Playable owned by the Graph that have no connected outputs.</para>
		/// </summary>
		public int GetRootPlayableCount()
		{
			return PlayableGraph.GetRootPlayableCount_Injected(ref this);
		}

		/// <summary>
		///   <para>Returns the number of PlayableOutput in the graph.</para>
		/// </summary>
		/// <returns>
		///   <para>The number of PlayableOutput in the graph.</para>
		/// </returns>
		public int GetOutputCount()
		{
			return PlayableGraph.GetOutputCount_Injected(ref this);
		}

		internal PlayableHandle CreatePlayableHandle()
		{
			PlayableHandle playableHandle;
			PlayableGraph.CreatePlayableHandle_Injected(ref this, out playableHandle);
			return playableHandle;
		}

		internal bool CreateScriptOutputInternal(string name, out PlayableOutputHandle handle)
		{
			return PlayableGraph.CreateScriptOutputInternal_Injected(ref this, name, out handle);
		}

		internal PlayableHandle GetRootPlayableInternal(int index)
		{
			PlayableHandle playableHandle;
			PlayableGraph.GetRootPlayableInternal_Injected(ref this, index, out playableHandle);
			return playableHandle;
		}

		internal void DestroyOutputInternal(PlayableOutputHandle handle)
		{
			PlayableGraph.DestroyOutputInternal_Injected(ref this, ref handle);
		}

		private bool GetOutputInternal(int index, out PlayableOutputHandle handle)
		{
			return PlayableGraph.GetOutputInternal_Injected(ref this, index, out handle);
		}

		private int GetOutputCountByTypeInternal(Type outputType)
		{
			return PlayableGraph.GetOutputCountByTypeInternal_Injected(ref this, outputType);
		}

		private bool GetOutputByTypeInternal(Type outputType, int index, out PlayableOutputHandle handle)
		{
			return PlayableGraph.GetOutputByTypeInternal_Injected(ref this, outputType, index, out handle);
		}

		private bool ConnectInternal(PlayableHandle source, int sourceOutputPort, PlayableHandle destination, int destinationInputPort)
		{
			return PlayableGraph.ConnectInternal_Injected(ref this, ref source, sourceOutputPort, ref destination, destinationInputPort);
		}

		private void DisconnectInternal(PlayableHandle playable, int inputPort)
		{
			PlayableGraph.DisconnectInternal_Injected(ref this, ref playable, inputPort);
		}

		private void DestroyPlayableInternal(PlayableHandle playable)
		{
			PlayableGraph.DestroyPlayableInternal_Injected(ref this, ref playable);
		}

		private void DestroySubgraphInternal(PlayableHandle playable)
		{
			PlayableGraph.DestroySubgraphInternal_Injected(ref this, ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Create_Injected(string name, out PlayableGraph ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValid_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsPlaying_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsDone_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Play_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Evaluate_Injected(ref PlayableGraph _unity_self, [DefaultValue("0")] float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DirectorUpdateMode GetTimeUpdateMode_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTimeUpdateMode_Injected(ref PlayableGraph _unity_self, DirectorUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IExposedPropertyTable GetResolver_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetResolver_Injected(ref PlayableGraph _unity_self, IExposedPropertyTable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPlayableCount_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRootPlayableCount_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetOutputCount_Injected(ref PlayableGraph _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreatePlayableHandle_Injected(ref PlayableGraph _unity_self, out PlayableHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateScriptOutputInternal_Injected(ref PlayableGraph _unity_self, string name, out PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootPlayableInternal_Injected(ref PlayableGraph _unity_self, int index, out PlayableHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyOutputInternal_Injected(ref PlayableGraph _unity_self, ref PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetOutputInternal_Injected(ref PlayableGraph _unity_self, int index, out PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetOutputCountByTypeInternal_Injected(ref PlayableGraph _unity_self, Type outputType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetOutputByTypeInternal_Injected(ref PlayableGraph _unity_self, Type outputType, int index, out PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ConnectInternal_Injected(ref PlayableGraph _unity_self, ref PlayableHandle source, int sourceOutputPort, ref PlayableHandle destination, int destinationInputPort);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisconnectInternal_Injected(ref PlayableGraph _unity_self, ref PlayableHandle playable, int inputPort);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyPlayableInternal_Injected(ref PlayableGraph _unity_self, ref PlayableHandle playable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroySubgraphInternal_Injected(ref PlayableGraph _unity_self, ref PlayableHandle playable);

		internal IntPtr m_Handle;

		internal uint m_Version;
	}
}
