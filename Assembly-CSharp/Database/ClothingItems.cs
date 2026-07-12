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
				this.Add(info.id, info.name, info.desc, info.outfitType, info.category, info.rarity, info.animFile);
			}
		}

		public void Add(string id, string name, string desc, ClothingOutfitUtility.OutfitType outfitType, PermitCategory category, PermitRarity rarity, string animFile)
		{
			ClothingItemResource clothingItemResource = new ClothingItemResource(id, name, desc, outfitType, category, rarity, animFile);
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
			new ClothingItems.Info("GlovesAthleticGreyCharcoal", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_CHARCOAL.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.GLOVES_ATHLETIC_CHARCOAL.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_athletic_grey_charcoal_kanim"),
			new ClothingItems.Info("TopJellypuffJacketBlueberry", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_BLUEBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_BLUEBERRY.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_jellypuffjacket_blueberry_kanim"),
			new ClothingItems.Info("TopJellypuffJacketGrape", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_GRAPE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_GRAPE.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_jellypuffjacket_grape_kanim"),
			new ClothingItems.Info("TopJellypuffJacketLemon", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_LEMON.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_LEMON.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_jellypuffjacket_lemon_kanim"),
			new ClothingItems.Info("TopJellypuffJacketLime", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_LIME.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_LIME.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_jellypuffjacket_lime_kanim"),
			new ClothingItems.Info("TopJellypuffJacketSatsuma", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_SATSUMA.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_jellypuffjacket_satsuma_kanim"),
			new ClothingItems.Info("TopJellypuffJacketStrawberry", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_STRAWBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_STRAWBERRY.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_jellypuffjacket_strawberry_kanim"),
			new ClothingItems.Info("TopJellypuffJacketWatermelon", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_WATERMELON.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JELLYPUFFJACKET_WATERMELON.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_jellypuffjacket_watermelon_kanim"),
			new ClothingItems.Info("GlovesCufflessBlueberry", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_BLUEBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_BLUEBERRY.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_cuffless_blueberry_kanim"),
			new ClothingItems.Info("GlovesCufflessGrape", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_GRAPE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_GRAPE.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_cuffless_grape_kanim"),
			new ClothingItems.Info("GlovesCufflessLemon", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_LEMON.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_LEMON.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_cuffless_lemon_kanim"),
			new ClothingItems.Info("GlovesCufflessLime", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_LIME.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_LIME.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_cuffless_lime_kanim"),
			new ClothingItems.Info("GlovesCufflessSatsuma", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_SATSUMA.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_cuffless_satsuma_kanim"),
			new ClothingItems.Info("GlovesCufflessStrawberry", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_STRAWBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_STRAWBERRY.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_cuffless_strawberry_kanim"),
			new ClothingItems.Info("GlovesCufflessWatermelon", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_WATERMELON.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_WATERMELON.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_cuffless_watermelon_kanim"),
			new ClothingItems.Info("visonly_AtmoHelmetClear", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Universal, "atmo_helmet_clear_kanim"),
			new ClothingItems.Info("visonly_AtmoSuitBasicBlue", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Universal, "atmosuit_basic_blue_kanim"),
			new ClothingItems.Info("visonly_AtmoGlovesBasicBlue", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Universal, "atmo_gloves_blue_kanim"),
			new ClothingItems.Info("visonly_AtmoBeltBasicBlue", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Universal, "atmo_belt_basic_blue_kanim"),
			new ClothingItems.Info("visonly_AtmoShoesBasicBlack", EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.DESC, PermitCategory.AtmoSuitShoes, PermitRarity.Universal, "atmo_shoes_basic_black_kanim"),
			new ClothingItems.Info("AtmoHelmetLimone", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.LIMONE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.LIMONE.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Universal, "atmo_helmet_limone_lime_kanim"),
			new ClothingItems.Info("AtmoSuitBasicYellow", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.LIMONE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.LIMONE.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Universal, "atmosuit_basic_yellow_kanim"),
			new ClothingItems.Info("AtmoGlovesLime", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.LIMONE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.LIMONE.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Universal, "atmo_gloves_lime_kanim"),
			new ClothingItems.Info("AtmoBeltBasicLime", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.LIMONE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.LIMONE.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Universal, "atmo_belt_basic_lime_kanim"),
			new ClothingItems.Info("AtmoShoesBasicYellow", EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.LIMONE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.LIMONE.DESC, PermitCategory.AtmoSuitShoes, PermitRarity.Universal, "atmo_shoes_basic_yellow_kanim"),
			new ClothingItems.Info("AtmoHelmetPuft", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.PUFT.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.PUFT.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Loyalty, "atmo_helmet_puft_kanim"),
			new ClothingItems.Info("AtmoSuitPuft", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.PUFT.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.PUFT.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Loyalty, "atmosuit_puft_kanim"),
			new ClothingItems.Info("AtmoGlovesPuft", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.PUFT.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.PUFT.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Loyalty, "atmo_gloves_puft_kanim"),
			new ClothingItems.Info("AtmoBeltPuft", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.PUFT.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.PUFT.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Loyalty, "atmo_belt_puft_kanim"),
			new ClothingItems.Info("AtmoShoesPuft", EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.PUFT.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.PUFT.DESC, PermitCategory.AtmoSuitShoes, PermitRarity.Loyalty, "atmo_shoes_puft_kanim"),
			new ClothingItems.Info("TopTShirtWhite", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.TSHIRT_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.TSHIRT_WHITE.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_tshirt_white_kanim"),
			new ClothingItems.Info("TopTShirtMagenta", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.TSHIRT_MAGENTA.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.TSHIRT_MAGENTA.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_tshirt_magenta_kanim"),
			new ClothingItems.Info("TopAthlete", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.ATHLETE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.ATHLETE.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_athlete_kanim"),
			new ClothingItems.Info("TopCircuitGreen", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.CIRCUIT_GREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.CIRCUIT_GREEN.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_circuit_green_kanim"),
			new ClothingItems.Info("GlovesBasicBlueGrey", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_BLUEGREY.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_BLUEGREY.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_bluegrey_kanim"),
			new ClothingItems.Info("GlovesBasicBrownKhaki", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_BROWN_KHAKI.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_BROWN_KHAKI.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_brown_khaki_kanim"),
			new ClothingItems.Info("GlovesAthlete", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.ATHLETE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.ATHLETE.DESC, PermitCategory.DupeGloves, PermitRarity.Decent, "gloves_athlete_kanim"),
			new ClothingItems.Info("GlovesCircuitGreen", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CIRCUIT_GREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CIRCUIT_GREEN.DESC, PermitCategory.DupeGloves, PermitRarity.Decent, "gloves_circuit_green_kanim"),
			new ClothingItems.Info("PantsBasicRedOrange", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_REDORANGE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_REDORANGE.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_redorange_kanim"),
			new ClothingItems.Info("PantsBasicLightBrown", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_LIGHTBROWN.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_LIGHTBROWN.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_lightbrown_kanim"),
			new ClothingItems.Info("PantsAthlete", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.ATHLETE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.ATHLETE.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "pants_athlete_kanim"),
			new ClothingItems.Info("PantsCircuitGreen", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.CIRCUIT_GREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.CIRCUIT_GREEN.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "pants_circuit_green_kanim"),
			new ClothingItems.Info("ShoesBasicBlueGrey", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_BLUEGREY.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_BLUEGREY.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_bluegrey_kanim"),
			new ClothingItems.Info("ShoesBasicTan", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_TAN.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_TAN.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_tan_kanim"),
			new ClothingItems.Info("AtmoHelmetSparkleRed", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.SPARKLE_RED.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.SPARKLE_RED.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_sparkle_red_kanim"),
			new ClothingItems.Info("AtmoHelmetSparkleGreen", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.SPARKLE_GREEN.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.SPARKLE_GREEN.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_sparkle_green_kanim"),
			new ClothingItems.Info("AtmoHelmetSparkleBlue", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.SPARKLE_BLUE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.SPARKLE_BLUE.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_sparkle_blue_kanim"),
			new ClothingItems.Info("AtmoHelmetSparklePurple", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.SPARKLE_PURPLE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.SPARKLE_PURPLE.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_sparkle_purple_kanim"),
			new ClothingItems.Info("AtmoSuitSparkleRed", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.SPARKLE_RED.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.SPARKLE_RED.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Splendid, "atmosuit_sparkle_red_kanim"),
			new ClothingItems.Info("AtmoSuitSparkleGreen", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.SPARKLE_GREEN.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.SPARKLE_GREEN.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Splendid, "atmosuit_sparkle_green_kanim"),
			new ClothingItems.Info("AtmoSuitSparkleBlue", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.SPARKLE_BLUE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.SPARKLE_BLUE.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Splendid, "atmosuit_sparkle_blue_kanim"),
			new ClothingItems.Info("AtmoSuitSparkleLavender", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.SPARKLE_LAVENDER.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.SPARKLE_LAVENDER.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Splendid, "atmosuit_sparkle_lavender_kanim"),
			new ClothingItems.Info("AtmoGlovesSparkleRed", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.SPARKLE_RED.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.SPARKLE_RED.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_sparkle_red_kanim"),
			new ClothingItems.Info("AtmoGlovesSparkleGreen", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.SPARKLE_GREEN.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.SPARKLE_GREEN.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_sparkle_green_kanim"),
			new ClothingItems.Info("AtmoGlovesSparkleBlue", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.SPARKLE_BLUE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.SPARKLE_BLUE.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_sparkle_blue_kanim"),
			new ClothingItems.Info("AtmoGlovesSparkleLavender", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.SPARKLE_LAVENDER.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.SPARKLE_LAVENDER.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_sparkle_lavender_kanim"),
			new ClothingItems.Info("AtmoBeltSparkleRed", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.SPARKLE_RED.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.SPARKLE_RED.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Splendid, "atmo_belt_sparkle_red_kanim"),
			new ClothingItems.Info("AtmoBeltSparkleGreen", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.SPARKLE_GREEN.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.SPARKLE_GREEN.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Splendid, "atmo_belt_sparkle_green_kanim"),
			new ClothingItems.Info("AtmoBeltSparkleBlue", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.SPARKLE_BLUE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.SPARKLE_BLUE.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Splendid, "atmo_belt_sparkle_blue_kanim"),
			new ClothingItems.Info("AtmoBeltSparkleLavender", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.SPARKLE_LAVENDER.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.SPARKLE_LAVENDER.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Splendid, "atmo_belt_sparkle_lavender_kanim"),
			new ClothingItems.Info("AtmoShoesSparkleBlack", EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.SPARKLE_BLACK.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.SPARKLE_BLACK.DESC, PermitCategory.AtmoSuitShoes, PermitRarity.Common, "atmo_shoes_sparkle_black_kanim"),
			new ClothingItems.Info("TopDenimBlue", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.DENIM_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.DENIM_BLUE.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_denim_blue_kanim"),
			new ClothingItems.Info("TopUndershirtExecutive", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_STRAWBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_STRAWBERRY.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_gonch_strawberry_kanim"),
			new ClothingItems.Info("TopUndershirtUnderling", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_SATSUMA.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_gonch_satsuma_kanim"),
			new ClothingItems.Info("TopUndershirtGroupthink", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_LEMON.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_LEMON.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_gonch_lemon_kanim"),
			new ClothingItems.Info("TopUndershirtStakeholder", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_LIME.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_LIME.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_gonch_lime_kanim"),
			new ClothingItems.Info("TopUndershirtAdmin", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_BLUEBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_BLUEBERRY.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_gonch_blueberry_kanim"),
			new ClothingItems.Info("TopUndershirtBuzzword", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_GRAPE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_GRAPE.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_gonch_grape_kanim"),
			new ClothingItems.Info("TopUndershirtSynergy", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_WATERMELON.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GONCH_WATERMELON.DESC, PermitCategory.DupeTops, PermitRarity.Decent, "top_gonch_watermelon_kanim"),
			new ClothingItems.Info("TopResearcher", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.NERD_BROWN.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.NERD_BROWN.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_nerd_white_cream_kanim"),
			new ClothingItems.Info("TopRebelGi", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GI_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.GI_WHITE.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_gi_white_kanim"),
			new ClothingItems.Info("BottomBriefsExecutive", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_STRAWBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_STRAWBERRY.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "bottom_gonch_strawberry_kanim"),
			new ClothingItems.Info("BottomBriefsUnderling", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_SATSUMA.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "bottom_gonch_satsuma_kanim"),
			new ClothingItems.Info("BottomBriefsGroupthink", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_LEMON.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_LEMON.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "bottom_gonch_lemon_kanim"),
			new ClothingItems.Info("BottomBriefsStakeholder", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_LIME.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_LIME.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "bottom_gonch_lime_kanim"),
			new ClothingItems.Info("BottomBriefsAdmin", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_BLUEBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_BLUEBERRY.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "bottom_gonch_blueberry_kanim"),
			new ClothingItems.Info("BottomBriefsBuzzword", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_GRAPE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_GRAPE.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "bottom_gonch_grape_kanim"),
			new ClothingItems.Info("BottomBriefsSynergy", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_WATERMELON.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GONCH_WATERMELON.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "bottom_gonch_watermelon_kanim"),
			new ClothingItems.Info("PantsJeans", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.DENIM_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.DENIM_BLUE.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "pants_denim_blue_kanim"),
			new ClothingItems.Info("PantsRebelGi", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GI_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.GI_WHITE.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "pants_gi_white_kanim"),
			new ClothingItems.Info("PantsResearch", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.NERD_BROWN.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.NERD_BROWN.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "pants_nerd_brown_kanim"),
			new ClothingItems.Info("ShoesBasicGray", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_GREY.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BASIC_GREY.DESC, PermitCategory.DupeShoes, PermitRarity.Common, "shoes_basic_grey_kanim"),
			new ClothingItems.Info("ShoesDenimBlue", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.DENIM_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.DENIM_BLUE.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "shoes_denim_blue_kanim"),
			new ClothingItems.Info("SocksLegwarmersBlueberry", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_BLUEBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_BLUEBERRY.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "socks_legwarmers_blueberry_kanim"),
			new ClothingItems.Info("SocksLegwarmersGrape", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_GRAPE.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_GRAPE.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "socks_legwarmers_grape_kanim"),
			new ClothingItems.Info("SocksLegwarmersLemon", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_LEMON.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_LEMON.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "socks_legwarmers_lemon_kanim"),
			new ClothingItems.Info("SocksLegwarmersLime", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_LIME.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_LIME.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "socks_legwarmers_lime_kanim"),
			new ClothingItems.Info("SocksLegwarmersSatsuma", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_SATSUMA.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "socks_legwarmers_satsuma_kanim"),
			new ClothingItems.Info("SocksLegwarmersStrawberry", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_STRAWBERRY.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_STRAWBERRY.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "socks_legwarmers_strawberry_kanim"),
			new ClothingItems.Info("SocksLegwarmersWatermelon", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_WATERMELON.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.LEGWARMERS_WATERMELON.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "socks_legwarmers_watermelon_kanim"),
			new ClothingItems.Info("GlovesCufflessBlack", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_BLACK.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.CUFFLESS_BLACK.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_cuffless_black_kanim"),
			new ClothingItems.Info("GlovesDenimBlue", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.DENIM_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.DENIM_BLUE.DESC, PermitCategory.DupeGloves, PermitRarity.Decent, "gloves_denim_blue_kanim"),
			new ClothingItems.Info("AtmoGlovesGold", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.GOLD.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.GOLD.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_gold_kanim"),
			new ClothingItems.Info("AtmoGlovesEggplant", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.PURPLE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.PURPLE.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_purple_kanim"),
			new ClothingItems.Info("AtmoHelmetEggplant", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.CLUBSHIRT_PURPLE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.CLUBSHIRT_PURPLE.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_clubshirt_purple_kanim"),
			new ClothingItems.Info("AtmoHelmetConfetti", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.TRIANGLES_TURQ.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.TRIANGLES_TURQ.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_triangles_turq_kanim"),
			new ClothingItems.Info("AtmoShoesStealth", EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.BASIC_BLACK.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.BASIC_BLACK.DESC, PermitCategory.AtmoSuitShoes, PermitRarity.Common, "atmo_shoes_basic_black_kanim"),
			new ClothingItems.Info("AtmoShoesEggplant", EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.BASIC_PURPLE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.BASIC_PURPLE.DESC, PermitCategory.AtmoSuitShoes, PermitRarity.Common, "atmo_shoes_basic_purple_kanim"),
			new ClothingItems.Info("AtmoSuitCrispEggplant", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.BASIC_PURPLE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.BASIC_PURPLE.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Nifty, "atmosuit_basic_purple_kanim"),
			new ClothingItems.Info("AtmoSuitConfetti", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.PRINT_TRIANGLES_TURQ.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.PRINT_TRIANGLES_TURQ.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Splendid, "atmosuit_print_triangles_turq_kanim"),
			new ClothingItems.Info("AtmoBeltBasicGold", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.BASIC_GOLD.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.BASIC_GOLD.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Nifty, "atmo_belt_basic_gold_kanim"),
			new ClothingItems.Info("AtmoBeltEggplant", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.TWOTONE_PURPLE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.TWOTONE_PURPLE.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Nifty, "atmo_belt_2tone_purple_kanim"),
			new ClothingItems.Info("SkirtBasicBlueMiddle", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_BLUE_MIDDLE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_BLUE_MIDDLE.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "skirt_basic_blue_middle_kanim"),
			new ClothingItems.Info("SkirtBasicPurple", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_PURPLE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_PURPLE.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "skirt_basic_purple_kanim"),
			new ClothingItems.Info("SkirtBasicGreen", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_GREEN.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_GREEN.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "skirt_basic_green_kanim"),
			new ClothingItems.Info("SkirtBasicOrange", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_ORANGE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_ORANGE.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "skirt_basic_orange_kanim"),
			new ClothingItems.Info("SkirtBasicPinkOrchid", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_PINK_ORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_PINK_ORCHID.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "skirt_basic_pink_orchid_kanim"),
			new ClothingItems.Info("SkirtBasicRed", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_RED.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_RED.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "skirt_basic_red_kanim"),
			new ClothingItems.Info("SkirtBasicYellow", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_YELLOW.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_YELLOW.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "skirt_basic_yellow_kanim"),
			new ClothingItems.Info("SkirtBasicPolkadot", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_POLKADOT.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_POLKADOT.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "skirt_basic_polkadot_kanim"),
			new ClothingItems.Info("SkirtBasicWatermelon", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_WATERMELON.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BASIC_WATERMELON.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "skirt_basic_watermelon_kanim"),
			new ClothingItems.Info("SkirtDenimBlue", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_DENIM_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_DENIM_BLUE.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "skirt_denim_blue_kanim"),
			new ClothingItems.Info("SkirtLeopardPrintBluePink", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_LEOPARD_PRINT_BLUE_PINK.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_LEOPARD_PRINT_BLUE_PINK.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "skirt_leopard_print_blue_pink_kanim"),
			new ClothingItems.Info("SkirtSparkleBlue", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_SPARKLE_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_SPARKLE_BLUE.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "skirt_sparkle_blue_kanim"),
			new ClothingItems.Info("AtmoBeltBasicGrey", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.BASIC_GREY.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.BASIC_GREY.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Nifty, "atmo_belt_basic_grey_kanim"),
			new ClothingItems.Info("AtmoBeltBasicNeonPink", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.BASIC_NEON_PINK.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.BASIC_NEON_PINK.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Nifty, "atmo_belt_basic_neon_pink_kanim"),
			new ClothingItems.Info("AtmoGlovesWhite", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.WHITE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.WHITE.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_white_kanim"),
			new ClothingItems.Info("AtmoGlovesStripesLavender", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.STRIPES_LAVENDER.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.STRIPES_LAVENDER.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_stripes_lavender_kanim"),
			new ClothingItems.Info("AtmoHelmetCummerbundRed", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.CUMMERBUND_RED.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.CUMMERBUND_RED.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_cummerbund_red_kanim"),
			new ClothingItems.Info("AtmoHelmetWorkoutLavender", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.WORKOUT_LAVENDER.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.WORKOUT_LAVENDER.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_workout_lavender_kanim"),
			new ClothingItems.Info("AtmoShoesBasicLavender", EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.BASIC_LAVENDER.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.BASIC_LAVENDER.DESC, PermitCategory.AtmoSuitShoes, PermitRarity.Common, "atmo_shoes_basic_lavender_kanim"),
			new ClothingItems.Info("AtmoSuitBasicNeonPink", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.BASIC_NEON_PINK.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.BASIC_NEON_PINK.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Nifty, "atmosuit_basic_neon_pink_kanim"),
			new ClothingItems.Info("AtmoSuitMultiRedBlack", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.MULTI_RED_BLACK.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.MULTI_RED_BLACK.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Splendid, "atmosuit_multi_red_black_kanim"),
			new ClothingItems.Info("TopJacketSmokingBurgundy", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JACKET_SMOKING_BURGUNDY.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.JACKET_SMOKING_BURGUNDY.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_jacket_smoking_burgundy_kanim"),
			new ClothingItems.Info("TopMechanic", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.MECHANIC.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.MECHANIC.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_mechanic_kanim"),
			new ClothingItems.Info("TopVelourBlack", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.VELOUR_BLACK.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.VELOUR_BLACK.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_velour_black_kanim"),
			new ClothingItems.Info("TopVelourBlue", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.VELOUR_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.VELOUR_BLUE.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_velour_blue_kanim"),
			new ClothingItems.Info("TopVelourPink", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.VELOUR_PINK.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.VELOUR_PINK.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_velour_pink_kanim"),
			new ClothingItems.Info("TopWaistcoatPinstripeSlate", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.WAISTCOAT_PINSTRIPE_SLATE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.WAISTCOAT_PINSTRIPE_SLATE.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_waistcoat_pinstripe_slate_kanim"),
			new ClothingItems.Info("TopWater", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.WATER.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.WATER.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_water_kanim"),
			new ClothingItems.Info("TopTweedPinkOrchid", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.TWEED_PINK_ORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.TWEED_PINK_ORCHID.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_tweed_pink_orchid_kanim"),
			new ClothingItems.Info("DressSleevelessBowBw", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.DRESS_SLEEVELESS_BOW_BW.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.DRESS_SLEEVELESS_BOW_BW.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "dress_sleeveless_bow_bw_kanim"),
			new ClothingItems.Info("BodysuitBallerinaPink", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BODYSUIT_BALLERINA_PINK.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.BODYSUIT_BALLERINA_PINK.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "bodysuit_ballerina_pink_kanim"),
			new ClothingItems.Info("PantsBasicOrangeSatsuma", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_ORANGE_SATSUMA.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.BASIC_ORANGE_SATSUMA.DESC, PermitCategory.DupeBottoms, PermitRarity.Common, "pants_basic_orange_satsuma_kanim"),
			new ClothingItems.Info("PantsPinstripeSlate", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.PINSTRIPE_SLATE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.PINSTRIPE_SLATE.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "pants_pinstripe_slate_kanim"),
			new ClothingItems.Info("PantsVelourBlack", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.VELOUR_BLACK.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.VELOUR_BLACK.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "pants_velour_black_kanim"),
			new ClothingItems.Info("PantsVelourBlue", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.VELOUR_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.VELOUR_BLUE.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "pants_velour_blue_kanim"),
			new ClothingItems.Info("PantsVelourPink", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.VELOUR_PINK.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.VELOUR_PINK.DESC, PermitCategory.DupeBottoms, PermitRarity.Decent, "pants_velour_pink_kanim"),
			new ClothingItems.Info("SkirtBallerinaPink", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BALLERINA_PINK.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_BALLERINA_PINK.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "skirt_ballerina_pink_kanim"),
			new ClothingItems.Info("SkirtTweedPinkOrchid", EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_TWEED_PINK_ORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.FACADES.SKIRT_TWEED_PINK_ORCHID.DESC, PermitCategory.DupeBottoms, PermitRarity.Nifty, "skirt_tweed_pink_orchid_kanim"),
			new ClothingItems.Info("ShoesBallerinaPink", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BALLERINA_PINK.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.BALLERINA_PINK.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "shoes_ballerina_pink_kanim"),
			new ClothingItems.Info("ShoesMaryjaneSocksBw", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.MARYJANE_SOCKS_BW.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.MARYJANE_SOCKS_BW.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "shoes_maryjane_socks_bw_kanim"),
			new ClothingItems.Info("ShoesClassicFlatsCreamCharcoal", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.CLASSICFLATS_CREAM_CHARCOAL.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.CLASSICFLATS_CREAM_CHARCOAL.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "shoes_classicflats_cream_charcoal_kanim"),
			new ClothingItems.Info("ShoesVelourBlue", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.VELOUR_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.VELOUR_BLUE.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "shoes_velour_blue_kanim"),
			new ClothingItems.Info("ShoesVelourPink", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.VELOUR_PINK.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.VELOUR_PINK.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "shoes_velour_pink_kanim"),
			new ClothingItems.Info("ShoesVelourBlack", EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.VELOUR_BLACK.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.FACADES.VELOUR_BLACK.DESC, PermitCategory.DupeShoes, PermitRarity.Decent, "shoes_velour_black_kanim"),
			new ClothingItems.Info("GlovesBasicGrey", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_GREY.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_GREY.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_grey_kanim"),
			new ClothingItems.Info("GlovesBasicPinksalmon", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_PINKSALMON.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_PINKSALMON.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_pinksalmon_kanim"),
			new ClothingItems.Info("GlovesBasicTan", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_TAN.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BASIC_TAN.DESC, PermitCategory.DupeGloves, PermitRarity.Common, "gloves_basic_tan_kanim"),
			new ClothingItems.Info("GlovesBallerinaPink", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BALLERINA_PINK.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.BALLERINA_PINK.DESC, PermitCategory.DupeGloves, PermitRarity.Decent, "gloves_ballerina_pink_kanim"),
			new ClothingItems.Info("GlovesFormalWhite", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.FORMAL_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.FORMAL_WHITE.DESC, PermitCategory.DupeGloves, PermitRarity.Decent, "gloves_formal_white_kanim"),
			new ClothingItems.Info("GlovesLongWhite", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.LONG_WHITE.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.LONG_WHITE.DESC, PermitCategory.DupeGloves, PermitRarity.Decent, "gloves_long_white_kanim"),
			new ClothingItems.Info("Gloves2ToneCreamCharcoal", EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.TWOTONE_CREAM_CHARCOAL.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.FACADES.TWOTONE_CREAM_CHARCOAL.DESC, PermitCategory.DupeGloves, PermitRarity.Decent, "gloves_2tone_cream_charcoal_kanim"),
			new ClothingItems.Info("AtmoHelmetRocketmelon", EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.CANTALOUPE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.FACADES.CANTALOUPE.DESC, PermitCategory.AtmoSuitHelmet, PermitRarity.Splendid, "atmo_helmet_cantaloupe_kanim"),
			new ClothingItems.Info("AtmoSuitRocketmelon", EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.CANTALOUPE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.FACADES.CANTALOUPE.DESC, PermitCategory.AtmoSuitBody, PermitRarity.Splendid, "atmosuit_cantaloupe_kanim"),
			new ClothingItems.Info("AtmoBeltRocketmelon", EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.CANTALOUPE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.FACADES.CANTALOUPE.DESC, PermitCategory.AtmoSuitBelt, PermitRarity.Nifty, "atmo_belt_cantaloupe_kanim"),
			new ClothingItems.Info("AtmoGlovesRocketmelon", EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.CANTALOUPE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.FACADES.CANTALOUPE.DESC, PermitCategory.AtmoSuitGloves, PermitRarity.Common, "atmo_gloves_cantaloupe_kanim"),
			new ClothingItems.Info("AtmoBootsRocketmelon", EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.CANTALOUPE.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.FACADES.CANTALOUPE.DESC, PermitCategory.AtmoSuitShoes, PermitRarity.Common, "atmo_shoes_cantaloupe_kanim"),
			new ClothingItems.Info("TopXSporchid", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.X_SPORCHID.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.X_SPORCHID.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_x_sporchid_kanim"),
			new ClothingItems.Info("TopX1Pinchapeppernutbells", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.X1_PINCHAPEPPERNUTBELLS.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.X1_PINCHAPEPPERNUTBELLS.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_x1_pinchapeppernutbells_kanim"),
			new ClothingItems.Info("TopPompomShinebugsPinkPeppernut", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.POMPOM_SHINEBUGS_PINK_PEPPERNUT.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.POMPOM_SHINEBUGS_PINK_PEPPERNUT.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_pompom_shinebugs_pink_peppernut_kanim"),
			new ClothingItems.Info("TopSnowflakeBlue", EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.SNOWFLAKE_BLUE.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.FACADES.SNOWFLAKE_BLUE.DESC, PermitCategory.DupeTops, PermitRarity.Nifty, "top_snowflake_blue_kanim")
		};

		public static ClothingItems.Info[] Infos_All = ClothingItems.Infos_Skins;

		public struct Info
		{
			public Info(string id, string name, string desc, PermitCategory category, PermitRarity rarity, string animFile)
			{
				Option<ClothingOutfitUtility.OutfitType> outfitTypeFor = PermitCategories.GetOutfitTypeFor(category);
				if (outfitTypeFor.IsNone())
				{
					throw new Exception(string.Format("Expected permit category {0} on ClothingItemResource \"{1}\" to have an {2} but none found.", category, id, "OutfitType"));
				}
				this.id = id;
				this.name = name;
				this.desc = desc;
				this.outfitType = outfitTypeFor.Unwrap();
				this.category = category;
				this.rarity = rarity;
				this.animFile = animFile;
			}

			public string id;

			public string name;

			public string desc;

			public ClothingOutfitUtility.OutfitType outfitType;

			public PermitCategory category;

			public PermitRarity rarity;

			public string animFile;
		}
	}
}
