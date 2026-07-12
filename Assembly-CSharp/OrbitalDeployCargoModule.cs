using System;
using KSerialization;
using UnityEngine;

public class OrbitalDeployCargoModule : GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.grounded;
		this.root.Enter(delegate(OrbitalDeployCargoModule.StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.OnStorageChange, delegate(OrbitalDeployCargoModule.StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.ClusterDestinationReached, delegate(OrbitalDeployCargoModule.StatesInstance smi)
		{
			if (smi.AutoDeploy && smi.IsValidDropLocation())
			{
				smi.DeployCargoPods();
			}
		});
		this.grounded.DefaultState(this.grounded.loaded).TagTransition(GameTags.RocketNotOnGround, this.not_grounded, false);
		this.grounded.loading.PlayAnim((OrbitalDeployCargoModule.StatesInstance smi) => smi.GetLoadingAnimName(), KAnim.PlayMode.Once).ParamTransition<bool>(this.hasCargo, this.grounded.empty, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsFalse).OnAnimQueueComplete(this.grounded.loaded);
		this.grounded.loaded.ParamTransition<bool>(this.hasCargo, this.grounded.empty, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsFalse).EventTransition(GameHashes.OnStorageChange, this.grounded.loading, (OrbitalDeployCargoModule.StatesInstance smi) => smi.NeedsVisualUpdate());
		this.grounded.empty.Enter(delegate(OrbitalDeployCargoModule.StatesInstance smi)
		{
			this.numVisualCapsules.Set(0, smi);
		}).PlayAnim("deployed").ParamTransition<bool>(this.hasCargo, this.grounded.loaded, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsTrue);
		this.not_grounded.DefaultState(this.not_grounded.loaded).TagTransition(GameTags.RocketNotOnGround, this.grounded, true);
		this.not_grounded.loaded.PlayAnim("loaded").ParamTransition<bool>(this.hasCargo, this.not_grounded.empty, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsFalse).OnSignal(this.emptyCargo, this.not_grounded.emptying);
		this.not_grounded.emptying.PlayAnim("deploying").GoTo(this.not_grounded.empty);
		this.not_grounded.empty.PlayAnim("deployed").ParamTransition<bool>(this.hasCargo, this.not_grounded.loaded, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsTrue);
	}

	public StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.BoolParameter hasCargo;

	public StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.Signal emptyCargo;

	public OrbitalDeployCargoModule.GroundedStates grounded;

	public OrbitalDeployCargoModule.NotGroundedStates not_grounded;

	public StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IntParameter numVisualCapsules;

	public class Def : StateMachine.BaseDef
	{
		public float numCapsules;
	}

	public class GroundedStates : GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State
	{
		public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State loading;

		public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State loaded;

		public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State empty;
	}

	public class NotGroundedStates : GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State
	{
		public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State loaded;

		public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State emptying;

		public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State empty;
	}

	public class StatesInstance : GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.GameInstance, IEmptyableCargo
	{
		public StatesInstance(IStateMachineTarget master, OrbitalDeployCargoModule.Def def)
			: base(master, def)
		{
			this.storage = base.GetComponent<Storage>();
			base.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new LoadingCompleteCondition(this.storage));
		}

		public bool NeedsVisualUpdate()
		{
			int num = base.sm.numVisualCapsules.Get(this);
			int num2 = Mathf.FloorToInt(this.storage.MassStored() / 200f);
			if (num < num2)
			{
				base.sm.numVisualCapsules.Delta(1, this);
				return true;
			}
			return false;
		}

		public string GetLoadingAnimName()
		{
			int num = base.sm.numVisualCapsules.Get(this);
			int num2 = Mathf.RoundToInt(this.storage.capacityKg / 200f);
			if (num == num2)
			{
				return "loading6_full";
			}
			if (num == num2 - 1)
			{
				return "loading5";
			}
			if (num == num2 - 2)
			{
				return "loading4";
			}
			if (num == num2 - 3 || num > 2)
			{
				return "loading3_repeat";
			}
			if (num == 2)
			{
				return "loading2";
			}
			if (num == 1)
			{
				return "loading1";
			}
			return "deployed";
		}

		public void DeployCargoPods()
		{
			Clustercraft component = base.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			ClusterGridEntity orbitAsteroid = component.GetOrbitAsteroid();
			if (orbitAsteroid != null)
			{
				WorldContainer component2 = orbitAsteroid.GetComponent<WorldContainer>();
				int id = component2.id;
				Vector3 vector = new Vector3(component2.minimumBounds.x + 1f, component2.maximumBounds.y, Grid.GetLayerZ(Grid.SceneLayer.Front));
				while (this.storage.MassStored() > 0f)
				{
					GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RailGunPayload"), vector);
					gameObject.GetComponent<Pickupable>().deleteOffGrid = false;
					float num = 0f;
					while (num < 200f && this.storage.MassStored() > 0f)
					{
						num += this.storage.Transfer(gameObject.GetComponent<Storage>(), GameTags.Stored, 200f - num, false, true);
					}
					gameObject.SetActive(true);
					gameObject.GetSMI<RailGunPayload.StatesInstance>().Travel(component.Location, component2.GetMyWorldLocation());
				}
			}
			this.CheckIfLoaded();
		}

		public bool CheckIfLoaded()
		{
			bool flag = this.storage.MassStored() > 0f;
			if (flag != base.sm.hasCargo.Get(this))
			{
				base.sm.hasCargo.Set(flag, this);
			}
			return flag;
		}

		public bool IsValidDropLocation()
		{
			return base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().GetOrbitAsteroid() != null;
		}

		public bool AutoDeploy
		{
			get
			{
				return this.autoDeploy;
			}
			set
			{
				this.autoDeploy = value;
			}
		}

		public bool CanAutoDeploy
		{
			get
			{
				return true;
			}
		}

		public void EmptyCargo()
		{
			this.DeployCargoPods();
		}

		public bool CanEmptyCargo()
		{
			return base.sm.hasCargo.Get(base.smi) && this.IsValidDropLocation();
		}

		public bool ChooseDuplicant
		{
			get
			{
				return false;
			}
		}

		public MinionIdentity ChosenDuplicant
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		private Storage storage;

		[Serialize]
		private bool autoDeploy;
	}
}
