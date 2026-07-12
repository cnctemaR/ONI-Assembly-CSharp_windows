using System;
using System.Collections.Generic;

public abstract class BionicColonyDiagnostic : ColonyDiagnostic
{
	public BionicColonyDiagnostic(int worldID, string name)
		: base(worldID, name)
	{
		this.RefreshData();
	}

	protected void RefreshData()
	{
		Components.Cmps<MinionIdentity> cmps;
		if (Components.LiveMinionIdentitiesByModel.TryGetValue(BionicMinionConfig.MODEL, out cmps))
		{
			this.bionics = cmps.GetWorldItems(base.worldID, true, new Func<MinionIdentity, bool>(this.MinionFilter));
			return;
		}
		this.bionics = new List<MinionIdentity>();
	}

	protected virtual bool MinionFilter(MinionIdentity minion)
	{
		return true;
	}

	public override ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult;
		if (this.ignoreInIdleRockets && ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out diagnosticResult))
		{
			return diagnosticResult;
		}
		this.RefreshData();
		diagnosticResult = base.Evaluate();
		if (diagnosticResult.opinion == ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
		{
			diagnosticResult.Message = this.GetDefaultResultMessage();
		}
		return diagnosticResult;
	}

	protected abstract string GetDefaultResultMessage();

	protected const bool INCLUDE_CHILD_WORLDS = true;

	protected List<MinionIdentity> bionics;

	protected bool ignoreInIdleRockets = true;
}
