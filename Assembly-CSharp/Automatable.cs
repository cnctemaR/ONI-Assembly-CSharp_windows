using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Automatable : KMonoBehaviour
{
	public bool GetAutomationOnly()
	{
		return this.automationOnly;
	}

	public void SetAutomationOnly(bool only)
	{
		this.automationOnly = only;
	}

	public bool AllowedByAutomation(GameObject worker)
	{
		return !this.GetAutomationOnly() || worker.GetComponent<SolidTransferArm>() != null;
	}

	[Serialize]
	private bool automationOnly = true;
}
