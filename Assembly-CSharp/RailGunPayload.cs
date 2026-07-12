using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RailGunPayload : GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.grounded.idle;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.grounded.DefaultState(this.grounded.idle).Enter(delegate(RailGunPayload.StatesInstance smi)
		{
			this.onSurface.Set(true, smi);
		}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.RailgunpayloadNeedsEmptying, null)
			.ToggleTag(GameTags.RailGunPayloadEmptyable)
			.ToggleTag(GameTags.ClusterEntityGrounded)
			.EventHandler(GameHashes.DroppedAll, delegate(RailGunPayload.StatesInstance smi)
			{
				smi.OnDroppedAll();
			})
			.OnSignal(this.launch, this.takeoff);
		this.grounded.idle.PlayAnim("idle");
		this.grounded.crater.Enter(delegate(RailGunPayload.StatesInstance smi)
		{
			smi.animController.randomiseLoopedOffset = true;
		}).Exit(delegate(RailGunPayload.StatesInstance smi)
		{
			smi.animController.randomiseLoopedOffset = false;
		}).PlayAnim("landed", KAnim.PlayMode.Loop)
			.EventTransition(GameHashes.OnStore, this.grounded.idle, null);
		this.takeoff.DefaultState(this.takeoff.launch).Enter(delegate(RailGunPayload.StatesInstance smi)
		{
			this.onSurface.Set(false, smi);
		}).PlayAnim("launching")
			.OnSignal(this.beginTravelling, this.travel);
		this.takeoff.launch.Enter(delegate(RailGunPayload.StatesInstance smi)
		{
			smi.StartTakeoff();
		}).GoTo(this.takeoff.airborne);
		this.takeoff.airborne.Update("Launch", delegate(RailGunPayload.StatesInstance smi, float dt)
		{
			smi.UpdateLaunch(dt);
		}, UpdateRate.SIM_EVERY_TICK, false);
		this.travel.DefaultState(this.travel.travelling).Enter(delegate(RailGunPayload.StatesInstance smi)
		{
			this.onSurface.Set(false, smi);
		}).Enter(delegate(RailGunPayload.StatesInstance smi)
		{
			smi.MoveToSpace();
		})
			.PlayAnim("idle")
			.ToggleTag(GameTags.EntityInSpace)
			.ToggleMainStatusItem(Db.Get().BuildingStatusItems.InFlight, (RailGunPayload.StatesInstance smi) => smi.GetComponent<ClusterTraveler>());
		this.travel.travelling.EventTransition(GameHashes.ClusterDestinationReached, this.travel.transferWorlds, null);
		this.travel.transferWorlds.Enter(delegate(RailGunPayload.StatesInstance smi)
		{
			smi.StartLand();
		}).GoTo(this.landing.landing);
		this.landing.DefaultState(this.landing.landing).ParamTransition<bool>(this.onSurface, this.grounded.crater, GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.IsTrue).ParamTransition<int>(this.destinationWorld, this.takeoff, (RailGunPayload.StatesInstance smi, int p) => p != -1)
			.Enter(delegate(RailGunPayload.StatesInstance smi)
			{
				smi.MoveToWorld();
			});
		this.landing.landing.PlayAnim("falling", KAnim.PlayMode.Loop).UpdateTransition(this.landing.impact, (RailGunPayload.StatesInstance smi, float dt) => smi.UpdateLanding(dt), UpdateRate.SIM_200ms, false).ToggleGravity(this.landing.impact);
		this.landing.impact.PlayAnim("land").TriggerOnEnter(GameHashes.JettisonCargo, null).OnAnimQueueComplete(this.grounded.crater);
	}

	public StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.IntParameter destinationWorld = new StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.IntParameter(-1);

	public StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.BoolParameter onSurface = new StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.BoolParameter(false);

	public StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.Signal beginTravelling;

	public StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.Signal launch;

	public RailGunPayload.TakeoffStates takeoff;

	public RailGunPayload.TravelStates travel;

	public RailGunPayload.LandingStates landing;

	public RailGunPayload.GroundedStates grounded;

	public class Def : StateMachine.BaseDef
	{
		public bool attractToBeacons;

		public string clusterAnimSymbolSwapTarget;

		public List<string> randomClusterSymbolSwaps;

		public string worldAnimSymbolSwapTarget;

		public List<string> randomWorldSymbolSwaps;
	}

	public class TakeoffStates : GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State
	{
		public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State launch;

		public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State airborne;
	}

	public class TravelStates : GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State
	{
		public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State travelling;

		public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State transferWorlds;
	}

	public class LandingStates : GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State
	{
		public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State landing;

		public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State impact;
	}

	public class GroundedStates : GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State
	{
		public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State crater;

		public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State idle;
	}

	public class StatesInstance : GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, RailGunPayload.Def def)
			: base(master, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
			DebugUtil.Assert(def.clusterAnimSymbolSwapTarget == null == (def.worldAnimSymbolSwapTarget == null), "Must specify both or neither symbol swap targets!");
			DebugUtil.Assert((def.randomClusterSymbolSwaps == null && def.randomWorldSymbolSwaps == null) || def.randomClusterSymbolSwaps.Count == def.randomWorldSymbolSwaps.Count, "Must specify the same number of swaps for both world and cluster!");
			if (def.clusterAnimSymbolSwapTarget != null && def.worldAnimSymbolSwapTarget != null)
			{
				if (this.randomSymbolSwapIndex == -1)
				{
					this.randomSymbolSwapIndex = global::UnityEngine.Random.Range(0, def.randomClusterSymbolSwaps.Count);
					global::Debug.Log(string.Format("Rolling a random symbol: {0}", this.randomSymbolSwapIndex), base.gameObject);
				}
				base.GetComponent<BallisticClusterGridEntity>().SwapSymbolFromSameAnim(def.clusterAnimSymbolSwapTarget, def.randomClusterSymbolSwaps[this.randomSymbolSwapIndex]);
				KAnim.Build.Symbol symbol = this.animController.AnimFiles[0].GetData().build.GetSymbol(def.randomWorldSymbolSwaps[this.randomSymbolSwapIndex]);
				this.animController.GetComponent<SymbolOverrideController>().AddSymbolOverride(def.worldAnimSymbolSwapTarget, symbol, 0);
			}
		}

		public void Launch(AxialI source, AxialI destination)
		{
			base.GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
			int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
			base.sm.destinationWorld.Set(asteroidWorldIdAtLocation, this);
			this.GoTo(base.sm.takeoff);
		}

		public void Travel(AxialI source, AxialI destination)
		{
			base.GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
			int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
			base.sm.destinationWorld.Set(asteroidWorldIdAtLocation, this);
			this.GoTo(base.sm.travel);
		}

		public void StartTakeoff()
		{
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
		}

		public void StartLand()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(base.sm.destinationWorld.Get(this));
			int num = Grid.InvalidCell;
			if (base.def.attractToBeacons)
			{
				num = ClusterManager.Instance.GetLandingBeaconLocation(world.id);
			}
			int num6;
			if (num != Grid.InvalidCell)
			{
				int num2;
				int num3;
				Grid.CellToXY(num, out num2, out num3);
				int num4 = Mathf.Max(num2 - 3, (int)world.minimumBounds.x);
				int num5 = Mathf.Min(num2 + 3, (int)world.maximumBounds.x);
				num6 = Mathf.RoundToInt((float)global::UnityEngine.Random.Range(num4, num5));
			}
			else
			{
				num6 = Mathf.RoundToInt(global::UnityEngine.Random.Range(world.minimumBounds.x + 3f, world.maximumBounds.x - 3f));
			}
			Vector3 vector = new Vector3((float)num6 + 0.5f, world.maximumBounds.y - 1f, Grid.GetLayerZ(Grid.SceneLayer.Front));
			base.transform.SetPosition(vector);
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
			GameComps.Fallers.Add(base.gameObject, new Vector2(0f, -10f));
			base.sm.destinationWorld.Set(-1, this);
		}

		public void UpdateLaunch(float dt)
		{
			if (base.gameObject.GetMyWorld() != null)
			{
				Vector3 vector = base.transform.GetPosition() + new Vector3(0f, this.takeoffVelocity * dt, 0f);
				base.transform.SetPosition(vector);
				return;
			}
			base.sm.beginTravelling.Trigger(this);
			ClusterGridEntity component = base.GetComponent<ClusterGridEntity>();
			if (ClusterGrid.Instance.GetAsteroidAtCell(component.Location) != null)
			{
				base.GetComponent<ClusterTraveler>().AdvancePathOneStep();
			}
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

		public void OnDroppedAll()
		{
			base.gameObject.DeleteObject();
		}

		public bool IsTraveling()
		{
			return base.IsInsideState(base.sm.travel.travelling);
		}

		public void MoveToSpace()
		{
			Pickupable component = base.GetComponent<Pickupable>();
			if (component != null)
			{
				component.deleteOffGrid = false;
			}
			base.gameObject.transform.SetPosition(new Vector3(-1f, -1f, 0f));
		}

		public void MoveToWorld()
		{
			Pickupable component = base.GetComponent<Pickupable>();
			if (component != null)
			{
				component.deleteOffGrid = true;
			}
		}

		[Serialize]
		public float takeoffVelocity;

		[Serialize]
		private int randomSymbolSwapIndex = -1;

		public KBatchedAnimController animController;
	}
}
