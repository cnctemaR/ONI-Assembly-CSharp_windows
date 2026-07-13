using System;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements
{
	internal abstract class DragEventsProcessor
	{
		internal bool isRegistered
		{
			get
			{
				return this.m_IsRegistered;
			}
		}

		internal DragEventsProcessor.DragState dragState
		{
			get
			{
				return this.m_DragState;
			}
		}

		protected virtual bool supportsDragEvents
		{
			get
			{
				return true;
			}
		}

		private bool useDragEvents
		{
			get
			{
				return this.isEditorContext && this.supportsDragEvents;
			}
		}

		protected IDragAndDrop dragAndDrop
		{
			get
			{
				return DragAndDropUtility.GetDragAndDrop(this.m_Target.panel);
			}
		}

		internal virtual bool isEditorContext
		{
			get
			{
				Assert.IsNotNull<VisualElement>(this.m_Target);
				Assert.IsNotNull<VisualElement>(this.m_Target.parent);
				return this.m_Target.panel.contextType == ContextType.Editor;
			}
		}

		internal DragEventsProcessor(VisualElement target)
		{
			this.m_Target = target;
			this.m_Target.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.RegisterCallbacksFromTarget), TrickleDown.NoTrickleDown);
			this.m_Target.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.UnregisterCallbacksFromTarget), TrickleDown.NoTrickleDown);
			this.RegisterCallbacksFromTarget();
		}

		private void RegisterCallbacksFromTarget(AttachToPanelEvent evt)
		{
			this.RegisterCallbacksFromTarget();
		}

		private void RegisterCallbacksFromTarget()
		{
			bool isRegistered = this.m_IsRegistered;
			if (!isRegistered)
			{
				this.m_IsRegistered = true;
				this.m_Target.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDownEvent), TrickleDown.NoTrickleDown);
				this.m_Target.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUpEvent), TrickleDown.TrickleDown);
				this.m_Target.RegisterCallback<PointerLeaveEvent>(new EventCallback<PointerLeaveEvent>(this.OnPointerLeaveEvent), TrickleDown.NoTrickleDown);
				this.m_Target.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMoveEvent), TrickleDown.NoTrickleDown);
				this.m_Target.RegisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancelEvent), TrickleDown.NoTrickleDown);
				this.m_Target.RegisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCapturedOut), TrickleDown.NoTrickleDown);
				this.m_Target.RegisterCallback<PointerOutEvent>(new EventCallback<PointerOutEvent>(this.OnPointerOutEvent), TrickleDown.NoTrickleDown);
				this.m_Target.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			}
		}

		private void UnregisterCallbacksFromTarget(DetachFromPanelEvent evt)
		{
			this.UnregisterCallbacksFromTarget(false);
		}

		internal void UnregisterCallbacksFromTarget(bool unregisterPanelEvents = false)
		{
			this.m_IsRegistered = false;
			this.m_Target.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDownEvent), TrickleDown.NoTrickleDown);
			this.m_Target.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUpEvent), TrickleDown.TrickleDown);
			this.m_Target.UnregisterCallback<PointerLeaveEvent>(new EventCallback<PointerLeaveEvent>(this.OnPointerLeaveEvent), TrickleDown.NoTrickleDown);
			this.m_Target.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMoveEvent), TrickleDown.NoTrickleDown);
			this.m_Target.UnregisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancelEvent), TrickleDown.NoTrickleDown);
			this.m_Target.UnregisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCapturedOut), TrickleDown.NoTrickleDown);
			this.m_Target.UnregisterCallback<PointerOutEvent>(new EventCallback<PointerOutEvent>(this.OnPointerOutEvent), TrickleDown.NoTrickleDown);
			this.m_Target.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			if (unregisterPanelEvents)
			{
				this.m_Target.UnregisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.RegisterCallbacksFromTarget), TrickleDown.NoTrickleDown);
				this.m_Target.UnregisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.UnregisterCallbacksFromTarget), TrickleDown.NoTrickleDown);
			}
		}

		protected abstract bool CanStartDrag(Vector3 pointerPosition, EventModifiers modifiers);

		protected internal abstract StartDragArgs StartDrag(Vector3 pointerPosition, EventModifiers modifiers);

		protected internal abstract void UpdateDrag(Vector3 pointerPosition, EventModifiers modifiers);

		protected internal abstract void OnDrop(Vector3 pointerPosition, EventModifiers modifiers);

		protected abstract void ClearDragAndDropUI(bool dragCancelled);

		private void OnPointerDownEvent(PointerDownEvent evt)
		{
			bool flag = evt.button != 0;
			if (flag)
			{
				this.m_DragState = DragEventsProcessor.DragState.None;
			}
			else
			{
				bool flag2 = this.CanStartDrag(evt.position, evt.modifiers);
				if (flag2)
				{
					this.m_DragState = DragEventsProcessor.DragState.CanStartDrag;
					this.m_Start = evt.position;
				}
			}
		}

		private void OnPointerOutEvent(PointerOutEvent evt)
		{
			bool flag = this.m_DragState == DragEventsProcessor.DragState.CanStartDrag;
			if (flag)
			{
				bool flag2 = (this.m_Start - evt.position).sqrMagnitude <= 0f;
				if (!flag2)
				{
					this.m_PendingPerformDrag = true;
					evt.StopPropagation();
				}
			}
		}

		internal void OnPointerUpEvent(PointerUpEvent evt)
		{
			bool flag = !this.useDragEvents && this.m_DragState == DragEventsProcessor.DragState.Dragging;
			if (flag)
			{
				DragEventsProcessor dragEventsProcessor = this.GetDropTarget(evt.position) ?? this;
				dragEventsProcessor.UpdateDrag(evt.position, evt.modifiers);
				dragEventsProcessor.OnDrop(evt.position, evt.modifiers);
				dragEventsProcessor.ClearDragAndDropUI(false);
				evt.StopPropagation();
			}
			this.m_Target.ReleasePointer(evt.pointerId);
			this.ClearDragAndDropUI(this.m_DragState == DragEventsProcessor.DragState.Dragging);
			this.dragAndDrop.DragCleanup();
			this.m_DragState = DragEventsProcessor.DragState.None;
			this.m_PendingPerformDrag = false;
		}

		private void OnPointerLeaveEvent(PointerLeaveEvent evt)
		{
			this.ClearDragAndDropUI(false);
		}

		private void OnPointerCancelEvent(PointerCancelEvent evt)
		{
			this.CancelDragAndDrop(evt.pointerId);
		}

		private void OnPointerCapturedOut(PointerCaptureOutEvent evt)
		{
			this.CancelDragAndDrop(-1);
		}

		private void OnGeometryChanged(GeometryChangedEvent evt)
		{
			bool flag = this.m_Target.resolvedStyle.display == DisplayStyle.None;
			if (flag)
			{
				this.CancelDragAndDrop(-1);
			}
		}

		private void CancelDragAndDrop(int releaseCapturePointerId = -1)
		{
			bool flag = this.m_DragState == DragEventsProcessor.DragState.None && !this.m_PendingPerformDrag;
			if (!flag)
			{
				bool flag2 = !this.useDragEvents;
				if (flag2)
				{
					this.ClearDragAndDropUI(true);
				}
				bool flag3 = releaseCapturePointerId != -1;
				if (flag3)
				{
					this.m_Target.ReleasePointer(releaseCapturePointerId);
				}
				this.ClearDragAndDropUI(this.m_DragState == DragEventsProcessor.DragState.Dragging);
				this.dragAndDrop.DragCleanup();
				this.m_DragState = DragEventsProcessor.DragState.None;
				this.m_PendingPerformDrag = false;
			}
		}

		private void OnPointerMoveEvent(PointerMoveEvent evt)
		{
			bool isHandledByDraggable = evt.isHandledByDraggable;
			if (!isHandledByDraggable)
			{
				bool flag = !this.useDragEvents && this.m_DragState == DragEventsProcessor.DragState.Dragging;
				if (flag)
				{
					DragEventsProcessor dragEventsProcessor = this.GetDropTarget(evt.position) ?? this;
					dragEventsProcessor.UpdateDrag(evt.position, evt.modifiers);
					this.m_PendingPerformDrag = false;
				}
				else
				{
					bool flag2 = this.m_DragState != DragEventsProcessor.DragState.CanStartDrag;
					if (!flag2)
					{
						bool flag3 = (this.m_Start - evt.position).sqrMagnitude >= 100f || this.m_PendingPerformDrag;
						if (flag3)
						{
							StartDragArgs startDragArgs = this.StartDrag(this.m_Start, evt.modifiers);
							bool flag4 = startDragArgs.visualMode == DragVisualMode.Rejected;
							if (flag4)
							{
								this.m_DragState = DragEventsProcessor.DragState.None;
							}
							else
							{
								bool flag5 = !this.useDragEvents;
								if (flag5)
								{
									bool supportsDragEvents = this.supportsDragEvents;
									if (supportsDragEvents)
									{
										this.dragAndDrop.StartDrag(startDragArgs, evt.position);
									}
								}
								else
								{
									bool flag6 = Event.current != null && Event.current.type != EventType.MouseDown && Event.current.type != EventType.MouseDrag;
									if (flag6)
									{
										return;
									}
									this.dragAndDrop.StartDrag(startDragArgs, evt.position);
								}
								this.m_DragState = DragEventsProcessor.DragState.Dragging;
								this.m_Target.CapturePointer(evt.pointerId);
								evt.isHandledByDraggable = true;
								this.m_PendingPerformDrag = false;
								evt.StopPropagation();
							}
						}
					}
				}
			}
		}

		private DragEventsProcessor GetDropTarget(Vector2 position)
		{
			DragEventsProcessor dragEventsProcessor = null;
			bool flag = this.m_Target.worldBound.Contains(position);
			if (flag)
			{
				dragEventsProcessor = this;
			}
			else
			{
				bool supportsDragEvents = this.supportsDragEvents;
				if (supportsDragEvents)
				{
					VisualElement visualElement = this.m_Target.elementPanel.Pick(position);
					BaseVerticalCollectionView baseVerticalCollectionView = ((visualElement != null) ? visualElement.GetFirstOfType<BaseVerticalCollectionView>() : null);
					dragEventsProcessor = ((baseVerticalCollectionView != null) ? baseVerticalCollectionView.dragger : null);
				}
			}
			return dragEventsProcessor;
		}

		private bool m_IsRegistered;

		private DragEventsProcessor.DragState m_DragState;

		private Vector3 m_Start;

		private bool m_PendingPerformDrag;

		protected readonly VisualElement m_Target;

		internal enum DragState
		{
			None,
			CanStartDrag,
			Dragging
		}
	}
}
