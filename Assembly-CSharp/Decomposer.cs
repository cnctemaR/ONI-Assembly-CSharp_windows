using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Decomposer")]
public class Decomposer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		StateMachineController component = base.GetComponent<StateMachineController>();
		if (component == null)
		{
			return;
		}
		DecompositionMonitor.Instance instance = new DecompositionMonitor.Instance(this, null, 1f, false);
		component.AddStateMachineInstance(instance);
		instance.StartSM();
		instance.dirtyWaterMaxRange = 3;
	}
}
