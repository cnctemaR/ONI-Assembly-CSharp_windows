using System;
using KSerialization;
using UnityEngine;

public class OilEater : StateMachineComponent<OilEater.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.animController.randomiseLoopedOffset = true;
		base.smi.StartSM();
	}

	public void Exhaust(float dt)
	{
		if (!base.smi.master.wiltCondition.IsWilting())
		{
			this.emittedMass += dt * this.emitRate;
			if (this.emittedMass >= this.minEmitMass)
			{
				int num = Grid.PosToCell(base.transform.position + this.emitOffset);
				PrimaryElement component = base.GetComponent<PrimaryElement>();
				SimMessages.AddRemoveSubstance(num, SimHashes.CarbonDioxide, CellEventLogger.Instance.ElementEmitted, this.emittedMass, component.Temperature, byte.MaxValue, 0, -1);
				this.emittedMass = 0f;
			}
		}
	}

	private const SimHashes srcElement = SimHashes.CrudeOil;

	private const SimHashes emitElement = SimHashes.CarbonDioxide;

	public float emitRate = 1f;

	public float minEmitMass = 0f;

	public Vector3 emitOffset = Vector3.zero;

	[Serialize]
	private float emittedMass;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	public class StatesInstance : GameStateMachine<OilEater.States, OilEater.StatesInstance, OilEater, object>.GameInstance
	{
		public StatesInstance(OilEater master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<OilEater.States, OilEater.StatesInstance, OilEater>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grow;
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter(delegate(OilEater.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, delegate(object data)
				{
					GameObject gameObject = (GameObject)data;
					CreatureHelpers.DeselectCreature(gameObject);
					Util.KDestroyGameObject(gameObject);
				}, smi.master.gameObject);
			});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.alive, (OilEater.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, this.alive, (OilEater.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, this.alive, (OilEater.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, this.dead, (OilEater.StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			this.grow.Enter(delegate(OilEater.StatesInstance smi)
			{
				if (smi.master.receptacleMonitor.HasReceptacle() && !this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, this.alive, null);
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.mature).Update(delegate(OilEater.StatesInstance smi)
			{
				smi.master.Exhaust(smi.dt);
			});
			this.alive.mature.EventTransition(GameHashes.Wilt, this.alive.wilting, (OilEater.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop);
			this.alive.wilting.PlayAnim("wilt1").EventTransition(GameHashes.WiltRecover, this.alive.mature, (OilEater.StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}

		public GameStateMachine<OilEater.States, OilEater.StatesInstance, OilEater, object>.State grow;

		public GameStateMachine<OilEater.States, OilEater.StatesInstance, OilEater, object>.State blocked_from_growing;

		public OilEater.States.AliveStates alive;

		public GameStateMachine<OilEater.States, OilEater.StatesInstance, OilEater, object>.State dead;

		public class AliveStates : GameStateMachine<OilEater.States, OilEater.StatesInstance, OilEater, object>.PlantAliveSubState
		{
			public GameStateMachine<OilEater.States, OilEater.StatesInstance, OilEater, object>.State mature;

			public GameStateMachine<OilEater.States, OilEater.StatesInstance, OilEater, object>.State wilting;
		}
	}
}
