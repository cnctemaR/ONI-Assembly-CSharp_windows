using System;
using UnityEngine;

public class CritterSensorSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.countCrittersToggle.onClick += this.ToggleCritters;
		this.countEggsToggle.onClick += this.ToggleEggs;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LogicCritterCountSensor>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetSensor = target.GetComponent<LogicCritterCountSensor>();
		this.crittersCheckmark.enabled = this.targetSensor.countCritters;
		this.eggsCheckmark.enabled = this.targetSensor.countEggs;
	}

	private void ToggleCritters()
	{
		this.targetSensor.countCritters = !this.targetSensor.countCritters;
		this.crittersCheckmark.enabled = this.targetSensor.countCritters;
	}

	private void ToggleEggs()
	{
		this.targetSensor.countEggs = !this.targetSensor.countEggs;
		this.eggsCheckmark.enabled = this.targetSensor.countEggs;
	}

	public LogicCritterCountSensor targetSensor;

	public KToggle countCrittersToggle;

	public KToggle countEggsToggle;

	public KImage crittersCheckmark;

	public KImage eggsCheckmark;
}
