using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ScheduleScreenEntry : KMonoBehaviour
{
	public Schedule schedule { get; private set; }

	public void Setup(Schedule schedule, Dictionary<string, ColorStyleSetting> paintStyles, Action<ScheduleScreenEntry, ScheduleBlockButton> onBlockClicked)
	{
		this.schedule = schedule;
		this.title.SetTitle(schedule.name);
		this.title.OnNameChanged += this.OnNameChanged;
		int num = 0;
		this.blockButtons = new List<ScheduleBlockButton>();
		foreach (ScheduleBlock scheduleBlock in schedule.GetBlocks())
		{
			ScheduleBlockButton scheduleBlockButton = Util.KInstantiateUI<ScheduleBlockButton>(this.blockButtonPrefab.gameObject, this.blockButtonContainer, true);
			scheduleBlockButton.Setup(num++, paintStyles, delegate(ScheduleBlockButton b)
			{
				onBlockClicked(this, b);
			});
			scheduleBlockButton.SetBlockTypes(scheduleBlock.allowed_types);
			this.blockButtons.Add(scheduleBlockButton);
		}
		this.minionWidgets = new List<ScheduleMinionWidget>();
		this.RebuildMinionWidgets();
		this.RefreshAlarmButton();
		this.alarmButton.onClick += this.OnAlarmClicked;
		this.deleteButton.onClick += this.OnDeleteClicked;
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
	}

	private void OnNameChanged(string newName)
	{
		this.schedule.name = newName;
	}

	private void OnAlarmClicked()
	{
		this.schedule.alarm = !this.schedule.alarm;
		this.RefreshAlarmButton();
	}

	private void RefreshAlarmButton()
	{
		this.alarmButton.isOn = this.schedule.alarm;
		ToolTip component = this.alarmButton.GetComponent<ToolTip>();
		component.SetSimpleTooltip((!this.schedule.alarm) ? UI.SCHEDULESCREEN.ALARM_BUTTON_OFF_TOOLTIP : UI.SCHEDULESCREEN.ALARM_BUTTON_ON_TOOLTIP);
		ToolTipScreen.Instance.MarkTooltipDirty(component);
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
		this.RebuildMinionWidgets();
	}

	[SerializeField]
	private ScheduleBlockButton blockButtonPrefab;

	[SerializeField]
	private GameObject blockButtonContainer;

	[SerializeField]
	private ScheduleMinionWidget minionWidgetPrefab;

	[SerializeField]
	private GameObject minionWidgetContainer;

	[SerializeField]
	private EditableTitleBar title;

	[SerializeField]
	private KToggle alarmButton;

	[SerializeField]
	private KButton deleteButton;

	private List<ScheduleBlockButton> blockButtons;

	private List<ScheduleMinionWidget> minionWidgets;
}
