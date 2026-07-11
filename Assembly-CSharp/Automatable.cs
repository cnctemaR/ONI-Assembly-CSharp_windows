using System;
using KSerialization;

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

	public bool AllowedByAutomation(bool is_transfer_arm)
	{
		return !this.GetAutomationOnly() || is_transfer_arm;
	}

	[Serialize]
	private bool automationOnly = true;
}
