using System;
using System.Collections.Generic;
using KSerialization;

public class ClusterCometDetector : GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.Enter(delegate(ClusterCometDetector.Instance smi)
		{
			smi.UpdateDetectionState(this.lastIsTargetDetected.Get(smi), true);
			smi.remainingSecondsToFreezeLogicSignal = 3f;
		}).Update(delegate(ClusterCometDetector.Instance smi, float deltaSeconds)
		{
			smi.remainingSecondsToFreezeLogicSignal -= deltaSeconds;
			if (smi.remainingSecondsToFreezeLogicSignal < 0f)
			{
				smi.remainingSecondsToFreezeLogicSignal = 0f;
				return;
			}
			smi.SetLogicSignal(this.lastIsTargetDetected.Get(smi));
		}, UpdateRate.SIM_200ms, false);
		this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (ClusterCometDetector.Instance smi) => smi.GetComponent<Operational>().IsOperational);
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
		this.on.working.pst.PlayAnim("detect_pst").OnAnimQueueComplete(this.on.loop);
	}

	public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State off;

	public ClusterCometDetector.OnStates on;

	public StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.BoolParameter lastIsTargetDetected;

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
			Option<SpaceScannerTarget> option;
			switch (this.GetDetectorState())
			{
			case ClusterCometDetector.Instance.ClusterCometDetectorState.MeteorShower:
				option = SpaceScannerTarget.MeteorShower();
				break;
			case ClusterCometDetector.Instance.ClusterCometDetectorState.BallisticObject:
				option = SpaceScannerTarget.BallisticObject();
				break;
			case ClusterCometDetector.Instance.ClusterCometDetectorState.Rocket:
				if (this.targetCraft != null && this.targetCraft.Get() != null)
				{
					option = SpaceScannerTarget.RocketDlc1(this.targetCraft.Get());
				}
				else
				{
					option = Option.None;
				}
				break;
			default:
				throw new NotImplementedException();
			}
			bool flag = option.IsSome() && Game.Instance.spaceScannerNetworkManager.IsTargetDetectedOnWorld(this.GetMyWorldId(), option.Unwrap());
			base.smi.sm.lastIsTargetDetected.Set(flag, this, false);
			this.UpdateDetectionState(flag, expectedDetectionForState);
		}

		public void SetLogicSignal(bool on)
		{
			base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, on ? 1 : 0);
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
			if (this.targetCraft == null)
			{
				return null;
			}
			return this.targetCraft.Get();
		}

		public bool ShowWorkingStatus;

		[Serialize]
		private ClusterCometDetector.Instance.ClusterCometDetectorState detectorState;

		[Serialize]
		private Ref<Clustercraft> targetCraft;

		[NonSerialized]
		public float remainingSecondsToFreezeLogicSignal;

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
