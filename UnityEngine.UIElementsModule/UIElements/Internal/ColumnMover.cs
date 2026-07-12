using System;
using System.Diagnostics;

namespace UnityEngine.UIElements.Internal
{
	internal class ColumnMover : PointerManipulator
	{
		public ColumnLayout columnLayout { get; set; }

		public bool active
		{
			get
			{
				return this.m_Active;
			}
			set
			{
				bool flag = this.m_Active == value;
				if (!flag)
				{
					this.m_Active = value;
					Action<ColumnMover> action = this.activeChanged;
					if (action != null)
					{
						action(this);
					}
				}
			}
		}

		public bool moving
		{
			get
			{
				return this.m_Moving;
			}
			set
			{
				bool flag = this.m_Moving == value;
				if (!flag)
				{
					this.m_Moving = value;
					Action<ColumnMover> action = this.movingChanged;
					if (action != null)
					{
						action(this);
					}
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ColumnMover> activeChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ColumnMover> movingChanged;

		public ColumnMover()
		{
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse
			});
		}

		protected override void RegisterCallbacksOnTarget()
		{
			base.target.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<MouseCaptureOutEvent>(new EventCallback<MouseCaptureOutEvent>(this.OnMouseCaptureOut), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCaptureOut), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<MouseCaptureOutEvent>(new EventCallback<MouseCaptureOutEvent>(this.OnMouseCaptureOut), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCaptureOut), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
		}

		protected void OnMouseDown(MouseDownEvent evt)
		{
			bool flag = base.CanStartManipulation(evt);
			if (flag)
			{
				this.ProcessDownEvent(evt, evt.localMousePosition, PointerId.mousePointerId);
			}
		}

		protected void OnMouseMove(MouseMoveEvent evt)
		{
			bool active = this.active;
			if (active)
			{
				this.ProcessMoveEvent(evt, evt.localMousePosition);
			}
		}

		protected void OnMouseUp(MouseUpEvent evt)
		{
			bool flag = this.active && base.CanStopManipulation(evt);
			if (flag)
			{
				this.ProcessUpEvent(evt, evt.localMousePosition, PointerId.mousePointerId);
			}
		}

		private void OnMouseCaptureOut(MouseCaptureOutEvent evt)
		{
			bool active = this.active;
			if (active)
			{
				this.ProcessCancelEvent(evt, PointerId.mousePointerId);
			}
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			bool flag = !base.CanStartManipulation(evt);
			if (!flag)
			{
				bool flag2 = evt.pointerId != PointerId.mousePointerId;
				if (flag2)
				{
					this.ProcessDownEvent(evt, evt.localPosition, evt.pointerId);
					base.target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
				}
				else
				{
					evt.StopImmediatePropagation();
				}
			}
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			bool flag = !this.active;
			if (!flag)
			{
				bool flag2 = evt.pointerId != PointerId.mousePointerId;
				if (flag2)
				{
					this.ProcessMoveEvent(evt, evt.localPosition);
					base.target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
				}
				else
				{
					evt.StopPropagation();
				}
			}
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			bool flag = !this.active || !base.CanStopManipulation(evt);
			if (!flag)
			{
				bool flag2 = evt.pointerId != PointerId.mousePointerId;
				if (flag2)
				{
					this.ProcessUpEvent(evt, evt.localPosition, evt.pointerId);
					base.target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
				}
				else
				{
					evt.StopPropagation();
				}
			}
		}

		private void OnPointerCancel(PointerCancelEvent evt)
		{
			bool flag = !this.active || !base.CanStopManipulation(evt);
			if (!flag)
			{
				bool flag2 = ColumnMover.IsNotMouseEvent(evt.pointerId);
				if (flag2)
				{
					this.ProcessCancelEvent(evt, evt.pointerId);
				}
			}
		}

		private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
		{
			bool flag = !this.active;
			if (!flag)
			{
				bool flag2 = ColumnMover.IsNotMouseEvent(evt.pointerId);
				if (flag2)
				{
					this.ProcessCancelEvent(evt, evt.pointerId);
				}
			}
		}

		private static bool IsNotMouseEvent(int pointerId)
		{
			return pointerId != PointerId.mousePointerId;
		}

		protected void ProcessCancelEvent(EventBase evt, int pointerId)
		{
			this.active = false;
			base.target.ReleasePointer(pointerId);
			bool flag = !(evt is IPointerEvent);
			if (flag)
			{
				base.target.panel.ProcessPointerCapture(pointerId);
			}
			bool moving = this.moving;
			if (moving)
			{
				this.EndDragMove(true);
			}
			evt.StopPropagation();
		}

		private void OnKeyDown(KeyDownEvent e)
		{
			bool flag = e.keyCode == KeyCode.Escape && this.moving;
			if (flag)
			{
				this.EndDragMove(true);
			}
		}

		private void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
		{
			bool active = this.active;
			if (active)
			{
				evt.StopImmediatePropagation();
			}
			else
			{
				base.target.CapturePointer(pointerId);
				bool flag = !(evt is IPointerEvent);
				if (flag)
				{
					base.target.panel.ProcessPointerCapture(pointerId);
				}
				VisualElement visualElement = evt.currentTarget as VisualElement;
				MultiColumnCollectionHeader firstAncestorOfType = visualElement.GetFirstAncestorOfType<MultiColumnCollectionHeader>();
				bool flag2 = !firstAncestorOfType.columns.reorderable;
				if (!flag2)
				{
					this.m_Header = firstAncestorOfType;
					Vector2 vector = visualElement.ChangeCoordinatesTo(this.m_Header, localPosition);
					this.columnLayout = this.m_Header.columnLayout;
					this.m_Cancelled = false;
					this.m_StartPos = vector.x;
					this.active = true;
					evt.StopPropagation();
				}
			}
		}

		private void ProcessMoveEvent(EventBase e, Vector2 localPosition)
		{
			bool cancelled = this.m_Cancelled;
			if (!cancelled)
			{
				VisualElement visualElement = e.currentTarget as VisualElement;
				Vector2 vector = visualElement.ChangeCoordinatesTo(this.m_Header, localPosition);
				bool flag = !this.moving && Mathf.Abs(this.m_StartPos - vector.x) > 5f;
				if (flag)
				{
					this.BeginDragMove(this.m_StartPos);
				}
				bool moving = this.moving;
				if (moving)
				{
					this.DragMove(vector.x);
				}
				e.StopPropagation();
			}
		}

		private void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
		{
			this.active = false;
			base.target.ReleasePointer(pointerId);
			bool flag = !(evt is IPointerEvent);
			if (flag)
			{
				base.target.panel.ProcessPointerCapture(pointerId);
			}
			bool flag2 = this.moving || this.m_Cancelled;
			this.EndDragMove(false);
			bool flag3 = flag2;
			if (flag3)
			{
				evt.StopImmediatePropagation();
			}
			else
			{
				evt.StopPropagation();
			}
		}

		private void BeginDragMove(float pos)
		{
			float num = 0f;
			Columns columns = this.columnLayout.columns;
			foreach (Column column in columns.visibleList)
			{
				num += this.columnLayout.GetDesiredWidth(column);
				bool flag = this.m_ColumnToMove == null;
				if (flag)
				{
					bool flag2 = num > pos;
					if (flag2)
					{
						this.m_ColumnToMove = column;
					}
				}
			}
			this.moving = true;
			this.m_LastPos = pos;
			this.m_PreviewElement = new MultiColumnHeaderColumnMovePreview();
			this.m_LocationPreviewElement = new MultiColumnHeaderColumnMoveLocationPreview();
			this.m_Header.hierarchy.Add(this.m_PreviewElement);
			ScrollView firstAncestorOfType = this.m_Header.GetFirstAncestorOfType<ScrollView>();
			VisualElement visualElement = ((firstAncestorOfType != null) ? firstAncestorOfType.parent : null) ?? this.m_Header;
			visualElement.hierarchy.Add(this.m_LocationPreviewElement);
			this.m_ColumnToMovePos = this.columnLayout.GetDesiredPosition(this.m_ColumnToMove);
			this.m_ColumnToMoveWidth = this.columnLayout.GetDesiredWidth(this.m_ColumnToMove);
			this.UpdateMoveLocation();
		}

		internal void DragMove(float pos)
		{
			this.m_LastPos = pos;
			this.UpdateMoveLocation();
		}

		private void UpdatePreviewPosition()
		{
			this.m_PreviewElement.style.left = this.m_ColumnToMovePos + this.m_LastPos - this.m_StartPos;
			this.m_PreviewElement.style.width = this.m_ColumnToMoveWidth;
			bool flag = this.m_DestinationColumn != null;
			if (flag)
			{
				this.m_LocationPreviewElement.style.left = this.columnLayout.GetDesiredPosition(this.m_DestinationColumn) + ((!this.m_MoveBeforeDestination) ? this.columnLayout.GetDesiredWidth(this.m_DestinationColumn) : 0f);
			}
		}

		private void UpdateMoveLocation()
		{
			float num = 0f;
			this.m_DestinationColumn = null;
			this.m_MoveBeforeDestination = false;
			foreach (Column column in this.columnLayout.columns.visibleList)
			{
				this.m_DestinationColumn = column;
				float desiredWidth = this.columnLayout.GetDesiredWidth(this.m_DestinationColumn);
				float num2 = num + desiredWidth / 2f;
				num += desiredWidth;
				bool flag = num > this.m_LastPos;
				if (flag)
				{
					this.m_MoveBeforeDestination = this.m_LastPos < num2;
					break;
				}
			}
			this.UpdatePreviewPosition();
		}

		private void EndDragMove(bool cancelled)
		{
			bool flag = !this.moving || this.m_Cancelled;
			if (!flag)
			{
				this.m_Cancelled = cancelled;
				bool flag2 = !cancelled;
				if (flag2)
				{
					int num = this.m_DestinationColumn.displayIndex;
					bool flag3 = !this.m_MoveBeforeDestination;
					if (flag3)
					{
						num++;
					}
					bool flag4 = this.m_ColumnToMove.displayIndex < num;
					if (flag4)
					{
						num--;
					}
					bool flag5 = this.m_ColumnToMove.displayIndex != num;
					if (flag5)
					{
						this.columnLayout.columns.ReorderDisplay(this.m_ColumnToMove.displayIndex, num);
					}
				}
				VisualElement previewElement = this.m_PreviewElement;
				if (previewElement != null)
				{
					previewElement.RemoveFromHierarchy();
				}
				this.m_PreviewElement = null;
				MultiColumnHeaderColumnMoveLocationPreview locationPreviewElement = this.m_LocationPreviewElement;
				if (locationPreviewElement != null)
				{
					locationPreviewElement.RemoveFromHierarchy();
				}
				this.m_LocationPreviewElement = null;
				this.m_ColumnToMove = null;
				this.moving = false;
			}
		}

		private const float k_StartDragDistance = 5f;

		private float m_StartPos;

		private float m_LastPos;

		private bool m_Active;

		private bool m_Moving;

		private bool m_Cancelled;

		private MultiColumnCollectionHeader m_Header;

		private VisualElement m_PreviewElement;

		private MultiColumnHeaderColumnMoveLocationPreview m_LocationPreviewElement;

		private Column m_ColumnToMove;

		private float m_ColumnToMovePos;

		private float m_ColumnToMoveWidth;

		private Column m_DestinationColumn;

		private bool m_MoveBeforeDestination;
	}
}
