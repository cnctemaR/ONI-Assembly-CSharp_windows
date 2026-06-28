using System;
using STRINGS;
using UnityEngine;

public class HatchDigger : StateMachineComponent<HatchDigger.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Heading = Vector2.right;
		base.smi.StartSM();
	}

	private Vector3 GetMoveTarget()
	{
		this.aimForEdibles();
		Vector3 walkMoveTarget = CreatureHelpers.GetWalkMoveTarget(this.transform, this.Heading);
		if (walkMoveTarget == this.transform.position)
		{
			this.ReverseDirection();
		}
		return walkMoveTarget;
	}

	private bool aimForEdibles()
	{
		int num = 16;
		int num2 = 2;
		int num3 = Grid.PosToCell(base.gameObject);
		int num4 = Grid.PosToCell(this.transform.position);
		for (int i = num2; i >= 0; i--)
		{
			for (int j = num; j > 0; j--)
			{
				int num5 = Grid.OffsetCell(num3, j, i);
				if (this.EdibleOnCell(num5) && CreatureHelpers.CheckHorizontalClear(Grid.CellToPos(num3), Grid.CellToPos(num5)))
				{
					num4 = num5;
				}
				int num6 = Grid.OffsetCell(num3, -j, i);
				if (this.EdibleOnCell(num6) && CreatureHelpers.CheckHorizontalClear(Grid.CellToPos(num3), Grid.CellToPos(num6)))
				{
					num4 = num6;
				}
			}
		}
		if (num4 != num3)
		{
			Vector3 vector = Grid.CellToPos(num4);
			if (vector.x > this.transform.position.x && this.Heading.x < 0f)
			{
				this.ReverseDirection();
			}
			if (vector.x < this.transform.position.x && this.Heading.x > 0f)
			{
				this.ReverseDirection();
			}
			return true;
		}
		return false;
	}

	private GameObject EdibleOnCell(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 3];
		if (gameObject != null && (gameObject.HasTag(GameTags.Ore) || gameObject.HasTag(GameTags.Edible) || gameObject.HasTag(GameTags.BuildableRaw) || gameObject.HasTag(GameTags.Solid)))
		{
			return gameObject;
		}
		return null;
	}

	private void ReverseDirection()
	{
		this.Heading.x = this.Heading.x * -1f;
	}

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpAdd]
	private SimpleMover mover;

	[MyCmpAdd]
	private BoxCollider2D mCollider;

	[MyCmpAdd]
	private Harvestable harvestable;

	private Vector2 Heading;

	public float moveSpeed = 0.75f;

	public bool alive = true;

	public float MassPerFeeding = 100f;

	public int RationsPerFeeding = 3;

	public float eatTime = 5f;

	private Vector3 harvestPosition;

	public class StatesInstance : GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.GameInstance
	{
		public StatesInstance(HatchDigger smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded.idle;
			base.serializable = true;
			this.grounded.Update(delegate(HatchDigger.StatesInstance smi)
			{
				int num = Grid.PosToCell(smi.transform.position + Vector3.down);
				if (Grid.IsValidCell(num) && !Grid.Solid[num])
				{
					smi.GoTo(this.fall);
				}
			}).EventTransition(GameHashes.WorkStarted, this.grounded.harvest, null).EventTransition(GameHashes.TooHotFatal, this.grounded.death, (HatchDigger.StatesInstance smi) => smi.master.alive && smi.timeinstate > 0f)
				.EventTransition(GameHashes.TooColdFatal, this.grounded.death, (HatchDigger.StatesInstance smi) => smi.master.alive && smi.timeinstate > 0f);
			this.grounded.idle.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				if (!smi.master.aimForEdibles() && global::UnityEngine.Random.Range(0f, 100f) > 90f)
				{
					smi.master.ReverseDirection();
				}
				if (global::UnityEngine.Random.Range(0, 100) > 50)
				{
					smi.GoTo(this.grounded.dig);
				}
				else if (smi.master.GetMoveTarget() != smi.transform.position)
				{
					smi.Queue("walk_pre", KAnim.PlayMode.Once);
					smi.GoTo(this.grounded.move);
				}
				else
				{
					smi.GoTo(this.grounded.idle_alt);
				}
			});
			this.grounded.dig.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				smi.ScheduleGoTo(0.75f, this.grounded.idle);
				int num2 = Grid.PosToCell(smi.gameObject);
				bool isSolid = ElementLoader.elements[(int)Grid.Cell[Grid.CellBelow(num2)].elementIdx].IsSolid;
				bool isSolid2 = ElementLoader.elements[(int)Grid.Cell[Grid.CellLeft(num2)].elementIdx].IsSolid;
				bool isSolid3 = ElementLoader.elements[(int)Grid.Cell[Grid.CellRight(num2)].elementIdx].IsSolid;
				if (isSolid && global::UnityEngine.Random.Range(0, 100) > 80)
				{
					smi.Play("hide", KAnim.PlayMode.Once);
					WorldDamage.Instance.ApplyDamage(Grid.CellBelow(Grid.PosToCell(smi.gameObject)), 1000f, -1, -1);
				}
				else if ((isSolid2 && isSolid3 && global::UnityEngine.Random.Range(0, 100) > 50) || (isSolid3 && !isSolid2))
				{
					smi.master.Heading = Vector2.right;
					CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
					smi.Play("eat_pre", KAnim.PlayMode.Once);
					WorldDamage.Instance.ApplyDamage(Grid.CellRight(Grid.PosToCell(smi.gameObject)), 1000f, -1, -1);
				}
				else if (isSolid2)
				{
					smi.master.Heading = Vector2.left;
					CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
					smi.Play("eat_pre", KAnim.PlayMode.Once);
					WorldDamage.Instance.ApplyDamage(Grid.CellLeft(Grid.PosToCell(smi.gameObject)), 1000f, -1, -1);
				}
			});
			this.grounded.idle_alt.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				smi.Queue("idle_loop", KAnim.PlayMode.Once);
			}).EventTransition(GameHashes.AnimQueueComplete, this.grounded.idle, (HatchDigger.StatesInstance smi) => smi.timeinstate > 0f);
			this.grounded.hide.pre.PlayAnim("hide", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.grounded.hide.loop);
			this.grounded.hide.loop.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				smi.master.mCollider.enabled = false;
				smi.master.harvestable.ForceCancelHarvest(null);
				smi.GetComponent<KSelectable>().Unselect();
			}).Update(delegate(HatchDigger.StatesInstance smi)
			{
				if (GameClock.Instance.GetCurrentDayAsPercentage() >= 0.875f && !Grid.Solid[Grid.PosToCell(smi.gameObject)])
				{
					smi.GoTo(this.grounded.emerge);
				}
			}).Exit(delegate(HatchDigger.StatesInstance smi)
			{
				smi.master.mCollider.enabled = true;
			});
			this.grounded.eat_pre.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				smi.Queue("eat_pre", KAnim.PlayMode.Once);
				smi.Queue("eat_loop", KAnim.PlayMode.Loop);
				smi.ScheduleGoTo(0.5f, this.grounded.eat);
			});
			this.grounded.eat.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				smi.ScheduleGoTo(smi.master.eatTime, this.grounded.eat_pst);
				GameObject gameObject = smi.master.EdibleOnCell(Grid.PosToCell(smi.master));
				if (gameObject != null)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, MISC.POPFX.RESOURCE_EATEN, smi.transform, 1.5f, false);
					Util.KDestroyGameObject(gameObject);
				}
			});
			this.grounded.eat_pst.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				smi.Queue("eat_pst", KAnim.PlayMode.Once);
			}).EventTransition(GameHashes.AnimQueueComplete, this.grounded.idle_alt, (HatchDigger.StatesInstance smi) => smi.timeinstate > 0f);
			this.grounded.move.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				Vector3 moveTarget = smi.master.GetMoveTarget();
				if (moveTarget.y != smi.transform.position.y)
				{
					smi.GoTo(this.grounded.jump);
				}
				else if (Mathf.Abs(moveTarget.x - smi.transform.position.x) < 2f)
				{
					smi.Queue("walk_loop", KAnim.PlayMode.Loop);
					smi.master.mover.MoveToTarget(smi.master.GetMoveTarget(), smi.master.moveSpeed);
				}
				else
				{
					smi.GoTo(this.grounded.jump);
				}
				CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
			}).EventTransition(GameHashes.CreatureMoveComplete, this.grounded.move_pst, (HatchDigger.StatesInstance smi) => smi.timeinstate > 0f);
			this.grounded.move_pst.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				if (global::UnityEngine.Random.Range(0f, 100f) > 85f)
				{
					smi.master.ReverseDirection();
				}
				if (GameClock.Instance.GetCurrentDayAsPercentage() < 0.875f)
				{
					smi.Queue("walk_pst", KAnim.PlayMode.Once);
					smi.GoTo(this.grounded.idle_alt);
				}
				else if (smi.master.EdibleOnCell(Grid.PosToCell(smi.transform.position)) != null && global::UnityEngine.Random.Range(0f, 100f) > 50f)
				{
					smi.Queue("walk_pst", KAnim.PlayMode.Once);
					smi.GoTo(this.grounded.eat_pre);
				}
				else if (smi.master.GetMoveTarget() != smi.transform.position)
				{
					smi.GoTo(this.grounded.move);
				}
				else
				{
					smi.Queue("walk_pst", KAnim.PlayMode.Once);
					smi.GoTo(this.grounded.idle_alt);
				}
			});
			this.grounded.jump.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				Vector3 moveTarget2 = smi.master.GetMoveTarget();
				CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
				if (moveTarget2.y - smi.transform.position.y >= 2f)
				{
					smi.Play("jump_up_dbl", KAnim.PlayMode.Once);
				}
				else if (moveTarget2.y - smi.transform.position.y >= 1f)
				{
					smi.Play("jump_up", KAnim.PlayMode.Once);
				}
				else if (moveTarget2.y - smi.transform.position.y < -1f)
				{
					smi.Play("jump_dn_dbl", KAnim.PlayMode.Once);
				}
				else if (moveTarget2.y - smi.transform.position.y < 0f)
				{
					smi.Play("jump_dn", KAnim.PlayMode.Once);
				}
				else
				{
					smi.Play("jump_over", KAnim.PlayMode.Once);
				}
			}).EventTransition(GameHashes.AnimQueueComplete, this.grounded.idle, null).Exit(delegate(HatchDigger.StatesInstance smi)
			{
				Vector3 moveTarget3 = smi.master.GetMoveTarget();
				smi.master.mover.TeleportToTarget(moveTarget3, 0f);
			});
			this.grounded.harvest.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				smi.master.mover.StopMovement();
				smi.master.harvestPosition = smi.transform.position;
				smi.Queue("harvest", KAnim.PlayMode.Loop);
			}).EventTransition(GameHashes.Harvest, this.grounded.death, null).EventTransition(GameHashes.WorkAborted, this.grounded.idle, null);
			this.grounded.harvest.Update(delegate(HatchDigger.StatesInstance smi)
			{
				smi.transform.SetPosition(smi.master.harvestPosition);
			}).EventTransition(GameHashes.NewDay, this.grounded.emerge, null);
			this.grounded.emerge.QueueAnim("emerge", false, null).OnAnimQueueComplete(this.grounded.idle);
			this.grounded.death.Enter(delegate(HatchDigger.StatesInstance smi)
			{
				smi.master.alive = false;
				smi.Queue("death", KAnim.PlayMode.Once);
				smi.Schedule(2f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.fall.ToggleGravity(this.grounded.idle);
		}

		public HatchDigger.States.GroundedState grounded;

		public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State fall;

		public class GroundedState : GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State
		{
			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State idle;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State idle_alt;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State dig;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State move;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State move_pst;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State jump;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State eat_pre;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State eat;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State eat_pst;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State harvest;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State death;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.PLPState hide;

			public GameStateMachine<HatchDigger.States, HatchDigger.StatesInstance, HatchDigger, object>.State emerge;
		}
	}
}
