using System;
using STRINGS;

namespace Database
{
	public class ClothingItems : ResourceSet<ClothingItemResource>
	{
		public ClothingItems(ResourceSet parent)
			: base("ClothingItems", parent)
		{
			base.Initialize();
			foreach (ClothingItems.Info info in ClothingItems.Infos)
			{
				this.Add(info.id, info.name, info.desc, info.category, info.rarity, info.animFile);
			}
		}

		public void Add(string id, string name, string desc, PermitCategory category, PermitRarity rarity, string animFile)
		{
			ClothingItemResource clothingItemResource = new ClothingItemResource(id, name, desc, category, rarity, animFile);
			this.resources.Add(clothingItemResource);
		}

		public static ClothingItems.Info[] Infos = new ClothingItems.Info[]
		{
			new ClothingItems.Info("TopBasicBlack", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_BLACK.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_BLACK.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_black_kanim"),
			new ClothingItems.Info("TopBasicWhite", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_WHITE.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_white_kanim"),
			new ClothingItems.Info("TopBasicRed", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_RED_BURNT.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_RED_BURNT.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_red_kanim"),
			new ClothingItems.Info("TopBasicOrange", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_ORANGE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_ORANGE.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_orange_kanim"),
			new ClothingItems.Info("TopBasicYellow", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_YELLOW.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_YELLOW.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_yellow_kanim"),
			new ClothingItems.Info("TopBasicGreen", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_GREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_GREEN.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_green_kanim"),
			new ClothingItems.Info("TopBasicAqua", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_BLUE_MIDDLE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_BLUE_MIDDLE.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_blue_middle_kanim"),
			new ClothingItems.Info("TopBasicPurple", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_PURPLE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_PURPLE.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_purple_kanim"),
			new ClothingItems.Info("TopBasicPinkOrchid", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_PINK_ORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BASIC_PINK_ORCHID.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_basic_pink_orchid_kanim"),
			new ClothingItems.Info("BottomBasicBlack", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_BLACK.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_BLACK.DESC, PermitCategory.DupeBottoms, PermitRarity.Universal, "pants_basic_black_kanim"),
			new ClothingItems.Info("BottomBasicWhite", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_WHITE.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_white_kanim"),
			new ClothingItems.Info("BottomBasicRed", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_RED.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_RED.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_red_kanim"),
			new ClothingItems.Info("BottomBasicOrange", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_ORANGE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_ORANGE.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_orange_kanim"),
			new ClothingItems.Info("BottomBasicYellow", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_YELLOW.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_YELLOW.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_yellow_kanim"),
			new ClothingItems.Info("BottomBasicGreen", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_GREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_GREEN.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_green_kanim"),
			new ClothingItems.Info("BottomBasicAqua", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_BLUE_MIDDLE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_BLUE_MIDDLE.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_blue_middle_kanim"),
			new ClothingItems.Info("BottomBasicPurple", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_PURPLE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_PURPLE.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_purple_kanim"),
			new ClothingItems.Info("BottomBasicPinkOrchid", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_PINK_ORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_PINK_ORCHID.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_pink_orchid_kanim"),
			new ClothingItems.Info("GlovesBasicBlack", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_BLACK.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_BLACK.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_black_kanim"),
			new ClothingItems.Info("GlovesBasicWhite", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_WHITE.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_white_kanim"),
			new ClothingItems.Info("GlovesBasicRed", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_RED.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_RED.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_red_kanim"),
			new ClothingItems.Info("GlovesBasicOrange", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_ORANGE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_ORANGE.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_orange_kanim"),
			new ClothingItems.Info("GlovesBasicYellow", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_YELLOW.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_YELLOW.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_yellow_kanim"),
			new ClothingItems.Info("GlovesBasicGreen", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_GREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_GREEN.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_green_kanim"),
			new ClothingItems.Info("GlovesBasicAqua", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_BLUE_MIDDLE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_BLUE_MIDDLE.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_blue_middle_kanim"),
			new ClothingItems.Info("GlovesBasicPurple", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_PURPLE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_PURPLE.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_purple_kanim"),
			new ClothingItems.Info("GlovesBasicPinkOrchid", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_PINK_ORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_PINK_ORCHID.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_pink_orchid_kanim"),
			new ClothingItems.Info("ShoesBasicBlack", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_BLACK.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_BLACK.DESC, PermitCategory.DupeShoes, PermitRarity.Universal, "shoes_basic_black_kanim"),
			new ClothingItems.Info("ShoesBasicWhite", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_WHITE.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_white_kanim"),
			new ClothingItems.Info("ShoesBasicRed", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_RED.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_RED.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_red_kanim"),
			new ClothingItems.Info("ShoesBasicOrange", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_ORANGE.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_ORANGE.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_orange_kanim"),
			new ClothingItems.Info("ShoesBasicYellow", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_YELLOW.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_YELLOW.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_yellow_kanim"),
			new ClothingItems.Info("ShoesBasicGreen", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_GREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_GREEN.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_green_kanim"),
			new ClothingItems.Info("ShoesBasicAqua", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_BLUE_MIDDLE.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_BLUE_MIDDLE.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_blue_middle_kanim"),
			new ClothingItems.Info("ShoesBasicPurple", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_PURPLE.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_PURPLE.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_purple_kanim"),
			new ClothingItems.Info("ShoesBasicPinkOrchid", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_PINK_ORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_PINK_ORCHID.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_pink_orchid_kanim")
		};

		public struct Info
		{
			public Info(string id, string name, string desc, PermitCategory category, PermitRarity rarity, string animFile)
			{
				this.id = id;
				this.name = name;
				this.desc = desc;
				this.category = category;
				this.rarity = rarity;
				this.animFile = animFile;
			}

			public string id;

			public string name;

			public string desc;

			public PermitCategory category;

			public PermitRarity rarity;

			public string animFile;
		}
	}
}
