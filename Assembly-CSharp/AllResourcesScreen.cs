using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class AllResourcesScreen : ShowOptimizedKScreen, ISim4000ms, ISim1000ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		AllResourcesScreen.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.ConsumeMouseScroll = true;
		this.Init();
	}

	protected override void OnForcedCleanUp()
	{
		AllResourcesScreen.Instance = null;
		base.OnForcedCleanUp();
	}

	public void SetFilter(string filter)
	{
		if (string.IsNullOrEmpty(filter))
		{
			filter = "";
		}
		this.searchInputField.text = filter;
	}

	public void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		this.Populate(null);
		Game.Instance.Subscribe(1983128072, new Action<object>(this.Populate));
		DiscoveredResources.Instance.OnDiscover += delegate(Tag a, Tag b)
		{
			this.Populate(null);
		};
		this.closeButton.onClick += delegate
		{
			this.Show(false);
		};
		this.clearSearchButton.onClick += delegate
		{
			this.searchInputField.text = "";
		};
		this.searchInputField.OnValueChangesPaused = delegate
		{
			this.SearchFilter(this.searchInputField.text);
		};
		KInputTextField kinputTextField = this.searchInputField;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(delegate
		{
			base.isEditing = true;
			UISounds.PlaySound(UISounds.Sound.Find);
		}));
		this.searchInputField.onEndEdit.AddListener(delegate(string value)
		{
			base.isEditing = false;
		});
		this.Show(false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			ManagementMenu.Instance.CloseAll();
			AllDiagnosticsScreen.Instance.Show(false);
			this.RefreshRows();
			return;
		}
		this.SetFilter(null);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			this.Show(false);
			e.Consumed = true;
		}
		if (base.isEditing)
		{
			e.Consumed = true;
			return;
		}
		base.OnKeyDown(e);
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			this.Show(false);
			e.Consumed = true;
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	public override float GetSortKey()
	{
		return 50f;
	}

	public void Populate(object data = null)
	{
		this.SpawnRows();
	}

	private void SpawnRows()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		this.allowDisplayCategories.Add(GameTags.MaterialCategories);
		this.allowDisplayCategories.Add(GameTags.CalorieCategories);
		this.allowDisplayCategories.Add(GameTags.UnitCategories);
		foreach (Tag tag in GameTags.MaterialCategories)
		{
			this.SpawnCategoryRow(tag, GameUtil.MeasureUnit.mass);
		}
		foreach (Tag tag2 in GameTags.CalorieCategories)
		{
			this.SpawnCategoryRow(tag2, GameUtil.MeasureUnit.kcal);
		}
		foreach (Tag tag3 in GameTags.UnitCategories)
		{
			this.SpawnCategoryRow(tag3, GameUtil.MeasureUnit.quantity);
		}
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> keyValuePair in this.categoryRows)
		{
			list.Add(keyValuePair.Key);
		}
		list.Sort((Tag a, Tag b) => a.ProperNameStripLink().CompareTo(b.ProperNameStripLink()));
		foreach (Tag tag4 in list)
		{
			this.categoryRows[tag4].GameObject.transform.SetAsLastSibling();
		}
	}

	private void SpawnCategoryRow(Tag categoryTag, GameUtil.MeasureUnit unit)
	{
		if (!this.categoryRows.ContainsKey(categoryTag))
		{
			GameObject gameObject = Util.KInstantiateUI(this.categoryLinePrefab, this.rootListContainer, true);
			AllResourcesScreen.CategoryRow categoryRow = new AllResourcesScreen.CategoryRow(categoryTag, gameObject);
			gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(categoryTag.ProperNameStripLink());
			this.categoryRows.Add(categoryTag, categoryRow);
			this.currentlyDisplayedRows.Add(categoryTag, true);
			this.units.Add(categoryTag, unit);
			GraphBase component = categoryRow.sparkLayer.GetComponent<GraphBase>();
			component.axis_x.min_value = 0f;
			component.axis_x.max_value = 600f;
			component.axis_x.guide_frequency = 120f;
			component.RefreshGuides();
		}
		GameObject container = this.categoryRows[categoryTag].FoldOutPanel.container;
		foreach (Tag tag in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(categoryTag))
		{
			if (!this.resourceRows.ContainsKey(tag))
			{
				GameObject gameObject2 = Util.KInstantiateUI(this.resourceLinePrefab, container, true);
				HierarchyReferences component2 = gameObject2.GetComponent<HierarchyReferences>();
				global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(tag, "ui", false);
				component2.GetReference<Image>("Icon").sprite = uisprite.first;
				component2.GetReference<Image>("Icon").color = uisprite.second;
				component2.GetReference<LocText>("NameLabel").SetText(tag.ProperNameStripLink());
				Tag targetTag = tag;
				MultiToggle pinToggle = component2.GetReference<MultiToggle>("PinToggle");
				MultiToggle pinToggle2 = pinToggle;
				pinToggle2.onClick = (global::System.Action)Delegate.Combine(pinToggle2.onClick, new global::System.Action(delegate
				{
					if (ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(targetTag))
					{
						ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Remove(targetTag);
					}
					else
					{
						ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Add(targetTag);
						if (DiscoveredResources.Instance.newDiscoveries.ContainsKey(targetTag))
						{
							DiscoveredResources.Instance.newDiscoveries.Remove(targetTag);
						}
					}
					this.RefreshPinnedState(targetTag);
					pinToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(targetTag) ? 1 : 0);
				}));
				gameObject2.GetComponent<MultiToggle>().onClick = pinToggle.onClick;
				MultiToggle notifyToggle = component2.GetReference<MultiToggle>("NotificationToggle");
				MultiToggle notifyToggle2 = notifyToggle;
				notifyToggle2.onClick = (global::System.Action)Delegate.Combine(notifyToggle2.onClick, new global::System.Action(delegate
				{
					if (ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(targetTag))
					{
						ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Remove(targetTag);
					}
					else
					{
						ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Add(targetTag);
					}
					this.RefreshPinnedState(targetTag);
					notifyToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(targetTag) ? 1 : 0);
				}));
				component2.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.min_value = 0f;
				component2.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.max_value = 600f;
				component2.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.guide_frequency = 120f;
				component2.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().RefreshGuides();
				AllResourcesScreen.ResourceRow resourceRow = new AllResourcesScreen.ResourceRow(tag, gameObject2);
				this.resourceRows.Add(tag, resourceRow);
				this.currentlyDisplayedRows.Add(tag, true);
				if (this.units.ContainsKey(tag))
				{
					global::Debug.LogError(string.Concat(new string[]
					{
						"Trying to add ",
						tag.ToString(),
						":UnitType ",
						this.units[tag].ToString(),
						" but units dictionary already has key ",
						tag.ToString(),
						" with unit type:",
						unit.ToString()
					}));
				}
				else
				{
					this.units.Add(tag, unit);
				}
			}
		}
	}

	private void FilterRowBySearch(Tag tag, string filter)
	{
		this.currentlyDisplayedRows[tag] = this.PassesSearchFilter(tag, filter);
	}

	private void SearchFilter(string search)
	{
		foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> keyValuePair in this.resourceRows)
		{
			this.FilterRowBySearch(keyValuePair.Key, search);
		}
		foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> keyValuePair2 in this.categoryRows)
		{
			if (this.PassesSearchFilter(keyValuePair2.Key, search))
			{
				this.currentlyDisplayedRows[keyValuePair2.Key] = true;
				using (HashSet<Tag>.Enumerator enumerator3 = DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(keyValuePair2.Key).GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Tag tag = enumerator3.Current;
						if (this.currentlyDisplayedRows.ContainsKey(tag))
						{
							this.currentlyDisplayedRows[tag] = true;
						}
					}
					continue;
				}
			}
			this.currentlyDisplayedRows[keyValuePair2.Key] = false;
		}
		this.EnableCategoriesByActiveChildren();
		this.SetRowsActive();
	}

	private bool PassesSearchFilter(Tag tag, string filter)
	{
		filter = filter.ToUpper();
		string text = tag.ProperNameStripLink().ToUpper();
		return !(filter != "") || text.Contains(filter);
	}

	private void EnableCategoriesByActiveChildren()
	{
		foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> keyValuePair in this.categoryRows)
		{
			if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(keyValuePair.Key).Count == 0)
			{
				this.currentlyDisplayedRows[keyValuePair.Key] = false;
			}
			else
			{
				GameObject container = keyValuePair.Value.GameObject.GetComponent<FoldOutPanel>().container;
				foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> keyValuePair2 in this.resourceRows)
				{
					if (!(keyValuePair2.Value.GameObject.transform.parent.gameObject != container))
					{
						this.currentlyDisplayedRows[keyValuePair.Key] = this.currentlyDisplayedRows[keyValuePair.Key] || this.currentlyDisplayedRows[keyValuePair2.Key];
					}
				}
			}
		}
	}

	private void RefreshPinnedState(Tag tag)
	{
		this.resourceRows[tag].notificiationToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(tag) ? 1 : 0);
		this.resourceRows[tag].pinToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(tag) ? 1 : 0);
	}

	public void RefreshRows()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		this.EnableCategoriesByActiveChildren();
		this.SetRowsActive();
		if (this.allowRefresh)
		{
			foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> keyValuePair in this.categoryRows)
			{
				if (keyValuePair.Value.GameObject.activeInHierarchy)
				{
					float amount = worldInventory.GetAmount(keyValuePair.Key, false);
					float totalAmount = worldInventory.GetTotalAmount(keyValuePair.Key, false);
					if (!worldInventory.HasValidCount)
					{
						keyValuePair.Value.availableLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
						keyValuePair.Value.totalLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
						keyValuePair.Value.reservedLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
					}
					else
					{
						switch (this.units[keyValuePair.Key])
						{
						case GameUtil.MeasureUnit.mass:
							if (keyValuePair.Value.CheckAvailableAmountChanged(amount, true))
							{
								keyValuePair.Value.availableLabel.SetText(GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							}
							if (keyValuePair.Value.CheckTotalResourceAmountChanged(totalAmount, true))
							{
								keyValuePair.Value.totalLabel.SetText(GameUtil.GetFormattedMass(totalAmount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							}
							if (keyValuePair.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
							{
								keyValuePair.Value.reservedLabel.SetText(GameUtil.GetFormattedMass(totalAmount - amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							}
							break;
						case GameUtil.MeasureUnit.kcal:
						{
							float num = WorldResourceAmountTracker<RationTracker>.Get().CountAmount(null, ClusterManager.Instance.activeWorld.worldInventory, true);
							if (keyValuePair.Value.CheckAvailableAmountChanged(amount, true))
							{
								keyValuePair.Value.availableLabel.SetText(GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true));
							}
							if (keyValuePair.Value.CheckTotalResourceAmountChanged(totalAmount, true))
							{
								keyValuePair.Value.totalLabel.SetText(GameUtil.GetFormattedCalories(totalAmount, GameUtil.TimeSlice.None, true));
							}
							if (keyValuePair.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
							{
								keyValuePair.Value.reservedLabel.SetText(GameUtil.GetFormattedCalories(totalAmount - amount, GameUtil.TimeSlice.None, true));
							}
							break;
						}
						case GameUtil.MeasureUnit.quantity:
							if (keyValuePair.Value.CheckAvailableAmountChanged(amount, true))
							{
								keyValuePair.Value.availableLabel.SetText(GameUtil.GetFormattedUnits(amount, GameUtil.TimeSlice.None, true, ""));
							}
							if (keyValuePair.Value.CheckTotalResourceAmountChanged(totalAmount, true))
							{
								keyValuePair.Value.totalLabel.SetText(GameUtil.GetFormattedUnits(totalAmount, GameUtil.TimeSlice.None, true, ""));
							}
							if (keyValuePair.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
							{
								keyValuePair.Value.reservedLabel.SetText(GameUtil.GetFormattedUnits(totalAmount - amount, GameUtil.TimeSlice.None, true, ""));
							}
							break;
						}
					}
				}
			}
			foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> keyValuePair2 in this.resourceRows)
			{
				if (keyValuePair2.Value.GameObject.activeInHierarchy)
				{
					float amount2 = worldInventory.GetAmount(keyValuePair2.Key, false);
					float totalAmount2 = worldInventory.GetTotalAmount(keyValuePair2.Key, false);
					if (!worldInventory.HasValidCount)
					{
						keyValuePair2.Value.availableLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
						keyValuePair2.Value.totalLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
						keyValuePair2.Value.reservedLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
					}
					else
					{
						switch (this.units[keyValuePair2.Key])
						{
						case GameUtil.MeasureUnit.mass:
							if (keyValuePair2.Value.CheckAvailableAmountChanged(amount2, true))
							{
								keyValuePair2.Value.availableLabel.SetText(GameUtil.GetFormattedMass(amount2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							}
							if (keyValuePair2.Value.CheckTotalResourceAmountChanged(totalAmount2, true))
							{
								keyValuePair2.Value.totalLabel.SetText(GameUtil.GetFormattedMass(totalAmount2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							}
							if (keyValuePair2.Value.CheckReservedResourceAmountChanged(totalAmount2 - amount2, true))
							{
								keyValuePair2.Value.reservedLabel.SetText(GameUtil.GetFormattedMass(totalAmount2 - amount2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							}
							break;
						case GameUtil.MeasureUnit.kcal:
						{
							float num2 = WorldResourceAmountTracker<RationTracker>.Get().CountAmountForItemWithID(keyValuePair2.Key.Name, ClusterManager.Instance.activeWorld.worldInventory, true);
							if (keyValuePair2.Value.CheckAvailableAmountChanged(num2, true))
							{
								keyValuePair2.Value.availableLabel.SetText(GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true));
							}
							if (keyValuePair2.Value.CheckTotalResourceAmountChanged(totalAmount2, true))
							{
								keyValuePair2.Value.totalLabel.SetText(GameUtil.GetFormattedCalories(totalAmount2, GameUtil.TimeSlice.None, true));
							}
							if (keyValuePair2.Value.CheckReservedResourceAmountChanged(totalAmount2 - amount2, true))
							{
								keyValuePair2.Value.reservedLabel.SetText(GameUtil.GetFormattedCalories(totalAmount2 - amount2, GameUtil.TimeSlice.None, true));
							}
							break;
						}
						case GameUtil.MeasureUnit.quantity:
							if (keyValuePair2.Value.CheckAvailableAmountChanged(amount2, true))
							{
								keyValuePair2.Value.availableLabel.SetText(GameUtil.GetFormattedUnits(amount2, GameUtil.TimeSlice.None, true, ""));
							}
							if (keyValuePair2.Value.CheckTotalResourceAmountChanged(totalAmount2, true))
							{
								keyValuePair2.Value.totalLabel.SetText(GameUtil.GetFormattedUnits(totalAmount2, GameUtil.TimeSlice.None, true, ""));
							}
							if (keyValuePair2.Value.CheckReservedResourceAmountChanged(totalAmount2 - amount2, true))
							{
								keyValuePair2.Value.reservedLabel.SetText(GameUtil.GetFormattedUnits(totalAmount2 - amount2, GameUtil.TimeSlice.None, true, ""));
							}
							break;
						}
					}
					this.RefreshPinnedState(keyValuePair2.Key);
				}
			}
		}
	}

	public int UniqueResourceRowCount()
	{
		return this.resourceRows.Count;
	}

	private void RefreshCharts()
	{
		float time = GameClock.Instance.GetTime();
		float num = 3000f;
		foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> keyValuePair in this.categoryRows)
		{
			ResourceTracker resourceStatistic = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, keyValuePair.Key);
			if (resourceStatistic != null)
			{
				SparkLayer sparkLayer = keyValuePair.Value.sparkLayer;
				global::Tuple<float, float>[] array = resourceStatistic.ChartableData(num);
				if (array.Length != 0)
				{
					sparkLayer.graph.axis_x.max_value = array[array.Length - 1].first;
				}
				else
				{
					sparkLayer.graph.axis_x.max_value = 0f;
				}
				sparkLayer.graph.axis_x.min_value = time - num;
				sparkLayer.RefreshLine(array, "resourceAmount");
			}
			else
			{
				DebugUtil.DevLogError("DevError: No tracker found for resource category " + keyValuePair.Key.ToString());
			}
		}
		foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> keyValuePair2 in this.resourceRows)
		{
			if (keyValuePair2.Value.GameObject.activeInHierarchy)
			{
				ResourceTracker resourceStatistic2 = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, keyValuePair2.Key);
				if (resourceStatistic2 != null)
				{
					SparkLayer sparkLayer2 = keyValuePair2.Value.sparkLayer;
					global::Tuple<float, float>[] array2 = resourceStatistic2.ChartableData(num);
					if (array2.Length != 0)
					{
						sparkLayer2.graph.axis_x.max_value = array2[array2.Length - 1].first;
					}
					else
					{
						sparkLayer2.graph.axis_x.max_value = 0f;
					}
					sparkLayer2.graph.axis_x.min_value = time - num;
					sparkLayer2.RefreshLine(array2, "resourceAmount");
				}
				else
				{
					DebugUtil.DevLogError("DevError: No tracker found for resource " + keyValuePair2.Key.ToString());
				}
			}
		}
	}

	private void SetRowsActive()
	{
		foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> keyValuePair in this.categoryRows)
		{
			if (keyValuePair.Value.GameObject.activeSelf != this.currentlyDisplayedRows[keyValuePair.Key])
			{
				keyValuePair.Value.GameObject.SetActive(this.currentlyDisplayedRows[keyValuePair.Key]);
			}
		}
		foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> keyValuePair2 in this.resourceRows)
		{
			if (keyValuePair2.Value.GameObject.activeSelf != this.currentlyDisplayedRows[keyValuePair2.Key])
			{
				keyValuePair2.Value.GameObject.SetActive(this.currentlyDisplayedRows[keyValuePair2.Key]);
				if (!this.currentlyDisplayedRows[keyValuePair2.Key] && keyValuePair2.Value.horizontalLayoutGroup.enabled)
				{
					keyValuePair2.Value.horizontalLayoutGroup.enabled = false;
				}
			}
		}
	}

	public void Sim4000ms(float dt)
	{
		if (!this.IsScreenActive())
		{
			return;
		}
		this.RefreshCharts();
	}

	public void Sim1000ms(float dt)
	{
		if (!this.IsScreenActive())
		{
			return;
		}
		this.RefreshRows();
	}

	private Dictionary<Tag, AllResourcesScreen.ResourceRow> resourceRows = new Dictionary<Tag, AllResourcesScreen.ResourceRow>();

	private Dictionary<Tag, AllResourcesScreen.CategoryRow> categoryRows = new Dictionary<Tag, AllResourcesScreen.CategoryRow>();

	public Dictionary<Tag, GameUtil.MeasureUnit> units = new Dictionary<Tag, GameUtil.MeasureUnit>();

	public GameObject rootListContainer;

	public GameObject resourceLinePrefab;

	public GameObject categoryLinePrefab;

	public KButton closeButton;

	public bool allowRefresh = true;

	[SerializeField]
	private KInputTextField searchInputField;

	[SerializeField]
	private KButton clearSearchButton;

	public static AllResourcesScreen Instance;

	public Dictionary<Tag, bool> currentlyDisplayedRows = new Dictionary<Tag, bool>();

	public List<TagSet> allowDisplayCategories = new List<TagSet>();

	private bool initialized;

	private class ScreenRowBase
	{
		public ScreenRowBase(Tag tag, GameObject gameObject)
		{
			this.Tag = tag;
			this.GameObject = gameObject;
			HierarchyReferences component = this.GameObject.GetComponent<HierarchyReferences>();
			this.availableLabel = component.GetReference<LocText>("AvailableLabel");
			this.totalLabel = component.GetReference<LocText>("TotalLabel");
			this.reservedLabel = component.GetReference<LocText>("ReservedLabel");
			this.sparkLayer = component.GetReference<SparkLayer>("Chart");
		}

		public Tag Tag { get; private set; }

		public GameObject GameObject { get; private set; }

		public bool CheckAvailableAmountChanged(float newAvailableResourceAmount, bool updateIfTrue)
		{
			bool flag = newAvailableResourceAmount != this.oldAvailableResourceAmount;
			if (flag && updateIfTrue)
			{
				this.oldAvailableResourceAmount = newAvailableResourceAmount;
			}
			return flag;
		}

		public bool CheckTotalResourceAmountChanged(float newTotalResourceAmount, bool updateIfTrue)
		{
			bool flag = newTotalResourceAmount != this.oldTotalResourceAmount;
			if (flag && updateIfTrue)
			{
				this.oldTotalResourceAmount = newTotalResourceAmount;
			}
			return flag;
		}

		public bool CheckReservedResourceAmountChanged(float newReservedResourceAmount, bool updateIfTrue)
		{
			bool flag = newReservedResourceAmount != this.oldReserverResourceAmount;
			if (flag && updateIfTrue)
			{
				this.oldReserverResourceAmount = newReservedResourceAmount;
			}
			return flag;
		}

		public LocText availableLabel;

		public LocText totalLabel;

		public LocText reservedLabel;

		public SparkLayer sparkLayer;

		private float oldAvailableResourceAmount = -1f;

		private float oldTotalResourceAmount = -1f;

		private float oldReserverResourceAmount = -1f;
	}

	private class CategoryRow : AllResourcesScreen.ScreenRowBase
	{
		public CategoryRow(Tag tag, GameObject gameObject)
			: base(tag, gameObject)
		{
			this.FoldOutPanel = base.GameObject.GetComponent<FoldOutPanel>();
		}

		public FoldOutPanel FoldOutPanel { get; private set; }
	}

	private class ResourceRow : AllResourcesScreen.ScreenRowBase
	{
		public ResourceRow(Tag tag, GameObject gameObject)
			: base(tag, gameObject)
		{
			HierarchyReferences component = base.GameObject.GetComponent<HierarchyReferences>();
			this.notificiationToggle = component.GetReference<MultiToggle>("NotificationToggle");
			this.pinToggle = component.GetReference<MultiToggle>("PinToggle");
			this.horizontalLayoutGroup = gameObject.GetComponent<HorizontalLayoutGroup>();
		}

		public MultiToggle notificiationToggle;

		public MultiToggle pinToggle;

		public HorizontalLayoutGroup horizontalLayoutGroup;
	}
}
