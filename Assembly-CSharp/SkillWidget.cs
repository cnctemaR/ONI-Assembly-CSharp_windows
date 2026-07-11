using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SkillWidget : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IEventSystemHandler
{
	public string skillID { get; private set; }

	public void Refresh(string skillID)
	{
		Skill skill = Db.Get().Skills.Get(skillID);
		if (skill == null)
		{
			global::Debug.LogWarning("DbSkills is missing skillId " + skillID);
			return;
		}
		this.Name.text = skill.Name;
		LocText name = this.Name;
		name.text = name.text + "\n(" + Db.Get().SkillGroups.Get(skill.skillGroup).Name + ")";
		this.skillID = skillID;
		this.tooltip.SetSimpleTooltip(this.SkillTooltip(skill));
		MinionIdentity minionIdentity = this.skillsScreen.CurrentlySelectedMinion as MinionIdentity;
		StoredMinionIdentity storedMinionIdentity = this.skillsScreen.CurrentlySelectedMinion as StoredMinionIdentity;
		MinionResume minionResume = null;
		if (minionIdentity != null)
		{
			minionResume = minionIdentity.GetComponent<MinionResume>();
			if (!(minionResume == null) && (minionResume.HasMasteredSkill(skillID) || minionResume.CanMasterSkill(skillID)))
			{
				this.TitleBarBG.color = ((!minionResume.HasMasteredSkill(skillID)) ? this.header_color_can_assign : this.header_color_has_skill);
				this.hatImage.material = this.defaultMaterial;
			}
			else
			{
				this.TitleBarBG.color = this.header_color_disabled;
				this.hatImage.material = this.desaturatedMaterial;
			}
		}
		else if (storedMinionIdentity != null)
		{
			if (storedMinionIdentity.HasMasteredSkill(skillID))
			{
				this.TitleBarBG.color = this.header_color_has_skill;
				this.hatImage.material = this.defaultMaterial;
			}
			else
			{
				this.TitleBarBG.color = this.header_color_disabled;
				this.hatImage.material = this.desaturatedMaterial;
			}
		}
		this.hatImage.sprite = Assets.GetSprite(skill.hat);
		bool flag = false;
		if (minionResume != null)
		{
			float num;
			minionResume.AptitudeBySkillGroup.TryGetValue(skill.skillGroup, out num);
			flag = num > 0f;
		}
		this.aptitudeBox.SetActive(flag);
		this.traitDisabledIcon.SetActive(minionResume != null && minionResume.CheckSkillTraitDisabled(skill.Id));
		string text = string.Empty;
		List<string> list = new List<string>();
		foreach (MinionIdentity minionIdentity2 in Components.LiveMinionIdentities.Items)
		{
			MinionResume component = minionIdentity2.GetComponent<MinionResume>();
			if (component != null && component.HasMasteredSkill(skillID))
			{
				list.Add(component.GetProperName());
			}
		}
		foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
		{
			foreach (MinionStorage.Info info in minionStorage.GetStoredMinionInfo())
			{
				if (info.serializedMinion != null)
				{
					StoredMinionIdentity storedMinionIdentity2 = info.serializedMinion.Get<StoredMinionIdentity>();
					if (storedMinionIdentity2 != null && storedMinionIdentity2.HasMasteredSkill(skillID))
					{
						list.Add(storedMinionIdentity2.GetProperName());
					}
				}
			}
		}
		this.masteryCount.gameObject.SetActive(list.Count > 0);
		foreach (string text2 in list)
		{
			text = text + "\n    • " + text2;
		}
		this.masteryCount.SetSimpleTooltip((list.Count <= 0) ? UI.ROLES_SCREEN.WIDGET.NO_MASTERS_TOOLTIP.text : string.Format(UI.ROLES_SCREEN.WIDGET.NUMBER_OF_MASTERS_TOOLTIP, text));
		this.masteryCount.GetComponentInChildren<LocText>().text = list.Count.ToString();
	}

	public void RefreshLines()
	{
		this.prerequisiteSkillWidgets.Clear();
		List<Vector2> list = new List<Vector2>();
		Skill skill = Db.Get().Skills.Get(this.skillID);
		foreach (string text in skill.priorSkills)
		{
			list.Add(this.skillsScreen.GetSkillWidgetLineTargetPosition(text));
			this.prerequisiteSkillWidgets.Add(this.skillsScreen.GetSkillWidget(text));
		}
		if (this.lines != null)
		{
			for (int i = this.lines.Length - 1; i >= 0; i--)
			{
				global::UnityEngine.Object.Destroy(this.lines[i].gameObject);
			}
		}
		this.linePoints.Clear();
		for (int j = 0; j < list.Count; j++)
		{
			float num = this.lines_left.GetPosition().x - list[j].x - 12f;
			float num2 = 0f;
			this.linePoints.Add(new Vector2(0f, num2));
			this.linePoints.Add(new Vector2(-num, num2));
			this.linePoints.Add(new Vector2(-num, num2));
			this.linePoints.Add(new Vector2(-num, -(this.lines_left.GetPosition().y - list[j].y)));
			this.linePoints.Add(new Vector2(-num, -(this.lines_left.GetPosition().y - list[j].y)));
			this.linePoints.Add(new Vector2(-(this.lines_left.GetPosition().x - list[j].x), -(this.lines_left.GetPosition().y - list[j].y)));
		}
		this.lines = new UILineRenderer[this.linePoints.Count / 2];
		int num3 = 0;
		for (int k = 0; k < this.linePoints.Count; k += 2)
		{
			GameObject gameObject = new GameObject("Line");
			gameObject.AddComponent<RectTransform>();
			gameObject.transform.SetParent(this.lines_left.transform);
			gameObject.transform.SetLocalPosition(Vector3.zero);
			gameObject.rectTransform().sizeDelta = Vector2.zero;
			this.lines[num3] = gameObject.AddComponent<UILineRenderer>();
			this.lines[num3].color = new Color(0.6509804f, 0.6509804f, 0.6509804f, 1f);
			this.lines[num3].Points = new Vector2[]
			{
				this.linePoints[k],
				this.linePoints[k + 1]
			};
			num3++;
		}
	}

	public void ToggleBorderHighlight(bool on)
	{
		this.borderHighlight.SetActive(on);
		if (this.lines != null)
		{
			foreach (UILineRenderer uilineRenderer in this.lines)
			{
				uilineRenderer.color = ((!on) ? this.line_color_default : this.line_color_active);
				uilineRenderer.LineThickness = (float)((!on) ? 2 : 4);
				uilineRenderer.SetAllDirty();
			}
		}
		for (int j = 0; j < this.prerequisiteSkillWidgets.Count; j++)
		{
			this.prerequisiteSkillWidgets[j].ToggleBorderHighlight(on);
		}
	}

	public string SkillTooltip(Skill skill)
	{
		string text = string.Empty;
		text += this.SkillPerksString(skill);
		return text + "\n" + this.DuplicantSkillString(skill);
	}

	public string SkillPerksString(Skill skill)
	{
		string text = string.Empty;
		foreach (SkillPerk skillPerk in skill.perks)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += "\n";
			}
			text = text + "• " + skillPerk.Name;
		}
		return text;
	}

	public string CriteriaString(Skill skill)
	{
		bool flag = false;
		string text = string.Empty;
		text = text + "<b>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.TITLE + "</b>\n";
		SkillGroup skillGroup = Db.Get().SkillGroups.Get(skill.skillGroup);
		if (skillGroup != null && skillGroup.relevantAttributes != null)
		{
			foreach (string text2 in skillGroup.relevantAttributes)
			{
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(text2);
				if (attribute != null)
				{
					text = text + "    • " + string.Format(UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.SKILLGROUP_ENABLED.DESCRIPTION, attribute.Name) + "\n";
					flag = true;
				}
			}
		}
		if (skill.priorSkills.Count > 0)
		{
			flag = true;
			for (int i = 0; i < skill.priorSkills.Count; i++)
			{
				text = text + "    • " + string.Format("{0}", Db.Get().Skills.Get(skill.priorSkills[i]).Name);
				text += "</color>";
				if (i != skill.priorSkills.Count - 1)
				{
					text += "\n";
				}
			}
		}
		if (!flag)
		{
			text = text + "    • " + string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.NONE, skill.Name);
		}
		return text;
	}

	public string DuplicantSkillString(Skill skill)
	{
		string text = string.Empty;
		MinionIdentity minionIdentity = this.skillsScreen.CurrentlySelectedMinion as MinionIdentity;
		if (minionIdentity != null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if (component == null)
			{
				return string.Empty;
			}
			LocString locString = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.CAN_MASTER;
			if (!component.HasMasteredSkill(skill.Id))
			{
				if (!component.CanMasterSkill(skill.Id))
				{
					text += "\n";
					locString = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.CANNOT_MASTER;
					text += string.Format(locString, minionIdentity.GetProperName(), skill.Name);
				}
			}
		}
		return text;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.ToggleBorderHighlight(true);
		this.skillsScreen.HoverSkill(this.skillID);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.ToggleBorderHighlight(false);
		this.skillsScreen.HoverSkill(null);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		MinionIdentity minionIdentity = this.skillsScreen.CurrentlySelectedMinion as MinionIdentity;
		if (minionIdentity != null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if (DebugHandler.InstantBuildMode && component.AvailableSkillpoints < 1)
			{
				component.ForceAddSkillPoint();
			}
			if (component != null && !component.HasMasteredSkill(this.skillID) && component.CanMasterSkill(this.skillID))
			{
				component.MasterSkill(this.skillID);
				this.skillsScreen.RefreshSkillWidgets();
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
	}

	[SerializeField]
	private LocText Name;

	[SerializeField]
	private LocText Description;

	[SerializeField]
	private Image TitleBarBG;

	[SerializeField]
	private SkillsScreen skillsScreen;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private RectTransform lines_left;

	[SerializeField]
	public RectTransform lines_right;

	[SerializeField]
	private Color header_color_has_skill;

	[SerializeField]
	private Color header_color_can_assign;

	[SerializeField]
	private Color header_color_disabled;

	[SerializeField]
	private Color line_color_default;

	[SerializeField]
	private Color line_color_active;

	[SerializeField]
	private Image hatImage;

	[SerializeField]
	private GameObject borderHighlight;

	[SerializeField]
	private ToolTip masteryCount;

	[SerializeField]
	private GameObject aptitudeBox;

	[SerializeField]
	private GameObject traitDisabledIcon;

	public TextStyleSetting TooltipTextStyle_Header;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private List<SkillWidget> prerequisiteSkillWidgets = new List<SkillWidget>();

	private UILineRenderer[] lines;

	private List<Vector2> linePoints = new List<Vector2>();

	public Material defaultMaterial;

	public Material desaturatedMaterial;
}
