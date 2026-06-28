using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ReceptacleSideScreen : SideScreenContent
{
	public override string GetTitle()
	{
		if (this.targetReceptacle == null)
		{
			return Strings.Get(this.titleKey).ToString().Replace("{0}", string.Empty);
		}
		return string.Format(Strings.Get(this.titleKey), this.targetReceptacle.GetProperName());
	}

	public void Initialize(SingleEntityReceptacle target)
	{
		if (target == null)
		{
			Debug.LogError("SingleObjectReceptacle provided was null.");
			return;
		}
		this.targetReceptacle = target;
		base.gameObject.SetActive(true);
		this.despositObjectMap = new Dictionary<KToggle, Tag>();
		this.entityToggles.ForEach(delegate(KToggle rbi)
		{
			global::UnityEngine.Object.Destroy(rbi.transform.parent.gameObject);
		});
		this.entityToggles.Clear();
		foreach (Tag tag in target.possibleDepositObjectTags)
		{
			List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(tag);
			List<IHasSortOrder> list = new List<IHasSortOrder>();
			foreach (GameObject gameObject in prefabsWithTag)
			{
				IHasSortOrder component = gameObject.GetComponent<IHasSortOrder>();
				if (component != null)
				{
					list.Add(component);
				}
			}
			list.Sort((IHasSortOrder a, IHasSortOrder b) => a.sortOrder - b.sortOrder);
			foreach (IHasSortOrder hasSortOrder in list)
			{
				GameObject gameObject2 = (hasSortOrder as MonoBehaviour).gameObject;
				GameObject gameObject3 = Util.KInstantiateUI(this.entityToggle, this.requestObjectList, false);
				gameObject3.SetActive(true);
				KToggle newToggle = gameObject3.transform.GetChild(0).GetComponent<KToggle>();
				string properName = gameObject2.GetProperName();
				gameObject3.GetComponentInChildrenOnly<LocText>().text = properName;
				Sprite entityIcon = this.GetEntityIcon(gameObject2.PrefabID());
				if (entityIcon == null)
				{
					entityIcon = this.elementPlaceholderSpr;
				}
				newToggle.gameObject.GetComponentInChildrenOnly<Image>().sprite = entityIcon;
				newToggle.onClick += delegate
				{
					this.ToggleClicked(newToggle);
				};
				newToggle.onPointerEnter += delegate
				{
					this.UpdateAvailableAmounts(null);
				};
				newToggle.gameObject.SetActive(true);
				this.despositObjectMap.Add(newToggle, gameObject2.PrefabID());
				this.entityToggles.Add(newToggle);
			}
		}
		this.selectedEntityToggle = null;
		if (this.entityToggles.Count > 0)
		{
			if (this.entityPreviousSelectionMap.ContainsKey(this.targetReceptacle))
			{
				int num = this.entityPreviousSelectionMap[this.targetReceptacle];
				this.ToggleClicked(this.entityToggles[num]);
			}
			else
			{
				this.subtitleLabel.SetText(UI.UISIDESCREENS.PLANTERSIDESCREEN.SELECTSEED_TITLE);
				this.requestSelectedEntityBtn.interactable = false;
				this.descriptionLabel.SetText(UI.UISIDESCREENS.PLANTERSIDESCREEN.SELECTSEED_DESC);
			}
		}
		this.targetReceptacle.gameObject.Subscribe(-1697596308, new EventSystem.EventHandler(this.UpdateState));
		this.UpdateState(null);
		this.handle = GameScheduler.Instance.SchedulePeriodic(base.name, 1f, new Action<object>(this.UpdateAvailableAmounts), null, null, 0f);
	}

	private void UpdateState(object data)
	{
		this.requestSelectedEntityBtn.ClearOnClick();
		if (this.CheckReceptacleOccupied())
		{
			this.targetReceptacle.Occupant.gameObject.Subscribe(1969584890, delegate
			{
				this.UpdateState(null);
			});
			Uprootable uprootable = this.targetReceptacle.Occupant.GetComponent<Uprootable>();
			if (uprootable != null && uprootable.IsMarkedForUproot)
			{
				this.requestSelectedEntityBtn.onClick += delegate
				{
					uprootable.ForceCancelUproot(null);
					this.UpdateState(null);
				};
				this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.targetReceptacle.stringKey_CancelRemove);
				this.requestSelectedEntityBtn.interactable = true;
				this.subtitleLabel.SetText(string.Format(Strings.Get(this.subtitleStringAwaitingRemoval).ToString(), this.targetReceptacle.Occupant.GetProperName()));
			}
			else
			{
				this.requestSelectedEntityBtn.onClick += delegate
				{
					this.targetReceptacle.OrderRemoveOccupant();
					this.UpdateState(null);
				};
				this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.targetReceptacle.stringKey_Remove);
				this.requestSelectedEntityBtn.interactable = true;
				this.subtitleLabel.SetText(string.Format(Strings.Get(this.subtitleStringEntityDeposited).ToString(), this.targetReceptacle.Occupant.GetProperName()));
			}
			this.ToggleSeedSelector(false);
			Tag tag = this.targetReceptacle.Occupant.GetComponent<KSelectable>().PrefabID();
			this.ConfigureActiveEntity(tag);
			string resultDescription = this.GetResultDescription(this.targetReceptacle.Occupant);
			this.descriptionLabel.SetText(resultDescription);
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
			this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.targetReceptacle.stringKey_CancelPlace);
			this.requestSelectedEntityBtn.interactable = true;
			this.ToggleSeedSelector(false);
			this.ConfigureActiveEntity(this.targetReceptacle.GetActiveRequest.tags[0]);
			GameObject prefab = Assets.GetPrefab(this.targetReceptacle.GetActiveRequest.tags[0]);
			if (prefab != null)
			{
				this.subtitleLabel.SetText(string.Format(Strings.Get(this.subtitleStringAwaitingDelivery).ToString(), prefab.GetProperName()));
				string resultDescription2 = this.GetResultDescription(prefab);
				this.descriptionLabel.SetText(resultDescription2);
			}
		}
		else if (this.selectedEntityToggle != null)
		{
			this.requestSelectedEntityBtn.onClick += delegate
			{
				this.targetReceptacle.CreateOrder(this.selectedDepositObjectTag);
				this.UpdateAvailableAmounts(null);
				this.UpdateState(null);
			};
			this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.targetReceptacle.stringKey_Place);
			bool flag = this.GetAvailableAmount(this.despositObjectMap[this.selectedEntityToggle]) > 0f;
			this.requestSelectedEntityBtn.interactable = flag;
			this.SetImageToggleState(this.selectedEntityToggle, (!flag) ? ImageToggleState.State.DisabledActive : ImageToggleState.State.Active);
			this.ToggleSeedSelector(true);
			GameObject prefab2 = Assets.GetPrefab(this.selectedDepositObjectTag);
			if (prefab2 != null)
			{
				this.subtitleLabel.SetText(string.Format(Strings.Get(this.subtitleStringAwaitingSelection).ToString(), prefab2.GetProperName()));
				string resultDescription3 = this.GetResultDescription(prefab2);
				this.descriptionLabel.SetText(resultDescription3);
			}
		}
		else
		{
			this.requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(this.targetReceptacle.stringKey_Place);
			this.requestSelectedEntityBtn.interactable = false;
			this.ToggleSeedSelector(true);
		}
	}

	private void ClearSelection()
	{
		foreach (KeyValuePair<KToggle, Tag> keyValuePair in this.despositObjectMap)
		{
			keyValuePair.Key.Deselect();
		}
	}

	private void ToggleSeedSelector(bool Show)
	{
		this.requestObjectListContainer.SetActive(Show);
		this.scrollBarContainer.SetActive(Show);
		this.requestObjectList.SetActive(Show);
		this.activeEntityContainer.SetActive(!Show);
	}

	protected override void OnCleanUp()
	{
		this.handle.Clear();
		base.OnCleanUp();
	}

	private void ConfigureActiveEntity(Tag tag)
	{
		GameObject prefab = Assets.GetPrefab(tag);
		string properName = prefab.GetProperName();
		this.activeEntityContainer.GetComponentInChildrenOnly<LocText>().text = properName;
		this.activeEntityContainer.transform.GetChild(0).gameObject.GetComponentInChildrenOnly<Image>().sprite = this.GetEntityIcon(tag);
	}

	protected virtual Sprite GetEntityIcon(Tag prefabTag)
	{
		GameObject prefab = Assets.GetPrefab(prefabTag);
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		return Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui");
	}

	public override void SetTarget(GameObject target)
	{
		SingleEntityReceptacle component = target.GetComponent<SingleEntityReceptacle>();
		if (component == null)
		{
			Debug.LogError("The object selected doesn't have a SingleObjectReceptacle!");
			return;
		}
		this.Initialize(component);
		this.UpdateAvailableAmounts(null);
	}

	private void SetImageToggleState(KToggle toggle, ImageToggleState.State state)
	{
		switch (state)
		{
		case ImageToggleState.State.Disabled:
			toggle.GetComponent<ImageToggleState>().SetDisabled();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = this.desaturatedMaterial;
			break;
		case ImageToggleState.State.Inactive:
			toggle.GetComponent<ImageToggleState>().SetInactive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = this.defaultMaterial;
			break;
		case ImageToggleState.State.Active:
			toggle.GetComponent<ImageToggleState>().SetActive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = this.defaultMaterial;
			break;
		case ImageToggleState.State.DisabledActive:
			toggle.GetComponent<ImageToggleState>().SetDisabledActive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = this.desaturatedMaterial;
			break;
		}
	}

	private void UpdateAvailableAmounts(object data)
	{
		foreach (KeyValuePair<KToggle, Tag> keyValuePair in this.despositObjectMap)
		{
			if (!keyValuePair.Key.transform.parent.gameObject.activeSelf)
			{
				keyValuePair.Key.transform.parent.gameObject.SetActive(true);
			}
			float availableAmount = this.GetAvailableAmount(keyValuePair.Value);
			keyValuePair.Key.transform.parent.gameObject.GetComponentsInChildren<LocText>()[1].text = availableAmount.ToString();
			if (availableAmount <= 0f)
			{
				if (this.selectedEntityToggle != keyValuePair.Key)
				{
					this.SetImageToggleState(keyValuePair.Key, ImageToggleState.State.Disabled);
				}
				else
				{
					this.SetImageToggleState(keyValuePair.Key, ImageToggleState.State.DisabledActive);
				}
			}
			else if (this.selectedEntityToggle != keyValuePair.Key)
			{
				this.SetImageToggleState(keyValuePair.Key, ImageToggleState.State.Inactive);
			}
			else
			{
				this.SetImageToggleState(keyValuePair.Key, ImageToggleState.State.Active);
			}
		}
	}

	private float GetAvailableAmount(Tag tag)
	{
		return WorldInventory.Instance.GetAmount(tag);
	}

	private void ToggleClicked(KToggle toggle)
	{
		if (!this.despositObjectMap.ContainsKey(toggle))
		{
			Debug.LogError("Recipe not found on recipe list.");
			return;
		}
		this.selectedEntityToggle = toggle;
		this.entityPreviousSelectionMap[this.targetReceptacle] = this.entityToggles.IndexOf(toggle);
		this.selectedDepositObjectTag = this.despositObjectMap[toggle];
		this.UpdateAvailableAmounts(null);
		this.UpdateState(null);
	}

	private void CreateOrder(bool isInfinite)
	{
		this.targetReceptacle.CreateOrder(this.selectedDepositObjectTag);
	}

	private bool CheckReceptacleOccupied()
	{
		return this.targetReceptacle.Occupant != null;
	}

	protected virtual string GetResultDescription(GameObject go)
	{
		string text = "Entity prefab has no info description component.";
		InfoDescription component = go.GetComponent<InfoDescription>();
		if (component)
		{
			text = component.description;
		}
		return text;
	}

	[SerializeField]
	private KButton requestSelectedEntityBtn;

	public GameObject activeEntityContainer;

	public GameObject nothingDiscoveredContainer;

	[SerializeField]
	private LocText descriptionLabel;

	private Dictionary<SingleEntityReceptacle, int> entityPreviousSelectionMap = new Dictionary<SingleEntityReceptacle, int>();

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

	public Material defaultMaterial;

	public Material desaturatedMaterial;

	[SerializeField]
	private GameObject requestObjectList;

	[SerializeField]
	private GameObject requestObjectListContainer;

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

	private KToggle selectedEntityToggle;

	private SingleEntityReceptacle targetReceptacle;

	private Tag selectedDepositObjectTag;

	private Dictionary<KToggle, Tag> despositObjectMap;

	private List<KToggle> entityToggles = new List<KToggle>();

	private SchedulerHandle handle;
}
