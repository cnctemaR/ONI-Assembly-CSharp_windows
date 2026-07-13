using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.Default)]
	public abstract class EventBase<T> : EventBase where T : EventBase<T>, new()
	{
		protected internal static void SetCreateFunction(Func<T> createMethod)
		{
			EventBase<T>.s_Pool.CreateFunc = createMethod;
		}

		protected EventBase()
			: base(EventBase<T>.EventCategory)
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
			bool flag = this.m_RefCount != 0;
			if (flag)
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

		internal static T GetPooled(EventBase e)
		{
			T pooled = EventBase<T>.GetPooled();
			bool flag = e != null;
			if (flag)
			{
				pooled.SetTriggerEventId(e.eventId);
			}
			return pooled;
		}

		private static void ReleasePooled(T evt)
		{
			bool pooled = evt.pooled;
			if (pooled)
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

		public sealed override void Dispose()
		{
			int num = this.m_RefCount - 1;
			this.m_RefCount = num;
			bool flag = num == 0;
			if (flag)
			{
				EventBase<T>.ReleasePooled((T)((object)this));
			}
		}

		public override long eventTypeId
		{
			get
			{
				return EventBase<T>.s_TypeId;
			}
		}

		private static readonly long s_TypeId = EventBase.RegisterEventType();

		private static readonly ObjectPool<T> s_Pool = new ObjectPool<T>(() => new T(), 100);

		private int m_RefCount;

		internal static readonly EventCategory EventCategory = EventInterestReflectionUtils.GetEventCategory(typeof(T));
	}
}
