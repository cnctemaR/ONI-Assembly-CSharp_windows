using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;

public class DrinkMilkStates : GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>
{
	private static void SetSceneLayer(DrinkMilkStates.Instance smi, Grid.SceneLayer layer)
	{
		SegmentedCreature.Instance smi2 = smi.GetSMI<SegmentedCreature.Instance>();
		if (smi2 != null && smi2.segments != null)
		{
			using (IEnumerator<SegmentedCreature.CreatureSegment> enumerator = smi2.segments.Reverse<SegmentedCreature.CreatureSegment>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SegmentedCreature.CreatureSegment creatureSegment = enumerator.Current;
					creatureSegment.animController.SetSceneLayer(layer);
				}
				return;
			}
		}
		smi.GetComponent<KBatchedAnimController>().SetSceneLayer(layer);
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.goingToDrink;
		this.root.Enter(new StateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State.Callback(DrinkMilkStates.SetTarget)).Enter(new StateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State.Callback(DrinkMilkStates.CheckIfCramped)).Enter(new StateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State.Callback(DrinkMilkStates.ReserveMilkFeeder))
			.Exit(new StateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State.Callback(DrinkMilkStates.UnreserveMilkFeeder))
			.Transition(this.behaviourComplete, delegate(DrinkMilkStates.Instance smi)
			{
				MilkFeeder.Instance instance = DrinkMilkStates.GetTargetMilkFeeder(smi);
				if (instance.IsNullOrDestroyed() || !MilkFeeder.ShouldBeOn(instance))
				{
					smi.GetComponent<KAnimControllerBase>().Queue("idle_loop", KAnim.PlayMode.Loop, 1f, 0f);
					return true;
				}
				return false;
			}, UpdateRate.SIM_200ms);
		GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State state = this.goingToDrink.Target(this.targetMilkFeeder).EventHandlerTransition(GameHashes.BuildingStrawChange, null, (DrinkMilkStates.Instance smi, object obj) => true).Target(this.masterTarget)
			.MoveTo(new Func<DrinkMilkStates.Instance, int>(DrinkMilkStates.GetCellToDrinkFrom), this.drink, null, false);
		string text = CREATURES.STATUSITEMS.LOOKINGFORMILK.NAME;
		string text2 = CREATURES.STATUSITEMS.LOOKINGFORMILK.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, statusItemCategory);
		GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State state2 = this.drink.DefaultState(this.drink.pre).Enter("FaceMilkFeeder", new StateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State.Callback(DrinkMilkStates.FaceMilkFeeder));
		string text4 = CREATURES.STATUSITEMS.DRINKINGMILK.NAME;
		string text5 = CREATURES.STATUSITEMS.DRINKINGMILK.TOOLTIP;
		string text6 = "";
		StatusItem.IconType iconType2 = StatusItem.IconType.Info;
		NotificationType notificationType2 = NotificationType.Neutral;
		bool flag2 = false;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text4, text5, text6, iconType2, notificationType2, flag2, default(HashedString), 129022, null, null, statusItemCategory).Enter(delegate(DrinkMilkStates.Instance smi)
		{
			DrinkMilkStates.SetSceneLayer(smi, smi.def.shouldBeBehindMilkTank ? Grid.SceneLayer.BuildingUse : Grid.SceneLayer.Creatures);
		}).Exit(delegate(DrinkMilkStates.Instance smi)
		{
			DrinkMilkStates.SetSceneLayer(smi, Grid.SceneLayer.Creatures);
		});
		this.drink.pre.Target(this.targetMilkFeeder).EventHandlerTransition(GameHashes.BuildingStrawChange, null, (DrinkMilkStates.Instance smi, object obj) => true).Target(this.masterTarget)
			.QueueAnim(new Func<DrinkMilkStates.Instance, string>(DrinkMilkStates.GetAnimDrinkPre), false, null)
			.OnAnimQueueComplete(this.drink.loop);
		this.drink.loop.Target(this.targetMilkFeeder).EventHandlerTransition(GameHashes.BuildingStrawChange, null, (DrinkMilkStates.Instance smi, object obj) => true).Target(this.masterTarget)
			.QueueAnim(new Func<DrinkMilkStates.Instance, string>(DrinkMilkStates.GetAnimDrinkLoop), true, null)
			.Enter(delegate(DrinkMilkStates.Instance smi)
			{
				MilkFeeder.Instance instance2 = DrinkMilkStates.GetTargetMilkFeeder(smi);
				if (instance2 != null)
				{
					instance2.RequestToStartFeeding(smi);
					return;
				}
				smi.GoTo(this.drink.pst);
			})
			.OnSignal(this.requestedToStopFeeding, this.drink.pst);
		this.drink.pst.Enter(new StateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State.Callback(DrinkMilkStates.DrinkMilkComplete)).QueueAnim(new Func<DrinkMilkStates.Instance, string>(DrinkMilkStates.GetAnimDrinkPst), false, null).OnAnimQueueComplete(this.behaviourComplete);
		this.behaviourComplete.QueueAnim("idle_loop", true, null).BehaviourComplete(GameTags.Creatures.Behaviour_TryToDrinkMilkFromFeeder, false);
	}

	private static MilkFeeder.Instance GetTargetMilkFeeder(DrinkMilkStates.Instance smi)
	{
		if (smi.sm.targetMilkFeeder.IsNullOrDestroyed())
		{
			return null;
		}
		GameObject gameObject = smi.sm.targetMilkFeeder.Get(smi);
		if (gameObject.IsNullOrDestroyed())
		{
			return null;
		}
		MilkFeeder.Instance smi2 = gameObject.GetSMI<MilkFeeder.Instance>();
		if (gameObject.IsNullOrDestroyed() || smi2.IsNullOrStopped())
		{
			return null;
		}
		return smi2;
	}

	private static void SetTarget(DrinkMilkStates.Instance smi)
	{
		smi.sm.targetMilkFeeder.Set(smi.GetSMI<DrinkMilkMonitor.Instance>().targetMilkFeeder.gameObject, smi, false);
	}

	private static void CheckIfCramped(DrinkMilkStates.Instance smi)
	{
		smi.critterIsCramped = smi.GetSMI<DrinkMilkMonitor.Instance>().doesTargetMilkFeederHaveSpaceForCritter;
	}

	private static void ReserveMilkFeeder(DrinkMilkStates.Instance smi)
	{
		MilkFeeder.Instance instance = DrinkMilkStates.GetTargetMilkFeeder(smi);
		if (instance == null)
		{
			return;
		}
		instance.SetReserved(true);
	}

	private static void UnreserveMilkFeeder(DrinkMilkStates.Instance smi)
	{
		MilkFeeder.Instance instance = DrinkMilkStates.GetTargetMilkFeeder(smi);
		if (instance == null)
		{
			return;
		}
		instance.SetReserved(false);
	}

	private static void DrinkMilkComplete(DrinkMilkStates.Instance smi)
	{
		MilkFeeder.Instance instance = DrinkMilkStates.GetTargetMilkFeeder(smi);
		if (instance == null)
		{
			return;
		}
		smi.GetSMI<DrinkMilkMonitor.Instance>().NotifyFinishedDrinkingMilkFrom(instance);
	}

	private static int GetCellToDrinkFrom(DrinkMilkStates.Instance smi)
	{
		MilkFeeder.Instance instance = DrinkMilkStates.GetTargetMilkFeeder(smi);
		if (instance == null)
		{
			return Grid.InvalidCell;
		}
		return smi.GetSMI<DrinkMilkMonitor.Instance>().GetDrinkCellOf(instance, smi.critterIsCramped);
	}

	private static string GetAnimDrinkPre(DrinkMilkStates.Instance smi)
	{
		if (smi.critterIsCramped)
		{
			return "drink_cramped_pre";
		}
		return "drink_pre";
	}

	private static string GetAnimDrinkLoop(DrinkMilkStates.Instance smi)
	{
		if (smi.critterIsCramped)
		{
			return "drink_cramped_loop";
		}
		return "drink_loop";
	}

	private static string GetAnimDrinkPst(DrinkMilkStates.Instance smi)
	{
		if (smi.critterIsCramped)
		{
			return "drink_cramped_pst";
		}
		return "drink_pst";
	}

	private static void FaceMilkFeeder(DrinkMilkStates.Instance smi)
	{
		MilkFeeder.Instance instance = DrinkMilkStates.GetTargetMilkFeeder(smi);
		if (instance == null)
		{
			return;
		}
		bool isRotated = instance.GetComponent<Rotatable>().IsRotated;
		float num;
		if (smi.critterIsCramped)
		{
			if (isRotated)
			{
				num = -20f;
			}
			else
			{
				num = 20f;
			}
		}
		else if (isRotated)
		{
			num = 20f;
		}
		else
		{
			num = -20f;
		}
		IApproachable approachable = smi.sm.targetMilkFeeder.Get<IApproachable>(smi);
		if (approachable == null)
		{
			return;
		}
		float num2 = approachable.transform.GetPosition().x + num;
		smi.GetComponent<Facing>().Face(num2);
	}

	public GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State goingToDrink;

	public DrinkMilkStates.EatingState drink;

	public GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State behaviourComplete;

	public StateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.TargetParameter targetMilkFeeder;

	public StateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.Signal requestedToStopFeeding;

	public class Def : StateMachine.BaseDef
	{
		public static CellOffset DrinkCellOffsetGet_CritterOneByOne(MilkFeeder.Instance milkFeederInstance, DrinkMilkMonitor.Instance critterInstance, bool isCramped)
		{
			return milkFeederInstance.GetComponent<Rotatable>().GetRotatedCellOffset(milkFeederInstance.GetDrinkCellOffset());
		}

		public static CellOffset DrinkCellOffsetGet_GassyMoo(MilkFeeder.Instance milkFeederInstance, DrinkMilkMonitor.Instance critterInstance, bool isCramped)
		{
			Rotatable component = milkFeederInstance.GetComponent<Rotatable>();
			CellOffset rotatedCellOffset = component.GetRotatedCellOffset(milkFeederInstance.GetDrinkCellOffset());
			if (component.IsRotated)
			{
				rotatedCellOffset.x--;
			}
			if (isCramped)
			{
				if (component.IsRotated)
				{
					rotatedCellOffset.x += 2;
				}
				else
				{
					rotatedCellOffset.x -= 2;
				}
			}
			return rotatedCellOffset;
		}

		public static CellOffset DrinkCellOffsetGet_TwoByTwo(MilkFeeder.Instance milkFeederInstance, DrinkMilkMonitor.Instance critterInstance, bool isCramped)
		{
			Rotatable component = milkFeederInstance.GetComponent<Rotatable>();
			CellOffset rotatedCellOffset = component.GetRotatedCellOffset(milkFeederInstance.GetDrinkCellOffset());
			if (!isCramped)
			{
				int x = Grid.CellToXY(Grid.OffsetCell(Grid.PosToCell(milkFeederInstance), rotatedCellOffset)).x;
				int x2 = Grid.PosToXY(critterInstance.transform.position).x;
				if (x > x2 && !component.IsRotated)
				{
					rotatedCellOffset.x++;
				}
				else if (x < x2 && component.IsRotated)
				{
					rotatedCellOffset.x--;
				}
				else if (x == x2)
				{
					if (component.IsRotated)
					{
						rotatedCellOffset.x--;
					}
					else
					{
						rotatedCellOffset.x++;
					}
				}
			}
			else if (component.IsRotated)
			{
				rotatedCellOffset.x++;
			}
			else
			{
				rotatedCellOffset.x--;
			}
			return rotatedCellOffset;
		}

		public bool shouldBeBehindMilkTank = true;

		public DrinkMilkStates.Def.DrinkCellOffsetGetFn drinkCellOffsetGetFn = new DrinkMilkStates.Def.DrinkCellOffsetGetFn(DrinkMilkStates.Def.DrinkCellOffsetGet_CritterOneByOne);

		public delegate CellOffset DrinkCellOffsetGetFn(MilkFeeder.Instance milkFeederInstance, DrinkMilkMonitor.Instance critterInstance, bool isCramped);
	}

	public new class Instance : GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.GameInstance
	{
		public Instance(Chore<DrinkMilkStates.Instance> chore, DrinkMilkStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviour_TryToDrinkMilkFromFeeder);
		}

		public void RequestToStopFeeding()
		{
			base.sm.requestedToStopFeeding.Trigger(base.smi);
		}

		public bool critterIsCramped;
	}

	public class EatingState : GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State
	{
		public GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State pre;

		public GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State loop;

		public GameStateMachine<DrinkMilkStates, DrinkMilkStates.Instance, IStateMachineTarget, DrinkMilkStates.Def>.State pst;
	}
}
