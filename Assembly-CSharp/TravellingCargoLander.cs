using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TravellingCargoLander : GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.init;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.InitializeOperationalFlag(RocketModule.landedFlag, false).Enter(delegate(TravellingCargoLander.StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.OnStorageChange, delegate(TravellingCargoLander.StatesInstance smi)
		{
			smi.CheckIfLoaded();
		});
		this.init.ParamTransition<bool>(this.isLanding, this.landing.landing, GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.IsTrue).ParamTransition<bool>(this.isLanded, this.grounded, GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.IsTrue).GoTo(this.travel);
		this.travel.DefaultState(this.travel.travelling).Enter(delegate(TravellingCargoLander.StatesInstance smi)
		{
			smi.MoveToSpace();
		}).PlayAnim("idle")
			.ToggleTag(GameTags.EntityInSpace)
			.ToggleMainStatusItem(Db.Get().BuildingStatusItems.InFlight, (TravellingCargoLander.StatesInstance smi) => smi.GetComponent<ClusterTraveler>());
		this.travel.travelling.EventTransition(GameHashes.ClusterDestinationReached, this.travel.transferWorlds, null);
		this.travel.transferWorlds.Enter(delegate(TravellingCargoLander.StatesInstance smi)
		{
			smi.StartLand();
		}).GoTo(this.landing.landing);
		this.landing.Enter(delegate(TravellingCargoLander.StatesInstance smi)
		{
			this.isLanding.Set(true, smi);
		}).Exit(delegate(TravellingCargoLander.StatesInstance smi)
		{
			this.isLanding.Set(false, smi);
		});
		this.landing.landing.PlayAnim("landing", KAnim.PlayMode.Loop).Enter(delegate(TravellingCargoLander.StatesInstance smi)
		{
			smi.ResetAnimPosition();
		}).Update(delegate(TravellingCargoLander.StatesInstance smi, float dt)
		{
			smi.LandingUpdate(dt);
		}, UpdateRate.SIM_EVERY_TICK, false)
			.Transition(this.landing.impact, (TravellingCargoLander.StatesInstance smi) => smi.flightAnimOffset <= 0f, UpdateRate.SIM_200ms)
			.Enter(delegate(TravellingCargoLander.StatesInstance smi)
			{
				smi.MoveToWorld();
			});
		this.landing.impact.PlayAnim("grounded_pre").OnAnimQueueComplete(this.grounded);
		this.grounded.DefaultState(this.grounded.loaded).ToggleOperationalFlag(RocketModule.landedFlag).Enter(delegate(TravellingCargoLander.StatesInstance smi)
		{
			smi.CheckIfLoaded();
		})
			.Enter(delegate(TravellingCargoLander.StatesInstance smi)
			{
				this.isLanded.Set(true, smi);
			});
		this.grounded.loaded.PlayAnim("grounded").ParamTransition<bool>(this.hasCargo, this.grounded.empty, GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.IsFalse).OnSignal(this.emptyCargo, this.grounded.emptying)
			.Enter(delegate(TravellingCargoLander.StatesInstance smi)
			{
				smi.DoLand();
			});
		this.grounded.emptying.PlayAnim("deploying").TriggerOnEnter(GameHashes.JettisonCargo, null).OnAnimQueueComplete(this.grounded.empty);
		this.grounded.empty.PlayAnim("deployed").ParamTransition<bool>(this.hasCargo, this.grounded.loaded, GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.IsTrue);
	}

	public StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.IntParameter destinationWorld = new StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.IntParameter(-1);

	public StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.BoolParameter isLanding = new StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.BoolParameter(false);

	public StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.BoolParameter isLanded = new StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.BoolParameter(false);

	public StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.BoolParameter hasCargo = new StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.BoolParameter(false);

	public StateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.Signal emptyCargo;

	public GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State init;

	public TravellingCargoLander.TravelStates travel;

	public TravellingCargoLander.LandingStates landing;

	public TravellingCargoLander.GroundedStates grounded;

	public class Def : StateMachine.BaseDef
	{
		public int landerWidth = 1;

		public float landingSpeed = 5f;

		public bool deployOnLanding;
	}

	public class TravelStates : GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State
	{
		public GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State travelling;

		public GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State transferWorlds;
	}

	public class LandingStates : GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State
	{
		public GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State landing;

		public GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State impact;
	}

	public class GroundedStates : GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State
	{
		public GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State loaded;

		public GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State emptying;

		public GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.State empty;
	}

	public class StatesInstance : GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, TravellingCargoLander.Def def)
			: base(master, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
		}

		public void Travel(AxialI source, AxialI destination)
		{
			base.GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
			int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
			base.sm.destinationWorld.Set(asteroidWorldIdAtLocation, this);
			this.GoTo(base.sm.travel);
		}

		public void StartLand()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(base.sm.destinationWorld.Get(this));
			Vector3 vector = Grid.CellToPosCBC(ClusterManager.Instance.GetRandomSurfaceCell(world.id, base.def.landerWidth, true), this.animController.sceneLayer);
			base.transform.SetPosition(vector);
		}

		public bool UpdateLanding(float dt)
		{
			if (base.gameObject.GetMyWorld() != null)
			{
				Vector3 position = base.transform.GetPosition();
				position.y -= 0.5f;
				int num = Grid.PosToCell(position);
				if (Grid.IsWorldValidCell(num) && Grid.IsSolidCell(num))
				{
					return true;
				}
			}
			return false;
		}

		public void MoveToSpace()
		{
			Pickupable component = base.GetComponent<Pickupable>();
			if (component != null)
			{
				component.deleteOffGrid = false;
			}
			base.gameObject.transform.SetPosition(new Vector3(-1f, -1f, Grid.GetLayerZ(this.animController.sceneLayer)));
		}

		public void MoveToWorld()
		{
			Pickupable component = base.GetComponent<Pickupable>();
			if (component != null)
			{
				component.deleteOffGrid = true;
			}
		}

		public void ResetAnimPosition()
		{
			this.animController.Offset = Vector3.up * this.flightAnimOffset;
		}

		public void LandingUpdate(float dt)
		{
			this.flightAnimOffset = Mathf.Max(this.flightAnimOffset - dt * base.def.landingSpeed, 0f);
			this.ResetAnimPosition();
		}

		public void DoLand()
		{
			this.animController.Offset = Vector3.zero;
			OccupyArea component = base.smi.GetComponent<OccupyArea>();
			if (component != null)
			{
				component.ApplyToCells = true;
			}
			if (base.def.deployOnLanding && this.CheckIfLoaded())
			{
				base.sm.emptyCargo.Trigger(this);
			}
		}

		public bool CheckIfLoaded()
		{
			bool flag = false;
			MinionStorage component = base.GetComponent<MinionStorage>();
			if (component != null)
			{
				flag |= component.GetStoredMinionInfo().Count > 0;
			}
			Storage component2 = base.GetComponent<Storage>();
			if (component2 != null && !component2.IsEmpty())
			{
				flag = true;
			}
			if (flag != base.sm.hasCargo.Get(this))
			{
				base.sm.hasCargo.Set(flag, this);
			}
			return flag;
		}

		[Serialize]
		public float flightAnimOffset = 50f;

		public KBatchedAnimController animController;
	}
}
