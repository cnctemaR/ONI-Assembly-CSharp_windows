using System;
using System.Collections.Generic;

namespace TUNING
{
	public class STORAGEFILTERS
	{
		public static List<Tag> FOOD = new List<Tag> { GameTags.Edible };

		public static List<Tag> NOT_EDIBLE = new List<Tag>
		{
			GameTags.Alloy,
			GameTags.RefinedMetal,
			GameTags.Metal,
			GameTags.BuildableRaw,
			GameTags.BuildableProcessed,
			GameTags.Farmable,
			GameTags.Organics,
			GameTags.Seed,
			GameTags.Filter,
			GameTags.ConsumableOre
		};

		public static List<Tag> LIQUIDS = new List<Tag> { GameTags.Liquid };

		public static List<Tag> DEFAULT = new List<Tag>
		{
			GameTags.Alloy,
			GameTags.RefinedMetal,
			GameTags.Metal,
			GameTags.BuildableRaw,
			GameTags.BuildableProcessed,
			GameTags.Farmable,
			GameTags.Organics,
			GameTags.Seed,
			GameTags.Filter,
			GameTags.ConsumableOre
		};
	}
}
