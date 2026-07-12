using System;
using System.Collections.Generic;
using System.Linq;
using Database;

public class PermitItems
{
	public static IEnumerable<KleiItems.ItemData> IterateInventory()
	{
		foreach (KleiItems.ItemData itemData in KleiItems.IterateInventory(PermitItems.ItemToPermit))
		{
			yield return itemData;
		}
		IEnumerator<KleiItems.ItemData> enumerator = null;
		yield break;
		yield break;
	}

	public static bool HasUnopenedItem()
	{
		return KleiItems.HasUnopenedItem(PermitItems.ItemToPermit);
	}

	public static bool HasUnclaimedRewards()
	{
		return KleiItems.HasUnclaimedRewards(PermitItems.ClaimableRewardSet);
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
		new PermitItems.ItemInfo("balloon_candy_watermelon", 189U, "BalloonCandyWatermelon")
	};

	private static Dictionary<string, PermitItems.ItemInfo> Mappings = PermitItems.ItemInfos.ToDictionary<PermitItems.ItemInfo, string>((PermitItems.ItemInfo x) => x.PermitId);

	private static Dictionary<string, string> ItemToPermit = PermitItems.ItemInfos.ToDictionary<PermitItems.ItemInfo, string, string>((PermitItems.ItemInfo x) => x.ItemType, (PermitItems.ItemInfo x) => x.PermitId);

	private static PermitItems.BoxInfo[] BoxInfos = new PermitItems.BoxInfo[]
	{
		new PermitItems.BoxInfo("MYSTERYBOX_u44_box_a", "Shipment X", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 80U, "ONI_giftbox_u44_box_a"),
		new PermitItems.BoxInfo("MYSTERYBOX_u44_box_b", "Shipment Y", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 81U, "ONI_giftbox_u44_box_b"),
		new PermitItems.BoxInfo("MYSTERYBOX_u44_box_c", "Shipment Z", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 82U, "ONI_giftbox_u44_box_c"),
		new PermitItems.BoxInfo("MYSTERYBOX_u45_box_a", "Team Players Crate", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 148U, "ONI_giftbox_u44_box_b"),
		new PermitItems.BoxInfo("MYSTERYBOX_u45_box_b", "Pizzazz Crate", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 149U, "ONI_giftbox_u44_box_c"),
		new PermitItems.BoxInfo("MYSTERYBOX_u46_box_a", "Superfruits Crate", "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.", 190U, "ONI_giftbox_u44_box_a")
	};

	private const string MYSTERYBOX_U44_DESC = "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.";

	private const string MYSTERYBOX_U45_DESC = "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.";

	private const string MYSTERYBOX_U46_DESC = "Unaddressed packages have been discovered near the Printing Pod. They bear Gravitas logos, and trace amounts of Neutronium have been detected.";

	private static HashSet<string> ClaimableRewardSet = new HashSet<string>(PermitItems.BoxInfos.Select<PermitItems.BoxInfo, string>((PermitItems.BoxInfo x) => x.ItemType));

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
		public BoxInfo(string type, string name, string desc, uint id, string icon)
		{
			this.ItemType = type;
			this.Name = name;
			this.Description = desc;
			this.TypeId = id;
			this.IconName = icon;
		}

		public string ItemType;

		public string Name;

		public string Description;

		public uint TypeId;

		public string IconName;
	}
}
