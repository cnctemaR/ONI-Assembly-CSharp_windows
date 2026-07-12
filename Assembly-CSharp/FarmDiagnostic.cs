using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FarmDiagnostic : ColonyDiagnostic
{
	public FarmDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.ALL_NAME)
	{
		this.icon = "icon_errand_farm";
		base.AddCriterion("CheckHasFarms", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKHASFARMS, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckHasFarms)));
		base.AddCriterion("CheckPlanted", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKPLANTED, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckPlanted)));
		base.AddCriterion("CheckWilting", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKWILTING, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckWilting)));
		base.AddCriterion("CheckOperational", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKOPERATIONAL, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckOperational)));
	}

	private void RefreshPlots()
	{
		this.plots = Components.PlantablePlots.GetItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed));
	}

	private ColonyDiagnostic.DiagnosticResult CheckHasFarms()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		if (this.plots.Count == 0)
		{
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE;
		}
		return diagnosticResult;
	}

	private ColonyDiagnostic.DiagnosticResult CheckPlanted()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		bool flag = false;
		using (List<PlantablePlot>.Enumerator enumerator = this.plots.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.plant != null)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE_PLANTED;
		}
		return diagnosticResult;
	}

	private ColonyDiagnostic.DiagnosticResult CheckWilting()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		foreach (PlantablePlot plantablePlot in this.plots)
		{
			if (plantablePlot.plant != null && plantablePlot.plant.HasTag(GameTags.Wilting))
			{
				StandardCropPlant component = plantablePlot.plant.GetComponent<StandardCropPlant>();
				if (component != null && component.smi.IsInsideState(component.smi.sm.alive.wilting) && component.smi.timeinstate > 15f)
				{
					diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
					diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.WILTING;
					diagnosticResult.clickThroughTarget = new global::Tuple<Vector3, GameObject>(plantablePlot.transform.position, plantablePlot.gameObject);
					break;
				}
			}
		}
		return diagnosticResult;
	}

	private ColonyDiagnostic.DiagnosticResult CheckOperational()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		foreach (PlantablePlot plantablePlot in this.plots)
		{
			if (plantablePlot.plant != null && !plantablePlot.HasTag(GameTags.Operational))
			{
				diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
				diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.INOPERATIONAL;
				diagnosticResult.clickThroughTarget = new global::Tuple<Vector3, GameObject>(plantablePlot.transform.position, plantablePlot.gameObject);
				break;
			}
		}
		return diagnosticResult;
	}

	public override string GetAverageValueString()
	{
		if (this.plots == null)
		{
			this.RefreshPlots();
		}
		return TrackerTool.Instance.GetWorldTracker<CropTracker>(base.worldID).GetCurrentValue().ToString() + "/" + this.plots.Count.ToString();
	}

	public override ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS, null);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out diagnosticResult))
		{
			return diagnosticResult;
		}
		this.RefreshPlots();
		return base.Evaluate();
	}

	private List<PlantablePlot> plots;
}
