using System;
using UnityEngine;

public class NoConfigSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		return false;
	}
}
