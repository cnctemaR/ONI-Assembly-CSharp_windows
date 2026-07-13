using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class TabDragger : PointerManipulator
	{
		private TabLayout tabLayout { get; set; }

		internal bool active { get; set; }

		internal bool isVertical { get; set; }

		internal bool moving
		{
			get
			{
				return this.m_Moving;
			}
			private set
			{
				bool flag = this.m_Moving == value;
				if (!flag)
				{
					this.m_Moving = value;
					this.m_TabToMove.EnableInClassList(Tab.draggingUssClassName, this.moving);
				}
			}
		}

		public TabDragger()
		{
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse
			});
		}

		protected override void RegisterCallbacksOnTarget()
		{
			base.target.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.TrickleDown);
			base.target.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.TrickleDown);
			base.target.RegisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCaptureOut), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCaptureOut), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			bool flag = !base.CanStartManipulation(evt);
			if (!flag)
			{
				bool active = this.active;
				if (active)
				{
					evt.StopImmediatePropagation();
				}
				else
				{
					this.ProcessDownEvent(evt, evt.localPosition, evt.pointerId);
				}
			}
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			bool flag = !this.active;
			if (!flag)
			{
				this.ProcessMoveEvent(evt, evt.localPosition);
			}
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			bool flag = !this.active || !base.CanStopManipulation(evt);
			if (!flag)
			{
				this.ProcessUpEvent(evt, evt.localPosition, evt.pointerId);
			}
		}

		private void OnPointerCancel(PointerCancelEvent evt)
		{
			bool flag = !this.active || !base.CanStopManipulation(evt);
			if (!flag)
			{
				this.ProcessCancelEvent(evt, evt.pointerId);
			}
		}

		private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
		{
			bool flag = !this.active;
			if (!flag)
			{
				this.ProcessCancelEvent(evt, evt.pointerId);
			}
		}

		private void ProcessCancelEvent(EventBase evt, int pointerId)
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
		}

		private void OnKeyDown(KeyDownEvent e)
		{
			bool flag = e.keyCode == KeyCode.Escape && this.moving;
			if (flag)
			{
				this.active = false;
				bool flag2 = this.m_DraggingPointerId != PointerId.invalidPointerId;
				if (flag2)
				{
					base.target.ReleasePointer(this.m_DraggingPointerId);
				}
				this.EndDragMove(true);
				e.StopPropagation();
			}
		}

		private void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
		{
			VisualElement visualElement = evt.currentTarget as VisualElement;
			TabView tabView = ((visualElement != null) ? visualElement.GetFirstAncestorOfType<TabView>() : null);
			bool flag = tabView == null || !tabView.reorderable;
			if (!flag)
			{
				base.target.CapturePointer(pointerId);
				this.m_DraggingPointerId = pointerId;
				bool flag2 = !(evt is IPointerEvent);
				if (flag2)
				{
					base.target.panel.ProcessPointerCapture(pointerId);
				}
				this.m_TabView = tabView;
				this.m_Header = tabView.header;
				this.isVertical = this.m_Header.resolvedStyle.flexDirection == FlexDirection.Column;
				this.tabLayout = new TabLayout(this.m_TabView, this.isVertical);
				Vector2 vector = visualElement.ChangeCoordinatesTo(this.m_Header, localPosition);
				this.m_Cancelled = false;
				this.m_StartPos = (this.isVertical ? vector.y : vector.x);
				this.active = true;
				evt.StopPropagation();
			}
		}

		private void ProcessMoveEvent(EventBase e, Vector2 localPosition)
		{
			bool cancelled = this.m_Cancelled;
			if (!cancelled)
			{
				VisualElement visualElement = e.currentTarget as VisualElement;
				Vector2 vector = visualElement.ChangeCoordinatesTo(this.m_Header, localPosition);
				float num = (this.isVertical ? vector.y : vector.x);
				bool flag = !this.moving && Mathf.Abs(this.m_StartPos - num) > 5f;
				if (flag)
				{
					this.BeginDragMove(this.m_StartPos);
				}
				bool moving = this.moving;
				if (moving)
				{
					this.DragMove(num);
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
			this.EndDragMove(false);
			evt.StopPropagation();
		}

		private void BeginDragMove(float pos)
		{
			float num = 0f;
			List<VisualElement> tabHeaders = this.m_TabView.tabHeaders;
			this.m_TabToMove = this.m_TabView.tabHeaders[0];
			foreach (VisualElement visualElement in tabHeaders)
			{
				num += (this.isVertical ? TabLayout.GetHeight(visualElement) : TabLayout.GetWidth(visualElement));
				bool flag = num > pos;
				if (flag)
				{
					this.m_TabToMove = visualElement;
					break;
				}
			}
			this.moving = true;
			this.m_LastPos = pos;
			this.m_PreviewElement = new TabDragPreview();
			this.m_LocationPreviewElement = new TabDragLocationPreview
			{
				classList = { this.isVertical ? TabDragLocationPreview.verticalUssClassName : TabDragLocationPreview.horizontalUssClassName }
			};
			this.m_Header.hierarchy.Add(this.m_PreviewElement);
			this.m_Header.Add(this.m_LocationPreviewElement);
			int num2 = this.m_TabView.tabHeaders.IndexOf(this.m_TabToMove);
			Tab tab = this.m_TabView.tabs[num2];
			this.m_TabView.activeTab = tab;
			this.m_TabToMovePos = this.tabLayout.GetTabOffset(this.m_TabToMove);
			this.UpdateMoveLocation();
		}

		private void DragMove(float pos)
		{
			this.m_LastPos = pos;
			this.UpdateMoveLocation();
		}

		private void UpdatePreviewPosition()
		{
			float num = this.m_TabToMovePos + this.m_LastPos - this.m_StartPos;
			float width = TabLayout.GetWidth(this.m_TabToMove);
			float tabOffset = this.tabLayout.GetTabOffset(this.m_DestinationTab);
			float num2 = (this.isVertical ? TabLayout.GetHeight(this.m_DestinationTab) : TabLayout.GetWidth(this.m_DestinationTab));
			float num3 = ((!this.m_MoveBeforeDestination) ? num2 : 0f);
			bool isVertical = this.isVertical;
			if (isVertical)
			{
				this.m_PreviewElement.style.top = num;
				this.m_PreviewElement.style.height = TabLayout.GetHeight(this.m_TabToMove);
				this.m_PreviewElement.style.width = width;
				bool flag = this.m_DestinationTab != null;
				if (flag)
				{
					this.m_LocationPreviewElement.preview.style.width = width;
					this.m_LocationPreviewElement.style.top = tabOffset + num3;
				}
			}
			else
			{
				this.m_PreviewElement.style.left = num;
				this.m_PreviewElement.style.width = width;
				bool flag2 = this.m_DestinationTab != null;
				if (flag2)
				{
					this.m_LocationPreviewElement.style.left = tabOffset + num3;
				}
			}
		}

		private void UpdateMoveLocation()
		{
			float num = 0f;
			this.m_DestinationTab = null;
			this.m_MoveBeforeDestination = false;
			foreach (VisualElement visualElement in this.m_TabView.tabHeaders)
			{
				this.m_DestinationTab = visualElement;
				float num2 = (this.isVertical ? TabLayout.GetHeight(this.m_DestinationTab) : TabLayout.GetWidth(this.m_DestinationTab));
				float num3 = num + num2 / 2f;
				num += num2;
				bool flag = num > this.m_LastPos;
				if (flag)
				{
					this.m_MoveBeforeDestination = this.m_LastPos < num3;
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
					int num = this.m_TabView.tabHeaders.IndexOf(this.m_TabToMove);
					int num2 = this.m_TabView.tabHeaders.IndexOf(this.m_DestinationTab);
					bool flag3 = !this.m_MoveBeforeDestination;
					if (flag3)
					{
						num2++;
					}
					bool flag4 = num < num2;
					if (flag4)
					{
						num2--;
					}
					bool flag5 = num != num2;
					if (flag5)
					{
						this.tabLayout.ReorderDisplay(num, num2);
					}
				}
				VisualElement previewElement = this.m_PreviewElement;
				if (previewElement != null)
				{
					previewElement.RemoveFromHierarchy();
				}
				this.m_PreviewElement = null;
				TabDragLocationPreview locationPreviewElement = this.m_LocationPreviewElement;
				if (locationPreviewElement != null)
				{
					locationPreviewElement.RemoveFromHierarchy();
				}
				this.m_LocationPreviewElement = null;
				this.moving = false;
				this.m_TabToMove = null;
			}
		}

		private const float k_StartDragDistance = 5f;

		private float m_StartPos;

		private float m_LastPos;

		private bool m_Moving;

		private bool m_Cancelled;

		private VisualElement m_Header;

		private TabView m_TabView;

		private VisualElement m_PreviewElement;

		private TabDragLocationPreview m_LocationPreviewElement;

		private VisualElement m_TabToMove;

		private float m_TabToMovePos;

		private VisualElement m_DestinationTab;

		private bool m_MoveBeforeDestination;

		private int m_DraggingPointerId = PointerId.invalidPointerId;
	}
}
