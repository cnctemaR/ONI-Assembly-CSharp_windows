using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.IMGUI)]
	public class IMGUIEvent : EventBase<IMGUIEvent>
	{
		static IMGUIEvent()
		{
			EventBase<IMGUIEvent>.SetCreateFunction(() => new IMGUIEvent());
		}

		public static IMGUIEvent GetPooled(Event systemEvent)
		{
			IMGUIEvent pooled = EventBase<IMGUIEvent>.GetPooled();
			pooled.imguiEvent = systemEvent;
			return pooled;
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.BubblesOrTricklesDown;
		}

		public IMGUIEvent()
		{
			this.LocalInit();
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToPanelRoot(this, panel);
		}
	}
}
