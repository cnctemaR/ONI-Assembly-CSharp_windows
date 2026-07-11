using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Generic base class for events, implementing event pooling and automatic registration to the event type system.</para>
	/// </summary>
	public abstract class EventBase<T> : EventBase where T : EventBase<T>, new()
	{
		public static long TypeId()
		{
			return EventBase<T>.s_TypeId;
		}

		public static T GetPooled()
		{
			T t = EventBase<T>.s_Pool.Get();
			t.Init();
			t.flags |= EventBase.EventFlags.Pooled;
			return t;
		}

		protected static void ReleasePooled(T evt)
		{
			if ((evt.flags & EventBase.EventFlags.Pooled) == EventBase.EventFlags.Pooled)
			{
				evt.Init();
				EventBase<T>.s_Pool.Release(evt);
				evt.flags &= ~EventBase.EventFlags.Pooled;
			}
		}

		public override void Dispose()
		{
			EventBase<T>.ReleasePooled((T)((object)this));
		}

		public override long GetEventTypeId()
		{
			return EventBase<T>.s_TypeId;
		}

		private static readonly long s_TypeId = EventBase.RegisterEventType();

		private static readonly ObjectPool<T> s_Pool = new ObjectPool<T>(100);
	}
}
