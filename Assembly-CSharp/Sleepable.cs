using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Sleepable")]
public class Sleepable : Workable
{
	private Sleepable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
		this.showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = null;
		this.synchronizeAnims = false;
		this.triggerWorkReactions = false;
		this.lightEfficiencyBonus = false;
	}

	protected override void OnSpawn()
	{
		Components.Sleepables.Add(this);
		base.SetWorkTime(float.PositiveInfinity);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		if (base.GetComponent<Building>() != null && component != null && component.CurrentHat != null)
		{
			return Sleepable.hatWorkAnims;
		}
		return Sleepable.normalWorkAnims;
	}

	public override HashedString[] GetWorkPstAnims(Worker worker, bool successfully_completed)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		if (base.GetComponent<Building>() != null && component != null && component.CurrentHat != null)
		{
			return Sleepable.hatWorkPstAnim;
		}
		return Sleepable.normalWorkPstAnim;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (this.operational != null)
		{
			this.operational.SetActive(true, false);
		}
		worker.Trigger(-1283701846, this);
		worker.GetComponent<Effects>().Add(this.effectName, false);
		this.isDoneSleeping = false;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.isDoneSleeping)
		{
			if (Time.time > this.wakeTime)
			{
				return true;
			}
		}
		else if (worker.GetSMI<StaminaMonitor.Instance>().ShouldExitSleep())
		{
			this.isDoneSleeping = true;
			this.wakeTime = Time.time + global::UnityEngine.Random.value * 3f;
		}
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		if (this.operational != null)
		{
			this.operational.SetActive(false, false);
		}
		if (worker != null)
		{
			Effects component = worker.GetComponent<Effects>();
			component.Remove(this.effectName);
			if (this.wakeEffects != null)
			{
				foreach (string text in this.wakeEffects)
				{
					component.Add(text, true);
				}
			}
			if (this.stretchOnWake && global::UnityEngine.Random.value < 0.33f)
			{
				new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_morning_stretch_kanim", new HashedString[] { "react" }, null);
			}
			if (worker.GetAmounts().Get(Db.Get().Amounts.Stamina).value < worker.GetAmounts().Get(Db.Get().Amounts.Stamina).GetMax())
			{
				worker.Trigger(1338475637, this);
			}
		}
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Sleepables.Remove(this);
	}

	private const float STRECH_CHANCE = 0.33f;

	[MyCmpGet]
	private Operational operational;

	public string effectName = "Sleep";

	public List<string> wakeEffects;

	public bool stretchOnWake = true;

	private float wakeTime;

	private bool isDoneSleeping;

	private static readonly HashedString[] normalWorkAnims = new HashedString[] { "working_pre", "working_loop" };

	private static readonly HashedString[] hatWorkAnims = new HashedString[] { "hat_pre", "working_loop" };

	private static readonly HashedString[] normalWorkPstAnim = new HashedString[] { "working_pst" };

	private static readonly HashedString[] hatWorkPstAnim = new HashedString[] { "hat_pst" };
}
