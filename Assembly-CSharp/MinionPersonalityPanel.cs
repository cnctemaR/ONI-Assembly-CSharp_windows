using System;
using System.Collections.Generic;
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
		this.resumePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.bioDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.bioPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.resumeDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.resumePanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
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
		this.RefreshResume();
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
			.Tooltip(string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", component.nameStringKey.ToUpper())), component.name))
			.EndDrawing();
	}

	private void RefreshResume()
	{
		MinionResume component = this.selectedTarget.GetComponent<MinionResume>();
		if (!component)
		{
			this.resumePanel.SetActive(false);
			return;
		}
		this.resumePanel.SetActive(true);
		this.resumePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = string.Format(UI.DETAILTABS.PERSONALITY.GROUPNAME_RESUME, this.selectedTarget.name.ToUpper());
		this.resumeDrawer.BeginDrawing();
		RoleConfig roleConfig = Game.Instance.roleManager.GetRole(component.CurrentRole);
		if (roleConfig.id == "NoRole")
		{
			this.resumeDrawer.NewLabel(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.CURRENT_ROLE.NAME, roleConfig.name) + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.CURRENT_ROLE.NOJOB_TOOLTIP, this.selectedTarget.name, roleConfig.name));
		}
		else
		{
			this.resumeDrawer.NewLabel(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.CURRENT_ROLE.NAME, roleConfig.name) + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.CURRENT_ROLE.TOOLTIP, this.selectedTarget.name, roleConfig.name));
		}
		int num = 0;
		if (num != 0)
		{
			this.resumeDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_ROLES).Tooltip(UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_ROLES_TOOLTIP);
		}
		foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryByRoleID)
		{
			if (keyValuePair.Value && !(keyValuePair.Key == "NoRole"))
			{
				roleConfig = Game.Instance.roleManager.GetRole(keyValuePair.Key);
				this.resumeDrawer.NewLabel(roleConfig.name).Tooltip(Game.Instance.roleManager.RoleTooltip(roleConfig.id));
				num++;
			}
		}
		int num2 = 0;
		if (num2 != 0)
		{
			this.resumeDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.PERKS.NAME + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.PERKS.TOOLTIP, this.selectedTarget.name));
		}
		foreach (KeyValuePair<HashedString, RoleGroup> keyValuePair2 in Game.Instance.roleManager.RoleGroups)
		{
			foreach (RoleConfig roleConfig2 in keyValuePair2.Value.roles)
			{
				if (roleConfig2.id == component.CurrentRole || component.MasteryByRoleID[roleConfig2.id])
				{
					foreach (RolePerk rolePerk in roleConfig2.perks)
					{
						this.resumeDrawer.NewLabel("  • " + rolePerk.description).Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.JOBTRAINING_TOOLTIP, this.selectedTarget.name, roleConfig2.GetProperName()));
						num2++;
					}
				}
			}
		}
		this.resumeDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.NAME + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.TOOLTIP, this.selectedTarget.name));
		if (component.AptitudeByRoleGroup.Count > 0)
		{
			foreach (KeyValuePair<HashedString, float> keyValuePair3 in component.AptitudeByRoleGroup)
			{
				if (keyValuePair3.Value != 0f)
				{
					this.resumeDrawer.NewLabel("  • " + Game.Instance.roleManager.RoleGroups[keyValuePair3.Key].Name).Tooltip(string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, Game.Instance.roleManager.RoleGroups[keyValuePair3.Key].Name, keyValuePair3.Value * ROLES.APTITUDE_EXPERIENCE_SCALE));
				}
			}
		}
		this.resumeDrawer.EndDrawing();
	}

	public GameObject attributesLabelTemplate;

	private GameObject bioPanel;

	private GameObject resumePanel;

	private DetailsPanelDrawer bioDrawer;

	private DetailsPanelDrawer resumeDrawer;

	public MinionEquipmentPanel panel;

	private SchedulerHandle updateHandle;
}
