using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Shockworm : StateMachineComponent<Shockworm.StatesInstance>, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = this.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		this.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.Subscribe(229718515, new Action<object>(this.OnThreatned));
		this.Subscribe(-21431934, new Action<object>(this.ClearThreat));
	}

	private void OnThreatned(object threat)
	{
		this.mainThreat = (GameObject)threat;
	}

	private void ClearThreat(object data)
	{
		this.mainThreat = null;
	}

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpAdd]
	private Weapon weapon;

	[MyCmpGet]
	public LoopingSounds loopingSounds;

	[NonSerialized]
	public string wingSound = "Shockworm_wings_LP";

	[SerializeField]
	private GameObject mainThreat;

	public class StatesInstance : GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.GameInstance
	{
		public StatesInstance(Shockworm smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.idleStates.idle;
			this.alive.EventTransition(GameHashes.Died, this.death, null).EventTransition(GameHashes.TooColdFatal, this.death, null).EventTransition(GameHashes.TooHotFatal, this.death, null)
				.EventTransition(GameHashes.Drowned, this.death, null)
				.EventTransition(GameHashes.Drowning, this.alive.distressed.Drowning, null)
				.EventTransition(GameHashes.EntombedChanged, this.death, null)
				.EventTransition(GameHashes.DebugGoTo, (Shockworm.StatesInstance smi) => Game.Instance, this.alive.idleStates.debug_go_to, null)
				.ToggleStateMachine((Shockworm.StatesInstance smi) => new ThreatMonitor.Instance(smi.master))
				.Enter(delegate(Shockworm.StatesInstance smi)
				{
					this.mover.Set(smi.master, smi);
					if (smi.master.loopingSounds != null)
					{
						smi.master.loopingSounds.AddLoopingSoundUpdater();
						smi.master.loopingSounds.StartSound(GlobalAssets.GetSound(smi.master.wingSound, false), smi.master.transform.position);
					}
				})
				.Exit(delegate(Shockworm.StatesInstance smi)
				{
					smi.master.loopingSounds.StopSound(GlobalAssets.GetSound(smi.master.wingSound, false));
					smi.master.loopingSounds.RemoveLoopingSoundUpdater();
				});
			this.alive.idleStates.EventTransition(GameHashes.Threatned, this.alive.attackStates.plan_attack, null);
			this.alive.attackStates.EventTransition(GameHashes.SafeFromThreats, this.alive.idleStates.idle, null);
			this.alive.distressed.Drowning.PlayAnim("hit", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.EnteredBreathableArea, this.alive.idleStates.move, null);
			this.alive.idleStates.idle.Enter(delegate(Shockworm.StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Once);
			}).EventHandler(GameHashes.AnimQueueComplete, delegate(Shockworm.StatesInstance smi)
			{
				smi.GoTo(this.alive.idleStates.move);
			}).EventTransition(GameHashes.TooColdFatal, this.death, null)
				.EventTransition(GameHashes.TooHotFatal, this.death, null);
			this.alive.attackStates.plan_attack.Enter(delegate(Shockworm.StatesInstance smi)
			{
				this.threatMoveTarget.Set(smi.master.mainThreat, smi);
				smi.GoTo(this.alive.attackStates.approachtarget);
			});
			this.alive.idleStates.move.InitializeStates(this.alive.idleStates.idle);
			this.alive.idleStates.debug_go_to.InitializeStates(this.alive.idleStates.idle);
			this.alive.attackStates.approachtarget.InitializeStates(this.mover, this.threatMoveTarget, this.alive.attackStates.regular, this.alive.idleStates.idle, OffsetGroups.Standard, NavigationTactics.Range_2_AvoidOverlaps);
			this.alive.attackStates.regular.PlayAnim("atk_pre", KAnim.PlayMode.Once, null).QueueAnim("atk_loop", false, null).QueueAnim("atk_pst", false, null)
				.OnAnimQueueComplete(this.alive.attackStates.plan_attack)
				.Enter(delegate(Shockworm.StatesInstance smi)
				{
					smi.Schedule(0.85f, delegate
					{
						smi.master.weapon.AttackArea(smi.master.transform.position);
					}, null);
				});
			this.death.ToggleGravity().PlayAnim("death", KAnim.PlayMode.Once, null).EventHandler(GameHashes.AnimQueueComplete, delegate(Shockworm.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.gameObject);
			})
				.Enter(delegate(Shockworm.StatesInstance smi)
				{
					Butcherable component = smi.master.GetComponent<Butcherable>();
					if (component)
					{
						component.OnButcherComplete();
					}
				});
		}

		public StateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.TargetParameter threatMoveTarget;

		public StateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.TargetParameter mover;

		public StateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.ObjectParameter<ThreatMonitor> threatMonitor;

		public Shockworm.States.AliveStates alive;

		public GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State death;

		public class AliveStates : GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State
		{
			public Shockworm.States.IdleStates idleStates;

			public Shockworm.States.AttackStates attackStates;

			public Shockworm.States.DistressStates distressed;
		}

		public class IdleStates : GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State
		{
			public GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State idle;

			public GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.IdleMoveSubState move;

			public GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.DebugGoToSubState debug_go_to;
		}

		public class AttackStates : GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State
		{
			public GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State plan_attack;

			public GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.ApproachSubState<AttackableBase> approachtarget;

			public GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State regular;
		}

		public class DistressStates : GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State
		{
			public GameStateMachine<Shockworm.States, Shockworm.StatesInstance, Shockworm, object>.State Drowning;
		}
	}
}
