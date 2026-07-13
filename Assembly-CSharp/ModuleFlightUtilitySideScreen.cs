using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ModuleFlightUtilitySideScreen : SideScreenContent
{
	private CraftModuleInterface craftModuleInterface
	{
		get
		{
			return this.targetCraft.GetComponent<CraftModuleInterface>();
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		base.ConsumeMouseScroll = true;
	}

	public override float GetSortKey()
	{
		return 21f;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (target.GetComponent<Clustercraft>() != null && this.HasFlightUtilityModule(target.GetComponent<CraftModuleInterface>()))
		{
			return true;
		}
		RocketControlStation component = target.GetComponent<RocketControlStation>();
		return component != null && this.HasFlightUtilityModule(component.GetMyWorld().GetComponent<Clustercraft>().ModuleInterface);
	}

	private bool HasFlightUtilityModule(CraftModuleInterface craftModuleInterface)
	{
		using (IEnumerator<Ref<RocketModuleCluster>> enumerator = craftModuleInterface.ClusterModules.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Get().GetSMI<IEmptyableCargo>() != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		if (target != null)
		{
			foreach (int num in this.refreshHandle)
			{
				target.Unsubscribe(num);
			}
			this.refreshHandle.Clear();
		}
		base.SetTarget(target);
		this.targetCraft = target.GetComponent<Clustercraft>();
		if (this.targetCraft == null && target.GetComponent<RocketControlStation>() != null)
		{
			this.targetCraft = target.GetMyWorld().GetComponent<Clustercraft>();
		}
		this.refreshHandle.Add(this.targetCraft.gameObject.Subscribe(-1298331547, new Action<object>(this.RefreshAll)));
		this.refreshHandle.Add(this.targetCraft.gameObject.Subscribe(1792516731, new Action<object>(this.RefreshAll)));
		this.BuildModules();
	}

	private void ClearModules()
	{
		foreach (KeyValuePair<IEmptyableCargo, HierarchyReferences> keyValuePair in this.modulePanels)
		{
			Util.KDestroyGameObject(keyValuePair.Value.gameObject);
		}
		this.modulePanels.Clear();
	}

	private void BuildModules()
	{
		this.ClearModules();
		foreach (Ref<RocketModuleCluster> @ref in this.craftModuleInterface.ClusterModules)
		{
			IEmptyableCargo smi = @ref.Get().GetSMI<IEmptyableCargo>();
			if (smi != null)
			{
				HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.modulePanelPrefab, this.moduleContentContainer, true);
				this.modulePanels.Add(smi, hierarchyReferences);
				this.RefreshModulePanel(smi);
			}
		}
		this.scrollRectLayout.preferredHeight = (this.scrollRectLayout.minHeight = Mathf.Min((float)this.modulePanels.Count, 2.25f) * this.modulePanelPrefab.GetComponent<RectTransform>().rect.height);
	}

	private void RefreshAll(object data = null)
	{
		this.BuildModules();
	}

	private void RefreshModulePanel(IEmptyableCargo module)
	{
		HierarchyReferences hierarchyReferences = this.modulePanels[module];
		hierarchyReferences.GetReference<Image>("icon").sprite = Def.GetUISprite(module.master.gameObject, "ui", false).first;
		hierarchyReferences.GetReference<RectTransform>("targetButtons").gameObject.SetActive(module.CanTargetClusterGridEntities);
		if (module.CanTargetClusterGridEntities)
		{
			KButton reference = hierarchyReferences.GetReference<KButton>("selectTargetButton");
			reference.onClick += delegate
			{
				ClusterMapScreen.Instance.ShowInSelectDestinationMode(module.master.GetComponent<ClusterDestinationSelector>());
			};
			KButton reference2 = hierarchyReferences.GetReference<KButton>("clearTargetButton");
			reference2.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.CLEAR_TARGET_BUTTON_TOOLTIP);
			reference2.onClick += delegate
			{
				module.master.GetComponent<EntityClusterDestinationSelector>().SetDestination(AxialI.INVALID);
				this.RefreshModulePanel(module);
			};
			if (module.master.GetComponent<EntityClusterDestinationSelector>().GetClusterEntityTarget() != null)
			{
				reference.GetComponentInChildren<LocText>().text = (module as StateMachine.Instance).GetMaster().GetComponent<EntityClusterDestinationSelector>().GetClusterEntityTarget()
					.GetProperName();
				reference.isInteractable = false;
			}
			else
			{
				reference.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_TARGET_BUTTON;
				reference.isInteractable = true;
			}
		}
		KButton reference3 = hierarchyReferences.GetReference<KButton>("button");
		reference3.isInteractable = module.CanEmptyCargo();
		reference3.GetComponentInChildren<LocText>().text = module.GetButtonText;
		reference3.GetComponentInChildren<ToolTip>().SetSimpleTooltip(module.GetButtonToolip);
		reference3.ClearOnClick();
		reference3.onClick += module.EmptyCargo;
		KButton reference4 = hierarchyReferences.GetReference<KButton>("repeatButton");
		if (module.CanAutoDeploy)
		{
			this.StyleRepeatButton(module);
			reference4.ClearOnClick();
			reference4.onClick += delegate
			{
				this.OnRepeatClicked(module);
			};
			reference4.gameObject.SetActive(true);
		}
		else
		{
			reference4.gameObject.SetActive(false);
		}
		DropDown reference5 = hierarchyReferences.GetReference<DropDown>("dropDown");
		reference5.targetDropDownContainer = GameScreenManager.Instance.ssOverlayCanvas;
		reference5.Close();
		CrewPortrait reference6 = hierarchyReferences.GetReference<CrewPortrait>("selectedPortrait");
		WorldContainer component = (module as StateMachine.Instance).GetMaster().GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<WorldContainer>();
		if (component != null && module.ChooseDuplicant)
		{
			if (module.ChosenDuplicant != null && module.ChosenDuplicant.HasTag(GameTags.Dead))
			{
				module.ChosenDuplicant = null;
			}
			int id = component.id;
			reference5.gameObject.SetActive(true);
			reference5.Initialize(Components.LiveMinionIdentities.GetWorldItems(id, false), new Action<IListableOption, object>(this.OnDuplicantEntryClick), null, new Action<DropDownEntry, object>(this.DropDownEntryRefreshAction), true, module);
			reference5.selectedLabel.text = ((module.ChosenDuplicant != null) ? this.GetDuplicantRowName(module.ChosenDuplicant) : UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_DUPLICANT.ToString());
			reference6.gameObject.SetActive(true);
			reference6.SetIdentityObject(module.ChosenDuplicant, false);
			reference5.openButton.isInteractable = !module.ModuleDeployed;
		}
		else
		{
			reference5.gameObject.SetActive(false);
			reference6.gameObject.SetActive(false);
		}
		hierarchyReferences.GetReference<LocText>("label").SetText(module.master.gameObject.GetProperName());
	}

	private string GetDuplicantRowName(MinionIdentity minion)
	{
		MinionResume component = minion.GetComponent<MinionResume>();
		if (component != null && component.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation))
		{
			return string.Format(UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.PILOT_FMT, minion.GetProperName());
		}
		return minion.GetProperName();
	}

	private void OnRepeatClicked(IEmptyableCargo module)
	{
		module.AutoDeploy = !module.AutoDeploy;
		this.StyleRepeatButton(module);
	}

	private void OnDuplicantEntryClick(IListableOption option, object data)
	{
		MinionIdentity minionIdentity = (MinionIdentity)option;
		IEmptyableCargo emptyableCargo = (IEmptyableCargo)data;
		emptyableCargo.ChosenDuplicant = minionIdentity;
		HierarchyReferences hierarchyReferences = this.modulePanels[emptyableCargo];
		hierarchyReferences.GetReference<DropDown>("dropDown").selectedLabel.text = ((emptyableCargo.ChosenDuplicant != null) ? this.GetDuplicantRowName(emptyableCargo.ChosenDuplicant) : UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_DUPLICANT.ToString());
		hierarchyReferences.GetReference<CrewPortrait>("selectedPortrait").SetIdentityObject(emptyableCargo.ChosenDuplicant, false);
		this.RefreshAll(null);
	}

	private void DropDownEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		MinionIdentity minionIdentity = (MinionIdentity)entry.entryData;
		entry.label.text = this.GetDuplicantRowName(minionIdentity);
		entry.portrait.SetIdentityObject(minionIdentity, false);
		bool flag = false;
		foreach (Ref<RocketModuleCluster> @ref in this.targetCraft.ModuleInterface.ClusterModules)
		{
			RocketModuleCluster rocketModuleCluster = @ref.Get();
			if (!(rocketModuleCluster == null))
			{
				IEmptyableCargo smi = rocketModuleCluster.GetSMI<IEmptyableCargo>();
				if (smi != null && !(((IEmptyableCargo)targetData).ChosenDuplicant == minionIdentity))
				{
					flag = flag || smi.ChosenDuplicant == minionIdentity;
				}
			}
		}
		entry.button.isInteractable = !flag;
	}

	private void StyleRepeatButton(IEmptyableCargo module)
	{
		KButton reference = this.modulePanels[module].GetReference<KButton>("repeatButton");
		reference.bgImage.colorStyleSetting = (module.AutoDeploy ? this.repeatOn : this.repeatOff);
		reference.bgImage.ApplyColorStyleSetting();
	}

	private Clustercraft targetCraft;

	public GameObject moduleContentContainer;

	public GameObject modulePanelPrefab;

	public ColorStyleSetting repeatOff;

	public ColorStyleSetting repeatOn;

	private Dictionary<IEmptyableCargo, HierarchyReferences> modulePanels = new Dictionary<IEmptyableCargo, HierarchyReferences>();

	[SerializeField]
	private LayoutElement scrollRectLayout;

	private List<int> refreshHandle = new List<int>();
}
