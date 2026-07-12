using System;
using UnityEngine;

public class LaunchPadMaterialDistributor : GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.inoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (LaunchPadMaterialDistributor.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.operational.DefaultState(this.operational.noRocket).EventTransition(GameHashes.OperationalChanged, this.inoperational, (LaunchPadMaterialDistributor.Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventHandler(GameHashes.ChainedNetworkChanged, delegate(LaunchPadMaterialDistributor.Instance smi, object data)
		{
			this.SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		});
		this.operational.noRocket.Enter(delegate(LaunchPadMaterialDistributor.Instance smi)
		{
			this.SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		}).EventHandler(GameHashes.RocketLanded, delegate(LaunchPadMaterialDistributor.Instance smi, object data)
		{
			this.SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		}).EventHandler(GameHashes.RocketCreated, delegate(LaunchPadMaterialDistributor.Instance smi, object data)
		{
			this.SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		})
			.ParamTransition<GameObject>(this.attachedRocket, this.operational.rocketLanding, (LaunchPadMaterialDistributor.Instance smi, GameObject p) => p != null);
		this.operational.rocketLanding.EventTransition(GameHashes.RocketLaunched, this.operational.rocketLost, null).OnTargetLost(this.attachedRocket, this.operational.rocketLost).Target(this.attachedRocket)
			.TagTransition(GameTags.RocketOnGround, this.operational.hasRocket, false)
			.Target(this.masterTarget);
		this.operational.hasRocket.DefaultState(this.operational.hasRocket.transferring).Update(delegate(LaunchPadMaterialDistributor.Instance smi, float dt)
		{
			smi.EmptyRocket(dt);
		}, UpdateRate.SIM_1000ms, false).Update(delegate(LaunchPadMaterialDistributor.Instance smi, float dt)
		{
			smi.FillRocket(dt);
		}, UpdateRate.SIM_1000ms, false)
			.EventTransition(GameHashes.RocketLaunched, this.operational.rocketLost, null)
			.OnTargetLost(this.attachedRocket, this.operational.rocketLost)
			.Target(this.attachedRocket)
			.EventTransition(GameHashes.DoLaunchRocket, this.operational.rocketLost, null)
			.Target(this.masterTarget);
		this.operational.hasRocket.transferring.DefaultState(this.operational.hasRocket.transferring.actual).ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoEmptying, null).ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFilling, null);
		this.operational.hasRocket.transferring.actual.ParamTransition<bool>(this.emptyComplete, this.operational.hasRocket.transferring.delay, (LaunchPadMaterialDistributor.Instance smi, bool p) => this.emptyComplete.Get(smi) && this.fillComplete.Get(smi)).ParamTransition<bool>(this.fillComplete, this.operational.hasRocket.transferring.delay, (LaunchPadMaterialDistributor.Instance smi, bool p) => this.emptyComplete.Get(smi) && this.fillComplete.Get(smi));
		this.operational.hasRocket.transferring.delay.ParamTransition<bool>(this.fillComplete, this.operational.hasRocket.transferring.actual, GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.IsFalse).ParamTransition<bool>(this.emptyComplete, this.operational.hasRocket.transferring.actual, GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.IsFalse).ScheduleGoTo(4f, this.operational.hasRocket.transferComplete);
		this.operational.hasRocket.transferComplete.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFull, null).ToggleTag(GameTags.TransferringCargoComplete).ParamTransition<bool>(this.fillComplete, this.operational.hasRocket.transferring, GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.IsFalse)
			.ParamTransition<bool>(this.emptyComplete, this.operational.hasRocket.transferring, GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.IsFalse);
		this.operational.rocketLost.Enter(delegate(LaunchPadMaterialDistributor.Instance smi)
		{
			this.emptyComplete.Set(false, smi, false);
			this.fillComplete.Set(false, smi, false);
			this.SetAttachedRocket(null, smi);
		}).GoTo(this.operational.noRocket);
	}

	private void SetAttachedRocket(RocketModuleCluster attached, LaunchPadMaterialDistributor.Instance smi)
	{
		HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet pooledHashSet = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
		smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref pooledHashSet);
		foreach (ChainedBuilding.StatesInstance statesInstance in pooledHashSet)
		{
			ModularConduitPortController.Instance smi2 = statesInstance.GetSMI<ModularConduitPortController.Instance>();
			if (smi2 != null)
			{
				smi2.SetRocket(attached != null);
			}
		}
		this.attachedRocket.Set(attached, smi);
		pooledHashSet.Recycle();
	}

	public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State inoperational;

	public LaunchPadMaterialDistributor.OperationalStates operational;

	private StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.TargetParameter attachedRocket;

	private StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.BoolParameter emptyComplete;

	private StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.BoolParameter fillComplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public class HasRocketStates : GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State
	{
		public LaunchPadMaterialDistributor.HasRocketStates.TransferringStates transferring;

		public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State transferComplete;

		public class TransferringStates : GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State
		{
			public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State actual;

			public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State delay;
		}
	}

	public class OperationalStates : GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State
	{
		public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State noRocket;

		public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State rocketLanding;

		public LaunchPadMaterialDistributor.HasRocketStates hasRocket;

		public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State rocketLost;
	}

	public new class Instance : GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, LaunchPadMaterialDistributor.Def def)
			: base(master, def)
		{
		}

		public RocketModuleCluster GetLandedRocketFromPad()
		{
			return base.GetComponent<LaunchPad>().LandedRocket;
		}

		public void EmptyRocket(float dt)
		{
			CraftModuleInterface craftInterface = base.sm.attachedRocket.Get<RocketModuleCluster>(base.smi).CraftInterface;
			DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			foreach (Ref<RocketModuleCluster> @ref in craftInterface.ClusterModules)
			{
				CargoBayCluster component = @ref.Get().GetComponent<CargoBayCluster>();
				if (component != null && component.storageType != CargoBay.CargoType.Entities && component.storage.MassStored() > 0f)
				{
					pooledDictionary[component.storageType].Add(component);
				}
			}
			bool flag = false;
			HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet pooledHashSet = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
			base.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref pooledHashSet);
			foreach (ChainedBuilding.StatesInstance statesInstance in pooledHashSet)
			{
				ModularConduitPortController.Instance smi = statesInstance.GetSMI<ModularConduitPortController.Instance>();
				IConduitDispenser component2 = statesInstance.GetComponent<IConduitDispenser>();
				Operational component3 = statesInstance.GetComponent<Operational>();
				bool flag2 = false;
				if (component2 != null && (smi == null || smi.SelectedMode == ModularConduitPortController.Mode.Unload || smi.SelectedMode == ModularConduitPortController.Mode.Both) && (component3 == null || component3.IsOperational))
				{
					smi.SetRocket(true);
					TreeFilterable component4 = statesInstance.GetComponent<TreeFilterable>();
					float num = component2.Storage.RemainingCapacity();
					foreach (CargoBayCluster cargoBayCluster in pooledDictionary[CargoBayConduit.ElementToCargoMap[component2.ConduitType]])
					{
						if (cargoBayCluster.storage.Count != 0)
						{
							for (int i = cargoBayCluster.storage.items.Count - 1; i >= 0; i--)
							{
								GameObject gameObject = cargoBayCluster.storage.items[i];
								if (component4.AcceptedTags.Contains(gameObject.PrefabID()))
								{
									flag2 = true;
									flag = true;
									if (num <= 0f)
									{
										break;
									}
									Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(num);
									if (pickupable != null)
									{
										component2.Storage.Store(pickupable.gameObject, false, false, true, false);
										num -= pickupable.PrimaryElement.Mass;
									}
								}
							}
						}
					}
				}
				if (smi != null)
				{
					smi.SetUnloading(flag2);
				}
			}
			pooledHashSet.Recycle();
			pooledDictionary[CargoBay.CargoType.Solids].Recycle();
			pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
			pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
			pooledDictionary.Recycle();
			base.sm.emptyComplete.Set(!flag, this, false);
		}

		public void FillRocket(float dt)
		{
			CraftModuleInterface craftInterface = base.sm.attachedRocket.Get<RocketModuleCluster>(base.smi).CraftInterface;
			DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			foreach (Ref<RocketModuleCluster> @ref in craftInterface.ClusterModules)
			{
				CargoBayCluster component = @ref.Get().GetComponent<CargoBayCluster>();
				if (component != null && component.storageType != CargoBay.CargoType.Entities && component.RemainingCapacity > 0f)
				{
					pooledDictionary[component.storageType].Add(component);
				}
			}
			bool flag = false;
			HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet pooledHashSet = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
			base.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref pooledHashSet);
			foreach (ChainedBuilding.StatesInstance statesInstance in pooledHashSet)
			{
				ModularConduitPortController.Instance smi = statesInstance.GetSMI<ModularConduitPortController.Instance>();
				IConduitConsumer component2 = statesInstance.GetComponent<IConduitConsumer>();
				bool flag2 = false;
				if (component2 != null && (smi == null || smi.SelectedMode == ModularConduitPortController.Mode.Load || smi.SelectedMode == ModularConduitPortController.Mode.Both))
				{
					smi.SetRocket(true);
					for (int i = component2.Storage.items.Count - 1; i >= 0; i--)
					{
						GameObject gameObject = component2.Storage.items[i];
						foreach (CargoBayCluster cargoBayCluster in pooledDictionary[CargoBayConduit.ElementToCargoMap[component2.ConduitType]])
						{
							float num = cargoBayCluster.RemainingCapacity;
							float num2 = component2.Storage.MassStored();
							if (num > 0f && num2 > 0f && cargoBayCluster.GetComponent<TreeFilterable>().AcceptedTags.Contains(gameObject.PrefabID()))
							{
								flag2 = true;
								flag = true;
								Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(num);
								if (pickupable != null)
								{
									cargoBayCluster.storage.Store(pickupable.gameObject, false, false, true, false);
									num -= pickupable.PrimaryElement.Mass;
								}
							}
						}
					}
				}
				if (smi != null)
				{
					smi.SetLoading(flag2);
				}
			}
			pooledHashSet.Recycle();
			pooledDictionary[CargoBay.CargoType.Solids].Recycle();
			pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
			pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
			pooledDictionary.Recycle();
			base.sm.fillComplete.Set(!flag, base.smi, false);
		}
	}
}
