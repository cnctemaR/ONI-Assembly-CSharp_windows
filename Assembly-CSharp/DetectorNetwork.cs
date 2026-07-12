using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DetectorNetwork : GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.inoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (DetectorNetwork.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.operational.DefaultState(this.operational.self_poor.poor).Update("CheckForInterference", delegate(DetectorNetwork.Instance smi, float dt)
		{
			smi.Update(dt);
		}, UpdateRate.SIM_1000ms, false).EventTransition(GameHashes.OperationalChanged, this.inoperational, (DetectorNetwork.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		this.operational.self_poor.InitializeStates(this).ToggleStatusItem(BUILDING.STATUSITEMS.DETECTORQUALITY.NAME, BUILDING.STATUSITEMS.DETECTORQUALITY.TOOLTIP, "status_item_interference", StatusItem.IconType.Custom, NotificationType.BadMinor, false, default(HashedString), 129022, (string str, DetectorNetwork.Instance smi) => str.Replace("{Quality}", GameUtil.GetFormattedPercent(smi.GetDishQuality() * 100f, GameUtil.TimeSlice.None)), null, null).ParamTransition<float>(this.selfQuality, this.operational.self_good, (DetectorNetwork.Instance smi, float p) => (double)p >= 0.8);
		this.operational.self_good.InitializeStates(this).ToggleStatusItem(BUILDING.STATUSITEMS.DETECTORQUALITY.NAME, BUILDING.STATUSITEMS.DETECTORQUALITY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, (string str, DetectorNetwork.Instance smi) => str.Replace("{Quality}", GameUtil.GetFormattedPercent(smi.GetDishQuality() * 100f, GameUtil.TimeSlice.None)), null, null).ParamTransition<float>(this.selfQuality, this.operational.self_poor, (DetectorNetwork.Instance smi, float p) => (double)p < 0.8);
	}

	public StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.FloatParameter selfQuality;

	public StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.FloatParameter networkQuality;

	public GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State inoperational;

	public DetectorNetwork.SelfStates operational;

	public class Def : StateMachine.BaseDef
	{
		public int interferenceRadius;

		public float worstWarningTime;

		public float bestWarningTime;

		public int bestNetworkSize;
	}

	public class SelfStates : GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State
	{
		public DetectorNetwork.NetworkStates self_poor;

		public DetectorNetwork.NetworkStates self_good;
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
			MathUtil.MinMax detectTimeRange = smi.GetDetectTimeRange();
			return str.Replace("{TotalQuality}", GameUtil.GetFormattedPercent(smi.ComputeTotalDishQuality() * 100f, GameUtil.TimeSlice.None)).Replace("{WorstTime}", GameUtil.GetFormattedTime(detectTimeRange.min, "F0")).Replace("{BestTime}", GameUtil.GetFormattedTime(detectTimeRange.max, "F0"));
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
			Components.DetectorNetworks.Add(this);
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			Components.DetectorNetworks.Remove(this);
		}

		public void Update(float dt)
		{
			this.CheckForVisibility();
			this.CheckForInterference();
			base.sm.selfQuality.Set(this.GetDishQuality(), base.smi, false);
			base.sm.networkQuality.Set(this.ComputeTotalDishQuality(), base.smi, false);
		}

		private void CheckForVisibility()
		{
			int num = Grid.PosToCell(this);
			int num2 = 0;
			num2 += DetectorNetwork.Instance.ScanVisiblityLine(num, 1, 1, base.def.interferenceRadius);
			num2 += DetectorNetwork.Instance.ScanVisiblityLine(num, -1, 1, base.def.interferenceRadius);
			this.visibleSkyCells = num2;
		}

		public static int ScanVisiblityLine(int start_cell, int x_offset, int y_offset, int radius)
		{
			int num = 0;
			int num2 = 0;
			while (Mathf.Abs(num2) <= radius)
			{
				int num3 = Grid.OffsetCell(start_cell, num2 * x_offset, num2 * y_offset);
				if (Grid.IsValidCell(num3))
				{
					if (Grid.ExposedToSunlight[num3] < 253)
					{
						break;
					}
					num++;
				}
				num2++;
			}
			return num;
		}

		private void CheckForInterference()
		{
			Extents extents = new Extents(Grid.PosToCell(this), base.def.interferenceRadius);
			List<ScenePartitionerEntry> list = new List<ScenePartitionerEntry>();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.industrialBuildings, list);
			float num = float.MaxValue;
			foreach (ScenePartitionerEntry scenePartitionerEntry in list)
			{
				GameObject gameObject = (GameObject)scenePartitionerEntry.obj;
				if (!(gameObject == base.gameObject))
				{
					float magnitude = (base.gameObject.transform.GetPosition() - gameObject.transform.GetPosition()).magnitude;
					num = Mathf.Min(num, magnitude);
				}
			}
			this.closestMachinery = num;
		}

		public float GetDishQuality()
		{
			if (!base.GetComponent<Operational>().IsOperational)
			{
				return 0f;
			}
			return Mathf.Clamp01(this.closestMachinery / (float)base.def.interferenceRadius) * Mathf.Clamp01((float)this.visibleSkyCells / ((float)base.def.interferenceRadius * 2f));
		}

		public float ComputeTotalDishQuality()
		{
			float num = 0f;
			foreach (DetectorNetwork.Instance instance in Components.DetectorNetworks.Items)
			{
				num += instance.GetDishQuality();
			}
			return num / (float)base.def.bestNetworkSize;
		}

		public MathUtil.MinMax GetDetectTimeRange()
		{
			float num = this.ComputeTotalDishQuality();
			return new MathUtil.MinMax(Mathf.Lerp(base.def.worstWarningTime, base.def.bestWarningTime, num), base.def.bestWarningTime);
		}

		private float closestMachinery = float.MaxValue;

		private int visibleSkyCells;
	}
}
