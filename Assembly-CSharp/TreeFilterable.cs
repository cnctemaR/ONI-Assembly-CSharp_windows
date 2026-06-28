using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TreeFilterable : KMonoBehaviour, ISaveLoadable
{
	public List<Tag> AcceptedTags
	{
		get
		{
			return this.acceptedTags;
		}
	}

	private void OnDiscover(Tag tag)
	{
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			Tag materialCategoryTag = element.GetMaterialCategoryTag();
			if (this.acceptedTags.Contains(materialCategoryTag))
			{
				this.AddTagToFilter(tag);
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	protected override void OnSpawn()
	{
		WorldInventory.Instance.OnDiscover += this.OnDiscover;
		if (this.storage != null)
		{
			List<Tag> list = new List<Tag>();
			list.AddRange(this.acceptedTags);
			list.AddRange(this.storage.GetAllTagsInStorage());
			this.UpdateFilters(list.Distinct<Tag>().ToList<Tag>());
		}
		if (this.OnFilterChanged != null)
		{
			this.OnFilterChanged(this.acceptedTags.ToArray());
		}
	}

	protected override void OnCleanUp()
	{
		WorldInventory.Instance.OnDiscover -= this.OnDiscover;
		base.OnCleanUp();
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		TreeFilterable component = gameObject.GetComponent<TreeFilterable>();
		if (component != null)
		{
			this.UpdateFilters(component.GetTags());
		}
	}

	public void SetTags(List<Tag> tags)
	{
		this.acceptedTags = tags;
	}

	public Tag[] GetTags()
	{
		return this.acceptedTags.ToArray();
	}

	public bool ContainsTag(Tag t)
	{
		return this.acceptedTags.Contains(t);
	}

	public void AddTagToFilter(Tag t)
	{
		if (this.ContainsTag(t))
		{
			return;
		}
		this.UpdateFilters(new List<Tag>(this.acceptedTags) { t });
	}

	public void RemoveTagFromFilter(Tag t)
	{
		if (!this.ContainsTag(t))
		{
			return;
		}
		List<Tag> list = new List<Tag>(this.acceptedTags);
		list.Remove(t);
		this.UpdateFilters(list);
	}

	public void UpdateFilters(IList<Tag> filters)
	{
		this.acceptedTags.Clear();
		this.acceptedTags.AddRange(filters);
		if (this.OnFilterChanged != null)
		{
			this.OnFilterChanged(this.acceptedTags.ToArray());
		}
		if (this.storage != null && this.storage.items != null)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in this.storage.items)
			{
				KPrefabID component = gameObject.GetComponent<KPrefabID>();
				bool flag = false;
				foreach (Tag tag in this.acceptedTags)
				{
					if (component.Tags.Contains(tag))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(gameObject);
				}
			}
			foreach (GameObject gameObject2 in list)
			{
				this.storage.Drop(gameObject2);
			}
		}
	}

	public string GetTagsAsStatus(int maxDisplays = 6)
	{
		string text = "Tags:\n";
		List<Tag> list = new List<Tag>(this.acceptedTags);
		list.Intersect<Tag>(this.storage.storageFilters);
		for (int i = 0; i < Mathf.Min(list.Count, maxDisplays); i++)
		{
			text += list[i].ProperName();
			if (i < Mathf.Min(list.Count, maxDisplays) - 1)
			{
				text += "\n";
			}
			if (i == maxDisplays - 1 && list.Count > maxDisplays)
			{
				text += "\n...";
				break;
			}
		}
		if (base.tag.Length == 0)
		{
			text = "No tags selected";
		}
		return text;
	}

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private UserMenu userMenu;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	public bool showUserMenu = true;

	[Serialize]
	[SerializeField]
	private List<Tag> acceptedTags = new List<Tag>();

	public Action<Tag[]> OnFilterChanged;
}
