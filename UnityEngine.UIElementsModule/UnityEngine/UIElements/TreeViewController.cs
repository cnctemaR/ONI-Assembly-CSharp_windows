using System;

namespace UnityEngine.UIElements
{
	public abstract class TreeViewController : BaseTreeViewController
	{
		protected TreeView treeView
		{
			get
			{
				return base.view as TreeView;
			}
		}

		protected override VisualElement MakeItem()
		{
			bool flag = this.treeView.makeItem == null;
			VisualElement visualElement;
			if (flag)
			{
				bool flag2 = this.treeView.bindItem != null;
				if (flag2)
				{
					throw new NotImplementedException("You must specify makeItem if bindItem is specified.");
				}
				visualElement = new Label();
			}
			else
			{
				visualElement = this.treeView.makeItem();
			}
			return visualElement;
		}

		protected override void BindItem(VisualElement element, int index)
		{
			bool flag = this.treeView.bindItem == null;
			if (flag)
			{
				bool flag2 = this.treeView.makeItem != null;
				bool flag3 = flag2;
				if (flag3)
				{
					throw new NotImplementedException("You must specify bindItem if makeItem is specified.");
				}
				Label label = (Label)element;
				object itemForIndex = this.GetItemForIndex(index);
				label.text = ((itemForIndex != null) ? itemForIndex.ToString() : null) ?? "null";
			}
			else
			{
				this.treeView.bindItem(element, index);
			}
		}

		protected override void UnbindItem(VisualElement element, int index)
		{
			Action<VisualElement, int> unbindItem = this.treeView.unbindItem;
			if (unbindItem != null)
			{
				unbindItem(element, index);
			}
		}

		protected override void DestroyItem(VisualElement element)
		{
			Action<VisualElement> destroyItem = this.treeView.destroyItem;
			if (destroyItem != null)
			{
				destroyItem(element);
			}
		}
	}
}
