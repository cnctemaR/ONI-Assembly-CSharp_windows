using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Automatable")]
public class Automatable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<Automatable>(-905833192, Automatable.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		Automatable component = ((GameObject)data).GetComponent<Automatable>();
		if (component != null)
		{
			this.automationOnly = component.automationOnly;
		}
	}

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

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<Automatable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Automatable>(delegate(Automatable component, object data)
	{
		component.OnCopySettings(data);
	});
}
