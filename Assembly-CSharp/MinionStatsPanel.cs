using System;
using System.Collections.Generic;
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
		this.stressPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.attributesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.traitsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.attributesDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.attributesPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.stressDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.stressPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
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
		this.RefreshAttributes();
		this.RefreshTraits();
		this.RefreshStress();
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
		int num2 = reportEntry.contextEntries.FindIndex((ReportManager.ReportEntry entry) => entry.context == identity.GetProperName());
		ReportManager.ReportEntry reportEntry2 = ((num2 == -1) ? null : reportEntry.contextEntries[num2]);
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

	private GameObject attributesPanel;

	private GameObject stressPanel;

	private GameObject traitsPanel;

	private DetailsPanelDrawer attributesDrawer;

	private DetailsPanelDrawer stressDrawer;

	private DetailsPanelDrawer traitsDrawer;

	private SchedulerHandle updateHandle;

	private List<ReportManager.ReportEntry.Note> stressNotes = new List<ReportManager.ReportEntry.Note>();
}
