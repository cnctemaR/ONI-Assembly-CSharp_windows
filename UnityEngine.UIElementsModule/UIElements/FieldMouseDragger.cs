using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class FieldMouseDragger<T> : BaseFieldMouseDragger
	{
		public FieldMouseDragger(IValueField<T> drivenField)
		{
			this.m_DrivenField = drivenField;
			this.m_DragElement = null;
			this.m_DragHotZone = new Rect(0f, 0f, -1f, -1f);
			this.dragging = false;
		}

		public bool dragging { get; set; }

		public T startValue { get; set; }

		public sealed override void SetDragZone(VisualElement dragElement, Rect hotZone)
		{
			bool flag = this.m_DragElement != null;
			if (flag)
			{
				this.m_DragElement.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.UpdateValueOnPointerDown), TrickleDown.NoTrickleDown);
				this.m_DragElement.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.UpdateValueOnPointerUp), TrickleDown.NoTrickleDown);
				this.m_DragElement.UnregisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.UpdateValueOnKeyDown), TrickleDown.NoTrickleDown);
			}
			this.m_DragElement = dragElement;
			this.m_DragHotZone = hotZone;
			bool flag2 = this.m_DragElement != null;
			if (flag2)
			{
				this.dragging = false;
				this.m_DragElement.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.UpdateValueOnPointerDown), TrickleDown.NoTrickleDown);
				this.m_DragElement.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.UpdateValueOnPointerUp), TrickleDown.NoTrickleDown);
				this.m_DragElement.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.UpdateValueOnKeyDown), TrickleDown.NoTrickleDown);
			}
		}

		private bool CanStartDrag(int button, Vector2 localPosition)
		{
			return button == 0 && (this.m_DragHotZone.width < 0f || this.m_DragHotZone.height < 0f || this.m_DragHotZone.Contains(this.m_DragElement.WorldToLocal(localPosition)));
		}

		private void UpdateValueOnPointerDown(PointerDownEvent evt)
		{
			bool flag = this.CanStartDrag(evt.button, evt.localPosition);
			if (flag)
			{
				bool flag2 = evt.pointerType == PointerType.mouse;
				if (flag2)
				{
					this.m_DragElement.CaptureMouse();
					this.ProcessDownEvent(evt);
				}
				else
				{
					bool flag3 = this.m_DragElement.panel.contextType == ContextType.Editor;
					if (flag3)
					{
						evt.PreventDefault();
						this.m_DragElement.CapturePointer(evt.pointerId);
						this.ProcessDownEvent(evt);
					}
				}
			}
		}

		private void ProcessDownEvent(EventBase evt)
		{
			evt.StopPropagation();
			this.dragging = true;
			this.m_DragElement.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.UpdateValueOnPointerMove), TrickleDown.NoTrickleDown);
			this.startValue = this.m_DrivenField.value;
			this.m_DrivenField.StartDragging();
			BaseVisualElementPanel baseVisualElementPanel = this.m_DragElement.panel as BaseVisualElementPanel;
			if (baseVisualElementPanel != null)
			{
				UIElementsBridge uiElementsBridge = baseVisualElementPanel.uiElementsBridge;
				if (uiElementsBridge != null)
				{
					uiElementsBridge.SetWantsMouseJumping(1);
				}
			}
		}

		private void UpdateValueOnPointerMove(PointerMoveEvent evt)
		{
			this.ProcessMoveEvent(evt.shiftKey, evt.altKey, evt.deltaPosition);
		}

		private void ProcessMoveEvent(bool shiftKey, bool altKey, Vector2 deltaPosition)
		{
			bool dragging = this.dragging;
			if (dragging)
			{
				DeltaSpeed deltaSpeed = (shiftKey ? DeltaSpeed.Fast : (altKey ? DeltaSpeed.Slow : DeltaSpeed.Normal));
				this.m_DrivenField.ApplyInputDeviceDelta(deltaPosition, deltaSpeed, this.startValue);
			}
		}

		private void UpdateValueOnPointerUp(PointerUpEvent evt)
		{
			this.ProcessUpEvent(evt, evt.pointerId);
		}

		private void ProcessUpEvent(EventBase evt, int pointerId)
		{
			bool dragging = this.dragging;
			if (dragging)
			{
				this.dragging = false;
				this.m_DragElement.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.UpdateValueOnPointerMove), TrickleDown.NoTrickleDown);
				this.m_DragElement.ReleasePointer(pointerId);
				bool flag = evt is IMouseEvent;
				if (flag)
				{
					this.m_DragElement.panel.ProcessPointerCapture(PointerId.mousePointerId);
				}
				BaseVisualElementPanel baseVisualElementPanel = this.m_DragElement.panel as BaseVisualElementPanel;
				if (baseVisualElementPanel != null)
				{
					UIElementsBridge uiElementsBridge = baseVisualElementPanel.uiElementsBridge;
					if (uiElementsBridge != null)
					{
						uiElementsBridge.SetWantsMouseJumping(0);
					}
				}
				this.m_DrivenField.StopDragging();
			}
		}

		private void UpdateValueOnKeyDown(KeyDownEvent evt)
		{
			bool flag = this.dragging && evt.keyCode == KeyCode.Escape;
			if (flag)
			{
				this.dragging = false;
				this.m_DrivenField.value = this.startValue;
				this.m_DrivenField.StopDragging();
				VisualElement visualElement = evt.target as VisualElement;
				IPanel panel = ((visualElement != null) ? visualElement.panel : null);
				panel.ReleasePointer(PointerId.mousePointerId);
				BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
				if (baseVisualElementPanel != null)
				{
					UIElementsBridge uiElementsBridge = baseVisualElementPanel.uiElementsBridge;
					if (uiElementsBridge != null)
					{
						uiElementsBridge.SetWantsMouseJumping(0);
					}
				}
			}
		}

		private readonly IValueField<T> m_DrivenField;

		private VisualElement m_DragElement;

		private Rect m_DragHotZone;
	}
}
