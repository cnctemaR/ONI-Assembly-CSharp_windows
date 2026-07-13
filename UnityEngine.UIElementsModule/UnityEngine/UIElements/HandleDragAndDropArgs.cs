using System;

namespace UnityEngine.UIElements
{
	public readonly struct HandleDragAndDropArgs
	{
		public Vector2 position { get; }

		public object target
		{
			get
			{
				return this.m_DragAndDropArgs.target;
			}
		}

		public int insertAtIndex
		{
			get
			{
				return this.m_DragAndDropArgs.insertAtIndex;
			}
		}

		public int parentId
		{
			get
			{
				return this.m_DragAndDropArgs.parentId;
			}
		}

		public int childIndex
		{
			get
			{
				return this.m_DragAndDropArgs.childIndex;
			}
		}

		public DragAndDropPosition dropPosition
		{
			get
			{
				return this.m_DragAndDropArgs.dragAndDropPosition;
			}
		}

		public DragAndDropData dragAndDropData
		{
			get
			{
				return this.m_DragAndDropArgs.dragAndDropData;
			}
		}

		internal EventModifiers modifiers
		{
			get
			{
				return this.m_DragAndDropArgs.modifiers;
			}
		}

		internal HandleDragAndDropArgs(Vector2 position, DragAndDropArgs dragAndDropArgs)
		{
			this.position = position;
			this.m_DragAndDropArgs = dragAndDropArgs;
		}

		private readonly DragAndDropArgs m_DragAndDropArgs;
	}
}
