using System;
using System.Linq;
using UnityEngine;

public class ScheduleMinionWidget : KMonoBehaviour
{
	public void ChangeAssignment(Schedule targetSchedule)
	{
		Output.Log(new object[]
		{
			"Assigning",
			this.schedulable,
			"from",
			ScheduleManager.Instance.GetSchedule(this.schedulable).name,
			"to",
			targetSchedule.name
		});
		ScheduleManager.Instance.GetSchedule(this.schedulable).Unassign(this.schedulable);
		targetSchedule.Assign(this.schedulable);
	}

	public void Setup(Schedulable schedulable)
	{
		this.schedulable = schedulable;
		IAssignableIdentity component = schedulable.GetComponent<IAssignableIdentity>();
		this.portrait.SetIdentityObject(component, true);
		this.label.text = component.GetProperName();
		this.dropDown.Initialize(ScheduleManager.Instance.GetSchedules().Cast<IListableOption>(), new Action<IListableOption, object>(this.OnDropEntryClick), null, new Action<DropDownEntry, object>(this.DropEntryRefreshAction), true, schedulable);
	}

	private void OnDropEntryClick(IListableOption option, object obj)
	{
		Schedule schedule = (Schedule)option;
		this.ChangeAssignment(schedule);
	}

	private void DropEntryRefreshAction(DropDownEntry entry, object obj)
	{
	}

	[SerializeField]
	private CrewPortrait portrait;

	[SerializeField]
	private DropDown dropDown;

	[SerializeField]
	private LocText label;

	private Schedulable schedulable;
}
