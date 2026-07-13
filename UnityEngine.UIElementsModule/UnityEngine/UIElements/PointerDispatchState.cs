using System;

namespace UnityEngine.UIElements
{
	internal class PointerDispatchState
	{
		public PointerDispatchState()
		{
			this.Reset();
		}

		internal void Reset()
		{
			for (int i = 0; i < this.m_PointerCapture.Length; i++)
			{
				this.m_PendingPointerCapture[i] = null;
				this.m_PointerCapture[i] = null;
				this.m_ShouldSendCompatibilityMouseEvents[i] = true;
			}
		}

		public IEventHandler GetCapturingElement(int pointerId)
		{
			return this.m_PendingPointerCapture[pointerId];
		}

		public bool HasPointerCapture(IEventHandler handler, int pointerId)
		{
			return this.m_PendingPointerCapture[pointerId] == handler;
		}

		public void CapturePointer(IEventHandler handler, int pointerId)
		{
			IEventHandler eventHandler = this.m_PendingPointerCapture[pointerId];
			bool flag = eventHandler == handler;
			if (!flag)
			{
				bool flag2 = pointerId == PointerId.mousePointerId && GUIUtility.hotControl != 0;
				if (flag2)
				{
					GUIUtility.hotControl = 0;
				}
				this.m_PendingPointerCapture[pointerId] = handler;
				VisualElement visualElement = eventHandler as VisualElement;
				if (visualElement != null)
				{
					visualElement.UpdatePointerCaptureFlag();
				}
				VisualElement visualElement2 = handler as VisualElement;
				if (visualElement2 != null)
				{
					visualElement2.UpdatePointerCaptureFlag();
				}
			}
		}

		public void ReleasePointer(int pointerId)
		{
			IEventHandler eventHandler = this.m_PendingPointerCapture[pointerId];
			bool flag = eventHandler == null;
			if (!flag)
			{
				this.m_PendingPointerCapture[pointerId] = null;
				VisualElement visualElement = eventHandler as VisualElement;
				if (visualElement != null)
				{
					visualElement.UpdatePointerCaptureFlag();
				}
			}
		}

		public void ReleasePointer(IEventHandler handler, int pointerId)
		{
			bool flag = handler == this.m_PendingPointerCapture[pointerId];
			if (flag)
			{
				this.ReleasePointer(pointerId);
			}
		}

		public void ProcessPointerCapture(int pointerId)
		{
			IEventHandler eventHandler = this.m_PointerCapture[pointerId];
			bool flag = eventHandler == this.m_PendingPointerCapture[pointerId];
			if (!flag)
			{
				bool flag2 = eventHandler != null;
				if (flag2)
				{
					using (PointerCaptureOutEvent pooled = PointerCaptureEventBase<PointerCaptureOutEvent>.GetPooled(eventHandler, this.m_PendingPointerCapture[pointerId], pointerId))
					{
						eventHandler.SendEvent(pooled);
					}
					bool flag3 = pointerId == PointerId.mousePointerId && this.m_PointerCapture[pointerId] == eventHandler;
					if (flag3)
					{
						using (MouseCaptureOutEvent pooled2 = PointerCaptureEventBase<MouseCaptureOutEvent>.GetPooled(eventHandler, this.m_PendingPointerCapture[pointerId], pointerId))
						{
							eventHandler.SendEvent(pooled2);
						}
					}
				}
				IEventHandler eventHandler2 = this.m_PendingPointerCapture[pointerId];
				bool flag4 = eventHandler2 != null;
				if (flag4)
				{
					using (PointerCaptureEvent pooled3 = PointerCaptureEventBase<PointerCaptureEvent>.GetPooled(eventHandler2, this.m_PointerCapture[pointerId], pointerId))
					{
						eventHandler2.SendEvent(pooled3);
					}
					bool flag5 = pointerId == PointerId.mousePointerId && this.m_PendingPointerCapture[pointerId] == eventHandler2;
					if (flag5)
					{
						using (MouseCaptureEvent pooled4 = PointerCaptureEventBase<MouseCaptureEvent>.GetPooled(eventHandler2, this.m_PointerCapture[pointerId], pointerId))
						{
							eventHandler2.SendEvent(pooled4);
						}
					}
				}
				this.m_PointerCapture[pointerId] = this.m_PendingPointerCapture[pointerId];
			}
		}

		public void ActivateCompatibilityMouseEvents(int pointerId)
		{
			this.m_ShouldSendCompatibilityMouseEvents[pointerId] = true;
		}

		public void PreventCompatibilityMouseEvents(int pointerId)
		{
			this.m_ShouldSendCompatibilityMouseEvents[pointerId] = false;
		}

		public bool ShouldSendCompatibilityMouseEvents(IPointerEvent evt)
		{
			return evt.isPrimary && this.m_ShouldSendCompatibilityMouseEvents[evt.pointerId];
		}

		private IEventHandler[] m_PendingPointerCapture = new IEventHandler[PointerId.maxPointers];

		private IEventHandler[] m_PointerCapture = new IEventHandler[PointerId.maxPointers];

		private bool[] m_ShouldSendCompatibilityMouseEvents = new bool[PointerId.maxPointers];
	}
}
