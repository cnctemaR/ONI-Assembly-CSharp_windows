using System;
using Klei;
using STRINGS;
using UnityEngine;

public class LayEggStates : GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.layeggpre;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.LAYINGANEGG.NAME, CREATURES.STATUSITEMS.LAYINGANEGG.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.layeggpre.Enter(new StateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State.Callback(LayEggStates.LayEgg)).Exit(new StateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State.Callback(LayEggStates.ShowEgg)).PlayAnim("lay_egg_pre")
			.OnAnimQueueComplete(this.layeggpst);
		this.layeggpst.PlayAnim("lay_egg_pst").OnAnimQueueComplete(this.moveaside);
		this.moveaside.MoveTo(new Func<LayEggStates.Instance, int>(LayEggStates.GetMoveAsideCell), this.lookategg, this.behaviourcomplete, false);
		this.lookategg.Enter(new StateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State.Callback(LayEggStates.FaceEgg)).GoTo(this.behaviourcomplete);
		this.behaviourcomplete.QueueAnim("idle_loop", true, null).BehaviourComplete(GameTags.Creatures.Fertile, false);
	}

	private static void LayEgg(LayEggStates.Instance smi)
	{
		smi.eggPos = smi.transform.GetPosition();
		smi.GetSMI<FertilityMonitor.Instance>().LayEgg();
	}

	private static void ShowEgg(LayEggStates.Instance smi)
	{
		smi.GetSMI<FertilityMonitor.Instance>().ShowEgg();
	}

	private static void FaceEgg(LayEggStates.Instance smi)
	{
		smi.Get<Facing>().Face(smi.eggPos);
	}

	private static int GetMoveAsideCell(LayEggStates.Instance smi)
	{
		int num = 1;
		if (GenericGameSettings.instance.acceleratedLifecycle)
		{
			num = 8;
		}
		int num2 = Grid.PosToCell(smi);
		if (Grid.IsValidCell(num2))
		{
			int num3 = Grid.OffsetCell(num2, num, 0);
			if (Grid.IsValidCell(num3) && !Grid.Solid[num3])
			{
				return num3;
			}
			int num4 = Grid.OffsetCell(num2, -num, 0);
			if (Grid.IsValidCell(num4))
			{
				return num4;
			}
		}
		return Grid.InvalidCell;
	}

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State layeggpre;

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State layeggpst;

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State moveaside;

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State lookategg;

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.GameInstance
	{
		public Instance(Chore<LayEggStates.Instance> chore, LayEggStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Fertile);
		}

		public Vector3 eggPos;
	}
}
