using System;

namespace UnityEngine.UIElements
{
	public sealed class PointerCancelEvent : PointerEventBase<PointerCancelEvent>
	{
		static PointerCancelEvent()
		{
			EventBase<PointerCancelEvent>.SetCreateFunction(() => new PointerCancelEvent());
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

		public PointerCancelEvent()
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
			panel.dispatcher.m_ClickDetector.ProcessEvent<PointerCancelEvent>(this);
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
			bool flag2 = base.pointerType == PointerType.tracked;
			if (flag2)
			{
				PointerDeviceState.RemoveTrackedState(base.pointerId);
			}
			Panel panel2 = panel as Panel;
			bool flag3 = panel2 != null;
			if (flag3)
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
