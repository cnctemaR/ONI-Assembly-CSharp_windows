using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MinionStatsPanel : TargetScreen
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<MinionIdentity>();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.resumePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.attributesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.resumeDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.resumePanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.attributesDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.attributesPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
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
		if (this.selectedTarget == null || this.selectedTarget.GetComponent<MinionIdentity>() == null)
		{
			return;
		}
		this.RefreshResume();
		this.RefreshAttributes();
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
		this.resumePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = string.Format(UI.DETAILTABS.PERSONALITY.GROUPNAME_RESUME, this.selectedTarget.name.ToUpper());
		this.resumeDrawer.BeginDrawing();
		List<Skill> list = new List<Skill>();
		foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
		{
			if (keyValuePair.Value)
			{
				Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
				list.Add(skill);
			}
		}
		this.resumeDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_SKILLS).Tooltip(UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_SKILLS_TOOLTIP);
		if (list.Count == 0)
		{
			this.resumeDrawer.NewLabel("  • " + UI.DETAILTABS.PERSONALITY.RESUME.NO_MASTERED_SKILLS.NAME).Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.NO_MASTERED_SKILLS.TOOLTIP, this.selectedTarget.name));
		}
		else
		{
			foreach (Skill skill2 in list)
			{
				string text = string.Empty;
				foreach (SkillPerk skillPerk in skill2.perks)
				{
					text = text + "  • " + skillPerk.Name + "\n";
				}
				this.resumeDrawer.NewLabel("  • " + skill2.Name).Tooltip(skill2.description + "\n" + text);
			}
		}
		this.resumeDrawer.EndDrawing();
	}

	public GameObject attributesLabelTemplate;

	private GameObject resumePanel;

	private GameObject attributesPanel;

	private DetailsPanelDrawer resumeDrawer;

	private DetailsPanelDrawer attributesDrawer;

	private SchedulerHandle updateHandle;
}
