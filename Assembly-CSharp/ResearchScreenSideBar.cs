using System;
using System.Collections.Generic;
using Database;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchScreenSideBar : KScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.PopulateProjects();
		this.PopulateFilterButtons();
		this.RefreshCategoriesContentExpanded();
		this.RefreshWidgets();
		this.searchBox.OnValueChangesPaused = delegate
		{
			this.SetTextFilter(this.searchBox.text, false);
		};
		KInputTextField kinputTextField = this.searchBox;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(delegate
		{
			base.isEditing = true;
		}));
		this.searchBox.onEndEdit.AddListener(delegate(string value)
		{
			base.isEditing = false;
		});
		this.clearSearchButton.onClick += delegate
		{
			this.ResetFilter();
		};
		this.ConfigCompletionFilters();
		base.ConsumeMouseScroll = true;
		Game.Instance.Subscribe(-107300940, new Action<object>(this.UpdateProjectFilter));
	}

	private void Update()
	{
		for (int i = 0; i < Math.Min(this.QueuedActivations.Count, this.activationPerFrame); i++)
		{
			this.QueuedActivations[i].SetActive(true);
		}
		this.QueuedActivations.RemoveRange(0, Math.Min(this.QueuedActivations.Count, this.activationPerFrame));
		for (int j = 0; j < Math.Min(this.QueuedDeactivations.Count, this.activationPerFrame); j++)
		{
			this.QueuedDeactivations[j].SetActive(false);
		}
		this.QueuedDeactivations.RemoveRange(0, Math.Min(this.QueuedDeactivations.Count, this.activationPerFrame));
	}

	private void ConfigCompletionFilters()
	{
		MultiToggle multiToggle = this.allFilter;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.All, false);
		}));
		MultiToggle multiToggle2 = this.completedFilter;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.Completed, false);
		}));
		MultiToggle multiToggle3 = this.availableFilter;
		multiToggle3.onClick = (global::System.Action)Delegate.Combine(multiToggle3.onClick, new global::System.Action(delegate
		{
			this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.Available, false);
		}));
		this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.All, false);
	}

	private void SetCompletionFilter(ResearchScreenSideBar.CompletionState state, bool suppressUpdate)
	{
		this.completionFilter = state;
		this.allFilter.GetComponent<MultiToggle>().ChangeState((this.completionFilter == ResearchScreenSideBar.CompletionState.All) ? 1 : 0);
		this.completedFilter.GetComponent<MultiToggle>().ChangeState((this.completionFilter == ResearchScreenSideBar.CompletionState.Completed) ? 1 : 0);
		this.availableFilter.GetComponent<MultiToggle>().ChangeState((this.completionFilter == ResearchScreenSideBar.CompletionState.Available) ? 1 : 0);
		if (!suppressUpdate)
		{
			this.UpdateProjectFilter(null);
		}
	}

	public override float GetSortKey()
	{
		if (base.isEditing)
		{
			return 50f;
		}
		return 21f;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.researchScreen != null && this.researchScreen.canvas && !this.researchScreen.canvas.enabled)
		{
			return;
		}
		if (base.isEditing)
		{
			e.Consumed = true;
			return;
		}
		if (!e.Consumed)
		{
			Vector2 vector = base.transform.rectTransform().InverseTransformPoint(KInputManager.GetMousePos());
			if (vector.x >= 0f && vector.x <= base.transform.rectTransform().rect.width)
			{
				if (e.TryConsume(global::Action.MouseRight))
				{
					return;
				}
				if (e.TryConsume(global::Action.MouseLeft))
				{
					return;
				}
				if (!KInputManager.currentControllerIsGamepad)
				{
					if (e.TryConsume(global::Action.ZoomIn))
					{
						return;
					}
					e.TryConsume(global::Action.ZoomOut);
					return;
				}
			}
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		this.RefreshWidgets();
	}

	private void SetTextFilter(string newValue, bool suppressUpdate)
	{
		if (base.isEditing)
		{
			foreach (KeyValuePair<string, GameObject> keyValuePair in this.filterButtons)
			{
				this.filterStates[keyValuePair.Key] = false;
				keyValuePair.Value.GetComponent<MultiToggle>().ChangeState(0);
			}
		}
		bool flag = this.IsTextFilterActive();
		this.currentSearchString = newValue;
		this.currentSearchStringUpper = this.currentSearchString.ToUpper().Trim();
		if (this.IsTextFilterActive())
		{
			Transform reference = this.projectCategories["SearchResults"].GetComponent<HierarchyReferences>().GetReference<Transform>("Content");
			using (Dictionary<string, GameObject>.Enumerator enumerator = this.projectTechs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, GameObject> keyValuePair2 = enumerator.Current;
					this.projectTechs[keyValuePair2.Key].transform.SetParent(reference);
				}
				goto IL_0183;
			}
		}
		if (flag)
		{
			foreach (KeyValuePair<string, GameObject> keyValuePair3 in this.projectTechs)
			{
				Transform reference2 = this.projectCategories[Db.Get().Techs.Get(keyValuePair3.Key).category].GetComponent<HierarchyReferences>().GetReference<Transform>("Content");
				this.projectTechs[keyValuePair3.Key].transform.SetParent(reference2);
			}
		}
		IL_0183:
		if (!suppressUpdate)
		{
			this.UpdateProjectFilter(null);
		}
	}

	private void UpdateProjectFilter(object data = null)
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.projectCategories)
		{
			dictionary.Add(keyValuePair.Key, false);
		}
		bool flag = this.IsTextFilterActive();
		if (flag)
		{
			dictionary["SearchResults"] = true;
			this.categoryExpanded["SearchResults"] = true;
		}
		this.RefreshProjectsActive();
		foreach (KeyValuePair<string, GameObject> keyValuePair2 in this.projectTechs)
		{
			if ((keyValuePair2.Value.activeSelf || this.QueuedActivations.Contains(keyValuePair2.Value)) && !this.QueuedDeactivations.Contains(keyValuePair2.Value))
			{
				dictionary[Db.Get().Techs.Get(keyValuePair2.Key).category] = !flag;
				this.categoryExpanded[Db.Get().Techs.Get(keyValuePair2.Key).category] = true;
			}
		}
		foreach (KeyValuePair<string, bool> keyValuePair3 in dictionary)
		{
			this.ChangeGameObjectActive(this.projectCategories[keyValuePair3.Key], keyValuePair3.Value);
		}
		this.RefreshCategoriesContentExpanded();
		foreach (GameObject gameObject in this.orderedTechs)
		{
			gameObject.transform.SetAsLastSibling();
		}
	}

	private int CompareTechScores(global::Tuple<GameObject, string> a, global::Tuple<GameObject, string> b)
	{
		int techMatchScore = this.GetTechMatchScore(a.second);
		int techMatchScore2 = this.GetTechMatchScore(b.second);
		int num = -techMatchScore.CompareTo(techMatchScore2);
		if (num != 0)
		{
			return num;
		}
		if (!this.IsTextFilterActive())
		{
			return num;
		}
		return this.techCaches[a.second].CompareTo(this.techCaches[b.second]);
	}

	private Comparer<global::Tuple<GameObject, string>> TechWidgetComparer
	{
		get
		{
			if (this.techWidgetComparer == null)
			{
				this.techWidgetComparer = Comparer<global::Tuple<GameObject, string>>.Create(new Comparison<global::Tuple<GameObject, string>>(this.CompareTechScores));
			}
			return this.techWidgetComparer;
		}
	}

	private void RefreshProjectsActive()
	{
		if (this.projectTechItems.Count == 0)
		{
			return;
		}
		Techs techs = Db.Get().Techs;
		if (this.techCaches == null)
		{
			this.techCaches = SearchUtil.CacheTechs();
		}
		if (this.IsTextFilterActive())
		{
			using (Dictionary<string, SearchUtil.TechCache>.Enumerator enumerator = this.techCaches.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, SearchUtil.TechCache> keyValuePair = enumerator.Current;
					keyValuePair.Value.Bind(this.currentSearchStringUpper);
				}
				goto IL_00B2;
			}
		}
		foreach (KeyValuePair<string, SearchUtil.TechCache> keyValuePair2 in this.techCaches)
		{
			keyValuePair2.Value.Reset();
		}
		IL_00B2:
		for (int num = 0; num != techs.Count; num++)
		{
			Tech tech = (Tech)techs.GetResource(num);
			SearchUtil.TechCache techCache = this.techCaches[tech.Id];
			foreach (KeyValuePair<string, GameObject> keyValuePair3 in this.projectTechItems[tech.Id])
			{
				bool flag = SearchUtil.IsPassingScore(this.GetTechItemMatchScore(techCache, keyValuePair3.Key));
				HierarchyReferences component = keyValuePair3.Value.GetComponent<HierarchyReferences>();
				component.GetReference<LocText>("Label").color = (flag ? Color.white : Color.grey);
				component.GetReference<Image>("Icon").color = (flag ? Color.white : new Color(1f, 1f, 1f, 0.5f));
			}
		}
		ListPool<global::Tuple<int, int>, ResearchScreen>.PooledList pooledList = ListPool<global::Tuple<int, int>, ResearchScreen>.Allocate();
		for (int num2 = 0; num2 != techs.Count; num2++)
		{
			Tech tech2 = (Tech)techs.GetResource(num2);
			pooledList.Add(new global::Tuple<int, int>(num2, tech2.tier));
		}
		pooledList.Sort((global::Tuple<int, int> a, global::Tuple<int, int> b) => a.second.CompareTo(b.second));
		ListPool<global::Tuple<GameObject, string>, ResearchScreenSideBar>.PooledList pooledList2 = ListPool<global::Tuple<GameObject, string>, ResearchScreenSideBar>.Allocate();
		foreach (global::Tuple<int, int> tuple in pooledList)
		{
			Tech tech3 = (Tech)techs.GetResource(tuple.first);
			GameObject gameObject = this.projectTechs[tech3.Id];
			bool flag2 = SearchUtil.IsPassingScore(this.GetTechMatchScore(tech3.Id));
			this.ChangeGameObjectActive(gameObject, flag2);
			this.researchScreen.GetEntry(tech3).UpdateFilterState(flag2);
			if (flag2)
			{
				global::Tuple<GameObject, string> tuple2 = new global::Tuple<GameObject, string>(gameObject, tech3.Id);
				int num3 = pooledList2.BinarySearch(tuple2, this.TechWidgetComparer);
				if (num3 < 0)
				{
					num3 = ~num3;
				}
				while (num3 < pooledList2.Count && this.CompareTechScores(tuple2, pooledList2[num3]) == 0)
				{
					num3++;
				}
				pooledList2.Insert(num3, tuple2);
			}
		}
		pooledList.Recycle();
		this.orderedTechs.Clear();
		foreach (global::Tuple<GameObject, string> tuple3 in pooledList2)
		{
			this.orderedTechs.Add(tuple3.first);
		}
		pooledList2.Recycle();
	}

	private void RefreshCategoriesContentExpanded()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.projectCategories)
		{
			keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").gameObject.SetActive(this.categoryExpanded[keyValuePair.Key]);
			keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(this.categoryExpanded[keyValuePair.Key] ? 1 : 0);
		}
	}

	private void CreateCategory(string categoryID, string title = null)
	{
		GameObject gameObject = Util.KInstantiateUI(this.techCategoryPrefabAlt, this.projectsContainer, true);
		gameObject.name = categoryID;
		if (title == null)
		{
			title = Strings.Get("STRINGS.RESEARCH.TREES.TITLE" + categoryID.ToUpper());
		}
		gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(title);
		this.categoryExpanded.Add(categoryID, false);
		this.projectCategories.Add(categoryID, gameObject);
		gameObject.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = delegate
		{
			this.categoryExpanded[categoryID] = !this.categoryExpanded[categoryID];
			this.RefreshCategoriesContentExpanded();
		};
	}

	private void PopulateProjects()
	{
		ListPool<global::Tuple<global::Tuple<string, GameObject>, int>, ResearchScreen>.PooledList pooledList = ListPool<global::Tuple<global::Tuple<string, GameObject>, int>, ResearchScreen>.Allocate();
		for (int i = 0; i < Db.Get().Techs.Count; i++)
		{
			Tech tech = (Tech)Db.Get().Techs.GetResource(i);
			if (!this.projectCategories.ContainsKey(tech.category))
			{
				this.CreateCategory(tech.category, null);
			}
			GameObject gameObject = this.SpawnTechWidget(tech.Id, this.projectCategories[tech.category]);
			pooledList.Add(new global::Tuple<global::Tuple<string, GameObject>, int>(new global::Tuple<string, GameObject>(tech.Id, gameObject), tech.tier));
			this.projectTechs.Add(tech.Id, gameObject);
			gameObject.GetComponent<ToolTip>().SetSimpleTooltip(tech.desc);
			MultiToggle component = gameObject.GetComponent<MultiToggle>();
			component.onEnter = (global::System.Action)Delegate.Combine(component.onEnter, new global::System.Action(delegate
			{
				this.researchScreen.TurnEverythingOff();
				this.researchScreen.GetEntry(tech).OnHover(true, tech);
				this.soundPlayer.Play(1);
			}));
			MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
			component2.onExit = (global::System.Action)Delegate.Combine(component2.onExit, new global::System.Action(delegate
			{
				this.researchScreen.TurnEverythingOff();
			}));
		}
		this.CreateCategory("SearchResults", UI.RESEARCHSCREEN.SEARCH_RESULTS_CATEGORY);
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.projectTechs)
		{
			Transform reference = this.projectCategories[Db.Get().Techs.Get(keyValuePair.Key).category].GetComponent<HierarchyReferences>().GetReference<Transform>("Content");
			this.projectTechs[keyValuePair.Key].transform.SetParent(reference);
		}
		pooledList.Sort((global::Tuple<global::Tuple<string, GameObject>, int> a, global::Tuple<global::Tuple<string, GameObject>, int> b) => a.second.CompareTo(b.second));
		foreach (global::Tuple<global::Tuple<string, GameObject>, int> tuple in pooledList)
		{
			tuple.first.second.transform.SetAsLastSibling();
		}
		pooledList.Recycle();
	}

	private void PopulateFilterButtons()
	{
		using (Dictionary<string, List<Tag>>.Enumerator enumerator = this.filterPresets.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, List<Tag>> kvp = enumerator.Current;
				GameObject gameObject = Util.KInstantiateUI(this.filterButtonPrefab, this.searchFiltersContainer, true);
				this.filterButtons.Add(kvp.Key, gameObject);
				this.filterStates.Add(kvp.Key, false);
				MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
				TMP_Text componentInChildren = gameObject.GetComponentInChildren<LocText>();
				StringEntry text = Strings.Get("STRINGS.UI.RESEARCHSCREEN.FILTER_BUTTONS." + kvp.Key.ToUpper());
				componentInChildren.SetText(text);
				MultiToggle toggle2 = toggle;
				toggle2.onClick = (global::System.Action)Delegate.Combine(toggle2.onClick, new global::System.Action(delegate
				{
					foreach (KeyValuePair<string, GameObject> keyValuePair in this.filterButtons)
					{
						if (keyValuePair.Key != kvp.Key)
						{
							this.filterStates[keyValuePair.Key] = false;
							this.filterButtons[keyValuePair.Key].GetComponent<MultiToggle>().ChangeState(this.filterStates[keyValuePair.Key] ? 1 : 0);
						}
					}
					this.filterStates[kvp.Key] = !this.filterStates[kvp.Key];
					toggle.ChangeState(this.filterStates[kvp.Key] ? 1 : 0);
					this.searchBox.text = (this.filterStates[kvp.Key] ? text.String : "");
				}));
			}
		}
	}

	public void RefreshQueue()
	{
	}

	private void RefreshWidgets()
	{
		List<TechInstance> researchQueue = Research.Instance.GetResearchQueue();
		using (Dictionary<string, GameObject>.Enumerator enumerator = this.projectTechs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, GameObject> kvp = enumerator.Current;
				if (Db.Get().Techs.Get(kvp.Key).IsComplete())
				{
					kvp.Value.GetComponent<MultiToggle>().ChangeState(2);
				}
				else if (researchQueue.Find((TechInstance match) => match.tech.Id == kvp.Key) != null)
				{
					kvp.Value.GetComponent<MultiToggle>().ChangeState(1);
				}
				else
				{
					kvp.Value.GetComponent<MultiToggle>().ChangeState(0);
				}
			}
		}
	}

	private void RefreshWidgetProgressBars(string techID, GameObject widget)
	{
		HierarchyReferences component = widget.GetComponent<HierarchyReferences>();
		ResearchPointInventory progressInventory = Research.Instance.GetTechInstance(techID).progressInventory;
		int num = 0;
		for (int i = 0; i < Research.Instance.researchTypes.Types.Count; i++)
		{
			if (Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID.ContainsKey(Research.Instance.researchTypes.Types[i].id) && Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[i].id] > 0f)
			{
				HierarchyReferences component2 = component.GetReference<RectTransform>("BarRows").GetChild(1 + num).GetComponent<HierarchyReferences>();
				float num2 = progressInventory.PointsByTypeID[Research.Instance.researchTypes.Types[i].id] / Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[i].id];
				RectTransform rectTransform = component2.GetReference<Image>("Bar").rectTransform;
				rectTransform.sizeDelta = new Vector2(rectTransform.parent.rectTransform().rect.width * num2, rectTransform.sizeDelta.y);
				component2.GetReference<LocText>("Label").SetText(progressInventory.PointsByTypeID[Research.Instance.researchTypes.Types[i].id].ToString() + "/" + Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[i].id].ToString());
				num++;
			}
		}
	}

	private GameObject SpawnTechWidget(string techID, GameObject parentContainer)
	{
		GameObject gameObject = Util.KInstantiateUI(this.techWidgetRootAltPrefab, parentContainer, true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		gameObject.name = Db.Get().Techs.Get(techID).Name;
		component.GetReference<LocText>("Label").SetText(Db.Get().Techs.Get(techID).Name);
		if (!this.projectTechItems.ContainsKey(techID))
		{
			this.projectTechItems.Add(techID, new Dictionary<string, GameObject>());
		}
		RectTransform reference = component.GetReference<RectTransform>("UnlockContainer");
		global::System.Action <>9__0;
		foreach (TechItem techItem in Db.Get().Techs.Get(techID).unlockedItems)
		{
			if (Game.IsCorrectDlcActiveForCurrentSave(techItem))
			{
				GameObject gameObject2 = Util.KInstantiateUI(this.techItemPrefab, reference.gameObject, true);
				gameObject2.GetComponentsInChildren<Image>()[1].sprite = techItem.UISprite();
				gameObject2.GetComponentsInChildren<LocText>()[0].SetText(techItem.Name);
				MultiToggle component2 = gameObject2.GetComponent<MultiToggle>();
				Delegate onClick = component2.onClick;
				global::System.Action action;
				if ((action = <>9__0) == null)
				{
					action = (<>9__0 = delegate
					{
						this.researchScreen.ZoomToTech(techID, false);
					});
				}
				component2.onClick = (global::System.Action)Delegate.Combine(onClick, action);
				gameObject2.GetComponentsInChildren<Image>()[0].color = (this.evenRow ? this.evenRowColor : this.oddRowColor);
				this.evenRow = !this.evenRow;
				if (!this.projectTechItems[techID].ContainsKey(techItem.Id))
				{
					this.projectTechItems[techID].Add(techItem.Id, gameObject2);
				}
			}
		}
		MultiToggle component3 = gameObject.GetComponent<MultiToggle>();
		component3.onClick = (global::System.Action)Delegate.Combine(component3.onClick, new global::System.Action(delegate
		{
			this.researchScreen.ZoomToTech(techID, false);
		}));
		return gameObject;
	}

	private void ChangeGameObjectActive(GameObject target, bool targetActiveState)
	{
		if (target.activeSelf != targetActiveState)
		{
			if (targetActiveState)
			{
				this.QueuedActivations.Add(target);
				if (this.QueuedDeactivations.Contains(target))
				{
					this.QueuedDeactivations.Remove(target);
					return;
				}
			}
			else
			{
				this.QueuedDeactivations.Add(target);
				if (this.QueuedActivations.Contains(target))
				{
					this.QueuedActivations.Remove(target);
				}
			}
		}
	}

	private bool IsTextFilterActive()
	{
		return !string.IsNullOrEmpty(this.currentSearchString);
	}

	private bool AnyFilterActive()
	{
		return this.completionFilter != ResearchScreenSideBar.CompletionState.All || this.IsTextFilterActive();
	}

	private int GetTechItemMatchScore(SearchUtil.TechCache techCache, string techItemID)
	{
		TechItem techItem = Db.Get().TechItems.Get(techItemID);
		if (!Game.IsCorrectDlcActiveForCurrentSave(techItem))
		{
			return 0;
		}
		switch (this.completionFilter)
		{
		case ResearchScreenSideBar.CompletionState.Available:
			if (techItem.IsComplete())
			{
				return 0;
			}
			if (!techItem.ParentTech.ArePrerequisitesComplete())
			{
				return 0;
			}
			break;
		case ResearchScreenSideBar.CompletionState.Completed:
			if (!techItem.IsComplete())
			{
				return 0;
			}
			break;
		}
		if (!this.IsTextFilterActive())
		{
			return 100;
		}
		return techCache.techItems[techItemID].Score;
	}

	private int GetTechMatchScore(string techID)
	{
		Tech tech = Db.Get().Techs.Get(techID);
		switch (this.completionFilter)
		{
		case ResearchScreenSideBar.CompletionState.Available:
			if (tech.IsComplete())
			{
				return 0;
			}
			if (!tech.ArePrerequisitesComplete())
			{
				return 0;
			}
			break;
		case ResearchScreenSideBar.CompletionState.Completed:
			if (!tech.IsComplete())
			{
				return 0;
			}
			break;
		}
		if (!this.IsTextFilterActive())
		{
			return 100;
		}
		return this.techCaches[techID].Score;
	}

	public void ResetFilter()
	{
		this.SetTextFilter("", true);
		this.searchBox.text = "";
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.filterButtons)
		{
			this.filterStates[keyValuePair.Key] = false;
			this.filterButtons[keyValuePair.Key].GetComponent<MultiToggle>().ChangeState(this.filterStates[keyValuePair.Key] ? 1 : 0);
		}
		this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.All, true);
		this.UpdateProjectFilter(null);
	}

	public void SetSearch(string newSearch)
	{
		newSearch = UI.StripLinkFormatting(newSearch);
		this.searchBox.text = newSearch;
		this.SetTextFilter(newSearch, false);
	}

	[Header("Containers")]
	[SerializeField]
	private GameObject queueContainer;

	[SerializeField]
	private GameObject projectsContainer;

	[SerializeField]
	private GameObject searchFiltersContainer;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject headerTechTypePrefab;

	[SerializeField]
	private GameObject filterButtonPrefab;

	[SerializeField]
	private GameObject techWidgetRootPrefab;

	[SerializeField]
	private GameObject techWidgetRootAltPrefab;

	[SerializeField]
	private GameObject techItemPrefab;

	[SerializeField]
	private GameObject techWidgetUnlockedItemPrefab;

	[SerializeField]
	private GameObject techWidgetRowPrefab;

	[SerializeField]
	private GameObject techCategoryPrefab;

	[SerializeField]
	private GameObject techCategoryPrefabAlt;

	[Header("Other references")]
	[SerializeField]
	private KInputTextField searchBox;

	[SerializeField]
	private MultiToggle allFilter;

	[SerializeField]
	private MultiToggle availableFilter;

	[SerializeField]
	private MultiToggle completedFilter;

	[SerializeField]
	private ResearchScreen researchScreen;

	[SerializeField]
	private KButton clearSearchButton;

	[SerializeField]
	private Color evenRowColor;

	[SerializeField]
	private Color oddRowColor;

	private ResearchScreenSideBar.CompletionState completionFilter;

	private Dictionary<string, bool> filterStates = new Dictionary<string, bool>();

	private Dictionary<string, bool> categoryExpanded = new Dictionary<string, bool>();

	private string currentSearchString = "";

	private string currentSearchStringUpper = "";

	private const string SEARCH_RESULTS_CATEGORY_ID = "SearchResults";

	private Dictionary<string, SearchUtil.TechCache> techCaches;

	private readonly Dictionary<string, SearchUtil.TechItemCache> techItemCaches = new Dictionary<string, SearchUtil.TechItemCache>();

	private readonly List<GameObject> orderedTechs = new List<GameObject>();

	private Dictionary<string, GameObject> queueTechs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> projectTechs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> projectCategories = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> filterButtons = new Dictionary<string, GameObject>();

	private Dictionary<string, Dictionary<string, GameObject>> projectTechItems = new Dictionary<string, Dictionary<string, GameObject>>();

	private Dictionary<string, List<Tag>> filterPresets = new Dictionary<string, List<Tag>>
	{
		{
			"Oxygen",
			new List<Tag>()
		},
		{
			"Food",
			new List<Tag>()
		},
		{
			"Water",
			new List<Tag>()
		},
		{
			"Power",
			new List<Tag>()
		},
		{
			"Morale",
			new List<Tag>()
		},
		{
			"Ranching",
			new List<Tag>()
		},
		{
			"Filter",
			new List<Tag>()
		},
		{
			"Tile",
			new List<Tag>()
		},
		{
			"Transport",
			new List<Tag>()
		},
		{
			"Automation",
			new List<Tag>()
		},
		{
			"Medicine",
			new List<Tag>()
		},
		{
			"Rocket",
			new List<Tag>()
		}
	};

	private List<GameObject> QueuedActivations = new List<GameObject>();

	private List<GameObject> QueuedDeactivations = new List<GameObject>();

	public ButtonSoundPlayer soundPlayer;

	[SerializeField]
	private int activationPerFrame = 5;

	private Comparer<global::Tuple<GameObject, string>> techWidgetComparer;

	private bool evenRow;

	private enum CompletionState
	{
		All,
		Available,
		Completed
	}
}
