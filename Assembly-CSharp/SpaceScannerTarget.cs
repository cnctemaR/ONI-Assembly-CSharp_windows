using System;

public readonly struct SpaceScannerTarget
{
	private SpaceScannerTarget(string id)
	{
		this.id = id;
	}

	public static SpaceScannerTarget MeteorShower()
	{
		return new SpaceScannerTarget("meteor_shower");
	}

	public static SpaceScannerTarget BallisticObject()
	{
		return new SpaceScannerTarget("ballistic_object");
	}

	public static SpaceScannerTarget RocketBaseGame(LaunchConditionManager rocket)
	{
		return new SpaceScannerTarget(string.Format("rocket_base_game::{0}", rocket.GetComponent<KPrefabID>().InstanceID));
	}

	public static SpaceScannerTarget RocketDlc1(Clustercraft rocket)
	{
		return new SpaceScannerTarget(string.Format("rocket_dlc1::{0}", rocket.GetComponent<KPrefabID>().InstanceID));
	}

	public readonly string id;
}
