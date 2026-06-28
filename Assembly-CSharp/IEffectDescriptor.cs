using System;
using System.Collections.Generic;

public interface IEffectDescriptor
{
	int DescriptionOrder { get; set; }

	List<Descriptor> GetRequirementDescriptions(BuildingDef def);

	List<Descriptor> GetEffectDescriptions(BuildingDef def);
}
