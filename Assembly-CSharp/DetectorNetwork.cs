using System;
using STRINGS;

public class DetectorNetwork : GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.inoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (DetectorNetwork.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.operational.InitializeStates(this).EventTransition(GameHashes.OperationalChanged, this.inoperational, (DetectorNetwork.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
	}

	public StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.FloatParameter networkQuality;

	public GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State inoperational;

	public DetectorNetwork.NetworkStates operational;

	public class Def : StateMachine.BaseDef
	{
	}

	public class NetworkStates : GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State
	{
		public DetectorNetwork.NetworkStates InitializeStates(DetectorNetwork parent)
		{
			base.DefaultState(this.poor);
			this.poor.ToggleStatusItem(BUILDING.STATUSITEMS.NETWORKQUALITY.NAME, BUILDING.STATUSITEMS.NETWORKQUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, default(HashedString), 129022, new Func<string, DetectorNetwork.Instance, string>(this.StringCallback), null, null).ParamTransition<float>(parent.networkQuality, this.good, (DetectorNetwork.Instance smi, float p) => (double)p >= 0.8);
			this.good.ToggleStatusItem(BUILDING.STATUSITEMS.NETWORKQUALITY.NAME, BUILDING.STATUSITEMS.NETWORKQUALITY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, new Func<string, DetectorNetwork.Instance, string>(this.StringCallback), null, null).ParamTransition<float>(parent.networkQuality, this.poor, (DetectorNetwork.Instance smi, float p) => (double)p < 0.8);
			return this;
		}

		private string StringCallback(string str, DetectorNetwork.Instance smi)
		{
			MathUtil.MinMax detectTimeRangeForWorld = Game.Instance.spaceScannerNetworkManager.GetDetectTimeRangeForWorld(smi.GetMyWorldId());
			float num = Game.Instance.spaceScannerNetworkManager.GetQualityForWorld(smi.GetMyWorldId());
			num = num.Remap(new ValueTuple<float, float>(0f, 1f), new ValueTuple<float, float>(0f, 0.5f));
			return str.Replace("{TotalQuality}", GameUtil.GetFormattedPercent(smi.GetNetworkQuality01() * 100f, GameUtil.TimeSlice.None)).Replace("{WorstTime}", GameUtil.GetFormattedTime(detectTimeRangeForWorld.min, "F0")).Replace("{BestTime}", GameUtil.GetFormattedTime(detectTimeRangeForWorld.max, "F0"))
				.Replace("{Coverage}", GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None));
		}

		public GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State poor;

		public GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State good;
	}

	public new class Instance : GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, DetectorNetwork.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			this.worldId = base.master.gameObject.GetMyWorldId();
			Components.DetectorNetworks.Add(this.worldId, this);
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			Components.DetectorNetworks.Remove(this.worldId, this);
		}

		public void Internal_SetNetworkQuality(float quality01)
		{
			base.sm.networkQuality.Set(quality01, base.smi, false);
		}

		public float GetNetworkQuality01()
		{
			return base.sm.networkQuality.Get(base.smi);
		}

		[NonSerialized]
		private int worldId;
	}
}
