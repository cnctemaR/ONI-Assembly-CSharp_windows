using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeave)]
	public sealed class PointerLeaveEvent : PointerEventBase<PointerLeaveEvent>
	{
		static PointerLeaveEvent()
		{
			EventBase<PointerLeaveEvent>.SetCreateFunction(() => new PointerLeaveEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.TricklesDown;
		}

		public PointerLeaveEvent()
		{
			this.LocalInit();
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToAssignedTarget(this, panel);
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
			base.elementTarget.containedPointerIds &= ~(1 << base.pointerId);
			base.elementTarget.UpdateHoverPseudoState();
		}
	}
}
