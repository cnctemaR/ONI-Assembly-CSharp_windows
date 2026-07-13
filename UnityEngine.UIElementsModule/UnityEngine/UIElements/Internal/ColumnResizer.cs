using System;

namespace UnityEngine.UIElements.Internal
{
	internal class ColumnResizer : PointerManipulator
	{
		public ColumnLayout columnLayout { get; set; }

		public bool preview { get; set; }

		public ColumnResizer(Column column)
		{
			this.m_Column = column;
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse
			});
			this.m_Active = false;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			base.target.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
		}

		private void OnKeyDown(KeyDownEvent e)
		{
			bool flag = e.keyCode == KeyCode.Escape && this.m_Resizing && this.preview;
			if (flag)
			{
				this.EndDragResize(0f, true);
			}
		}

		private void OnPointerDown(PointerDownEvent e)
		{
			bool active = this.m_Active;
			if (active)
			{
				e.StopImmediatePropagation();
			}
			else
			{
				bool flag = base.CanStartManipulation(e);
				if (flag)
				{
					VisualElement visualElement = e.currentTarget as VisualElement;
					this.m_Header = visualElement.GetFirstAncestorOfType<MultiColumnCollectionHeader>();
					this.preview = this.m_Column.collection.resizePreview;
					bool preview = this.preview;
					if (preview)
					{
						bool flag2 = this.m_PreviewElement == null;
						if (flag2)
						{
							this.m_PreviewElement = new MultiColumnHeaderColumnResizePreview();
						}
						ScrollView firstAncestorOfType = this.m_Header.GetFirstAncestorOfType<ScrollView>();
						VisualElement visualElement2 = ((firstAncestorOfType != null) ? firstAncestorOfType.parent : null) ?? this.m_Header.parent;
						visualElement2.hierarchy.Add(this.m_PreviewElement);
					}
					this.columnLayout = this.m_Header.columnLayout;
					this.m_Start = visualElement.ChangeCoordinatesTo(this.m_Header, e.localPosition);
					this.BeginDragResize(this.m_Start.x);
					this.m_Active = true;
					base.target.CaptureMouse();
					e.StopPropagation();
				}
			}
		}

		private void OnPointerMove(PointerMoveEvent e)
		{
			bool flag = !this.m_Active || !base.target.HasPointerCapture(e.pointerId);
			if (!flag)
			{
				VisualElement visualElement = e.currentTarget as VisualElement;
				Vector2 vector = visualElement.ChangeCoordinatesTo(this.m_Header, e.localPosition);
				this.DragResize(vector.x);
				e.StopPropagation();
			}
		}

		private void OnPointerUp(PointerUpEvent e)
		{
			bool flag = !this.m_Active || !base.target.HasPointerCapture(e.pointerId) || !base.CanStopManipulation(e);
			if (!flag)
			{
				VisualElement visualElement = e.currentTarget as VisualElement;
				Vector2 vector = visualElement.ChangeCoordinatesTo(this.m_Header, e.localPosition);
				this.EndDragResize(vector.x, false);
				this.m_Active = false;
				base.target.ReleasePointer(e.pointerId);
				e.StopPropagation();
			}
		}

		private void BeginDragResize(float pos)
		{
			this.m_Resizing = true;
			ColumnLayout columnLayout = this.columnLayout;
			if (columnLayout != null)
			{
				columnLayout.BeginDragResize(this.m_Column, this.m_Start.x, this.preview);
			}
			bool preview = this.preview;
			if (preview)
			{
				this.UpdatePreviewPosition();
			}
		}

		private void DragResize(float pos)
		{
			bool flag = !this.m_Resizing;
			if (!flag)
			{
				ColumnLayout columnLayout = this.columnLayout;
				if (columnLayout != null)
				{
					columnLayout.DragResize(this.m_Column, pos);
				}
				bool preview = this.preview;
				if (preview)
				{
					this.UpdatePreviewPosition();
				}
			}
		}

		private void UpdatePreviewPosition()
		{
			this.m_PreviewElement.style.left = this.columnLayout.GetDesiredPosition(this.m_Column) + this.columnLayout.GetDesiredWidth(this.m_Column);
		}

		private void EndDragResize(float pos, bool cancelled)
		{
			bool flag = !this.m_Resizing;
			if (!flag)
			{
				bool preview = this.preview;
				if (preview)
				{
					VisualElement previewElement = this.m_PreviewElement;
					if (previewElement != null)
					{
						previewElement.RemoveFromHierarchy();
					}
					this.m_PreviewElement = null;
				}
				ColumnLayout columnLayout = this.columnLayout;
				if (columnLayout != null)
				{
					columnLayout.EndDragResize(this.m_Column, cancelled);
				}
				this.m_Resizing = false;
			}
		}

		private Vector2 m_Start;

		protected bool m_Active;

		private bool m_Resizing;

		private MultiColumnCollectionHeader m_Header;

		private Column m_Column;

		private VisualElement m_PreviewElement;
	}
}
