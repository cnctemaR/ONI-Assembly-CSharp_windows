using System;
using UnityEngine;

public class Glom : StateMachineComponent<Glom.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Vector3 position = base.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		base.transform.SetPosition(position);
		this.Heading = Vector2.right;
		base.smi.StartSM();
	}

	private bool CellIsClean(int cell)
	{
		int elementIndex = ElementLoader.GetElementIndex(this.dirtyEmitElement);
		int elementIdx = (int)Grid.Cell[cell].elementIdx;
		return elementIdx != elementIndex || Grid.Cell[cell].mass < 1f;
	}

	private void ReverseDirection()
	{
		this.Heading.x = this.Heading.x * -1f;
	}

	private void DropDirty(float amount)
	{
		ElementEmitter component = base.GetComponent<ElementEmitter>();
		if (component != null)
		{
			component.ForceEmit(amount, this.emitDiseaseIdx, Mathf.RoundToInt((float)this.emitDiseasePerKg * amount), -1f);
		}
	}

	private Vector2 Heading;

	public SimHashes dirtyEmitElement;

	public float dirtyProbabilityPercent;

	public float dirtyCellToTargetMass;

	public float dirtyMassPerDirty;

	public float dirtyMassReleaseOnDeath;

	public byte emitDiseaseIdx = byte.MaxValue;

	public int emitDiseasePerKg;

	public class StatesInstance : GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.GameInstance
	{
		public StatesInstance(Glom smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Glom.States, Glom.StatesInstance, Glom>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.embedded;
			this.embedded.Update(delegate(Glom.StatesInstance smi)
			{
				if (!Grid.Solid[Grid.PosToCell(smi.transform.position)])
				{
					smi.GoTo(this.fall);
				}
			});
			this.alive.EventTransition(GameHashes.Died, this.death, null).EventTransition(GameHashes.TooColdFatal, this.death, null).EventTransition(GameHashes.TooHotFatal, this.death, null)
				.TagTransition(GameTags.Entombed, this.death, false)
				.TagTransition(GameTags.Dead, this.death, false)
				.ToggleStateMachine((Glom.StatesInstance smi) => new ThreatMonitor.Instance(smi.master))
				.Enter(delegate(Glom.StatesInstance smi)
				{
					this.mover.Set(smi.master.gameObject, smi);
				});
			this.alive.grounded.Update(delegate(Glom.StatesInstance smi)
			{
				int num = Grid.PosToCell(smi.transform.position + Vector3.down);
				if (Grid.IsValidCell(num) && !Grid.Solid[num])
				{
					smi.GoTo(this.fall);
				}
			});
			this.alive.grounded.flee.InitializeStates(this.mover, this.alive.grounded.idling.idle);
			this.alive.grounded.idling.idle.PlayAnim("idle", KAnim.PlayMode.Loop).Enter(delegate(Glom.StatesInstance smi)
			{
				if (smi.master.CellIsClean(Grid.PosToCell(smi.master)) && global::UnityEngine.Random.Range(0f, 100f) < smi.master.dirtyProbabilityPercent)
				{
					smi.ScheduleGoTo(1f, this.alive.grounded.dirty);
				}
				else
				{
					smi.ScheduleGoTo(2f, this.alive.grounded.idling.move);
				}
			}).EventTransition(GameHashes.Attacked, this.alive.grounded.flee, null);
			this.alive.grounded.idling.move.InitializeStates(this.alive.grounded.idling.idle).EventTransition(GameHashes.Attacked, this.alive.grounded.flee, null);
			this.alive.grounded.dirty.Enter(delegate(Glom.StatesInstance smi)
			{
				smi.Play("dirty", KAnim.PlayMode.Once);
				smi.master.DropDirty(smi.master.dirtyMassPerDirty);
				smi.ScheduleGoTo(1f, this.alive.grounded.idling.idle);
			});
			this.death.Enter(delegate(Glom.StatesInstance smi)
			{
				smi.master.DropDirty(smi.master.dirtyMassReleaseOnDeath);
				smi.Play("death", KAnim.PlayMode.Once);
				CreatureHelpers.DeselectCreature(smi.gameObject);
				smi.Schedule(2f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.fall.ToggleGravity(this.alive.grounded.idling.idle);
		}

		public Glom.States.AliveStates alive;

		public GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.State death;

		public GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.State embedded;

		public StateMachine<Glom.States, Glom.StatesInstance, Glom, object>.TargetParameter mover;

		public GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.State fall;

		public class AliveStates : GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.State
		{
			public Glom.States.GroundedState grounded = new Glom.States.GroundedState();
		}

		public class GroundedState : GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.State
		{
			public Glom.States.GroundedState.IdleStates idling;

			public GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.CreatureFleeSubState<Approachable> flee;

			public GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.State dirty;

			public class IdleStates : GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.State
			{
				public GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.State idle;

				public GameStateMachine<Glom.States, Glom.StatesInstance, Glom, object>.IdleMoveSubState move;
			}
		}
	}
}
