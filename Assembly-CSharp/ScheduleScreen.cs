using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScheduleScreen : KScreen
{
	public override float GetSortKey()
	{
		return 50f;
	}

	protected override void OnPrefabInit()
	{
		base.ConsumeMouseScroll = true;
		this.entries = new List<ScheduleScreenEntry>();
		this.paintStyles = new Dictionary<string, ColorStyleSetting>();
		this.paintStyles["Hygene"] = this.hygene_color;
		this.paintStyles["Worktime"] = this.work_color;
		this.paintStyles["Recreation"] = this.recreation_color;
		this.paintStyles["Sleep"] = this.sleep_color;
	}

	protected override void OnSpawn()
	{
		this.paintButtons = new List<SchedulePaintButton>();
		foreach (ScheduleGroup scheduleGroup in Db.Get().ScheduleGroups.allGroups)
		{
			this.AddPaintButton(scheduleGroup);
		}
		foreach (Schedule schedule in ScheduleManager.Instance.GetSchedules())
		{
			this.AddScheduleEntry(schedule);
		}
		this.addScheduleButton.onClick += this.OnAddScheduleClick;
		this.closeButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		ScheduleManager.Instance.onSchedulesChanged += this.OnSchedulesChanged;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		ScheduleManager.Instance.onSchedulesChanged -= this.OnSchedulesChanged;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			base.Activate();
		}
	}

	private void AddPaintButton(ScheduleGroup group)
	{
		SchedulePaintButton schedulePaintButton = Util.KInstantiateUI<SchedulePaintButton>(this.paintButtonPrefab.gameObject, this.paintButtonContainer, true);
		schedulePaintButton.SetGroup(group, this.paintStyles, new Action<SchedulePaintButton>(this.OnPaintButtonClick));
		schedulePaintButton.SetToggle(false);
		this.paintButtons.Add(schedulePaintButton);
	}

	private void OnAddScheduleClick()
	{
		ScheduleManager.Instance.AddDefaultSchedule(false);
	}

	private void OnPaintButtonClick(SchedulePaintButton clicked)
	{
		if (this.selectedPaint != clicked)
		{
			foreach (SchedulePaintButton schedulePaintButton in this.paintButtons)
			{
				schedulePaintButton.SetToggle(schedulePaintButton == clicked);
			}
			this.selectedPaint = clicked;
			return;
		}
		clicked.SetToggle(false);
		this.selectedPaint = null;
	}

	private void OnPaintDragged(ScheduleScreenEntry entry, float ratio)
	{
		if (this.selectedPaint == null)
		{
			return;
		}
		int num = Mathf.FloorToInt(ratio * (float)entry.schedule.GetBlocks().Count);
		entry.schedule.SetGroup(num, this.selectedPaint.group);
	}

	private void AddScheduleEntry(Schedule schedule)
	{
		ScheduleScreenEntry scheduleScreenEntry = Util.KInstantiateUI<ScheduleScreenEntry>(this.scheduleEntryPrefab.gameObject, this.scheduleEntryContainer, true);
		scheduleScreenEntry.Setup(schedule, this.paintStyles, new Action<ScheduleScreenEntry, float>(this.OnPaintDragged));
		this.entries.Add(scheduleScreenEntry);
	}

	private void OnSchedulesChanged(List<Schedule> schedules)
	{
		foreach (ScheduleScreenEntry scheduleScreenEntry in this.entries)
		{
			Util.KDestroyGameObject(scheduleScreenEntry);
		}
		this.entries.Clear();
		foreach (Schedule schedule in schedules)
		{
			this.AddScheduleEntry(schedule);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
				return;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool CheckBlockedInput()
	{
		bool flag = false;
		if (global::UnityEngine.EventSystems.EventSystem.current != null)
		{
			GameObject currentSelectedGameObject = global::UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				foreach (ScheduleScreenEntry scheduleScreenEntry in this.entries)
				{
					if (currentSelectedGameObject == scheduleScreenEntry.GetNameInputField())
					{
						flag = true;
						break;
					}
				}
			}
		}
		return flag;
	}

	[SerializeField]
	private SchedulePaintButton paintButtonPrefab;

	[SerializeField]
	private GameObject paintButtonContainer;

	[SerializeField]
	private ScheduleScreenEntry scheduleEntryPrefab;

	[SerializeField]
	private GameObject scheduleEntryContainer;

	[SerializeField]
	private KButton addScheduleButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private ColorStyleSetting hygene_color;

	[SerializeField]
	private ColorStyleSetting work_color;

	[SerializeField]
	private ColorStyleSetting recreation_color;

	[SerializeField]
	private ColorStyleSetting sleep_color;

	private Dictionary<string, ColorStyleSetting> paintStyles;

	private List<ScheduleScreenEntry> entries;

	private List<SchedulePaintButton> paintButtons;

	private SchedulePaintButton selectedPaint;
}
