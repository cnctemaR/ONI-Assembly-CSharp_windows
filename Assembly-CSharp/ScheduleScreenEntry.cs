using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduleScreenEntry")]
public class ScheduleScreenEntry : KMonoBehaviour
{
	public Schedule schedule { get; private set; }

	public void Setup(Schedule schedule, Dictionary<string, ColorStyleSetting> paintStyles, Action<ScheduleScreenEntry, float> onPaintDragged)
	{
		this.schedule = schedule;
		base.gameObject.name = "Schedule_" + schedule.name;
		this.title.SetTitle(schedule.name);
		this.title.OnNameChanged += this.OnNameChanged;
		this.blockButtonContainer.Setup(delegate(float f)
		{
			onPaintDragged(this, f);
		});
		int num = 0;
		this.blockButtons = new List<ScheduleBlockButton>();
		int count = schedule.GetBlocks().Count;
		foreach (ScheduleBlock scheduleBlock in schedule.GetBlocks())
		{
			ScheduleBlockButton scheduleBlockButton = Util.KInstantiateUI<ScheduleBlockButton>(this.blockButtonPrefab.gameObject, this.blockButtonContainer.gameObject, true);
			scheduleBlockButton.Setup(num++, paintStyles, count);
			scheduleBlockButton.SetBlockTypes(scheduleBlock.allowed_types);
			this.blockButtons.Add(scheduleBlockButton);
		}
		this.minionWidgets = new List<ScheduleMinionWidget>();
		this.blankMinionWidget = Util.KInstantiateUI<ScheduleMinionWidget>(this.minionWidgetPrefab.gameObject, this.minionWidgetContainer, false);
		this.blankMinionWidget.SetupBlank(schedule);
		this.RebuildMinionWidgets();
		this.RefreshNotes();
		this.RefreshAlarmButton();
		this.optionsButton.onClick += this.OnOptionsClicked;
		HierarchyReferences component = this.optionsPanel.GetComponent<HierarchyReferences>();
		MultiToggle reference = component.GetReference<MultiToggle>("AlarmButton");
		reference.onClick = (global::System.Action)Delegate.Combine(reference.onClick, new global::System.Action(this.OnAlarmClicked));
		component.GetReference<KButton>("ResetButton").onClick += this.OnResetClicked;
		component.GetReference<KButton>("DeleteButton").onClick += this.OnDeleteClicked;
		schedule.onChanged = (Action<Schedule>)Delegate.Combine(schedule.onChanged, new Action<Schedule>(this.OnScheduleChanged));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.schedule != null)
		{
			Schedule schedule = this.schedule;
			schedule.onChanged = (Action<Schedule>)Delegate.Remove(schedule.onChanged, new Action<Schedule>(this.OnScheduleChanged));
		}
	}

	public GameObject GetNameInputField()
	{
		return this.title.inputField.gameObject;
	}

	private void RebuildMinionWidgets()
	{
		if (!this.MinionWidgetsNeedRebuild())
		{
			return;
		}
		foreach (ScheduleMinionWidget scheduleMinionWidget in this.minionWidgets)
		{
			Util.KDestroyGameObject(scheduleMinionWidget);
		}
		this.minionWidgets.Clear();
		foreach (Ref<Schedulable> @ref in this.schedule.GetAssigned())
		{
			ScheduleMinionWidget scheduleMinionWidget2 = Util.KInstantiateUI<ScheduleMinionWidget>(this.minionWidgetPrefab.gameObject, this.minionWidgetContainer, true);
			scheduleMinionWidget2.Setup(@ref.Get());
			this.minionWidgets.Add(scheduleMinionWidget2);
		}
		if (Components.LiveMinionIdentities.Count > this.schedule.GetAssigned().Count)
		{
			this.blankMinionWidget.transform.SetAsLastSibling();
			this.blankMinionWidget.gameObject.SetActive(true);
			return;
		}
		this.blankMinionWidget.gameObject.SetActive(false);
	}

	private bool MinionWidgetsNeedRebuild()
	{
		List<Ref<Schedulable>> assigned = this.schedule.GetAssigned();
		if (assigned.Count != this.minionWidgets.Count)
		{
			return true;
		}
		if (assigned.Count != Components.LiveMinionIdentities.Count != this.blankMinionWidget.gameObject.activeSelf)
		{
			return true;
		}
		for (int i = 0; i < assigned.Count; i++)
		{
			if (assigned[i].Get() != this.minionWidgets[i].schedulable)
			{
				return true;
			}
		}
		return false;
	}

	public void RefreshWidgetWorldData()
	{
		foreach (ScheduleMinionWidget scheduleMinionWidget in this.minionWidgets)
		{
			if (!scheduleMinionWidget.IsNullOrDestroyed())
			{
				scheduleMinionWidget.RefreshWidgetWorldData();
			}
		}
	}

	private void OnNameChanged(string newName)
	{
		this.schedule.name = newName;
		base.gameObject.name = "Schedule_" + this.schedule.name;
	}

	private void OnOptionsClicked()
	{
		this.optionsPanel.gameObject.SetActive(!this.optionsPanel.gameObject.activeSelf);
		this.optionsPanel.GetComponent<Selectable>().Select();
	}

	private void OnAlarmClicked()
	{
		this.schedule.alarmActivated = !this.schedule.alarmActivated;
		this.RefreshAlarmButton();
	}

	private void RefreshAlarmButton()
	{
		MultiToggle reference = this.optionsPanel.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("AlarmButton");
		reference.ChangeState(this.schedule.alarmActivated ? 1 : 0);
		ToolTip component = reference.GetComponent<ToolTip>();
		component.SetSimpleTooltip(this.schedule.alarmActivated ? UI.SCHEDULESCREEN.ALARM_BUTTON_ON_TOOLTIP : UI.SCHEDULESCREEN.ALARM_BUTTON_OFF_TOOLTIP);
		ToolTipScreen.Instance.MarkTooltipDirty(component);
		this.alarmField.text = (this.schedule.alarmActivated ? UI.SCHEDULESCREEN.ALARM_TITLE_ENABLED : UI.SCHEDULESCREEN.ALARM_TITLE_DISABLED);
	}

	private void OnResetClicked()
	{
		this.schedule.SetBlocksToGroupDefaults(Db.Get().ScheduleGroups.allGroups);
	}

	private void OnDeleteClicked()
	{
		ScheduleManager.Instance.DeleteSchedule(this.schedule);
	}

	private void OnScheduleChanged(Schedule changedSchedule)
	{
		foreach (ScheduleBlockButton scheduleBlockButton in this.blockButtons)
		{
			scheduleBlockButton.SetBlockTypes(changedSchedule.GetBlock(scheduleBlockButton.idx).allowed_types);
		}
		this.RefreshNotes();
		this.RebuildMinionWidgets();
	}

	private void RefreshNotes()
	{
		this.blockTypeCounts.Clear();
		foreach (ScheduleBlockType scheduleBlockType in Db.Get().ScheduleBlockTypes.resources)
		{
			this.blockTypeCounts[scheduleBlockType.Id] = 0;
		}
		foreach (ScheduleBlock scheduleBlock in this.schedule.GetBlocks())
		{
			foreach (ScheduleBlockType scheduleBlockType2 in scheduleBlock.allowed_types)
			{
				Dictionary<string, int> dictionary = this.blockTypeCounts;
				string id = scheduleBlockType2.Id;
				int num = dictionary[id];
				dictionary[id] = num + 1;
			}
		}
		if (this.noteEntryRight == null)
		{
			return;
		}
		ToolTip component = this.noteEntryRight.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		int num2 = 0;
		foreach (KeyValuePair<string, int> keyValuePair in this.blockTypeCounts)
		{
			if (keyValuePair.Value == 0)
			{
				num2++;
				component.AddMultiStringTooltip(string.Format(UI.SCHEDULEGROUPS.NOTIME, Db.Get().ScheduleBlockTypes.Get(keyValuePair.Key).Name), null);
			}
		}
		if (num2 > 0)
		{
			this.noteEntryRight.text = string.Format(UI.SCHEDULEGROUPS.MISSINGBLOCKS, num2);
		}
		else
		{
			this.noteEntryRight.text = "";
		}
		string breakBonus = QualityOfLifeNeed.GetBreakBonus(this.blockTypeCounts[Db.Get().ScheduleBlockTypes.Recreation.Id]);
		if (breakBonus != null)
		{
			Effect effect = Db.Get().effects.Get(breakBonus);
			if (effect != null)
			{
				foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
				{
					if (attributeModifier.AttributeId == Db.Get().Attributes.QualityOfLife.Id)
					{
						this.noteEntryLeft.text = string.Format(UI.SCHEDULESCREEN.DOWNTIME_MORALE, attributeModifier.GetFormattedString());
						this.noteEntryLeft.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.SCHEDULESCREEN.SCHEDULE_DOWNTIME_MORALE, attributeModifier.GetFormattedString()));
					}
				}
			}
		}
	}

	[SerializeField]
	private ScheduleBlockButton blockButtonPrefab;

	[SerializeField]
	private ScheduleBlockPainter blockButtonContainer;

	[SerializeField]
	private ScheduleMinionWidget minionWidgetPrefab;

	[SerializeField]
	private GameObject minionWidgetContainer;

	private ScheduleMinionWidget blankMinionWidget;

	[SerializeField]
	private EditableTitleBar title;

	[SerializeField]
	private LocText alarmField;

	[SerializeField]
	private KButton optionsButton;

	[SerializeField]
	private DialogPanel optionsPanel;

	[SerializeField]
	private LocText noteEntryLeft;

	[SerializeField]
	private LocText noteEntryRight;

	private List<ScheduleBlockButton> blockButtons;

	private List<ScheduleMinionWidget> minionWidgets;

	private Dictionary<string, int> blockTypeCounts = new Dictionary<string, int>();
}
