using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Flut : StateMachineComponent<Flut.StatesInstance>, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = base.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.butcherable.SetReadyToButcher(false);
		if (CreatureHelpers.isSwimmable(Grid.PosToCell(base.gameObject)))
		{
			this.butcherable.SetReadyToButcher(false);
		}
		else
		{
			this.butcherable.SetReadyToButcher(true);
		}
		this.GetBodyOfWater();
	}

	private void MarkCell(bool mark)
	{
		if (mark)
		{
			if (Grid.IsValidCell(this.markedCell) && Grid.Objects[this.markedCell, 5] == base.gameObject)
			{
				Grid.Objects[this.markedCell, 5] = null;
			}
			this.markedCell = Grid.PosToCell(base.gameObject);
			Grid.Objects[this.markedCell, 5] = base.gameObject;
		}
		else if (Grid.Objects[this.markedCell, 5] == base.gameObject)
		{
			Grid.Objects[this.markedCell, 5] = null;
		}
	}

	private BodyOfWater GetBodyOfWater()
	{
		int num = Grid.PosToCell(base.gameObject);
		BodyOfWater bodyOfWater = null;
		if (Grid.IsSubstantialLiquid(num, 0.6f))
		{
			bodyOfWater = WaterBodyProbe.Instance.GetBodyIfKnown(Grid.PosToCell(base.gameObject));
			if (bodyOfWater == null)
			{
				bodyOfWater = WaterBodyProbe.Instance.FindBodyOfWater(base.gameObject);
			}
		}
		if (bodyOfWater == null)
		{
			return null;
		}
		bodyOfWater.AddObjectToBody(base.gameObject);
		return bodyOfWater;
	}

	private FishingLure LookForLure()
	{
		BodyOfWater bodyOfWater = this.GetBodyOfWater();
		if (bodyOfWater == null)
		{
			return null;
		}
		List<FishingLure> list = new List<FishingLure>();
		foreach (GameObject gameObject in bodyOfWater.containedObjects)
		{
			if (gameObject != null)
			{
				FishingLure component = gameObject.GetComponent<FishingLure>();
				if (component != null && component.isBeingWorked())
				{
					list.Add(component);
				}
			}
		}
		float num = (float)(this.MaxAttractionDistance + 1);
		FishingLure fishingLure = null;
		foreach (FishingLure fishingLure2 in list)
		{
			float num2 = Vector3.Distance(fishingLure2.transform.position, base.transform.position);
			if (fishingLure2.HookedObject == null && num2 < num)
			{
				num = num2;
				fishingLure = fishingLure2;
			}
		}
		if (fishingLure != null)
		{
			return fishingLure;
		}
		return null;
	}

	private void CleanUp()
	{
		if (SelectTool.Instance.selected != null && SelectTool.Instance.selected == base.GetComponent<KSelectable>())
		{
			SelectTool.Instance.Select(null, false);
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	[MyCmpAdd]
	private Navigator nav;

	[MyCmpAdd]
	private Butcherable butcherable;

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpReq]
	private Operational operational;

	private int MaxAttractionDistance = 30;

	private float LandSuffocateTime = 150f;

	private Vector3 caughtLocation;

	private int markedCell = -1;

	public class StatesInstance : GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.GameInstance
	{
		public StatesInstance(Flut smi)
			: base(smi)
		{
		}

		public bool instanceIsMoving;
	}

	public class States : GameStateMachine<Flut.States, Flut.StatesInstance, Flut>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.swimming.peacefully.idling.idle;
			base.serializable = true;
			this.alive.EventTransition(GameHashes.TooHotFatal, this.dead.idle, (Flut.StatesInstance smi) => smi.timeinstate > 0f).EventTransition(GameHashes.TooColdFatal, this.dead.idle, (Flut.StatesInstance smi) => smi.timeinstate > 0f).EventTransition(GameHashes.Died, this.dead.idle, null)
				.EventTransition(GameHashes.DebugGoTo, (Flut.StatesInstance smi) => Game.Instance, this.alive.swimming.peacefully.idling.debug_go_to, null)
				.ToggleStateMachine((Flut.StatesInstance smi) => new ThreatMonitor.Instance(smi.master))
				.Enter(delegate(Flut.StatesInstance smi)
				{
					this.mover.Set(smi.master.gameObject, smi);
					smi.master.operational.SetActive(true, false);
				})
				.Exit(delegate(Flut.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
			this.alive.falling.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Falling).ToggleGravity(this.alive.grounded).Enter(delegate(Flut.StatesInstance smi)
			{
				smi.Play("hook_loop", KAnim.PlayMode.Loop);
			})
				.Update(delegate(Flut.StatesInstance smi)
				{
					if (CreatureHelpers.isSwimmable(Grid.PosToCell(smi.master.gameObject)))
					{
						BodyOfWater.MakeSplash(smi.master.transform.position);
						smi.GoTo(this.alive.swimming.peacefully.idling.idle);
					}
					else if (Grid.Solid[Grid.CellBelow(Grid.PosToCell(smi.master.gameObject))])
					{
						smi.GoTo(this.alive.grounded);
					}
				})
				.Exit(delegate(Flut.StatesInstance smi)
				{
					Vector3 vector = CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(smi.master.gameObject)) + Vector3.down / 2f;
					smi.master.transform.SetPosition(vector);
				})
				.EventTransition(GameHashes.Landed, this.alive.grounded, null);
			this.alive.grounded.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Struggling).Enter(delegate(Flut.StatesInstance smi)
			{
				smi.master.MarkCell(true);
				smi.Play("flop_loop", KAnim.PlayMode.Loop);
				smi.master.butcherable.SetReadyToButcher(true);
				smi.ScheduleGoTo(smi.master.LandSuffocateTime, this.dead.idle);
			}).Update(delegate(Flut.StatesInstance smi)
			{
				int num = Grid.PosToCell(smi.master.gameObject);
				if (CreatureHelpers.isSwimmable(num))
				{
					smi.GoTo(this.alive.swimming.peacefully.idling.idle);
				}
				if (!Grid.Solid[Grid.CellBelow(num)])
				{
					smi.GoTo(this.alive.falling);
				}
			})
				.EventHandler(GameHashes.Butcher, delegate(Flut.StatesInstance smi)
				{
					smi.Schedule(0.1f, delegate(object d)
					{
						Util.KDestroyGameObject(smi.gameObject);
					}, null);
				})
				.Update(delegate(Flut.StatesInstance smi)
				{
					int num2 = Grid.PosToCell(smi.gameObject);
					if (CreatureHelpers.isSwimmable(Grid.CellDownLeft(num2)))
					{
					}
					if (CreatureHelpers.isSwimmable(Grid.CellDownRight(num2)))
					{
					}
					if (CreatureHelpers.isSwimmable(Grid.CellRight(num2)))
					{
					}
					if (CreatureHelpers.isSwimmable(Grid.CellLeft(num2)))
					{
					}
				})
				.EventTransition(GameHashes.TooHotFatal, this.dead.idle, null)
				.EventTransition(GameHashes.TooColdFatal, this.dead.idle, null)
				.Exit(delegate(Flut.StatesInstance smi)
				{
					smi.master.butcherable.SetReadyToButcher(false);
				});
			this.alive.swimming.Enter(delegate(Flut.StatesInstance smi)
			{
				smi.master.MarkCell(true);
				smi.master.GetBodyOfWater();
				smi.master.butcherable.SetReadyToButcher(false);
			}).Update(delegate(Flut.StatesInstance smi)
			{
				int num3 = Grid.PosToCell(smi.master.gameObject);
				int num4 = Grid.CellBelow(num3);
				if (!Grid.Solid[num4] && !Grid.IsSubstantialLiquid(num3, 0.35f) && !Grid.Solid[num3])
				{
					smi.GoTo(this.alive.falling);
				}
			}).EventTransition(GameHashes.Harvest, this.alive.hooked, null)
				.EventTransition(GameHashes.CreatureReproduce, this.alive.swimming.peacefully.lay, null);
			this.alive.swimming.peacefully.EventTransition(GameHashes.Attacked, this.alive.swimming.flee, null);
			this.alive.swimming.flee.InitializeStates(this.mover, this.alive.swimming.peacefully.idling.idle);
			this.alive.swimming.peacefully.idling.debug_go_to.InitializeStates(this.alive.swimming.peacefully.idling.idle);
			this.alive.swimming.peacefully.idling.idle.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Idle).Enter(delegate(Flut.StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Loop);
				smi.Schedule(3f, delegate(object d)
				{
					FishingLure fishingLure = smi.master.LookForLure();
					if (global::UnityEngine.Random.Range(0f, 100f) > 50f && fishingLure != null)
					{
						this.lureMoveTarget.Set(fishingLure.gameObject, smi);
						smi.GoTo(this.alive.swimming.peacefully.moveToLure);
					}
					else
					{
						smi.GoTo(this.alive.swimming.peacefully.idling.move);
					}
				}, null);
			}).Update(delegate(Flut.StatesInstance smi)
			{
				int num5 = Grid.PosToCell(smi.master.gameObject);
				int num6 = Grid.CellBelow(num5);
				if (!CreatureHelpers.isSwimmable(num5) && Grid.Solid[num6])
				{
					smi.GoTo(this.alive.grounded);
				}
			});
			this.alive.swimming.peacefully.idling.move.InitializeStates(this.alive.swimming.peacefully.idling.idle);
			this.alive.swimming.peacefully.moveToLure.InitializeStates(this.mover, this.lureMoveTarget, this.alive.swimming.peacefully.idling.idle, this.alive.swimming.peacefully.idling.idle, null, null).Enter(delegate(Flut.StatesInstance smi)
			{
				smi.Schedule(2f, delegate(object d)
				{
					smi.master.nav.Stop(false);
					smi.GoTo(this.alive.swimming.peacefully.idling.idle);
				}, null);
			});
			this.alive.swimming.peacefully.lay.ToggleMainStatusItem(Db.Get().CreatureStatusItems.LayingAnEgg).Enter(delegate(Flut.StatesInstance smi)
			{
				smi.animController.Queue("lay", KAnim.PlayMode.Once, 1f, 0f);
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(smi.master.GetComponent<AquaticReproducer>().EggPrefabTag), smi.master.transform.position, Quaternion.identity, null, null, true, 0);
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, gameObject.GetComponent<KPrefabID>().GetProperName(), gameObject.transform, 1.5f, false);
				gameObject.SetActive(true);
			}).OnAnimQueueComplete(this.alive.swimming.peacefully.idling.idle);
			this.alive.hooked.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Struggling).Enter(delegate(Flut.StatesInstance smi)
			{
				if (smi.GetComponent<KSelectable>().HasStatusItem(Db.Get().CreatureStatusItems.ConsideringLure))
				{
					smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().CreatureStatusItems.ConsideringLure, false);
				}
				smi.Play("caught_loop", KAnim.PlayMode.Loop);
				smi.master.nav.Stop(false);
			}).EventHandler(GameHashes.Harvest, delegate(Flut.StatesInstance smi)
			{
				smi.master.nav.Stop(false);
				smi.GoTo(this.alive.falling);
			});
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead);
			this.dead.idle.Enter(delegate(Flut.StatesInstance smi)
			{
				smi.master.butcherable.SetReadyToButcher(true);
				smi.Play("death", KAnim.PlayMode.Once);
				smi.master.MarkCell(true);
			}).Update(delegate(Flut.StatesInstance smi)
			{
				int num7 = Grid.PosToCell(smi.master);
				if (!Grid.Solid[Grid.CellBelow(num7)])
				{
					smi.GoTo(this.dead.falling);
				}
			}).EventHandler(GameHashes.Butcher, delegate(Flut.StatesInstance smi)
			{
				smi.Schedule(0.1f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.dead.falling.ToggleGravity(this.dead.idle).PlayAnim("death", KAnim.PlayMode.Loop);
		}

		public StateMachine<Flut.States, Flut.StatesInstance, Flut, object>.TargetParameter lureMoveTarget;

		public StateMachine<Flut.States, Flut.StatesInstance, Flut, object>.TargetParameter mover;

		public Flut.States.AliveStates alive;

		public Flut.States.DeadStates dead;

		public class AliveStates : GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State
		{
			public Flut.States.SwimmingState swimming;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State falling;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State grounded;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State hooked;
		}

		public class SwimmingState : GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State
		{
			public Flut.States.Peacefull peacefully;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.CreatureFleeSubState<IApproachable> flee;
		}

		public class Peacefull : GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State
		{
			public Flut.States.IdleStates idling;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.ApproachSubState<IApproachable> moveToLure;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State lay;
		}

		public class IdleStates : GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State
		{
			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State idle;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.IdleMoveSubState move;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.DebugGoToSubState debug_go_to;
		}

		public class DeadStates : GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State
		{
			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State idle;

			public GameStateMachine<Flut.States, Flut.StatesInstance, Flut, object>.State falling;
		}
	}
}
