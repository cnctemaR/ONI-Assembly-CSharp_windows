using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal abstract class BaseReorderableDragAndDropController : ICollectionDragAndDropController, IDragAndDropController<IListDragAndDropArgs>, IReorderable
	{
		public IEnumerable<int> GetSortedSelectedIds()
		{
			return this.m_SortedSelectedIds;
		}

		protected BaseReorderableDragAndDropController(BaseVerticalCollectionView view)
		{
			this.m_View = view;
		}

		public virtual bool enableReordering { get; set; } = true;

		public virtual bool CanStartDrag(IEnumerable<int> itemIds)
		{
			return this.enableReordering;
		}

		public virtual StartDragArgs SetupDragAndDrop(IEnumerable<int> itemIds, bool skipText = false)
		{
			this.m_SortedSelectedIds.Clear();
			string text = string.Empty;
			bool flag = itemIds != null;
			if (flag)
			{
				foreach (int num in itemIds)
				{
					this.m_SortedSelectedIds.Add(num);
					bool flag2 = skipText;
					if (!flag2)
					{
						bool flag3 = string.IsNullOrEmpty(text);
						if (flag3)
						{
							ReusableCollectionItem recycledItemFromId = this.m_View.GetRecycledItemFromId(num);
							Label label = ((recycledItemFromId != null) ? recycledItemFromId.rootElement.Q<Label>(null, null) : null);
							text = ((label != null) ? label.text : string.Format("Item {0}", num));
						}
						else
						{
							text = "<Multiple>";
							skipText = true;
						}
					}
				}
			}
			this.m_SortedSelectedIds.Sort(new Comparison<int>(this.CompareId));
			return new StartDragArgs(text, DragVisualMode.Move);
		}

		protected virtual int CompareId(int id1, int id2)
		{
			return id1.CompareTo(id2);
		}

		public abstract DragVisualMode HandleDragAndDrop(IListDragAndDropArgs args);

		public abstract void OnDrop(IListDragAndDropArgs args);

		public virtual void DragCleanup()
		{
		}

		public virtual void HandleAutoExpand(ReusableCollectionItem item, Vector2 pointerPosition)
		{
		}

		protected readonly BaseVerticalCollectionView m_View;

		protected List<int> m_SortedSelectedIds = new List<int>();
	}
}
