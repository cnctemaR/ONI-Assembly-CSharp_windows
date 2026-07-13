using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class ClickDetector
	{
		internal static int s_DoubleClickTime { get; set; } = -1;

		public ClickDetector()
		{
			this.m_ClickStatus = new List<ClickDetector.ButtonClickStatus>(PointerId.maxPointers);
			for (int i = 0; i < PointerId.maxPointers; i++)
			{
				this.m_ClickStatus.Add(new ClickDetector.ButtonClickStatus());
			}
			bool flag = ClickDetector.s_DoubleClickTime == -1;
			if (flag)
			{
				ClickDetector.s_DoubleClickTime = Event.GetDoubleClickTime();
			}
		}

		private void StartClickTracking(EventBase evt)
		{
			IPointerEvent pointerEvent = evt as IPointerEvent;
			bool flag = pointerEvent == null;
			if (!flag)
			{
				ClickDetector.ButtonClickStatus buttonClickStatus = this.m_ClickStatus[pointerEvent.pointerId];
				VisualElement elementTarget = evt.elementTarget;
				bool flag2 = elementTarget != buttonClickStatus.m_Target;
				if (flag2)
				{
					buttonClickStatus.Reset();
				}
				buttonClickStatus.m_Target = elementTarget;
				bool flag3 = evt.timestamp - buttonClickStatus.m_LastPointerDownTime > (long)ClickDetector.s_DoubleClickTime;
				if (flag3)
				{
					buttonClickStatus.m_ClickCount = 1;
				}
				else
				{
					buttonClickStatus.m_ClickCount++;
				}
				buttonClickStatus.m_LastPointerDownTime = evt.timestamp;
				buttonClickStatus.m_PointerDownPosition = pointerEvent.position;
			}
		}

		private void SendClickEvent(EventBase evt)
		{
			IPointerEvent pointerEvent = evt as IPointerEvent;
			bool flag = pointerEvent == null;
			if (!flag)
			{
				ClickDetector.ButtonClickStatus buttonClickStatus = this.m_ClickStatus[pointerEvent.pointerId];
				VisualElement elementTarget = evt.elementTarget;
				bool flag2 = elementTarget != null && ClickDetector.ContainsPointer(elementTarget, pointerEvent);
				if (flag2)
				{
					bool flag3 = buttonClickStatus.m_Target != null && buttonClickStatus.m_ClickCount > 0;
					if (flag3)
					{
						VisualElement visualElement = buttonClickStatus.m_Target.FindCommonAncestor(evt.elementTarget);
						bool flag4 = visualElement != null;
						if (flag4)
						{
							using (ClickEvent pooled = ClickEvent.GetPooled(pointerEvent, buttonClickStatus.m_ClickCount))
							{
								pooled.elementTarget = visualElement;
								visualElement.SendEvent(pooled, DispatchMode.Immediate);
							}
						}
					}
				}
			}
		}

		private void CancelClickTracking(EventBase evt)
		{
			IPointerEvent pointerEvent = evt as IPointerEvent;
			bool flag = pointerEvent == null;
			if (!flag)
			{
				ClickDetector.ButtonClickStatus buttonClickStatus = this.m_ClickStatus[pointerEvent.pointerId];
				buttonClickStatus.Reset();
			}
		}

		public void ProcessEvent<TEvent>(PointerEventBase<TEvent> evt) where TEvent : PointerEventBase<TEvent>, new()
		{
			bool flag = evt.eventTypeId == EventBase<PointerDownEvent>.TypeId() && evt.button == 0;
			if (flag)
			{
				this.StartClickTracking(evt);
			}
			else
			{
				bool flag2 = evt.eventTypeId == EventBase<PointerMoveEvent>.TypeId();
				if (flag2)
				{
					bool flag3 = evt.button == 0 && (evt.pressedButtons & 1) == 1;
					if (flag3)
					{
						this.StartClickTracking(evt);
					}
					else
					{
						bool flag4 = evt.button == 0 && (evt.pressedButtons & 1) == 0;
						if (flag4)
						{
							this.SendClickEvent(evt);
						}
						else
						{
							ClickDetector.ButtonClickStatus buttonClickStatus = this.m_ClickStatus[evt.pointerId];
							bool flag5 = buttonClickStatus.m_Target != null;
							if (flag5)
							{
								buttonClickStatus.m_LastPointerDownTime = 0L;
							}
						}
					}
				}
				else
				{
					bool flag6 = evt.eventTypeId == EventBase<PointerCancelEvent>.TypeId();
					if (flag6)
					{
						this.CancelClickTracking(evt);
					}
					else
					{
						bool flag7 = evt.eventTypeId == EventBase<PointerUpEvent>.TypeId() && evt.button == 0;
						if (flag7)
						{
							this.SendClickEvent(evt);
						}
					}
				}
			}
		}

		private static bool ContainsPointer(VisualElement element, IPointerEvent pe)
		{
			bool flag = !element.worldBound.Contains(pe.position) || element.panel == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool isFlat = element.elementPanel.isFlat;
				if (isFlat)
				{
					VisualElement topElementUnderPointer = element.elementPanel.GetTopElementUnderPointer(pe.pointerId);
					flag2 = element == topElementUnderPointer || element.Contains(topElementUnderPointer);
				}
				else
				{
					flag2 = true;
				}
			}
			return flag2;
		}

		internal void Cleanup(List<VisualElement> elements)
		{
			foreach (ClickDetector.ButtonClickStatus buttonClickStatus in this.m_ClickStatus)
			{
				bool flag = buttonClickStatus.m_Target == null;
				if (!flag)
				{
					bool flag2 = elements.Contains(buttonClickStatus.m_Target);
					if (flag2)
					{
						buttonClickStatus.Reset();
					}
				}
			}
		}

		private List<ClickDetector.ButtonClickStatus> m_ClickStatus;

		private class ButtonClickStatus
		{
			public void Reset()
			{
				this.m_Target = null;
				this.m_ClickCount = 0;
				this.m_LastPointerDownTime = 0L;
				this.m_PointerDownPosition = Vector3.zero;
			}

			public VisualElement m_Target;

			public Vector3 m_PointerDownPosition;

			public long m_LastPointerDownTime;

			public int m_ClickCount;
		}
	}
}
