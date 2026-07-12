using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using STRINGS;

public class PermitItems
{
	public static IEnumerable<KleiItems.ItemData> IterateInventory()
	{
		foreach (KleiItems.ItemData itemData in KleiItems.IterateInventory(PermitItems.ItemToPermit, PermitItems.BoxSet))
		{
			yield return itemData;
		}
		IEnumerator<KleiItems.ItemData> enumerator = null;
		yield break;
		yield break;
	}

	public static bool HasUnopenedItem()
	{
		return KleiItems.HasUnopenedItem(PermitItems.ItemToPermit, PermitItems.BoxSet);
	}

	public static bool IsPermitUnlocked(PermitResource permit)
	{
		return PermitItems.GetOwnedCount(permit) > 0;
	}

	public static int GetOwnedCount(PermitResource permit)
	{
		int num = 0;
		PermitItems.ItemInfo itemInfo;
		if (PermitItems.Mappings.TryGetValue(permit.Id, out itemInfo))
		{
			num = KleiItems.GetOwnedItemCount(itemInfo.ItemType);
		}
		return num;
	}

	public static bool TryGetBoxInfo(KleiItems.ItemData item, out string name, out string desc, out string icon_name)
	{
		PermitItems.BoxInfo boxInfo;
		if (PermitItems.BoxMappings.TryGetValue(item.Id, out boxInfo))
		{
			name = boxInfo.Name;
			desc = boxInfo.Description;
			icon_name = boxInfo.IconName;
			return true;
		}
		name = null;
		desc = null;
		icon_name = null;
		return false;
	}

	public static bool TryGetBarterPrice(string permit_id, out ulong buy_price, out ulong sell_price)
	{
		buy_price = (sell_price = 0UL);
		PermitItems.ItemInfo itemInfo;
		return PermitItems.Mappings.TryGetValue(permit_id, out itemInfo) && KleiItems.TryGetBarterPrice(itemInfo.ItemType, out buy_price, out sell_price);
	}

	public static void QueueRequestOpenOrUnboxItem(KleiItems.ItemData item, KleiItems.ResponseCallback cb)
	{
		DebugUtil.DevAssert(!item.IsOpened, "Can't open already opened item.", null);
		if (item.IsOpened)
		{
			return;
		}
		if (PermitItems.BoxSet.Contains(item.Id))
		{
			KleiItems.AddRequestMysteryBoxOpened(item.ItemId, cb);
			return;
		}
		KleiItems.AddRequestItemOpened(item.ItemId, cb);
	}

	public static string GetServerTypeFromPermit(PermitResource resource)
	{
		foreach (PermitItems.ItemInfo itemInfo in PermitItems.ItemInfos)
		{
			if (itemInfo.PermitId == resource.Id)
			{
				return itemInfo.ItemType;
			}
		}
		Debug.LogError("No matching server ItemType for requested PermitResource " + resource.Id);
		return null;
	}

