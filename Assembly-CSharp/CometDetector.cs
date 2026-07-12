using System;
using System.Collections.Generic;
using KSerialization;

public class CometDetector : GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (CometDetector.Instance smi) => smi.GetComponent<Operational>().IsOperational).Update("Scan Sky", delegate(CometDetector.Instance smi, float dt)
		{
			smi.ScanSky(false);
		}, UpdateRate.SIM_4000ms, false);
		this.on.DefaultState(this.on.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.DetectorScanning, null).Enter("ToggleActive", delegate(CometDetector.Instance smi)
		{
			smi.GetComponent<Operational>().SetActive(true, false);
		})
			.Exit("ToggleActive", delegate(CometDetector.Instance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			});
		this.on.pre.PlayAnim("on_pre").OnAnimQueueComplete(this.on.loop);
		this.on.loop.PlayAnim("on", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.on.pst, (CometDetector.Instance smi) => !smi.GetComponent<Operational>().IsOperational).TagTransition(GameTags.Detecting, this.on.working, false)
			.Enter("UpdateLogic", delegate(CometDetector.Instance smi)
			{
				smi.UpdateDetectionState(smi.HasTag(GameTags.Detecting), false);
			})
			.Update("Scan Sky", delegate(CometDetector.Instance smi, float dt)
			{
				smi.ScanSky(false);
			}, UpdateRate.SIM_200ms, false);
		this.on.pst.PlayAnim("on_pst").OnAnimQueueComplete(this.off);
		this.on.working.DefaultState(this.on.working.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.IncomingMeteors, null).Enter("UpdateLogic", delegate(CometDetector.Instance smi)
		{
			smi.SetLogicSignal(true);
		})
			.Exit("UpdateLogic", delegate(CometDetector.Instance smi)
			{
				smi.SetLogicSignal(false);
			})
			.Update("Scan Sky", delegate(CometDetector.Instance smi, float dt)
			{
				smi.ScanSky(true);
			}, UpdateRate.SIM_200ms, false);
		this.on.working.pre.PlayAnim("detect_pre").OnAnimQueueComplete(this.on.working.loop);
		this.on.working.loop.PlayAnim("detect_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.on.working.pst, (CometDetector.Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, this.on.working.pst, (CometDetector.Instance smi) => !smi.GetComponent<Operational>().IsActive)
			.TagTransition(GameTags.Detecting, this.on.working.pst, true);
		this.on.working.pst.PlayAnim("detect_pst").OnAnimQueueComplete(this.on.loop);
	}

	public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State off;

	public CometDetector.OnStates on;

	public class Def : StateMachine.BaseDef
	{
	}

	public class OnStates : GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State
	{
		public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State pre;

		public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State loop;

		public CometDetector.WorkingStates working;

		public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State pst;
	}

	public class WorkingStates : GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State
	{
		public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State pre;

		public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State loop;

		public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State pst;
	}

	public new class Instance : GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CometDetector.Def def)
			: base(master, def)
		{
			this.detectorNetworkDef = new DetectorNetwork.Def();
			this.targetCraft = new Ref<LaunchConditionManager>();
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
			LaunchConditionManager launchConditionManager = this.targetCraft.Get();
			Option<SpaceScannerTarget> option;
			if (launchConditionManager == null)
			{
				option = SpaceScannerTarget.MeteorShower();
			}
			else if (SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.targetCraft.Get()).state == Spacecraft.MissionState.Destroyed)
			{
				option = Option.None;
			}
			else
			{
				option = SpaceScannerTarget.RocketBaseGame(launchConditionManager);
			}
			bool flag = option.IsSome() && Game.Instance.spaceScannerNetworkManager.IsTargetDetectedOnWorld(this.GetMyWorldId(), option.Unwrap());
			this.UpdateDetectionState(flag, expectedDetectionForState);
		}

		public void SetLogicSignal(bool on)
		{
			base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, on ? 1 : 0);
		}

		public void SetTargetCraft(LaunchConditionManager target)
		{
			this.targetCraft.Set(target);
		}

		public LaunchConditionManager GetTargetCraft()
		{
			return this.targetCraft.Get();
		}

		public bool ShowWorkingStatus;

		[Serialize]
		private Ref<LaunchConditionManager> targetCraft;

		private DetectorNetwork.Def detectorNetworkDef;

		private DetectorNetwork.Instance detectorNetwork;

		private List<GameplayEventInstance> meteorShowers = new List<GameplayEventInstance>();
	}
}
