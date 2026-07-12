using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ResetSkillsStation")]
public class ResetSkillsStation : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.lightEfficiencyBonus = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OnAssign(this.assignable.assignee);
		this.assignable.OnAssign += this.OnAssign;
	}

	private void OnAssign(IAssignableIdentity obj)
	{
		if (obj != null)
		{
			this.CreateChore();
			return;
		}
		if (this.chore != null)
		{
			this.chore.Cancel("Unassigned");
			this.chore = null;
		}
	}

	private void CreateChore()
	{
		this.chore = new WorkChore<ResetSkillsStation>(Db.Get().ChoreTypes.UnlearnSkill, this, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		base.GetComponent<Operational>().SetActive(true, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.assignable.Unassign();
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null)
		{
			component.ResetSkillLevels(true);
			component.SetHats(component.CurrentHat, null);
			component.ApplyTargetHat();
			this.notification = new Notification(MISC.NOTIFICATIONS.RESETSKILL.NAME, NotificationType.Good, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.RESETSKILL.TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null, true, false, false);
			worker.GetComponent<Notifier>().Add(this.notification, "");
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		base.GetComponent<Operational>().SetActive(false, false);
		this.chore = null;
	}

	[MyCmpReq]
	public Assignable assignable;

	private Notification notification;

	private Chore chore;
}
