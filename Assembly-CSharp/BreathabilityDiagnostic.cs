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
		base.AddCriterion("CheckBionicOxygen", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.CRITERIA.CHECKLOWBIONICOXYGEN, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckLowBionicOxygen)));
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
					SuffocationMonitor.Instance smi = minionIdentity.GetSMI<SuffocationMonitor.Instance>();
					if (smi != null && smi.IsInsideState(smi.sm.noOxygen.suffocating))
					{
						return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.SUFFOCATING, new global::Tuple<Vector3, GameObject>(smi.transform.position, smi.gameObject));
					}
				}
				goto IL_009B;
			}
			goto IL_008D;
			IL_009B:
			return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.NORMAL, null);
		}
		IL_008D:
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

	private ColonyDiagnostic.DiagnosticResult CheckLowBionicOxygen()
	{
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID, false);
		if (worldItems.Count != 0)
		{
			foreach (MinionIdentity minionIdentity in worldItems)
			{
				if (minionIdentity.HasTag(GameTags.Minions.Models.Bionic))
				{
					BionicOxygenTankMonitor.Instance smi = minionIdentity.GetSMI<BionicOxygenTankMonitor.Instance>();
					if (smi.OxygenPercentage <= 0f)
					{
						return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.NEAR_OR_EMPTY_BIONIC_TANKS, new global::Tuple<Vector3, GameObject>(minionIdentity.transform.position, minionIdentity.gameObject));
					}
					if (smi.OxygenPercentage < 0.5f)
					{
						return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Concern, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.POOR_BIONIC_TANKS, new global::Tuple<Vector3, GameObject>(minionIdentity.transform.position, minionIdentity.gameObject));
					}
				}
			}
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
