using System;

namespace UnityEngine.Playables
{
	internal struct DataPlayable<TPayload> : IPlayable, IEquatable<DataPlayable<TPayload>> where TPayload : struct
	{
		public static DataPlayable<TPayload> Null
		{
			get
			{
				return DataPlayable<TPayload>.m_NullPlayable;
			}
		}

		public static DataPlayable<TPayload> Create(PlayableGraph graph, int inputCount = 0)
		{
			return DataPlayable<TPayload>.Create(graph, default(TPayload), inputCount);
		}

		public static DataPlayable<TPayload> Create(PlayableGraph graph, TPayload payload, int inputCount = 0)
		{
			PlayableHandle playableHandle = DataPlayable<TPayload>.CreateHandle(graph, payload, inputCount);
			return new DataPlayable<TPayload>(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, TPayload payload, int inputCount)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !DataPlayableBindings.CreateHandleInternal(graph, ref @null);
			PlayableHandle playableHandle;
			if (flag)
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				@null.SetInputCount(inputCount);
				@null.SetScriptInstance(payload);
				playableHandle = @null;
			}
			return playableHandle;
		}

		internal DataPlayable(PlayableHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = typeof(TPayload) != handle.GetPlayableType();
				if (flag2)
				{
					throw new InvalidCastException(string.Format("Incompatible handle: Trying to assign a playable data of type `{0}` that is not compatible with the Payload of type `{1}`.", handle.GetPlayableType(), typeof(TPayload)));
				}
			}
			this.m_Handle = handle;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public TPayload GetPayload()
		{
			return this.m_Handle.GetPayload<TPayload>();
		}

		public void SetPayload(TPayload payload)
		{
			this.m_Handle.SetPayload<TPayload>(payload);
		}

		public static implicit operator Playable(DataPlayable<TPayload> playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator DataPlayable<TPayload>(Playable playable)
		{
			return new DataPlayable<TPayload>(playable.GetHandle());
		}

		public bool Equals(DataPlayable<TPayload> other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		private PlayableHandle m_Handle;

		private static readonly DataPlayable<TPayload> m_NullPlayable = new DataPlayable<TPayload>(PlayableHandle.Null);
	}
}
