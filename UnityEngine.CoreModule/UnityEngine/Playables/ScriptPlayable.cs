using System;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>A IPlayable implementation that contains a PlayableBehaviour for the PlayableGraph. PlayableBehaviour can be used to write custom Playable that implement their own PrepareFrame callback.</para>
	/// </summary>
	public struct ScriptPlayable<T> : IPlayable, IEquatable<ScriptPlayable<T>> where T : class, IPlayableBehaviour, new()
	{
		internal ScriptPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!typeof(T).IsAssignableFrom(handle.GetPlayableType()))
				{
					throw new InvalidCastException(string.Format("Incompatible handle: Trying to assign a playable data of type `{0}` that is not compatible with the PlayableBehaviour of type `{1}`.", handle.GetPlayableType(), typeof(T)));
				}
			}
			this.m_Handle = handle;
		}

		public static ScriptPlayable<T> Null
		{
			get
			{
				return ScriptPlayable<T>.m_NullPlayable;
			}
		}

		public static ScriptPlayable<T> Create(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle playableHandle = ScriptPlayable<T>.CreateHandle(graph, (T)((object)null), inputCount);
			return new ScriptPlayable<T>(playableHandle);
		}

		public static ScriptPlayable<T> Create(PlayableGraph graph, T template, int inputCount = 0)
		{
			PlayableHandle playableHandle = ScriptPlayable<T>.CreateHandle(graph, template, inputCount);
			return new ScriptPlayable<T>(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, T template, int inputCount)
		{
			object obj;
			if (template == null)
			{
				obj = ScriptPlayable<T>.CreateScriptInstance();
			}
			else
			{
				obj = ScriptPlayable<T>.CloneScriptInstance(template);
			}
			PlayableHandle playableHandle;
			if (obj == null)
			{
				Debug.LogError("Could not create a ScriptPlayable of Type " + typeof(T).ToString());
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				PlayableHandle playableHandle2 = graph.CreatePlayableHandle();
				if (!playableHandle2.IsValid())
				{
					playableHandle = PlayableHandle.Null;
				}
				else
				{
					playableHandle2.SetInputCount(inputCount);
					playableHandle2.SetScriptInstance(obj);
					playableHandle = playableHandle2;
				}
			}
			return playableHandle;
		}

		private static object CreateScriptInstance()
		{
			IPlayableBehaviour playableBehaviour;
			if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
			{
				playableBehaviour = ScriptableObject.CreateInstance(typeof(T)) as T;
			}
			else
			{
				playableBehaviour = new T();
			}
			return playableBehaviour;
		}

		private static object CloneScriptInstance(IPlayableBehaviour source)
		{
			Object @object = source as Object;
			object obj;
			if (@object != null)
			{
				obj = ScriptPlayable<T>.CloneScriptInstanceFromEngineObject(@object);
			}
			else
			{
				ICloneable cloneable = source as ICloneable;
				if (cloneable != null)
				{
					obj = ScriptPlayable<T>.CloneScriptInstanceFromIClonable(cloneable);
				}
				else
				{
					obj = null;
				}
			}
			return obj;
		}

		private static object CloneScriptInstanceFromEngineObject(Object source)
		{
			Object @object = Object.Instantiate(source);
			if (@object != null)
			{
				@object.hideFlags |= HideFlags.DontSave;
			}
			return @object;
		}

		private static object CloneScriptInstanceFromIClonable(ICloneable source)
		{
			return source.Clone();
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public T GetBehaviour()
		{
			return this.m_Handle.GetObject<T>();
		}

		public static implicit operator Playable(ScriptPlayable<T> playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator ScriptPlayable<T>(Playable playable)
		{
			return new ScriptPlayable<T>(playable.GetHandle());
		}

		public bool Equals(ScriptPlayable<T> other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		private PlayableHandle m_Handle;

		private static readonly ScriptPlayable<T> m_NullPlayable = new ScriptPlayable<T>(PlayableHandle.Null);
	}
}
