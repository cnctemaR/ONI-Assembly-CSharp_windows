using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class RoleWidget : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
	public string roleID { get; private set; }

	public void Refresh(string roleID)
	{
		this.Name.text = Game.Instance.roleManager.GetRole(roleID).name;
		if (roleID != "NoRole")
		{
			LocText name = this.Name;
			name.text = name.text + " (" + Game.Instance.roleManager.RoleGroups[Game.Instance.roleManager.GetRole(roleID).roleGroup].Name + ")";
		}
		this.Description.text = Game.Instance.roleManager.GetRole(roleID).description;
		this.roleID = roleID;
		this.NoSlots.SetActive(Game.Instance.roleManager.NumberOfSlotsUnlocked(roleID) == 0);
		this.tooltip.SetSimpleTooltip(Game.Instance.roleManager.RoleTooltip(roleID));
		int count = Game.Instance.roleManager.GetRoleAssignees(roleID).Count;
		if (this.Slots.Count > count)
		{
			for (int i = this.Slots.Count - 1; i > count; i--)
			{
				this.rolesScreen.RecycleSlotWidget(this.Slots[i]);
				this.Slots.RemoveAt(i);
			}
		}
		else
		{
			for (int j = this.Slots.Count; j < count + 1; j++)
			{
				GameObject slotWidget = this.rolesScreen.GetSlotWidget(this.SlotContainer);
				this.Slots.Add(slotWidget);
			}
		}
		List<MinionResume> roleAssignees = Game.Instance.roleManager.GetRoleAssignees(roleID);
		bool flag = false;
		for (int k = 0; k < count; k++)
		{
			this.RefreshSlot(this.SlotContainer.transform.GetChild(k).gameObject, roleAssignees[k]);
			if (!this.SlotContainer.transform.GetChild(k).gameObject.activeSelf)
			{
				this.SlotContainer.transform.GetChild(k).gameObject.SetActive(true);
			}
		}
		this.RefreshSlot(this.SlotContainer.transform.GetChild(count).gameObject, null);
		this.SlotContainer.transform.GetChild(count).gameObject.SetActive(!flag);
		bool flag2 = false;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if (Game.Instance.roleManager.CanAssignToRole(roleID, component))
			{
				flag2 = true;
			}
		}
		if (flag2 || roleAssignees.Count > 0)
		{
			this.TitleBarBG.color = ((roleAssignees.Count <= 0) ? this.header_color_inactive : this.header_color_active);
		}
		else
		{
			this.TitleBarBG.color = this.header_color_disabled;
		}
		string text = string.Empty;
		List<MinionResume> list = new List<MinionResume>();
		foreach (MinionIdentity minionIdentity2 in Components.LiveMinionIdentities.Items)
		{
			MinionResume component2 = minionIdentity2.GetComponent<MinionResume>();
			if (component2.MasteryByRoleID[roleID])
			{
				list.Add(component2);
			}
		}
		this.masteryCount.gameObject.SetActive(list.Count > 0 && roleID != "NoRole");
		foreach (MinionResume minionResume in list)
		{
			text = text + "\n    • " + minionResume.GetProperName();
		}
		this.masteryCount.SetSimpleTooltip((list.Count <= 0) ? UI.ROLES_SCREEN.WIDGET.NO_MASTERS_TOOLTIP.text : string.Format(UI.ROLES_SCREEN.WIDGET.NUMBER_OF_MASTERS_TOOLTIP, text));
		this.masteryCount.GetComponentInChildren<LocText>().text = list.Count.ToString();
	}

	public void RefreshLines()
	{
		this.prerequisiteRoleWidgets.Clear();
		List<Vector2> list = new List<Vector2>();
		foreach (RoleAssignmentRequirement roleAssignmentRequirement in Game.Instance.roleManager.GetRole(this.roleID).requirements)
		{
			if (roleAssignmentRequirement is PreviousRoleAssignmentRequirement)
			{
				list.Add(this.rolesScreen.GetRoleWidgetLineTargetPosition((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID));
				this.prerequisiteRoleWidgets.Add(this.rolesScreen.GetRoleWidget((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID));
			}
		}
		if (this.lines != null)
		{
			for (int j = this.lines.Length - 1; j >= 0; j--)
			{
				global::UnityEngine.Object.Destroy(this.lines[j].gameObject);
			}
		}
		this.linePoints.Clear();
		for (int k = 0; k < list.Count; k++)
		{
			float num = this.lines_left.GetPosition().x - list[k].x - 12f;
			float num2 = 0f;
			this.linePoints.Add(new Vector2(0f, num2));
			this.linePoints.Add(new Vector2(-num, num2));
			this.linePoints.Add(new Vector2(-num, num2));
			this.linePoints.Add(new Vector2(-num, -(this.lines_left.GetPosition().y - list[k].y)));
			this.linePoints.Add(new Vector2(-num, -(this.lines_left.GetPosition().y - list[k].y)));
			this.linePoints.Add(new Vector2(-(this.lines_left.GetPosition().x - list[k].x), -(this.lines_left.GetPosition().y - list[k].y)));
		}
		this.lines = new UILineRenderer[this.linePoints.Count / 2];
		int num3 = 0;
		for (int l = 0; l < this.linePoints.Count; l += 2)
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
				this.linePoints[l],
				this.linePoints[l + 1]
			};
			num3++;
		}
	}

	private void RefreshSlot(GameObject slot, MinionResume occupier)
	{
		HierarchyReferences component = slot.GetComponent<HierarchyReferences>();
		component.GetReference<CrewPortrait>("Portrait").GetComponentInChildren<KBatchedAnimController>().enabled = occupier != null;
		if (occupier != null)
		{
			component.GetReference<CrewPortrait>("Portrait").SetIdentityObject(occupier.GetComponent<MinionIdentity>(), true);
			component.GetReference<LocText>("Label").gameObject.SetActive(true);
			component.GetReference<LayoutElement>("DropDownLayout").minWidth = ((!(this.roleID == "NoRole")) ? 100f : 64f);
			component.GetReference<LayoutElement>("DropDownLayout").GetComponentInChildren<LocText>().text = ((!(this.roleID == "NoRole")) ? UI.ROLES_SCREEN.SLOTS.UNASSIGNED : UI.ROLES_SCREEN.SLOTS.PICK_JOB);
			component.GetReference<LocText>("Label").text = occupier.GetProperName();
			if (occupier.CurrentRole != occupier.TargetRole)
			{
				LocText reference = component.GetReference<LocText>("Label");
				reference.text = reference.text + " " + UI.ROLES_SCREEN.SLOTS.ASSIGNMENT_PENDING;
			}
			component.GetReference("BG").GetComponent<KImage>().ColorState = KImage.ColorSelector.Active;
			string text = string.Empty;
			foreach (KeyValuePair<HashedString, float> keyValuePair in occupier.AptitudeByRoleGroup)
			{
				if (keyValuePair.Value != 0f)
				{
					text = text + "\n    • " + Game.Instance.roleManager.RoleGroups[keyValuePair.Key].Name;
				}
			}
			if (text != string.Empty)
			{
				text = text.Insert(0, "<b>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.APTITUDES + "</b>");
				text += "\n\n";
			}
			text = text + "<b>" + UI.DETAILTABS.STATS.NAME + "</b>";
			string text3;
			foreach (AttributeInstance attributeInstance in occupier.GetAttributes())
			{
				if (attributeInstance.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill)
				{
					string text2 = UIConstants.ColorPrefixWhite;
					if (attributeInstance.GetTotalValue() > 0f)
					{
						text2 = UIConstants.ColorPrefixGreen;
					}
					else if (attributeInstance.GetTotalValue() < 0f)
					{
						text2 = UIConstants.ColorPrefixRed;
					}
					text3 = text;
					text = string.Concat(new object[]
					{
						text3,
						"\n    • ",
						attributeInstance.Name,
						": ",
						text2,
						attributeInstance.GetTotalValue(),
						UIConstants.ColorSuffix
					});
				}
			}
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				"\n\n",
				UI.ROLES_SCREEN.HIGHEST_EXPECTATIONS_TIER,
				"\n    • ",
				RolesScreen.tierNames[occupier.GetComponent<MinionResume>().HighestTierRole()]
			});
			text = text.Insert(0, "<b>" + occupier.GetProperName() + "</b>\n\n");
			component.GetReference<ToolTip>("PortraitTooltip").SetSimpleTooltip(text);
			if (this.roleID != "NoRole")
			{
				component.GetReference("UnassignButton").gameObject.SetActive(true);
				component.GetReference("ProgressBar").gameObject.SetActive(true);
				component.GetReference("ProgressBar").gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").text = GameUtil.GetFormattedPercent(Mathf.Floor(100f * (occupier.ExperienceByRoleID[this.roleID] / Game.Instance.roleManager.GetRole(this.roleID).experienceRequired)), GameUtil.TimeSlice.None);
				component.GetReference("ProgressBar").gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Progress").fillAmount = ((!occupier.ExperienceByRoleID.ContainsKey(this.roleID)) ? 0f : (occupier.ExperienceByRoleID[this.roleID] / Game.Instance.roleManager.GetRole(this.roleID).experienceRequired));
				component.GetReference("ProgressBar").GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.ROLES_SCREEN.ROLE_PROGRESS, Mathf.RoundToInt(occupier.ExperienceByRoleID[this.roleID]), Game.Instance.roleManager.GetRole(this.roleID).experienceRequired));
				component.GetComponent<ToolTip>().enabled = false;
				component.GetReference<DropDown>("DropDown").gameObject.SetActive(false);
			}
			else
			{
				component.GetComponent<ToolTip>().enabled = false;
				List<IListableOption> list = new List<IListableOption>();
				foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
				{
					list.Add(minionIdentity);
				}
				DropDown reference2 = component.GetReference<DropDown>("DropDown");
				reference2.gameObject.SetActive(true);
				reference2.Initialize(Game.Instance.roleManager.RolesConfigs.Cast<IListableOption>(), new Action<IListableOption, object>(this.OnMinionDropEntryClick), new Func<IListableOption, IListableOption, object, int>(this.minionDropDownSort), new Action<DropDownEntry, object>(this.minionDropEntryRefreshAction), false, occupier);
			}
		}
		else
		{
			slot.GetComponent<MultiToggle>().onClick = delegate
			{
			};
			List<IListableOption> list2 = new List<IListableOption>();
			foreach (MinionIdentity minionIdentity2 in Components.LiveMinionIdentities.Items)
			{
				list2.Add(minionIdentity2);
			}
			Action<IListableOption, object> action = delegate(IListableOption minion, object data)
			{
				if (minion == null)
				{
					return;
				}
				Game.Instance.roleManager.AssignToRole(this.roleID, (minion as MinionIdentity).GetComponent<MinionResume>(), false, false);
				this.rolesScreen.RefreshRoleWidgets();
				this.rolesScreen.RefreshSideBar();
			};
			DropDown reference3 = component.GetReference<DropDown>("DropDown");
			reference3.gameObject.SetActive(true);
			reference3.Initialize(list2, action, new Func<IListableOption, IListableOption, object, int>(this.roleSlotDropDownSort), new Action<DropDownEntry, object>(this.roleRefreshAction), false, Game.Instance.roleManager.GetRole(this.roleID));
			component.GetReference<LocText>("Label").gameObject.SetActive(false);
			component.GetReference<LayoutElement>("DropDownLayout").minWidth = 156f;
			component.GetReference<LayoutElement>("DropDownLayout").GetComponentInChildren<LocText>().text = ((!(this.roleID == "NoRole")) ? UI.ROLES_SCREEN.SLOTS.UNASSIGNED : UI.ROLES_SCREEN.SLOTS.PICK_DUPLICANT);
			component.GetReference<CrewPortrait>("Portrait").SetIdentityObject(null, true);
			component.GetReference("UnassignButton").gameObject.SetActive(false);
			component.GetReference("ProgressBar").gameObject.SetActive(false);
			component.GetReference("BG").GetComponent<KImage>().ColorState = KImage.ColorSelector.Active;
		}
		component.GetReference<KButton>("UnassignButton").ClearOnClick();
		component.GetReference<KButton>("UnassignButton").onClick += delegate
		{
			MinionResume minionResume = Game.Instance.roleManager.GetRoleAssignees(this.roleID)[slot.transform.GetSiblingIndex()];
			Game.Instance.roleManager.Unassign(minionResume, false);
			minionResume.SetTargetRole("NoRole");
			this.Refresh(this.roleID);
			this.rolesScreen.RefreshSideBar();
			this.rolesScreen.RefreshWidgetPositions();
		};
	}

	private void OnMinionDropEntryClick(IListableOption role, object data)
	{
		if (role != null)
		{
			Game.Instance.roleManager.AssignToRole((role as RoleConfig).id, data as MinionResume, false, false);
			this.rolesScreen.RefreshRoleWidgets();
			this.rolesScreen.RefreshSideBar();
		}
	}

	private void minionDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		Image reference = entry.GetComponent<HierarchyReferences>().GetReference<Image>("SkillBox");
		Image reference2 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("AptitudeBox");
		RoleConfig roleConfig = entry.entryData as RoleConfig;
		MinionResume minionResume = targetData as MinionResume;
		if (roleConfig == null)
		{
			reference.transform.parent.gameObject.SetActive(false);
			reference2.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			entry.tooltip.SetSimpleTooltip(Game.Instance.roleManager.RoleCriteriaString(roleConfig.id, minionResume));
			entry.button.isInteractable = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, minionResume);
			reference.transform.parent.gameObject.SetActive(true);
			reference2.transform.parent.gameObject.SetActive(true);
			float num = 0f;
			for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
			{
				num += minionResume.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
			}
			float num2 = minionResume.AptitudeByRoleGroup[roleConfig.roleGroup];
			reference.transform.parent.GetComponentInChildren<LocText>(true).text = num.ToString();
			if (roleConfig.relevantAttributes.Length > 0)
			{
				reference.SetAlpha(GameUtil.AttributeSkillToAlpha(num));
			}
			else
			{
				reference.SetAlpha(0f);
			}
			reference2.gameObject.SetActive(num2 > 0f);
		}
	}

	private int minionDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		RoleConfig roleConfig = a as RoleConfig;
		RoleConfig roleConfig2 = b as RoleConfig;
		MinionResume minionResume = targetData as MinionResume;
		bool flag = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, minionResume);
		bool flag2 = Game.Instance.roleManager.CanAssignToRole(roleConfig2.id, minionResume);
		if (flag && !flag2)
		{
			return 1;
		}
		if (flag2 && !flag)
		{
			return -1;
		}
		float num = minionResume.AptitudeByRoleGroup[roleConfig.roleGroup];
		float num2 = minionResume.AptitudeByRoleGroup[roleConfig2.roleGroup];
		if (num != num2)
		{
			return (num <= num2) ? (-1) : 1;
		}
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
		{
			num3 += minionResume.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
		}
		for (int j = 0; j < roleConfig2.relevantAttributes.Length; j++)
		{
			num4 += minionResume.GetAttributes().Get(roleConfig2.relevantAttributes[j]).GetTotalDisplayValue();
		}
		if (num3 > num4)
		{
			return 1;
		}
		if (num4 > num3)
		{
			return -1;
		}
		return 0;
	}

	private int roleSlotDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		MinionResume component = (a as MinionIdentity).GetComponent<MinionResume>();
		MinionResume component2 = (b as MinionIdentity).GetComponent<MinionResume>();
		RoleConfig roleConfig = targetData as RoleConfig;
		if (component.CurrentRole != component2.CurrentRole)
		{
			if (component.CurrentRole == roleConfig.id)
			{
				return -1;
			}
			if (component2.CurrentRole == roleConfig.id)
			{
				return 1;
			}
		}
		bool flag = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, component);
		bool flag2 = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, component);
		if (flag && !flag2)
		{
			return -1;
		}
		if (flag2 && !flag)
		{
			return 1;
		}
		float num = component.AptitudeByRoleGroup[roleConfig.roleGroup];
		float num2 = component2.AptitudeByRoleGroup[roleConfig.roleGroup];
		if (num != num2)
		{
			return (num <= num2) ? (-1) : 1;
		}
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
		{
			num3 += component.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
			num4 += component2.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
		}
		if (num3 > num4)
		{
			return 1;
		}
		if (num4 > num3)
		{
			return -1;
		}
		if (component.CurrentRole == "NoRole" == (component2.CurrentRole == "NoRole"))
		{
			return 0;
		}
		if (component.CurrentRole == "NoRole")
		{
			return 1;
		}
		return -1;
	}

	private void roleRefreshAction(DropDownEntry entry, object targetData)
	{
		RoleConfig roleConfig = targetData as RoleConfig;
		MinionIdentity minionIdentity = null;
		if (entry != null)
		{
			minionIdentity = entry.entryData as MinionIdentity;
		}
		MinionResume minionResume = null;
		if (minionIdentity != null)
		{
			entry.button.isInteractable = Game.Instance.roleManager.CanAssignToRole(this.roleID, minionIdentity.GetComponent<MinionResume>());
			minionResume = minionIdentity.GetComponent<MinionResume>();
			entry.tooltip.SetSimpleTooltip(Game.Instance.roleManager.RoleCriteriaString(this.roleID, minionResume));
			if (minionResume.CurrentRole == "NoRole")
			{
				entry.label.text = minionIdentity.GetProperName();
			}
			else
			{
				entry.label.text = string.Format(UI.ROLES_SCREEN.DROPDOWN.NAME_AND_ROLE, minionIdentity.GetProperName(), Game.Instance.roleManager.GetRole(minionResume.CurrentRole).name);
			}
			if (entry.button.isInteractable)
			{
				string currentRole = (entry.entryData as MinionIdentity).GetComponent<MinionResume>().CurrentRole;
			}
		}
		Image reference = entry.GetComponent<HierarchyReferences>().GetReference<Image>("AptitudeBox");
		if (minionIdentity == null)
		{
			reference.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			reference.transform.parent.gameObject.SetActive(true);
			float num = minionResume.AptitudeByRoleGroup[roleConfig.roleGroup];
			if (roleConfig.relevantAttributes.Length > 0)
			{
				reference.gameObject.SetActive(num > 0f);
			}
			else
			{
				reference.gameObject.SetActive(false);
			}
		}
		Image reference2 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("SkillBox");
		if (minionIdentity == null)
		{
			reference2.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			reference2.transform.parent.gameObject.SetActive(true);
			float num2 = 0f;
			for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
			{
				num2 += minionIdentity.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
			}
			reference2.transform.parent.GetComponentInChildren<LocText>(true).text = num2.ToString();
			if (roleConfig.relevantAttributes.Length > 0)
			{
				reference2.SetAlpha(GameUtil.AttributeSkillToAlpha(num2));
			}
			else
			{
				reference2.SetAlpha(0f);
			}
		}
	}

	public void ToggleBorderHighlight(bool on)
	{
		this.borderHighlight.SetActive(on);
		if (this.lines != null)
		{
			foreach (UILineRenderer uilineRenderer in this.lines)
			{
				uilineRenderer.LineThickness = (float)((!on) ? 2 : 4);
				uilineRenderer.SetAllDirty();
			}
		}
		for (int j = 0; j < this.prerequisiteRoleWidgets.Count; j++)
		{
			this.prerequisiteRoleWidgets[j].ToggleBorderHighlight(on);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.ToggleBorderHighlight(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.ToggleBorderHighlight(false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
	}

	[SerializeField]
	private LocText Name;

	[SerializeField]
	private LocText Description;

	[SerializeField]
	private Image TitleBarBG;

	[SerializeField]
	private GameObject NoSlots;

	[SerializeField]
	private GameObject SlotContainer;

	[SerializeField]
	private RolesScreen rolesScreen;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private ColorStyleSetting[] dropDownColorStyles;

	[SerializeField]
	private RectTransform lines_left;

	[SerializeField]
	public RectTransform lines_right;

	[SerializeField]
	private Color header_color_active;

	[SerializeField]
	private Color header_color_inactive;

	[SerializeField]
	private Color header_color_disabled;

	[SerializeField]
	private Color line_color_default;

	[SerializeField]
	private Color line_color_active;

	[SerializeField]
	private GameObject borderHighlight;

	[SerializeField]
	private ToolTip masteryCount;

	private List<GameObject> Slots = new List<GameObject>();

	private List<RoleWidget> prerequisiteRoleWidgets = new List<RoleWidget>();

	private UILineRenderer[] lines;

	private List<Vector2> linePoints = new List<Vector2>();
}
