using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.ChangeValue)]
	public class ChangeEvent<T> : EventBase<ChangeEvent<T>>, IChangeEvent
	{
		static ChangeEvent()
		{
			EventBase<ChangeEvent<T>>.SetCreateFunction(() => new ChangeEvent<T>());
		}

		public T previousValue { get; protected set; }

		public T newValue { get; protected set; }

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.BubblesOrTricklesDown;
			this.previousValue = default(T);
			this.newValue = default(T);
		}

		public static ChangeEvent<T> GetPooled(T previousValue, T newValue)
		{
			ChangeEvent<T> pooled = EventBase<ChangeEvent<T>>.GetPooled();
			pooled.previousValue = previousValue;
			pooled.newValue = newValue;
			return pooled;
		}

		public ChangeEvent()
		{
			this.LocalInit();
		}
	}
}
