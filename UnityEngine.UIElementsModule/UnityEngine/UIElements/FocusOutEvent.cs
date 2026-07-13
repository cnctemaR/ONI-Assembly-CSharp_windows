using System;

namespace UnityEngine.UIElements
{
	public class FocusOutEvent : FocusEventBase<FocusOutEvent>
	{
		static FocusOutEvent()
		{
			EventBase<FocusOutEvent>.SetCreateFunction(() => new FocusOutEvent());
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

		public FocusOutEvent()
		{
			this.LocalInit();
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			bool flag = base.relatedTarget == null;
			if (flag)
			{
				base.focusController.ProcessPendingFocusChange(null);
			}
			base.PostDispatch(panel);
		}
	}
}
