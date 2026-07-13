using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class ListViewReorderableDragAndDropController : BaseReorderableDragAndDropController
	{
		public ListViewReorderableDragAndDropController(BaseListView view)
			: base(view)
		{
			this.m_ListView = view;
		}

		public override DragVisualMode HandleDragAndDrop(IListDragAndDropArgs args)
		{
			bool flag = args.dragAndDropPosition == DragAndDropPosition.OverItem || !this.CanDrop();
			DragVisualMode dragVisualMode;
			if (flag)
			{
				dragVisualMode = DragVisualMode.Rejected;
			}
			else
			{
				dragVisualMode = ((args.dragAndDropData.source == this.m_ListView) ? DragVisualMode.Move : DragVisualMode.Rejected);
			}
			return dragVisualMode;
		}

		public override void OnDrop(IListDragAndDropArgs args)
		{
			base.OnDrop(args);
			bool flag = !this.m_ListView.reorderable;
			if (!flag)
			{
				int insertAtIndex = args.insertAtIndex;
				int num = 0;
				int num2 = 0;
				for (int i = this.m_SortedSelectedIds.Count - 1; i >= 0; i--)
				{
					int num3 = this.m_SortedSelectedIds[i];
					int num4 = this.m_View.viewController.GetIndexForId(num3);
					bool flag2 = num4 < 0;
					if (!flag2)
					{
						int num5 = insertAtIndex - num;
						bool flag3 = num4 >= insertAtIndex;
						if (flag3)
						{
							num4 += num2;
							num2++;
						}
						else
						{
							bool flag4 = num4 < num5;
							if (flag4)
							{
								num++;
								num5--;
							}
						}
						this.m_ListView.viewController.Move(num4, num5);
					}
				}
				bool flag5 = this.m_ListView.selectionType > SelectionType.None;
				if (flag5)
				{
					List<int> list = new List<int>();
					for (int j = 0; j < this.m_SortedSelectedIds.Count; j++)
					{
						list.Add(insertAtIndex - num + j);
					}
					this.m_ListView.SetSelectionWithoutNotify(list);
				}
				else
				{
					this.m_ListView.ClearSelection();
				}
			}
		}

		protected readonly BaseListView m_ListView;
	}
}
