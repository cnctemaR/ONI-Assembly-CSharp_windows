using System;
using System.Collections.Generic;
using UnityEngine;

public class KTreeControl : MonoBehaviour
{
	public void SetUserItemRoot(KTreeControl.UserItem rootItem)
	{
		if (this.root != null)
		{
			global::UnityEngine.Object.Destroy(this.root);
		}
		this.root = this.CreateItem(rootItem);
		this.root.transform.SetParent(base.transform, false);
	}

	private KTreeItem CreateItem(KTreeControl.UserItem userItem)
	{
		KTreeItem ktreeItem = global::UnityEngine.Object.Instantiate<KTreeItem>(this.treeItemPrefab);
		ktreeItem.text = userItem.text;
		ktreeItem.userData = userItem.userData;
		ktreeItem.onOpenChanged += this.OnOpenChanged;
		ktreeItem.onCheckChanged += this.OnCheckChanged;
		if (userItem.children != null)
		{
			for (int i = 0; i < userItem.children.Count; i++)
			{
				KTreeItem ktreeItem2 = this.CreateItem(userItem.children[i]);
				ktreeItem.AddChild(ktreeItem2);
			}
		}
		return ktreeItem;
	}

	private void OnOpenChanged(KTreeItem item, bool value)
	{
	}

	private void OnCheckChanged(KTreeItem item, bool isChecked)
	{
		if (item.parent != null)
		{
			bool flag = true;
			using (IEnumerator<KTreeItem> enumerator = item.parent.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.checkboxChecked)
					{
						flag = false;
						break;
					}
				}
			}
			item.parent.checkboxChecked = flag;
			this.ChangeChecks(item.parent, flag);
		}
		if (item.children != null)
		{
			foreach (KTreeItem ktreeItem in item.children)
			{
				ktreeItem.checkboxChecked = isChecked;
				this.OnCheckChanged(ktreeItem, isChecked);
			}
		}
	}

	private void ChangeChecks(KTreeItem item, bool isChecked)
	{
		if (item.parent != null)
		{
			bool flag = true;
			using (IEnumerator<KTreeItem> enumerator = item.parent.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.checkboxChecked)
					{
						flag = false;
						break;
					}
				}
			}
			item.parent.checkboxChecked = flag;
			this.ChangeChecks(item.parent, flag);
		}
	}

	[SerializeField]
	private KTreeItem treeItemPrefab;

	[NonSerialized]
	public KTreeItem root;

	public class UserItem
	{
		public string text;

		public object userData;

		public IList<KTreeControl.UserItem> children;
	}
}
