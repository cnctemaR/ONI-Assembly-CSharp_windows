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
	public HashSet<Tag> AcceptedTags
	{
		get
		{
			return this.acceptedTagSet;
		}
	}

	[OnDeserialized]
	[Obsolete]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 20))
		{
			this.filterByStorageCategoriesOnSpawn = false;
		}
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 29))
		{
			this.acceptedTagSet.UnionWith(this.acceptedTags);
			this.acceptedTags = null;
		}
	}

	private void OnDiscover(Tag category_tag, Tag tag)
	{
		if (this.preventAutoAddOnDiscovery)
		{
			return;
		}
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
							if (!this.acceptedTagSet.Contains(tag3))
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
				if (!(tag4 == tag) && !this.acceptedTagSet.Contains(tag4))
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
			HashSet<Tag> hashSet = new HashSet<Tag>(this.acceptedTagSet);
			hashSet.UnionWith(this.storage.GetAllIDsInStorage());
			this.UpdateFilters(hashSet);
		}
		if (this.OnFilterChanged != null)
		{
			this.OnFilterChanged(this.acceptedTagSet);
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
		foreach (Tag tag in this.acceptedTagSet)
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
		if (this.copySettingsEnabled)
		{
			TreeFilterable component = ((GameObject)data).GetComponent<TreeFilterable>();
			if (component != null)
			{
				this.UpdateFilters(component.GetTags());
			}
		}
	}

	public HashSet<Tag> GetTags()
	{
		return this.acceptedTagSet;
	}

	public bool ContainsTag(Tag t)
	{
		return this.acceptedTagSet.Contains(t);
	}

	public void AddTagToFilter(Tag t)
	{
		if (this.ContainsTag(t))
		{
			return;
		}
		this.UpdateFilters(new HashSet<Tag>(this.acceptedTagSet) { t });
	}

	public void RemoveTagFromFilter(Tag t)
	{
		if (!this.ContainsTag(t))
		{
			return;
		}
		HashSet<Tag> hashSet = new HashSet<Tag>(this.acceptedTagSet);
		hashSet.Remove(t);
		this.UpdateFilters(hashSet);
	}

	public void UpdateFilters(HashSet<Tag> filters)
	{
		this.acceptedTagSet.Clear();
		this.acceptedTagSet.UnionWith(filters);
		if (this.OnFilterChanged != null)
		{
			this.OnFilterChanged(this.acceptedTagSet);
		}
		this.RefreshTint();
		if (!this.dropIncorrectOnFilterChange || this.storage == null || this.storage.items == null)
		{
			return;
		}
		if (!this.filterAllStoragesOnBuilding)
		{
			this.DropFilteredItemsFromTargetStorage(this.storage);
			return;
		}
		foreach (Storage storage in base.GetComponents<Storage>())
		{
			this.DropFilteredItemsFromTargetStorage(storage);
		}
	}

	private void DropFilteredItemsFromTargetStorage(Storage targetStorage)
	{
		for (int i = targetStorage.items.Count - 1; i >= 0; i--)
		{
			GameObject gameObject = targetStorage.items[i];
			if (!(gameObject == null))
			{
				KPrefabID component = gameObject.GetComponent<KPrefabID>();
				if (!this.acceptedTagSet.Contains(component.PrefabTag))
				{
					targetStorage.Drop(gameObject, true);
				}
			}
		}
	}

	public string GetTagsAsStatus(int maxDisplays = 6)
	{
		string text = "Tags:\n";
		List<Tag> list = new List<Tag>(this.storage.storageFilters);
		list.Intersect<Tag>(this.acceptedTagSet);
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
		bool flag = this.acceptedTagSet != null && this.acceptedTagSet.Count != 0;
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

	public bool copySettingsEnabled = true;

	public bool preventAutoAddOnDiscovery;

	public bool filterAllStoragesOnBuilding;

	public TreeFilterable.UISideScreenHeight uiHeight = TreeFilterable.UISideScreenHeight.Tall;

	public bool filterByStorageCategoriesOnSpawn = true;

	[SerializeField]
	[Serialize]
	[Obsolete("Deprecated, use acceptedTagSet")]
	private List<Tag> acceptedTags = new List<Tag>();

	[SerializeField]
	[Serialize]
	private HashSet<Tag> acceptedTagSet = new HashSet<Tag>();

	public Action<HashSet<Tag>> OnFilterChanged;

	private static readonly EventSystem.IntraObjectHandler<TreeFilterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<TreeFilterable>(delegate(TreeFilterable component, object data)
	{
		component.OnCopySettings(data);
	});

	public enum UISideScreenHeight
	{
		Short,
		Tall
	}
}
