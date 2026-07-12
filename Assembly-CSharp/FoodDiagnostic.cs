using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FoodDiagnostic : ColonyDiagnostic
{
	public FoodDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.ALL_NAME)
	{
		this.tracker = TrackerTool.Instance.GetWorldTracker<KCalTracker>(worldID);
		this.icon = "icon_category_food";
		this.trackerSampleCountSeconds = 150f;
		this.presentationSetting = ColonyDiagnostic.PresentationSetting.CurrentValue;
		base.AddCriterion("CheckEnoughFood", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA.CHECKENOUGHFOOD, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckEnoughFood)));
		base.AddCriterion("CheckStarvation", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA.CHECKSTARVATION, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckStarvation)));
	}

	private ColonyDiagnostic.DiagnosticResult CheckAnyFood()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA_HAS_FOOD.PASS, null);
		if (Components.LiveMinionIdentities.GetWorldItems(base.worldID, false).Count != 0)
		{
			if (this.tracker.GetDataTimeLength() < 10f)
			{
				diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
				diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
			}
			else if (this.tracker.GetAverageValue(this.trackerSampleCountSeconds) == 0f)
			{
				diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Bad;
				diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA_HAS_FOOD.FAIL;
			}
		}
		return diagnosticResult;
	}

	private ColonyDiagnostic.DiagnosticResult CheckEnoughFood()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID, false);
		if (this.tracker.GetDataTimeLength() < 10f)
		{
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
		}
		else
		{
			int num = 3000;
			if ((float)worldItems.Count * (1000f * (float)num) > this.tracker.GetAverageValue(this.trackerSampleCountSeconds))
			{
				diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
				float currentValue = this.tracker.GetCurrentValue();
				float num2 = (float)Components.LiveMinionIdentities.GetWorldItems(base.worldID, false).Count * -1000000f;
				string formattedCalories = GameUtil.GetFormattedCalories(currentValue, GameUtil.TimeSlice.None, true);
				string formattedCalories2 = GameUtil.GetFormattedCalories(Mathf.Abs(num2), GameUtil.TimeSlice.None, true);
				string text = MISC.NOTIFICATIONS.FOODLOW.TOOLTIP;
				text = text.Replace("{0}", formattedCalories);
				text = text.Replace("{1}", formattedCalories2);
				diagnosticResult.Message = text;
			}
		}
		return diagnosticResult;
	}

	private ColonyDiagnostic.DiagnosticResult CheckStarvation()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.GetWorldItems(base.worldID, false))
		{
			if (!minionIdentity.IsNull())
			{
				CalorieMonitor.Instance smi = minionIdentity.GetSMI<CalorieMonitor.Instance>();
				if (!smi.IsNullOrStopped() && smi.IsInsideState(smi.sm.hungry.starving))
				{
					diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Bad;
					diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.HUNGRY;
					diagnosticResult.clickThroughTarget = new global::Tuple<Vector3, GameObject>(smi.gameObject.transform.position, smi.gameObject);
				}
			}
		}
		return diagnosticResult;
	}

	public override string GetCurrentValueString()
	{
		return GameUtil.GetFormattedCalories(this.tracker.GetCurrentValue(), GameUtil.TimeSlice.None, true);
	}

	public override ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS, null);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out diagnosticResult))
		{
			return diagnosticResult;
		}
		return base.Evaluate();
	}
}
