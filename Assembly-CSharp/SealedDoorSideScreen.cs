using System;
using UnityEngine;

public class SealedDoorSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.button.onClick += delegate
		{
			this.target.OrderUnseal();
		};
		this.Refresh();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Door>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		Door component = target.GetComponent<Door>();
		if (component == null)
		{
			global::Debug.LogError("Target doesn't have a Door associated with it.");
			return;
		}
		this.target = component;
		this.Refresh();
	}

	private void Refresh()
	{
		if (!this.target.isSealed)
		{
			this.ContentContainer.SetActive(false);
			return;
		}
		this.ContentContainer.SetActive(true);
	}

	[SerializeField]
	private LocText label;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private Door target;
}
