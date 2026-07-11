using System;
using System.Linq;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ScheduleMinionWidget : KMonoBehaviour
{
	public Schedulable schedulable { get; private set; }

	public void ChangeAssignment(Schedule targetSchedule, Schedulable schedulable)
	{
		DebugUtil.LogArgs(new object[]
		{
			"Assigning",
			schedulable,
			"from",
			ScheduleManager.Instance.GetSchedule(schedulable).name,
			"to",
			targetSchedule.name
		});
		ScheduleManager.Instance.GetSchedule(schedulable).Unassign(schedulable);
		targetSchedule.Assign(schedulable);
	}

	public void Setup(Schedulable schedulable)
	{
		this.schedulable = schedulable;
		IAssignableIdentity component = schedulable.GetComponent<IAssignableIdentity>();
		this.portrait.SetIdentityObject(component, true);
		this.label.text = component.GetProperName();
		MinionIdentity minionIdentity = component as MinionIdentity;
		StoredMinionIdentity storedMinionIdentity = component as StoredMinionIdentity;
		if (minionIdentity != null)
		{
			Traits component2 = minionIdentity.GetComponent<Traits>();
			if (component2.HasTrait("NightOwl"))
			{
				this.nightOwlIcon.SetActive(true);
			}
			else if (component2.HasTrait("EarlyBird"))
			{
				this.earlyBirdIcon.SetActive(true);
			}
		}
		else if (storedMinionIdentity != null)
		{
			if (storedMinionIdentity.traitIDs.Contains("NightOwl"))
			{
				this.nightOwlIcon.SetActive(true);
			}
			else if (storedMinionIdentity.traitIDs.Contains("EarlyBird"))
			{
				this.earlyBirdIcon.SetActive(true);
			}
		}
		this.dropDown.Initialize(ScheduleManager.Instance.GetSchedules().Cast<IListableOption>(), new Action<IListableOption, object>(this.OnDropEntryClick), null, new Action<DropDownEntry, object>(this.DropEntryRefreshAction), false, schedulable);
	}

	private void OnDropEntryClick(IListableOption option, object obj)
	{
		Schedule schedule = (Schedule)option;
		this.ChangeAssignment(schedule, this.schedulable);
	}

	private void DropEntryRefreshAction(DropDownEntry entry, object obj)
	{
		Schedule schedule = (Schedule)entry.entryData;
		if (((Schedulable)obj).GetSchedule() == schedule)
		{
			entry.label.text = string.Format(UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_ASSIGNED, schedule.name);
			entry.button.isInteractable = false;
			return;
		}
		entry.label.text = schedule.name;
		entry.button.isInteractable = true;
	}

	public void SetupBlank(Schedule schedule)
	{
		this.label.text = UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_BLANK;
		this.dropDown.Initialize(Components.LiveMinionIdentities.Items.Cast<IListableOption>(), new Action<IListableOption, object>(this.OnBlankDropEntryClick), new Func<IListableOption, IListableOption, object, int>(this.BlankDropEntrySort), new Action<DropDownEntry, object>(this.BlankDropEntryRefreshAction), false, schedule);
		Components.LiveMinionIdentities.OnAdd += this.OnLivingMinionsChanged;
		Components.LiveMinionIdentities.OnRemove += this.OnLivingMinionsChanged;
	}

	private void OnLivingMinionsChanged(MinionIdentity minion)
	{
		this.dropDown.ChangeContent(Components.LiveMinionIdentities.Items.Cast<IListableOption>());
	}

	private void OnBlankDropEntryClick(IListableOption option, object obj)
	{
		Schedule schedule = (Schedule)obj;
		MinionIdentity minionIdentity = (MinionIdentity)option;
		this.ChangeAssignment(schedule, minionIdentity.GetComponent<Schedulable>());
	}

	private void BlankDropEntryRefreshAction(DropDownEntry entry, object obj)
	{
		Schedule schedule = (Schedule)obj;
		MinionIdentity minionIdentity = (MinionIdentity)entry.entryData;
		if (schedule.IsAssigned(minionIdentity.GetComponent<Schedulable>()))
		{
			entry.label.text = string.Format(UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_ASSIGNED, minionIdentity.GetProperName());
			entry.button.isInteractable = false;
		}
		else
		{
			entry.label.text = minionIdentity.GetProperName();
			entry.button.isInteractable = true;
		}
		Traits component = minionIdentity.GetComponent<Traits>();
		entry.gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("NightOwlIcon").gameObject.SetActive(component.HasTrait("NightOwl"));
		entry.gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("EarlyBirdIcon").gameObject.SetActive(component.HasTrait("EarlyBird"));
	}

	private int BlankDropEntrySort(IListableOption a, IListableOption b, object obj)
	{
		Schedule schedule = (Schedule)obj;
		MinionIdentity minionIdentity = (MinionIdentity)a;
		MinionIdentity minionIdentity2 = (MinionIdentity)b;
		bool flag = schedule.IsAssigned(minionIdentity.GetComponent<Schedulable>());
		bool flag2 = schedule.IsAssigned(minionIdentity2.GetComponent<Schedulable>());
		if (flag && !flag2)
		{
			return -1;
		}
		if (!flag && flag2)
		{
			return 1;
		}
		return 0;
	}

	protected override void OnCleanUp()
	{
		Components.LiveMinionIdentities.OnAdd -= this.OnLivingMinionsChanged;
		Components.LiveMinionIdentities.OnRemove -= this.OnLivingMinionsChanged;
	}

	[SerializeField]
	private CrewPortrait portrait;

	[SerializeField]
	private DropDown dropDown;

	[SerializeField]
	private LocText label;

	[SerializeField]
	private GameObject nightOwlIcon;

	[SerializeField]
	private GameObject earlyBirdIcon;
}
