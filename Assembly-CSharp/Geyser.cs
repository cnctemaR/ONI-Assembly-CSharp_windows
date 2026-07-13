using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Geyser : StateMachineComponent<Geyser.StatesInstance>, IGameObjectEffectDescriptor
{
	public float timeShift { get; private set; }

	public float GetCurrentLifeTime()
	{
		return GameClock.Instance.GetTime() + this.timeShift;
	}

	public void AlterTime(float timeOffset, bool shouldSurviveSaveLoad = false)
	{
		this.timeShift = Mathf.Max(timeOffset, -GameClock.Instance.GetTime());
		if (shouldSurviveSaveLoad)
		{
			this.serializedTimeShift = this.timeShift;
		}
		float num = this.RemainingEruptTime();
		float num2 = this.RemainingNonEruptTime();
		float num3 = this.RemainingActiveTime();
		float num4 = this.RemainingDormantTime();
		this.configuration.GetYearLength();
		if (num2 == 0f)
		{
			if ((num4 == 0f && this.configuration.GetYearOnDuration() - num3 < this.configuration.GetOnDuration() - num) | (num3 == 0f && this.configuration.GetYearOffDuration() - num4 >= this.configuration.GetOnDuration() - num))
			{
				base.smi.GoTo(base.smi.sm.dormant);
				return;
			}
			base.smi.GoTo(base.smi.sm.erupt);
			return;
		}
		else
		{
			bool flag = (num4 == 0f && this.configuration.GetYearOnDuration() - num3 < this.configuration.GetIterationLength() - num2) | (num3 == 0f && this.configuration.GetYearOffDuration() - num4 >= this.configuration.GetIterationLength() - num2);
			float num5 = this.RemainingEruptPreTime();
			if (flag && num5 <= 0f)
			{
				base.smi.GoTo(base.smi.sm.dormant);
				return;
			}
			if (num5 <= 0f)
			{
				base.smi.GoTo(base.smi.sm.idle);
				return;
			}
			float num6 = this.PreDuration() - num5;
			if ((num3 == 0f) ? (this.configuration.GetYearOffDuration() - num4 > num6) : (num6 > this.configuration.GetYearOnDuration() - num3))
			{
				base.smi.GoTo(base.smi.sm.dormant);
				return;
			}
			base.smi.GoTo(base.smi.sm.pre_erupt);
			return;
		}
	}

	public void ShiftTimeTo(Geyser.TimeShiftStep step, bool shouldSurviveSaveLoad = false)
	{
		float num = this.RemainingEruptTime();
		float num2 = this.RemainingNonEruptTime();
		float num3 = this.RemainingActiveTime();
		float num4 = this.RemainingDormantTime();
		float yearLength = this.configuration.GetYearLength();
		switch (step)
		{
		case Geyser.TimeShiftStep.ActiveState:
		{
			float num5 = ((num3 > 0f) ? (this.configuration.GetYearOnDuration() - num3) : (yearLength - num4));
			this.AlterTime(this.timeShift - num5, shouldSurviveSaveLoad);
			return;
		}
		case Geyser.TimeShiftStep.DormantState:
		{
			float num6 = ((num3 > 0f) ? num3 : (-(this.configuration.GetYearOffDuration() - num4)));
			this.AlterTime(this.timeShift + num6, shouldSurviveSaveLoad);
			return;
		}
		case Geyser.TimeShiftStep.NextIteration:
		{
			float num7 = ((num > 0f) ? (num + this.configuration.GetOffDuration()) : num2);
			this.AlterTime(this.timeShift + num7, shouldSurviveSaveLoad);
			return;
		}
		case Geyser.TimeShiftStep.PreviousIteration:
		{
			float num8 = ((num > 0f) ? (-(this.configuration.GetOnDuration() - num)) : (-(this.configuration.GetIterationLength() - num2)));
			if (num > 0f && Mathf.Abs(num8) < this.configuration.GetOnDuration() * 0.05f)
			{
				num8 -= this.configuration.GetIterationLength();
			}
			this.AlterTime(this.timeShift + num8, shouldSurviveSaveLoad);
			return;
		}
		default:
			return;
		}
	}

	public void AddModification(Geyser.GeyserModification modification)
	{
		this.modifications.Add(modification);
		this.UpdateModifier();
	}

	public void RemoveModification(Geyser.GeyserModification modification)
	{
		this.modifications.Remove(modification);
		this.UpdateModifier();
	}

	private void UpdateModifier()
	{
		this.modifier.Clear();
		foreach (Geyser.GeyserModification geyserModification in this.modifications)
		{
			this.modifier.AddValues(geyserModification);
		}
		this.configuration.SetModifier(this.modifier);
		this.ApplyConfigurationEmissionValues(this.configuration);
		this.RefreshGeotunerFeedback();
	}

	public void RefreshGeotunerFeedback()
	{
		this.RefreshGeotunerStatusItem();
		this.RefreshStudiedMeter();
	}

	private void RefreshGeotunerStatusItem()
	{
		KSelectable component = base.gameObject.GetComponent<KSelectable>();
		if (this.GetAmountOfGeotunersPointingThisGeyser() > 0)
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.GeyserGeotuned, this);
			return;
		}
		component.RemoveStatusItem(Db.Get().BuildingStatusItems.GeyserGeotuned, this);
	}

	private void RefreshStudiedMeter()
	{
		if (this.studyable.Studied)
		{
			bool flag = this.GetAmountOfGeotunersPointingThisGeyser() > 0;
			GeyserConfig.TrackerMeterAnimNames trackerMeterAnimNames = GeyserConfig.TrackerMeterAnimNames.tracker;
			if (flag)
			{
				trackerMeterAnimNames = GeyserConfig.TrackerMeterAnimNames.geotracker;
				int amountOfGeotunersAffectingThisGeyser = this.GetAmountOfGeotunersAffectingThisGeyser();
				if (amountOfGeotunersAffectingThisGeyser > 0)
				{
					trackerMeterAnimNames = GeyserConfig.TrackerMeterAnimNames.geotracker_minor;
				}
				if (amountOfGeotunersAffectingThisGeyser >= 5)
				{
					trackerMeterAnimNames = GeyserConfig.TrackerMeterAnimNames.geotracker_major;
				}
			}
			this.studyable.studiedIndicator.meterController.Play(trackerMeterAnimNames.ToString(), KAnim.PlayMode.Loop, 1f, 0f);
		}
	}

	public int GetAmountOfGeotunersPointingThisGeyser()
	{
		return Components.GeoTuners.GetItems(base.gameObject.GetMyWorldId()).Count<GeoTuner.Instance>((GeoTuner.Instance x) => x.GetAssignedGeyser() == this);
	}

	public int GetAmountOfGeotunersPointingOrWillPointAtThisGeyser()
	{
		return Components.GeoTuners.GetItems(base.gameObject.GetMyWorldId()).Count<GeoTuner.Instance>((GeoTuner.Instance x) => x.GetAssignedGeyser() == this || x.GetFutureGeyser() == this);
	}

	public int GetAmountOfGeotunersAffectingThisGeyser()
	{
		int num = 0;
		for (int i = 0; i < this.modifications.Count; i++)
		{
			if (this.modifications[i].originID.Contains("GeoTuner"))
			{
				num++;
			}
		}
		return num;
	}

	private void OnGeotunerChanged(object o)
	{
		this.RefreshGeotunerFeedback();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		if (this.configuration == null || this.configuration.typeId == HashedString.Invalid)
		{
			this.configuration = base.GetComponent<GeyserConfigurator>().MakeConfiguration();
		}
		else
		{
			PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
			if (this.configuration.geyserType.geyserTemperature - component.Temperature != 0f)
			{
				SimTemperatureTransfer component2 = base.gameObject.GetComponent<SimTemperatureTransfer>();
				component2.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Combine(component2.onSimRegistered, new Action<SimTemperatureTransfer>(this.OnSimRegistered));
			}
		}
		this.ApplyConfigurationEmissionValues(this.configuration);
		base.GetComponent<CodexEntryRedirector>().CodexID = "GEYSERGENERIC" + this.configuration.geyserType.id.ToUpper();
		this.GenerateName();
		this.timeShift = this.serializedTimeShift;
		base.smi.StartSM();
		Workable component3 = base.GetComponent<Studyable>();
		if (component3 != null)
		{
			component3.alwaysShowProgressBar = true;
		}
		Components.Geysers.Add(base.gameObject.GetMyWorldId(), this);
		base.gameObject.Subscribe(1763323737, new Action<object>(this.OnGeotunerChanged));
		this.RefreshStudiedMeter();
		this.UpdateModifier();
	}

	private void GenerateName()
	{
		StringKey stringKey = new StringKey("STRINGS.CREATURES.SPECIES.GEYSER." + this.configuration.geyserType.id.ToUpper() + ".NAME");
		if (this.nameable.savedName == Strings.Get(stringKey))
		{
			int num = Grid.PosToCell(base.gameObject);
			Quadrant[] quadrantOfCell = base.gameObject.GetMyWorld().GetQuadrantOfCell(num, 2);
			int num2 = (int)quadrantOfCell[0];
			string text = num2.ToString();
			num2 = (int)quadrantOfCell[1];
			string text2 = text + num2.ToString();
			string[] array = NAMEGEN.GEYSER_IDS.IDs.ToString().Split('\n', StringSplitOptions.None);
			string text3 = array[global::UnityEngine.Random.Range(0, array.Length)];
			string text4 = string.Concat(new string[]
			{
				UI.StripLinkFormatting(base.gameObject.GetProperName()),
				" ",
				text3,
				text2,
				"‑",
				global::UnityEngine.Random.Range(0, 10).ToString()
			});
			this.nameable.SetName(text4);
		}
	}

	public void ApplyConfigurationEmissionValues(GeyserConfigurator.GeyserInstanceConfiguration config)
	{
		this.emitter.emitRange = 2;
		this.emitter.maxPressure = config.GetMaxPressure();
		this.emitter.outputElement = new ElementConverter.OutputElement(config.GetEmitRate(), config.GetElement(), config.GetTemperature(), false, false, (float)this.outputOffset.x, (float)this.outputOffset.y, 1f, config.GetDiseaseIdx(), Mathf.RoundToInt((float)config.GetDiseaseCount() * config.GetEmitRate()), true);
		if (this.emitter.IsSimActive)
		{
			this.emitter.SetSimActive(true);
		}
	}

	public void Unentomb()
	{
		OccupyArea component = base.GetComponent<OccupyArea>();
		int num = Grid.PosToCell(this);
		foreach (CellOffset cellOffset in component.OccupiedCellsOffsets)
		{
			int num2 = Grid.OffsetCell(num, cellOffset);
			if (Grid.IsSolidCell(num2) && Grid.Element[num2].id != SimHashes.Unobtanium)
			{
				SimMessages.Dig(num2, -1, false);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		base.gameObject.Unsubscribe(1763323737, new Action<object>(this.OnGeotunerChanged));
		Components.Geysers.Remove(base.gameObject.GetMyWorldId(), this);
	}

	private void OnSimRegistered(SimTemperatureTransfer stt)
	{
		PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
		if (this.configuration.geyserType.geyserTemperature - component.Temperature != 0f)
		{
			component.Temperature = this.configuration.geyserType.geyserTemperature;
		}
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(this.OnSimRegistered));
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
		if (expectedPhase != Geyser.Phase.Any && phase != expectedPhase)
		{
			return 0f;
		}
		return num3;
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
		if (expectedPhase != Geyser.Phase.Any && phase != expectedPhase)
		{
			return 0f;
		}
		return num3;
	}

	private float IdleDuration()
	{
		return this.configuration.GetOffDuration() * 0.84999996f;
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
		return this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Off);
	}

	public float RemainingEruptPreTime()
	{
		return this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Pre);
	}

	public float RemainingEruptTime()
	{
		return this.RemainingPhaseTimeFrom2(this.configuration.GetOnDuration(), this.configuration.GetOffDuration(), this.GetCurrentLifeTime(), Geyser.Phase.On);
	}

	public float RemainingEruptPostTime()
	{
		return this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Pst);
	}

	public float RemainingNonEruptTime()
	{
		return this.RemainingPhaseTimeFrom2(this.configuration.GetOnDuration(), this.configuration.GetOffDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Off);
	}

	public float RemainingDormantTime()
	{
		return this.RemainingPhaseTimeFrom2(this.configuration.GetYearOnDuration(), this.configuration.GetYearOffDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Off);
	}

	public float RemainingActiveTime()
	{
		return this.RemainingPhaseTimeFrom2(this.configuration.GetYearOnDuration(), this.configuration.GetYearOffDuration(), this.GetCurrentLifeTime(), Geyser.Phase.On);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		string text = ElementLoader.FindElementByHash(this.configuration.GetElement()).tag.ProperName();
		List<GeoTuner.Instance> items = Components.GeoTuners.GetItems(base.gameObject.GetMyWorldId());
		GeoTuner.Instance instance = items.Find((GeoTuner.Instance g) => g.GetAssignedGeyser() == this);
		int num = items.Count<GeoTuner.Instance>((GeoTuner.Instance x) => x.GetAssignedGeyser() == this);
		bool flag = num > 0;
		string text2 = string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION, ElementLoader.FindElementByHash(this.configuration.GetElement()).name, GameUtil.GetFormattedMass(this.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(this.configuration.GetTemperature(), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
		if (flag)
		{
			Func<float, float> func = delegate(float emissionPerCycleModifier)
			{
				float num8 = 600f / this.configuration.GetIterationLength();
				return emissionPerCycleModifier / num8 / this.configuration.GetOnDuration();
			};
			int amountOfGeotunersAffectingThisGeyser = this.GetAmountOfGeotunersAffectingThisGeyser();
			float num2 = ((Geyser.temperatureModificationMethod == Geyser.ModificationMethod.Percentages) ? (instance.currentGeyserModification.temperatureModifier * this.configuration.geyserType.temperature) : instance.currentGeyserModification.temperatureModifier);
			float num3 = func((Geyser.massModificationMethod == Geyser.ModificationMethod.Percentages) ? (instance.currentGeyserModification.massPerCycleModifier * this.configuration.scaledRate) : instance.currentGeyserModification.massPerCycleModifier);
			float num4 = (float)amountOfGeotunersAffectingThisGeyser * num2;
			float num5 = (float)amountOfGeotunersAffectingThisGeyser * num3;
			string text3 = ((num4 > 0f) ? "+" : "") + GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true, false);
			string text4 = ((num5 > 0f) ? "+" : "") + GameUtil.GetFormattedMass(num5, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}");
			string text5 = ((num2 > 0f) ? "+" : "") + GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true, false);
			string text6 = ((num3 > 0f) ? "+" : "") + GameUtil.GetFormattedMass(num3, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}");
			text2 = string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED, ElementLoader.FindElementByHash(this.configuration.GetElement()).name, GameUtil.GetFormattedMass(this.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(this.configuration.GetTemperature(), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			text2 += "\n";
			text2 = text2 + "\n" + string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED_COUNT, amountOfGeotunersAffectingThisGeyser.ToString(), num.ToString());
			text2 += "\n";
			text2 = text2 + "\n" + string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED_TOTAL, text4, text3);
			for (int i = 0; i < amountOfGeotunersAffectingThisGeyser; i++)
			{
				string text7 = "\n    • " + UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_GEOTUNER_MODIFIER_ROW_TITLE.ToString();
				text7 = text7 + text6 + " " + text5;
				text2 += text7;
			}
		}
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_PRODUCTION, text, GameUtil.GetFormattedMass(this.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(this.configuration.GetTemperature(), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), text2, Descriptor.DescriptorType.Effect, false));
		if (this.configuration.GetDiseaseIdx() != 255)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_DISEASE, GameUtil.GetFormattedDiseaseName(this.configuration.GetDiseaseIdx(), false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_DISEASE, GameUtil.GetFormattedDiseaseName(this.configuration.GetDiseaseIdx(), false)), Descriptor.DescriptorType.Effect, false));
		}
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_PERIOD, GameUtil.GetFormattedTime(this.configuration.GetOnDuration(), "F0"), GameUtil.GetFormattedTime(this.configuration.GetIterationLength(), "F0")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PERIOD, GameUtil.GetFormattedTime(this.configuration.GetOnDuration(), "F0"), GameUtil.GetFormattedTime(this.configuration.GetIterationLength(), "F0")), Descriptor.DescriptorType.Effect, false));
		Studyable component = base.GetComponent<Studyable>();
		if (component && !component.Studied)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_UNSTUDIED, Array.Empty<object>()), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_UNSTUDIED, Array.Empty<object>()), Descriptor.DescriptorType.Effect, false));
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_AVR_OUTPUT_UNSTUDIED, Array.Empty<object>()), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_UNSTUDIED, Array.Empty<object>()), Descriptor.DescriptorType.Effect, false));
		}
		else
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_PERIOD, GameUtil.GetFormattedCycles(this.configuration.GetYearOnDuration(), "F1", false), GameUtil.GetFormattedCycles(this.configuration.GetYearLength(), "F1", false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_PERIOD, GameUtil.GetFormattedCycles(this.configuration.GetYearOnDuration(), "F1", false), GameUtil.GetFormattedCycles(this.configuration.GetYearLength(), "F1", false)), Descriptor.DescriptorType.Effect, false));
			if (base.smi.IsInsideState(base.smi.sm.dormant))
			{
				list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_ACTIVE, GameUtil.GetFormattedCycles(this.RemainingDormantTime(), "F1", false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_ACTIVE, GameUtil.GetFormattedCycles(this.RemainingDormantTime(), "F1", false)), Descriptor.DescriptorType.Effect, false));
			}
			else
			{
				list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_DORMANT, GameUtil.GetFormattedCycles(this.RemainingActiveTime(), "F1", false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_DORMANT, GameUtil.GetFormattedCycles(this.RemainingActiveTime(), "F1", false)), Descriptor.DescriptorType.Effect, false));
			}
			string text8 = UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT.Replace("{average}", GameUtil.GetFormattedMass(this.configuration.GetAverageEmission(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{element}", this.configuration.geyserType.element.CreateTag().ProperName());
			if (flag)
			{
				text8 += "\n";
				text8 = text8 + "\n" + UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_BREAKDOWN_TITLE;
				int amountOfGeotunersAffectingThisGeyser2 = this.GetAmountOfGeotunersAffectingThisGeyser();
				float num6 = ((Geyser.massModificationMethod == Geyser.ModificationMethod.Percentages) ? (instance.currentGeyserModification.massPerCycleModifier * 100f) : (instance.currentGeyserModification.massPerCycleModifier * 100f / this.configuration.scaledRate));
				float num7 = num6 * (float)amountOfGeotunersAffectingThisGeyser2;
				text8 = text8 + GameUtil.AddPositiveSign(num7.ToString("0.0"), num7 > 0f) + "%";
				for (int j = 0; j < amountOfGeotunersAffectingThisGeyser2; j++)
				{
					string text9 = "\n    • " + UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_BREAKDOWN_ROW.ToString();
					text9 = text9 + GameUtil.AddPositiveSign(num6.ToString("0.0"), num6 > 0f) + "%";
					text8 += text9;
				}
			}
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_AVR_OUTPUT, GameUtil.GetFormattedMass(this.configuration.GetAverageEmission(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), text8, Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	public static Geyser.ModificationMethod massModificationMethod = Geyser.ModificationMethod.Percentages;

	public static Geyser.ModificationMethod temperatureModificationMethod = Geyser.ModificationMethod.Values;

	public static Geyser.ModificationMethod IterationDurationModificationMethod = Geyser.ModificationMethod.Percentages;

	public static Geyser.ModificationMethod IterationPercentageModificationMethod = Geyser.ModificationMethod.Percentages;

	public static Geyser.ModificationMethod yearDurationModificationMethod = Geyser.ModificationMethod.Percentages;

	public static Geyser.ModificationMethod yearPercentageModificationMethod = Geyser.ModificationMethod.Percentages;

	public static Geyser.ModificationMethod maxPressureModificationMethod = Geyser.ModificationMethod.Percentages;

	[MyCmpAdd]
	private ElementEmitter emitter;

	[MyCmpAdd]
	private UserNameable nameable;

	[MyCmpGet]
	private Studyable studyable;

	[Serialize]
	public GeyserConfigurator.GeyserInstanceConfiguration configuration;

	public Vector2I outputOffset;

	public List<Geyser.GeyserModification> modifications = new List<Geyser.GeyserModification>();

	private Geyser.GeyserModification modifier;

	[Serialize]
	private float serializedTimeShift;

	private const float PRE_PCT = 0.1f;

	private const float POST_PCT = 0.05f;

	public enum ModificationMethod
	{
		Values,
		Percentages
	}

	public struct GeyserModification
	{
		public void Clear()
		{
			this.massPerCycleModifier = 0f;
			this.temperatureModifier = 0f;
			this.iterationDurationModifier = 0f;
			this.iterationPercentageModifier = 0f;
			this.yearDurationModifier = 0f;
			this.yearPercentageModifier = 0f;
			this.maxPressureModifier = 0f;
			this.modifyElement = false;
			this.newElement = (SimHashes)0;
		}

		public void AddValues(Geyser.GeyserModification modification)
		{
			this.massPerCycleModifier += modification.massPerCycleModifier;
			this.temperatureModifier += modification.temperatureModifier;
			this.iterationDurationModifier += modification.iterationDurationModifier;
			this.iterationPercentageModifier += modification.iterationPercentageModifier;
			this.yearDurationModifier += modification.yearDurationModifier;
			this.yearPercentageModifier += modification.yearPercentageModifier;
			this.maxPressureModifier += modification.maxPressureModifier;
			this.modifyElement |= modification.modifyElement;
			this.newElement = ((modification.newElement == (SimHashes)0) ? this.newElement : modification.newElement);
		}

		public bool IsNewElementInUse()
		{
			return this.modifyElement && this.newElement > (SimHashes)0;
		}

		public string originID;

		public float massPerCycleModifier;

		public float temperatureModifier;

		public float iterationDurationModifier;

		public float iterationPercentageModifier;

		public float yearDurationModifier;

		public float yearPercentageModifier;

		public float maxPressureModifier;

		public bool modifyElement;

		public SimHashes newElement;
	}

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
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
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
			this.erupt.TriggerOnEnter(GameHashes.GeyserEruption, (Geyser.StatesInstance smi) => true).TriggerOnExit(GameHashes.GeyserEruption, (Geyser.StatesInstance smi) => false).DefaultState(this.erupt.erupting)
				.ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.RemainingEruptTime(), this.post_erupt)
				.Enter(delegate(Geyser.StatesInstance smi)
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

	public enum TimeShiftStep
	{
		ActiveState,
		DormantState,
		NextIteration,
		PreviousIteration
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
