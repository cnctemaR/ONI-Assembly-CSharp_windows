using System;
using System.Collections.Generic;
using UnityEngine;

public interface IConverterByproduct
{
	Tag ByproductAssociatedInputTag { get; }

	Tag ByproductTag { get; }

	float ByproductRate { get; }

	bool ByproductIsContinuous { get; }

	void GetByproductDescriptors(GameObject go, List<Descriptor> descriptors);
}
