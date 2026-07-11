using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UserNameable")]
public class UserNameable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (string.IsNullOrEmpty(this.savedName))
		{
			this.SetName(base.gameObject.GetProperName());
			return;
		}
		this.SetName(this.savedName);
	}

	public void SetName(string name)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		base.name = name;
		if (component != null)
		{
			component.SetName(name);
		}
		base.gameObject.name = name;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
		this.savedName = name;
		base.Trigger(1102426921, name);
	}

	[Serialize]
	public string savedName = "";
}
