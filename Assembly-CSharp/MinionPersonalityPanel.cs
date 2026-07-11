using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class MinionPersonalityPanel : TargetScreen
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<MinionIdentity>() != null;
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
	}

	public override void OnSelectTarget(GameObject target)
	{
		this.panel.SetSelectedMinion(target);
		this.panel.Refresh(null);
		base.OnSelectTarget(target);
		this.Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if (this.panel == null)
		{
			this.panel = base.GetComponent<MinionEquipmentPanel>();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.bioPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.traitsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.bioDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.bioPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.traitsDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.traitsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
	}

	protected override void OnCleanUp()
	{
		this.updateHandle.ClearScheduler();
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.panel == null)
		{
			this.panel = base.GetComponent<MinionEquipmentPanel>();
		}
		this.Refresh();
		this.ScheduleUpdate();
	}

	private void ScheduleUpdate()
	{
		this.updateHandle = UIScheduler.Instance.Schedule("RefreshMinionPersonalityPanel", 1f, delegate(object o)
		{
			this.Refresh();
			this.ScheduleUpdate();
		}, null, null);
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(this.attributesLabelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void Refresh()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (this.selectedTarget == null || this.selectedTarget.GetComponent<MinionIdentity>() == null)
		{
			return;
		}
		this.RefreshBio();
		this.RefreshTraits();
	}

	private void RefreshBio()
	{
		MinionIdentity component = this.selectedTarget.GetComponent<MinionIdentity>();
		if (!component)
		{
			this.bioPanel.SetActive(false);
			return;
		}
		this.bioPanel.SetActive(true);
		this.bioPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.PERSONALITY.GROUPNAME_BIO;
		this.bioDrawer.BeginDrawing().NewLabel(DUPLICANTS.NAMETITLE + component.name).NewLabel(DUPLICANTS.ARRIVALTIME + ((float)GameClock.Instance.GetCycle() - component.arrivalTime) + " Cycles")
			.Tooltip(string.Format(DUPLICANTS.ARRIVALTIME_TOOLTIP, component.arrivalTime, component.name))
			.NewLabel(DUPLICANTS.GENDERTITLE + string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.GENDER.{0}.NAME", component.genderStringKey.ToUpper())), component.gender))
			.NewLabel(string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", component.nameStringKey.ToUpper())), component.name))
			.Tooltip(string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", component.nameStringKey.ToUpper())), component.name));
		MinionResume component2 = this.selectedTarget.GetComponent<MinionResume>();
		if (component2 != null && component2.AptitudeBySkillGroup.Count > 0)
		{
			this.bioDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.NAME + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.TOOLTIP, this.selectedTarget.name));
			foreach (KeyValuePair<HashedString, float> keyValuePair in component2.AptitudeBySkillGroup)
			{
				if (keyValuePair.Value != 0f)
				{
					SkillGroup skillGroup = Db.Get().SkillGroups.Get(keyValuePair.Key);
					this.bioDrawer.NewLabel("  • " + skillGroup.Name).Tooltip(string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, skillGroup.Name, keyValuePair.Value * ROLES.APTITUDE_EXPERIENCE_SCALE));
				}
			}
		}
		this.bioDrawer.EndDrawing();
	}

	private void RefreshTraits()
	{
		MinionIdentity component = this.selectedTarget.GetComponent<MinionIdentity>();
		if (!component)
		{
			this.traitsPanel.SetActive(false);
			return;
		}
		this.traitsPanel.SetActive(true);
		this.traitsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_TRAITS;
		this.traitsDrawer.BeginDrawing();
		foreach (Trait trait in this.selectedTarget.GetComponent<Traits>().TraitList)
		{
			this.traitsDrawer.NewLabel(trait.Name).Tooltip(trait.GetTooltip());
		}
		this.traitsDrawer.EndDrawing();
	}

	public GameObject attributesLabelTemplate;

	private GameObject bioPanel;

	private GameObject traitsPanel;

	private DetailsPanelDrawer bioDrawer;

	private DetailsPanelDrawer traitsDrawer;

	public MinionEquipmentPanel panel;

	private SchedulerHandle updateHandle;
}
