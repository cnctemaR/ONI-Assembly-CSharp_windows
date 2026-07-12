using System;
using STRINGS;
using UnityEngine;

public class BionicBatteryDiagnostic : BionicColonyDiagnostic
{
	public BionicBatteryDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.BIONICBATTERYDIAGNOSTIC.ALL_NAME)
	{
		this.tracker = TrackerTool.Instance.GetWorldTracker<ElectrobankJoulesTracker>(worldID);
		this.icon = "BionicPower";
		this.trackerSampleCountSeconds = 150f;
		this.presentationSetting = ColonyDiagnostic.PresentationSetting.CurrentValue;
		base.AddCriterion("CheckEnoughBatteries", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BIONICBATTERYDIAGNOSTIC.CRITERIA.CHECKENOUGHBATTERIES, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckEnoughBatteries)));
		base.AddCriterion("CheckPowerLevel", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BIONICBATTERYDIAGNOSTIC.CRITERIA.CHECKPOWERLEVEL, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckPowerLevel)));
		BionicBatteryMonitor.WattageModifier difficultyModifier = BionicBatteryMonitor.GetDifficultyModifier();
		this.multiplier = (difficultyModifier.value + 200f) / 200f;
		this.recommendedJoulesPerBionic = 480000f * this.multiplier;
		this.bionicJoulesPerCycle = 120000f * this.multiplier;
	}

	private ColonyDiagnostic.DiagnosticResult CheckEnoughBatteries()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		if (this.tracker.GetDataTimeLength() < 10f)
		{
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
		}
		else if ((float)this.bionics.Count * this.recommendedJoulesPerBionic > this.tracker.GetAverageValue(this.trackerSampleCountSeconds))
		{
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
			float currentValue = this.tracker.GetCurrentValue();
			float num = this.bionicJoulesPerCycle * (float)this.bionics.Count;
			string formattedJoules = GameUtil.GetFormattedJoules(currentValue, "F1", GameUtil.TimeSlice.None);
			string formattedJoules2 = GameUtil.GetFormattedJoules(Mathf.Abs(num), "F1", GameUtil.TimeSlice.None);
			string text = UI.COLONY_DIAGNOSTICS.BIONICBATTERYDIAGNOSTIC.CRITERIA_BATTERIES.LOW_POWERBANKS;
			text = text.Replace("{0}", formattedJoules);
			text = text.Replace("{1}", formattedJoules2);
			diagnosticResult.Message = text;
		}
		return diagnosticResult;
	}

	private ColonyDiagnostic.DiagnosticResult CheckPowerLevel()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		foreach (MinionIdentity minionIdentity in this.bionics)
		{
			if (!minionIdentity.isNull)
			{
				BionicBatteryMonitor.Instance smi = minionIdentity.GetSMI<BionicBatteryMonitor.Instance>();
				if (!smi.IsNullOrStopped())
				{
					if (smi.IsInsideState(smi.sm.online.critical) && diagnosticResult.opinion != ColonyDiagnostic.DiagnosticResult.Opinion.Bad)
					{
						diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
						diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.BIONICBATTERYDIAGNOSTIC.CRITERIA_POWERLEVEL.CRITICAL_MODE;
						diagnosticResult.clickThroughTarget = new global::Tuple<Vector3, GameObject>(smi.gameObject.transform.position, smi.gameObject);
					}
					if (smi.IsInsideState(smi.sm.offline))
					{
						diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Bad;
						diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.BIONICBATTERYDIAGNOSTIC.CRITERIA_POWERLEVEL.POWERLESS;
						diagnosticResult.clickThroughTarget = new global::Tuple<Vector3, GameObject>(smi.gameObject.transform.position, smi.gameObject);
					}
				}
			}
		}
		return diagnosticResult;
	}

	public override string GetCurrentValueString()
	{
		return GameUtil.GetFormattedJoules(this.tracker.GetCurrentValue(), "F1", GameUtil.TimeSlice.None);
	}

	protected override string GetDefaultResultMessage()
	{
		return UI.COLONY_DIAGNOSTICS.BIONICBATTERYDIAGNOSTIC.CRITERIA_BATTERIES.PASS;
	}

	private float bionicJoulesPerCycle;

	private float recommendedJoulesPerBionic;

	private float multiplier = 1f;
}
