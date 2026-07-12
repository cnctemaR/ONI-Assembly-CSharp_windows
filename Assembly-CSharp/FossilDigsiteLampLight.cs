using System;
using System.Collections.Generic;
using UnityEngine;

public class FossilDigsiteLampLight : Light2D
{
	public bool independent { get; private set; }

	protected override void OnPrefabInit()
	{
		base.Subscribe<FossilDigsiteLampLight>(-592767678, FossilDigsiteLampLight.OnOperationalChangedDelegate);
		base.IntensityAnimation = 1f;
	}

	public void SetIndependentState(bool isIndependent, bool checkOperational = true)
	{
		this.independent = isIndependent;
		Operational component = base.GetComponent<Operational>();
		if (component != null && this.independent && checkOperational && base.enabled != component.IsOperational)
		{
			base.enabled = component.IsOperational;
		}
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		if (this.independent || base.enabled)
		{
			return base.GetDescriptors(go);
		}
		return new List<Descriptor>();
	}

	private static readonly EventSystem.IntraObjectHandler<FossilDigsiteLampLight> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<FossilDigsiteLampLight>(delegate(FossilDigsiteLampLight light, object data)
	{
		if (light.independent)
		{
			light.enabled = (bool)data;
		}
	});
}
