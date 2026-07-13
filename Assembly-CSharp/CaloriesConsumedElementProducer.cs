using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CaloriesConsumedElementProducer : KMonoBehaviour, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		new CaloriesConsumedSecondaryExcretionMonitor.Instance(base.gameObject.GetComponent<StateMachineController>())
		{
			sm = 
			{
				producedElement = this.producedElement
			},
			sm = 
			{
				kgProducedPerKcalConsumed = this.kgProducedPerKcalConsumed
			}
		}.StartSM();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.DIET_ADDITIONAL_PRODUCED.Replace("{Items}", ElementLoader.GetElement(this.producedElement.CreateTag()).name), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_ADDITIONAL_PRODUCED.Replace("{Items}", ElementLoader.GetElement(this.producedElement.CreateTag()).name), Descriptor.DescriptorType.Effect, false)
		};
	}

	public SimHashes producedElement;

	public float kgProducedPerKcalConsumed = 1f;
}
