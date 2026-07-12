using System;
using System.Collections.Generic;
using System.Linq;

namespace TUNING
{
	public class STORAGEFILTERS
	{
		public static List<Tag> FOOD = new List<Tag>
		{
			GameTags.Edible,
			GameTags.CookingIngredient,
			GameTags.Medicine
		};

		public static List<Tag> BAGABLE_CREATURES = new List<Tag> { GameTags.BagableCreature };

		public static List<Tag> SWIMMING_CREATURES = new List<Tag> { GameTags.SwimmingCreature };

		public static List<Tag> NOT_EDIBLE_SOLIDS = new List<Tag>
		{
			GameTags.Alloy,
			GameTags.RefinedMetal,
			GameTags.Metal,
			GameTags.BuildableRaw,
			GameTags.BuildableProcessed,
			GameTags.Farmable,
			GameTags.Organics,
			GameTags.Compostable,
			GameTags.Seed,
			GameTags.Agriculture,
			GameTags.Filter,
			GameTags.ConsumableOre,
			GameTags.Liquifiable,
			GameTags.IndustrialProduct,
			GameTags.IndustrialIngredient,
			GameTags.MedicalSupplies,
			GameTags.Clothes,
			GameTags.ManufacturedMaterial,
			GameTags.Egg,
			GameTags.RareMaterials,
			GameTags.Other,
			GameTags.StoryTraitResource
		};

		public static List<Tag> LIQUIDS = new List<Tag> { GameTags.Liquid };

		public static List<Tag> GASES = new List<Tag>
		{
			GameTags.Breathable,
			GameTags.Unbreathable
		};

		public static List<Tag> PAYLOADS = new List<Tag> { "RailGunPayload" };

		public static Tag[] SOLID_TRANSFER_ARM_CONVEYABLE = new List<Tag>
		{
			GameTags.Seed,
			GameTags.CropSeed
		}.Concat<Tag>(STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat<Tag>(STORAGEFILTERS.FOOD).Concat<Tag>(STORAGEFILTERS.PAYLOADS)).ToArray<Tag>();
	}
}
