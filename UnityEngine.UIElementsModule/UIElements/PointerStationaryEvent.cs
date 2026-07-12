using System;

namespace UnityEngine.UIElements
{
	public sealed class PointerStationaryEvent : PointerEventBase<PointerStationaryEvent>
	{
		static PointerStationaryEvent()
		{
			EventBase<PointerStationaryEvent>.SetCreateFunction(() => new PointerStationaryEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles | EventBase.EventPropagation.TricklesDown | EventBase.EventPropagation.Cancellable;
			((IPointerEventInternal)this).triggeredByOS = true;
			((IPointerEventInternal)this).recomputeTopElementUnderPointer = true;
		}

		public PointerStationaryEvent()
		{
			this.LocalInit();
		}
	}
}
