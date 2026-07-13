using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ReactorDiagnostic : ColonyDiagnostic
{
	public ReactorDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.ALL_NAME)
	{
		this.icon = "overlay_radiation";
		base.AddCriterion("CheckTemperature", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA.CHECKTEMPERATURE, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckTemperature)));
		base.AddCriterion("CheckCoolant", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA.CHECKCOOLANT, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckCoolant)));
	}

	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	private ColonyDiagnostic.DiagnosticResult CheckTemperature()
	{
		List<Reactor> worldItems = Components.NuclearReactors.GetWorldItems(base.worldID, false);
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
		diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.NORMAL;
		foreach (Reactor reactor in worldItems)
		{
			if (reactor.FuelTemperature > 1254.8625f)
			{
				diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Warning;
				diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA_TEMPERATURE_WARNING;
				diagnosticResult.clickThroughTarget = new global::Tuple<Vector3, GameObject>(reactor.gameObject.transform.position, reactor.gameObject);
			}
		}
		return diagnosticResult;
	}

	private ColonyDiagnostic.DiagnosticResult CheckCoolant()
	{
		List<Reactor> worldItems = Components.NuclearReactors.GetWorldItems(base.worldID, false);
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
		diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.NORMAL;
		foreach (Reactor reactor in worldItems)
		{
			if (reactor.On && reactor.ReserveCoolantMass <= 45f)
			{
				diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
				diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA_COOLANT_WARNING;
				diagnosticResult.clickThroughTarget = new global::Tuple<Vector3, GameObject>(reactor.gameObject.transform.position, reactor.gameObject);
			}
		}
		return diagnosticResult;
	}
}
