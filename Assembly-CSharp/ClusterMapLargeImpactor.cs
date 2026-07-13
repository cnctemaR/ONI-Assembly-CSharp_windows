using System;
using KSerialization;

public class ClusterMapLargeImpactor : GameStateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.traveling;
		this.traveling.DefaultState(this.traveling.unidentified).EventTransition(GameHashes.ClusterDestinationReached, this.arrived, null);
		this.traveling.unidentified.ParamTransition<bool>(this.IsIdentified, this.traveling.identified, GameStateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>.IsTrue);
		this.traveling.identified.ParamTransition<bool>(this.IsIdentified, this.traveling.unidentified, GameStateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>.IsFalse).ToggleStatusItem(Db.Get().MiscStatusItems.ClusterMeteorRemainingTravelTime, null);
		this.arrived.DoNothing();
	}

	public StateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>.BoolParameter IsIdentified;

	public ClusterMapLargeImpactor.TravelingState traveling;

	public GameStateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>.State arrived;

	public class Def : StateMachine.BaseDef
	{
		public string name;

		public string description;

		public string eventID;

		public int destinationWorldID;

		public float arrivalTime;
	}

	public class TravelingState : GameStateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>.State
	{
		public GameStateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>.State unidentified;

		public GameStateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>.State identified;
	}

	public new class Instance : GameStateMachine<ClusterMapLargeImpactor, ClusterMapLargeImpactor.Instance, IStateMachineTarget, ClusterMapLargeImpactor.Def>.GameInstance
	{
		public WorldContainer World_Destination
		{
			get
			{
				return ClusterManager.Instance.GetWorld(this.DestinationWorldID);
			}
		}

		public AxialI ClusterGridPosition()
		{
			return this.visualizer.Location;
		}

		public Instance(IStateMachineTarget master, ClusterMapLargeImpactor.Def def)
			: base(master, def)
		{
			this.traveler.getSpeedCB = new Func<float>(this.GetSpeed);
			this.traveler.onTravelCB = new global::System.Action(this.OnTravellerMoved);
		}

		private void OnTravellerMoved()
		{
			Game.Instance.Trigger(-1975776133, this);
		}

		protected override void OnCleanUp()
		{
			Components.LongRangeMissileTargetables.Remove(base.gameObject.GetComponent<ClusterGridEntity>());
			this.visualizer.Deselect();
			base.OnCleanUp();
		}

		public override void StartSM()
		{
			base.StartSM();
			if (this.DestinationWorldID < 0)
			{
				this.Setup(base.def.destinationWorldID, base.def.arrivalTime);
			}
			Components.LongRangeMissileTargetables.Add(base.gameObject.GetComponent<ClusterGridEntity>());
			this.RefreshVisuals(false);
		}

		public void RefreshVisuals(bool playIdentifyAnimationIfVisible = false)
		{
			this.selectable.SetName(base.def.name);
			this.descriptor.description = base.def.description;
			this.visualizer.PlayRevealAnimation(playIdentifyAnimationIfVisible);
			base.Trigger(1980521255, null);
		}

		public void Setup(int destinationWorldID, float arrivalTime)
		{
			this.DestinationWorldID = destinationWorldID;
			this.ArrivalTime = arrivalTime;
			AxialI location = this.World_Destination.GetComponent<ClusterGridEntity>().Location;
			this.destinationSelector.SetDestination(location);
			this.traveler.RevalidatePath(false);
			ClusterFogOfWarManager.Instance smi = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			foreach (AxialI axialI in this.traveler.CurrentPath)
			{
				smi.RevealLocation(axialI, 0, 0);
			}
			int count = this.traveler.CurrentPath.Count;
			float num = arrivalTime - GameUtil.GetCurrentTimeInCycles() * 600f;
			this.Speed = (float)count / num * 600f;
		}

		public float GetSpeed()
		{
			return this.Speed;
		}

		[Serialize]
		public int DestinationWorldID = -1;

		[Serialize]
		public float ArrivalTime;

		[Serialize]
		private float Speed;

		[MyCmpGet]
		private InfoDescription descriptor;

		[MyCmpGet]
		private KSelectable selectable;

		[MyCmpGet]
		private ClusterMapMeteorShowerVisualizer visualizer;

		[MyCmpGet]
		private ClusterTraveler traveler;

		[MyCmpGet]
		private ClusterDestinationSelector destinationSelector;
	}
}
