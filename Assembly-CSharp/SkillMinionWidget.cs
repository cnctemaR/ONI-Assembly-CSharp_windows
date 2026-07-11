using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillMinionWidget : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
	public IAssignableIdentity minion { get; private set; }

	public void SetMinon(IAssignableIdentity identity)
	{
		this.minion = identity;
		this.portrait.SetIdentityObject(this.minion, true);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.ToggleHover(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.ToggleHover(false);
	}

	private void ToggleHover(bool on)
	{
		if (this.skillsScreen.CurrentlySelectedMinion != this.minion)
		{
			this.SetColor((!on) ? this.unselected_color : this.hover_color);
		}
	}

	private void SetColor(Color color)
	{
		this.background.color = color;
		if (this.minion != null && this.minion as StoredMinionIdentity != null)
		{
			base.GetComponent<CanvasGroup>().alpha = 0.6f;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		this.skillsScreen.CurrentlySelectedMinion = this.minion;
		this.skillsScreen.RefreshSkillWidgets();
		KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
	}

	public void Refresh()
	{
		if (this.minion == null)
		{
			return;
		}
		this.portrait.SetIdentityObject(this.minion, true);
		string text = string.Empty;
		MinionIdentity minionIdentity = this.minion as MinionIdentity;
		this.hatDropDown.gameObject.SetActive(true);
		if (minionIdentity != null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			int availableSkillpoints = component.AvailableSkillpoints;
			int totalSkillPointsGained = component.TotalSkillPointsGained;
			this.masteryPoints.text = ((availableSkillpoints <= 0) ? "0" : GameUtil.ApplyBoldString(GameUtil.ColourizeString(new Color(0.5f, 1f, 0.5f, 1f), availableSkillpoints.ToString())));
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(component);
			AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(component);
			this.morale.text = string.Format("{0}/{1}", attributeInstance.GetTotalValue(), attributeInstance2.GetTotalValue());
			this.RefreshToolTip(component);
			List<IListableOption> list = new List<IListableOption>();
			foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
			{
				if (keyValuePair.Value)
				{
					list.Add(new SkillListable(keyValuePair.Key));
				}
			}
			this.hatDropDown.Initialize(list, new Action<IListableOption, object>(this.OnHatDropEntryClick), new Func<IListableOption, IListableOption, object, int>(this.hatDropDownSort), new Action<DropDownEntry, object>(this.hatDropEntryRefreshAction), false, this.minion);
			text = ((!string.IsNullOrEmpty(component.TargetHat)) ? component.TargetHat : component.CurrentHat);
		}
		else
		{
			StoredMinionIdentity storedMinionIdentity = this.minion as StoredMinionIdentity;
			ToolTip component2 = base.GetComponent<ToolTip>();
			component2.ClearMultiStringTooltip();
			component2.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, storedMinionIdentity.GetStorageReason(), this.minion.GetProperName()), null);
			text = ((!string.IsNullOrEmpty(storedMinionIdentity.targetHat)) ? storedMinionIdentity.targetHat : storedMinionIdentity.currentHat);
			this.masteryPoints.text = UI.TABLESCREENS.NA;
			this.morale.text = UI.TABLESCREENS.NA;
		}
		this.SetColor((this.skillsScreen.CurrentlySelectedMinion != this.minion) ? this.unselected_color : this.selected_color);
		HierarchyReferences component3 = base.GetComponent<HierarchyReferences>();
		this.RefreshHat(text);
		component3.GetReference("openButton").gameObject.SetActive(minionIdentity != null);
	}

	private void RefreshToolTip(MinionResume resume)
	{
		if (resume != null)
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(resume);
			AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(resume);
			ToolTip component = base.GetComponent<ToolTip>();
			component.ClearMultiStringTooltip();
			component.AddMultiStringTooltip(this.minion.GetProperName() + "\n\n", this.TooltipTextStyle_Header);
			component.AddMultiStringTooltip(string.Format(UI.SKILLS_SCREEN.CURRENT_MORALE, attributeInstance.GetTotalValue(), attributeInstance2.GetTotalValue()), null);
			component.AddMultiStringTooltip("\n" + UI.DETAILTABS.STATS.NAME + "\n\n", this.TooltipTextStyle_Header);
			foreach (AttributeInstance attributeInstance3 in resume.GetAttributes())
			{
				if (attributeInstance3.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill)
				{
					string text = UIConstants.ColorPrefixWhite;
					if (attributeInstance3.GetTotalValue() > 0f)
					{
						text = UIConstants.ColorPrefixGreen;
					}
					else if (attributeInstance3.GetTotalValue() < 0f)
					{
						text = UIConstants.ColorPrefixRed;
					}
					component.AddMultiStringTooltip(string.Concat(new object[]
					{
						"    • ",
						attributeInstance3.Name,
						": ",
						text,
						attributeInstance3.GetTotalValue(),
						UIConstants.ColorSuffix
					}), null);
				}
			}
		}
	}

	public void RefreshHat(string hat)
	{
		HierarchyReferences component = base.GetComponent<HierarchyReferences>();
		component.GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite((!string.IsNullOrEmpty(hat)) ? hat : "hat_role_none");
	}

	private void OnHatDropEntryClick(IListableOption skill, object data)
	{
		MinionIdentity minionIdentity = this.minion as MinionIdentity;
		if (minionIdentity == null)
		{
			return;
		}
		MinionResume component = minionIdentity.GetComponent<MinionResume>();
		if (skill != null)
		{
			HierarchyReferences component2 = base.GetComponent<HierarchyReferences>();
			component2.GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite((skill as SkillListable).skillHat);
			if (component != null)
			{
				string skillHat = (skill as SkillListable).skillHat;
				component.SetHats(component.CurrentHat, skillHat);
				if (component.OwnsHat(skillHat))
				{
					new PutOnHatChore(component, Db.Get().ChoreTypes.SwitchHat);
				}
			}
		}
		else
		{
			HierarchyReferences component3 = base.GetComponent<HierarchyReferences>();
			component3.GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite("hat_role_none");
			if (component != null)
			{
				component.SetHats(component.CurrentHat, null);
				component.ApplyTargetHat();
			}
		}
		if (this.minion == this.skillsScreen.CurrentlySelectedMinion)
		{
			this.skillsScreen.selectedHat.sprite = Assets.GetSprite(component.TargetHat);
		}
	}

	private void hatDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		if (entry.entryData != null)
		{
			SkillListable skillListable = entry.entryData as SkillListable;
			entry.image.sprite = Assets.GetSprite(skillListable.skillHat);
		}
	}

	private int hatDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		return 0;
	}

	[SerializeField]
	private SkillsScreen skillsScreen;

	[SerializeField]
	private CrewPortrait portrait;

	[SerializeField]
	private LocText masteryPoints;

	[SerializeField]
	private LocText morale;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image hat_background;

	[SerializeField]
	private Color selected_color;

	[SerializeField]
	private Color unselected_color;

	[SerializeField]
	private Color hover_color;

	[SerializeField]
	private DropDown hatDropDown;

	[SerializeField]
	private TextStyleSetting TooltipTextStyle_Header;

	[SerializeField]
	private TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;
}
