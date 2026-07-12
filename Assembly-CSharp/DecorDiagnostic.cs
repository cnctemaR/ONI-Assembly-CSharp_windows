using System;
using System.Collections.Generic;
using STRINGS;

public class DecorDiagnostic : ColonyDiagnostic
{
	public DecorDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.DECORDIAGNOSTIC.ALL_NAME)
	{
		this.icon = "icon_category_decor";
		base.AddCriterion("CheckDecor", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.DECORDIAGNOSTIC.CRITERIA.CHECKDECOR, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckDecor)));
	}

	private ColonyDiagnostic.DiagnosticResult CheckDecor()
	{
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID, false);
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS, null);
		if (worldItems.Count == 0)
		{
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
			diagnosticResult.Message = base.NO_MINIONS;
		}
		return diagnosticResult;
	}

	public override ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, base.NO_MINIONS, null);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out diagnosticResult))
		{
			return diagnosticResult;
		}
		return base.Evaluate();
	}
}
