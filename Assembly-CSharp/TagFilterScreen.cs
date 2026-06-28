using System;
using System.Collections.Generic;
using UnityEngine;

public class TagFilterScreen : SideScreenContent
{
	public override void SetTarget(GameObject target)
	{
		if (target == null)
		{
			global::Debug.LogError("The target object provided was null", null);
			return;
		}
		this.targetFilterable = target.GetComponent<TreeFilterable>();
		if (this.targetFilterable == null)
		{
			global::Debug.LogError("The target provided does not have a Tree Filterable component", null);
			return;
		}
		if (!this.targetFilterable.showUserMenu)
		{
			return;
		}
		this.Filter(this.targetFilterable.AcceptedTags);
		base.Activate();
	}

	protected override void OnActivate()
	{
		this.rootItem = this.BuildDisplay(this.rootTag);
		this.treeControl.SetUserItemRoot(this.rootItem);
		this.treeControl.root.opened = true;
		this.Filter(this.treeControl.root, this.acceptedTags, false);
	}

	public static List<Tag> GetAllTags()
	{
		List<Tag> list = new List<Tag>();
		foreach (TagFilterScreen.TagEntry tagEntry in TagFilterScreen.defaultRootTag.children)
		{
			if (tagEntry.tag.IsValid)
			{
				list.Add(tagEntry.tag);
			}
		}
		return list;
	}

	private KTreeControl.UserItem BuildDisplay(TagFilterScreen.TagEntry root)
	{
		KTreeControl.UserItem userItem = null;
		if (root.name != null && root.name != string.Empty)
		{
			userItem = new KTreeControl.UserItem
			{
				text = root.name,
				userData = root.tag
			};
			List<KTreeControl.UserItem> list = new List<KTreeControl.UserItem>();
			if (root.children != null)
			{
				foreach (TagFilterScreen.TagEntry tagEntry in root.children)
				{
					list.Add(this.BuildDisplay(tagEntry));
				}
			}
			userItem.children = list;
		}
		return userItem;
	}

	private static KTreeControl.UserItem CreateTree(string tree_name, Tag tree_tag, IList<Element> items)
	{
		KTreeControl.UserItem userItem = new KTreeControl.UserItem
		{
			text = tree_name,
			userData = tree_tag,
			children = new List<KTreeControl.UserItem>()
		};
		foreach (Element element in items)
		{
			KTreeControl.UserItem userItem2 = new KTreeControl.UserItem
			{
				text = element.name,
				userData = GameTagExtensions.Create(element.id)
			};
			userItem.children.Add(userItem2);
		}
		return userItem;
	}

	public void SetRootTag(TagFilterScreen.TagEntry root_tag)
	{
		this.rootTag = root_tag;
	}

	public void Filter(List<Tag> acceptedTags)
	{
		this.acceptedTags = acceptedTags;
	}

	private void Filter(KTreeItem root, List<Tag> acceptedTags, bool parentEnabled)
	{
		root.checkboxChecked = parentEnabled || (root.userData != null && acceptedTags.Contains((Tag)root.userData));
		foreach (KTreeItem ktreeItem in root.children)
		{
			this.Filter(ktreeItem, acceptedTags, root.checkboxChecked);
		}
		if (!root.checkboxChecked && root.children.Count > 0)
		{
			bool flag = true;
			foreach (KTreeItem ktreeItem2 in root.children)
			{
				if (!ktreeItem2.checkboxChecked)
				{
					flag = false;
					break;
				}
			}
			root.checkboxChecked = flag;
		}
	}

	private void AddEnabledTags(KTreeItem root, List<Tag> tags)
	{
		bool flag = false;
		if (root.userData != null)
		{
			Tag tag = (Tag)root.userData;
			if (tag.IsValid && root.checkboxChecked)
			{
				flag = true;
				tags.Add(tag);
			}
		}
		if (!flag)
		{
			foreach (KTreeItem ktreeItem in root.children)
			{
				this.AddEnabledTags(ktreeItem, tags);
			}
		}
	}

	private void UpdateFilters()
	{
		if (this.targetFilterable == null)
		{
			global::Debug.LogError("Cannot update the filters on a null target.", null);
			return;
		}
		List<Tag> list = new List<Tag>();
		this.AddEnabledTags(this.treeControl.root, list);
		this.targetFilterable.UpdateFilters(list);
	}

	[SerializeField]
	private KTreeControl treeControl;

	private KTreeControl.UserItem rootItem;

	private TagFilterScreen.TagEntry rootTag = TagFilterScreen.defaultRootTag;

	private List<Tag> acceptedTags = new List<Tag>();

	private TreeFilterable targetFilterable;

	public static TagFilterScreen.TagEntry defaultRootTag = new TagFilterScreen.TagEntry
	{
		name = "All",
		tag = default(Tag),
		children = new TagFilterScreen.TagEntry[0]
	};

	public class TagEntry
	{
		public string name;

		public Tag tag;

		public TagFilterScreen.TagEntry[] children;
	}
}
