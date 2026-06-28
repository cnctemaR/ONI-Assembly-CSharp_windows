using System;
using STRINGS;
using UnityEngine;

public class YieldEffect
{
	public class AddHarvestUnitsMultiple : IYieldEffect
	{
		public AddHarvestUnitsMultiple(float modifier)
		{
			this.modifier = modifier;
		}

		public Crop.CropVal ApplyToCropVal(GameObject plant, Crop.CropVal modiFiedVal, Crop.CropVal originalVal)
		{
			Crop.CropVal cropVal = modiFiedVal;
			cropVal.numProduced = Mathf.CeilToInt((float)modiFiedVal.numProduced + (float)originalVal.numProduced * this.modifier);
			return cropVal;
		}

		public SeedProducer.SeedInfo ApplyToSeed(GameObject plant, SeedProducer.SeedInfo modifiedVal, SeedProducer.SeedInfo originalVal)
		{
			return modifiedVal;
		}

		public void ApplyToCrop(GameObject plant, GameObject crop)
		{
		}

		public Descriptor[] GetDescription(GameObject plant)
		{
			Crop component = plant.GetComponent<Crop>();
			Tag tag = new Tag(component.cropVal.cropId);
			GameObject prefab = Assets.GetPrefab(tag);
			int num = Mathf.CeilToInt((float)component.cropVal.numProduced * this.modifier) + component.cropVal.numProduced;
			return new Descriptor[]
			{
				new Descriptor(string.Format(CREATURES.CROP_EFFECTS.MODIFY_HARVEST_UNITS.DESC, num, prefab.GetProperName()), string.Format(CREATURES.CROP_EFFECTS.MODIFY_HARVEST_UNITS.TOOLTIP, num, prefab.GetProperName()), Descriptor.DescriptorType.CropHarvest, false)
			};
		}

		public void ApplyToTransformation(Crop crop, EconomyDetails details, EconomyDetails.Transformation transformation)
		{
			EconomyDetails.Resource resource = details.GetResource(new Tag(crop.cropVal.cropId));
			EconomyDetails.Transformation.Delta delta = transformation.GetDelta(resource);
			delta.amount += delta.amount * this.modifier;
			EconomyDetails.Transformation.Delta delta2 = transformation.GetDelta(details.caloriesResource);
			if (delta2 != null)
			{
				delta2.amount += delta2.amount * this.modifier;
			}
		}

		private float modifier = 1f;
	}

	public class AddSeeds : IYieldEffect
	{
		public AddSeeds(int numSeeds)
		{
			this.numSeeds = numSeeds;
		}

		public Crop.CropVal ApplyToCropVal(GameObject plant, Crop.CropVal modiFiedVal, Crop.CropVal originalVal)
		{
			return modiFiedVal;
		}

		public SeedProducer.SeedInfo ApplyToSeed(GameObject plant, SeedProducer.SeedInfo modifiedVal, SeedProducer.SeedInfo originalVal)
		{
			SeedProducer.SeedInfo seedInfo = modifiedVal;
			seedInfo.newSeedsProduced += this.numSeeds;
			return seedInfo;
		}

		public void ApplyToCrop(GameObject plant, GameObject crop)
		{
		}

		public void ApplyToTransformation(Crop crop, EconomyDetails economy, EconomyDetails.Transformation transformation)
		{
			SeedProducer component = crop.GetComponent<SeedProducer>();
			EconomyDetails.Resource resource = economy.CreateResource(new Tag(component.seedInfo.seedId), economy.amountResourceType);
			EconomyDetails.Transformation.Delta delta = transformation.GetDelta(resource);
			if (delta == null)
			{
				delta = new EconomyDetails.Transformation.Delta(resource, 0f);
				transformation.AddDelta(delta);
			}
			delta.amount += (float)(component.seedInfo.newSeedsProduced + this.numSeeds);
		}

		public Descriptor[] GetDescription(GameObject plant)
		{
			SeedProducer component = plant.GetComponent<SeedProducer>();
			SeedProducer.ProductionType productionType = component.seedInfo.productionType;
			string text = "Seed";
			string text2 = string.Empty;
			string text3 = CREATURES.CROP_EFFECTS.ADD_SEEDS.TOOLTIP;
			if (component != null)
			{
				GameObject prefab = Assets.GetPrefab(new Tag(component.seedInfo.seedId));
				if (prefab != null)
				{
					text = prefab.GetProperName();
				}
			}
			if (productionType == SeedProducer.ProductionType.FinalHarvest)
			{
				text2 = CREATURES.CROP_EFFECTS.ADD_SEEDS.FINAL_HARVEST_ONLY;
				text3 = CREATURES.CROP_EFFECTS.ADD_SEEDS.TOOLTIP_FINAL_HARVEST_ONLY;
			}
			return new Descriptor[]
			{
				new Descriptor(string.Format(CREATURES.CROP_EFFECTS.ADD_SEEDS.DESC, this.numSeeds + component.seedInfo.newSeedsProduced, text, text2), string.Format(text3, this.numSeeds), Descriptor.DescriptorType.CropHarvest, false)
			};
		}

		private int numSeeds = 1;
	}
}
