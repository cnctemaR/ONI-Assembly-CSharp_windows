using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class AccessControlSideScreen : SideScreenContent
{
	public override string GetTitle()
	{
		if (this.target != null)
		{
			return string.Format(base.GetTitle(), this.target.GetProperName());
		}
		return base.GetTitle();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.SpawnContainers();
		Game.Instance.Subscribe(586301400, new Action<object>(this.OnMinionsChanged));
		Components.LiveMinionIdentities.OnAdd += new Action<MinionIdentity>(this.OnMinionsChanged);
		Components.LiveMinionIdentities.OnRemove += new Action<MinionIdentity>(this.OnMinionsChanged);
	}

	private void OnMinionsChanged(object data)
	{
		if (this.target == null)
		{
			return;
		}
		this.Refresh();
	}

	private void SpawnContainers()
	{
		if (this.containersSpawned)
		{
			return;
		}
		this.standardMinionSectionHeader = Util.KInstantiateUI(this.entityCategoryPrefab, this.scrollContents, true);
		this.standardMinionSectionContent = this.standardMinionSectionHeader.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").gameObject;
		this.bionicMinionSectionHeader = Util.KInstantiateUI(this.entityCategoryPrefab, this.scrollContents, true);
		this.bionicMinionSectionContent = this.bionicMinionSectionHeader.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").gameObject;
		this.robotSectionHeader = Util.KInstantiateUI(this.entityCategoryPrefab, this.scrollContents, true);
		this.robotSectionContent = this.robotSectionHeader.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").gameObject;
		this.containersSpawned = true;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<AccessControl>() != null && target.GetComponent<AccessControl>().controlEnabled;
	}

	public override void SetTarget(GameObject target)
	{
		if (this.target != null)
		{
			this.ClearTarget();
		}
		this.SpawnContainers();
		this.target = target.GetComponent<AccessControl>();
		this.doorTarget = target.GetComponent<Door>();
		if (this.target == null)
		{
			return;
		}
		target.Subscribe(1734268753, new Action<object>(this.OnDoorStateChanged));
		target.Subscribe(-1525636549, new Action<object>(this.OnAccessControlChanged));
		base.gameObject.SetActive(true);
		this.RefreshContainerObjects();
		this.Refresh();
	}

	public override void ClearTarget()
	{
		base.ClearTarget();
		if (this.target != null)
		{
			this.target.Unsubscribe(1734268753, new Action<object>(this.OnDoorStateChanged));
			this.target.Unsubscribe(-1525636549, new Action<object>(this.OnAccessControlChanged));
		}
	}

	private void Refresh()
	{
		Rotatable component = this.target.GetComponent<Rotatable>();
		if (component != null)
		{
			bool isRotated = component.IsRotated;
		}
		this.ClearOldRows();
		this.PopulateRows();
		this.standardMinionSectionHeader.GetComponent<HierarchyReferences>().GetReference<RectTransform>("EmptyRow").gameObject.SetActive(this.standardMinionSectionContent.transform.childCount <= 1);
		if (this.standardMinionSectionContent.transform.childCount <= 1)
		{
			this.ToggleCategoryCollapsed(false, this.standardMinionSectionContent.rectTransform(), this.standardMinionSectionHeader.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("CollapseToggle"));
		}
		this.bionicMinionSectionHeader.GetComponent<HierarchyReferences>().GetReference<RectTransform>("EmptyRow").gameObject.SetActive(this.bionicMinionSectionContent.transform.childCount <= 1);
		if (this.bionicMinionSectionContent.transform.childCount <= 1)
		{
			this.ToggleCategoryCollapsed(false, this.bionicMinionSectionContent.rectTransform(), this.bionicMinionSectionHeader.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("CollapseToggle"));
		}
		this.robotSectionHeader.GetComponent<HierarchyReferences>().GetReference<RectTransform>("EmptyRow").gameObject.SetActive(this.robotSectionContent.transform.childCount <= 1);
		if (!this.robotsHasEverBeenOpened)
		{
			this.ToggleCategoryCollapsed(false, this.robotSectionContent.rectTransform(), this.robotSectionHeader.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("CollapseToggle"));
		}
		foreach (GameObject gameObject in this.setInactiveQueue)
		{
			gameObject.SetActive(false);
		}
		this.disabledOverlay.SetActive(this.target.GetComponent<AccessControl>().overrideAccess == Door.ControlState.Locked);
	}

	private void ClearOldRows()
	{
		foreach (KeyValuePair<MinionAssignablesProxy, GameObject> keyValuePair in this.minionIdentityRows)
		{
			this.inactiveRowPool.Add(keyValuePair.Value);
			this.setInactiveQueue.Add(keyValuePair.Value);
		}
		this.minionIdentityRows.Clear();
		foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.robotRows)
		{
			this.inactiveRowPool.Add(keyValuePair2.Value);
			this.setInactiveQueue.Add(keyValuePair2.Value);
		}
		this.robotRows.Clear();
	}

	private void RefreshContainerObjects()
	{
		this.<RefreshContainerObjects>g__RefreshContainer|29_0(this.standardMinionSectionHeader, GameTags.Minions.Models.Standard, true);
		this.<RefreshContainerObjects>g__RefreshContainer|29_0(this.bionicMinionSectionHeader, GameTags.Minions.Models.Bionic, Game.IsDlcActiveForCurrentSave("DLC3_ID"));
		this.<RefreshContainerObjects>g__RefreshContainer|29_0(this.robotSectionHeader, GameTags.Robot, true);
	}

	private void ToggleCategoryCollapsed(bool targetState, RectTransform content, MultiToggle collapseToggle)
	{
		content.gameObject.SetActive(targetState);
		collapseToggle.ChangeState(content.gameObject.activeSelf ? 1 : 0);
	}

	private GameObject InstantiateIndentityRow(GameObject parent)
	{
		if (this.inactiveRowPool.Count > 0)
		{
			GameObject gameObject = this.inactiveRowPool[0];
			this.inactiveRowPool.Remove(gameObject);
			if (gameObject.transform.parent != parent.transform)
			{
				gameObject.transform.SetParent(parent.transform);
			}
			gameObject.transform.SetAsLastSibling();
			gameObject.SetActive(true);
			if (this.setInactiveQueue.Contains(gameObject))
			{
				this.setInactiveQueue.Remove(gameObject);
			}
			return gameObject;
		}
		return Util.KInstantiateUI(this.rowPrefab, parent, true);
	}

	private void PopulateRows()
	{
		for (int i = 0; i < Components.MinionAssignablesProxy.Count; i++)
		{
			MinionAssignablesProxy minionAssignablesProxy = Components.MinionAssignablesProxy[i];
			if (!minionAssignablesProxy.HasTag(GameTags.Dead))
			{
				this.ConfigureRow(minionAssignablesProxy);
			}
		}
		if (Game.IsDlcActiveForCurrentSave("DLC3_ID"))
		{
			this.ConfigureRow(GameTags.Robots.Models.FetchDrone);
		}
		if (Game.IsDlcActiveForCurrentSave("EXPANSION1_ID"))
		{
			this.ConfigureRow(GameTags.Robots.Models.ScoutRover);
		}
		this.ConfigureRow(GameTags.Robots.Models.MorbRover);
	}

	private void ConfigureRow(object entity)
	{
		GameObject gameObject = null;
		MinionAssignablesProxy minion = entity as MinionAssignablesProxy;
		Tag robotTag = GameTags.Robot;
		if (entity is Tag)
		{
			robotTag = (Tag)entity;
		}
		if (minion != null)
		{
			GameObject targetGameObject = minion.GetTargetGameObject();
			StoredMinionIdentity component = targetGameObject.GetComponent<StoredMinionIdentity>();
			if (component != null)
			{
				if (component.model == GameTags.Minions.Models.Standard)
				{
					gameObject = this.standardMinionSectionContent;
				}
				else if (component.model == GameTags.Minions.Models.Bionic)
				{
					gameObject = this.bionicMinionSectionContent;
				}
			}
			else if (targetGameObject.HasTag(GameTags.Minions.Models.Standard))
			{
				gameObject = this.standardMinionSectionContent;
			}
			else if (targetGameObject.HasTag(GameTags.Minions.Models.Bionic))
			{
				gameObject = this.bionicMinionSectionContent;
			}
		}
		else
		{
			gameObject = this.robotSectionContent;
		}
		GameObject gameObject2 = this.InstantiateIndentityRow(gameObject);
		HierarchyReferences component2 = gameObject2.GetComponent<HierarchyReferences>();
		CrewPortrait reference = component2.GetReference<CrewPortrait>("Portrait");
		RectTransform reference2 = component2.GetReference<RectTransform>("Icon");
		if (minion != null)
		{
			if ((global::UnityEngine.Object)reference.identityObject != minion)
			{
				reference.SetIdentityObject(minion, false);
			}
			reference.transform.parent.gameObject.SetActive(true);
			reference2.gameObject.SetActive(false);
		}
		else
		{
			reference.transform.parent.gameObject.SetActive(false);
			reference2.gameObject.SetActive(true);
			reference2.GetComponent<Image>().sprite = Def.GetUISprite(robotTag, "ui", false).first;
			component2.GetReference<LocText>("NameLabel").SetText(robotTag.ProperName());
		}
		MultiToggle reference3 = component2.GetReference<MultiToggle>("UseDefaultButton");
		reference3.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.MINION_SELECT_TOOLTIP);
		if (minion != null)
		{
			reference3.ChangeState(this.target.IsDefaultPermission(minion) ? 1 : 0);
			component2.GetReference<LocText>("AccessSettingLabel").SetText(this.target.IsDefaultPermission(minion) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_DEFAULT : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_CUSTOM);
		}
		else
		{
			reference3.ChangeState(this.target.IsDefaultPermission(robotTag) ? 1 : 0);
			component2.GetReference<LocText>("AccessSettingLabel").SetText(this.target.IsDefaultPermission(robotTag) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_DEFAULT : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_CUSTOM);
		}
		reference3.onClick = delegate
		{
			if (minion != null)
			{
				if (this.target.IsDefaultPermission(minion))
				{
					this.target.SetPermission(minion, this.target.GetDefaultPermission(minion.GetMinionModel()));
				}
				else
				{
					this.target.ClearPermission(minion);
				}
			}
			else if (this.target.IsDefaultPermission(robotTag))
			{
				this.target.ClearPermission(robotTag, GameTags.Robot);
				this.target.SetPermission(robotTag, this.target.GetDefaultPermission(robotTag));
			}
			else
			{
				this.target.ClearPermission(robotTag, GameTags.Robot);
			}
			this.Refresh();
		};
		MultiToggle reference4 = component2.GetReference<MultiToggle>("ToggleLeft");
		MultiToggle reference5 = component2.GetReference<MultiToggle>("ToggleRight");
		AccessControl.Permission permission;
		if (minion != null)
		{
			permission = this.target.GetSetPermission(minion);
		}
		else
		{
			permission = this.target.GetSetPermission(robotTag);
		}
		bool flag = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoLeft;
		bool flag2 = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoRight;
		reference4.ChangeState(flag ? 0 : 1);
		reference5.ChangeState(flag2 ? 0 : 1);
		reference4.onClick = delegate
		{
			if (minion != null)
			{
				switch (this.target.GetSetPermission(minion))
				{
				case AccessControl.Permission.Both:
					this.target.SetPermission(minion, AccessControl.Permission.GoRight);
					break;
				case AccessControl.Permission.GoLeft:
					this.target.SetPermission(minion, AccessControl.Permission.Neither);
					break;
				case AccessControl.Permission.GoRight:
					this.target.SetPermission(minion, AccessControl.Permission.Both);
					break;
				case AccessControl.Permission.Neither:
					this.target.SetPermission(minion, AccessControl.Permission.GoLeft);
					break;
				}
			}
			else
			{
				switch (this.target.GetSetPermission(robotTag))
				{
				case AccessControl.Permission.Both:
					this.target.SetPermission(robotTag, AccessControl.Permission.GoRight);
					break;
				case AccessControl.Permission.GoLeft:
					this.target.SetPermission(robotTag, AccessControl.Permission.Neither);
					break;
				case AccessControl.Permission.GoRight:
					this.target.SetPermission(robotTag, AccessControl.Permission.Both);
					break;
				case AccessControl.Permission.Neither:
					this.target.SetPermission(robotTag, AccessControl.Permission.GoLeft);
					break;
				}
			}
			this.Refresh();
		};
		reference5.onClick = delegate
		{
			if (minion != null)
			{
				switch (this.target.GetSetPermission(minion))
				{
				case AccessControl.Permission.Both:
					this.target.SetPermission(minion, AccessControl.Permission.GoLeft);
					break;
				case AccessControl.Permission.GoLeft:
					this.target.SetPermission(minion, AccessControl.Permission.Both);
					break;
				case AccessControl.Permission.GoRight:
					this.target.SetPermission(minion, AccessControl.Permission.Neither);
					break;
				case AccessControl.Permission.Neither:
					this.target.SetPermission(minion, AccessControl.Permission.GoRight);
					break;
				}
			}
			else
			{
				switch (this.target.GetSetPermission(robotTag))
				{
				case AccessControl.Permission.Both:
					this.target.SetPermission(robotTag, AccessControl.Permission.GoLeft);
					break;
				case AccessControl.Permission.GoLeft:
					this.target.SetPermission(robotTag, AccessControl.Permission.Both);
					break;
				case AccessControl.Permission.GoRight:
					this.target.SetPermission(robotTag, AccessControl.Permission.Neither);
					break;
				case AccessControl.Permission.Neither:
					this.target.SetPermission(robotTag, AccessControl.Permission.GoRight);
					break;
				}
			}
			this.Refresh();
		};
		GameObject gameObject3 = component2.GetReference<RectTransform>("DirectionToggles").gameObject;
		RectTransform reference6 = component2.GetReference<RectTransform>("DittoMark");
		if (minion != null)
		{
			gameObject3.SetActive(!this.target.IsDefaultPermission(minion));
			reference6.gameObject.SetActive(this.target.IsDefaultPermission(minion));
		}
		else
		{
			gameObject3.SetActive(!this.target.IsDefaultPermission(robotTag));
			reference6.gameObject.SetActive(this.target.IsDefaultPermission(robotTag));
		}
		if (minion != null)
		{
			this.minionIdentityRows.Add(minion, gameObject2);
			return;
		}
		this.robotRows.Add(robotTag, gameObject2);
	}

	private void OnDoorStateChanged(object data)
	{
		this.Refresh();
	}

	private void OnAccessControlChanged(object data)
	{
		this.Refresh();
	}

	[CompilerGenerated]
	private void <RefreshContainerObjects>g__RefreshContainer|29_0(GameObject container, Tag containerTag, bool enabled)
	{
		if (!enabled)
		{
			container.SetActive(false);
			return;
		}
		container.SetActive(true);
		HierarchyReferences component = container.GetComponent<HierarchyReferences>();
		MultiToggle reference = component.GetReference<MultiToggle>("ToggleLeft");
		MultiToggle reference2 = component.GetReference<MultiToggle>("ToggleRight");
		component.GetReference<LocText>("CategoryLabel");
		MultiToggle collapseToggle = component.GetReference<MultiToggle>("CollapseToggle");
		RectTransform content = component.GetReference<RectTransform>("Content");
		component.GetReference<LocText>("CategoryLabel").SetText(AccessControlSideScreen.categoryNames[containerTag]);
		component.GetReference<ToolTip>("HeaderTooltip").SetSimpleTooltip(UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.CATEGORY_HEADER_TOOLTIP);
		AccessControl.Permission defaultPermission = this.target.GetDefaultPermission(containerTag);
		bool flag = defaultPermission == AccessControl.Permission.Both || defaultPermission == AccessControl.Permission.GoLeft;
		bool flag2 = defaultPermission == AccessControl.Permission.Both || defaultPermission == AccessControl.Permission.GoRight;
		reference.ChangeState(flag ? 0 : 1);
		reference2.ChangeState(flag2 ? 0 : 1);
		reference.onClick = delegate
		{
			switch (this.target.GetDefaultPermission(containerTag))
			{
			case AccessControl.Permission.Both:
				this.target.SetDefaultPermission(containerTag, AccessControl.Permission.GoRight);
				break;
			case AccessControl.Permission.GoLeft:
				this.target.SetDefaultPermission(containerTag, AccessControl.Permission.Neither);
				break;
			case AccessControl.Permission.GoRight:
				this.target.SetDefaultPermission(containerTag, AccessControl.Permission.Both);
				break;
			case AccessControl.Permission.Neither:
				this.target.SetDefaultPermission(containerTag, AccessControl.Permission.GoLeft);
				break;
			}
			this.RefreshContainerObjects();
		};
		reference2.onClick = delegate
		{
			switch (this.target.GetDefaultPermission(containerTag))
			{
			case AccessControl.Permission.Both:
				this.target.SetDefaultPermission(containerTag, AccessControl.Permission.GoLeft);
				break;
			case AccessControl.Permission.GoLeft:
				this.target.SetDefaultPermission(containerTag, AccessControl.Permission.Both);
				break;
			case AccessControl.Permission.GoRight:
				this.target.SetDefaultPermission(containerTag, AccessControl.Permission.Neither);
				break;
			case AccessControl.Permission.Neither:
				this.target.SetDefaultPermission(containerTag, AccessControl.Permission.GoRight);
				break;
			}
			this.RefreshContainerObjects();
		};
		collapseToggle.onClick = delegate
		{
			if (containerTag == GameTags.Robot)
			{
				this.robotsHasEverBeenOpened = true;
			}
			this.ToggleCategoryCollapsed(!content.gameObject.activeSelf, content, collapseToggle);
		};
	}

	[SerializeField]
	private GameObject entityCategoryPrefab;

	[SerializeField]
	private GameObject rowPrefab;

	[SerializeField]
	private GameObject disabledOverlay;

	[SerializeField]
	private KImage headerBG;

	[SerializeField]
	private GameObject scrollContents;

	private GameObject standardMinionSectionHeader;

	private GameObject standardMinionSectionContent;

	private GameObject bionicMinionSectionHeader;

	private GameObject bionicMinionSectionContent;

	private GameObject robotSectionHeader;

	private GameObject robotSectionContent;

	private AccessControl target;

	private Door doorTarget;

	private bool containersSpawned;

	private List<GameObject> inactiveRowPool = new List<GameObject>();

	private Dictionary<MinionAssignablesProxy, GameObject> minionIdentityRows = new Dictionary<MinionAssignablesProxy, GameObject>();

	private Dictionary<Tag, GameObject> robotRows = new Dictionary<Tag, GameObject>();

	private static Dictionary<Tag, string> categoryNames = new Dictionary<Tag, string>
	{
		{
			GameTags.Minions.Models.Standard,
			DUPLICANTS.MODEL.STANDARD.NAME_ADJECTIVE
		},
		{
			GameTags.Minions.Models.Bionic,
			DUPLICANTS.MODEL.BIONIC.NAME_ADJECTIVE
		},
		{
			GameTags.Robot,
			ROBOTS.CATEGORY_NAME
		}
	};

	private List<GameObject> setInactiveQueue = new List<GameObject>();

	private bool robotsHasEverBeenOpened;
}
