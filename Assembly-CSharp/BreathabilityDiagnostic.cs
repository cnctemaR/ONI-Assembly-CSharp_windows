using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BreathabilityDiagnostic : ColonyDiagnostic
{
	public BreathabilityDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.ALL_NAME)
	{
		this.tracker = TrackerTool.Instance.GetWorldTracker<BreathabilityTracker>(worldID);
		this.trackerSampleCountSeconds = 50f;
		this.icon = "overlay_oxygen";
		base.AddCriterion("CheckSuffocation", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.CRITERIA.CHECKSUFFOCATION, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckSuffocation)));
		base.AddCriterion("CheckLowBreathability", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.CRITERIA.CHECKLOWBREATHABILITY, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckLowBreathability)));
	}

	private ColonyDiagnostic.DiagnosticResult CheckSuffocation()
	{
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID, false);
		if (worldItems.Count != 0)
		{
			using (List<MinionIdentity>.Enumerator enumerator = worldItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MinionIdentity minionIdentity = enumerator.Current;
					minionIdentity.GetComponent<OxygenBreather>().GetGasProvider();
					SuffocationMonitor.Instance smi = minionIdentity.GetSMI<SuffocationMonitor.Instance>();
					if (smi != null && smi.IsInsideState(smi.sm.nooxygen.suffocating))
					{
						return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.SUFFOCATING, new global::Tuple<Vector3, GameObject>(smi.transform.position, smi.gameObject));
					}
				}
				goto IL_00A7;
			}
			goto IL_0099;
			IL_00A7:
			return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.NORMAL, null);
		}
		IL_0099:
		return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, base.NO_MINIONS, null);
	}

	private ColonyDiagnostic.DiagnosticResult CheckLowBreathability()
	{
		if (Components.LiveMinionIdentities.GetWorldItems(base.worldID, false).Count != 0 && this.tracker.GetAverageValue(this.trackerSampleCountSeconds) < 60f)
		{
			return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Concern, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.POOR, null);
		}
		return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.NORMAL, null);
	}

	public override ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult;
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out diagnosticResult))
		{
			return diagnosticResult;
		}
		return base.Evaluate();
	}
}
