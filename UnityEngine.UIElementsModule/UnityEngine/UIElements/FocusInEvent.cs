using System;

namespace UnityEngine.UIElements
{
	public class FocusInEvent : FocusEventBase<FocusInEvent>
	{
		static FocusInEvent()
		{
			EventBase<FocusInEvent>.SetCreateFunction(() => new FocusInEvent());
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

		public FocusInEvent()
		{
			this.LocalInit();
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			base.focusController.ProcessPendingFocusChange(base.elementTarget);
			base.PostDispatch(panel);
		}
	}
}
