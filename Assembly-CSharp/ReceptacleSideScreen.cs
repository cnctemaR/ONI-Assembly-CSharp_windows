using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceptacleSideScreen : SideScreenContent, IRender1000ms
{
	public override string GetTitle()
	{
		if (this.targetReceptacle == null)
		{
			return Strings.Get(this.titleKey).ToString().Replace("{0}", "");
		}
		return string.Format(Strings.Get(this.titleKey), this.targetReceptacle.GetProperName());
	}

	private void RecycleToggle(GameObject toggle)
	{
		toggle.SetActive(false);
		this.recycledEntityToggles.Add(toggle);
	}

	private GameObject SpawnToggle(GameObject parent)
	{
		if (this.recycledEntityToggles.Count > 0)
		{
			GameObject gameObject = this.recycledEntityToggles[this.recycledEntityToggles.Count - 1];
			this.recycledEntityToggles.RemoveAt(this.recycledEntityToggles.Count - 1);
			gameObject.transform.SetParent(parent.transform);
			gameObject.SetActive(true);
			return gameObject;
		}
		return Util.KInstantiateUI(this.entityToggle, parent, true);
	}

	private void RefreshCategoryOpen(GameObject categoryHeader, GameObject categoryGrid, Tag tag)
	{
		categoryHeader.GetComponent<MultiToggle>().ChangeState(this.categoryExpandedStatus[tag] ? 0 : 1);
		categoryGrid.gameObject.SetActive(this.categoryExpandedStatus[tag]);
	}

	public void Initialize(SingleEntityReceptacle target)
	{
		if (target == null)
		{
			global::Debug.LogError("SingleObjectReceptacle provided was null.");
			return;
		}
		this.targetReceptacle = target;
		base.gameObject.SetActive(true);
		this.depositObjectMap = new Dictionary<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity>();
		this.entityToggles.ForEach(delegate(ReceptacleToggle rbi)
		{
			this.RecycleToggle(rbi.gameObject);
		});
		this.entityToggles.Clear();
		List<GameObject> list = new List<GameObject>();
		if (this.targetReceptacle.possibleDepositObjectTags.Count == 1)
		{
			this.categoryStartExpanded = true;
		}
		using (IEnumerator<Tag> enumerator = this.targetReceptacle.possibleDepositObjectTags.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Tag tag = enumerator.Current;
				List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(tag);
				int num = prefabsWithTag.Count;
				if (this.categoryExpandedStatus.ContainsKey(tag))
				{
					this.categoryExpandedStatus[tag] = this.categoryStartExpanded;
				}
				if (!this.contentContainers.ContainsKey(tag))
				{
					GameObject gameObject = Util.KInstantiateUI(this.categoryContainerPrefab, this.requestObjectListContainerContent, true);
					this.contentContainers.Add(tag, gameObject);
					HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
					component.GetReference<LocText>("HeaderLabel").SetText(tag.ProperName());
					this.categoryExpandedStatus.Add(tag, this.categoryStartExpanded);
					MultiToggle toggle = gameObject.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("HeaderToggle");
					GridLayoutGroup grid = component.GetReference<GridLayoutGroup>("GridLayout");
					MultiToggle toggle3 = toggle;
					toggle3.onClick = (global::System.Action)Delegate.Combine(toggle3.onClick, new global::System.Action(delegate
					{
						this.categoryExpandedStatus[tag] = !this.categoryExpandedStatus[tag];
						this.RefreshCategoryOpen(toggle.gameObject, grid.gameObject, tag);
					}));
					this.RefreshCategoryOpen(toggle.gameObject, grid.gameObject, tag);
				}
				this.RefreshCategoryOpen(this.contentContainers[tag].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("HeaderToggle").gameObject, this.contentContainers[tag].GetComponent<HierarchyReferences>().GetReference<GridLayoutGroup>("GridLayout").gameObject, tag);
				List<IHasSortOrder> list2 = new List<IHasSortOrder>();
				foreach (GameObject gameObject2 in prefabsWithTag)
				{
					if (!this.targetReceptacle.IsValidEntity(gameObject2) || list.Contains(gameObject2))
					{
						num--;
					}
					else
					{
						IHasSortOrder component2 = gameObject2.GetComponent<IHasSortOrder>();
						if (component2 != null)
						{
							list.Add(gameObject2);
							list2.Add(component2);
						}
					}
				}
				global::Debug.Assert(list2.Count == num, "Not all entities in this receptacle implement IHasSortOrder!");
				list2.Sort((IHasSortOrder a, IHasSortOrder b) => a.sortOrder - b.sortOrder);
				foreach (IHasSortOrder hasSortOrder in list2)
				{
					GameObject gameObject3 = (hasSortOrder as MonoBehaviour).gameObject;
					GameObject gameObject4 = this.SpawnToggle(this.contentContainers[tag].GetComponent<HierarchyReferences>().GetReference("GridLayout").gameObject);
					gameObject4.transform.SetAsLastSibling();
					gameObject4.SetActive(true);
					ReceptacleToggle newToggle = gameObject4.GetComponent<ReceptacleToggle>();
					IReceptacleDirection component3 = gameObject3.GetComponent<IReceptacleDirection>();
					string entityName = this.GetEntityName(gameObject3.PrefabID());
					newToggle.title.text = entityName;
					Sprite entityIcon = this.GetEntityIcon(gameObject3.PrefabID());
					if (entityIcon == null)
					{
						entityIcon = this.elementPlaceholderSpr;
					}
					newToggle.image.sprite = entityIcon;
					if (newToggle.toggle == null)
					{
						newToggle.toggle = newToggle.GetComponentInChildren<MultiToggle>();
					}
					MultiToggle toggle2 = newToggle.toggle;
					toggle2.onClick = (global::System.Action)Delegate.Combine(toggle2.onClick, new global::System.Action(delegate
					{
						this.ToggleClicked(newToggle);
					}));
					ToolTip component4 = newToggle.GetComponent<ToolTip>();
					if (component4 != null)
					{
						component4.SetSimpleTooltip(this.GetEntityTooltip(gameObject3.PrefabID()));
					}
					this.depositObjectMap.Add(newToggle, new ReceptacleSideScreen.SelectableEntity
					{
						tag = gameObject3.PrefabID(),
						direction = ((component3 != null) ? component3.Direction : SingleEntityReceptacle.ReceptacleDirection.Top),
						asset = gameObject3
					});
					this.entityToggles.Add(newToggle);
				}
			}
		}
		this.RestoreSelectionFromOccupant();
		this.selectedEntityToggle = null;
		if (this.entityToggles.Count > 0)
		{
			if (this.entityPreviousSelectionMap.ContainsKey(this.targetReceptacle))
			{
				int num2 = this.entityPreviousSelectionMap[this.targetReceptacle];
				this.ToggleClicked(this.entityToggles[num2]);
			}
			else
			{
				this.subtitleLabel.SetText(Strings.Get(this.subtitleStringSelect).ToString());
				this.requestSelectedEntityBtn.isInteractable = false;
				this.descriptionLabel.SetText(Strings.Get(this.subtitleStringSelectDescription).ToString());
				this.HideAllDescriptorPanels();
			}
		}
		this.onStorageChangedHandle = this.targetReceptacle.gameObject.Subscribe(-1697596308, new Action<object>(this.CheckAmountsAndUpdate));
		this.onOccupantValidChangedHandle = this.targetReceptacle.gameObject.Subscribe(-1820564715, new Action<object>(this.OnOccupantValidChanged));
		this.UpdateState(null);
		SimAndRenderScheduler.instance.Add(this, false);
	}

	protected virtual void UpdateState(object data)
	{
		this.requestSelectedEntityBtn.ClearOnClick();
		if (this.targetReceptacle == null)
		{
			return;
		}
		if (this.CheckReceptacleOccupied())
		{
			Uprootable uprootable = this.targetReceptacle.Occupant.GetComponent<Uprootable>();
			if (uprootable != null && uprootable.IsMarkedForUproot)
			{
				this.requestSelectedEntityBtn.onClick += delegate
				{
					uprootable.ForceCancelUproot(null);
					this.UpdateState(null);
				};
				this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.requestStringCancelRemove).ToString();
				this.subtitleLabel.SetText(string.Format(Strings.Get(this.subtitleStringAwaitingRemoval).ToString(), this.targetReceptacle.Occupant.GetProperName()));
			}
			else
			{
				this.requestSelectedEntityBtn.onClick += delegate
				{
					this.targetReceptacle.OrderRemoveOccupant();
					this.UpdateState(null);
				};
				this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.requestStringRemove).ToString();
				this.subtitleLabel.SetText(string.Format(Strings.Get(this.subtitleStringEntityDeposited).ToString(), this.targetReceptacle.Occupant.GetProperName()));
			}
			this.requestSelectedEntityBtn.isInteractable = true;
			this.ToggleObjectPicker(false);
			Tag tag = this.targetReceptacle.Occupant.GetComponent<KSelectable>().PrefabID();
			this.ConfigureActiveEntity(tag);
			this.SetResultDescriptions(this.targetReceptacle.Occupant);
		}
		else if (this.targetReceptacle.GetActiveRequest != null)
		{
			this.requestSelectedEntityBtn.onClick += delegate
			{
				this.targetReceptacle.CancelActiveRequest();
				this.ClearSelection();
				this.UpdateAvailableAmounts(null);
				this.UpdateState(null);
			};
			this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.requestStringCancelDeposit).ToString();
			this.requestSelectedEntityBtn.isInteractable = true;
			this.ToggleObjectPicker(false);
			this.ConfigureActiveEntity(this.targetReceptacle.GetActiveRequest.tagsFirst);
			GameObject prefab = Assets.GetPrefab(this.targetReceptacle.GetActiveRequest.tagsFirst);
			if (prefab != null)
			{
				this.subtitleLabel.SetText(string.Format(Strings.Get(this.subtitleStringAwaitingDelivery).ToString(), prefab.GetProperName()));
				this.SetResultDescriptions(prefab);
			}
		}
		else if (this.selectedEntityToggle != null)
		{
			this.requestSelectedEntityBtn.onClick += delegate
			{
				this.targetReceptacle.CreateOrder(this.selectedDepositObjectTag, this.selectedDepositObjectAdditionalTag);
				this.UpdateAvailableAmounts(null);
				this.UpdateState(null);
			};
			this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.requestStringDeposit).ToString();
			this.targetReceptacle.SetPreview(this.depositObjectMap[this.selectedEntityToggle].tag, false);
			bool flag = this.CanDepositEntity(this.depositObjectMap[this.selectedEntityToggle], true);
			this.requestSelectedEntityBtn.isInteractable = flag;
			this.ToggleObjectPicker(true);
			GameObject prefab2 = Assets.GetPrefab(this.selectedDepositObjectTag);
			if (prefab2 != null)
			{
				this.subtitleLabel.SetText(string.Format(Strings.Get(this.subtitleStringAwaitingSelection).ToString(), prefab2.GetProperName()));
				this.SetResultDescriptions(prefab2);
			}
		}
		else
		{
			this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.requestStringDeposit).ToString();
			this.requestSelectedEntityBtn.isInteractable = false;
			this.ToggleObjectPicker(true);
		}
		this.UpdateAvailableAmounts(null);
		this.RefreshToggleStates();
		this.UpdateListeners();
	}

	private void UpdateListeners()
	{
		if (this.CheckReceptacleOccupied())
		{
			if (this.onObjectDestroyedHandle == -1)
			{
				this.onObjectDestroyedHandle = this.targetReceptacle.Occupant.gameObject.Subscribe(1969584890, delegate(object d)
				{
					this.UpdateState(null);
				});
				return;
			}
		}
		else if (this.onObjectDestroyedHandle != -1)
		{
			this.onObjectDestroyedHandle = -1;
		}
	}

	private void OnOccupantValidChanged(object _)
	{
		if (this.targetReceptacle == null)
		{
			return;
		}
		if (!this.CheckReceptacleOccupied() && this.targetReceptacle.GetActiveRequest != null)
		{
			bool flag = false;
			ReceptacleSideScreen.SelectableEntity selectableEntity;
			if (this.depositObjectMap.TryGetValue(this.selectedEntityToggle, out selectableEntity))
			{
				flag = this.CanDepositEntity(selectableEntity, true);
			}
			if (!flag)
			{
				this.targetReceptacle.CancelActiveRequest();
				this.ClearSelection();
				this.UpdateState(null);
				this.UpdateAvailableAmounts(null);
			}
		}
	}

	protected bool CanDepositEntity(ReceptacleSideScreen.SelectableEntity entity, bool runAdditionalCanDepositTest = false)
	{
		return this.ValidRotationForDeposit(entity.direction) && (!this.RequiresAvailableAmountToDeposit() || this.GetAvailableAmount(entity.tag) > 0f) && (!runAdditionalCanDepositTest || this.AdditionalCanDepositTest());
	}

	protected virtual bool AdditionalCanDepositTest()
	{
		return true;
	}

	protected virtual bool RequiresAvailableAmountToDeposit()
	{
		return true;
	}

	private void ClearSelection()
	{
		this.selectedEntityToggle = null;
		this.RefreshToggleStates();
	}

	private void ToggleObjectPicker(bool Show)
	{
		this.requestObjectListContainer.SetActive(Show);
		if (this.scrollBarContainer != null)
		{
			this.scrollBarContainer.SetActive(Show);
		}
		this.requestObjectListContainer.SetActive(Show);
		this.activeEntityContainer.SetActive(!Show);
	}

	private void ConfigureActiveEntity(Tag tag)
	{
		string properName = Assets.GetPrefab(tag).GetProperName();
		HierarchyReferences component = this.activeEntityContainer.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("Label").text = properName;
		component.GetReference<Image>("Icon").sprite = this.GetEntityIcon(tag);
	}

	protected virtual string GetEntityName(Tag prefabTag)
	{
		return Assets.GetPrefab(prefabTag).GetProperName();
	}

	protected virtual string GetEntityTooltip(Tag prefabTag)
	{
		InfoDescription component = Assets.GetPrefab(prefabTag).GetComponent<InfoDescription>();
		string text = this.GetEntityName(prefabTag);
		if (component != null)
		{
			text = text + "\n\n" + component.description;
		}
		return text;
	}

	protected virtual Sprite GetEntityIcon(Tag prefabTag)
	{
		return Def.GetUISprite(Assets.GetPrefab(prefabTag), "ui", false).first;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		SingleEntityReceptacle component = target.GetComponent<SingleEntityReceptacle>();
		return component != null && component.enabled && target.GetComponent<PlantablePlot>() == null && target.GetComponent<EggIncubator>() == null && target.GetComponent<SpecialCargoBayClusterReceptacle>() == null;
	}

	public override void SetTarget(GameObject target)
	{
		SingleEntityReceptacle component = target.GetComponent<SingleEntityReceptacle>();
		if (component == null)
		{
			global::Debug.LogError("The object selected doesn't have a SingleObjectReceptacle!");
			return;
		}
		this.Initialize(component);
		this.UpdateState(null);
	}

	protected virtual void RestoreSelectionFromOccupant()
	{
	}

	public override void ClearTarget()
	{
		if (this.targetReceptacle != null)
		{
			if (this.CheckReceptacleOccupied())
			{
				this.targetReceptacle.Occupant.gameObject.Unsubscribe(this.onObjectDestroyedHandle);
				this.onObjectDestroyedHandle = -1;
			}
			this.targetReceptacle.Unsubscribe(this.onStorageChangedHandle);
			this.onStorageChangedHandle = -1;
			this.targetReceptacle.Unsubscribe(this.onOccupantValidChangedHandle);
			this.onOccupantValidChangedHandle = -1;
			if (this.targetReceptacle.GetActiveRequest == null)
			{
				this.targetReceptacle.SetPreview(Tag.Invalid, false);
			}
			SimAndRenderScheduler.instance.Remove(this);
			this.targetReceptacle = null;
		}
	}

	protected void RefreshToggleStates()
	{
		foreach (KeyValuePair<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity> keyValuePair in this.depositObjectMap)
		{
			if (this.selectedEntityToggle != keyValuePair.Key)
			{
				if (this.CanDepositEntity(keyValuePair.Value, false))
				{
					this.SetToggleState(keyValuePair.Key.toggle, ImageToggleState.State.Inactive);
				}
				else
				{
					this.SetToggleState(keyValuePair.Key.toggle, ImageToggleState.State.Disabled);
				}
			}
			else if (this.CanDepositEntity(keyValuePair.Value, false))
			{
				this.SetToggleState(keyValuePair.Key.toggle, ImageToggleState.State.Active);
			}
			else
			{
				this.SetToggleState(keyValuePair.Key.toggle, ImageToggleState.State.DisabledActive);
			}
		}
	}

	protected void SetToggleState(MultiToggle toggle, ImageToggleState.State state)
	{
		switch (state)
		{
		case ImageToggleState.State.Disabled:
			toggle.ChangeState(2);
			toggle.gameObject.GetComponentsInChildrenOnly<Image>()[1].material = this.desaturatedMaterial;
			return;
		case ImageToggleState.State.Inactive:
			toggle.ChangeState(0);
			toggle.gameObject.GetComponentsInChildrenOnly<Image>()[1].material = this.defaultMaterial;
			return;
		case ImageToggleState.State.Active:
			toggle.ChangeState(1);
			toggle.gameObject.GetComponentsInChildrenOnly<Image>()[1].material = this.defaultMaterial;
			return;
		case ImageToggleState.State.DisabledActive:
			toggle.ChangeState(3);
			toggle.gameObject.GetComponentsInChildrenOnly<Image>()[1].material = this.desaturatedMaterial;
			return;
		default:
			return;
		}
	}

	public void Render1000ms(float dt)
	{
		this.CheckAmountsAndUpdate(null);
	}

	private void CheckAmountsAndUpdate(object data)
	{
		if (this.targetReceptacle == null)
		{
			return;
		}
		if (this.UpdateAvailableAmounts(null))
		{
			this.UpdateState(null);
		}
	}

	private bool UpdateAvailableAmounts(object data)
	{
		bool flag = false;
		foreach (KeyValuePair<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity> keyValuePair in this.depositObjectMap)
		{
			if (!DebugHandler.InstantBuildMode && this.hideUndiscoveredEntities && !DiscoveredResources.Instance.IsDiscovered(keyValuePair.Value.tag))
			{
				keyValuePair.Key.gameObject.SetActive(false);
			}
			else if (!keyValuePair.Key.gameObject.activeSelf)
			{
				keyValuePair.Key.gameObject.SetActive(true);
			}
			float availableAmount = this.GetAvailableAmount(keyValuePair.Value.tag);
			if (keyValuePair.Value.lastAmount != availableAmount)
			{
				flag = true;
				keyValuePair.Value.lastAmount = availableAmount;
				keyValuePair.Key.amount.text = availableAmount.ToString();
			}
			if (!this.ValidRotationForDeposit(keyValuePair.Value.direction) || availableAmount <= 0f)
			{
				if (this.selectedEntityToggle != keyValuePair.Key)
				{
					keyValuePair.Key.toggle.ChangeState(2);
				}
				else
				{
					keyValuePair.Key.toggle.ChangeState(3);
				}
			}
			else if (this.selectedEntityToggle != keyValuePair.Key)
			{
				keyValuePair.Key.toggle.ChangeState(0);
			}
			else
			{
				keyValuePair.Key.toggle.ChangeState(1);
			}
		}
		foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.contentContainers)
		{
			Transform transform = keyValuePair2.Value.GetComponent<HierarchyReferences>().GetReference<GridLayoutGroup>("GridLayout").transform;
			bool flag2 = false;
			for (int i = 0; i < transform.childCount; i++)
			{
				if (transform.GetChild(i).gameObject.activeSelf)
				{
					flag2 = true;
					break;
				}
			}
			if (keyValuePair2.Value.activeSelf != flag2)
			{
				keyValuePair2.Value.SetActive(flag2);
			}
		}
		return flag;
	}

	protected float GetAvailableAmount(Tag tag)
	{
		if (this.ALLOW_ORDER_IGNORING_WOLRD_NEED)
		{
			IEnumerable<Pickupable> pickupables = this.targetReceptacle.GetMyWorld().worldInventory.GetPickupables(tag, true);
			float num = 0f;
			foreach (Pickupable pickupable in pickupables)
			{
				num += (float)Mathf.CeilToInt(pickupable.TotalAmount);
			}
			return num;
		}
		return this.targetReceptacle.GetMyWorld().worldInventory.GetAmount(tag, true);
	}

	private bool ValidRotationForDeposit(SingleEntityReceptacle.ReceptacleDirection depositDir)
	{
		return this.targetReceptacle.rotatable == null || depositDir == this.targetReceptacle.Direction;
	}

	protected virtual void ToggleClicked(ReceptacleToggle toggle)
	{
		if (!this.depositObjectMap.ContainsKey(toggle))
		{
			global::Debug.LogError("Recipe not found on recipe list.");
			return;
		}
		this.selectedEntityToggle = toggle;
		this.entityPreviousSelectionMap[this.targetReceptacle] = this.entityToggles.IndexOf(toggle);
		this.selectedDepositObjectTag = this.depositObjectMap[toggle].tag;
		MutantPlant component = this.depositObjectMap[toggle].asset.GetComponent<MutantPlant>();
		this.selectedDepositObjectAdditionalTag = (component ? component.SubSpeciesID : Tag.Invalid);
		this.RefreshToggleStates();
		this.UpdateAvailableAmounts(null);
		this.UpdateState(null);
	}

	private void CreateOrder(bool isInfinite)
	{
		this.targetReceptacle.CreateOrder(this.selectedDepositObjectTag, this.selectedDepositObjectAdditionalTag);
	}

	protected bool CheckReceptacleOccupied()
	{
		return this.targetReceptacle != null && this.targetReceptacle.Occupant != null;
	}

	protected virtual void SetResultDescriptions(GameObject go)
	{
		string text = "";
		InfoDescription component = go.GetComponent<InfoDescription>();
		if (component)
		{
			text = component.description;
		}
		else
		{
			KPrefabID component2 = go.GetComponent<KPrefabID>();
			if (component2 != null)
			{
				Element element = ElementLoader.GetElement(component2.PrefabID());
				if (element != null)
				{
					text = element.Description();
				}
			}
			else
			{
				text = go.GetProperName();
			}
		}
		this.descriptionLabel.SetText(text);
	}

	protected virtual void HideAllDescriptorPanels()
	{
		for (int i = 0; i < this.descriptorPanels.Count; i++)
		{
			this.descriptorPanels[i].gameObject.SetActive(false);
		}
	}

	protected bool ALLOW_ORDER_IGNORING_WOLRD_NEED = true;

	[SerializeField]
	protected KButton requestSelectedEntityBtn;

	[SerializeField]
	private string requestStringDeposit;

	[SerializeField]
	private string requestStringCancelDeposit;

	[SerializeField]
	private string requestStringRemove;

	[SerializeField]
	private string requestStringCancelRemove;

	public GameObject activeEntityContainer;

	public GameObject nothingDiscoveredContainer;

	[SerializeField]
	private bool categoryStartExpanded;

	[SerializeField]
	private GameObject categoryContainerPrefab;

	private Dictionary<Tag, GameObject> contentContainers = new Dictionary<Tag, GameObject>();

	[SerializeField]
	protected LocText descriptionLabel;

	protected Dictionary<SingleEntityReceptacle, int> entityPreviousSelectionMap = new Dictionary<SingleEntityReceptacle, int>();

	[SerializeField]
	private string subtitleStringSelect;

	[SerializeField]
	private string subtitleStringSelectDescription;

	[SerializeField]
	private string subtitleStringAwaitingSelection;

	[SerializeField]
	private string subtitleStringAwaitingDelivery;

	[SerializeField]
	private string subtitleStringEntityDeposited;

	[SerializeField]
	private string subtitleStringAwaitingRemoval;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private List<DescriptorPanel> descriptorPanels;

	public Material defaultMaterial;

	public Material desaturatedMaterial;

	[SerializeField]
	private GameObject requestObjectListContainer;

	[SerializeField]
	private GameObject requestObjectListContainerContent;

	[SerializeField]
	private GameObject scrollBarContainer;

	[SerializeField]
	private GameObject entityToggle;

	[SerializeField]
	private Sprite buttonSelectedBG;

	[SerializeField]
	private Sprite buttonNormalBG;

	[SerializeField]
	private Sprite elementPlaceholderSpr;

	[SerializeField]
	private bool hideUndiscoveredEntities;

	protected ReceptacleToggle selectedEntityToggle;

	protected SingleEntityReceptacle targetReceptacle;

	protected Tag selectedDepositObjectTag;

	protected Tag selectedDepositObjectAdditionalTag;

	protected Dictionary<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity> depositObjectMap;

	protected List<ReceptacleToggle> entityToggles = new List<ReceptacleToggle>();

	private List<GameObject> recycledEntityToggles = new List<GameObject>();

	private Dictionary<Tag, bool> categoryExpandedStatus = new Dictionary<Tag, bool>();

	private int onObjectDestroyedHandle = -1;

	private int onOccupantValidChangedHandle = -1;

	private int onStorageChangedHandle = -1;

	protected class SelectableEntity
	{
		public Tag tag;

		public SingleEntityReceptacle.ReceptacleDirection direction;

		public GameObject asset;

		public float lastAmount = -1f;
	}
}
