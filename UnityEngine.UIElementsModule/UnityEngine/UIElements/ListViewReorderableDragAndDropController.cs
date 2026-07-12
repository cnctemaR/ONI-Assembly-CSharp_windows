using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements
{
	internal class ListViewReorderableDragAndDropController : IListViewDragAndDropController, IDragAndDropController<object, IListDragAndDropArgs>, IReorderable<object>
	{
		public ListViewReorderableDragAndDropController(ListView listView)
		{
			this.m_ListView = listView;
			this.enableReordering = true;
		}

		public bool enableReordering { get; set; }

		public Action<ItemMoveArgs<object>> onItemMoved { get; set; }

		public virtual bool CanStartDrag(IEnumerable<object> items)
		{
			return this.enableReordering;
		}

		public virtual StartDragArgs SetupDragAndDrop(IEnumerable<object> items)
		{
			string text = string.Empty;
			foreach (object obj in items)
			{
				bool flag = string.IsNullOrEmpty(text);
				if (!flag)
				{
					text = "<Multiple>";
					break;
				}
				int selectedIndex = this.m_ListView.selectedIndex;
				ListView.RecycledItem recycledItemFromIndex = this.m_ListView.GetRecycledItemFromIndex(selectedIndex);
				Label label = ((recycledItemFromIndex != null) ? recycledItemFromIndex.element.Q<Label>(null, null) : null);
				text = ((label != null) ? label.text : string.Format("Item {0}", selectedIndex));
			}
			return new StartDragArgs(text, this.m_ListView);
		}

		public virtual DragVisualMode HandleDragAndDrop(IListDragAndDropArgs args)
		{
			bool flag = args.dragAndDropPosition == DragAndDropPosition.OverItem || !this.enableReordering;
			DragVisualMode dragVisualMode;
			if (flag)
			{
				dragVisualMode = DragVisualMode.Rejected;
			}
			else
			{
				dragVisualMode = ((args.dragAndDropData.userData == this.m_ListView) ? DragVisualMode.Move : DragVisualMode.Rejected);
			}
			return dragVisualMode;
		}

		public virtual void OnDrop(IListDragAndDropArgs args)
		{
			int num = 0;
			int[] array = this.m_ListView.selectedIndices.OrderBy<int, int>((int i) => i).ToArray<int>();
			for (int j = array.Length - 1; j >= 0; j--)
			{
				int num2 = array[j];
				bool flag = num2 < args.insertAtIndex;
				if (flag)
				{
					num--;
				}
				this.m_ListView.itemsSource.RemoveAt(num2);
			}
			DragAndDropPosition dragAndDropPosition = args.dragAndDropPosition;
			DragAndDropPosition dragAndDropPosition2 = dragAndDropPosition;
			if (dragAndDropPosition2 - DragAndDropPosition.BetweenItems > 1)
			{
				throw new ArgumentException(string.Format("{0} is not supported by {1}.", args.dragAndDropPosition, "ListViewReorderableDragAndDropController"));
			}
			this.InsertRange(args.insertAtIndex + num);
			this.m_ListView.Refresh();
		}

		private void InsertRange(int index)
		{
			List<int> list = new List<int>();
			object[] array = this.m_ListView.selectedItems.ToArray<object>();
			int[] array2 = this.m_ListView.selectedIndices.ToArray<int>();
			for (int i = 0; i < array.Length; i++)
			{
				object obj = array[i];
				this.m_ListView.itemsSource.Insert(index, obj);
				Action<ItemMoveArgs<object>> onItemMoved = this.onItemMoved;
				if (onItemMoved != null)
				{
					onItemMoved(new ItemMoveArgs<object>
					{
						item = obj,
						newIndex = index,
						previousIndex = array2[i]
					});
				}
				list.Add(index);
				index++;
			}
			this.m_ListView.SetSelectionWithoutNotify(list);
		}

		protected readonly ListView m_ListView;
	}
}
