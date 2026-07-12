using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class PinnedResourcesPanel : KScreen, IRender1000ms
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		PinnedResourcesPanel.Instance = this;
		this.Populate(null);
		Game.Instance.Subscribe(1983128072, new Action<object>(this.Populate));
		MultiToggle component = this.headerButton.GetComponent<MultiToggle>();
		component.onClick = (global::System.Action)Delegate.Combine(component.onClick, new global::System.Action(delegate
		{
			this.Refresh();
		}));
		MultiToggle component2 = this.seeAllButton.GetComponent<MultiToggle>();
		component2.onClick = (global::System.Action)Delegate.Combine(component2.onClick, new global::System.Action(delegate
		{
			AllResourcesScreen.Instance.Show(!AllResourcesScreen.Instance.gameObject.activeSelf);
		}));
		MultiToggle component3 = this.clearNewButton.GetComponent<MultiToggle>();
		component3.onClick = (global::System.Action)Delegate.Combine(component3.onClick, new global::System.Action(delegate
		{
			this.ClearAllNew();
		}));
		this.clearAllButton.onClick += delegate
		{
			this.ClearAllNew();
			this.UnPinAll();
			this.Refresh();
		};
		AllResourcesScreen.Instance.Init();
		this.Refresh();
	}

	public void ClearExcessiveNewItems()
	{
		if (DiscoveredResources.Instance.CheckAllDiscoveredAreNew())
		{
			DiscoveredResources.Instance.newDiscoveries.Clear();
		}
	}

	private void ClearAllNew()
	{
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.rows)
		{
			if (keyValuePair.Value.activeSelf && DiscoveredResources.Instance.newDiscoveries.ContainsKey(keyValuePair.Key))
			{
				DiscoveredResources.Instance.newDiscoveries.Remove(keyValuePair.Key);
			}
		}
	}

	private void UnPinAll()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.rows)
		{
			worldInventory.pinnedResources.Remove(keyValuePair.Key);
		}
	}

	public void Populate(object data = null)
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		using (Dictionary<Tag, float>.Enumerator enumerator = DiscoveredResources.Instance.newDiscoveries.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<Tag, float> resource = enumerator.Current;
				if (!this.rows.ContainsKey(resource.Key) && this.IsDisplayedTag(resource.Key))
				{
					GameObject gameObject = Util.KInstantiateUI(this.linePrefab, this.rowContainer, false);
					this.rows.Add(resource.Key, gameObject);
					MultiToggle component = gameObject.GetComponent<MultiToggle>();
					component.onClick = (global::System.Action)Delegate.Combine(component.onClick, new global::System.Action(delegate
					{
						List<Pickupable> list = ClusterManager.Instance.activeWorld.worldInventory.CreatePickupablesList(resource.Key);
						if (list != null && list.Count > 0)
						{
							SelectTool.Instance.SelectAndFocus(list[this.clickIdx % list.Count].transform.position, list[this.clickIdx % list.Count].GetComponent<KSelectable>());
							this.clickIdx++;
							return;
						}
						this.clickIdx = 0;
					}));
				}
			}
		}
		using (List<Tag>.Enumerator enumerator2 = worldInventory.pinnedResources.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				Tag tag = enumerator2.Current;
				if (!this.rows.ContainsKey(tag))
				{
					GameObject gameObject2 = Util.KInstantiateUI(this.linePrefab, this.rowContainer, false);
					MultiToggle component2 = gameObject2.GetComponent<MultiToggle>();
					component2.onClick = (global::System.Action)Delegate.Combine(component2.onClick, new global::System.Action(delegate
					{
						List<Pickupable> list2 = ClusterManager.Instance.activeWorld.worldInventory.CreatePickupablesList(tag);
						if (list2 != null && list2.Count > 0)
						{
							SelectTool.Instance.SelectAndFocus(list2[this.clickIdx % list2.Count].transform.position, list2[this.clickIdx % list2.Count].GetComponent<KSelectable>());
							this.clickIdx++;
							return;
						}
						this.clickIdx = 0;
					}));
					this.rows.Add(tag, gameObject2);
				}
			}
		}
		foreach (Tag tag2 in worldInventory.notifyResources)
		{
			if (!this.rows.ContainsKey(tag2))
			{
				GameObject gameObject3 = Util.KInstantiateUI(this.linePrefab, this.rowContainer, false);
				this.rows.Add(tag2, gameObject3);
			}
		}
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.rows)
		{
			if (false || worldInventory.pinnedResources.Contains(keyValuePair.Key) || worldInventory.notifyResources.Contains(keyValuePair.Key) || (DiscoveredResources.Instance.newDiscoveries.ContainsKey(keyValuePair.Key) && worldInventory.GetAmount(keyValuePair.Key, false) > 0f))
			{
				if (!keyValuePair.Value.activeSelf)
				{
					keyValuePair.Value.SetActive(true);
				}
			}
			else if (keyValuePair.Value.activeSelf)
			{
				keyValuePair.Value.SetActive(false);
			}
		}
		foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.rows)
		{
			keyValuePair2.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("PinToggle").gameObject.SetActive(worldInventory.pinnedResources.Contains(keyValuePair2.Key));
		}
		this.SortRows();
	}

	private void SortRows()
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.rows)
		{
			list.Add(keyValuePair.Key);
		}
		list.Sort((Tag a, Tag b) => a.ProperNameStripLink().CompareTo(b.ProperNameStripLink()));
		foreach (Tag tag in list)
		{
			this.rows[tag].transform.SetAsLastSibling();
		}
		this.clearNewButton.transform.SetAsLastSibling();
		this.seeAllButton.transform.SetAsLastSibling();
	}

	private bool IsDisplayedTag(Tag tag)
	{
		foreach (TagSet tagSet in AllResourcesScreen.Instance.allowDisplayCategories)
		{
			foreach (KeyValuePair<Tag, HashSet<Tag>> keyValuePair in DiscoveredResources.Instance.GetDiscoveredResourcesFromTagSet(tagSet))
			{
				if (keyValuePair.Value.Contains(tag))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void SyncRows()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		bool flag = false;
		foreach (Tag tag in worldInventory.pinnedResources)
		{
			if (!this.rows.ContainsKey(tag))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			foreach (KeyValuePair<Tag, float> keyValuePair in DiscoveredResources.Instance.newDiscoveries)
			{
				if (!this.rows.ContainsKey(keyValuePair.Key) && this.IsDisplayedTag(keyValuePair.Key))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			foreach (Tag tag2 in worldInventory.notifyResources)
			{
				if (!this.rows.ContainsKey(tag2))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.rows)
			{
				if (worldInventory.pinnedResources.Contains(keyValuePair2.Key) != keyValuePair2.Value.activeSelf)
				{
					flag = true;
					break;
				}
				if (worldInventory.notifyResources.Contains(keyValuePair2.Key) != keyValuePair2.Value.activeSelf)
				{
					flag = true;
					break;
				}
				if (DiscoveredResources.Instance.newDiscoveries.ContainsKey(keyValuePair2.Key) != keyValuePair2.Value.activeSelf)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			this.Populate(null);
		}
	}

	public void Refresh()
	{
		this.SyncRows();
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		bool flag = false;
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.rows)
		{
			if (keyValuePair.Value.activeSelf)
			{
				this.RefreshLine(keyValuePair.Key, worldInventory);
				flag = flag || DiscoveredResources.Instance.newDiscoveries.ContainsKey(keyValuePair.Key);
			}
		}
		this.clearNewButton.gameObject.SetActive(flag);
		this.seeAllButton.GetComponentInChildren<LocText>().SetText(string.Format(UI.RESOURCESCREEN.SEE_ALL, AllResourcesScreen.Instance.UniqueResourceRowCount()));
	}

	private void RefreshLine(Tag tag, WorldInventory inventory)
	{
		Tag tag2 = tag;
		HierarchyReferences component = this.rows[tag2].GetComponent<HierarchyReferences>();
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(tag2, "ui", false);
		component.GetReference<Image>("Icon").sprite = uisprite.first;
		component.GetReference<Image>("Icon").color = uisprite.second;
		component.GetReference<LocText>("NameLabel").SetText(tag2.ProperNameStripLink());
		if (!AllResourcesScreen.Instance.units.ContainsKey(tag))
		{
			AllResourcesScreen.Instance.units.Add(tag, GameUtil.MeasureUnit.quantity);
		}
		if (!inventory.HasValidCount)
		{
			component.GetReference<LocText>("ValueLabel").SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
		}
		else
		{
			switch (AllResourcesScreen.Instance.units[tag])
			{
			case GameUtil.MeasureUnit.mass:
				component.GetReference<LocText>("ValueLabel").SetText(GameUtil.GetFormattedMass(inventory.GetAmount(tag2, false), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				break;
			case GameUtil.MeasureUnit.kcal:
			{
				float num = RationTracker.Get().CountRationsByFoodType(tag.Name, ClusterManager.Instance.activeWorld.worldInventory, true);
				component.GetReference<LocText>("ValueLabel").SetText(GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true));
				break;
			}
			case GameUtil.MeasureUnit.quantity:
				component.GetReference<LocText>("ValueLabel").SetText(GameUtil.GetFormattedUnits(inventory.GetAmount(tag2, false), GameUtil.TimeSlice.None, true, ""));
				break;
			}
		}
		component.GetReference<MultiToggle>("PinToggle").onClick = delegate
		{
			inventory.pinnedResources.Remove(tag);
			this.SyncRows();
		};
		component.GetReference<MultiToggle>("NotifyToggle").onClick = delegate
		{
			inventory.notifyResources.Remove(tag);
			this.SyncRows();
		};
		component.GetReference("NewLabel").gameObject.SetActive(DiscoveredResources.Instance.newDiscoveries.ContainsKey(tag));
		component.GetReference("NewLabel").GetComponent<MultiToggle>().onClick = delegate
		{
			AllResourcesScreen.Instance.Show(!AllResourcesScreen.Instance.gameObject.activeSelf);
		};
	}

	public void Render1000ms(float dt)
	{
		if (this.headerButton != null && this.headerButton.CurrentState == 0)
		{
			return;
		}
		this.Refresh();
	}

	public GameObject linePrefab;

	public GameObject rowContainer;

	public MultiToggle headerButton;

	public MultiToggle clearNewButton;

	public KButton clearAllButton;

	public MultiToggle seeAllButton;

	private Dictionary<Tag, GameObject> rows = new Dictionary<Tag, GameObject>();

	public static PinnedResourcesPanel Instance;

	private int clickIdx;
}
