using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class PanelChangedEventBase<T> : EventBase<T>, IPanelChangedEvent, IPropagatableEvent where T : PanelChangedEventBase<T>, new()
	{
		protected PanelChangedEventBase()
		{
			this.Init();
		}

		public IPanel originPanel { get; private set; }

		public IPanel destinationPanel { get; private set; }

		protected override void Init()
		{
			base.Init();
			this.originPanel = null;
			this.destinationPanel = null;
		}

		public static T GetPooled(IPanel originPanel, IPanel destinationPanel)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.originPanel = originPanel;
			pooled.destinationPanel = destinationPanel;
			return pooled;
		}
	}
}
