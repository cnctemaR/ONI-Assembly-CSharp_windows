using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JobsTableScreen : TableScreen
{
	public override float GetSortKey()
	{
		return 102f;
	}

	protected override void OnActivate()
	{
		JobsTableScreen.Instance = this;
		this.title = UI.JOBSSCREEN.TITLE;
		base.OnActivate();
		this.resetSettingsButton.onClick += this.OnResetSettingsClicked;
		this.priorityInfo = new List<JobsTableScreen.PriorityInfo>
		{
			new JobsTableScreen.PriorityInfo(0, Assets.GetSprite("icon_priority_disabled"), UI.JOBSSCREEN.PRIORITY.DISABLED),
			new JobsTableScreen.PriorityInfo(1, Assets.GetSprite("icon_priority_down_2"), UI.JOBSSCREEN.PRIORITY.VERYLOW),
			new JobsTableScreen.PriorityInfo(2, Assets.GetSprite("icon_priority_down"), UI.JOBSSCREEN.PRIORITY.LOW),
			new JobsTableScreen.PriorityInfo(3, Assets.GetSprite("icon_priority_flat"), UI.JOBSSCREEN.PRIORITY.STANDARD),
			new JobsTableScreen.PriorityInfo(4, Assets.GetSprite("icon_priority_up"), UI.JOBSSCREEN.PRIORITY.HIGH),
			new JobsTableScreen.PriorityInfo(5, Assets.GetSprite("icon_priority_up_2"), UI.JOBSSCREEN.PRIORITY.VERYHIGH),
			new JobsTableScreen.PriorityInfo(5, Assets.GetSprite("icon_priority_automatic"), UI.JOBSSCREEN.PRIORITY.VERYHIGH)
		};
		this.prioritySprites = new List<Sprite>();
		foreach (JobsTableScreen.PriorityInfo priorityInfo in this.priorityInfo)
		{
			this.prioritySprites.Add(priorityInfo.sprite);
		}
		base.AddPortraitColumn("Portrait", new Action<MinionIdentity, GameObject>(base.on_load_portrait), null, true);
		base.AddButtonLabelColumn("Names", new Action<MinionIdentity, GameObject>(this.ConfigureNameLabel), new Func<MinionIdentity, GameObject, string>(base.get_value_name_label), delegate(GameObject widget_go)
		{
			base.GetWidgetRow(widget_go).SelectMinion();
		}, delegate(GameObject widget_go)
		{
			base.GetWidgetRow(widget_go).SelectAndFocusMinion();
		}, new Comparison<MinionIdentity>(base.compare_rows_alphabetical), null, new Action<MinionIdentity, GameObject, ToolTip>(base.on_tooltip_sort_alphabetically), false);
		List<ChoreGroup> list = new List<ChoreGroup>(Db.Get().ChoreGroups);
			from @group in list
			orderby @group.DefaultPersonalPriority descending, @group.Name
			select @group;
		foreach (ChoreGroup choreGroup in list)
		{
			PrioritizationGroupTableColumn prioritizationGroupTableColumn = new PrioritizationGroupTableColumn(choreGroup, new Action<MinionIdentity, GameObject>(this.LoadValue), new Action<object, int>(this.ChangePersonalPriority), new Func<object, string>(this.HoverPersonalPriority), new Action<object, int>(this.ChangeColumnPriority), new Func<object, string>(this.HoverChangeColumnPriorityButton), new Action<object>(this.OnSortClicked), new Func<object, string>(this.OnSortHovered));
			base.RegisterColumn(choreGroup.Id, prioritizationGroupTableColumn);
		}
		PrioritizeRowTableColumn prioritizeRowTableColumn = new PrioritizeRowTableColumn(null, new Action<object, int>(this.ChangeRowPriority), new Func<object, int, string>(this.HoverChangeRowPriorityButton));
		base.RegisterColumn("prioritize_row", prioritizeRowTableColumn);
		this.settingsButton.onClick += this.OnSettingsButtonClicked;
		this.resetSettingsButton.onClick += this.OnResetSettingsClicked;
		this.toggleAdvancedModeButton.onClick += this.OnAdvancedModeToggleClicked;
		this.toggleAdvancedModeButton.fgImage.gameObject.SetActive(Game.Instance.advancedPersonalPriorities);
		this.RefreshEffectListeners();
	}

	private string HoverPersonalPriority(object widget_go_obj)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = base.GetWidgetColumn(gameObject) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		string text = null;
		TableRow widgetRow = base.GetWidgetRow(gameObject);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			if (rowType != TableRow.RowType.Default)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					text = UI.JOBSSCREEN.ITEM_TOOLTIP.ToString();
					text = text.Replace("{Name}", widgetRow.name);
				}
			}
			else
			{
				text = UI.JOBSSCREEN.NEW_MINION_ITEM_TOOLTIP.ToString();
			}
			ToolTip componentInChildren = gameObject.GetComponentInChildren<ToolTip>();
			IPersonalPriorityManager priorityManager = this.GetPriorityManager(widgetRow);
			bool flag;
			int personalPriority = priorityManager.GetPersonalPriority(choreGroup, out flag);
			string text2 = this.GetPriorityStr(personalPriority);
			MinionIdentity minionIdentity = widgetRow.GetMinionIdentity();
			if (minionIdentity != null && flag)
			{
				text = UI.JOBSSCREEN.ITEM_AUTO_ASSIGNED_TOOLTIP.ToString();
				text = text.Replace("{Role}", minionIdentity.GetComponent<MinionResume>().GetCurrentRoleString());
				text = text.Replace("{Name}", minionIdentity.GetProperName());
			}
			if (priorityManager.IsChoreGroupDisabled(choreGroup))
			{
				Trait trait = null;
				Traits component = minionIdentity.GetComponent<Traits>();
				foreach (Trait trait2 in component)
				{
					if (trait2.disabledChoreGroups != null)
					{
						foreach (ChoreGroup choreGroup2 in trait2.disabledChoreGroups)
						{
							if (choreGroup2.IdHash == choreGroup.IdHash)
							{
								trait = trait2;
								break;
							}
						}
						if (trait != null)
						{
							break;
						}
					}
				}
				text = UI.JOBSSCREEN.TRAIT_DISABLED.ToString();
				text = text.Replace("{Name}", minionIdentity.GetProperName());
				text = text.Replace("{Job}", choreGroup.Name);
				text = text.Replace("{Trait}", trait.Name);
				componentInChildren.ClearMultiStringTooltip();
				componentInChildren.AddMultiStringTooltip(text, null);
			}
			else
			{
				text = text.Replace("{Job}", choreGroup.Name);
				text = text.Replace("{Priority}", text2);
				componentInChildren.ClearMultiStringTooltip();
				componentInChildren.AddMultiStringTooltip(text, null);
				if (minionIdentity != null)
				{
					text = "\n" + UI.JOBSSCREEN.MINION_SKILL_TOOLTIP.ToString();
					text = text.Replace("{Name}", minionIdentity.GetProperName());
					text = text.Replace("{Attribute}", choreGroup.attribute.Name);
					AttributeInstance attributeInstance = minionIdentity.GetAttributes().Get(choreGroup.attribute);
					float totalValue = attributeInstance.GetTotalValue();
					TextStyleSetting textStyleSetting = this.TooltipTextStyle_Ability;
					if (totalValue > 0f)
					{
						textStyleSetting = this.TooltipTextStyle_AbilityPositiveModifier;
					}
					else if (totalValue < 0f)
					{
						textStyleSetting = this.TooltipTextStyle_AbilityNegativeModifier;
					}
					text += GameUtil.ColourizeString(textStyleSetting.textColor, totalValue.ToString());
					componentInChildren.AddMultiStringTooltip(text, null);
				}
				componentInChildren.AddMultiStringTooltip(UI.HORIZONTAL_RULE + "\n" + this.GetUsageString(), null);
			}
			return string.Empty;
		}
		string text3 = UI.JOBSSCREEN.HEADER_TOOLTIP.ToString();
		text3 = text3.Replace("{Job}", choreGroup.Name);
		string text4 = UI.JOBSSCREEN.HEADER_DETAILS_TOOLTIP.ToString();
		text4 = text4.Replace("{Description}", choreGroup.description);
		HashSet<string> hashSet = new HashSet<string>();
		foreach (ChoreType choreType in choreGroup.choreTypes)
		{
			hashSet.Add(choreType.Name);
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		foreach (string text5 in hashSet)
		{
			stringBuilder.Append(text5);
			if (num < hashSet.Count - 1)
			{
				stringBuilder.Append(", ");
			}
			num++;
		}
		text4 = text4.Replace("{ChoreList}", stringBuilder.ToString());
		text3 = text3.Replace("{Details}", text4);
		return text3;
	}

	private string HoverChangeColumnPriorityButton(object widget_go_obj)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = base.GetWidgetColumn(gameObject) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		LocString header_CHANGE_TOOLTIP = UI.JOBSSCREEN.HEADER_CHANGE_TOOLTIP;
		string text = header_CHANGE_TOOLTIP.ToString();
		return text.Replace("{Job}", choreGroup.Name);
	}

	private string GetUsageString()
	{
		return UI.JOBSSCREEN.INCREASE_PRIORITY_TUTORIAL.ToString().Replace("{Key}", GameUtil.GetHotkeyString(global::Action.MouseLeft)) + "\n" + UI.JOBSSCREEN.DECREASE_PRIORITY_TUTORIAL.ToString().Replace("{Key}", GameUtil.GetHotkeyString(global::Action.MouseRight));
	}

	private string HoverChangeRowPriorityButton(object widget_go_obj, int delta)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		LocString locString = null;
		LocString locString2 = null;
		string text = null;
		TableRow widgetRow = base.GetWidgetRow(gameObject);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			if (rowType != TableRow.RowType.Default)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					locString = UI.JOBSSCREEN.INCREASE_ROW_PRIORITY_MINION_TOOLTIP;
					locString2 = UI.JOBSSCREEN.DECREASE_ROW_PRIORITY_MINION_TOOLTIP;
					text = widgetRow.GetMinionIdentity().GetProperName();
				}
			}
			else
			{
				locString = UI.JOBSSCREEN.INCREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP;
				locString2 = UI.JOBSSCREEN.DECREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP;
			}
			LocString locString3 = ((delta <= 0) ? locString2 : locString);
			string text2 = locString3.ToString();
			if (text != null)
			{
				text2 = text2.Replace("{Name}", text);
			}
			return text2;
		}
		return null;
	}

	private void OnSortClicked(object widget_go_obj)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = base.GetWidgetColumn(gameObject) as PrioritizationGroupTableColumn;
		ChoreGroup chore_group = prioritizationGroupTableColumn.userData as ChoreGroup;
		if (this.active_sort_column == prioritizationGroupTableColumn)
		{
			this.sort_is_reversed = !this.sort_is_reversed;
		}
		this.active_sort_column = prioritizationGroupTableColumn;
		this.active_sort_method = delegate(MinionIdentity m1, MinionIdentity m2)
		{
			ChoreConsumer component = m1.GetComponent<ChoreConsumer>();
			ChoreConsumer component2 = m2.GetComponent<ChoreConsumer>();
			bool flag;
			int personalPriority = component.GetPersonalPriority(chore_group, out flag);
			int personalPriority2 = component2.GetPersonalPriority(chore_group, out flag);
			return personalPriority2 - personalPriority;
		};
		base.SortRows();
	}

	private string OnSortHovered(object widget_go_obj)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = base.GetWidgetColumn(gameObject) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		return UI.JOBSSCREEN.SORT_TOOLTIP.ToString().Replace("{Job}", choreGroup.Name);
	}

	private IPersonalPriorityManager GetPriorityManager(TableRow row)
	{
		IPersonalPriorityManager personalPriorityManager = null;
		TableRow.RowType rowType = row.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType == TableRow.RowType.Minion)
			{
				personalPriorityManager = row.GetMinionIdentity().GetComponent<ChoreConsumer>();
			}
		}
		else
		{
			personalPriorityManager = Immigration.Instance;
		}
		return personalPriorityManager;
	}

	private LocString GetPriorityStr(int priority)
	{
		priority = Mathf.Clamp(priority, 0, 5);
		LocString locString = null;
		foreach (JobsTableScreen.PriorityInfo priorityInfo in this.priorityInfo)
		{
			if (priorityInfo.priority == priority)
			{
				locString = priorityInfo.name;
			}
		}
		return locString;
	}

	private void LoadValue(MinionIdentity minion, GameObject widget_go)
	{
		if (widget_go == null)
		{
			return;
		}
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = base.GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			if (rowType == TableRow.RowType.Default || rowType == TableRow.RowType.Minion)
			{
				IPersonalPriorityManager priorityManager = this.GetPriorityManager(widgetRow);
				bool flag = priorityManager.IsChoreGroupDisabled(choreGroup);
				HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
				KImage kimage = component.GetReference("FG") as KImage;
				kimage.raycastTarget = flag;
				ToolTip toolTip = component.GetReference("FGToolTip") as ToolTip;
				toolTip.enabled = flag;
			}
		}
		else
		{
			this.InitializeHeader(choreGroup, widget_go);
		}
		IPersonalPriorityManager priorityManager2 = this.GetPriorityManager(widgetRow);
		if (priorityManager2 != null)
		{
			this.UpdateWidget(widget_go, choreGroup, priorityManager2);
		}
	}

	private JobsTableScreen.PriorityInfo GetPriorityInfo(int priority)
	{
		JobsTableScreen.PriorityInfo priorityInfo = default(JobsTableScreen.PriorityInfo);
		for (int i = 0; i < this.priorityInfo.Count; i++)
		{
			if (this.priorityInfo[i].priority == priority)
			{
				priorityInfo = this.priorityInfo[i];
				break;
			}
		}
		return priorityInfo;
	}

	private void ChangePersonalPriority(object widget_go_obj, int delta)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		if (widget_go_obj == null)
		{
			return;
		}
		TableRow widgetRow = base.GetWidgetRow(gameObject);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
		}
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = base.GetWidgetColumn(gameObject) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		IPersonalPriorityManager priorityManager = this.GetPriorityManager(widgetRow);
		this.ChangePersonalPriority(priorityManager, choreGroup, delta, true);
		this.UpdateWidget(gameObject, choreGroup, priorityManager);
	}

	private void ChangeColumnPriority(object widget_go_obj, int new_priority)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		if (widget_go_obj == null)
		{
			return;
		}
		TableRow widgetRow = base.GetWidgetRow(gameObject);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
		}
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = base.GetWidgetColumn(gameObject) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		foreach (TableRow tableRow in this.rows)
		{
			IPersonalPriorityManager priorityManager = this.GetPriorityManager(tableRow);
			if (priorityManager != null)
			{
				priorityManager.SetPersonalPriority(choreGroup, new_priority, false);
				GameObject widget = tableRow.GetWidget(prioritizationGroupTableColumn);
				this.UpdateWidget(widget, choreGroup, priorityManager);
			}
		}
	}

	private void ChangeRowPriority(object widget_go_obj, int delta)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		if (widget_go_obj == null)
		{
			return;
		}
		TableRow widgetRow = base.GetWidgetRow(gameObject);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			IPersonalPriorityManager priorityManager = this.GetPriorityManager(widgetRow);
			foreach (TableColumn tableColumn in this.columns.Values)
			{
				PrioritizationGroupTableColumn prioritizationGroupTableColumn = tableColumn as PrioritizationGroupTableColumn;
				if (prioritizationGroupTableColumn != null)
				{
					ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
					GameObject widget = widgetRow.GetWidget(prioritizationGroupTableColumn);
					this.ChangePersonalPriority(priorityManager, choreGroup, delta, false);
					this.UpdateWidget(widget, choreGroup, priorityManager);
				}
			}
			return;
		}
	}

	private void ChangePersonalPriority(IPersonalPriorityManager priority_mgr, ChoreGroup chore_group, int delta, bool wrap_around)
	{
		if (priority_mgr.IsChoreGroupDisabled(chore_group))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
			return;
		}
		bool flag;
		int num = priority_mgr.GetPersonalPriority(chore_group, out flag);
		bool flag2 = false;
		if (num == 5)
		{
			if (delta > 0)
			{
				if (priority_mgr.CanRoleManageChoreGroup(chore_group))
				{
					flag2 = true;
				}
			}
			else if (!flag)
			{
				num += delta;
			}
		}
		else
		{
			num += delta;
		}
		num = Mathf.Clamp(num, 0, 5);
		if (wrap_around)
		{
			num %= 6;
			if (num < 0)
			{
				num += 6;
			}
		}
		priority_mgr.SetPersonalPriority(chore_group, num, flag2);
		if (delta > 0)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
	}

	private void UpdateWidget(GameObject widget_go, ChoreGroup chore_group, IPersonalPriorityManager priority_mgr)
	{
		int num = 0;
		bool flag = false;
		int num2 = 0;
		bool flag2 = priority_mgr.IsChoreGroupDisabled(chore_group);
		if (!flag2)
		{
			num2 = priority_mgr.GetPersonalPriority(chore_group, out flag);
		}
		num2 = Mathf.Clamp(num2, 0, 5);
		if (!flag)
		{
			for (int i = 0; i < this.priorityInfo.Count - 1; i++)
			{
				if (this.priorityInfo[i].priority == num2)
				{
					num = i;
					break;
				}
			}
		}
		else
		{
			num = this.priorityInfo.Count - 1;
		}
		OptionSelector component = widget_go.GetComponent<OptionSelector>();
		int associatedSkillLevel = priority_mgr.GetAssociatedSkillLevel(chore_group);
		Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 128);
		if (associatedSkillLevel > 0)
		{
			float num3 = (float)(associatedSkillLevel - this.skillLevelLow);
			num3 /= (float)(this.skillLevelHigh - this.skillLevelLow);
			color = Color32.Lerp(this.skillOutlineColourLow, this.skillOutlineColourHigh, num3);
		}
		component.ConfigureItem(flag2, new OptionSelector.DisplayOptionInfo
		{
			bgOptions = null,
			fgOptions = this.prioritySprites,
			bgIndex = 0,
			fgIndex = num,
			fillColour = color
		});
		ToolTip componentInChildren = widget_go.transform.GetComponentInChildren<ToolTip>();
		if (componentInChildren != null)
		{
			componentInChildren.toolTip = this.HoverPersonalPriority(widget_go);
			componentInChildren.forceRefresh = true;
		}
	}

	public void ToggleColumnSortWidgets(bool show)
	{
		foreach (KeyValuePair<string, TableColumn> keyValuePair in this.columns)
		{
			if (keyValuePair.Value.column_sort_toggle != null)
			{
				keyValuePair.Value.column_sort_toggle.gameObject.SetActive(show);
			}
		}
	}

	public void Refresh(MinionResume minion_resume)
	{
		if (this == null)
		{
			return;
		}
		foreach (TableRow tableRow in this.rows)
		{
			MinionIdentity minionIdentity = tableRow.GetMinionIdentity();
			if (!(minionIdentity == null))
			{
				if (!(minionIdentity.gameObject != minion_resume.gameObject))
				{
					foreach (TableColumn tableColumn in this.columns.Values)
					{
						PrioritizationGroupTableColumn prioritizationGroupTableColumn = tableColumn as PrioritizationGroupTableColumn;
						if (prioritizationGroupTableColumn != null)
						{
							GameObject widget = tableRow.GetWidget(prioritizationGroupTableColumn);
							this.UpdateWidget(widget, prioritizationGroupTableColumn.userData as ChoreGroup, minionIdentity.GetComponent<ChoreConsumer>());
						}
					}
				}
			}
		}
	}

	protected override void RefreshRows()
	{
		base.RefreshRows();
		this.RefreshEffectListeners();
		if (this.dynamicRowSpacing)
		{
			this.SizeRows();
		}
	}

	private void SizeRows()
	{
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < this.header_row.transform.childCount; i++)
		{
			Transform child = this.header_row.transform.GetChild(i);
			LayoutElement component = child.GetComponent<LayoutElement>();
			if (component != null && !component.ignoreLayout)
			{
				num2++;
				num += component.minWidth;
			}
			else
			{
				HorizontalOrVerticalLayoutGroup component2 = child.GetComponent<HorizontalOrVerticalLayoutGroup>();
				if (component2 != null)
				{
					float x = component2.rectTransform().sizeDelta.x;
					num += x;
					num2++;
				}
			}
		}
		float width = base.gameObject.rectTransform().rect.width;
		float num3 = 0f;
		HorizontalLayoutGroup component3 = this.header_row.GetComponent<HorizontalLayoutGroup>();
		component3.spacing = num3;
		component3.childAlignment = TextAnchor.UpperLeft;
		foreach (TableRow tableRow in this.rows)
		{
			tableRow.transform.GetComponentInChildren<HorizontalLayoutGroup>().spacing = num3;
		}
	}

	private void RefreshEffectListeners()
	{
		for (int i = 0; i < this.EffectListeners.Count; i++)
		{
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.level_up);
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.effect_added);
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.effect_removed);
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.disease_added);
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.effect_added);
		}
		this.EffectListeners.Clear();
		for (int j = 0; j < Components.LiveMinionIdentities.Count; j++)
		{
			JobsTableScreen.SkillEventHandlerID skillEventHandlerID = default(JobsTableScreen.SkillEventHandlerID);
			MinionIdentity id2 = Components.LiveMinionIdentities[j];
			skillEventHandlerID.level_up = Components.LiveMinionIdentities[j].gameObject.Subscribe(-110704193, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id2);
			});
			skillEventHandlerID.effect_added = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1901442097, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id2);
			});
			skillEventHandlerID.effect_removed = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1157678353, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id2);
			});
			skillEventHandlerID.disease_added = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1089020, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id2);
			});
			skillEventHandlerID.disease_cured = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1516186173, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id2);
			});
		}
		for (int k = 0; k < Components.LiveMinionIdentities.Count; k++)
		{
			MinionIdentity id = Components.LiveMinionIdentities[k];
			Components.LiveMinionIdentities[k].gameObject.Subscribe(540773776, delegate(object new_role)
			{
				this.MarkSingleMinionRowDirty(id);
			});
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (this.dirty_single_minion_rows.Count != 0)
		{
			foreach (MinionIdentity minionIdentity in this.dirty_single_minion_rows)
			{
				if (!(minionIdentity == null))
				{
					this.RefreshSingleMinionRow(minionIdentity);
				}
			}
			this.dirty_single_minion_rows.Clear();
		}
	}

	protected void MarkSingleMinionRowDirty(MinionIdentity id)
	{
		this.dirty_single_minion_rows.Add(id);
	}

	private void RefreshSingleMinionRow(MinionIdentity id)
	{
		foreach (KeyValuePair<string, TableColumn> keyValuePair in this.columns)
		{
			if (keyValuePair.Value != null)
			{
				if (keyValuePair.Value.on_load_action != null)
				{
					foreach (KeyValuePair<TableRow, GameObject> keyValuePair2 in keyValuePair.Value.widgets_by_row)
					{
						if (!(keyValuePair2.Value == null))
						{
							if (!(keyValuePair2.Key.GetMinionIdentity() != id))
							{
								keyValuePair.Value.on_load_action(id, keyValuePair2.Value);
							}
						}
					}
					keyValuePair.Value.on_load_action(null, this.rows[0].GetWidget(keyValuePair.Value));
				}
			}
		}
	}

	protected override void OnCmpDisable()
	{
		global::UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		base.OnCmpDisable();
		foreach (TableColumn tableColumn in this.columns.Values)
		{
			foreach (TableRow tableRow in this.rows)
			{
				GameObject widget = tableRow.GetWidget(tableColumn);
				if (!(widget == null))
				{
					GroupSelectorWidget[] componentsInChildren = widget.GetComponentsInChildren<GroupSelectorWidget>();
					if (componentsInChildren != null)
					{
						foreach (GroupSelectorWidget groupSelectorWidget in componentsInChildren)
						{
							groupSelectorWidget.CloseSubPanel();
						}
					}
					GroupSelectorHeaderWidget[] componentsInChildren2 = widget.GetComponentsInChildren<GroupSelectorHeaderWidget>();
					if (componentsInChildren2 != null)
					{
						foreach (GroupSelectorHeaderWidget groupSelectorHeaderWidget in componentsInChildren2)
						{
							groupSelectorHeaderWidget.CloseSubPanel();
						}
					}
					SelectablePanel[] componentsInChildren3 = widget.GetComponentsInChildren<SelectablePanel>();
					if (componentsInChildren3 != null)
					{
						foreach (SelectablePanel selectablePanel in componentsInChildren3)
						{
							selectablePanel.gameObject.SetActive(false);
						}
					}
				}
			}
		}
		this.optionsPanel.gameObject.SetActive(false);
	}

	private void GetMouseHoverInfo(out bool is_hovering_screen, out bool is_hovering_button)
	{
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		if (current == null)
		{
			is_hovering_button = false;
			is_hovering_screen = false;
		}
		List<RaycastResult> list = new List<RaycastResult>();
		current.RaycastAll(new PointerEventData(current)
		{
			position = Input.mousePosition
		}, list);
		bool flag = false;
		bool flag2 = false;
		foreach (RaycastResult raycastResult in list)
		{
			if (raycastResult.gameObject.GetComponent<OptionSelector>() != null || (raycastResult.gameObject.transform.parent != null && raycastResult.gameObject.transform.parent.GetComponent<OptionSelector>() != null))
			{
				flag = true;
				flag2 = true;
				break;
			}
			if (this.HasParent(raycastResult.gameObject, base.gameObject))
			{
				flag2 = true;
			}
		}
		is_hovering_screen = flag2;
		is_hovering_button = flag;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		bool flag = false;
		if (e.IsAction(global::Action.MouseRight))
		{
			bool flag2;
			bool flag3;
			this.GetMouseHoverInfo(out flag2, out flag3);
			if (flag2)
			{
				flag = true;
				if (!e.Consumed)
				{
					e.TryConsume(global::Action.MouseRight);
				}
			}
		}
		if (!flag)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		bool flag = false;
		if (e.IsAction(global::Action.MouseRight))
		{
			bool flag2;
			bool flag3;
			this.GetMouseHoverInfo(out flag2, out flag3);
			if (flag2)
			{
				flag = true;
				if (!flag3)
				{
					UISounds.PlaySound(UISounds.Sound.Negative);
				}
				if (!e.Consumed)
				{
					e.TryConsume(global::Action.MouseRight);
				}
			}
		}
		if (!flag)
		{
			base.OnKeyUp(e);
		}
	}

	private bool HasParent(GameObject obj, GameObject parent)
	{
		bool flag = false;
		Transform transform = parent.transform;
		Transform transform2 = obj.transform;
		while (transform2 != null)
		{
			if (transform2 == transform)
			{
				flag = true;
				break;
			}
			transform2 = transform2.parent;
		}
		return flag;
	}

	private void ConfigureNameLabel(MinionIdentity minion, GameObject widget_go)
	{
		base.on_load_name_label(minion, widget_go);
		if (minion == null)
		{
			return;
		}
		ToolTip component = widget_go.GetComponent<ToolTip>();
		if (component != null)
		{
			ToolTip toolTip = component;
			toolTip.OnToolTip = (Func<string>)Delegate.Combine(toolTip.OnToolTip, new Func<string>(delegate
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<b>" + UI.DETAILTABS.STATS.NAME + "</b>");
				foreach (AttributeInstance attributeInstance in minion.GetAttributes())
				{
					if (attributeInstance.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill)
					{
						string text = UIConstants.ColorPrefixWhite;
						if (attributeInstance.GetTotalValue() > 0f)
						{
							text = UIConstants.ColorPrefixGreen;
						}
						else if (attributeInstance.GetTotalValue() < 0f)
						{
							text = UIConstants.ColorPrefixRed;
						}
						stringBuilder.Append(string.Concat(new object[]
						{
							"\n    • ",
							attributeInstance.Name,
							": ",
							text,
							attributeInstance.GetTotalValue(),
							UIConstants.ColorSuffix
						}));
					}
				}
				return stringBuilder.ToString();
			}));
		}
	}

	private void InitializeHeader(ChoreGroup chore_group, GameObject widget_go)
	{
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		HierarchyReferences hierarchyReferences = component.GetReference("PrioritizationWidget") as HierarchyReferences;
		GameObject items_root = hierarchyReferences.GetReference("ItemPanel").gameObject;
		if (items_root.transform.childCount > 0)
		{
			return;
		}
		items_root.SetActive(false);
		LocText locText = component.GetReference("Label") as LocText;
		locText.text = chore_group.Name;
		KButton kbutton = component.GetReference("PrioritizeButton") as KButton;
		Selectable selectable = items_root.GetComponent<Selectable>();
		kbutton.onClick += delegate
		{
			selectable.Select();
			items_root.SetActive(true);
		};
		GameObject gameObject = hierarchyReferences.GetReference("ItemTemplate").gameObject;
		for (int i = 5; i >= 0; i--)
		{
			JobsTableScreen.PriorityInfo priorityInfo = this.GetPriorityInfo(i);
			if (priorityInfo.name != null)
			{
				GameObject gameObject2 = Util.KInstantiateUI(gameObject, items_root, true);
				KButton component2 = gameObject2.GetComponent<KButton>();
				HierarchyReferences component3 = gameObject2.GetComponent<HierarchyReferences>();
				KImage kimage = component3.GetReference("Icon") as KImage;
				LocText locText2 = component3.GetReference("Label") as LocText;
				int new_priority = i;
				component2.onClick += delegate
				{
					this.ChangeColumnPriority(widget_go, new_priority);
					global::UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
				};
				kimage.sprite = priorityInfo.sprite;
				locText2.text = priorityInfo.name;
			}
		}
	}

	private void OnSettingsButtonClicked()
	{
		this.optionsPanel.gameObject.SetActive(true);
		this.optionsPanel.GetComponent<Selectable>().Select();
	}

	private void OnResetSettingsClicked()
	{
		if (Game.Instance.advancedPersonalPriorities)
		{
			if (Immigration.Instance != null)
			{
				Immigration.Instance.ResetPersonalPriorities();
			}
			foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
			{
				if (!(minionIdentity == null))
				{
					Immigration.Instance.ApplyDefaultPersonalPriorities(minionIdentity.gameObject);
					Game.Instance.roleManager.ResetPersonalPriorities(minionIdentity);
				}
			}
		}
		else
		{
			foreach (MinionIdentity minionIdentity2 in Components.LiveMinionIdentities)
			{
				if (!(minionIdentity2 == null))
				{
					ChoreConsumer component = minionIdentity2.GetComponent<ChoreConsumer>();
					foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups)
					{
						component.SetPersonalPriority(choreGroup, 3, false);
					}
					Game.Instance.roleManager.ResetPersonalPriorities(minionIdentity2);
				}
			}
		}
		base.MarkRowsDirty();
	}

	private void OnAdvancedModeToggleClicked()
	{
		Game.Instance.advancedPersonalPriorities = !Game.Instance.advancedPersonalPriorities;
		this.toggleAdvancedModeButton.fgImage.gameObject.SetActive(Game.Instance.advancedPersonalPriorities);
	}

	[SerializeField]
	private Color32 skillOutlineColourLow = Color.white;

	[SerializeField]
	private Color32 skillOutlineColourHigh = new Color(0.72156864f, 0.44313726f, 0.5803922f);

	[SerializeField]
	private int skillLevelLow = 1;

	[SerializeField]
	private int skillLevelHigh = 10;

	[SerializeField]
	private KButton settingsButton;

	[SerializeField]
	private KButton resetSettingsButton;

	[SerializeField]
	private KButton toggleAdvancedModeButton;

	[SerializeField]
	private KImage optionsPanel;

	public static JobsTableScreen Instance;

	[SerializeField]
	private bool dynamicRowSpacing = true;

	public TextStyleSetting TooltipTextStyle_Ability;

	public TextStyleSetting TooltipTextStyle_AbilityPositiveModifier;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private HashSet<MinionIdentity> dirty_single_minion_rows = new HashSet<MinionIdentity>();

	private List<JobsTableScreen.PriorityInfo> priorityInfo;

	private List<Sprite> prioritySprites;

	private List<KeyValuePair<GameObject, JobsTableScreen.SkillEventHandlerID>> EffectListeners = new List<KeyValuePair<GameObject, JobsTableScreen.SkillEventHandlerID>>();

	private struct PriorityInfo
	{
		public PriorityInfo(int priority, Sprite sprite, LocString name)
		{
			this.priority = priority;
			this.sprite = sprite;
			this.name = name;
		}

		public int priority;

		public Sprite sprite;

		public LocString name;
	}

	private struct SkillEventHandlerID
	{
		public int level_up;

		public int effect_added;

		public int effect_removed;

		public int disease_added;

		public int disease_cured;
	}
}