	private static PermitItems.ItemInfo[] ItemInfos = new PermitItems.ItemInfo[]
	{
		new PermitItems.ItemInfo("top_basic_black", 1U, "TopBasicBlack"),
		new PermitItems.ItemInfo("top_basic_white", 2U, "TopBasicWhite"),
		new PermitItems.ItemInfo("top_basic_red", 3U, "TopBasicRed"),
		new PermitItems.ItemInfo("top_basic_orange", 4U, "TopBasicOrange"),
		new PermitItems.ItemInfo("top_basic_yellow", 5U, "TopBasicYellow"),
		new PermitItems.ItemInfo("top_basic_green", 6U, "TopBasicGreen"),
		new PermitItems.ItemInfo("top_basic_blue_middle", 7U, "TopBasicAqua"),
		new PermitItems.ItemInfo("top_basic_purple", 8U, "TopBasicPurple"),
		new PermitItems.ItemInfo("top_basic_pink_orchid", 9U, "TopBasicPinkOrchid"),
		new PermitItems.ItemInfo("pants_basic_white", 11U, "BottomBasicWhite"),
		new PermitItems.ItemInfo("pants_basic_red", 12U, "BottomBasicRed"),
		new PermitItems.ItemInfo("pants_basic_orange", 13U, "BottomBasicOrange"),
		new PermitItems.ItemInfo("pants_basic_yellow", 14U, "BottomBasicYellow"),
		new PermitItems.ItemInfo("pants_basic_green", 15U, "BottomBasicGreen"),
		new PermitItems.ItemInfo("pants_basic_blue_middle", 16U, "BottomBasicAqua"),
		new PermitItems.ItemInfo("pants_basic_purple", 17U, "BottomBasicPurple"),
		new PermitItems.ItemInfo("pants_basic_pink_orchid", 18U, "BottomBasicPinkOrchid"),
		new PermitItems.ItemInfo("gloves_basic_black", 19U, "GlovesBasicBlack"),
		new PermitItems.ItemInfo("gloves_basic_white", 20U, "GlovesBasicWhite"),
		new PermitItems.ItemInfo("gloves_basic_red", 21U, "GlovesBasicRed"),
		new PermitItems.ItemInfo("gloves_basic_orange", 22U, "GlovesBasicOrange"),
		new PermitItems.ItemInfo("gloves_basic_yellow", 23U, "GlovesBasicYellow"),
		new PermitItems.ItemInfo("gloves_basic_green", 24U, "GlovesBasicGreen"),
		new PermitItems.ItemInfo("gloves_basic_blue_middle", 25U, "GlovesBasicAqua"),
		new PermitItems.ItemInfo("gloves_basic_purple", 26U, "GlovesBasicPurple"),
		new PermitItems.ItemInfo("gloves_basic_pink_orchid", 27U, "GlovesBasicPinkOrchid"),
		new PermitItems.ItemInfo("shoes_basic_white", 30U, "ShoesBasicWhite"),
		new PermitItems.ItemInfo("shoes_basic_red", 31U, "ShoesBasicRed"),
		new PermitItems.ItemInfo("shoes_basic_orange", 32U, "ShoesBasicOrange"),
		new PermitItems.ItemInfo("shoes_basic_yellow", 33U, "ShoesBasicYellow"),
		new PermitItems.ItemInfo("shoes_basic_green", 34U, "ShoesBasicGreen"),
		new PermitItems.ItemInfo("shoes_basic_blue_middle", 35U, "ShoesBasicAqua"),
		new PermitItems.ItemInfo("shoes_basic_purple", 36U, "ShoesBasicPurple"),
		new PermitItems.ItemInfo("shoes_basic_pink_orchid", 37U, "ShoesBasicPinkOrchid"),
		new PermitItems.ItemInfo("flowervase_retro", 39U, "FlowerVase_retro"),
		new PermitItems.ItemInfo("flowervase_retro_red", 40U, "FlowerVase_retro_red"),
		new PermitItems.ItemInfo("flowervase_retro_white", 41U, "FlowerVase_retro_white"),
		new PermitItems.ItemInfo("flowervase_retro_green", 42U, "FlowerVase_retro_green"),
		new PermitItems.ItemInfo("flowervase_retro_blue", 43U, "FlowerVase_retro_blue"),
		new PermitItems.ItemInfo("elegantbed_boat", 44U, "LuxuryBed_boat"),
		new PermitItems.ItemInfo("elegantbed_bouncy", 45U, "LuxuryBed_bouncy"),
		new PermitItems.ItemInfo("elegantbed_grandprix", 46U, "LuxuryBed_grandprix"),
		new PermitItems.ItemInfo("elegantbed_rocket", 47U, "LuxuryBed_rocket"),
		new PermitItems.ItemInfo("elegantbed_puft", 48U, "LuxuryBed_puft"),
		new PermitItems.ItemInfo("walls_pastel_pink", 49U, "ExteriorWall_pastel_pink"),
		new PermitItems.ItemInfo("walls_pastel_yellow", 50U, "ExteriorWall_pastel_yellow"),
		new PermitItems.ItemInfo("walls_pastel_green", 51U, "ExteriorWall_pastel_green"),
		new PermitItems.ItemInfo("walls_pastel_blue", 52U, "ExteriorWall_pastel_blue"),
		new PermitItems.ItemInfo("walls_pastel_purple", 53U, "ExteriorWall_pastel_purple"),
		new PermitItems.ItemInfo("walls_balm_lily", 54U, "ExteriorWall_balm_lily"),
		new PermitItems.ItemInfo("walls_clouds", 55U, "ExteriorWall_clouds"),
		new PermitItems.ItemInfo("walls_coffee", 56U, "ExteriorWall_coffee"),
		new PermitItems.ItemInfo("walls_mosaic", 57U, "ExteriorWall_mosaic"),
		new PermitItems.ItemInfo("walls_mushbar", 58U, "ExteriorWall_mushbar"),
		new PermitItems.ItemInfo("walls_plaid", 59U, "ExteriorWall_plaid"),
		new PermitItems.ItemInfo("walls_rain", 60U, "ExteriorWall_rain"),
		new PermitItems.ItemInfo("walls_rainbow", 61U, "ExteriorWall_rainbow"),
		new PermitItems.ItemInfo("walls_snow", 62U, "ExteriorWall_snow"),
		new PermitItems.ItemInfo("walls_sun", 63U, "ExteriorWall_sun"),
		new PermitItems.ItemInfo("walls_polka", 64U, "ExteriorWall_polka"),
		new PermitItems.ItemInfo("painting_art_i", 65U, "Canvas_Good7"),
		new PermitItems.ItemInfo("painting_art_j", 66U, "Canvas_Good8"),
		new PermitItems.ItemInfo("painting_art_k", 67U, "Canvas_Good9"),
		new PermitItems.ItemInfo("painting_tall_art_g", 68U, "CanvasTall_Good5"),
		new PermitItems.ItemInfo("painting_tall_art_h", 69U, "CanvasTall_Good6"),
		new PermitItems.ItemInfo("painting_tall_art_i", 70U, "CanvasTall_Good7"),
		new PermitItems.ItemInfo("painting_wide_art_g", 71U, "CanvasWide_Good5"),
		new PermitItems.ItemInfo("painting_wide_art_h", 72U, "CanvasWide_Good6"),
		new PermitItems.ItemInfo("painting_wide_art_i", 73U, "CanvasWide_Good7"),
		new PermitItems.ItemInfo("sculpture_amazing_4", 74U, "Sculpture_Good4"),
		new PermitItems.ItemInfo("sculpture_1x2_amazing_4", 75U, "SmallSculpture_Good4"),
		new PermitItems.ItemInfo("sculpture_metal_amazing_4", 76U, "MetalSculpture_Good4"),
		new PermitItems.ItemInfo("sculpture_marble_amazing_4", 77U, "MarbleSculpture_Good4"),
		new PermitItems.ItemInfo("sculpture_marble_amazing_5", 78U, "MarbleSculpture_Good5"),
		new PermitItems.ItemInfo("icesculpture_idle_2", 79U, "IceSculpture_Average2"),
		new PermitItems.ItemInfo("top_raglan_deep_red", 83U, "TopRaglanDeepRed"),
		new PermitItems.ItemInfo("top_raglan_cobalt", 84U, "TopRaglanCobalt"),
		new PermitItems.ItemInfo("top_raglan_flamingo", 85U, "TopRaglanFlamingo"),
		new PermitItems.ItemInfo("top_raglan_kelly_green", 86U, "TopRaglanKellyGreen"),
		new PermitItems.ItemInfo("top_raglan_charcoal", 87U, "TopRaglanCharcoal"),
		new PermitItems.ItemInfo("top_raglan_lemon", 88U, "TopRaglanLemon"),
		new PermitItems.ItemInfo("top_raglan_satsuma", 89U, "TopRaglanSatsuma"),
		new PermitItems.ItemInfo("shorts_basic_deep_red", 91U, "ShortsBasicDeepRed"),
		new PermitItems.ItemInfo("shorts_basic_satsuma", 92U, "ShortsBasicSatsuma"),
		new PermitItems.ItemInfo("shorts_basic_yellowcake", 93U, "ShortsBasicYellowcake"),
		new PermitItems.ItemInfo("shorts_basic_kelly_green", 94U, "ShortsBasicKellyGreen"),
		new PermitItems.ItemInfo("shorts_basic_blue_cobalt", 95U, "ShortsBasicBlueCobalt"),
		new PermitItems.ItemInfo("shorts_basic_pink_flamingo", 96U, "ShortsBasicPinkFlamingo"),
		new PermitItems.ItemInfo("shorts_basic_charcoal", 97U, "ShortsBasicCharcoal"),
		new PermitItems.ItemInfo("socks_athletic_deep_red", 98U, "SocksAthleticDeepRed"),
		new PermitItems.ItemInfo("socks_athletic_orange_satsuma", 99U, "SocksAthleticOrangeSatsuma"),
		new PermitItems.ItemInfo("socks_athletic_yellow_lemon", 100U, "SocksAthleticYellowLemon"),
		new PermitItems.ItemInfo("socks_athletic_green_kelly", 101U, "SocksAthleticGreenKelly"),
		new PermitItems.ItemInfo("socks_athletic_blue_cobalt", 102U, "SocksAthleticBlueCobalt"),
		new PermitItems.ItemInfo("socks_athletic_pink_flamingo", 103U, "SocksAthleticPinkFlamingo"),
		new PermitItems.ItemInfo("socks_athletic_grey_charcoal", 104U, "SocksAthleticGreyCharcoal"),
		new PermitItems.ItemInfo("gloves_athletic_red_deep", 105U, "GlovesAthleticRedDeep"),
		new PermitItems.ItemInfo("gloves_athletic_orange_satsuma", 106U, "GlovesAthleticOrangeSatsuma"),
		new PermitItems.ItemInfo("gloves_athletic_yellow_lemon", 107U, "GlovesAthleticYellowLemon"),
		new PermitItems.ItemInfo("gloves_athletic_green_kelly", 108U, "GlovesAthleticGreenKelly"),
		new PermitItems.ItemInfo("gloves_athletic_blue_cobalt", 109U, "GlovesAthleticBlueCobalt"),
		new PermitItems.ItemInfo("gloves_athletic_pink_flamingo", 110U, "GlovesAthleticPinkFlamingo"),
		new PermitItems.ItemInfo("gloves_athletic_grey_charcoal", 111U, "GlovesAthleticGreyCharcoal"),
		new PermitItems.ItemInfo("walls_diagonal_red_deep_white", 112U, "ExteriorWall_diagonal_red_deep_white"),
		new PermitItems.ItemInfo("walls_diagonal_orange_satsuma_white", 113U, "ExteriorWall_diagonal_orange_satsuma_white"),
		new PermitItems.ItemInfo("walls_diagonal_yellow_lemon_white", 114U, "ExteriorWall_diagonal_yellow_lemon_white"),
		new PermitItems.ItemInfo("walls_diagonal_green_kelly_white", 115U, "ExteriorWall_diagonal_green_kelly_white"),
		new PermitItems.ItemInfo("walls_diagonal_blue_cobalt_white", 116U, "ExteriorWall_diagonal_blue_cobalt_white"),
		new PermitItems.ItemInfo("walls_diagonal_pink_flamingo_white", 117U, "ExteriorWall_diagonal_pink_flamingo_white"),
		new PermitItems.ItemInfo("walls_diagonal_grey_charcoal_white", 118U, "ExteriorWall_diagonal_grey_charcoal_white"),
		new PermitItems.ItemInfo("walls_circle_red_deep_white", 119U, "ExteriorWall_circle_red_deep_white"),
		new PermitItems.ItemInfo("walls_circle_orange_satsuma_white", 120U, "ExteriorWall_circle_orange_satsuma_white"),
		new PermitItems.ItemInfo("walls_circle_yellow_lemon_white", 121U, "ExteriorWall_circle_yellow_lemon_white"),
		new PermitItems.ItemInfo("walls_circle_green_kelly_white", 122U, "ExteriorWall_circle_green_kelly_white"),
		new PermitItems.ItemInfo("walls_circle_blue_cobalt_white", 123U, "ExteriorWall_circle_blue_cobalt_white"),
		new PermitItems.ItemInfo("walls_circle_pink_flamingo_white", 124U, "ExteriorWall_circle_pink_flamingo_white"),
		new PermitItems.ItemInfo("walls_circle_grey_charcoal_white", 125U, "ExteriorWall_circle_grey_charcoal_white"),
		new PermitItems.ItemInfo("bed_star_curtain", 126U, "Bed_star_curtain"),
		new PermitItems.ItemInfo("bed_canopy", 127U, "Bed_canopy"),
		new PermitItems.ItemInfo("bed_rowan_tropical", 128U, "Bed_rowan_tropical"),
		new PermitItems.ItemInfo("bed_ada_science_lab", 129U, "Bed_ada_science_lab"),
		new PermitItems.ItemInfo("ceilinglight_mining", 130U, "CeilingLight_mining"),
		new PermitItems.ItemInfo("ceilinglight_flower", 131U, "CeilingLight_flower"),
		new PermitItems.ItemInfo("ceilinglight_polka_lamp_shade", 132U, "CeilingLight_polka_lamp_shade"),
		new PermitItems.ItemInfo("ceilinglight_burt_shower", 133U, "CeilingLight_burt_shower"),
		new PermitItems.ItemInfo("ceilinglight_ada_flask_round", 134U, "CeilingLight_ada_flask_round"),
		new PermitItems.ItemInfo("balloon_red_fireengine_long_sparkles_kanim", 135U, "BalloonRedFireEngineLongSparkles"),
		new PermitItems.ItemInfo("balloon_yellow_long_sparkles_kanim", 136U, "BalloonYellowLongSparkles"),
		new PermitItems.ItemInfo("balloon_blue_long_sparkles_kanim", 137U, "BalloonBlueLongSparkles"),
		new PermitItems.ItemInfo("balloon_green_long_sparkles_kanim", 138U, "BalloonGreenLongSparkles"),
		new PermitItems.ItemInfo("balloon_pink_long_sparkles_kanim", 139U, "BalloonPinkLongSparkles"),
		new PermitItems.ItemInfo("balloon_purple_long_sparkles_kanim", 140U, "BalloonPurpleLongSparkles"),
		new PermitItems.ItemInfo("balloon_babypacu_egg_kanim", 141U, "BalloonBabyPacuEgg"),
		new PermitItems.ItemInfo("balloon_babyglossydrecko_egg_kanim", 142U, "BalloonBabyGlossyDreckoEgg"),
		new PermitItems.ItemInfo("balloon_babyhatch_egg_kanim", 143U, "BalloonBabyHatchEgg"),
		new PermitItems.ItemInfo("balloon_babypokeshell_egg_kanim", 144U, "BalloonBabyPokeshellEgg"),
		new PermitItems.ItemInfo("balloon_babypuft_egg_kanim", 145U, "BalloonBabyPuftEgg"),
		new PermitItems.ItemInfo("balloon_babyshovole_egg_kanim", 146U, "BalloonBabyShovoleEgg"),
		new PermitItems.ItemInfo("balloon_babypip_egg_kanim", 147U, "BalloonBabyPipEgg"),
		new PermitItems.ItemInfo("top_jellypuffjacket_blueberry", 150U, "TopJellypuffJacketBlueberry"),
		new PermitItems.ItemInfo("top_jellypuffjacket_grape", 151U, "TopJellypuffJacketGrape"),
		new PermitItems.ItemInfo("top_jellypuffjacket_lemon", 152U, "TopJellypuffJacketLemon"),
		new PermitItems.ItemInfo("top_jellypuffjacket_lime", 153U, "TopJellypuffJacketLime"),
		new PermitItems.ItemInfo("top_jellypuffjacket_satsuma", 154U, "TopJellypuffJacketSatsuma"),
		new PermitItems.ItemInfo("top_jellypuffjacket_strawberry", 155U, "TopJellypuffJacketStrawberry"),
		new PermitItems.ItemInfo("top_jellypuffjacket_watermelon", 156U, "TopJellypuffJacketWatermelon"),
		new PermitItems.ItemInfo("gloves_cuffless_blueberry", 157U, "GlovesCufflessBlueberry"),
		new PermitItems.ItemInfo("gloves_cuffless_grape", 158U, "GlovesCufflessGrape"),
		new PermitItems.ItemInfo("gloves_cuffless_lemon", 159U, "GlovesCufflessLemon"),
		new PermitItems.ItemInfo("gloves_cuffless_lime", 160U, "GlovesCufflessLime"),
		new PermitItems.ItemInfo("gloves_cuffless_satsuma", 161U, "GlovesCufflessSatsuma"),
		new PermitItems.ItemInfo("gloves_cuffless_strawberry", 162U, "GlovesCufflessStrawberry"),
		new PermitItems.ItemInfo("gloves_cuffless_watermelon", 163U, "GlovesCufflessWatermelon"),
		new PermitItems.ItemInfo("flowervase_wall_retro_blue", 164U, "FlowerVaseWall_retro_green"),
		new PermitItems.ItemInfo("flowervase_wall_retro_green", 165U, "FlowerVaseWall_retro_yellow"),
		new PermitItems.ItemInfo("flowervase_wall_retro_red", 166U, "FlowerVaseWall_retro_red"),
		new PermitItems.ItemInfo("flowervase_wall_retro_white", 167U, "FlowerVaseWall_retro_blue"),
		new PermitItems.ItemInfo("flowervase_wall_retro_yellow", 168U, "FlowerVaseWall_retro_white"),
		new PermitItems.ItemInfo("walls_basic_blue_cobalt", 169U, "ExteriorWall_basic_blue_cobalt"),
		new PermitItems.ItemInfo("walls_basic_green_kelly", 170U, "ExteriorWall_basic_green_kelly"),
		new PermitItems.ItemInfo("walls_basic_grey_charcoal", 171U, "ExteriorWall_basic_grey_charcoal"),
		new PermitItems.ItemInfo("walls_basic_orange_satsuma", 172U, "ExteriorWall_basic_orange_satsuma"),
		new PermitItems.ItemInfo("walls_basic_pink_flamingo", 173U, "ExteriorWall_basic_pink_flamingo"),
		new PermitItems.ItemInfo("walls_basic_red_deep", 174U, "ExteriorWall_basic_red_deep"),
		new PermitItems.ItemInfo("walls_basic_yellow_lemon", 175U, "ExteriorWall_basic_yellow_lemon"),
		new PermitItems.ItemInfo("walls_blueberries", 176U, "ExteriorWall_blueberries"),
		new PermitItems.ItemInfo("walls_grapes", 177U, "ExteriorWall_grapes"),
		new PermitItems.ItemInfo("walls_lemon", 178U, "ExteriorWall_lemon"),
		new PermitItems.ItemInfo("walls_lime", 179U, "ExteriorWall_lime"),
		new PermitItems.ItemInfo("walls_satsuma", 180U, "ExteriorWall_satsuma"),
		new PermitItems.ItemInfo("walls_strawberry", 181U, "ExteriorWall_strawberry"),
		new PermitItems.ItemInfo("walls_watermelon", 182U, "ExteriorWall_watermelon"),
		new PermitItems.ItemInfo("balloon_candy_blueberry", 183U, "BalloonCandyBlueberry"),
		new PermitItems.ItemInfo("balloon_candy_grape", 184U, "BalloonCandyGrape"),
		new PermitItems.ItemInfo("balloon_candy_lemon", 185U, "BalloonCandyLemon"),
		new PermitItems.ItemInfo("balloon_candy_lime", 186U, "BalloonCandyLime"),
		new PermitItems.ItemInfo("balloon_candy_orange", 187U, "BalloonCandyOrange"),
		new PermitItems.ItemInfo("balloon_candy_strawberry", 188U, "BalloonCandyStrawberry"),
		new PermitItems.ItemInfo("balloon_candy_watermelon", 189U, "BalloonCandyWatermelon"),
		new PermitItems.ItemInfo("atmo_helmet_puft", 191U, "AtmoHelmetPuft"),
		new PermitItems.ItemInfo("atmo_suit_puft", 192U, "AtmoSuitPuft"),
		new PermitItems.ItemInfo("atmo_gloves_puft", 193U, "AtmoGlovesPuft"),
		new PermitItems.ItemInfo("atmo_belt_puft", 194U, "AtmoBeltPuft"),
		new PermitItems.ItemInfo("atmo_shoes_puft", 195U, "AtmoShoesPuft"),
		new PermitItems.ItemInfo("top_tshirt_white", 197U, "TopTShirtWhite"),
		new PermitItems.ItemInfo("top_tshirt_magenta", 198U, "TopTShirtMagenta"),
		new PermitItems.ItemInfo("top_athlete", 199U, "TopAthlete"),
		new PermitItems.ItemInfo("top_circuit_green", 200U, "TopCircuitGreen"),
		new PermitItems.ItemInfo("gloves_basic_bluegrey", 201U, "GlovesBasicBlueGrey"),
		new PermitItems.ItemInfo("gloves_basic_brown_khaki", 202U, "GlovesBasicBrownKhaki"),
		new PermitItems.ItemInfo("gloves_athlete", 203U, "GlovesAthlete"),
		new PermitItems.ItemInfo("gloves_circuit_green", 204U, "GlovesCircuitGreen"),
		new PermitItems.ItemInfo("pants_basic_redorange", 205U, "PantsBasicRedOrange"),
		new PermitItems.ItemInfo("pants_basic_lightbrown", 206U, "PantsBasicLightBrown"),
		new PermitItems.ItemInfo("pants_athlete", 207U, "PantsAthlete"),
		new PermitItems.ItemInfo("pants_circuit_green", 208U, "PantsCircuitGreen"),
		new PermitItems.ItemInfo("shoes_basic_bluegrey", 209U, "ShoesBasicBlueGrey"),
		new PermitItems.ItemInfo("shoes_basic_tan", 210U, "ShoesBasicTan"),
		new PermitItems.ItemInfo("atmo_helmet_sparkle_red", 211U, "AtmoHelmetSparkleRed"),
		new PermitItems.ItemInfo("atmo_helmet_sparkle_green", 212U, "AtmoHelmetSparkleGreen"),
		new PermitItems.ItemInfo("atmo_helmet_sparkle_blue", 213U, "AtmoHelmetSparkleBlue"),
		new PermitItems.ItemInfo("atmo_helmet_sparkle_purple", 214U, "AtmoHelmetSparklePurple"),
		new PermitItems.ItemInfo("atmosuit_sparkle_red", 215U, "AtmoSuitSparkleRed"),
		new PermitItems.ItemInfo("atmosuit_sparkle_green", 216U, "AtmoSuitSparkleGreen"),
		new PermitItems.ItemInfo("atmosuit_sparkle_blue", 217U, "AtmoSuitSparkleBlue"),
		new PermitItems.ItemInfo("atmosuit_sparkle_lavender", 218U, "AtmoSuitSparkleLavender"),
		new PermitItems.ItemInfo("atmo_gloves_sparkle_red", 219U, "AtmoGlovesSparkleRed"),
		new PermitItems.ItemInfo("atmo_gloves_sparkle_green", 220U, "AtmoGlovesSparkleGreen"),
		new PermitItems.ItemInfo("atmo_gloves_sparkle_blue", 221U, "AtmoGlovesSparkleBlue"),
		new PermitItems.ItemInfo("atmo_gloves_sparkle_lavender", 222U, "AtmoGlovesSparkleLavender"),
		new PermitItems.ItemInfo("atmo_belt_sparkle_red", 223U, "AtmoBeltSparkleRed"),
		new PermitItems.ItemInfo("atmo_belt_sparkle_green", 224U, "AtmoBeltSparkleGreen"),
		new PermitItems.ItemInfo("atmo_belt_sparkle_blue", 225U, "AtmoBeltSparkleBlue"),
		new PermitItems.ItemInfo("atmo_belt_sparkle_lavender", 226U, "AtmoBeltSparkleLavender"),
		new PermitItems.ItemInfo("atmo_shoes_sparkle_black", 227U, "AtmoShoesSparkleBlack"),
		new PermitItems.ItemInfo("flowervase_hanging_retro_red", 228U, "FlowerVaseHanging_retro_red"),
		new PermitItems.ItemInfo("flowervase_hanging_retro_green", 229U, "FlowerVaseHanging_retro_green"),
		new PermitItems.ItemInfo("flowervase_hanging_retro_blue", 230U, "FlowerVaseHanging_retro_blue"),
		new PermitItems.ItemInfo("flowervase_hanging_retro_yellow", 231U, "FlowerVaseHanging_retro_yellow"),
		new PermitItems.ItemInfo("flowervase_hanging_retro_white", 232U, "FlowerVaseHanging_retro_white"),
		new PermitItems.ItemInfo("walls_toiletpaper", 233U, "ExteriorWall_toiletpaper"),
		new PermitItems.ItemInfo("walls_plunger", 234U, "ExteriorWall_plunger"),
		new PermitItems.ItemInfo("walls_tropical", 235U, "ExteriorWall_tropical"),
		new PermitItems.ItemInfo("painting_art_l", 236U, "Canvas_Good10"),
		new PermitItems.ItemInfo("painting_art_m", 237U, "Canvas_Good11"),
		new PermitItems.ItemInfo("painting_tall_art_j", 238U, "CanvasTall_Good8"),
		new PermitItems.ItemInfo("painting_tall_art_k", 239U, "CanvasTall_Good9"),
		new PermitItems.ItemInfo("painting_wide_art_j", 240U, "CanvasWide_Good8"),
		new PermitItems.ItemInfo("painting_wide_art_k", 241U, "CanvasWide_Good9"),
		new PermitItems.ItemInfo("top_denim_blue", 242U, "TopDenimBlue"),
		new PermitItems.ItemInfo("top_undershirt_executive", 243U, "TopUndershirtExecutive"),
		new PermitItems.ItemInfo("top_undershirt_underling", 244U, "TopUndershirtUnderling"),
		new PermitItems.ItemInfo("top_undershirt_groupthink", 245U, "TopUndershirtGroupthink"),
		new PermitItems.ItemInfo("top_undershirt_stakeholder", 246U, "TopUndershirtStakeholder"),
		new PermitItems.ItemInfo("top_undershirt_admin", 247U, "TopUndershirtAdmin"),
		new PermitItems.ItemInfo("top_undershirt_buzzword", 248U, "TopUndershirtBuzzword"),
		new PermitItems.ItemInfo("top_undershirtshirt_synergy", 249U, "TopUndershirtSynergy"),
		new PermitItems.ItemInfo("top_researcher", 250U, "TopResearcher"),
		new PermitItems.ItemInfo("top_rebel_gi", 251U, "TopRebelGi"),
		new PermitItems.ItemInfo("bottom_briefs_executive", 252U, "BottomBriefsExecutive"),
		new PermitItems.ItemInfo("bottom_briefs_underling", 253U, "BottomBriefsUnderling"),
		new PermitItems.ItemInfo("bottom_briefs_groupthink", 254U, "BottomBriefsGroupthink"),
		new PermitItems.ItemInfo("bottom_briefs_stakeholder", 255U, "BottomBriefsStakeholder"),
		new PermitItems.ItemInfo("bottom_briefs_admin", 256U, "BottomBriefsAdmin"),
		new PermitItems.ItemInfo("bottom_briefs_buzzword", 257U, "BottomBriefsBuzzword"),
		new PermitItems.ItemInfo("bottom_briefs_synergy", 258U, "BottomBriefsSynergy"),
		new PermitItems.ItemInfo("pants_jeans", 259U, "PantsJeans"),
		new PermitItems.ItemInfo("pants_rebel_gi", 260U, "PantsRebelGi"),
		new PermitItems.ItemInfo("pants_research", 261U, "PantsResearch"),
		new PermitItems.ItemInfo("shoes_basic_gray", 262U, "ShoesBasicGray"),
		new PermitItems.ItemInfo("shoes_denim_blue", 263U, "ShoesDenimBlue"),
		new PermitItems.ItemInfo("socks_legwarmers_blueberry", 264U, "SocksLegwarmersBlueberry"),
		new PermitItems.ItemInfo("socks_legwarmers_grape", 265U, "SocksLegwarmersGrape"),
		new PermitItems.ItemInfo("socks_legwarmers_lemon", 266U, "SocksLegwarmersLemon"),
		new PermitItems.ItemInfo("socks_legwarmers_lime", 267U, "SocksLegwarmersLime"),
		new PermitItems.ItemInfo("socks_legwarmers_satsuma", 268U, "SocksLegwarmersSatsuma"),
		new PermitItems.ItemInfo("socks_legwarmers_strawberry", 269U, "SocksLegwarmersStrawberry"),
		new PermitItems.ItemInfo("socks_legwarmers_watermelon", 270U, "SocksLegwarmersWatermelon"),
		new PermitItems.ItemInfo("gloves_cuffless_black", 271U, "GlovesCufflessBlack"),
		new PermitItems.ItemInfo("gloves_denim_blue", 272U, "GlovesDenimBlue"),
		new PermitItems.ItemInfo("atmo_gloves_gold", 273U, "AtmoGlovesGold"),
		new PermitItems.ItemInfo("atmo_gloves_eggplant", 274U, "AtmoGlovesEggplant"),
		new PermitItems.ItemInfo("atmo_helmet_eggplant", 275U, "AtmoHelmetEggplant"),
		new PermitItems.ItemInfo("atmo_helmet_confetti", 276U, "AtmoHelmetConfetti"),
		new PermitItems.ItemInfo("atmo_shoes_stealth", 277U, "AtmoShoesStealth"),
		new PermitItems.ItemInfo("atmo_shoes_eggplant", 278U, "AtmoShoesEggplant"),
		new PermitItems.ItemInfo("atmosuit_crisp_eggplant", 279U, "AtmoSuitCrispEggplant"),
		new PermitItems.ItemInfo("atmosuit_confetti", 280U, "AtmoSuitConfetti"),
		new PermitItems.ItemInfo("atmo_belt_basic_gold", 281U, "AtmoBeltBasicGold"),
		new PermitItems.ItemInfo("atmo_belt_eggplant", 282U, "AtmoBeltEggplant"),
		new PermitItems.ItemInfo("item_pedestal_hand", 283U, "ItemPedestal_hand"),
		new PermitItems.ItemInfo("massage_table_shiatsu", 284U, "MassageTable_shiatsu"),
		new PermitItems.ItemInfo("rock_crusher_hands", 285U, "RockCrusher_hands"),
		new PermitItems.ItemInfo("rock_crusher_teeth", 286U, "RockCrusher_teeth"),
		new PermitItems.ItemInfo("water_cooler_round_body", 287U, "WaterCooler_round_body"),
		new PermitItems.ItemInfo("walls_stripes_blue", 288U, "ExteriorWall_stripes_blue"),
		new PermitItems.ItemInfo("walls_stripes_diagonal_blue", 289U, "ExteriorWall_stripes_diagonal_blue"),
		new PermitItems.ItemInfo("walls_stripes_circle_blue", 290U, "ExteriorWall_stripes_circle_blue"),
		new PermitItems.ItemInfo("walls_squares_red_deep_white", 291U, "ExteriorWall_squares_red_deep_white"),
		new PermitItems.ItemInfo("walls_squares_orange_satsuma_white", 292U, "ExteriorWall_squares_orange_satsuma_white"),
		new PermitItems.ItemInfo("walls_squares_yellow_lemon_white", 293U, "ExteriorWall_squares_yellow_lemon_white"),
		new PermitItems.ItemInfo("walls_squares_green_kelly_white", 294U, "ExteriorWall_squares_green_kelly_white"),
		new PermitItems.ItemInfo("walls_squares_blue_cobalt_white", 295U, "ExteriorWall_squares_blue_cobalt_white"),
		new PermitItems.ItemInfo("walls_squares_pink_flamingo_white", 296U, "ExteriorWall_squares_pink_flamingo_white"),
		new PermitItems.ItemInfo("walls_squares_grey_charcoal_white", 297U, "ExteriorWall_squares_grey_charcoal_white"),
		new PermitItems.ItemInfo("canvas_good_13", 298U, "Canvas_Good13"),
		new PermitItems.ItemInfo("canvas_wide_good_10", 299U, "CanvasWide_Good10"),
		new PermitItems.ItemInfo("canvas_tall_good_11", 300U, "CanvasTall_Good11"),
		new PermitItems.ItemInfo("sculpture_good_5", 301U, "Sculpture_Good5"),
		new PermitItems.ItemInfo("small_sculpture_good_5", 302U, "SmallSculpture_Good5"),
		new PermitItems.ItemInfo("small_sculpture_good_6", 303U, "SmallSculpture_Good6"),
		new PermitItems.ItemInfo("metal_sculpture_good_5", 304U, "MetalSculpture_Good5"),
		new PermitItems.ItemInfo("ice_sculpture_average_3", 305U, "IceSculpture_Average3"),
		new PermitItems.ItemInfo("skirt_basic_blue_middle", 306U, "SkirtBasicBlueMiddle"),
		new PermitItems.ItemInfo("skirt_basic_purple", 307U, "SkirtBasicPurple"),
		new PermitItems.ItemInfo("skirt_basic_green", 308U, "SkirtBasicGreen"),
		new PermitItems.ItemInfo("skirt_basic_orange", 309U, "SkirtBasicOrange"),
		new PermitItems.ItemInfo("skirt_basic_pink_orchid", 310U, "SkirtBasicPinkOrchid"),
		new PermitItems.ItemInfo("skirt_basic_red", 311U, "SkirtBasicRed"),
		new PermitItems.ItemInfo("skirt_basic_yellow", 312U, "SkirtBasicYellow"),
		new PermitItems.ItemInfo("skirt_basic_polkadot", 313U, "SkirtBasicPolkadot"),
		new PermitItems.ItemInfo("skirt_basic_watermelon", 314U, "SkirtBasicWatermelon"),
		new PermitItems.ItemInfo("skirt_denim_blue", 315U, "SkirtDenimBlue"),
		new PermitItems.ItemInfo("skirt_leopard_print_blue_pink", 316U, "SkirtLeopardPrintBluePink"),
		new PermitItems.ItemInfo("skirt_sparkle_blue", 317U, "SkirtSparkleBlue"),
		new PermitItems.ItemInfo("atmo_belt_basic_grey", 318U, "AtmoBeltBasicGrey"),
		new PermitItems.ItemInfo("atmo_belt_basic_neon_pink", 319U, "AtmoBeltBasicNeonPink"),
		new PermitItems.ItemInfo("atmo_gloves_white", 320U, "AtmoGlovesWhite"),
		new PermitItems.ItemInfo("atmo_gloves_stripes_lavender", 321U, "AtmoGlovesStripesLavender"),
		new PermitItems.ItemInfo("atmo_helmet_cummerbund_red", 322U, "AtmoHelmetCummerbundRed"),
		new PermitItems.ItemInfo("atmo_helmet_workout_lavender", 323U, "AtmoHelmetWorkoutLavender"),
		new PermitItems.ItemInfo("atmo_shoes_basic_lavender", 324U, "AtmoShoesBasicLavender"),
		new PermitItems.ItemInfo("atmosuit_basic_neon_pink", 325U, "AtmoSuitBasicNeonPink"),
		new PermitItems.ItemInfo("atmosuit_multi_red_black", 326U, "AtmoSuitMultiRedBlack"),
		new PermitItems.ItemInfo("egg_cracker_beaker", 327U, "EggCracker_beaker"),
		new PermitItems.ItemInfo("egg_cracker_flower", 328U, "EggCracker_flower"),
		new PermitItems.ItemInfo("egg_cracker_hands", 329U, "EggCracker_hands"),
		new PermitItems.ItemInfo("ceilinglight_rubiks", 330U, "CeilingLight_rubiks"),
		new PermitItems.ItemInfo("flowervase_hanging_beaker", 331U, "FlowerVaseHanging_beaker"),
		new PermitItems.ItemInfo("flowervase_hanging_rubiks", 332U, "FlowerVaseHanging_rubiks"),
		new PermitItems.ItemInfo("elegantbed_hand", 333U, "LuxuryBed_hand"),
		new PermitItems.ItemInfo("elegantbed_rubiks", 334U, "LuxuryBed_rubiks"),
		new PermitItems.ItemInfo("rock_crusher_roundstamp", 335U, "RockCrusher_roundstamp"),
		new PermitItems.ItemInfo("rock_crusher_spikebeds", 336U, "RockCrusher_spikebeds"),
		new PermitItems.ItemInfo("storagelocker_green_mush", 337U, "StorageLocker_green_mush"),
		new PermitItems.ItemInfo("storagelocker_red_rose", 338U, "StorageLocker_red_rose"),
		new PermitItems.ItemInfo("storagelocker_blue_babytears", 339U, "StorageLocker_blue_babytears"),
		new PermitItems.ItemInfo("storagelocker_purple_brainfat", 340U, "StorageLocker_purple_brainfat"),
		new PermitItems.ItemInfo("storagelocker_yellow_tartar", 341U, "StorageLocker_yellow_tartar"),
		new PermitItems.ItemInfo("planterbox_mealwood", 342U, "PlanterBox_mealwood"),
		new PermitItems.ItemInfo("planterbox_bristleblossom", 343U, "PlanterBox_bristleblossom"),
		new PermitItems.ItemInfo("planterbox_wheezewort", 344U, "PlanterBox_wheezewort"),
		new PermitItems.ItemInfo("planterbox_sleetwheat", 345U, "PlanterBox_sleetwheat"),
		new PermitItems.ItemInfo("planterbox_salmon_pink", 346U, "PlanterBox_salmon_pink"),
		new PermitItems.ItemInfo("gasstorage_lightgold", 347U, "GasReservoir_lightgold"),
		new PermitItems.ItemInfo("gasstorage_peagreen", 348U, "GasReservoir_peagreen"),
		new PermitItems.ItemInfo("gasstorage_lightcobalt", 349U, "GasReservoir_lightcobalt"),
		new PermitItems.ItemInfo("gasstorage_polka_darkpurpleresin", 350U, "GasReservoir_polka_darkpurpleresin"),
		new PermitItems.ItemInfo("gasstorage_polka_darknavynookgreen", 351U, "GasReservoir_polka_darknavynookgreen"),
		new PermitItems.ItemInfo("walls_kitchen_retro1", 352U, "ExteriorWall_kitchen_retro1"),
		new PermitItems.ItemInfo("walls_plus_red_deep_white", 353U, "ExteriorWall_plus_red_deep_white"),
		new PermitItems.ItemInfo("walls_plus_orange_satsuma_white", 354U, "ExteriorWall_plus_orange_satsuma_white"),
		new PermitItems.ItemInfo("walls_plus_yellow_lemon_white", 355U, "ExteriorWall_plus_yellow_lemon_white"),
		new PermitItems.ItemInfo("walls_plus_green_kelly_white", 356U, "ExteriorWall_plus_green_kelly_white"),
		new PermitItems.ItemInfo("walls_plus_blue_cobalt_white", 357U, "ExteriorWall_plus_blue_cobalt_white"),
		new PermitItems.ItemInfo("walls_plus_pink_flamingo_white", 358U, "ExteriorWall_plus_pink_flamingo_white"),
		new PermitItems.ItemInfo("walls_plus_grey_charcoal_white", 359U, "ExteriorWall_plus_grey_charcoal_white"),
		new PermitItems.ItemInfo("painting_art_n", 360U, "Canvas_Good12"),
		new PermitItems.ItemInfo("painting_art_p", 361U, "Canvas_Good14"),
		new PermitItems.ItemInfo("painting_wide_art_m", 362U, "CanvasWide_Good11"),
		new PermitItems.ItemInfo("painting_tall_art_l", 363U, "CanvasTall_Good10"),
		new PermitItems.ItemInfo("sculpture_amazing_6", 364U, "Sculpture_Good6"),
		new PermitItems.ItemInfo("balloon_hand_gold", 365U, "BalloonHandGold"),
		new PermitItems.ItemInfo("atmo_belt_cantaloupe", 367U, "AtmoBeltRocketmelon"),
		new PermitItems.ItemInfo("atmosuit_cantaloupe", 368U, "AtmoSuitRocketmelon"),
		new PermitItems.ItemInfo("atmo_gloves_cantaloupe", 369U, "AtmoGlovesRocketmelon"),
		new PermitItems.ItemInfo("atmo_helmet_cantaloupe", 370U, "AtmoHelmetRocketmelon"),
		new PermitItems.ItemInfo("atmo_shoes_cantaloupe", 371U, "AtmoBootsRocketmelon"),
		new PermitItems.ItemInfo("pants_basic_orange_satsuma", 372U, "PantsBasicOrangeSatsuma"),
		new PermitItems.ItemInfo("pants_pinstripe_slate", 373U, "PantsPinstripeSlate"),
		new PermitItems.ItemInfo("pants_velour_black", 374U, "PantsVelourBlack"),
		new PermitItems.ItemInfo("pants_velour_blue", 375U, "PantsVelourBlue"),
		new PermitItems.ItemInfo("pants_velour_pink", 376U, "PantsVelourPink"),
		new PermitItems.ItemInfo("skirt_ballerina_pink", 377U, "SkirtBallerinaPink"),
		new PermitItems.ItemInfo("skirt_tweed_pink_orchid", 378U, "SkirtTweedPinkOrchid"),
		new PermitItems.ItemInfo("shoes_ballerina_pink", 379U, "ShoesBallerinaPink"),
		new PermitItems.ItemInfo("shoes_maryjane_socks_bw", 380U, "ShoesMaryjaneSocksBw"),
		new PermitItems.ItemInfo("shoes_classicflats_cream_charcoal", 381U, "ShoesClassicFlatsCreamCharcoal"),
		new PermitItems.ItemInfo("shoes_velour_blue", 382U, "ShoesVelourBlue"),
		new PermitItems.ItemInfo("shoes_velour_pink", 383U, "ShoesVelourPink"),
		new PermitItems.ItemInfo("shoes_velour_black", 384U, "ShoesVelourBlack"),
		new PermitItems.ItemInfo("gloves_basic_grey", 385U, "GlovesBasicGrey"),
		new PermitItems.ItemInfo("gloves_basic_pinksalmon", 386U, "GlovesBasicPinksalmon"),
		new PermitItems.ItemInfo("gloves_basic_tan", 387U, "GlovesBasicTan"),
		new PermitItems.ItemInfo("gloves_ballerina_pink", 388U, "GlovesBallerinaPink"),
		new PermitItems.ItemInfo("gloves_formal_white", 389U, "GlovesFormalWhite"),
		new PermitItems.ItemInfo("gloves_long_white", 390U, "GlovesLongWhite"),
		new PermitItems.ItemInfo("gloves_2tone_cream_charcoal", 391U, "Gloves2ToneCreamCharcoal"),
		new PermitItems.ItemInfo("top_jacket_smoking_burgundy", 392U, "TopJacketSmokingBurgundy"),
		new PermitItems.ItemInfo("top_mechanic", 393U, "TopMechanic"),
		new PermitItems.ItemInfo("top_velour_black", 394U, "TopVelourBlack"),
		new PermitItems.ItemInfo("top_velour_blue", 395U, "TopVelourBlue"),
		new PermitItems.ItemInfo("top_velour_pink", 396U, "TopVelourPink"),
		new PermitItems.ItemInfo("top_waistcoat_pinstripe_slate", 397U, "TopWaistcoatPinstripeSlate"),
		new PermitItems.ItemInfo("top_water", 398U, "TopWater"),
		new PermitItems.ItemInfo("top_tweed_pink_orchid", 399U, "TopTweedPinkOrchid"),
		new PermitItems.ItemInfo("dress_sleeveless_bow_bw", 400U, "DressSleevelessBowBw"),
		new PermitItems.ItemInfo("bodysuit_ballerina_pink", 401U, "BodysuitBallerinaPink"),
		new PermitItems.ItemInfo("rock_crusher_chomp", 402U, "RockCrusher_chomp"),
		new PermitItems.ItemInfo("rock_crusher_gears", 403U, "RockCrusher_gears"),
		new PermitItems.ItemInfo("rock_crusher_balloon", 404U, "RockCrusher_balloon"),
		new PermitItems.ItemInfo("storagelocker_polka_darknavynookgreen", 405U, "StorageLocker_polka_darknavynookgreen"),
		new PermitItems.ItemInfo("storagelocker_polka_darkpurpleresin", 406U, "StorageLocker_polka_darkpurpleresin"),
		new PermitItems.ItemInfo("gasstorage_blue_babytears", 407U, "GasReservoir_blue_babytears"),
		new PermitItems.ItemInfo("gasstorage_yellow_tartar", 408U, "GasReservoir_yellow_tartar"),
		new PermitItems.ItemInfo("gasstorage_green_mush", 409U, "GasReservoir_green_mush"),
		new PermitItems.ItemInfo("gasstorage_red_rose", 410U, "GasReservoir_red_rose"),
		new PermitItems.ItemInfo("gasstorage_purple_brainfat", 411U, "GasReservoir_purple_brainfat"),
		new PermitItems.ItemInfo("masseur_balloon", 412U, "MassageTable_balloon"),
		new PermitItems.ItemInfo("watercooler_balloon", 413U, "WaterCooler_balloon"),
		new PermitItems.ItemInfo("top_x_sporchid", 415U, "TopXSporchid"),
		new PermitItems.ItemInfo("top_x1_pinchapeppernutbells", 416U, "TopX1Pinchapeppernutbells"),
		new PermitItems.ItemInfo("top_pompom_shinebugs_pink_peppernut", 417U, "TopPompomShinebugsPinkPeppernut"),
		new PermitItems.ItemInfo("top_snowflake_blue", 418U, "TopSnowflakeBlue"),
		new PermitItems.ItemInfo("bed_stringlights", 419U, "Bed_stringlights"),
		new PermitItems.ItemInfo("corner_tile_shineornaments", 420U, "CornerMoulding_shineornaments"),
		new PermitItems.ItemInfo("crown_moulding_shineornaments", 421U, "CrownMoulding_shineornaments"),
		new PermitItems.ItemInfo("floorlamp_leg", 422U, "FloorLamp_leg"),
		new PermitItems.ItemInfo("floorlamp_bristle_blossom", 423U, "FloorLamp_bristle_blossom"),
		new PermitItems.ItemInfo("storagelocker_stripes_red_white", 424U, "StorageLocker_stripes_red_white"),
		new PermitItems.ItemInfo("fridge_stripes_red_white", 425U, "Refrigerator_stripes_red_white"),
		new PermitItems.ItemInfo("walls_stripes_rose", 426U, "ExteriorWall_stripes_rose"),
		new PermitItems.ItemInfo("walls_stripes_diagonal_rose", 427U, "ExteriorWall_stripes_diagonal_rose"),
		new PermitItems.ItemInfo("walls_stripes_circle_rose", 428U, "ExteriorWall_stripes_circle_rose"),
		new PermitItems.ItemInfo("walls_stripes_mush", 429U, "ExteriorWall_stripes_mush"),
		new PermitItems.ItemInfo("walls_stripes_diagonal_mush", 430U, "ExteriorWall_stripes_diagonal_mush"),
		new PermitItems.ItemInfo("walls_stripes_circle_mush", 431U, "ExteriorWall_stripes_circle_mush"),
		new PermitItems.ItemInfo("painting_art_q", 432U, "Canvas_Good15"),
		new PermitItems.ItemInfo("painting_tall_art_p", 433U, "CanvasTall_Good14"),
		new PermitItems.ItemInfo("atmo_belt_2tone_brown", 434U, "AtmoBeltTwoToneBrown"),
		new PermitItems.ItemInfo("atmosuit_multi_blue_grey_black", 435U, "AtmoSuitMultiBlueGreyBlack"),
		new PermitItems.ItemInfo("atmosuit_multi_blue_yellow_red", 436U, "AtmoSuitMultiBlueYellowRed"),
		new PermitItems.ItemInfo("atmo_gloves_brown", 437U, "AtmoGlovesBrown"),
		new PermitItems.ItemInfo("atmo_helmet_mondrian_blue_red_yellow", 438U, "AtmoHelmetMondrianBlueRedYellow"),
		new PermitItems.ItemInfo("atmo_helmet_overalls_red", 439U, "AtmoHelmetOverallsRed"),
		new PermitItems.ItemInfo("pj_clovers_glitch_kelly", 440U, "PjCloversGlitchKelly"),
		new PermitItems.ItemInfo("pj_hearts_chilli_strawberry", 441U, "PjHeartsChilliStrawberry"),
		new PermitItems.ItemInfo("bottom_ginch_pink_gluon", 442U, "BottomGinchPinkGluon"),
		new PermitItems.ItemInfo("bottom_ginch_purple_cortex", 443U, "BottomGinchPurpleCortex"),
		new PermitItems.ItemInfo("bottom_ginch_blue_frosty", 444U, "BottomGinchBlueFrosty"),
		new PermitItems.ItemInfo("bottom_ginch_teal_locus", 445U, "BottomGinchTealLocus"),
		new PermitItems.ItemInfo("bottom_ginch_green_goop", 446U, "BottomGinchGreenGoop"),
		new PermitItems.ItemInfo("bottom_ginch_yellow_bile", 447U, "BottomGinchYellowBile"),
		new PermitItems.ItemInfo("bottom_ginch_orange_nybble", 448U, "BottomGinchOrangeNybble"),
		new PermitItems.ItemInfo("bottom_ginch_red_ironbow", 449U, "BottomGinchRedIronbow"),
		new PermitItems.ItemInfo("bottom_ginch_grey_phlegm", 450U, "BottomGinchGreyPhlegm"),
		new PermitItems.ItemInfo("bottom_ginch_grey_obelus", 451U, "BottomGinchGreyObelus"),
		new PermitItems.ItemInfo("pants_knit_polkadot_turq", 452U, "PantsKnitPolkadotTurq"),
		new PermitItems.ItemInfo("pants_gi_belt_white_black", 453U, "PantsGiBeltWhiteBlack"),
		new PermitItems.ItemInfo("pants_belt_khaki_tan", 454U, "PantsBeltKhakiTan"),
		new PermitItems.ItemInfo("shoes_flashy", 455U, "ShoesFlashy"),
		new PermitItems.ItemInfo("socks_ginch_pink_saltrock", 456U, "SocksGinchPinkSaltrock"),
		new PermitItems.ItemInfo("socks_ginch_purple_dusky", 457U, "SocksGinchPurpleDusky"),
		new PermitItems.ItemInfo("socks_ginch_blue_basin", 458U, "SocksGinchBlueBasin"),
		new PermitItems.ItemInfo("socks_ginch_teal_balmy", 459U, "SocksGinchTealBalmy"),
		new PermitItems.ItemInfo("socks_ginch_green_lime", 460U, "SocksGinchGreenLime"),
		new PermitItems.ItemInfo("socks_ginch_yellow_yellowcake", 461U, "SocksGinchYellowYellowcake"),
		new PermitItems.ItemInfo("socks_ginch_orange_atomic", 462U, "SocksGinchOrangeAtomic"),
		new PermitItems.ItemInfo("socks_ginch_red_magma", 463U, "SocksGinchRedMagma"),
		new PermitItems.ItemInfo("socks_ginch_grey_grey", 464U, "SocksGinchGreyGrey"),
		new PermitItems.ItemInfo("socks_ginch_grey_charcoal", 465U, "SocksGinchGreyCharcoal"),
		new PermitItems.ItemInfo("gloves_basic_slate", 466U, "GlovesBasicSlate"),
		new PermitItems.ItemInfo("gloves_knit_gold", 467U, "GlovesKnitGold"),
		new PermitItems.ItemInfo("gloves_knit_magenta", 468U, "GlovesKnitMagenta"),
		new PermitItems.ItemInfo("gloves_sparkle_white", 469U, "GlovesSparkleWhite"),
		new PermitItems.ItemInfo("gloves_ginch_pink_saltrock", 470U, "GlovesGinchPinkSaltrock"),
		new PermitItems.ItemInfo("gloves_ginch_purple_dusky", 471U, "GlovesGinchPurpleDusky"),
		new PermitItems.ItemInfo("gloves_ginch_blue_basin", 472U, "GlovesGinchBlueBasin"),
		new PermitItems.ItemInfo("gloves_ginch_teal_balmy", 473U, "GlovesGinchTealBalmy"),
		new PermitItems.ItemInfo("gloves_ginch_green_lime", 474U, "GlovesGinchGreenLime"),
		new PermitItems.ItemInfo("gloves_ginch_yellow_yellowcake", 475U, "GlovesGinchYellowYellowcake"),
		new PermitItems.ItemInfo("gloves_ginch_orange_atomic", 476U, "GlovesGinchOrangeAtomic"),
		new PermitItems.ItemInfo("gloves_ginch_red_magma", 477U, "GlovesGinchRedMagma"),
		new PermitItems.ItemInfo("gloves_ginch_grey_grey", 478U, "GlovesGinchGreyGrey"),
		new PermitItems.ItemInfo("gloves_ginch_grey_charcoal", 479U, "GlovesGinchGreyCharcoal"),
		new PermitItems.ItemInfo("top_builder", 480U, "TopBuilder"),
		new PermitItems.ItemInfo("top_floral_pink", 481U, "TopFloralPink"),
		new PermitItems.ItemInfo("top_ginch_pink_saltrock", 482U, "TopGinchPinkSaltrock"),
		new PermitItems.ItemInfo("top_ginch_purple_dusky", 483U, "TopGinchPurpleDusky"),
		new PermitItems.ItemInfo("top_ginch_blue_basin", 484U, "TopGinchBlueBasin"),
		new PermitItems.ItemInfo("top_ginch_teal_balmy", 485U, "TopGinchTealBalmy"),
		new PermitItems.ItemInfo("top_ginch_green_lime", 486U, "TopGinchGreenLime"),
		new PermitItems.ItemInfo("top_ginch_yellow_yellowcake", 487U, "TopGinchYellowYellowcake"),
		new PermitItems.ItemInfo("top_ginch_orange_atomic", 488U, "TopGinchOrangeAtomic"),
		new PermitItems.ItemInfo("top_ginch_red_magma", 489U, "TopGinchRedMagma"),
		new PermitItems.ItemInfo("top_ginch_grey_grey", 490U, "TopGinchGreyGrey"),
		new PermitItems.ItemInfo("top_ginch_grey_charcoal", 491U, "TopGinchGreyCharcoal"),
		new PermitItems.ItemInfo("top_knit_polkadot_turq", 492U, "TopKnitPolkadotTurq"),
		new PermitItems.ItemInfo("top_flashy", 493U, "TopFlashy"),
		new PermitItems.ItemInfo("fridge_blue_babytears", 494U, "Refrigerator_blue_babytears"),
		new PermitItems.ItemInfo("fridge_green_mush", 495U, "Refrigerator_green_mush"),
		new PermitItems.ItemInfo("fridge_red_rose", 496U, "Refrigerator_red_rose"),
		new PermitItems.ItemInfo("fridge_yellow_tartar", 497U, "Refrigerator_yellow_tartar"),
		new PermitItems.ItemInfo("fridge_purple_brainfat", 498U, "Refrigerator_purple_brainfat"),
		new PermitItems.ItemInfo("microbemusher_purple_brainfat", 499U, "MicrobeMusher_purple_brainfat"),
		new PermitItems.ItemInfo("microbemusher_yellow_tartar", 500U, "MicrobeMusher_yellow_tartar"),
		new PermitItems.ItemInfo("microbemusher_red_rose", 501U, "MicrobeMusher_red_rose"),
		new PermitItems.ItemInfo("microbemusher_green_mush", 502U, "MicrobeMusher_green_mush"),
		new PermitItems.ItemInfo("microbemusher_blue_babytears", 503U, "MicrobeMusher_blue_babytears"),
		new PermitItems.ItemInfo("wash_sink_purple_brainfat", 504U, "WashSink_purple_brainfat"),
		new PermitItems.ItemInfo("wash_sink_blue_babytears", 505U, "WashSink_blue_babytears"),
		new PermitItems.ItemInfo("wash_sink_green_mush", 506U, "WashSink_green_mush"),
		new PermitItems.ItemInfo("wash_sink_yellow_tartar", 507U, "WashSink_yellow_tartar"),
		new PermitItems.ItemInfo("wash_sink_red_rose", 508U, "WashSink_red_rose"),
		new PermitItems.ItemInfo("toiletflush_polka_darkpurpleresin", 509U, "FlushToilet_polka_darkpurpleresin"),
		new PermitItems.ItemInfo("toiletflush_polka_darknavynookgreen", 510U, "FlushToilet_polka_darknavynookgreen"),
		new PermitItems.ItemInfo("toiletflush_purple_brainfat", 511U, "FlushToilet_purple_brainfat"),
		new PermitItems.ItemInfo("toiletflush_yellow_tartar", 512U, "FlushToilet_yellow_tartar"),
		new PermitItems.ItemInfo("toiletflush_red_rose", 513U, "FlushToilet_red_rose"),
		new PermitItems.ItemInfo("toiletflush_green_mush", 514U, "FlushToilet_green_mush"),
		new PermitItems.ItemInfo("toiletflush_blue_babytears", 515U, "FlushToilet_blue_babytears"),
		new PermitItems.ItemInfo("elegantbed_red_rose", 516U, "LuxuryBed_red_rose"),
		new PermitItems.ItemInfo("elegantbed_green_mush", 517U, "LuxuryBed_green_mush"),
		new PermitItems.ItemInfo("elegantbed_yellow_tartar", 518U, "LuxuryBed_yellow_tartar"),
		new PermitItems.ItemInfo("elegantbed_purple_brainfat", 519U, "LuxuryBed_purple_brainfat"),
		new PermitItems.ItemInfo("watercooler_yellow_tartar", 520U, "WaterCooler_yellow_tartar"),
		new PermitItems.ItemInfo("watercooler_red_rose", 521U, "WaterCooler_red_rose"),
		new PermitItems.ItemInfo("watercooler_green_mush", 522U, "WaterCooler_green_mush"),
		new PermitItems.ItemInfo("watercooler_purple_brainfat", 523U, "WaterCooler_purple_brainfat"),
		new PermitItems.ItemInfo("watercooler_blue_babytears", 524U, "WaterCooler_blue_babytears"),
		new PermitItems.ItemInfo("walls_stripes_yellow_tartar", 525U, "ExteriorWall_stripes_yellow_tartar"),
		new PermitItems.ItemInfo("walls_stripes_diagonal_yellow_tartar", 526U, "ExteriorWall_stripes_diagonal_yellow_tartar"),
		new PermitItems.ItemInfo("walls_stripes_circle_yellow_tartar", 527U, "ExteriorWall_stripes_circle_yellow_tartar"),
		new PermitItems.ItemInfo("walls_stripes_purple_brainfat", 528U, "ExteriorWall_stripes_purple_brainfat"),
		new PermitItems.ItemInfo("walls_stripes_diagonal_purple_brainfat", 529U, "ExteriorWall_stripes_diagonal_purple_brainfat"),
		new PermitItems.ItemInfo("walls_stripes_circle_purple_brainfat", 530U, "ExteriorWall_stripes_circle_purple_brainfat"),
		new PermitItems.ItemInfo("walls_floppy_azulene_vitro", 531U, "ExteriorWall_floppy_azulene_vitro"),
		new PermitItems.ItemInfo("walls_floppy_black_white", 532U, "ExteriorWall_floppy_black_white"),
		new PermitItems.ItemInfo("walls_floppy_peagreen_balmy", 533U, "ExteriorWall_floppy_peagreen_balmy"),
		new PermitItems.ItemInfo("walls_floppy_satsuma_yellowcake", 534U, "ExteriorWall_floppy_satsuma_yellowcake"),
		new PermitItems.ItemInfo("walls_floppy_magma_amino", 535U, "ExteriorWall_floppy_magma_amino"),
		new PermitItems.ItemInfo("walls_orange_juice", 536U, "ExteriorWall_orange_juice"),
		new PermitItems.ItemInfo("walls_paint_blots", 537U, "ExteriorWall_paint_blots"),
		new PermitItems.ItemInfo("walls_telescope", 538U, "ExteriorWall_telescope"),
		new PermitItems.ItemInfo("walls_tictactoe_o", 539U, "ExteriorWall_tictactoe_o"),
		new PermitItems.ItemInfo("walls_tictactoe_x", 540U, "ExteriorWall_tictactoe_x"),
		new PermitItems.ItemInfo("walls_dice_1", 541U, "ExteriorWall_dice_1"),
		new PermitItems.ItemInfo("walls_dice_2", 542U, "ExteriorWall_dice_2"),
		new PermitItems.ItemInfo("walls_dice_3", 543U, "ExteriorWall_dice_3"),
		new PermitItems.ItemInfo("walls_dice_4", 544U, "ExteriorWall_dice_4"),
		new PermitItems.ItemInfo("walls_dice_5", 545U, "ExteriorWall_dice_5"),
		new PermitItems.ItemInfo("walls_dice_6", 546U, "ExteriorWall_dice_6"),
		new PermitItems.ItemInfo("painting_art_r", 547U, "Canvas_Good16"),
		new PermitItems.ItemInfo("painting_wide_art_o", 548U, "CanvasWide_Good13")
	};

