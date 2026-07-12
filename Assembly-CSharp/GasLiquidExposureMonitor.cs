using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

public class GasLiquidExposureMonitor : GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.normal;
		this.root.Update(new Action<GasLiquidExposureMonitor.Instance, float>(this.UpdateExposure), UpdateRate.SIM_33ms, false);
		this.normal.ParamTransition<bool>(this.isIrritated, this.irritated, (GasLiquidExposureMonitor.Instance smi, bool p) => this.isIrritated.Get(smi));
		this.irritated.ParamTransition<bool>(this.isIrritated, this.normal, (GasLiquidExposureMonitor.Instance smi, bool p) => !this.isIrritated.Get(smi)).ToggleStatusItem(Db.Get().DuplicantStatusItems.GasLiquidIrritation, (GasLiquidExposureMonitor.Instance smi) => smi).DefaultState(this.irritated.irritated);
		this.irritated.irritated.Transition(this.irritated.rubbingEyes, new StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.Transition.ConditionCallback(GasLiquidExposureMonitor.CanReact), UpdateRate.SIM_200ms);
		this.irritated.rubbingEyes.Exit(delegate(GasLiquidExposureMonitor.Instance smi)
		{
			smi.lastReactTime = GameClock.Instance.GetTime();
		}).ToggleReactable((GasLiquidExposureMonitor.Instance smi) => smi.GetReactable()).OnSignal(this.reactFinished, this.irritated.irritated);
	}

	private static bool CanReact(GasLiquidExposureMonitor.Instance smi)
	{
		return GameClock.Instance.GetTime() > smi.lastReactTime + 60f;
	}

	private static void InitializeCustomRates()
	{
		if (GasLiquidExposureMonitor.customExposureRates != null)
		{
			return;
		}
		GasLiquidExposureMonitor.minorIrritationEffect = Db.Get().effects.Get("MinorIrritation");
		GasLiquidExposureMonitor.majorIrritationEffect = Db.Get().effects.Get("MajorIrritation");
		GasLiquidExposureMonitor.customExposureRates = new Dictionary<SimHashes, float>();
		float num = -1f;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.Water] = num;
		float num2 = -0.25f;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.CarbonDioxide] = num2;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.Oxygen] = num2;
		float num3 = 0f;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.ContaminatedOxygen] = num3;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.DirtyWater] = num3;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.ViscoGel] = num3;
		float num4 = 0.5f;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.Hydrogen] = num4;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.SaltWater] = num4;
		float num5 = 1f;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.ChlorineGas] = num5;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.EthanolGas] = num5;
		float num6 = 3f;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.Chlorine] = num6;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.SourGas] = num6;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.Brine] = num6;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.Ethanol] = num6;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.SuperCoolant] = num6;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.CrudeOil] = num6;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.Naphtha] = num6;
		GasLiquidExposureMonitor.customExposureRates[SimHashes.Petroleum] = num6;
	}

	public float GetCurrentExposure(GasLiquidExposureMonitor.Instance smi)
	{
		float num;
		if (GasLiquidExposureMonitor.customExposureRates.TryGetValue(smi.CurrentlyExposedToElement().id, out num))
		{
			return num;
		}
		return 0f;
	}

	private void UpdateExposure(GasLiquidExposureMonitor.Instance smi, float dt)
	{
		GasLiquidExposureMonitor.InitializeCustomRates();
		float num = 0f;
		smi.isAirtightSuit = false;
		int num2 = Grid.CellAbove(Grid.PosToCell(smi.gameObject));
		if (Grid.IsValidCell(num2))
		{
			Element element = Grid.Element[num2];
			float num3;
			if (!GasLiquidExposureMonitor.customExposureRates.TryGetValue(element.id, out num3))
			{
				if (Grid.Temperature[num2] >= -13657.5f && Grid.Temperature[num2] <= 27315f)
				{
					num3 = 1f;
				}
				else
				{
					num3 = 2f;
				}
			}
			if (smi.master.gameObject.HasTag(GameTags.HasSuitTank) && smi.gameObject.GetComponent<SuitEquipper>().IsWearingAirtightSuit())
			{
				smi.isAirtightSuit = true;
				num = GasLiquidExposureMonitor.customExposureRates[SimHashes.Oxygen];
			}
			else if (element.IsGas)
			{
				num = num3 * Grid.Mass[num2] / 1f;
			}
			else if (element.IsLiquid)
			{
				num = num3 * Grid.Mass[num2] / 1000f;
			}
		}
		smi.exposureRate = num;
		smi.exposure += smi.exposureRate * dt;
		smi.exposure = MathUtil.Clamp(0f, 30f, smi.exposure);
		this.ApplyEffects(smi);
	}

	private void ApplyEffects(GasLiquidExposureMonitor.Instance smi)
	{
		if (smi.IsMinorIrritation())
		{
			smi.effects.Add(GasLiquidExposureMonitor.minorIrritationEffect, true);
			this.isIrritated.Set(true, smi);
			return;
		}
		if (smi.IsMajorIrritation())
		{
			smi.effects.Add(GasLiquidExposureMonitor.majorIrritationEffect, true);
			this.isIrritated.Set(true, smi);
			return;
		}
		smi.effects.Remove(GasLiquidExposureMonitor.minorIrritationEffect);
		smi.effects.Remove(GasLiquidExposureMonitor.majorIrritationEffect);
		this.isIrritated.Set(false, smi);
	}

	public Effect GetAppliedEffect(GasLiquidExposureMonitor.Instance smi)
	{
		if (smi.IsMinorIrritation())
		{
			return GasLiquidExposureMonitor.minorIrritationEffect;
		}
		if (smi.IsMajorIrritation())
		{
			return GasLiquidExposureMonitor.majorIrritationEffect;
		}
		return null;
	}

	public const float MIN_REACT_INTERVAL = 60f;

	private static Dictionary<SimHashes, float> customExposureRates;

	private static Effect minorIrritationEffect;

	private static Effect majorIrritationEffect;

	public StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.BoolParameter isIrritated;

	public StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.Signal reactFinished;

	public GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State normal;

	public GasLiquidExposureMonitor.IrritatedStates irritated;

	public class Def : StateMachine.BaseDef
	{
	}

	public class TUNING
	{
		public const float MINOR_IRRITATION_THRESHOLD = 8f;

		public const float MAJOR_IRRITATION_THRESHOLD = 15f;

		public const float MAX_EXPOSURE = 30f;

		public const float GAS_UNITS = 1f;

		public const float LIQUID_UNITS = 1000f;

		public const float REDUCE_EXPOSURE_RATE_FAST = -1f;

		public const float REDUCE_EXPOSURE_RATE_SLOW = -0.25f;

		public const float NO_CHANGE = 0f;

		public const float SLOW_EXPOSURE_RATE = 0.5f;

		public const float NORMAL_EXPOSURE_RATE = 1f;

		public const float QUICK_EXPOSURE_RATE = 3f;

		public const float DEFAULT_MIN_TEMPERATURE = -13657.5f;

		public const float DEFAULT_MAX_TEMPERATURE = 27315f;

		public const float DEFAULT_LOW_RATE = 1f;

		public const float DEFAULT_HIGH_RATE = 2f;
	}

	public class IrritatedStates : GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State
	{
		public GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State irritated;

		public GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State rubbingEyes;
	}

	public new class Instance : GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.GameInstance
	{
		public float minorIrritationThreshold
		{
			get
			{
				return 8f;
			}
		}

		public Instance(IStateMachineTarget master, GasLiquidExposureMonitor.Def def)
			: base(master, def)
		{
			this.effects = master.GetComponent<Effects>();
		}

		public Reactable GetReactable()
		{
			EmoteReactable emoteReactable = new SelfEmoteReactable(base.master.gameObject, "IrritatedEyes", Db.Get().ChoreTypes.Cough, "anim_irritated_eyes_kanim", 0f, 0f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "irritated_eyes",
				finishcb = delegate(GameObject go)
				{
					base.sm.reactFinished.Trigger(this);
				}
			});
			emoteReactable.preventChoreInterruption = true;
			return emoteReactable;
		}

		public bool IsMinorIrritation()
		{
			return this.exposure >= 8f && this.exposure < 15f;
		}

		public bool IsMajorIrritation()
		{
			return this.exposure >= 15f;
		}

		public Element CurrentlyExposedToElement()
		{
			if (this.isAirtightSuit)
			{
				return ElementLoader.GetElement(SimHashes.Oxygen.CreateTag());
			}
			int num = Grid.CellAbove(Grid.PosToCell(base.smi.gameObject));
			return Grid.Element[num];
		}

		public void ResetExposure()
		{
			this.exposure = 0f;
		}

		[Serialize]
		public float exposure;

		[Serialize]
		public float lastReactTime;

		[Serialize]
		public float exposureRate;

		public Effects effects;

		public bool isAirtightSuit;
	}
}
