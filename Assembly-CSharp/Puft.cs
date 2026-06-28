using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Puft : StateMachineComponent<Puft.StatesInstance>, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = this.transform.position;
		this.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.consumer.enabled = false;
		base.smi.StartSM();
	}

	private int GetBreathMoveTarget()
	{
		return this.breathTargetCell;
	}

	private int FindTargetGasCell()
	{
		return GameUtil.FloodFillFind(new Func<int, bool>(this.isTargetElement), Grid.PosToCell(base.gameObject), 8);
	}

	private bool isTargetElement(int cell)
	{
		return ElementLoader.elements[(int)Grid.Cell[cell].elementIdx] == ElementLoader.FindElementByHash(SimHashes.ContaminatedOxygen) && Grid.Cell[cell].mass > 0.1f && this.nav.CanReach(cell);
	}

	public void OnAttacked(object data)
	{
		if (!base.smi.IsInsideState(base.smi.sm.alive.flee))
		{
			base.smi.GoTo(base.smi.sm.alive.flee);
		}
	}

	[MyCmpAdd]
	private ElementConsumer consumer;

	[MyCmpAdd]
	private ElementEmitter emitter;

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpAdd]
	private Navigator nav;

	private int breathTargetCell = -1;

	public class StatesInstance : GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.GameInstance
	{
		public StatesInstance(Puft smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Puft.States, Puft.StatesInstance, Puft>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.idle.idle;
			this.root.Enter(delegate(Puft.StatesInstance smi)
			{
				this.mover.Set(smi.master.gameObject, smi);
			});
			this.alive.ToggleStateMachine((Puft.StatesInstance smi) => new ThreatMonitor.Instance(smi.master)).EventTransition(GameHashes.TooColdFatal, this.death, null).EventTransition(GameHashes.TooHotFatal, this.death, null)
				.EventTransition(GameHashes.Drowning, this.alive.distressed.Drowning, null)
				.EventTransition(GameHashes.Drowned, this.death, null)
				.EventTransition(GameHashes.EntombedChanged, this.death, null)
				.EventTransition(GameHashes.Died, this.death, null)
				.Enter(delegate(Puft.StatesInstance smi)
				{
					smi.Subscribe(-787691065, new EventSystem.EventHandler(smi.master.OnAttacked));
				});
			this.alive.flee.InitializeStates(this.mover, this.alive.idle.idle);
			this.alive.distressed.Drowning.PlayAnim("harvest", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.EnteredBreathableArea, this.alive.idle.move, null);
			this.alive.idle.idle.Enter(delegate(Puft.StatesInstance smi)
			{
				smi.Play("idle_loop", KAnim.PlayMode.Loop);
				if (smi.master.consumer.IsElementAvailable && !Grid.Solid[Grid.CellAbove(Grid.PosToCell(smi.gameObject))])
				{
					smi.GoTo(this.alive.inhale.pre);
				}
				else
				{
					smi.Schedule(2f, delegate(object d)
					{
						int num = smi.master.FindTargetGasCell();
						if (num != -1)
						{
							smi.master.breathTargetCell = num;
							smi.GoTo(this.alive.moveToBreathable);
						}
						else
						{
							smi.ScheduleGoTo(2f, this.alive.idle.move);
						}
					}, null);
				}
			});
			this.alive.idle.move.InitializeStates(this.alive.idle.idle);
			this.alive.moveToBreathable.MoveTo((Puft.StatesInstance smi) => smi.master.GetBreathMoveTarget(), this.alive.idle.idle, this.alive.idle.idle, false);
			this.alive.full.alt.GoTo(this.alive.full.full).PlayAnim("idle_loop_full", KAnim.PlayMode.Loop, null);
			this.alive.full.full.MoveTo((Puft.StatesInstance smi) => Grid.CellAbove(Grid.PosToCell(smi.master.gameObject)), this.alive.full.alt, this.alive.full.fart, false).PlayAnim("idle_loop_full", KAnim.PlayMode.Loop, null);
			this.alive.full.fart.PlayAnim("fart", KAnim.PlayMode.Once, null).Enter(delegate(Puft.StatesInstance smi)
			{
				smi.Schedule(1f, delegate
				{
					smi.master.emitter.ForceEmit(smi.master.consumer.consumedMass, ElementLoader.elementTable[smi.master.emitter.outputElement.elementHash].defaultValues.temperature);
					smi.master.consumer.consumedMass = 0f;
				}, null);
			}).OnAnimQueueComplete(this.alive.idle.idle);
			this.alive.inhale.pre.Enter(delegate(Puft.StatesInstance smi)
			{
				smi.Play("inhale_pre", KAnim.PlayMode.Once);
			}).EventTransition(GameHashes.AnimQueueComplete, this.alive.inhale.loop, (Puft.StatesInstance smi) => smi.timeinstate > 0f);
			this.alive.inhale.loop.Enter(delegate(Puft.StatesInstance smi)
			{
				smi.master.consumer.enabled = true;
				smi.Play("inhale_loop", KAnim.PlayMode.Loop);
				smi.Schedule(3f, delegate
				{
					if (smi.master.consumer.consumedMass > 1f)
					{
						smi.GoTo(this.alive.inhale.pst);
					}
					else
					{
						smi.GoTo(this.alive.idle.move);
					}
				}, null);
			});
			this.alive.inhale.pst.Enter(delegate(Puft.StatesInstance smi)
			{
				smi.master.consumer.enabled = false;
				smi.Play("inhale_pst", KAnim.PlayMode.Once);
			}).EventTransition(GameHashes.AnimQueueComplete, this.alive.full.alt, null);
			this.death.ToggleGravity().PlayAnim("death", KAnim.PlayMode.Once, null).EventHandler(GameHashes.AnimQueueComplete, delegate(Puft.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.gameObject);
			})
				.Enter(delegate(Puft.StatesInstance smi)
				{
					smi.Schedule(2f, delegate(object d)
					{
						Util.KDestroyGameObject(smi.master.gameObject);
					}, null);
				});
		}

		public StateMachine<Puft.States, Puft.StatesInstance, Puft>.TargetParameter breathMoveTarget;

		public StateMachine<Puft.States, Puft.StatesInstance, Puft>.TargetParameter mover;

		public Puft.States.AliveStates alive;

		public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State death;

		public class AliveStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State
		{
			public Puft.States.IdleStates idle;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.ApproachSubState<Approachable> moveToBreathable;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.CreatureFleeSubState<Approachable> flee;

			public Puft.States.InhaleStates inhale;

			public Puft.States.FullStates full;

			public Puft.States.DistressStates distressed;
		}

		public class FullStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State
		{
			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.ApproachSubState<Approachable> full;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State alt;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State fart;
		}

		public class IdleStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State
		{
			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State idle;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.IdleMoveSubState move;
		}

		public class InhaleStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State
		{
			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State pre;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State loop;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State pst;
		}

		public class DistressStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State
		{
			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft>.State Drowning;
		}
	}
}
