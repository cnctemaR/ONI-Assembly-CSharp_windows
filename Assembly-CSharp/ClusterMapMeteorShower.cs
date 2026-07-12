using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClusterMapMeteorShower : GameStateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.traveling;
		this.traveling.DefaultState(this.traveling.unidentified).EventTransition(GameHashes.ClusterDestinationReached, this.arrived, null);
		this.traveling.unidentified.ParamTransition<bool>(this.IsIdentified, this.traveling.identified, GameStateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.IsTrue);
		this.traveling.identified.ParamTransition<bool>(this.IsIdentified, this.traveling.unidentified, GameStateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.IsFalse).ToggleStatusItem(Db.Get().MiscStatusItems.ClusterMeteorRemainingTravelTime, null);
		this.arrived.Enter(new StateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.State.Callback(ClusterMapMeteorShower.DestinationReached));
	}

	public static void DestinationReached(ClusterMapMeteorShower.Instance smi)
	{
		smi.DestinationReached();
		Util.KDestroyGameObject(smi.gameObject);
	}

	public StateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.BoolParameter IsIdentified;

	public ClusterMapMeteorShower.TravelingState traveling;

	public GameStateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.State arrived;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			GameplayEvent gameplayEvent = Db.Get().GameplayEvents.Get(this.eventID);
			List<Descriptor> list = new List<Descriptor>();
			ClusterMapMeteorShower.Instance smi = go.GetSMI<ClusterMapMeteorShower.Instance>();
			if (smi != null && smi.sm.IsIdentified.Get(smi) && gameplayEvent is MeteorShowerEvent)
			{
				List<MeteorShowerEvent.BombardmentInfo> meteorsInfo = (gameplayEvent as MeteorShowerEvent).GetMeteorsInfo();
				float num = 0f;
				foreach (MeteorShowerEvent.BombardmentInfo bombardmentInfo in meteorsInfo)
				{
					num += bombardmentInfo.weight;
				}
				foreach (MeteorShowerEvent.BombardmentInfo bombardmentInfo2 in meteorsInfo)
				{
					GameObject prefab = Assets.GetPrefab(bombardmentInfo2.prefab);
					string formattedPercent = GameUtil.GetFormattedPercent((float)Mathf.RoundToInt(bombardmentInfo2.weight / num * 100f), GameUtil.TimeSlice.None);
					string text = prefab.GetProperName() + " " + formattedPercent;
					Descriptor descriptor = new Descriptor(text, UI.GAMEOBJECTEFFECTS.TOOLTIPS.METEOR_SHOWER_SINGLE_METEOR_PERCENTAGE_TOOLTIP, Descriptor.DescriptorType.Effect, false);
					list.Add(descriptor);
				}
			}
			return list;
		}

		public string name;

		public string description;

		public string description_Hidden;

		public string name_Hidden;

		public string eventID;

		public int destinationWorldID;

		public float arrivalTime;
	}

	public class TravelingState : GameStateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.State
	{
		public GameStateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.State unidentified;

		public GameStateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.State identified;
	}

	public new class Instance : GameStateMachine<ClusterMapMeteorShower, ClusterMapMeteorShower.Instance, IStateMachineTarget, ClusterMapMeteorShower.Def>.GameInstance, ISidescreenButtonControl
	{
		public WorldContainer World_Destination
		{
			get
			{
				return ClusterManager.Instance.GetWorld(this.DestinationWorldID);
			}
		}

		public string SidescreenButtonText
		{
			get
			{
				if (!base.smi.sm.IsIdentified.Get(base.smi))
				{
					return "Identify";
				}
				return "Dev Hide";
			}
		}

		public string SidescreenButtonTooltip
		{
			get
			{
				if (!base.smi.sm.IsIdentified.Get(base.smi))
				{
					return "Identifies the meteor shower";
				}
				return "Dev unidentify back";
			}
		}

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

		public Instance(IStateMachineTarget master, ClusterMapMeteorShower.Def def)
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
			if (this.DestinationWorldID < 0)
			{
				this.Setup(base.def.destinationWorldID, base.def.arrivalTime);
			}
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

		public void Setup(int destinationWorldID, float arrivalTime)
		{
			this.DestinationWorldID = destinationWorldID;
			this.ArrivalTime = arrivalTime;
			AxialI location = this.World_Destination.GetComponent<ClusterGridEntity>().Location;
			this.destinationSelector.SetDestination(location);
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

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
			throw new NotImplementedException();
		}

		public bool SidescreenEnabled()
		{
			return false;
		}

		public bool SidescreenButtonInteractable()
		{
			return true;
		}

		public void OnSidescreenButtonPressed()
		{
			this.Identify();
		}

		public int HorizontalGroupID()
		{
			return -1;
		}

		public int ButtonSideScreenSortOrder()
		{
			return SORTORDER.KEEPSAKES;
		}

		[Serialize]
		public int DestinationWorldID = -1;

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
