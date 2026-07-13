using System;

namespace UnityEngine.UIElements
{
	public class ListViewController : BaseListViewController
	{
		protected ListView listView
		{
			get
			{
				return base.view as ListView;
			}
		}

		protected override VisualElement MakeItem()
		{
			bool flag = this.listView.makeItem == null;
			VisualElement visualElement;
			if (flag)
			{
				bool flag2 = this.listView.bindItem != null;
				if (flag2)
				{
					throw new NotImplementedException("You must specify makeItem if bindItem is specified.");
				}
				visualElement = new Label();
			}
			else
			{
				visualElement = this.listView.makeItem();
			}
			return visualElement;
		}

		protected override void BindItem(VisualElement element, int index)
		{
			bool flag = this.listView.bindItem == null;
			if (flag)
			{
				bool flag2 = this.listView.makeItem != null;
				bool flag3 = this.listView.autoAssignSource && flag2;
				if (!flag3)
				{
					bool flag4 = flag2;
					if (flag4)
					{
						throw new NotImplementedException("You must specify bindItem if makeItem is specified.");
					}
					Label label = (Label)element;
					object obj = this.listView.itemsSource[index];
					label.text = ((obj != null) ? obj.ToString() : null) ?? "null";
				}
			}
			else
			{
				this.listView.bindItem(element, index);
			}
		}

		protected override void UnbindItem(VisualElement element, int index)
		{
			Action<VisualElement, int> unbindItem = this.listView.unbindItem;
			if (unbindItem != null)
			{
				unbindItem(element, index);
			}
		}

		protected override void DestroyItem(VisualElement element)
		{
			Action<VisualElement> destroyItem = this.listView.destroyItem;
			if (destroyItem != null)
			{
				destroyItem(element);
			}
		}
	}
}
