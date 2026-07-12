using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FoodDehydrator : GameStateMachine<FoodDehydrator, FoodDehydrator.StatesInstance, IStateMachineTarget, FoodDehydrator.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		this.waitingForFuelStatus.resolveStringCallback = (string str, object obj) => string.Format(str, FOODDEHYDRATORTUNING.FUEL_TAG.ProperName(), GameUtil.GetFormattedMass(5.0000005f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		default_state = this.waitingForFuel;
		this.waitingForFuel.Enter(delegate(FoodDehydrator.StatesInstance smi)
		{
			smi.operational.SetFlag(FoodDehydrator.foodDehydratorFlag, false);
		}).EventTransition(GameHashes.OnStorageChange, this.working, (FoodDehydrator.StatesInstance smi) => smi.GetAvailableFuel() >= 5.0000005f).ToggleStatusItem(this.waitingForFuelStatus, null);
		this.working.Enter(delegate(FoodDehydrator.StatesInstance smi)
		{
			smi.complexFabricator.SetQueueDirty();
			smi.operational.SetFlag(FoodDehydrator.foodDehydratorFlag, true);
		}).EventHandler(GameHashes.FabricatorOrdersUpdated, delegate(FoodDehydrator.StatesInstance smi)
		{
			smi.UpdateFoodSymbol();
		}).EnterTransition(this.requestEmpty, (FoodDehydrator.StatesInstance smi) => smi.RequiresEmptying())
			.EventTransition(GameHashes.OnStorageChange, this.waitingForFuel, (FoodDehydrator.StatesInstance smi) => smi.GetAvailableFuel() <= 0f)
			.EventHandlerTransition(GameHashes.FabricatorOrderCompleted, this.requestEmpty, (FoodDehydrator.StatesInstance smi, object data) => smi.RequiresEmptying())
			.EventHandler(GameHashes.FabricatorOrderStarted, delegate(FoodDehydrator.StatesInstance smi)
			{
				smi.UpdateFoodSymbol();
			});
		this.requestEmpty.ToggleRecurringChore(new Func<FoodDehydrator.StatesInstance, Chore>(this.CreateChore), (FoodDehydrator.StatesInstance smi) => smi.RequiresEmptying()).EventHandlerTransition(GameHashes.OnStorageChange, this.working, (FoodDehydrator.StatesInstance smi, object data) => !smi.RequiresEmptying()).Enter(delegate(FoodDehydrator.StatesInstance smi)
		{
			smi.operational.SetFlag(FoodDehydrator.foodDehydratorFlag, false);
			smi.UpdateFoodSymbol();
		})
			.ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingEmptyBuilding, null);
	}

	private Chore CreateChore(FoodDehydrator.StatesInstance smi)
	{
		return new WorkChore<FoodDehydratorWorkableEmpty>(Db.Get().ChoreTypes.FoodFetch, smi.master.GetComponent<FoodDehydratorWorkableEmpty>(), null, true, new Action<Chore>(smi.OnEmptyComplete), null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
	}

	private StatusItem waitingForFuelStatus = new StatusItem("waitingForFuelStatus", BUILDING.STATUSITEMS.ENOUGH_FUEL.NAME, BUILDING.STATUSITEMS.ENOUGH_FUEL.TOOLTIP, "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022, true, null);

	private static readonly Operational.Flag foodDehydratorFlag = new Operational.Flag("food_dehydrator", Operational.Flag.Type.Requirement);

	private GameStateMachine<FoodDehydrator, FoodDehydrator.StatesInstance, IStateMachineTarget, FoodDehydrator.Def>.State waitingForFuel;

	private GameStateMachine<FoodDehydrator, FoodDehydrator.StatesInstance, IStateMachineTarget, FoodDehydrator.Def>.State working;

	private GameStateMachine<FoodDehydrator, FoodDehydrator.StatesInstance, IStateMachineTarget, FoodDehydrator.Def>.State requestEmpty;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			Descriptor descriptor = new Descriptor(UI.BUILDINGEFFECTS.FOOD_DEHYDRATOR_WATER_OUTPUT, UI.BUILDINGEFFECTS.TOOLTIPS.FOOD_DEHYDRATOR_WATER_OUTPUT, Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor);
			return list;
		}
	}

	public class StatesInstance : GameStateMachine<FoodDehydrator, FoodDehydrator.StatesInstance, IStateMachineTarget, FoodDehydrator.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, FoodDehydrator.Def def)
			: base(master, def)
		{
			this.SetupFoodSymbol();
		}

		public float GetAvailableFuel()
		{
			return this.complexFabricator.inStorage.GetMassAvailable(FOODDEHYDRATORTUNING.FUEL_TAG);
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

		public void SetupFoodSymbol()
		{
			GameObject gameObject = Util.NewGameObject(base.gameObject, "food_symbol");
			gameObject.SetActive(false);
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			bool flag;
			Vector3 vector = component.GetSymbolTransform(FoodDehydrator.StatesInstance.HASH_FOOD, out flag).GetColumn(3);
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
			gameObject.transform.SetPosition(vector);
			this.foodKBAC = gameObject.AddComponent<KBatchedAnimController>();
			this.foodKBAC.AnimFiles = new KAnimFile[] { Assets.GetAnim("mushbar_kanim") };
			this.foodKBAC.initialAnim = "object";
			component.SetSymbolVisiblity(FoodDehydrator.StatesInstance.HASH_FOOD, false);
			this.foodKBAC.sceneLayer = Grid.SceneLayer.BuildingUse;
			KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
			kbatchedAnimTracker.symbol = new HashedString("food");
			kbatchedAnimTracker.offset = Vector3.zero;
		}

		public void UpdateFoodSymbol()
		{
			ComplexRecipe currentWorkingOrder = this.complexFabricator.CurrentWorkingOrder;
			if (this.complexFabricator.CurrentWorkingOrder != null)
			{
				this.foodKBAC.gameObject.SetActive(true);
				GameObject prefab = Assets.GetPrefab(currentWorkingOrder.ingredients[this.foodIngredientIdx].material);
				this.foodKBAC.SwapAnims(prefab.GetComponent<KBatchedAnimController>().AnimFiles);
				this.foodKBAC.Play("object", KAnim.PlayMode.Loop, 1f, 0f);
				return;
			}
			if (this.complexFabricator.outStorage.items.Count > 0)
			{
				this.foodKBAC.gameObject.SetActive(true);
				this.foodKBAC.SwapAnims(this.complexFabricator.outStorage.items[0].GetComponent<KBatchedAnimController>().AnimFiles);
				this.foodKBAC.Play("object", KAnim.PlayMode.Loop, 1f, 0f);
				return;
			}
			this.foodKBAC.gameObject.SetActive(false);
		}

		[MyCmpReq]
		public Operational operational;

		[MyCmpReq]
		public ComplexFabricator complexFabricator;

		private static string HASH_FOOD = "food";

		private KBatchedAnimController foodKBAC;

		private int foodIngredientIdx;
	}
}
