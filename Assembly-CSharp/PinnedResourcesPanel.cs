using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class PinnedResourcesPanel : KScreen, IRender1000ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.rowContainerLayout = this.rowContainer.GetComponent<QuickLayout>();
	}

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
			bool flag = !AllResourcesScreen.Instance.isHiddenButActive;
			AllResourcesScreen.Instance.Show(!flag);
		}));
		this.seeAllLabel = this.seeAllButton.GetComponentInChildren<LocText>();
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

	protected override void OnForcedCleanUp()
	{
		PinnedResourcesPanel.Instance = null;
		base.OnForcedCleanUp();
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
		foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> keyValuePair in this.rows)
		{
			if (keyValuePair.Value.gameObject.activeSelf && DiscoveredResources.Instance.newDiscoveries.ContainsKey(keyValuePair.Key))
			{
				DiscoveredResources.Instance.newDiscoveries.Remove(keyValuePair.Key);
			}
		}
	}

	private void UnPinAll()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> keyValuePair in this.rows)
		{
			worldInventory.pinnedResources.Remove(keyValuePair.Key);
		}
	}

	private PinnedResourcesPanel.PinnedResourceRow CreateRow(Tag tag)
	{
		PinnedResourcesPanel.PinnedResourceRow pinnedResourceRow = new PinnedResourcesPanel.PinnedResourceRow(tag);
		GameObject gameObject = Util.KInstantiateUI(this.linePrefab, this.rowContainer, false);
		pinnedResourceRow.gameObject = gameObject;
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		pinnedResourceRow.icon = component.GetReference<Image>("Icon");
		pinnedResourceRow.nameLabel = component.GetReference<LocText>("NameLabel");
		pinnedResourceRow.valueLabel = component.GetReference<LocText>("ValueLabel");
		pinnedResourceRow.pinToggle = component.GetReference<MultiToggle>("PinToggle");
		pinnedResourceRow.notifyToggle = component.GetReference<MultiToggle>("NotifyToggle");
		pinnedResourceRow.newLabel = component.GetReference<MultiToggle>("NewLabel");
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(tag, "ui", false);
		pinnedResourceRow.icon.sprite = uisprite.first;
		pinnedResourceRow.icon.color = uisprite.second;
		pinnedResourceRow.nameLabel.SetText(tag.ProperNameStripLink());
		MultiToggle component2 = pinnedResourceRow.gameObject.GetComponent<MultiToggle>();
		component2.onClick = (global::System.Action)Delegate.Combine(component2.onClick, new global::System.Action(delegate
		{
			List<Pickupable> list = ClusterManager.Instance.activeWorld.worldInventory.CreatePickupablesList(tag);
			if (list != null && list.Count > 0)
			{
				SelectTool.Instance.SelectAndFocus(list[this.clickIdx % list.Count].transform.position, list[this.clickIdx % list.Count].GetComponent<KSelectable>());
				this.clickIdx++;
				return;
			}
			this.clickIdx = 0;
		}));
		return pinnedResourceRow;
	}

	public void Populate(object data = null)
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		foreach (KeyValuePair<Tag, float> keyValuePair in DiscoveredResources.Instance.newDiscoveries)
		{
			if (!this.rows.ContainsKey(keyValuePair.Key) && this.IsDisplayedTag(keyValuePair.Key))
			{
				this.rows.Add(keyValuePair.Key, this.CreateRow(keyValuePair.Key));
			}
		}
		foreach (Tag tag in worldInventory.pinnedResources)
		{
			if (!this.rows.ContainsKey(tag))
			{
				this.rows.Add(tag, this.CreateRow(tag));
			}
		}
		foreach (Tag tag2 in worldInventory.notifyResources)
		{
			if (!this.rows.ContainsKey(tag2))
			{
				this.rows.Add(tag2, this.CreateRow(tag2));
			}
		}
		foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> keyValuePair2 in this.rows)
		{
			if (false || worldInventory.pinnedResources.Contains(keyValuePair2.Key) || worldInventory.notifyResources.Contains(keyValuePair2.Key) || (DiscoveredResources.Instance.newDiscoveries.ContainsKey(keyValuePair2.Key) && worldInventory.GetAmount(keyValuePair2.Key, false) > 0f))
			{
				if (!keyValuePair2.Value.gameObject.activeSelf)
				{
					keyValuePair2.Value.gameObject.SetActive(true);
				}
			}
			else if (keyValuePair2.Value.gameObject.activeSelf)
			{
				keyValuePair2.Value.gameObject.SetActive(false);
			}
		}
		foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> keyValuePair3 in this.rows)
		{
			keyValuePair3.Value.pinToggle.gameObject.SetActive(worldInventory.pinnedResources.Contains(keyValuePair3.Key));
		}
		this.SortRows();
		this.rowContainerLayout.ForceUpdate();
	}

	private void SortRows()
	{
		List<PinnedResourcesPanel.PinnedResourceRow> list = new List<PinnedResourcesPanel.PinnedResourceRow>();
		foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> keyValuePair in this.rows)
		{
			list.Add(keyValuePair.Value);
		}
		list.Sort((PinnedResourcesPanel.PinnedResourceRow a, PinnedResourcesPanel.PinnedResourceRow b) => a.SortableNameWithoutLink.CompareTo(b.SortableNameWithoutLink));
		foreach (PinnedResourcesPanel.PinnedResourceRow pinnedResourceRow in list)
		{
			this.rows[pinnedResourceRow.Tag].gameObject.transform.SetAsLastSibling();
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
			foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> keyValuePair2 in this.rows)
			{
				if ((worldInventory.pinnedResources.Contains(keyValuePair2.Key) || worldInventory.notifyResources.Contains(keyValuePair2.Key) || (DiscoveredResources.Instance.newDiscoveries.ContainsKey(keyValuePair2.Key) && worldInventory.GetAmount(keyValuePair2.Key, false) > 0f)) != keyValuePair2.Value.gameObject.activeSelf)
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
		foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> keyValuePair in this.rows)
		{
			if (keyValuePair.Value.gameObject.activeSelf)
			{
				this.RefreshLine(keyValuePair.Key, worldInventory, false);
				flag = flag || DiscoveredResources.Instance.newDiscoveries.ContainsKey(keyValuePair.Key);
			}
		}
		this.clearNewButton.gameObject.SetActive(flag);
		this.seeAllLabel.SetText(string.Format(UI.RESOURCESCREEN.SEE_ALL, AllResourcesScreen.Instance.UniqueResourceRowCount()));
	}

	private void RefreshLine(Tag tag, WorldInventory inventory, bool initialConfig = false)
	{
		Tag tag2 = tag;
		if (!AllResourcesScreen.Instance.units.ContainsKey(tag))
		{
			AllResourcesScreen.Instance.units.Add(tag, GameUtil.MeasureUnit.quantity);
		}
		if (!inventory.HasValidCount)
		{
			this.rows[tag].valueLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
		}
		else
		{
			switch (AllResourcesScreen.Instance.units[tag])
			{
			case GameUtil.MeasureUnit.mass:
			{
				float amount = inventory.GetAmount(tag2, false);
				if (this.rows[tag].CheckAmountChanged(amount, true))
				{
					this.rows[tag].valueLabel.SetText(GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				}
				break;
			}
			case GameUtil.MeasureUnit.kcal:
			{
				float num = RationTracker.Get().CountRationsByFoodType(tag.Name, ClusterManager.Instance.activeWorld.worldInventory, true);
				if (this.rows[tag].CheckAmountChanged(num, true))
				{
					this.rows[tag].valueLabel.SetText(GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true));
				}
				break;
			}
			case GameUtil.MeasureUnit.quantity:
			{
				float amount2 = inventory.GetAmount(tag2, false);
				if (this.rows[tag].CheckAmountChanged(amount2, true))
				{
					this.rows[tag].valueLabel.SetText(GameUtil.GetFormattedUnits(amount2, GameUtil.TimeSlice.None, true, ""));
				}
				break;
			}
			}
		}
		this.rows[tag].pinToggle.onClick = delegate
		{
			inventory.pinnedResources.Remove(tag);
			this.SyncRows();
		};
		this.rows[tag].notifyToggle.onClick = delegate
		{
			inventory.notifyResources.Remove(tag);
			this.SyncRows();
		};
		this.rows[tag].newLabel.gameObject.SetActive(DiscoveredResources.Instance.newDiscoveries.ContainsKey(tag));
		this.rows[tag].newLabel.onClick = delegate
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

	private LocText seeAllLabel;

	private QuickLayout rowContainerLayout;

	private Dictionary<Tag, PinnedResourcesPanel.PinnedResourceRow> rows = new Dictionary<Tag, PinnedResourcesPanel.PinnedResourceRow>();

	public static PinnedResourcesPanel Instance;

	private int clickIdx;

	public class PinnedResourceRow
	{
		public PinnedResourceRow(Tag tag)
		{
			this.Tag = tag;
			this.SortableNameWithoutLink = tag.ProperNameStripLink();
		}

		public Tag Tag { get; private set; }

		public string SortableNameWithoutLink { get; private set; }

		public bool CheckAmountChanged(float newResourceAmount, bool updateIfTrue)
		{
			bool flag = newResourceAmount != this.oldResourceAmount;
			if (flag && updateIfTrue)
			{
				this.oldResourceAmount = newResourceAmount;
			}
			return flag;
		}

		public GameObject gameObject;

		public Image icon;

		public LocText nameLabel;

		public LocText valueLabel;

		public MultiToggle pinToggle;

		public MultiToggle notifyToggle;

		public MultiToggle newLabel;

		private float oldResourceAmount = -1f;
	}
}
