using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SelfChargingElectrobankDiagnostic : ColonyDiagnostic
{
	public SelfChargingElectrobankDiagnostic(int worldID)
		: base(worldID, UI.SELFCHARGINGBATTERYDIAGNOSTIC.ALL_NAME)
	{
		this.icon = "overlay_radiation";
		base.AddCriterion("CheckLifetime", new DiagnosticCriterion(UI.SELFCHARGINGBATTERYDIAGNOSTIC.CRITERIA.CHECKSELFCHARGINGBATTERYLIFE, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckLifetime)));
	}

	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1.Concat<string>(DlcManager.DLC3);
	}

	private ColonyDiagnostic.DiagnosticResult CheckLifetime()
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.SELFCHARGINGBATTERYDIAGNOSTIC.NORMAL, null);
		foreach (SelfChargingElectrobank selfChargingElectrobank in Components.SelfChargingElectrobanks.GetItems(base.worldID))
		{
			if (selfChargingElectrobank.LifetimeRemaining <= this.WARNING_LIFETIME)
			{
				diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
				if (diagnosticResult.clickThroughObjects == null)
				{
					diagnosticResult.clickThroughObjects = new List<GameObject>();
				}
				diagnosticResult.clickThroughObjects.Add(selfChargingElectrobank.gameObject);
				diagnosticResult.Message = UI.SELFCHARGINGBATTERYDIAGNOSTIC.CRITERIA_BATTERYLIFE_WARNING;
			}
		}
		return diagnosticResult;
	}

	private float WARNING_LIFETIME = 600f;
}
