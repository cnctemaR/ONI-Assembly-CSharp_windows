using System;

namespace UnityEngine.UIElements
{
	[Obsolete("Not sent by input backend.")]
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
			base.propagation = EventBase.EventPropagation.BubblesOrTricklesDown;
			base.recomputeTopElementUnderPointer = true;
		}

		public PointerStationaryEvent()
		{
			this.LocalInit();
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, base.pointerId, base.position);
		}
	}
}
