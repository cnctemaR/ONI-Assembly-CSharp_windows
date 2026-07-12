using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/TreeFilterable")]
public class TreeFilterable : KMonoBehaviour, ISaveLoadable
{
	public List<Tag> AcceptedTags
	{
		get
		{
			return this.acceptedTags;
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 20))
		{
			this.filterByStorageCategoriesOnSpawn = false;
		}
	}

	private void OnDiscover(Tag category_tag, Tag tag)
	{
		if (this.storage.storageFilters.Contains(category_tag))
		{
			bool flag = false;
			if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(category_tag).Count <= 1)
			{
				foreach (Tag tag2 in this.storage.storageFilters)
				{
					if (!(tag2 == category_tag) && DiscoveredResources.Instance.IsDiscovered(tag2))
					{
						flag = true;
						foreach (Tag tag3 in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(tag2))
						{
							if (!this.acceptedTags.Contains(tag3))
							{
								return;
							}
						}
					}
				}
				if (!flag)
				{
					return;
				}
			}
			foreach (Tag tag4 in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(category_tag))
			{
				if (!(tag4 == tag) && !this.acceptedTags.Contains(tag4))
				{
					return;
				}
			}
			this.AddTagToFilter(tag);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<TreeFilterable>(-905833192, TreeFilterable.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		DiscoveredResources.Instance.OnDiscover += this.OnDiscover;
		if (this.autoSelectStoredOnLoad && this.storage != null)
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
		this.RefreshTint();
		if (this.filterByStorageCategoriesOnSpawn)
		{
			this.RemoveIncorrectAcceptedTags();
		}
	}

	private void RemoveIncorrectAcceptedTags()
	{
		List<Tag> list = new List<Tag>();
		foreach (Tag tag in this.acceptedTags)
		{
			bool flag = false;
			foreach (Tag tag2 in this.storage.storageFilters)
			{
				if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(tag2).Contains(tag))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(tag);
			}
		}
		foreach (Tag tag3 in list)
		{
			this.RemoveTagFromFilter(tag3);
		}
	}

	protected override void OnCleanUp()
	{
		DiscoveredResources.Instance.OnDiscover -= this.OnDiscover;
		base.OnCleanUp();
	}

	private void OnCopySettings(object data)
	{
		TreeFilterable component = ((GameObject)data).GetComponent<TreeFilterable>();
		if (component != null)
		{
			this.UpdateFilters(component.GetTags());
		}
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
		this.RefreshTint();
		if (this.dropIncorrectOnFilterChange && this.storage != null && this.storage.items != null)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in this.storage.items)
			{
				if (!(gameObject == null))
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
			}
			foreach (GameObject gameObject2 in list)
			{
				this.storage.Drop(gameObject2, true);
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

	private void RefreshTint()
	{
		bool flag = this.acceptedTags != null && this.acceptedTags.Count != 0;
		base.GetComponent<KBatchedAnimController>().TintColour = (flag ? this.filterTint : this.noFilterTint);
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoStorageFilterSet, !flag, this);
	}

	[MyCmpReq]
	private Storage storage;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	public static readonly Color32 FILTER_TINT = Color.white;

	public static readonly Color32 NO_FILTER_TINT = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);

	public Color32 filterTint = TreeFilterable.FILTER_TINT;

	public Color32 noFilterTint = TreeFilterable.NO_FILTER_TINT;

	[SerializeField]
	public bool dropIncorrectOnFilterChange = true;

	[SerializeField]
	public bool autoSelectStoredOnLoad = true;

	public bool showUserMenu = true;

	public bool filterByStorageCategoriesOnSpawn = true;

	[SerializeField]
	[Serialize]
	private List<Tag> acceptedTags = new List<Tag>();

	public Action<Tag[]> OnFilterChanged;

	private static readonly EventSystem.IntraObjectHandler<TreeFilterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<TreeFilterable>(delegate(TreeFilterable component, object data)
	{
		component.OnCopySettings(data);
	});
}
