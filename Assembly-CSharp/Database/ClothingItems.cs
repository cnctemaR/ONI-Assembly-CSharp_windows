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
			foreach (ClothingItems.Info info in ClothingItems.Infos_All)
			{
				this.Add(info.id, info.name, info.desc, info.category, info.rarity, info.animFile);
			}
		}

		public void Add(string id, string name, string desc, PermitCategory category, PermitRarity rarity, string animFile)
		{
			ClothingItemResource clothingItemResource = new ClothingItemResource(id, name, desc, category, rarity, animFile);
			this.resources.Add(clothingItemResource);
		}

		public ClothingItemResource TryResolveAccessoryResource(ResourceGuid AccessoryGuid)
		{
			if (AccessoryGuid.Guid != null)
			{
				string[] array = AccessoryGuid.Guid.Split(new char[] { '.' });
				if (array.Length != 0)
				{
					string symbol_name = array[array.Length - 1];
					return this.resources.Find((ClothingItemResource ci) => symbol_name.Contains(ci.Id));
				}
			}
			return null;
		}

		public static ClothingItems.Info[] Infos_Skins = new ClothingItems.Info[]
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
			new ClothingItems.Info("ShoesBasicPinkOrchid", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_PINK_ORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_PINK_ORCHID.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_pink_orchid_kanim"),
			new ClothingItems.Info("TopRaglanDeepRed", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_DEEPRED.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_DEEPRED.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_raglan_deepred_kanim"),
			new ClothingItems.Info("TopRaglanCobalt", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_COBALT.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_COBALT.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_raglan_cobalt_kanim"),
			new ClothingItems.Info("TopRaglanFlamingo", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_FLAMINGO.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_FLAMINGO.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_raglan_flamingo_kanim"),
			new ClothingItems.Info("TopRaglanKellyGreen", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_KELLYGREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_KELLYGREEN.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_raglan_kellygreen_kanim"),
			new ClothingItems.Info("TopRaglanCharcoal", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_CHARCOAL.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_CHARCOAL.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_raglan_charcoal_kanim"),
			new ClothingItems.Info("TopRaglanLemon", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_LEMON.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_LEMON.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_raglan_lemon_kanim"),
			new ClothingItems.Info("TopRaglanSatsuma", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.RAGLANTOP_SATSUMA.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_raglan_satsuma_kanim"),
			new ClothingItems.Info("ShortsBasicDeepRed", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_DEEPRED.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_DEEPRED.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "shorts_basic_deepred_kanim"),
			new ClothingItems.Info("ShortsBasicSatsuma", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_SATSUMA.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "shorts_basic_satsuma_kanim"),
			new ClothingItems.Info("ShortsBasicYellowcake", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_YELLOWCAKE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_YELLOWCAKE.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "shorts_basic_yellowcake_kanim"),
			new ClothingItems.Info("ShortsBasicKellyGreen", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_KELLYGREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_KELLYGREEN.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "shorts_basic_kellygreen_kanim"),
			new ClothingItems.Info("ShortsBasicBlueCobalt", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_BLUE_COBALT.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_BLUE_COBALT.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "shorts_basic_blue_cobalt_kanim"),
			new ClothingItems.Info("ShortsBasicPinkFlamingo", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_PINK_FLAMINGO.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_PINK_FLAMINGO.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "shorts_basic_pink_flamingo_kanim"),
			new ClothingItems.Info("ShortsBasicCharcoal", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_CHARCOAL.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SHORTS_BASIC_CHARCOAL.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "shorts_basic_charcoal_kanim"),
			new ClothingItems.Info("SocksAthleticDeepRed", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_DEEPRED.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_DEEPRED.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "socks_athletic_red_deep_kanim"),
			new ClothingItems.Info("SocksAthleticOrangeSatsuma", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_SATSUMA.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "socks_athletic_orange_satsuma_kanim"),
			new ClothingItems.Info("SocksAthleticYellowLemon", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_LEMON.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_LEMON.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "socks_athletic_yellow_lemon_kanim"),
			new ClothingItems.Info("SocksAthleticGreenKelly", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_KELLYGREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_KELLYGREEN.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "socks_athletic_green_kelly_kanim"),
			new ClothingItems.Info("SocksAthleticBlueCobalt", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_COBALT.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_COBALT.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "socks_athletic_blue_cobalt_kanim"),
			new ClothingItems.Info("SocksAthleticPinkFlamingo", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_FLAMINGO.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_FLAMINGO.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "socks_athletic_pink_flamingo_kanim"),
			new ClothingItems.Info("SocksAthleticGreyCharcoal", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_CHARCOAL.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.SOCKS_ATHLETIC_CHARCOAL.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "socks_athletic_grey_charcoal_kanim"),
			new ClothingItems.Info("GlovesAthleticRedDeep", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_DEEPRED.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_DEEPRED.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_athletic_red_deep_kanim"),
			new ClothingItems.Info("GlovesAthleticOrangeSatsuma", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_SATSUMA.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_athletic_orange_satsuma_kanim"),
			new ClothingItems.Info("GlovesAthleticYellowLemon", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_LEMON.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_LEMON.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_athletic_yellow_lemon_kanim"),
			new ClothingItems.Info("GlovesAthleticGreenKelly", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_KELLYGREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_KELLYGREEN.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_athletic_green_kelly_kanim"),
			new ClothingItems.Info("GlovesAthleticBlueCobalt", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_COBALT.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_COBALT.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_athletic_blue_cobalt_kanim"),
			new ClothingItems.Info("GlovesAthleticPinkFlamingo", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_FLAMINGO.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_FLAMINGO.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_athletic_pink_flamingo_kanim"),
			new ClothingItems.Info("GlovesAthleticGreyCharcoal", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_CHARCOAL.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_CHARCOAL.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_athletic_grey_charcoal_kanim")
		};

		public static ClothingItems.Info[] Infos_All = ClothingItems.Infos_Skins;

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
