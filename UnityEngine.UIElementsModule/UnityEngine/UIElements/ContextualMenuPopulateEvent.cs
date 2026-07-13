using System;

namespace UnityEngine.UIElements
{
	public class ContextualMenuPopulateEvent : MouseEventBase<ContextualMenuPopulateEvent>
	{
		static ContextualMenuPopulateEvent()
		{
			EventBase<ContextualMenuPopulateEvent>.SetCreateFunction(() => new ContextualMenuPopulateEvent());
		}

		public DropdownMenu menu { get; private set; }

		public EventBase triggerEvent { get; private set; }

		public static ContextualMenuPopulateEvent GetPooled(EventBase triggerEvent, DropdownMenu menu, IEventHandler target, ContextualMenuManager menuManager)
		{
			ContextualMenuPopulateEvent pooled = EventBase<ContextualMenuPopulateEvent>.GetPooled(triggerEvent);
			bool flag = triggerEvent != null;
			if (flag)
			{
				triggerEvent.Acquire();
				pooled.triggerEvent = triggerEvent;
				IMouseEvent mouseEvent = triggerEvent as IMouseEvent;
				bool flag2 = mouseEvent != null;
				if (flag2)
				{
					pooled.modifiers = mouseEvent.modifiers;
					pooled.mousePosition = mouseEvent.mousePosition;
					pooled.localMousePosition = mouseEvent.mousePosition;
					pooled.mouseDelta = mouseEvent.mouseDelta;
					pooled.button = mouseEvent.button;
					pooled.clickCount = mouseEvent.clickCount;
				}
				else
				{
					IPointerEvent pointerEvent = triggerEvent as IPointerEvent;
					bool flag3 = pointerEvent != null;
					if (flag3)
					{
						pooled.modifiers = pointerEvent.modifiers;
						pooled.mousePosition = pointerEvent.position;
						pooled.localMousePosition = pointerEvent.position;
						pooled.mouseDelta = pointerEvent.deltaPosition;
						pooled.button = pointerEvent.button;
						pooled.clickCount = pointerEvent.clickCount;
					}
				}
			}
			pooled.elementTarget = (VisualElement)target;
			pooled.menu = menu;
			pooled.m_ContextualMenuManager = menuManager;
			return pooled;
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			this.menu = null;
			this.m_ContextualMenuManager = null;
			bool flag = this.triggerEvent != null;
			if (flag)
			{
				this.triggerEvent.Dispose();
				this.triggerEvent = null;
			}
		}

		public ContextualMenuPopulateEvent()
		{
			this.LocalInit();
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			bool flag = this.menu.Count > 0 && this.m_ContextualMenuManager != null;
			if (flag)
			{
				this.menu.PrepareForDisplay(this.triggerEvent);
				this.m_ContextualMenuManager.DoDisplayMenu(this.menu, this.triggerEvent);
			}
			base.PostDispatch(panel);
		}

		private ContextualMenuManager m_ContextualMenuManager;
	}
}
