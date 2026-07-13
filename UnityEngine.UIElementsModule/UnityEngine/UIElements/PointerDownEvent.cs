using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.PointerDown)]
	public sealed class PointerDownEvent : PointerEventBase<PointerDownEvent>
	{
		static PointerDownEvent()
		{
			EventBase<PointerDownEvent>.SetCreateFunction(() => new PointerDownEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles | EventBase.EventPropagation.TricklesDown | EventBase.EventPropagation.SkipDisabledElements;
			base.recomputeTopElementUnderPointer = true;
		}

		public PointerDownEvent()
		{
			this.LocalInit();
		}

		internal override IMouseEvent GetPooledCompatibilityMouseEvent()
		{
			return MouseDownEvent.GetPooled(this);
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			Panel panel2 = panel as Panel;
			bool flag = panel2 != null;
			if (flag)
			{
				ContextualMenuManager contextualMenuManager = panel2.contextualMenuManager;
				if (contextualMenuManager != null)
				{
					contextualMenuManager.BeforePointerDown();
				}
			}
			base.PreDispatch(panel);
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			panel.focusController.SwitchFocusOnEvent(panel.focusController.GetLeafFocusedElement(), this);
			panel.dispatcher.m_ClickDetector.ProcessEvent<PointerDownEvent>(this);
			base.PostDispatch(panel);
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, base.pointerId, base.position);
		}
	}
}
