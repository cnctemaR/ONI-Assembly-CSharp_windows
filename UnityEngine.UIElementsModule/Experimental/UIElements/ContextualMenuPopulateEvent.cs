using System;

namespace UnityEngine.Experimental.UIElements
{
	public class ContextualMenuPopulateEvent : MouseEventBase<ContextualMenuPopulateEvent>
	{
		public ContextualMenuPopulateEvent()
		{
			this.Init();
		}

		public DropdownMenu menu { get; private set; }

		public EventBase triggerEvent { get; private set; }

		public static ContextualMenuPopulateEvent GetPooled(EventBase triggerEvent, DropdownMenu menu, IEventHandler target, ContextualMenuManager menuManager)
		{
			ContextualMenuPopulateEvent pooled = EventBase<ContextualMenuPopulateEvent>.GetPooled();
			if (triggerEvent != null)
			{
				triggerEvent.Acquire();
				pooled.triggerEvent = triggerEvent;
				IMouseEvent mouseEvent = triggerEvent as IMouseEvent;
				if (mouseEvent != null)
				{
					pooled.modifiers = mouseEvent.modifiers;
					pooled.mousePosition = mouseEvent.mousePosition;
					pooled.localMousePosition = mouseEvent.mousePosition;
					pooled.mouseDelta = mouseEvent.mouseDelta;
					pooled.button = mouseEvent.button;
					pooled.clickCount = mouseEvent.clickCount;
				}
				IMouseEventInternal mouseEventInternal = triggerEvent as IMouseEventInternal;
				if (mouseEventInternal != null)
				{
					((IMouseEventInternal)pooled).hasUnderlyingPhysicalEvent = mouseEventInternal.hasUnderlyingPhysicalEvent;
				}
			}
			pooled.target = target;
			pooled.menu = menu;
			pooled.m_ContextualMenuManager = menuManager;
			return pooled;
		}

		protected override void Init()
		{
			base.Init();
			this.menu = null;
			this.m_ContextualMenuManager = null;
			if (this.triggerEvent != null)
			{
				this.triggerEvent.Dispose();
				this.triggerEvent = null;
			}
		}

		protected internal override void PostDispatch()
		{
			if (!base.isDefaultPrevented && this.m_ContextualMenuManager != null)
			{
				this.menu.PrepareForDisplay(this.triggerEvent);
				this.m_ContextualMenuManager.DoDisplayMenu(this.menu, this.triggerEvent);
			}
		}

		private ContextualMenuManager m_ContextualMenuManager;
	}
}
