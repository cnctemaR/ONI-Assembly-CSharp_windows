using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class MinionStatsPanel : TargetScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.bioPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.traitsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.expectationsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.stressPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.aptitudePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.attributesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.resumePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.perkPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.attributesDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.attributesPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.stressDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.stressPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.aptitudeDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.aptitudePanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.traitsDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.traitsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.expectationsDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.expectationsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.bioDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.bioPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.resumeDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.resumePanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.perkDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.perkPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
	}

	protected override void OnCleanUp()
	{
		this.updateHandle.ClearScheduler();
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Refresh();
		this.ScheduleUpdate();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		this.Refresh();
	}

	private void ScheduleUpdate()
	{
		this.updateHandle = UIScheduler.Instance.Schedule("RefreshMinionStatsPanel", 1f, delegate(object o)
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
		this.RefreshAttributes();
		this.RefreshTraits();
		this.RefreshStress();
		this.RefreshAptitudes();
		this.RefreshExpectations();
		this.RefreshBio();
		this.RefreshResume();
		this.RefreshPerks();
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
		this.bioPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_BIO;
		this.bioDrawer.BeginDrawing().NewLabel(string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", component.nameStringKey.ToUpper())), component.name)).EndDrawing();
	}

	private void RefreshAttributes()
	{
		MinionIdentity component = this.selectedTarget.GetComponent<MinionIdentity>();
		if (!component)
		{
			this.attributesPanel.SetActive(false);
			return;
		}
		this.attributesPanel.SetActive(true);
		this.attributesPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_ATTRIBUTES;
		List<AttributeInstance> list = new List<AttributeInstance>(this.selectedTarget.GetAttributes().AttributeTable);
		List<AttributeInstance> list2 = list.FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill);
		this.attributesDrawer.BeginDrawing();
		if (list2.Count > 0)
		{
			foreach (AttributeInstance attributeInstance in list2)
			{
				this.attributesDrawer.NewLabel(string.Format("{0}: {1}", attributeInstance.Name, attributeInstance.GetFormattedValue())).Tooltip(attributeInstance.GetAttributeValueTooltip());
			}
		}
		this.attributesDrawer.EndDrawing();
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
		this.resumePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_RESUME;
		this.resumeDrawer.BeginDrawing();
		RoleConfig roleConfig = Game.Instance.roleManager.GetRole(component.CurrentRole);
		this.resumeDrawer.NewLabel(string.Format(UI.DETAILTABS.STATS.RESUME.CURRENT_ROLE, roleConfig.name) + "\n").Tooltip(Game.Instance.roleManager.RoleTooltip(roleConfig.id)).NewLabel(UI.DETAILTABS.STATS.RESUME.MASTERED_ROLES)
			.Tooltip(UI.DETAILTABS.STATS.RESUME.MASTERED_ROLES_TOOLTIP);
		int num = 0;
		foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryByRoleID)
		{
			if (keyValuePair.Value && !(keyValuePair.Key == "NoRole"))
			{
				roleConfig = Game.Instance.roleManager.GetRole(keyValuePair.Key);
				this.resumeDrawer.NewLabel(roleConfig.name).Tooltip(Game.Instance.roleManager.RoleTooltip(roleConfig.id));
				num++;
			}
		}
		if (num == 0)
		{
			this.resumeDrawer.NewLabel(UI.DETAILTABS.STATS.RESUME.NO_MASTERED_ROLES);
		}
		this.resumeDrawer.EndDrawing();
	}

	private void RefreshStress()
	{
		MinionIdentity identity = this.selectedTarget.GetComponent<MinionIdentity>();
		if (!identity)
		{
			this.stressPanel.SetActive(false);
			return;
		}
		this.stressPanel.SetActive(true);
		this.stressPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_STRESS;
		ReportManager.ReportEntry reportEntry = ReportManager.Instance.TodaysReport.reportEntries.Find((ReportManager.ReportEntry entry) => entry.reportType == ReportManager.ReportType.StressDelta);
		this.stressDrawer.BeginDrawing();
		float num = 0f;
		this.stressNotes.Clear();
		ReportManager.ReportEntry reportEntry2 = reportEntry.contextEntries.Find((ReportManager.ReportEntry entry) => entry.context == identity.GetProperName());
		if (reportEntry2 != null)
		{
			reportEntry2.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
			{
				this.stressNotes.Add(note);
			});
			this.stressNotes.Sort((ReportManager.ReportEntry.Note a, ReportManager.ReportEntry.Note b) => a.value.CompareTo(b.value));
			for (int i = 0; i < this.stressNotes.Count; i++)
			{
				this.stressDrawer.NewLabel(string.Concat(new string[]
				{
					(this.stressNotes[i].value <= 0f) ? string.Empty : UIConstants.ColorPrefixRed,
					this.stressNotes[i].note,
					": ",
					Util.FormatTwoDecimalPlace(this.stressNotes[i].value),
					"%",
					(this.stressNotes[i].value <= 0f) ? string.Empty : UIConstants.ColorSuffix
				}));
				num += this.stressNotes[i].value;
			}
		}
		this.stressDrawer.NewLabel(((num <= 0f) ? string.Empty : UIConstants.ColorPrefixRed) + string.Format(UI.DETAILTABS.DETAILS.NET_STRESS, Util.FormatTwoDecimalPlace(num)) + ((num <= 0f) ? string.Empty : UIConstants.ColorSuffix));
		this.stressDrawer.EndDrawing();
	}

	private void RefreshAptitudes()
	{
		MinionResume component = this.selectedTarget.GetComponent<MinionResume>();
		if (!component)
		{
			this.aptitudePanel.SetActive(false);
			return;
		}
		this.aptitudePanel.SetActive(true);
		this.aptitudePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_APTITUDES;
		this.aptitudeDrawer.BeginDrawing();
		if (component.AptitudeByRoleGroup.Count > 0)
		{
			foreach (KeyValuePair<HashedString, float> keyValuePair in component.AptitudeByRoleGroup)
			{
				if (keyValuePair.Value != 0f)
				{
					this.aptitudeDrawer.NewLabel(Game.Instance.roleManager.RoleGroups[keyValuePair.Key].Name).Tooltip(string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, Game.Instance.roleManager.RoleGroups[keyValuePair.Key].Name, keyValuePair.Value * ROLES.APTITUDE_EXPERIENCE_SCALE));
				}
			}
		}
		this.aptitudeDrawer.EndDrawing();
	}

	private void RefreshExpectations()
	{
		MinionIdentity component = this.selectedTarget.GetComponent<MinionIdentity>();
		if (!component)
		{
			this.expectationsPanel.SetActive(false);
			return;
		}
		this.expectationsPanel.SetActive(true);
		this.expectationsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_EXPECTATIONS;
		List<AttributeInstance> list = new List<AttributeInstance>(this.selectedTarget.GetAttributes().AttributeTable);
		List<AttributeInstance> list2 = list.FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.Expectation);
		this.expectationsDrawer.BeginDrawing();
		if (list2.Count > 0)
		{
			foreach (AttributeInstance attributeInstance in list2)
			{
				this.expectationsDrawer.NewLabel(string.Format("{0}: {1}", attributeInstance.Name, attributeInstance.GetFormattedValue())).Tooltip(attributeInstance.GetAttributeValueTooltip());
			}
		}
		this.expectationsDrawer.EndDrawing();
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

	private void RefreshPerks()
	{
		MinionIdentity component = this.selectedTarget.GetComponent<MinionIdentity>();
		if (!component)
		{
			this.perkPanel.SetActive(false);
			return;
		}
		this.perkPanel.SetActive(true);
		this.perkPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_PERKS;
		MinionResume component2 = component.GetComponent<MinionResume>();
		this.perkDrawer.BeginDrawing();
		foreach (KeyValuePair<HashedString, RoleGroup> keyValuePair in Game.Instance.roleManager.RoleGroups)
		{
			foreach (RoleConfig roleConfig in keyValuePair.Value.roles)
			{
				if (roleConfig.id == component2.CurrentRole || component2.MasteryByRoleID[roleConfig.id])
				{
					foreach (RolePerk rolePerk in roleConfig.perks)
					{
						this.perkDrawer.NewLabel(rolePerk.description).Tooltip(roleConfig.GetProperName());
					}
				}
			}
		}
		this.perkDrawer.EndDrawing();
	}

	public GameObject attributesLabelTemplate;

	private GameObject attributesPanel;

	private GameObject stressPanel;

	private GameObject aptitudePanel;

	private GameObject expectationsPanel;

	private GameObject traitsPanel;

	private GameObject bioPanel;

	private GameObject resumePanel;

	private GameObject perkPanel;

	private DetailsPanelDrawer attributesDrawer;

	private DetailsPanelDrawer stressDrawer;

	private DetailsPanelDrawer aptitudeDrawer;

	private DetailsPanelDrawer traitsDrawer;

	private DetailsPanelDrawer expectationsDrawer;

	private DetailsPanelDrawer bioDrawer;

	private DetailsPanelDrawer resumeDrawer;

	private DetailsPanelDrawer perkDrawer;

	private SchedulerHandle updateHandle;

	private List<ReportManager.ReportEntry.Note> stressNotes = new List<ReportManager.ReportEntry.Note>();
}
