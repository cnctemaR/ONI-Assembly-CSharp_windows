using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class AllResourcesScreen : KScreen, ISim4000ms, ISim1000ms
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
		this.searchInputField.onValueChanged.AddListener(delegate(string value)
		{
			this.SearchFilter(value);
		});
		KInputTextField kinputTextField = this.searchInputField;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(delegate
		{
			base.isEditing = true;
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
		}
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
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.categoryRows)
		{
			list.Add(keyValuePair.Key);
		}
		list.Sort((Tag a, Tag b) => a.ProperNameStripLink().CompareTo(b.ProperNameStripLink()));
		foreach (Tag tag4 in list)
		{
			this.categoryRows[tag4].transform.SetAsLastSibling();
		}
	}

	private void SpawnCategoryRow(Tag categoryTag, GameUtil.MeasureUnit unit)
	{
		if (!this.categoryRows.ContainsKey(categoryTag))
		{
			GameObject gameObject = Util.KInstantiateUI(this.categoryLinePrefab, this.rootListContainer, true);
			gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(categoryTag.ProperNameStripLink());
			this.categoryRows.Add(categoryTag, gameObject);
			this.currentlyDisplayedRows.Add(categoryTag, true);
			this.units.Add(categoryTag, unit);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.min_value = 0f;
			component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.max_value = 600f;
			component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.guide_frequency = 120f;
			component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().RefreshGuides();
		}
		GameObject container = this.categoryRows[categoryTag].GetComponent<FoldOutPanel>().container;
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
				this.resourceRows.Add(tag, gameObject2);
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
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.resourceRows)
		{
			this.FilterRowBySearch(keyValuePair.Key, search);
		}
		foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.categoryRows)
		{
			if (this.PassesSearchFilter(keyValuePair2.Key, search))
			{
				this.currentlyDisplayedRows[keyValuePair2.Key] = true;
				using (HashSet<Tag>.Enumerator enumerator2 = DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(keyValuePair2.Key).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Tag tag = enumerator2.Current;
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
		string text = tag.ProperName().ToUpper();
		return !(filter != "") || text.Contains(filter) || tag.Name.ToUpper().Contains(filter);
	}

	private void EnableCategoriesByActiveChildren()
	{
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.categoryRows)
		{
			if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(keyValuePair.Key).Count == 0)
			{
				this.currentlyDisplayedRows[keyValuePair.Key] = false;
			}
			else
			{
				GameObject container = keyValuePair.Value.GetComponent<FoldOutPanel>().container;
				foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.resourceRows)
				{
					if (!(keyValuePair2.Value.transform.parent.gameObject != container))
					{
						this.currentlyDisplayedRows[keyValuePair.Key] = this.currentlyDisplayedRows[keyValuePair.Key] || this.currentlyDisplayedRows[keyValuePair2.Key];
					}
				}
			}
		}
	}

	private void RefreshPinnedState(Tag tag)
	{
		this.resourceRows[tag].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("NotificationToggle").ChangeState(ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(tag) ? 1 : 0);
		this.resourceRows[tag].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("PinToggle").ChangeState(ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(tag) ? 1 : 0);
	}

	public void RefreshRows()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		if (this.allowRefresh)
		{
			foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.categoryRows)
			{
				HierarchyReferences component = keyValuePair.Value.GetComponent<HierarchyReferences>();
				float amount = worldInventory.GetAmount(keyValuePair.Key, false);
				float totalAmount = worldInventory.GetTotalAmount(keyValuePair.Key, false);
				if (!worldInventory.HasValidCount)
				{
					component.GetReference<LocText>("AvailableLabel").SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
					component.GetReference<LocText>("TotalLabel").SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
					component.GetReference<LocText>("ReservedLabel").SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
				}
				else
				{
					switch (this.units[keyValuePair.Key])
					{
					case GameUtil.MeasureUnit.mass:
						component.GetReference<LocText>("AvailableLabel").SetText(GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						component.GetReference<LocText>("TotalLabel").SetText(GameUtil.GetFormattedMass(totalAmount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						component.GetReference<LocText>("ReservedLabel").SetText(GameUtil.GetFormattedMass(totalAmount - amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						break;
					case GameUtil.MeasureUnit.kcal:
					{
						float num = RationTracker.Get().CountRations(null, ClusterManager.Instance.activeWorld.worldInventory, true);
						component.GetReference<LocText>("AvailableLabel").SetText(GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true));
						component.GetReference<LocText>("TotalLabel").SetText(GameUtil.GetFormattedCalories(totalAmount, GameUtil.TimeSlice.None, true));
						component.GetReference<LocText>("ReservedLabel").SetText(GameUtil.GetFormattedCalories(totalAmount - amount, GameUtil.TimeSlice.None, true));
						break;
					}
					case GameUtil.MeasureUnit.quantity:
						component.GetReference<LocText>("AvailableLabel").SetText(GameUtil.GetFormattedUnits(amount, GameUtil.TimeSlice.None, true, ""));
						component.GetReference<LocText>("TotalLabel").SetText(GameUtil.GetFormattedUnits(totalAmount, GameUtil.TimeSlice.None, true, ""));
						component.GetReference<LocText>("ReservedLabel").SetText(GameUtil.GetFormattedUnits(totalAmount - amount, GameUtil.TimeSlice.None, true, ""));
						break;
					}
				}
			}
			foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.resourceRows)
			{
				HierarchyReferences component2 = keyValuePair2.Value.GetComponent<HierarchyReferences>();
				float amount2 = worldInventory.GetAmount(keyValuePair2.Key, false);
				float totalAmount2 = worldInventory.GetTotalAmount(keyValuePair2.Key, false);
				if (!worldInventory.HasValidCount)
				{
					component2.GetReference<LocText>("AvailableLabel").SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
					component2.GetReference<LocText>("TotalLabel").SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
					component2.GetReference<LocText>("ReservedLabel").SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
				}
				else
				{
					switch (this.units[keyValuePair2.Key])
					{
					case GameUtil.MeasureUnit.mass:
						component2.GetReference<LocText>("AvailableLabel").SetText(GameUtil.GetFormattedMass(amount2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						component2.GetReference<LocText>("TotalLabel").SetText(GameUtil.GetFormattedMass(totalAmount2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						component2.GetReference<LocText>("ReservedLabel").SetText(GameUtil.GetFormattedMass(totalAmount2 - amount2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						break;
					case GameUtil.MeasureUnit.kcal:
					{
						float num2 = RationTracker.Get().CountRationsByFoodType(keyValuePair2.Key.Name, ClusterManager.Instance.activeWorld.worldInventory, true);
						component2.GetReference<LocText>("AvailableLabel").SetText(GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true));
						component2.GetReference<LocText>("TotalLabel").SetText(GameUtil.GetFormattedCalories(totalAmount2, GameUtil.TimeSlice.None, true));
						component2.GetReference<LocText>("ReservedLabel").SetText(GameUtil.GetFormattedCalories(totalAmount2 - amount2, GameUtil.TimeSlice.None, true));
						break;
					}
					case GameUtil.MeasureUnit.quantity:
						component2.GetReference<LocText>("AvailableLabel").SetText(GameUtil.GetFormattedUnits(amount2, GameUtil.TimeSlice.None, true, ""));
						component2.GetReference<LocText>("TotalLabel").SetText(GameUtil.GetFormattedUnits(totalAmount2, GameUtil.TimeSlice.None, true, ""));
						component2.GetReference<LocText>("ReservedLabel").SetText(GameUtil.GetFormattedUnits(totalAmount2 - amount2, GameUtil.TimeSlice.None, true, ""));
						break;
					}
				}
				this.RefreshPinnedState(keyValuePair2.Key);
			}
		}
		this.EnableCategoriesByActiveChildren();
		this.SetRowsActive();
	}

	public int UniqueResourceRowCount()
	{
		return this.resourceRows.Count;
	}

	private void RefreshCharts()
	{
		float time = GameClock.Instance.GetTime();
		float num = 3000f;
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.categoryRows)
		{
			HierarchyReferences hierarchyReferences = keyValuePair.Value.GetComponent<HierarchyReferences>();
			ResourceTracker resourceStatistic = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, keyValuePair.Key);
			if (resourceStatistic != null)
			{
				SparkLayer reference = hierarchyReferences.GetReference<SparkLayer>("Chart");
				global::Tuple<float, float>[] array = resourceStatistic.ChartableData(num);
				if (array.Length != 0)
				{
					reference.graph.axis_x.max_value = array[array.Length - 1].first;
				}
				else
				{
					reference.graph.axis_x.max_value = 0f;
				}
				reference.graph.axis_x.min_value = time - num;
				reference.RefreshLine(array, "resourceAmount");
			}
			else
			{
				DebugUtil.DevLogError("DevError: No tracker found for resource category " + keyValuePair.Key.ToString());
			}
		}
		foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.resourceRows)
		{
			HierarchyReferences hierarchyReferences = keyValuePair2.Value.GetComponent<HierarchyReferences>();
			ResourceTracker resourceStatistic2 = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, keyValuePair2.Key);
			if (resourceStatistic2 != null)
			{
				SparkLayer reference2 = hierarchyReferences.GetReference<SparkLayer>("Chart");
				global::Tuple<float, float>[] array2 = resourceStatistic2.ChartableData(num);
				if (array2.Length != 0)
				{
					reference2.graph.axis_x.max_value = array2[array2.Length - 1].first;
				}
				else
				{
					reference2.graph.axis_x.max_value = 0f;
				}
				reference2.graph.axis_x.min_value = time - num;
				reference2.RefreshLine(array2, "resourceAmount");
			}
			else
			{
				DebugUtil.DevLogError("DevError: No tracker found for resource " + keyValuePair2.Key.ToString());
			}
		}
	}

	private void SetRowsActive()
	{
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.categoryRows)
		{
			if (keyValuePair.Value.activeSelf != this.currentlyDisplayedRows[keyValuePair.Key])
			{
				keyValuePair.Value.SetActive(this.currentlyDisplayedRows[keyValuePair.Key]);
			}
		}
		foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.resourceRows)
		{
			if (keyValuePair2.Value.activeSelf != this.currentlyDisplayedRows[keyValuePair2.Key])
			{
				keyValuePair2.Value.SetActive(this.currentlyDisplayedRows[keyValuePair2.Key]);
			}
		}
	}

	public void Sim4000ms(float dt)
	{
		this.RefreshCharts();
	}

	public void Sim1000ms(float dt)
	{
		this.RefreshRows();
	}

	private Dictionary<Tag, GameObject> resourceRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, GameObject> categoryRows = new Dictionary<Tag, GameObject>();

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
}
