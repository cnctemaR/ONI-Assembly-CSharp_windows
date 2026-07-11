using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class EventBase<T> : EventBase where T : EventBase<T>, new()
	{
		protected EventBase()
		{
			this.m_RefCount = 0;
		}

		public static long TypeId()
		{
			return EventBase<T>.s_TypeId;
		}

		protected override void Init()
		{
			base.Init();
			if (this.m_RefCount != 0)
			{
				Debug.Log("Event improperly released.");
				this.m_RefCount = 0;
			}
		}

		public static T GetPooled()
		{
			T t = EventBase<T>.s_Pool.Get();
			t.Init();
			t.pooled = true;
			t.Acquire();
			return t;
		}

		private static void ReleasePooled(T evt)
		{
			if (evt.pooled)
			{
				evt.Init();
				EventBase<T>.s_Pool.Release(evt);
				evt.pooled = false;
			}
		}

		internal override void Acquire()
		{
			this.m_RefCount++;
		}

		public override void Dispose()
		{
			if (--this.m_RefCount == 0)
			{
				EventBase<T>.ReleasePooled((T)((object)this));
			}
		}

		public override long GetEventTypeId()
		{
			return EventBase<T>.s_TypeId;
		}

		private static readonly long s_TypeId = EventBase.RegisterEventType();

		private static readonly ObjectPool<T> s_Pool = new ObjectPool<T>(100);

		private int m_RefCount;
	}
}
