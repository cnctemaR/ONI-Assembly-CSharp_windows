using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.PointerMove)]
	public sealed class PointerMoveEvent : PointerEventBase<PointerMoveEvent>
	{
		static PointerMoveEvent()
		{
			EventBase<PointerMoveEvent>.SetCreateFunction(() => new PointerMoveEvent());
		}

		internal bool isHandledByDraggable { get; set; }

		internal bool isPointerUpDown
		{
			get
			{
				return base.button >= 0;
			}
		}

		internal bool isPointerDown
		{
			get
			{
				return base.button >= 0 && (base.pressedButtons & (1 << base.button)) != 0;
			}
		}

		internal bool isPointerUp
		{
			get
			{
				return base.button >= 0 && (base.pressedButtons & (1 << base.button)) == 0;
			}
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
			this.isHandledByDraggable = false;
		}

		public PointerMoveEvent()
		{
			this.LocalInit();
		}

		internal override IMouseEvent GetPooledCompatibilityMouseEvent()
		{
			bool flag = base.imguiEvent != null && base.imguiEvent.rawType == EventType.MouseDown;
			IMouseEvent mouseEvent;
			if (flag)
			{
				mouseEvent = MouseDownEvent.GetPooled(this);
			}
			else
			{
				bool flag2 = base.imguiEvent != null && base.imguiEvent.rawType == EventType.MouseUp;
				if (flag2)
				{
					mouseEvent = MouseUpEvent.GetPooled(this);
				}
				else
				{
					mouseEvent = MouseMoveEvent.GetPooled(this);
				}
			}
			return mouseEvent;
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			panel.dispatcher.m_ClickDetector.ProcessEvent<PointerMoveEvent>(this);
			base.PostDispatch(panel);
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, base.pointerId, base.position);
		}
	}
}
