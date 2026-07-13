using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class ClusterMapResourceMeteor : GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.traveling;
		this.traveling.DefaultState(this.traveling.unidentified).EventTransition(GameHashes.ClusterDestinationReached, this.leaving, null);
		this.traveling.unidentified.ParamTransition<bool>(this.IsIdentified, this.traveling.identified, GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.IsTrue);
		this.traveling.identified.ParamTransition<bool>(this.IsIdentified, this.traveling.unidentified, GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.IsFalse).ToggleStatusItem(Db.Get().MiscStatusItems.ClusterMeteorRemainingTravelTime, null);
		this.leaving.Enter(new StateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.State.Callback(ClusterMapResourceMeteor.DestinationReached));
	}

	public static void DestinationReached(ClusterMapResourceMeteor.Instance smi)
	{
		smi.DestinationReached();
		Util.KDestroyGameObject(smi.gameObject);
	}

	public StateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.BoolParameter IsIdentified;

	public ClusterMapResourceMeteor.TravelingState traveling;

	public GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.State leaving;

	public GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.State left;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>();
		}

		public string name;

		public string description;

		public string description_Hidden;

		public string name_Hidden;

		public string eventID;

		private AxialI destination;

		public float arrivalTime;
	}

	public class TravelingState : GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.State
	{
		public GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.State unidentified;

		public GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.State identified;
	}

	public new class Instance : GameStateMachine<ClusterMapResourceMeteor, ClusterMapResourceMeteor.Instance, IStateMachineTarget, ClusterMapResourceMeteor.Def>.GameInstance
	{
		public bool HasBeenIdentified
		{
			get
			{
				return base.sm.IsIdentified.Get(this);
			}
		}

		public float IdentifyingProgress
		{
			get
			{
				return this.identifyingProgress;
			}
		}

		public AxialI ClusterGridPosition()
		{
			return this.visualizer.Location;
		}

		public Instance(IStateMachineTarget master, ClusterMapResourceMeteor.Def def)
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
			this.visualizer.Deselect();
			base.OnCleanUp();
		}

		public void Identify()
		{
			if (!this.HasBeenIdentified)
			{
				this.identifyingProgress = 1f;
				base.sm.IsIdentified.Set(true, this, false);
				Game.Instance.Trigger(1427028915, this);
				this.RefreshVisuals(true);
				if (ClusterMapScreen.Instance.IsActive())
				{
					KFMOD.PlayUISound(GlobalAssets.GetSound("ClusterMapMeteor_Reveal", false));
				}
			}
		}

		public void ProgressIdentifiction(float points)
		{
			if (!this.HasBeenIdentified)
			{
				this.identifyingProgress += points;
				this.identifyingProgress = Mathf.Clamp(this.identifyingProgress, 0f, 1f);
				if (this.identifyingProgress == 1f)
				{
					this.Identify();
				}
			}
		}

		public override void StartSM()
		{
			base.StartSM();
			this.RefreshVisuals(false);
		}

		public void RefreshVisuals(bool playIdentifyAnimationIfVisible = false)
		{
			if (this.HasBeenIdentified)
			{
				this.selectable.SetName(base.def.name);
				this.descriptor.description = base.def.description;
				this.visualizer.PlayRevealAnimation(playIdentifyAnimationIfVisible);
			}
			else
			{
				this.selectable.SetName(base.def.name_Hidden);
				this.descriptor.description = base.def.description_Hidden;
				this.visualizer.PlayHideAnimation();
			}
			base.Trigger(1980521255, null);
		}

		public void Setup(AxialI destination, float arrivalTime)
		{
			this.Destination = destination;
			this.ArrivalTime = arrivalTime;
			this.destinationSelector.SetDestination(destination);
			this.traveler.RevalidatePath(false);
			int count = this.traveler.CurrentPath.Count;
			float num = arrivalTime - GameUtil.GetCurrentTimeInCycles() * 600f;
			this.Speed = (float)count / num * 600f;
		}

		public float GetSpeed()
		{
			return this.Speed;
		}

		public void DestinationReached()
		{
			global::System.Action onDestinationReached = this.OnDestinationReached;
			if (onDestinationReached == null)
			{
				return;
			}
			onDestinationReached();
		}

		[Serialize]
		public AxialI Destination;

		[Serialize]
		public float ArrivalTime;

		[Serialize]
		private float Speed;

		[Serialize]
		private float identifyingProgress;

		public global::System.Action OnDestinationReached;

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
