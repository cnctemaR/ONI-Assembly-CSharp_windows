using System;
using UnityEngine;

public interface IYieldEffect
{
	Crop.CropVal ApplyToCropVal(GameObject plant, Crop.CropVal modifiedVal, Crop.CropVal originalVal);

	SeedProducer.SeedInfo ApplyToSeed(GameObject plant, SeedProducer.SeedInfo modifiedVal, SeedProducer.SeedInfo originalVal);

	void ApplyToCrop(GameObject plant, GameObject crop);

	Descriptor[] GetDescription(GameObject plant);

	void ApplyToTransformation(Crop crop, EconomyDetails economy_details, EconomyDetails.Transformation transformation);
}
