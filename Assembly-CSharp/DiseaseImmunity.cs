using System;
using Klei.AI;
using KSerialization;
using TUNING;

public class DiseaseImmunity : KMonoBehaviour
{
	public float ExposureModifier
	{
		get
		{
			return this.exposureModifier;
		}
	}

	protected override void OnPrefabInit()
	{
		Traits component = base.GetComponent<Traits>();
		if (!component.HasTrait("StrongImmuneSystem"))
		{
			this.ModifyDiseaseExposure(TRAITS.STRONGIMMUNESYSTEM_MODIFIER);
		}
		else if (!component.HasTrait("WeakImmuneSystem"))
		{
			this.ModifyDiseaseExposure(TRAITS.WEAKIMMUNESYSTEM_MODIFIER);
		}
	}

	public void ModifyDiseaseExposure(float modifier)
	{
		this.exposureModifier *= modifier;
	}

	[Serialize]
	private float exposureModifier = 1f;
}
