using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Telepad : StateMachineComponent<Telepad.StatesInstance>, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.GetComponent<Deconstructable>().allowDeconstruction = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
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
			minionIdentity.GetComponent<Effects>().Add("NewCrewArrival", false);
		}
		for (int i = 0; i < num2; i++)
		{
			GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.MinionPrefab, SceneOrganizer.Instance.GetFolder(Folder.Minions), null);
			gameObject.transform.localPosition = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			starting_stats.Apply(gameObject);
			ChoreProvider component = gameObject.GetComponent<ChoreProvider>();
			new EmoteChore(component, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new string[] { "portalbirth" }, null);
		}
		base.smi.sm.closePortal.Trigger(base.smi);
	}

	public float GetTimeRemaining()
	{
		return Immigration.Instance.GetTimeRemaining();
	}

	private const float MAX_IMMIGRATION_TIME = 120f;

	[MyCmpReq]
	private KSelectable selectable;

	private List<MinionStartingStats> minionStats;

	public class StatesInstance : GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad>.GameInstance
	{
		public StatesInstance(Telepad master)
			: base(master)
		{
		}

		public bool IsColonyLost()
		{
			return GameFlowManager.Instance != null && GameFlowManager.Instance.IsGameOver();
		}
	}

	public class States : GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			base.serializable = true;
			this.idle.PlayAnim("idle", KAnim.PlayMode.Once, null).OnSignal(this.openPortal, this.opening);
			this.opening.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.open);
			this.open.OnSignal(this.closePortal, this.close).PlayAnim("working_loop", KAnim.PlayMode.Loop, null).Transition(this.close, (Telepad.StatesInstance smi) => smi.IsColonyLost());
			this.close.PlayAnims((Telepad.StatesInstance smi) => new string[] { "working_loop", "working_pst" }, KAnim.PlayMode.Once).OnAnimQueueComplete(this.idle);
		}

		public StateMachine<Telepad.States, Telepad.StatesInstance, Telepad>.Signal openPortal;

		public StateMachine<Telepad.States, Telepad.StatesInstance, Telepad>.Signal closePortal;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad>.State idle;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad>.State opening;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad>.State open;

		public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad>.State close;
	}
}