	private static Dictionary<string, PermitItems.ItemInfo> Mappings = PermitItems.ItemInfos.ToDictionary<PermitItems.ItemInfo, string>((PermitItems.ItemInfo x) => x.PermitId);

	private static Dictionary<string, string> ItemToPermit = PermitItems.ItemInfos.ToDictionary<PermitItems.ItemInfo, string, string>((PermitItems.ItemInfo x) => x.ItemType, (PermitItems.ItemInfo x) => x.PermitId);

	private static PermitItems.BoxInfo[] BoxInfos = new PermitItems.BoxInfo[]
	{
		new PermitItems.BoxInfo("MYSTERYBOX_u44_box_a", "Shipment X", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 80U, "ONI_giftbox_u44_box_a", true),
		new PermitItems.BoxInfo("MYSTERYBOX_u44_box_b", "Shipment Y", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 81U, "ONI_giftbox_u44_box_b", true),
		new PermitItems.BoxInfo("MYSTERYBOX_u44_box_c", "Shipment Z", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 82U, "ONI_giftbox_u44_box_c", true),
		new PermitItems.BoxInfo("MYSTERYBOX_u45_box_a", "Team Players Crate", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 148U, "ONI_giftbox_u44_box_b", true),
		new PermitItems.BoxInfo("MYSTERYBOX_u45_box_b", "Pizzazz Crate", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 149U, "ONI_giftbox_u44_box_c", true),
		new PermitItems.BoxInfo("MYSTERYBOX_u46_box_a", "Superfruits Crate", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 190U, "ONI_giftbox_u44_box_a", true),
		new PermitItems.BoxInfo("MYSTERYBOX_u47_klei_fest", EQUIPMENT.PREFABS.ATMO_SUIT_SET.PUFT.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SET.PUFT.DESC, 196U, "ONI_box_puft_atmo_set", false),
		new PermitItems.BoxInfo("MYSTERYBOX_u50_winter_holiday", EQUIPMENT.PREFABS.HOLIDAY_2023_CRATE.NAME, EQUIPMENT.PREFABS.HOLIDAY_2023_CRATE.DESC, 414U, "Holiday_2023_gift_box", false)
	};

