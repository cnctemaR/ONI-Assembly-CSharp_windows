using System;
using System.Collections.Generic;
using Klei.AI;

public class DefragmentationZone : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetReportType(ReportManager.ReportType.PersonalTime);
		this.showProgressBar = false;
		this.workerStatusItem = null;
		this.synchronizeAnims = false;
		this.triggerWorkReactions = false;
		this.lightEfficiencyBonus = false;
		this.approachable = base.GetComponent<IApproachable>();
		this.workAnims = new HashedString[] { "microchip_bed_pre", "microchip_bed_loop" };
		this.workingPstComplete = new HashedString[] { "microchip_bed_pst" };
		this.workingPstFailed = new HashedString[] { "microchip_bed_pst" };
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.SetWorkTime(float.PositiveInfinity);
		this.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Combine(this.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent));
	}

	private void OnWorkableEvent(Workable workable, Workable.WorkableEvent workable_event)
	{
		if (workable_event == Workable.WorkableEvent.WorkStarted)
		{
			this.AddRoomEffects();
		}
	}

	private void AddRoomEffects()
	{
		if (base.worker == null)
		{
			return;
		}
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject == null)
		{
			return;
		}
		RoomType roomType = roomOfGameObject.roomType;
		List<EffectInstance> list = null;
		roomType.TriggerRoomEffects(base.GetComponent<KPrefabID>(), base.worker.GetComponent<Effects>(), out list);
		if (list != null)
		{
			foreach (EffectInstance effectInstance in list)
			{
				effectInstance.timeRemaining = 1800f;
			}
		}
	}

	public override bool InstantlyFinish(WorkerBase worker)
	{
		return false;
	}

	private const float BEDROOM_EFFECTS_DURATION_OVERRIDE = 1800f;

	[MyCmpGet]
	public Assignable assignable;

	public IApproachable approachable;
}
