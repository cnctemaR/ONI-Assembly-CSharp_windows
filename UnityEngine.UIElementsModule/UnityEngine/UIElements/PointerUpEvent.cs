using System;

namespace UnityEngine.UIElements
{
	public sealed class PointerUpEvent : PointerEventBase<PointerUpEvent>
	{
		static PointerUpEvent()
		{
			EventBase<PointerUpEvent>.SetCreateFunction(() => new PointerUpEvent());
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

		public PointerUpEvent()
		{
			this.LocalInit();
		}

		internal override IMouseEvent GetPooledCompatibilityMouseEvent()
		{
			return MouseUpEvent.GetPooled(this);
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			panel.dispatcher.m_ClickDetector.ProcessEvent<PointerUpEvent>(this);
			bool flag = PointerType.IsDirectManipulationDevice(base.pointerType);
			if (flag)
			{
				panel.ReleasePointer(base.pointerId);
				BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
				if (baseVisualElementPanel != null)
				{
					baseVisualElementPanel.ClearCachedElementUnderPointer(base.pointerId, this);
				}
			}
			Panel panel2 = panel as Panel;
			bool flag2 = panel2 != null;
			if (flag2)
			{
				ContextualMenuManager contextualMenuManager = panel2.contextualMenuManager;
				if (contextualMenuManager != null)
				{
					contextualMenuManager.AfterPointerUp();
				}
			}
			base.PostDispatch(panel);
			panel.ActivateCompatibilityMouseEvents(base.pointerId);
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, base.pointerId, base.position);
		}
	}
}
