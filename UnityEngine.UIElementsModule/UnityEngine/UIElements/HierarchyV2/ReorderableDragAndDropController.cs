using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal class ReorderableDragAndDropController : ICollectionDragAndDropController, IDragAndDropController<IListDragAndDropArgs>, IReorderable
	{
		public IEnumerable<int> GetSortedSelectedIndices()
		{
			return this.m_SortedSelectedIndices;
		}

		public ReorderableDragAndDropController(CollectionView view)
		{
			this.m_CollectionView = view;
		}

		public DragVisualMode HandleDragAndDrop(IListDragAndDropArgs args)
		{
			bool flag = args.dragAndDropPosition == DragAndDropPosition.OverItem;
			DragVisualMode dragVisualMode;
			if (flag)
			{
				dragVisualMode = DragVisualMode.Rejected;
			}
			else
			{
				dragVisualMode = ((args.dragAndDropData.source == this.m_CollectionView) ? DragVisualMode.Move : DragVisualMode.Rejected);
			}
			return dragVisualMode;
		}

		public void OnDrop(IListDragAndDropArgs args)
		{
			int insertAtIndex = args.insertAtIndex;
			int num = 0;
			int num2 = 0;
			for (int i = this.m_SortedSelectedIndices.Count - 1; i >= 0; i--)
			{
				int num3 = this.m_SortedSelectedIndices[i];
				bool flag = num3 < 0;
				if (!flag)
				{
					int num4 = insertAtIndex - num;
					bool flag2 = num3 >= insertAtIndex;
					if (flag2)
					{
						num3 += num2;
						num2++;
					}
					else
					{
						bool flag3 = num3 < num4;
						if (flag3)
						{
							num++;
							num4--;
						}
					}
					this.m_CollectionView.Move(num3, num4);
				}
			}
			bool flag4 = this.m_CollectionView.selectionType > SelectionType.None;
			if (flag4)
			{
				List<int> list = new List<int>();
				for (int j = 0; j < this.m_SortedSelectedIndices.Count; j++)
				{
					list.Add(insertAtIndex - num + j);
				}
				this.m_CollectionView.SetSelectionWithoutNotify(list);
			}
			else
			{
				this.m_CollectionView.ClearSelection();
			}
			this.m_CollectionView.RefreshItems();
		}

		public bool enableReordering { get; set; } = true;

		public bool CanStartDrag(IEnumerable<int> itemIndices)
		{
			return this.enableReordering;
		}

		public virtual bool CanDrop()
		{
			return true;
		}

		public StartDragArgs SetupDragAndDrop(IEnumerable<int> itemIndices, bool skipText = false)
		{
			this.m_SortedSelectedIndices.Clear();
			string text = string.Empty;
			bool flag = itemIndices != null;
			if (flag)
			{
				foreach (int num in itemIndices)
				{
					this.m_SortedSelectedIndices.Add(num);
					bool flag2 = skipText;
					if (!flag2)
					{
						bool flag3 = string.IsNullOrEmpty(text);
						if (flag3)
						{
							VisualElement rootElementForIndex = this.m_CollectionView.GetRootElementForIndex(num);
							Label label = ((rootElementForIndex != null) ? rootElementForIndex.Q<Label>(null, null) : null);
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
			this.m_SortedSelectedIndices.Sort(new Comparison<int>(this.CompareIndex));
			return new StartDragArgs(text, DragVisualMode.Move);
		}

		private int CompareIndex(int index1, int index2)
		{
			return index1.CompareTo(index2);
		}

		private readonly CollectionView m_CollectionView;

		private readonly List<int> m_SortedSelectedIndices = new List<int>();
	}
}
