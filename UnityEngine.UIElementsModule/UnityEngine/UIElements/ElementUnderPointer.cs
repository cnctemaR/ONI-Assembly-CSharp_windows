using System;

namespace UnityEngine.UIElements
{
	internal class ElementUnderPointer
	{
		public ElementUnderPointer()
		{
			this.m_IsPrimaryPointer = new bool[PointerId.maxPointers];
			this.m_IsPrimaryPointer[PointerId.mousePointerId] = true;
			this.m_IsPrimaryPointer[PointerId.touchPointerIdBase] = true;
			for (int i = 0; i < PointerId.penPointerCount; i++)
			{
				this.m_IsPrimaryPointer[PointerId.penPointerIdBase + i] = true;
			}
		}

		internal VisualElement GetTopElementUnderPointer(int pointerId, out Vector2 pickPosition, out bool isTemporary)
		{
			pickPosition = this.m_PickingPointerPositions[pointerId];
			isTemporary = this.m_IsPickingPointerTemporaries[pointerId];
			return this.m_PendingTopElementUnderPointer[pointerId];
		}

		internal VisualElement GetTopElementUnderPointer(int pointerId)
		{
			return this.m_PendingTopElementUnderPointer[pointerId];
		}

		internal void RemoveElementUnderPointer(VisualElement elementToRemove)
		{
			for (int i = 0; i < this.m_TopElementUnderPointer.Length; i++)
			{
				VisualElement visualElement = this.m_TopElementUnderPointer[i];
				bool flag = visualElement == elementToRemove;
				if (flag)
				{
					this.SetElementUnderPointer(null, i, null);
				}
			}
		}

		internal void SetElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, Vector2 pointerPos)
		{
			Debug.Assert(pointerId >= 0, "SetElementUnderPointer expects pointerId >= 0");
			VisualElement visualElement = this.m_TopElementUnderPointer[pointerId];
			this.m_IsPickingPointerTemporaries[pointerId] = false;
			this.m_PickingPointerPositions[pointerId] = pointerPos;
			bool flag = visualElement == newElementUnderPointer;
			if (!flag)
			{
				this.m_PendingTopElementUnderPointer[pointerId] = newElementUnderPointer;
				this.m_TriggerEvent[pointerId] = null;
			}
		}

		private Vector2 GetEventPointerPosition(EventBase triggerEvent)
		{
			IPointerEvent pointerEvent = triggerEvent as IPointerEvent;
			bool flag = pointerEvent != null;
			Vector2 vector;
			if (flag)
			{
				vector = new Vector2(pointerEvent.position.x, pointerEvent.position.y);
			}
			else
			{
				IMouseEvent mouseEvent = triggerEvent as IMouseEvent;
				bool flag2 = mouseEvent != null;
				if (flag2)
				{
					vector = mouseEvent.mousePosition;
				}
				else
				{
					vector = new Vector2(float.MinValue, float.MinValue);
				}
			}
			return vector;
		}

		internal void SetTemporaryElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, EventBase triggerEvent)
		{
			this.SetElementUnderPointer(newElementUnderPointer, pointerId, triggerEvent, true);
		}

		internal void SetElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, EventBase triggerEvent)
		{
			this.SetElementUnderPointer(newElementUnderPointer, pointerId, triggerEvent, false);
		}

		private void SetElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, EventBase triggerEvent, bool temporary)
		{
			Debug.Assert(pointerId >= 0, "SetElementUnderPointer expects pointerId >= 0");
			this.m_IsPickingPointerTemporaries[pointerId] = temporary;
			this.m_PickingPointerPositions[pointerId] = this.GetEventPointerPosition(triggerEvent);
			this.m_PendingTopElementUnderPointer[pointerId] = newElementUnderPointer;
			VisualElement visualElement = this.m_TopElementUnderPointer[pointerId];
			bool flag = visualElement == newElementUnderPointer;
			if (!flag)
			{
				IPointerOrMouseEvent pointerOrMouseEvent;
				bool flag2;
				if (this.m_TriggerEvent[pointerId] == null)
				{
					pointerOrMouseEvent = triggerEvent as IPointerOrMouseEvent;
					flag2 = pointerOrMouseEvent != null;
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					this.m_TriggerEvent[pointerId] = pointerOrMouseEvent;
					bool[] isPrimaryPointer = this.m_IsPrimaryPointer;
					IPointerEvent pointerEvent = pointerOrMouseEvent as IPointerEvent;
					isPrimaryPointer[pointerId] = pointerEvent == null || pointerEvent.isPrimary;
				}
			}
		}

		internal bool CommitElementUnderPointers(EventDispatcher dispatcher, ContextType contextType)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < PointerId.maxPointers; i++)
			{
				IPointerOrMouseEvent pointerOrMouseEvent = this.m_TriggerEvent[i];
				VisualElement visualElement = this.m_TopElementUnderPointer[i];
				VisualElement visualElement2 = this.m_PendingTopElementUnderPointer[i];
				bool flag3 = visualElement == visualElement2;
				if (flag3)
				{
					bool flag4 = pointerOrMouseEvent != null;
					if (flag4)
					{
						this.m_PickingPointerPositions[i] = pointerOrMouseEvent.position;
					}
				}
				else
				{
					flag = true;
					this.m_TopElementUnderPointer[i] = visualElement2;
					Vector2 vector = ((pointerOrMouseEvent != null) ? pointerOrMouseEvent.position : PointerDeviceState.GetPointerPosition(i, contextType));
					this.m_PickingPointerPositions[i] = vector;
					using (new EventDispatcherGate(dispatcher))
					{
						IPointerEvent pointerEvent = pointerOrMouseEvent as IPointerEvent;
						PointerEventsHelper.SendOverOut(visualElement, visualElement2, pointerEvent, vector, i);
						PointerEventsHelper.SendEnterLeave<PointerLeaveEvent, PointerEnterEvent>(visualElement, visualElement2, null, vector, i);
						IMouseEvent mouseEvent;
						if ((mouseEvent = pointerOrMouseEvent as IMouseEvent) == null)
						{
							IPointerEventInternal pointerEventInternal = pointerOrMouseEvent as IPointerEventInternal;
							mouseEvent = ((pointerEventInternal != null) ? pointerEventInternal.compatibilityMouseEvent : null);
						}
						IMouseEvent mouseEvent2 = mouseEvent;
						bool flag5 = (mouseEvent2 != null || this.m_IsPrimaryPointer[i]) && !flag2;
						if (flag5)
						{
							flag2 = true;
							MouseEventsHelper.SendMouseOverMouseOut(visualElement, visualElement2, mouseEvent2, vector);
							MouseEventsHelper.SendEnterLeave<MouseLeaveEvent, MouseEnterEvent>(visualElement, visualElement2, mouseEvent2, vector);
						}
					}
					this.m_TriggerEvent[i] = null;
				}
			}
			return flag;
		}

		private VisualElement[] m_PendingTopElementUnderPointer = new VisualElement[PointerId.maxPointers];

		private VisualElement[] m_TopElementUnderPointer = new VisualElement[PointerId.maxPointers];

		private IPointerOrMouseEvent[] m_TriggerEvent = new IPointerOrMouseEvent[PointerId.maxPointers];

		private Vector2[] m_PickingPointerPositions = new Vector2[PointerId.maxPointers];

		private readonly bool[] m_IsPrimaryPointer;

		private bool[] m_IsPickingPointerTemporaries = new bool[PointerId.maxPointers];
	}
}
