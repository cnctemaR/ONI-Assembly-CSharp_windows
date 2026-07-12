using System;
using STRINGS;

public class RocketOxidizerDiagnostic : ColonyDiagnostic
{
	public RocketOxidizerDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.ALL_NAME)
	{
		this.tracker = TrackerTool.Instance.GetWorldTracker<RocketOxidizerTracker>(worldID);
		this.icon = "rocket_oxidizer";
	}

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		Clustercraft component = ClusterManager.Instance.GetWorld(base.worldID).gameObject.GetComponent<Clustercraft>();
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS, null);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out diagnosticResult))
		{
			return diagnosticResult;
		}
		diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
		diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.NORMAL;
		RocketEngineCluster engine = component.ModuleInterface.GetEngine();
		if (component.ModuleInterface.OxidizerPowerRemaining == 0f && engine != null && engine.requireOxidizer)
		{
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.WARNING;
		}
		return diagnosticResult;
	}
}