	private const string MYSTERYBOX_U44_DESC = "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.";

	private const string MYSTERYBOX_U45_DESC = "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.";

	private const string MYSTERYBOX_U46_DESC = "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.";

	private static Dictionary<string, PermitItems.BoxInfo> BoxMappings = PermitItems.BoxInfos.ToDictionary<PermitItems.BoxInfo, string>((PermitItems.BoxInfo x) => x.ItemType);

	private static HashSet<string> BoxSet = new HashSet<string>(PermitItems.BoxInfos.Select<PermitItems.BoxInfo, string>((PermitItems.BoxInfo x) => x.ItemType));

	private struct ItemInfo
	{
		public ItemInfo(string itemType, uint typeId, string permitId)
		{
			this.ItemType = itemType;
			this.PermitId = permitId;
			this.TypeId = typeId;
		}

		public string ItemType;

		public uint TypeId;

		public string PermitId;
	}

	private struct BoxInfo
	{
		public BoxInfo(string type, string name, string desc, uint id, string icon, bool account_reward)
		{
			this.ItemType = type;
			this.Name = name;
			this.Description = desc;
			this.TypeId = id;
			this.IconName = icon;
			this.AccountReward = account_reward;
		}

		public string ItemType;

		public string Name;

		public string Description;

		public uint TypeId;

		public string IconName;

		public bool AccountReward;
	}
}
