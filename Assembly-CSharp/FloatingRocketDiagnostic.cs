using System;
using STRINGS;

public class FloatingRocketDiagnostic : ColonyDiagnostic
{
	public FloatingRocketDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.ALL_NAME)
	{
		this.icon = "icon_errand_rocketry";
	}

	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public override ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(base.worldID);
		Clustercraft component = world.gameObject.GetComponent<Clustercraft>();
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, base.NO_MINIONS, null);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out diagnosticResult))
		{
			return diagnosticResult;
		}
		diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
		if (world.ParentWorldId == 255 || world.ParentWorldId == world.id)
		{
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_FLIGHT;
			if (component.Destination == component.Location)
			{
				bool flag = false;
				foreach (Ref<RocketModuleCluster> @ref in component.ModuleInterface.ClusterModules)
				{
					ResourceHarvestModule.StatesInstance smi = @ref.Get().GetSMI<ResourceHarvestModule.StatesInstance>();
					if (smi != null && smi.IsInsideState(smi.sm.not_grounded.drilling))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
					diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_UTILITY;
				}
				else
				{
					diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion;
					diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.WARNING_NO_DESTINATION;
				}
			}
			else if (component.Speed == 0f)
			{
				diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
				diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.WARNING_NO_SPEED;
			}
		}
		else
		{
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_LANDED;
		}
		return diagnosticResult;
	}
}
