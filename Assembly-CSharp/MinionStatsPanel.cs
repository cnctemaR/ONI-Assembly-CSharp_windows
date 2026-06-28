using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MinionStatsPanel : TargetScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.traitsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.attributesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
	}

	private void Update()
	{
		this.Refresh();
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
		this.RefreshAttributes();
		this.RefreshTraits();
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
		List<AttributeInstance> list3 = list.FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.Expectation);
		if (list2.Count > 0)
		{
			GameObject gameObject = this.AddOrGetLabel(this.attributeLabels, this.attributesPanel, "SkillsTitle");
			gameObject.GetComponent<LocText>().text = UI.DETAILTABS.STATS.GROUPNAME_ATTRIBUTES_SKILLS;
			foreach (AttributeInstance attributeInstance in list2)
			{
				gameObject = this.AddOrGetLabel(this.attributeLabels, this.attributesPanel, attributeInstance.Id);
				gameObject.GetComponent<LocText>().text = string.Format("{0}: {1}", attributeInstance.Name, attributeInstance.GetFormattedValue(false));
				gameObject.GetComponent<ToolTip>().toolTip = attributeInstance.GetAttributeValueTooltip();
			}
		}
		if (list3.Count > 0)
		{
			GameObject gameObject2 = this.AddOrGetLabel(this.attributeLabels, this.attributesPanel, "ExpectationsTitle");
			gameObject2.GetComponent<LocText>().text = UI.DETAILTABS.STATS.GROUPNAME_ATTRIBUTES_EXPECTATIONS;
			foreach (AttributeInstance attributeInstance2 in list3)
			{
				gameObject2 = this.AddOrGetLabel(this.attributeLabels, this.attributesPanel, attributeInstance2.Id);
				gameObject2.GetComponent<LocText>().text = string.Format("{0}: {1}", attributeInstance2.Name, attributeInstance2.GetFormattedValue(false));
				gameObject2.GetComponent<ToolTip>().toolTip = attributeInstance2.GetAttributeValueTooltip();
			}
		}
	}

	private void RefreshTraits()
	{
		MinionIdentity component = this.selectedTarget.GetComponent<MinionIdentity>();
		if (!component)
		{
			this.traitsPanel.SetActive(false);
			return;
		}
		Traits component2 = this.selectedTarget.GetComponent<Traits>();
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.traitLabels)
		{
			bool flag = false;
			foreach (Trait trait in component2)
			{
				if (trait.Id == keyValuePair.Key)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				keyValuePair.Value.SetActive(false);
			}
		}
		this.traitsPanel.SetActive(true);
		this.traitsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_TRAITS;
		foreach (Trait trait2 in this.selectedTarget.GetComponent<Traits>().TraitList)
		{
			GameObject gameObject = this.AddOrGetLabel(this.traitLabels, this.traitsPanel, trait2.Id);
			gameObject.GetComponent<LocText>().text = trait2.Name;
			gameObject.GetComponent<ToolTip>().toolTip = trait2.GetTooltip();
		}
	}

	public GameObject attributesLabelTemplate;

	private GameObject attributesPanel;

	private GameObject traitsPanel;

	private Dictionary<string, GameObject> attributeLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> traitLabels = new Dictionary<string, GameObject>();
}
