using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Geyser : StateMachineComponent<Geyser.StatesInstance>, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		if (this.configuration == null || this.configuration.typeId == HashedString.Invalid)
		{
			this.configuration = base.GetComponent<GeyserConfigurator>().MakeConfiguration();
		}
		this.emitter.emitRange = 2;
		this.emitter.maxPressure = this.configuration.GetMaxPressure();
		ElementEmitter elementEmitter = this.emitter;
		float emitRate = this.configuration.GetEmitRate();
		SimHashes element = this.configuration.GetElement();
		float temperature = this.configuration.GetTemperature();
		float num = (float)this.outputOffset.x;
		float num2 = (float)this.outputOffset.y;
		elementEmitter.outputElement = new ElementConverter.OutputElement(emitRate, element, temperature, false, false, num, num2, 1f, this.configuration.GetDiseaseIdx(), Mathf.RoundToInt((float)this.configuration.GetDiseaseCount() * this.configuration.GetEmitRate()));
		base.smi.StartSM();
		Workable component = base.GetComponent<Studyable>();
		if (component != null)
		{
			component.alwaysShowProgressBar = true;
		}
	}

	public float RemainingPhaseTimeFrom2(float onDuration, float offDuration, float time, Geyser.Phase expectedPhase)
	{
		float num = onDuration + offDuration;
		float num2 = time % num;
		float num3;
		Geyser.Phase phase;
		if (num2 < onDuration)
		{
			num3 = Mathf.Max(onDuration - num2, 0f);
			phase = Geyser.Phase.On;
		}
		else
		{
			num3 = Mathf.Max(onDuration + offDuration - num2, 0f);
			phase = Geyser.Phase.Off;
		}
		return (expectedPhase != Geyser.Phase.Any && phase != expectedPhase) ? 0f : num3;
	}

	public float RemainingPhaseTimeFrom4(float onDuration, float pstDuration, float offDuration, float preDuration, float time, Geyser.Phase expectedPhase)
	{
		float num = onDuration + pstDuration + offDuration + preDuration;
		float num2 = time % num;
		float num3;
		Geyser.Phase phase;
		if (num2 < onDuration)
		{
			num3 = onDuration - num2;
			phase = Geyser.Phase.On;
		}
		else if (num2 < onDuration + pstDuration)
		{
			num3 = onDuration + pstDuration - num2;
			phase = Geyser.Phase.Pst;
		}
		else if (num2 < onDuration + pstDuration + offDuration)
		{
			num3 = onDuration + pstDuration + offDuration - num2;
			phase = Geyser.Phase.Off;
		}
		else
		{
			num3 = onDuration + pstDuration + offDuration + preDuration - num2;
			phase = Geyser.Phase.Pre;
		}
		return (expectedPhase != Geyser.Phase.Any && phase != expectedPhase) ? 0f : num3;
	}

	private float IdleDuration()
	{
		return this.configuration.GetOffDuration() * 0.85f;
	}

	private float PreDuration()
	{
		return this.configuration.GetOffDuration() * 0.1f;
	}

	private float PostDuration()
	{
		return this.configuration.GetOffDuration() * 0.05f;
	}

	private float EruptDuration()
	{
		return this.configuration.GetOnDuration();
	}

	public bool ShouldGoDormant()
	{
		return this.RemainingActiveTime() <= 0f;
	}

	public float RemainingIdleTime()
	{
		return this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), GameClock.Instance.GetTime(), Geyser.Phase.Off);
	}

	public float RemainingEruptPreTime()
	{
		return this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), GameClock.Instance.GetTime(), Geyser.Phase.Pre);
	}

	public float RemainingEruptTime()
	{
		return this.RemainingPhaseTimeFrom2(this.configuration.GetOnDuration(), this.configuration.GetOffDuration(), GameClock.Instance.GetTime(), Geyser.Phase.On);
	}

	public float RemainingEruptPostTime()
	{
		return this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), GameClock.Instance.GetTime(), Geyser.Phase.Pst);
	}

	public float RemainingNonEruptTime()
	{
		return this.RemainingPhaseTimeFrom2(this.configuration.GetOnDuration(), this.configuration.GetOffDuration(), GameClock.Instance.GetTime(), Geyser.Phase.Off);
	}

	public float RemainingDormantTime()
	{
		return this.RemainingPhaseTimeFrom2(this.configuration.GetYearOnDuration(), this.configuration.GetYearOffDuration(), GameClock.Instance.GetTime(), Geyser.Phase.Off);
	}

	public float RemainingActiveTime()
	{
		return this.RemainingPhaseTimeFrom2(this.configuration.GetYearOnDuration(), this.configuration.GetYearOffDuration(), GameClock.Instance.GetTime(), Geyser.Phase.On);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(this.configuration.GetElement());
		string text = element.tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_PRODUCTION, text, GameUtil.GetFormattedMass(this.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(this.configuration.GetTemperature(), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION, this.configuration.GetElement().ToString(), GameUtil.GetFormattedMass(this.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(this.configuration.GetTemperature(), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect, false));
		if (this.configuration.GetDiseaseIdx() != 255)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_DISEASE, GameUtil.GetFormattedDiseaseName(this.configuration.GetDiseaseIdx(), false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_DISEASE, GameUtil.GetFormattedDiseaseName(this.configuration.GetDiseaseIdx(), false)), Descriptor.DescriptorType.Effect, false));
		}
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_PERIOD, GameUtil.GetFormattedTime(this.configuration.GetOnDuration()), GameUtil.GetFormattedTime(this.configuration.GetIterationLength())), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PERIOD, GameUtil.GetFormattedTime(this.configuration.GetOnDuration()), GameUtil.GetFormattedTime(this.configuration.GetIterationLength())), Descriptor.DescriptorType.Effect, false));
		Studyable component = base.GetComponent<Studyable>();
		if (component && !component.Studied)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_UNSTUDIED, new object[0]), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_UNSTUDIED, new object[0]), Descriptor.DescriptorType.Effect, false));
		}
		else
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_PERIOD, GameUtil.GetFormattedCycles(this.configuration.GetYearOnDuration(), "F1"), GameUtil.GetFormattedCycles(this.configuration.GetYearLength(), "F1")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_PERIOD, GameUtil.GetFormattedCycles(this.configuration.GetYearOnDuration(), "F1"), GameUtil.GetFormattedCycles(this.configuration.GetYearLength(), "F1")), Descriptor.DescriptorType.Effect, false));
			if (base.smi.IsInsideState(base.smi.sm.dormant))
			{
				list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_ACTIVE, GameUtil.GetFormattedCycles(this.RemainingDormantTime(), "F1")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_ACTIVE, GameUtil.GetFormattedCycles(this.RemainingDormantTime(), "F1")), Descriptor.DescriptorType.Effect, false));
			}
			else
			{
				list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_DORMANT, GameUtil.GetFormattedCycles(this.RemainingActiveTime(), "F1")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_DORMANT, GameUtil.GetFormattedCycles(this.RemainingActiveTime(), "F1")), Descriptor.DescriptorType.Effect, false));
			}
		}
		return list;
	}

	[MyCmpAdd]
	private ElementEmitter emitter;

	[Serialize]
	public GeyserConfigurator.GeyserInstanceConfiguration configuration;

	public Vector2I outputOffset;

	private const float PRE_PCT = 0.1f;

	private const float POST_PCT = 0.05f;

	public class StatesInstance : GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.GameInstance
	{
		public StatesInstance(Geyser smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			base.serializable = true;
			this.root.DefaultState(this.idle).Enter(delegate(Geyser.StatesInstance smi)
			{
				smi.master.emitter.SetEmitting(false);
			});
			this.dormant.PlayAnim("inactive", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutDormant, null).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.RemainingDormantTime(), this.pre_erupt);
			this.idle.PlayAnim("inactive", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutIdle, null).Enter(delegate(Geyser.StatesInstance smi)
			{
				if (smi.master.ShouldGoDormant())
				{
					smi.GoTo(this.dormant);
				}
			})
				.ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.RemainingIdleTime(), this.pre_erupt);
			this.pre_erupt.PlayAnim("shake", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding, null).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.RemainingEruptPreTime(), this.erupt);
			this.erupt.DefaultState(this.erupt.erupting).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.RemainingEruptTime(), this.post_erupt).Enter(delegate(Geyser.StatesInstance smi)
			{
				smi.master.emitter.SetEmitting(true);
			})
				.Exit(delegate(Geyser.StatesInstance smi)
				{
					smi.master.emitter.SetEmitting(false);
				});
			this.erupt.erupting.EventTransition(GameHashes.EmitterBlocked, this.erupt.overpressure, (Geyser.StatesInstance smi) => smi.GetComponent<ElementEmitter>().isEmitterBlocked).PlayAnim("erupt", KAnim.PlayMode.Loop);
			this.erupt.overpressure.EventTransition(GameHashes.EmitterUnblocked, this.erupt.erupting, (Geyser.StatesInstance smi) => !smi.GetComponent<ElementEmitter>().isEmitterBlocked).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure, null).PlayAnim("inactive", KAnim.PlayMode.Loop);
			this.post_erupt.PlayAnim("shake", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutIdle, null).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.RemainingEruptPostTime(), this.idle);
		}

		public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State dormant;

		public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State idle;

		public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State pre_erupt;

		public Geyser.States.EruptState erupt;

		public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State post_erupt;

		public class EruptState : GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State
		{
			public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State erupting;

			public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State overpressure;
		}
	}

	public enum Phase
	{
		Pre,
		On,
		Pst,
		Off,
		Any
	}
}
