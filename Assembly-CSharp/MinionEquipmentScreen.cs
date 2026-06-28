using System;
using UnityEngine;

public class MinionEquipmentScreen : TargetScreen
{
	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
	}

	public override void OnSelectTarget(GameObject target)
	{
		this.panel.SetSelectedMinion(target);
		this.panel.Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if (this.panel == null)
		{
			this.panel = base.GetComponent<MinionEquipmentPanel>();
		}
	}

	public MinionEquipmentPanel panel;
}
