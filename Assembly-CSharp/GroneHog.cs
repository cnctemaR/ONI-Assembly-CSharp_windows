using System;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GroneHog : StateMachineComponent<GroneHog.StatesInstance>, ISaveLoadable
{
	public GroneHogMound mMound
	{
		get
		{
			return this.moundRef.Get();
		}
		set
		{
			this.moundRef.Set(value);
		}
	}

	public void SetMound(GroneHogMound mound)
	{
		this.mMound = mound;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.GoTo(base.smi.sm.grounded.idle);
	}

	private void OnEnable()
	{
		this.fleeing = false;
		if (global::UnityEngine.Random.Range(0f, 100f) > 50f)
		{
			this.Heading = Vector2.right;
		}
		else
		{
			this.Heading = Vector2.left;
		}
		base.smi.GoTo(base.smi.sm.grounded.idle_alt);
	}

	public bool AtMound()
	{
		return this.mMound != null && Grid.PosToCell(this.transform.position) == Grid.PosToCell(this.mMound.transform.position);
	}

	private void EnterMound()
	{
		if (SelectTool.Instance.selected != null && SelectTool.Instance.selected.gameObject == base.gameObject)
		{
			SelectTool.Instance.Select(null, false);
		}
		base.gameObject.SetActive(false);
	}

	private Vector3 GetMoveTarget(bool reverseDirectionIfNecessary = true)
	{
		Vector3 walkMoveTarget = CreatureHelpers.GetWalkMoveTarget(this.transform, this.Heading);
		if (reverseDirectionIfNecessary && walkMoveTarget == this.transform.position)
		{
			this.ReverseDirection();
		}
		return walkMoveTarget;
	}

	private void FightOrFlight()
	{
		if (this.mMound != null && this.GetMoveTarget(false) != this.transform.position)
		{
			this.fleeing = true;
			this.SetDirectionToMound();
			base.smi.GoTo(base.smi.sm.grounded.startle);
		}
		else
		{
			base.smi.GoTo(base.smi.sm.grounded.harvest);
		}
	}

	private void SetDirectionToMound()
	{
		if (this.mMound != null)
		{
			if (this.mMound.transform.position.x < this.transform.position.x)
			{
				this.Heading = Vector3.left;
			}
			else if (this.mMound.transform.position.x > this.transform.position.x)
			{
				this.Heading = Vector3.right;
			}
		}
	}

	private void death()
	{
		if (this.mMound != null)
		{
			this.mMound.gameObject.Trigger(1623392196, this);
		}
	}

	private void ReverseDirection()
	{
		this.Heading.x = this.Heading.x * -1f;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.moundRef != null)
		{
			this.moundRef.Get<GroneHogMound>().RestoreHog(base.gameObject);
		}
	}

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpAdd]
	private SimpleMover mover;

	[MyCmpAdd]
	private ElementEmitter emitter;

	[MyCmpAdd]
	private BoxCollider2D mCollider;

	[MyCmpAdd]
	private Harvestable harvestable;

	private Vector2 Heading;

	private float moveSpeed = 0.75f;

	private float fleeMoveSpeed = 0.5f;

	private float jumpDuration = 1f;

	[Serialize]
	private Ref<GroneHogMound> moundRef = new Ref<GroneHogMound>();

	private bool fleeing;

	private Vector3 harvestPosition;

	public class StatesInstance : GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.GameInstance
	{
		public StatesInstance(GroneHog master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded.idle;
			this.grounded.Update(delegate(GroneHog.StatesInstance smi)
			{
				int num = Grid.PosToCell(smi.transform.position + Vector3.down);
				if (Grid.IsValidCell(num) && !Grid.Solid[num])
				{
					smi.GoTo(this.fall);
				}
			}).EventHandler(GameHashes.WorkStarted, delegate(GroneHog.StatesInstance smi)
			{
				smi.master.FightOrFlight();
			}).EventTransition(GameHashes.Harvest, this.grounded.death, null);
			this.grounded.idle.Enter(delegate(GroneHog.StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Loop);
				if (!smi.master.fleeing && global::UnityEngine.Random.Range(0f, 100f) > 80f)
				{
					smi.master.ReverseDirection();
				}
				if (!smi.master.fleeing && smi.master.mMound != null && CreatureHelpers.CrewNearby(smi.transform, 6))
				{
					smi.GoTo(this.grounded.startle);
					return;
				}
				if (smi.master.GetMoveTarget(true) != smi.transform.position)
				{
					if (!smi.master.fleeing)
					{
						smi.ScheduleGoTo(smi.master.moveSpeed, this.grounded.move);
					}
					else if (smi.master.AtMound() && smi.master.mMound.HogEnter())
					{
						smi.master.EnterMound();
					}
					else
					{
						smi.ScheduleGoTo(0f, this.grounded.move_flee);
					}
				}
				else
				{
					smi.ScheduleGoTo(smi.master.moveSpeed, this.grounded.idle_alt);
				}
			});
			this.grounded.idle_alt.Enter(delegate(GroneHog.StatesInstance smi)
			{
				if (!smi.master.fleeing && smi.master.mMound != null && CreatureHelpers.CrewNearby(smi.transform, 6))
				{
					smi.GoTo(this.grounded.startle);
					return;
				}
				smi.ScheduleGoTo(smi.master.moveSpeed, this.grounded.idle);
			});
			this.grounded.move.Enter(delegate(GroneHog.StatesInstance smi)
			{
				if (!smi.master.fleeing && smi.master.mMound != null && CreatureHelpers.CrewNearby(smi.master.transform, 6))
				{
					smi.GoTo(this.grounded.startle);
					return;
				}
				Vector3 moveTarget = smi.master.GetMoveTarget(true);
				if (moveTarget.y != smi.transform.position.y)
				{
					smi.GoTo(this.grounded.jump);
				}
				else if (Mathf.Abs(moveTarget.x - smi.transform.position.x) < 2f)
				{
					smi.Play("walk", KAnim.PlayMode.Loop);
					smi.master.mover.MoveToTarget(smi.master.GetMoveTarget(true), smi.master.moveSpeed);
				}
				else
				{
					smi.GoTo(this.grounded.jump);
				}
				CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
				smi.ScheduleGoTo(smi.master.moveSpeed, this.grounded.idle);
			});
			this.grounded.move_flee.Enter(delegate(GroneHog.StatesInstance smi)
			{
				smi.master.SetDirectionToMound();
				Vector3 moveTarget2 = smi.master.GetMoveTarget(false);
				if (moveTarget2.y != smi.transform.position.y)
				{
					smi.GoTo(this.grounded.jump);
				}
				else if (Mathf.Abs(moveTarget2.x - smi.transform.position.x) < 2f)
				{
					smi.Play("run", KAnim.PlayMode.Once);
					smi.master.mover.MoveToTarget(smi.master.GetMoveTarget(true), smi.master.fleeMoveSpeed);
				}
				else
				{
					smi.GoTo(this.grounded.jump);
				}
				CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
				smi.ScheduleGoTo(smi.master.fleeMoveSpeed, this.grounded.idle);
			});
			this.grounded.jump.Enter(delegate(GroneHog.StatesInstance smi)
			{
				Vector3 moveTarget3 = smi.master.GetMoveTarget(true);
				CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
				if (moveTarget3.y - smi.transform.position.y >= 2f)
				{
					smi.Play("jump_2", KAnim.PlayMode.Once);
				}
				else if (moveTarget3.y - smi.transform.position.y >= 1f)
				{
					smi.Play("jump", KAnim.PlayMode.Once);
				}
				else if (moveTarget3.y - smi.transform.position.y < 0f)
				{
					smi.Play("jump_down", KAnim.PlayMode.Once);
				}
				else
				{
					smi.Play("jump_cross", KAnim.PlayMode.Once);
				}
				smi.ScheduleGoTo(smi.master.jumpDuration, this.grounded.idle);
			}).Exit(delegate(GroneHog.StatesInstance smi)
			{
				Vector3 moveTarget4 = smi.master.GetMoveTarget(true);
				smi.master.mover.TeleportToTarget(moveTarget4, 0f);
			});
			this.grounded.startle.Enter(delegate(GroneHog.StatesInstance smi)
			{
				smi.master.fleeing = true;
				smi.master.mover.StopMovement();
				smi.master.harvestable.ForceCancelHarvest(null);
				smi.Play("startle", KAnim.PlayMode.Once);
				smi.ScheduleGoTo(smi.master.moveSpeed, this.grounded.idle);
			});
			this.grounded.harvest.Enter(delegate(GroneHog.StatesInstance smi)
			{
				smi.master.mover.StopMovement();
				smi.master.harvestPosition = smi.transform.position;
				smi.Play("harvest", KAnim.PlayMode.Loop);
			}).EventTransition(GameHashes.WorkAborted, this.grounded.idle, null);
			this.grounded.harvest.Update(delegate(GroneHog.StatesInstance smi)
			{
				smi.transform.SetPosition(smi.master.harvestPosition);
			});
			this.grounded.death.Enter(delegate(GroneHog.StatesInstance smi)
			{
				smi.Play("death", KAnim.PlayMode.Once);
				smi.master.death();
			}).EventHandler(GameHashes.Butcher, delegate(GroneHog.StatesInstance smi)
			{
				smi.Schedule(0.1f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.fall.ToggleGravity(this.grounded.idle);
		}

		public GroneHog.States.GroundedState grounded = new GroneHog.States.GroundedState();

		public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State fall = new GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State();

		public class GroundedState : GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State
		{
			public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State idle;

			public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State idle_alt;

			public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State move;

			public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State move_flee;

			public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State startle;

			public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State jump;

			public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State harvest;

			public GameStateMachine<GroneHog.States, GroneHog.StatesInstance, GroneHog, object>.State death;
		}
	}
}
