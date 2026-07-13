using System;
using UnityEngine;

public class FoodSmoker : GameStateMachine<FoodSmoker, FoodSmoker.StatesInstance, IStateMachineTarget, FoodSmoker.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.working;
		this.working.Enter(delegate(FoodSmoker.StatesInstance smi)
		{
			smi.complexFabricator.SetQueueDirty();
			smi.operational.SetFlag(FoodSmoker.foodSmokerFlag, true);
		}).EnterTransition(this.requestEmpty, (FoodSmoker.StatesInstance smi) => smi.RequiresEmptying()).EventHandlerTransition(GameHashes.FabricatorOrderCompleted, this.requestEmpty, (FoodSmoker.StatesInstance smi, object data) => smi.RequiresEmptying());
		this.requestEmpty.ToggleRecurringChore(new Func<FoodSmoker.StatesInstance, Chore>(this.CreateChore), new Action<FoodSmoker.StatesInstance, Chore>(FoodSmoker.SetRemoteChore), (FoodSmoker.StatesInstance smi) => smi.RequiresEmptying()).EventHandlerTransition(GameHashes.OnStorageChange, this.working, (FoodSmoker.StatesInstance smi, object data) => !smi.RequiresEmptying()).Enter(delegate(FoodSmoker.StatesInstance smi)
		{
			smi.operational.SetFlag(FoodSmoker.foodSmokerFlag, false);
		})
			.ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingEmptyBuilding, null);
	}

	private static void SetRemoteChore(FoodSmoker.StatesInstance smi, Chore chore)
	{
		smi.remoteChore.SetChore(chore);
	}

	private Chore CreateChore(FoodSmoker.StatesInstance smi)
	{
		WorkChore<FoodSmokerWorkableEmpty> workChore = new WorkChore<FoodSmokerWorkableEmpty>(Db.Get().ChoreTypes.Cook, smi.master.GetComponent<FoodSmokerWorkableEmpty>(), null, true, new Action<Chore>(smi.OnEmptyComplete), null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, null);
		workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanGasRange.Id);
		return workChore;
	}

	private static readonly Operational.Flag foodSmokerFlag = new Operational.Flag("food_smoker", Operational.Flag.Type.Requirement);

	private GameStateMachine<FoodSmoker, FoodSmoker.StatesInstance, IStateMachineTarget, FoodSmoker.Def>.State working;

	private GameStateMachine<FoodSmoker, FoodSmoker.StatesInstance, IStateMachineTarget, FoodSmoker.Def>.State requestEmpty;

	public class Def : StateMachine.BaseDef
	{
	}

	public class StatesInstance : GameStateMachine<FoodSmoker, FoodSmoker.StatesInstance, IStateMachineTarget, FoodSmoker.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, FoodSmoker.Def def)
			: base(master, def)
		{
		}

		public bool RequiresEmptying()
		{
			return !this.complexFabricator.outStorage.IsEmpty();
		}

		public void OnEmptyComplete(Chore obj)
		{
			Vector3 vector = Grid.CellToPosLCC(Grid.PosToCell(this), Grid.SceneLayer.Ore);
			this.complexFabricator.outStorage.DropAll(vector, false, true, default(Vector3), true, null);
		}

		[MyCmpAdd]
		public ManuallySetRemoteWorkTargetComponent remoteChore;

		[MyCmpReq]
		public Operational operational;

		[MyCmpReq]
		public ComplexFabricator complexFabricator;
	}
}
