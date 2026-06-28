using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectEffectDescriptor
{
	int DescriptionOrder { get; set; }

	List<Descriptor> GetRequirementDescriptions(GameObject go);

	List<string> GetEffectDescriptions(GameObject go);
}
