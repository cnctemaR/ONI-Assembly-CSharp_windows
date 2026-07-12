using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

public class ClusterCometDetector : GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (ClusterCometDetector.Instance smi) => smi.GetComponent<Operational>().IsOperational).Update("Scan Sky", delegate(ClusterCometDetector.Instance smi, float dt)
		{
			smi.ScanSky(false);
		}, UpdateRate.SIM_4000ms, false);
		this.on.DefaultState(this.on.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.DetectorScanning, null).Enter("ToggleActive", delegate(ClusterCometDetector.Instance smi)
		{
			smi.GetComponent<Operational>().SetActive(true, false);
		})
			.Exit("ToggleActive", delegate(ClusterCometDetector.Instance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			});
		this.on.pre.PlayAnim("on_pre").OnAnimQueueComplete(this.on.loop);
		this.on.loop.PlayAnim("on", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.on.pst, (ClusterCometDetector.Instance smi) => !smi.GetComponent<Operational>().IsOperational).TagTransition(GameTags.Detecting, this.on.working, false)
			.Enter("UpdateLogic", delegate(ClusterCometDetector.Instance smi)
			{
				smi.UpdateDetectionState(smi.HasTag(GameTags.Detecting), false);
			})
			.Update("Scan Sky", delegate(ClusterCometDetector.Instance smi, float dt)
			{
				smi.ScanSky(false);
			}, UpdateRate.SIM_200ms, false);
		this.on.pst.PlayAnim("on_pst").OnAnimQueueComplete(this.off);
		this.on.working.DefaultState(this.on.working.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.IncomingMeteors, null).Enter("UpdateLogic", delegate(ClusterCometDetector.Instance smi)
		{
			smi.SetLogicSignal(true);
		})
			.Exit("UpdateLogic", delegate(ClusterCometDetector.Instance smi)
			{
				smi.SetLogicSignal(false);
			})
			.Update("Scan Sky", delegate(ClusterCometDetector.Instance smi, float dt)
			{
				smi.ScanSky(true);
			}, UpdateRate.SIM_200ms, false);
		this.on.working.pre.PlayAnim("detect_pre").OnAnimQueueComplete(this.on.working.loop);
		this.on.working.loop.PlayAnim("detect_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.on.working.pst, (ClusterCometDetector.Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, this.on.working.pst, (ClusterCometDetector.Instance smi) => !smi.GetComponent<Operational>().IsActive)
			.TagTransition(GameTags.Detecting, this.on.working.pst, true);
		this.on.working.pst.PlayAnim("detect_pst").OnAnimQueueComplete(this.on.loop).Enter("Reroll", delegate(ClusterCometDetector.Instance smi)
		{
			smi.RerollAccuracy();
		});
	}

	public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State off;

	public ClusterCometDetector.OnStates on;

	public class Def : StateMachine.BaseDef
	{
	}

	public class OnStates : GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State
	{
		public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State pre;

		public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State loop;

		public ClusterCometDetector.WorkingStates working;

		public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State pst;
	}

	public class WorkingStates : GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State
	{
		public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State pre;

		public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State loop;

		public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State pst;
	}

	public new class Instance : GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ClusterCometDetector.Def def)
			: base(master, def)
		{
			this.detectorNetworkDef = new DetectorNetwork.Def();
			this.detectorNetworkDef.interferenceRadius = 15;
			this.detectorNetworkDef.worstWarningTime = 1f;
			this.detectorNetworkDef.bestWarningTime = 200f;
			this.detectorNetworkDef.bestNetworkSize = 6;
			this.RerollAccuracy();
		}

		public override void StartSM()
		{
			if (this.detectorNetwork == null)
			{
				this.detectorNetwork = (DetectorNetwork.Instance)this.detectorNetworkDef.CreateSMI(base.master);
			}
			this.detectorNetwork.StartSM();
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			this.detectorNetwork.StopSM(reason);
		}

		public void UpdateDetectionState(bool currentDetection, bool expectedDetectionForState)
		{
			KPrefabID component = base.GetComponent<KPrefabID>();
			if (currentDetection)
			{
				component.AddTag(GameTags.Detecting, false);
			}
			else
			{
				component.RemoveTag(GameTags.Detecting);
			}
			if (currentDetection == expectedDetectionForState)
			{
				this.SetLogicSignal(currentDetection);
			}
		}

		public void ScanSky(bool expectedDetectionForState)
		{
			float detectTime = this.GetDetectTime();
			int myWorldId = this.GetMyWorldId();
			if (this.GetDetectorState() == ClusterCometDetector.Instance.ClusterCometDetectorState.MeteorShower)
			{
				SaveGame.Instance.GetComponent<GameplayEventManager>().GetActiveEventsOfType<MeteorShowerEvent>(myWorldId, ref this.meteorShowers);
				float num = float.MaxValue;
				foreach (GameplayEventInstance gameplayEventInstance in this.meteorShowers)
				{
					MeteorShowerEvent.StatesInstance statesInstance = gameplayEventInstance.smi as MeteorShowerEvent.StatesInstance;
					if (statesInstance != null)
					{
						num = Mathf.Min(num, statesInstance.TimeUntilNextShower());
					}
				}
				this.meteorShowers.Clear();
				this.UpdateDetectionState(num < detectTime, expectedDetectionForState);
			}
			if (this.GetDetectorState() == ClusterCometDetector.Instance.ClusterCometDetectorState.BallisticObject)
			{
				float num2 = float.MaxValue;
				foreach (object obj in Components.ClusterTravelers)
				{
					ClusterTraveler clusterTraveler = (ClusterTraveler)obj;
					bool flag = clusterTraveler.IsTraveling();
					bool flag2 = clusterTraveler.GetComponent<Clustercraft>() != null;
					if (flag && !flag2 && clusterTraveler.GetDestinationWorldID() == myWorldId)
					{
						num2 = Mathf.Min(num2, clusterTraveler.TravelETA());
					}
				}
				this.UpdateDetectionState(num2 < detectTime, expectedDetectionForState);
			}
			if (this.GetDetectorState() == ClusterCometDetector.Instance.ClusterCometDetectorState.Rocket && this.targetCraft != null)
			{
				Clustercraft clustercraft = this.targetCraft.Get();
				if (!clustercraft.IsNullOrDestroyed())
				{
					ClusterTraveler component = clustercraft.GetComponent<ClusterTraveler>();
					bool flag3 = false;
					if (clustercraft.Status != Clustercraft.CraftStatus.Grounded)
					{
						bool flag4 = component.GetDestinationWorldID() == myWorldId;
						bool flag5 = component.IsTraveling();
						bool flag6 = clustercraft.HasResourcesToMove(1, Clustercraft.CombustionResource.All);
						float num3 = component.TravelETA();
						flag3 = (flag4 && flag5 && flag6 && num3 < detectTime) || (!flag5 && flag4 && clustercraft.Status == Clustercraft.CraftStatus.Landing);
						if (!flag3)
						{
							ClusterGridEntity adjacentAsteroid = clustercraft.GetAdjacentAsteroid();
							flag3 = ((adjacentAsteroid != null) ? ClusterUtil.GetAsteroidWorldIdAtLocation(adjacentAsteroid.Location) : ((int)ClusterManager.INVALID_WORLD_IDX)) == myWorldId && clustercraft.Status == Clustercraft.CraftStatus.Launching;
						}
					}
					this.UpdateDetectionState(flag3, expectedDetectionForState);
				}
			}
		}

		public void RerollAccuracy()
		{
			this.nextAccuracy = global::UnityEngine.Random.value;
		}

		public void SetLogicSignal(bool on)
		{
			base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, on ? 1 : 0);
		}

		public float GetDetectTime()
		{
			return this.detectorNetwork.GetDetectTimeRange().Lerp(this.nextAccuracy);
		}

		public void SetDetectorState(ClusterCometDetector.Instance.ClusterCometDetectorState newState)
		{
			this.detectorState = newState;
		}

		public ClusterCometDetector.Instance.ClusterCometDetectorState GetDetectorState()
		{
			return this.detectorState;
		}

		public void SetClustercraftTarget(Clustercraft target)
		{
			if (target)
			{
				this.targetCraft = new Ref<Clustercraft>(target);
				return;
			}
			this.targetCraft = null;
		}

		public Clustercraft GetClustercraftTarget()
		{
			Ref<Clustercraft> @ref = this.targetCraft;
			if (@ref == null)
			{
				return null;
			}
			return @ref.Get();
		}

		public bool ShowWorkingStatus;

		private const float BEST_WARNING_TIME = 200f;

		private const float WORST_WARNING_TIME = 1f;

		private const float VARIANCE = 50f;

		private const int MAX_DISH_COUNT = 6;

		private const int INTERFERENCE_RADIUS = 15;

		[Serialize]
		private ClusterCometDetector.Instance.ClusterCometDetectorState detectorState;

		[Serialize]
		private float nextAccuracy;

		[Serialize]
		private Ref<Clustercraft> targetCraft;

		private DetectorNetwork.Def detectorNetworkDef;

		private DetectorNetwork.Instance detectorNetwork;

		private List<GameplayEventInstance> meteorShowers = new List<GameplayEventInstance>();

		public enum ClusterCometDetectorState
		{
			MeteorShower,
			BallisticObject,
			Rocket
		}
	}
}
