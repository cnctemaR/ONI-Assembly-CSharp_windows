using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MinionPersonalityPanel : DetailScreenTab
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<MinionIdentity>() != null;
	}

	protected override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		this.Refresh();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.bioPanel = base.CreateCollapsableSection(UI.DETAILTABS.PERSONALITY.GROUPNAME_BIO);
		this.traitsPanel = base.CreateCollapsableSection(UI.DETAILTABS.STATS.GROUPNAME_TRAITS);
		this.attributesPanel = base.CreateCollapsableSection(UI.DETAILTABS.STATS.GROUPNAME_ATTRIBUTES);
		this.resumePanel = base.CreateCollapsableSection(UI.DETAILTABS.PERSONALITY.GROUPNAME_RESUME);
		this.amenitiesPanel = base.CreateCollapsableSection(UI.DETAILTABS.PERSONALITY.EQUIPMENT.GROUPNAME_ROOMS);
		this.equipmentPanel = base.CreateCollapsableSection(UI.DETAILTABS.PERSONALITY.EQUIPMENT.GROUPNAME_OWNABLE);
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

	private void ScheduleUpdate()
	{
		this.updateHandle = UIScheduler.Instance.Schedule("RefreshMinionPersonalityPanel", 1f, delegate(object o)
		{
			this.Refresh();
			this.ScheduleUpdate();
		}, null, null);
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
		MinionPersonalityPanel.RefreshBioPanel(this.bioPanel, this.selectedTarget);
		MinionPersonalityPanel.RefreshTraitsPanel(this.traitsPanel, this.selectedTarget);
		MinionPersonalityPanel.RefreshAmenitiesPanel(this.amenitiesPanel, this.selectedTarget);
		MinionPersonalityPanel.RefreshEquipmentPanel(this.equipmentPanel, this.selectedTarget);
		MinionPersonalityPanel.RefreshResumePanel(this.resumePanel, this.selectedTarget);
		MinionPersonalityPanel.RefreshAttributesPanel(this.attributesPanel, this.selectedTarget);
	}

	private static void RefreshBioPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		MinionIdentity component = targetEntity.GetComponent<MinionIdentity>();
		if (!component)
		{
			targetPanel.SetActive(false);
			return;
		}
		targetPanel.SetActive(true);
		targetPanel.SetLabel("name", DUPLICANTS.NAMETITLE + component.name, "");
		targetPanel.SetLabel("model", DUPLICANTS.MODELTITLE + component.model.ProperName(), GameTags.Minions.Models.GetModelTooltipForTag(component.model));
		targetPanel.SetLabel("age", DUPLICANTS.ARRIVALTIME + GameUtil.GetFormattedCycles(((float)GameClock.Instance.GetCycle() - component.arrivalTime) * 600f, "F0", true), string.Format(DUPLICANTS.ARRIVALTIME_TOOLTIP, component.arrivalTime + 1f, component.name));
		targetPanel.SetLabel("gender", DUPLICANTS.GENDERTITLE + string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.GENDER.{0}.NAME", component.genderStringKey.ToUpper())), component.gender), "");
		targetPanel.SetLabel("personality", string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", component.nameStringKey.ToUpper())), component.name), string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", component.nameStringKey.ToUpper())), component.name));
		MinionResume component2 = targetEntity.GetComponent<MinionResume>();
		if (component2 != null && component2.AptitudeBySkillGroup.Count > 0)
		{
			targetPanel.SetLabel("interestHeader", UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.NAME + "\n", string.Format(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.TOOLTIP, targetEntity.name));
			foreach (KeyValuePair<HashedString, float> keyValuePair in component2.AptitudeBySkillGroup)
			{
				if (keyValuePair.Value != 0f)
				{
					SkillGroup skillGroup = Db.Get().SkillGroups.TryGet(keyValuePair.Key);
					if (skillGroup != null)
					{
						targetPanel.SetLabel(skillGroup.Name, "  • " + skillGroup.Name, string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, skillGroup.Name, keyValuePair.Value));
					}
				}
			}
		}
		targetPanel.Commit();
	}

	private static void RefreshTraitsPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		if (!targetEntity.GetComponent<MinionIdentity>())
		{
			targetPanel.SetActive(false);
			return;
		}
		targetPanel.SetActive(true);
		foreach (Trait trait in targetEntity.GetComponent<Traits>().TraitList)
		{
			if (!string.IsNullOrEmpty(trait.Name))
			{
				targetPanel.SetLabel(trait.Id, trait.Name, trait.GetTooltip());
			}
		}
		targetPanel.Commit();
	}

	private static void RefreshEquipmentPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		Assignables equipment = targetEntity.GetComponent<MinionIdentity>().GetEquipment();
		bool flag = false;
		foreach (AssignableSlotInstance assignableSlotInstance in equipment.Slots)
		{
			if (assignableSlotInstance.slot.showInUI && assignableSlotInstance.IsAssigned())
			{
				flag = true;
				string name = assignableSlotInstance.assignable.GetComponent<KSelectable>().GetName();
				string text = "";
				List<Descriptor> list = new List<Descriptor>(GameUtil.GetGameObjectEffects(assignableSlotInstance.assignable.gameObject, false));
				if (list.Count > 0)
				{
					text += "\n";
					foreach (Descriptor descriptor in list)
					{
						text = text + "  • " + descriptor.IndentedText() + "\n";
					}
				}
				targetPanel.SetLabel(assignableSlotInstance.slot.Name, string.Format("{0}: {1}", assignableSlotInstance.slot.Name, name), string.Format(UI.DETAILTABS.PERSONALITY.EQUIPMENT.ASSIGNED_TOOLTIP, name, text, targetEntity.GetProperName()));
			}
		}
		if (!flag)
		{
			targetPanel.SetLabel("NoSuitAssigned", UI.DETAILTABS.PERSONALITY.EQUIPMENT.NOEQUIPMENT, string.Format(UI.DETAILTABS.PERSONALITY.EQUIPMENT.NOEQUIPMENT_TOOLTIP, targetEntity.GetProperName()));
		}
		targetPanel.Commit();
	}

	private static void RefreshAmenitiesPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		Assignables soleOwner = targetEntity.GetComponent<MinionIdentity>().GetSoleOwner();
		bool flag = false;
		foreach (AssignableSlotInstance assignableSlotInstance in soleOwner.Slots)
		{
			if (assignableSlotInstance.slot.showInUI && assignableSlotInstance.IsAssigned())
			{
				flag = true;
				string name = assignableSlotInstance.assignable.GetComponent<KSelectable>().GetName();
				string text = "";
				List<Descriptor> list = new List<Descriptor>(GameUtil.GetGameObjectEffects(assignableSlotInstance.assignable.gameObject, false));
				if (list.Count > 0)
				{
					text += "\n";
					foreach (Descriptor descriptor in list)
					{
						text = text + "  • " + descriptor.IndentedText() + "\n";
					}
				}
				targetPanel.SetLabel(assignableSlotInstance.slot.Name, string.Format("{0}: {1}", assignableSlotInstance.slot.Name, name), string.Format(UI.DETAILTABS.PERSONALITY.EQUIPMENT.ASSIGNED_TOOLTIP, name, text, targetEntity.GetProperName()));
			}
		}
		if (!flag)
		{
			targetPanel.SetLabel("NothingAssigned", UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES, string.Format(UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES_TOOLTIP, targetEntity.GetProperName()));
		}
		targetPanel.Commit();
	}

	private static void RefreshAttributesPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		if (!targetEntity.GetComponent<MinionIdentity>())
		{
			targetPanel.SetActive(false);
			return;
		}
		List<AttributeInstance> list = new List<AttributeInstance>(targetEntity.GetAttributes().AttributeTable).FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill);
		if (list.Count > 0)
		{
			foreach (AttributeInstance attributeInstance in list)
			{
				targetPanel.SetLabel(attributeInstance.Id, string.Format("{0}: {1}", attributeInstance.Name, attributeInstance.GetFormattedValue()), attributeInstance.GetAttributeValueTooltip());
			}
		}
		targetPanel.Commit();
	}

	private static void RefreshResumePanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		MinionResume component = targetEntity.GetComponent<MinionResume>();
		targetPanel.SetTitle(string.Format(UI.DETAILTABS.PERSONALITY.GROUPNAME_RESUME, targetEntity.name.ToUpper()));
		List<Skill> list = new List<Skill>();
		foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
		{
			if (keyValuePair.Value)
			{
				Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
				list.Add(skill);
			}
		}
		targetPanel.SetLabel("mastered_skills_header", UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_SKILLS, UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_SKILLS_TOOLTIP);
		if (list.Count == 0)
		{
			targetPanel.SetLabel("no_skills", "  • " + UI.DETAILTABS.PERSONALITY.RESUME.NO_MASTERED_SKILLS.NAME, string.Format(UI.DETAILTABS.PERSONALITY.RESUME.NO_MASTERED_SKILLS.TOOLTIP, targetEntity.name));
		}
		else
		{
			foreach (Skill skill2 in list)
			{
				if (Game.IsCorrectDlcActiveForCurrentSave(skill2))
				{
					string text = "";
					foreach (SkillPerk skillPerk in skill2.perks)
					{
						if (Game.IsCorrectDlcActiveForCurrentSave(skillPerk))
						{
							text = text + "  • " + skillPerk.Name + "\n";
						}
					}
					targetPanel.SetLabel(skill2.Id, "  • " + skill2.Name, skill2.description + "\n" + text);
				}
			}
		}
		targetPanel.Commit();
	}

	private CollapsibleDetailContentPanel bioPanel;

	private CollapsibleDetailContentPanel traitsPanel;

	private CollapsibleDetailContentPanel resumePanel;

	private CollapsibleDetailContentPanel attributesPanel;

	private CollapsibleDetailContentPanel equipmentPanel;

	private CollapsibleDetailContentPanel amenitiesPanel;

	private SchedulerHandle updateHandle;
}
