using System;
using UnityEngine;

public class MinionVitalsScreen : TargetScreen
{
	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
	}

	public override void OnSelectTarget(GameObject target)
	{
		this.panel.selectedEntity = target;
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
			this.panel = base.GetComponent<MinionVitalsPanel>();
		}
		this.panel.Init();
	}

	public MinionVitalsPanel panel;
}
