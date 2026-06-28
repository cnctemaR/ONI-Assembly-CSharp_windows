using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class Telepad : StateMachineComponent<Telepad.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.GetComponent<Deconstructable>().allowDeconstruction = false;
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(Grid.PosToCell(this), out num, out num2);
		if (num == 0)
		{
			global::Debug.LogError(string.Concat(new string[]
			{
				"Headquarters spawned at: (",
				num.ToString(),
				",",
				num2.ToString(),
				")"
			}), null);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Telepads.Add(this);
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		this.meter.gameObject.SetActive(false);
		this.meter.gameObject.SetActive(true);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		Components.Telepads.Remove(this);
		base.OnCleanUp();
	}

	public void Update()
	{
		if (base.smi.IsColonyLost())
		{
			return;
		}
		if (Immigration.Instance.ImmigrantsAvailable)
		{
			base.smi.sm.openPortal.Trigger(base.smi);
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.NewDuplicantsAvailable, this);
		}
		else
		{
			base.smi.sm.closePortal.Trigger(base.smi);
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Wattson, this);
		}
		if (this.GetTimeRemaining() < -120f)
		{
			Messenger.Instance.QueueMessage(new DuplicantsLeftMessage());
			Immigration.Instance.SpawnMinions();
		}
	}

	public void RejectAll()
	{
		Immigration.Instance.SpawnMinions();
		base.smi.sm.closePortal.Trigger(base.smi);
	}

	public void OnClickImmigrant(MinionStartingStats starting_stats)
	{
		int num = Grid.PosToCell(this);
		int num2 = Immigration.Instance.SpawnMinions();
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
		{
			minionIdentity.GetComponent<Effects>().Add("NewCrewArrival", true);
		}
		for (int i = 0; i < num2; i++)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), SceneOrganizer.Instance.GetFolder(Folder.Minions), null);
			gameObject.transform.SetLocalPosition(Grid.CellToPosCBC(num, Grid.SceneLayer.Move));
			gameObject.SetActive(true);
			starting_stats.Apply(gameObject);
			Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
			ChoreProvider component = gameObject.GetComponent<ChoreProvider>();
			new EmoteChore(component, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", Telepad.PortalBirthAnim, null);
		}
		base.smi.sm.closePortal.Trigger(base.smi);
	}

	public float GetTimeRemaining()
	{
		return Immigration.Instance.GetTimeRemaining();
	}

	[MyCmpReq]
	private KSelectable selectable;

	private MeterController meter;

	private const float MAX_IMMIGRATION_TIME = 120f;

	private const int NUM_METER_NOTCHES = 8;

	private List<MinionStartingStats> minionStats;

	private static readonly HashedString[] PortalBirthAnim = new HashedString[] { "portalbirth" };

	public class StatesInstance : GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.GameInstance
	{
		public StatesInstance(Telepad master)
			: base(master)
		{
		}

		public bool IsColonyLost()
		{
			return GameFlowManager.Instance != null && GameFlowManager.Instance.IsGameOver();
		}

		public void UpdateMeter()
		{
			float timeRemaining = Immigration.Instance.GetTimeRemaining();
			float totalWaitTime = Immigration.Instance.GetTotalWaitTime();
			float num = Mathf.Clamp01(1f - timeRemaining / totalWaitTime);
			base.master.meter.SetPositionPercent(num);
		}
	}

	public class States : GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			base.serializable = true;
			this.idle.Enter(delegate(Telepad.StatesInstance smi)
			{
				smi.UpdateMeter();
			}).Update("TelepadMeter", delegate(Telepad.StatesInstance smi, float dt)
			{
				smi.UpdateMeter();
			}, UpdateRate.SIM_4000ms, false).EventTransition(GameHashes.OperationalChanged, this.unoperational, (Telepad.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.PlayAnim("idle")
				.OnSignal(this.openPortal, this.opening);
			this.unoperational.PlayAnim("idle").Enter("StopImmigration", delegate(Telepad.StatesInstance smi)
			{
				Immigration.Instance.Stop();
				smi.master.meter.SetPositionPercent(0f);
			}).Exit("StartImmigration", delegate(Telepad.StatesInstance smi)
			{
				Immigration.Instance.Restart();
			})
				.EventTransition(GameHashes.OperationalChanged, this.idle, (Telepad.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			this.opening.Enter(delegate(Telepad.StatesInstance smi)
			{
				smi.master.meter.SetPositionPercent(1f);
			}).PlayAnim("working_pre").OnAnimQueueComplete(this.open);
			this.open.OnSignal(this.closePortal, this.close).Enter(delegate(Telepad.StatesInstance smi)
			{
				smi.master.meter.SetPositionPercent(1f);
			}).PlayAnim("working_loop", KAnim.PlayMode.Loop)
				.Transition(this.close, (Telepad.StatesInstance smi) => smi.IsColonyLost(), UpdateRate.SIM_200ms)
				.EventTransition(GameHashes.OperationalChanged, this.close, (Telepad.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.close.Enter(delegate(Telepad.StatesInstance smi)
			{
				smi.master.meter.SetPositionPercent(0f);
			}).PlayAnims((Telepad.StatesInstance smi) => Telepad.States.workingAnims, KAnim.PlayMode.Once).OnAnimQueueComplete(this.idle);
		}

		public StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Signal openPortal;

		public StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Signal closePortal;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State idle;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State opening;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State open;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State close;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State unoperational;

		private static readonly HashedString[] workingAnims = new HashedString[] { "working_loop", "working_pst" };
	}
}
