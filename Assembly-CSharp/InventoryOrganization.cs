using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public static class InventoryOrganization
{
	public static string GetPermitSubcategory(PermitResource permit)
	{
		foreach (KeyValuePair<string, List<string>> keyValuePair in InventoryOrganization.subcategoryIdToPermitIdsMap)
		{
			string text;
			List<string> list;
			keyValuePair.Deconstruct(out text, out list);
			string text2 = text;
			if (list.Contains(permit.Id))
			{
				return text2;
			}
		}
		return "UNCATEGORIZED";
	}

	public static string GetCategoryName(string categoryId)
	{
		return Strings.Get("STRINGS.UI.KLEI_INVENTORY_SCREEN.TOP_LEVEL_CATEGORIES." + categoryId.ToUpper());
	}

	public static string GetSubcategoryName(string subcategoryId)
	{
		return Strings.Get("STRINGS.UI.KLEI_INVENTORY_SCREEN.SUBCATEGORIES." + subcategoryId.ToUpper());
	}

	public static void Initialize()
	{
		if (InventoryOrganization.initialized)
		{
			return;
		}
		InventoryOrganization.initialized = true;
		InventoryOrganization.GenerateTopLevelCategories();
		InventoryOrganization.GenerateSubcategories();
		foreach (KeyValuePair<string, List<string>> keyValuePair in InventoryOrganization.categoryIdToSubcategoryIdsMap)
		{
			string text;
			List<string> list;
			keyValuePair.Deconstruct(out text, out list);
			string text2 = text;
			List<string> list2 = list;
			bool flag = true;
			foreach (string text3 in list2)
			{
				List<string> list3;
				if (InventoryOrganization.subcategoryIdToPermitIdsMap.TryGetValue(text3, out list3) && list3.Count != 0)
				{
					flag = false;
					break;
				}
			}
			InventoryOrganization.categoryIdToIsEmptyMap[text2] = flag;
		}
	}

	private static void AddTopLevelCategory(string categoryID, Sprite icon, string[] subcategoryIDs)
	{
		InventoryOrganization.categoryIdToSubcategoryIdsMap.Add(categoryID, new List<string>(subcategoryIDs));
		InventoryOrganization.categoryIdToIconMap.Add(categoryID, icon);
	}

	private static void AddSubcategory(string subcategoryID, Sprite icon, int sortkey, string[] permitIDs)
	{
		if (InventoryOrganization.subcategoryIdToPermitIdsMap.ContainsKey(subcategoryID))
		{
			return;
		}
		InventoryOrganization.subcategoryIdToPresentationDataMap.Add(subcategoryID, new InventoryOrganization.SubcategoryPresentationData(subcategoryID, icon, sortkey));
		InventoryOrganization.subcategoryIdToPermitIdsMap.Add(subcategoryID, new List<string>());
		for (int i = 0; i < permitIDs.Length; i++)
		{
			InventoryOrganization.subcategoryIdToPermitIdsMap[subcategoryID].Add(permitIDs[i]);
		}
	}

	private static void GenerateTopLevelCategories()
	{
		InventoryOrganization.AddTopLevelCategory("CLOTHING_TOPS", Assets.GetSprite("icon_inventory_tops"), new string[] { "CLOTHING_TOPS_BASIC", "CLOTHING_TOPS_TSHIRT", "CLOTHING_TOPS_FANCY", "CLOTHING_TOPS_JACKET", "CLOTHING_TOPS_UNDERSHIRT", "CLOTHING_TOPS_DRESS" });
		InventoryOrganization.AddTopLevelCategory("CLOTHING_BOTTOMS", Assets.GetSprite("icon_inventory_bottoms"), new string[] { "CLOTHING_BOTTOMS_BASIC", "CLOTHING_BOTTOMS_FANCY", "CLOTHING_BOTTOMS_SHORTS", "CLOTHING_BOTTOMS_SKIRTS", "CLOTHING_BOTTOMS_UNDERWEAR" });
		InventoryOrganization.AddTopLevelCategory("CLOTHING_GLOVES", Assets.GetSprite("icon_inventory_gloves"), new string[] { "CLOTHING_GLOVES_BASIC", "CLOTHING_GLOVES_PRINTS", "CLOTHING_GLOVES_SHORT", "CLOTHING_GLOVES_FORMAL" });
		InventoryOrganization.AddTopLevelCategory("CLOTHING_SHOES", Assets.GetSprite("icon_inventory_shoes"), new string[] { "CLOTHING_SHOES_BASIC", "CLOTHING_SHOES_FANCY", "CLOTHING_SHOE_SOCKS" });
		InventoryOrganization.AddTopLevelCategory("ATMOSUITS", Assets.GetSprite("icon_inventory_atmosuit_helmet"), new string[] { "ATMOSUIT_HELMETS_BASIC", "ATMOSUIT_HELMETS_FANCY", "ATMOSUIT_BODIES_BASIC", "ATMOSUIT_BODIES_FANCY", "ATMOSUIT_BELTS_BASIC", "ATMOSUIT_BELTS_FANCY", "ATMOSUIT_GLOVES_BASIC", "ATMOSUIT_GLOVES_FANCY", "ATMOSUIT_SHOES_BASIC", "ATMOSUIT_SHOES_FANCY" });
		InventoryOrganization.AddTopLevelCategory("JETSUITS", Assets.GetSprite("icon_inventory_jetsuit_helmet"), new string[] { "JETSUIT_HELMETS_BASIC", "JETSUIT_BODIES_BASIC", "JETSUIT_GLOVES_BASIC", "JETSUIT_SHOES_BASIC" });
		InventoryOrganization.AddTopLevelCategory("BUILDINGS", Assets.GetSprite("icon_inventory_buildings"), new string[]
		{
			"BUILDINGS_BED_COT", "BUILDINGS_BED_LUXURY", "BUILDINGS_FLOWER_VASE", "BUILDING_CEILING_LIGHT", "BUILDINGS_STORAGE", "BUILDINGS_INDUSTRIAL", "BUILDINGS_FOOD", "BUILDINGS_RANCHING", "BUILDINGS_WASHROOM", "BUILDINGS_RECREATION",
			"BUILDINGS_PRINTING_POD", "BUILDINGS_RESEARCH", "BUILDINGS_AUTOMATION", "BUILDINGS_ELECTIC_WIRES"
		});
		InventoryOrganization.AddTopLevelCategory("WALLPAPERS", Def.GetFacadeUISprite("ExteriorWall_tropical"), new string[] { "BUILDING_WALLPAPER_BASIC", "BUILDING_WALLPAPER_FANCY", "BUILDING_WALLPAPER_PRINTS" });
		InventoryOrganization.AddTopLevelCategory("ARTWORK", Assets.GetSprite("icon_inventory_artworks"), new string[] { "BUILDING_CANVAS_STANDARD", "BUILDING_CANVAS_PORTRAIT", "BUILDING_CANVAS_LANDSCAPE", "BUILDING_SCULPTURE", "MONUMENT_BOTTOM", "MONUMENT_MIDDLE", "MONUMENT_TOP" });
		InventoryOrganization.AddTopLevelCategory("JOY_RESPONSES", Assets.GetSprite("icon_inventory_joyresponses"), new string[] { "JOY_BALLOON" });
	}

	private static void GenerateSubcategories()
	{
		InventoryOrganization.AddSubcategory("BUILDING_CEILING_LIGHT", Def.GetUISprite("CeilingLight", "ui", false).first, 100, new string[]
		{
			"CeilingLight_mining", "CeilingLight_flower", "CeilingLight_polka_lamp_shade", "CeilingLight_burt_shower", "CeilingLight_ada_flask_round", "CeilingLight_rubiks", "FloorLamp_leg", "FloorLamp_bristle_blossom", "permit_floorlamp_cottage", "permit_glassceilinglight_jelly_pink",
			"permit_glassceilinglight_jelly_blue"
		});
		InventoryOrganization.AddSubcategory("BUILDINGS_BED_COT", Def.GetUISprite("Bed", "ui", false).first, 200, new string[] { "Bed_star_curtain", "Bed_canopy", "Bed_rowan_tropical", "Bed_ada_science_lab", "Bed_stringlights", "permit_bed_jorge", "permit_bed_cottage", "permit_bed_rock" });
		InventoryOrganization.AddSubcategory("BUILDINGS_BED_LUXURY", Def.GetUISprite("LuxuryBed", "ui", false).first, 300, new string[]
		{
			"LuxuryBed_boat", "LuxuryBed_bouncy", "LuxuryBed_grandprix", "LuxuryBed_rocket", "LuxuryBed_puft", "LuxuryBed_hand", "LuxuryBed_rubiks", "LuxuryBed_red_rose", "LuxuryBed_green_mush", "LuxuryBed_yellow_tartar",
			"LuxuryBed_purple_brainfat", "permit_elegantbed_hatch", "permit_elegantbed_pipsqueak", "permit_elegantbed_clam", "permit_elegantbed_submarine"
		});
		InventoryOrganization.AddSubcategory("BUILDINGS_FLOWER_VASE", Def.GetUISprite("FlowerVase", "ui", false).first, 400, new string[]
		{
			"FlowerVase_retro", "FlowerVase_retro_red", "FlowerVase_retro_white", "FlowerVase_retro_green", "FlowerVase_retro_blue", "FlowerVaseWall_retro_green", "FlowerVaseWall_retro_yellow", "FlowerVaseWall_retro_red", "FlowerVaseWall_retro_blue", "FlowerVaseWall_retro_white",
			"FlowerVaseHanging_retro_red", "FlowerVaseHanging_retro_green", "FlowerVaseHanging_retro_blue", "FlowerVaseHanging_retro_yellow", "FlowerVaseHanging_retro_white", "FlowerVaseHanging_beaker", "FlowerVaseHanging_rubiks", "PlanterBox_mealwood", "PlanterBox_bristleblossom", "PlanterBox_wheezewort",
			"PlanterBox_sleetwheat", "PlanterBox_salmon_pink"
		});
		InventoryOrganization.AddSubcategory("BUILDING_WALLPAPER_BASIC", Assets.GetSprite("icon_inventory_solid_wallpapers"), 500, new string[]
		{
			"ExteriorWall_basic_white", "ExteriorWall_basic_blue_cobalt", "ExteriorWall_basic_green_kelly", "ExteriorWall_basic_grey_charcoal", "ExteriorWall_basic_orange_satsuma", "ExteriorWall_basic_pink_flamingo", "ExteriorWall_basic_red_deep", "ExteriorWall_basic_yellow_lemon", "ExteriorWall_pastel_pink", "ExteriorWall_pastel_yellow",
			"ExteriorWall_pastel_green", "ExteriorWall_pastel_blue", "ExteriorWall_pastel_purple", "permit_walls_basic_blue_nautical"
		});
		InventoryOrganization.AddSubcategory("BUILDING_WALLPAPER_FANCY", Assets.GetSprite("icon_inventory_geometric_wallpapers"), 600, new string[]
		{
			"ExteriorWall_diagonal_red_deep_white", "ExteriorWall_diagonal_orange_satsuma_white", "ExteriorWall_diagonal_yellow_lemon_white", "ExteriorWall_diagonal_green_kelly_white", "ExteriorWall_diagonal_blue_cobalt_white", "ExteriorWall_diagonal_pink_flamingo_white", "ExteriorWall_diagonal_grey_charcoal_white", "ExteriorWall_circle_red_deep_white", "ExteriorWall_circle_orange_satsuma_white", "ExteriorWall_circle_yellow_lemon_white",
			"ExteriorWall_circle_green_kelly_white", "ExteriorWall_circle_blue_cobalt_white", "ExteriorWall_circle_pink_flamingo_white", "ExteriorWall_circle_grey_charcoal_white", "ExteriorWall_stripes_blue", "ExteriorWall_stripes_diagonal_blue", "ExteriorWall_stripes_circle_blue", "ExteriorWall_squares_red_deep_white", "ExteriorWall_squares_orange_satsuma_white", "ExteriorWall_squares_yellow_lemon_white",
			"ExteriorWall_squares_green_kelly_white", "ExteriorWall_squares_blue_cobalt_white", "ExteriorWall_squares_pink_flamingo_white", "ExteriorWall_squares_grey_charcoal_white", "ExteriorWall_plus_red_deep_white", "ExteriorWall_plus_orange_satsuma_white", "ExteriorWall_plus_yellow_lemon_white", "ExteriorWall_plus_green_kelly_white", "ExteriorWall_plus_blue_cobalt_white", "ExteriorWall_plus_pink_flamingo_white",
			"ExteriorWall_plus_grey_charcoal_white", "permit_walls_triangle_chilli_white", "permit_walls_triangle_satsuma_white", "permit_walls_triangle_lemon_white", "permit_walls_triangle_kelly_white", "permit_walls_triangle_cobalt_white", "permit_walls_triangle_flamingo_white", "permit_walls_triangle_charcoal_white", "permit_walls_triangle_white_chilli", "permit_walls_triangle_white_satsuma",
			"permit_walls_triangle_white_lemon", "permit_walls_triangle_white_kelly", "permit_walls_triangle_white_cobalt", "permit_walls_triangle_white_flamingo", "permit_walls_triangle_white_charcoal", "permit_walls_x_chilli_white", "permit_walls_x_satsuma_white", "permit_walls_x_lemon_white", "permit_walls_x_kelly_white", "permit_walls_x_cobalt_white",
			"permit_walls_x_flamingo_white", "permit_walls_x_charcoal_white", "ExteriorWall_stripes_rose", "ExteriorWall_stripes_diagonal_rose", "ExteriorWall_stripes_circle_rose", "ExteriorWall_stripes_mush", "ExteriorWall_stripes_diagonal_mush", "ExteriorWall_stripes_circle_mush", "ExteriorWall_stripes_yellow_tartar", "ExteriorWall_stripes_diagonal_yellow_tartar",
			"ExteriorWall_stripes_circle_yellow_tartar", "ExteriorWall_stripes_purple_brainfat", "ExteriorWall_stripes_diagonal_purple_brainfat", "ExteriorWall_stripes_circle_purple_brainfat", "permit_walls_diagonal_blue_nautical_white", "permit_walls_circle_blue_nautical_white", "permit_walls_circle_white_blue_nautical", "permit_walls_stripes_diagonal_white_nautical", "permit_walls_stripes_white_nautical", "permit_walls_stripes_circle_nautical_white",
			"permit_walls_stripes_circle_white_nautical", "permit_walls_squares_blue_nautical_white", "permit_walls_plus_blue_nautical_white", "permit_walls_square_blue_nautical_white", "permit_walls_triangle_white_nautical", "permit_walls_triangle_nautical_white", "permit_walls_x_nautical_white"
		});
		InventoryOrganization.AddSubcategory("BUILDING_WALLPAPER_PRINTS", Assets.GetSprite("icon_inventory_patterned_wallpapers"), 700, new string[]
		{
			"ExteriorWall_balm_lily", "ExteriorWall_clouds", "ExteriorWall_coffee", "ExteriorWall_mosaic", "ExteriorWall_mushbar", "ExteriorWall_plaid", "ExteriorWall_rain", "ExteriorWall_rainbow", "ExteriorWall_snow", "ExteriorWall_sun",
			"ExteriorWall_polka", "ExteriorWall_blueberries", "ExteriorWall_grapes", "ExteriorWall_lemon", "ExteriorWall_lime", "ExteriorWall_satsuma", "ExteriorWall_strawberry", "ExteriorWall_watermelon", "ExteriorWall_toiletpaper", "ExteriorWall_plunger",
			"ExteriorWall_tropical", "ExteriorWall_kitchen_retro1", "ExteriorWall_floppy_azulene_vitro", "ExteriorWall_floppy_black_white", "ExteriorWall_floppy_peagreen_balmy", "ExteriorWall_floppy_satsuma_yellowcake", "ExteriorWall_floppy_magma_amino", "ExteriorWall_orange_juice", "ExteriorWall_paint_blots", "ExteriorWall_telescope",
			"ExteriorWall_tictactoe_o", "ExteriorWall_tictactoe_x", "ExteriorWall_dice_1", "ExteriorWall_dice_2", "ExteriorWall_dice_3", "ExteriorWall_dice_4", "ExteriorWall_dice_5", "ExteriorWall_dice_6", "permit_walls_wood_panel", "permit_walls_igloo",
			"permit_walls_forest", "permit_walls_southwest", "permit_walls_circuit_lightcobalt", "permit_walls_circuit_bogey", "permit_walls_circuit_punk", "permit_walls_arcade", "permit_walls_chameleo", "permit_walls_paculacanth", "permit_walls_raptor", "permit_walls_stego",
			"permit_walls_fossil_chameleo", "permit_walls_fossil_paculacanth", "permit_walls_fossil_raptor", "permit_walls_fossil_stego", "permit_walls_silhouette_prehistoriccritters", "permit_walls_stripes_neutronium", "permit_walls_waves", "permit_walls_waves_half", "permit_walls_waves01", "permit_walls_waves02",
			"permit_walls_wavescrests", "permit_walls_wavescrests_half", "permit_walls_wavescrashing", "permit_walls_sea", "permit_walls_sea_anchor", "permit_walls_sea_wheel", "permit_walls_sea_lifepreserver", "permit_walls_sea_compass", "permit_walls_wavesswirls", "permit_walls_aqua_blank",
			"permit_walls_aqua_bubbles", "permit_walls_aqua_jellyfish", "permit_walls_aqua_silt_top", "permit_walls_aqua_silt", "permit_walls_aqua_kelp_top", "permit_walls_aqua_kelp", "permit_walls_aqua_kelp_silt", "permit_walls_aqua_beaker", "permit_walls_aqua_pacu", "permit_walls_aqua_puffer",
			"permit_walls_aqua_turtle"
		});
		InventoryOrganization.AddSubcategory("BUILDINGS_RECREATION", Def.GetUISprite("WaterCooler", "ui", false).first, 700, new string[]
		{
			"ItemPedestal_hand", "permit_pedestal_screw_chrome", "permit_pedestal_screw_brass", "permit_pedestal_arcade", "permit_pedestal_battery", "permit_pedestal_cottage", "MassageTable_shiatsu", "MassageTable_balloon", "permit_masseur_prehistoric", "WaterCooler_round_body",
			"WaterCooler_balloon", "WaterCooler_yellow_tartar", "WaterCooler_red_rose", "WaterCooler_green_mush", "WaterCooler_purple_brainfat", "WaterCooler_blue_babytears", "CornerMoulding_shineornaments", "CrownMoulding_shineornaments", "permit_corner_tile_retro_nautical", "permit_crown_moulding_retro_nautical",
			"permit_crown_moulding_neutronium", "permit_corner_tile_neutronium", "permit_pedestal_neutronium", "permit_beach_chair_beakon", "permit_beach_chair_pokeshell", "permit_beach_chair_kraken", "permit_mechanical_surfboard_jaws"
		});
		InventoryOrganization.AddSubcategory("BUILDINGS_PRINTING_POD", Def.GetUISprite("Headquarters", "ui", false).first, 750, new string[] { "permit_headquarters_ceres", "permit_porta_pod_y_ice", "permit_hqbase_cyberpunk", "permit_porta_pod_y_cyberpunk", "permit_hqbase_dino", "permit_porta_pod_y_dino", "permit_hqbase_neutronium", "permit_porta_pod_y_neutronium", "permit_hqbase_aquatic", "permit_porta_pod_y_aquatic" });
		InventoryOrganization.AddSubcategory("BUILDINGS_STORAGE", Def.GetUISprite("StorageLocker", "ui", false).first, 800, new string[]
		{
			"StorageLocker_green_mush", "StorageLocker_red_rose", "StorageLocker_blue_babytears", "StorageLocker_purple_brainfat", "StorageLocker_yellow_tartar", "StorageLocker_polka_darknavynookgreen", "StorageLocker_polka_darkpurpleresin", "StorageLocker_stripes_red_white", "permit_storagelocker_neutronium", "permit_storagelocker_stripes_nautical",
			"permit_storagelocker_waterbubbles", "Refrigerator_stripes_red_white", "Refrigerator_blue_babytears", "Refrigerator_green_mush", "Refrigerator_red_rose", "Refrigerator_yellow_tartar", "Refrigerator_purple_brainfat", "permit_fridge_neutronium", "permit_fridge_stripes_nautical", "permit_fridge_waterbubbles",
			"GasReservoir_lightgold", "GasReservoir_peagreen", "GasReservoir_lightcobalt", "GasReservoir_polka_darkpurpleresin", "GasReservoir_polka_darknavynookgreen", "GasReservoir_blue_babytears", "GasReservoir_yellow_tartar", "GasReservoir_green_mush", "GasReservoir_red_rose", "GasReservoir_purple_brainfat",
			"permit_gasstorage_dartle", "permit_gasstorage_lumb", "permit_smartstoragelocker_gravitas", "permit_gasstorage_stripes_nautical", "permit_gasstorage_waterbubbles"
		});
		InventoryOrganization.AddSubcategory("BUILDINGS_INDUSTRIAL", Def.GetUISprite("RockCrusher", "ui", false).first, 800, new string[]
		{
			"RockCrusher_hands", "RockCrusher_teeth", "RockCrusher_roundstamp", "RockCrusher_spikebeds", "RockCrusher_chomp", "RockCrusher_gears", "RockCrusher_balloon", "permit_craftingstation_cyberpunk", "permit_milkpress_stego", "permit_mineraldeoxidizer_neutronium",
			"permit_waterpump_neutronium", "permit_co2scrubber_neutronium", "permit_electrolyzer_neutronium", "permit_respeccer_neutronium", "permit_waterpurifier_neutronium", "permit_metalrefinery_neutronium", "permit_boiler_neutronium", "permit_liquidconditioner_neutronium"
		});
		InventoryOrganization.AddSubcategory("BUILDINGS_FOOD", Def.GetUISprite("EggCracker", "ui", false).first, 800, new string[] { "EggCracker_beaker", "EggCracker_flower", "EggCracker_hands", "MicrobeMusher_purple_brainfat", "MicrobeMusher_yellow_tartar", "MicrobeMusher_red_rose", "MicrobeMusher_green_mush", "MicrobeMusher_blue_babytears", "permit_cookingstation_cottage", "permit_cookingstation_gourmet_cottage" });
		InventoryOrganization.AddSubcategory("BUILDINGS_RANCHING", Def.GetUISprite("RanchStation", "ui", false).first, 800, new string[] { "permit_rancherstation_cottage", "permit_rancherstation_dino", "permit_rancherstation_neutronium", "permit_underwater_critter_condo_aquatic" });
		InventoryOrganization.AddSubcategory("BUILDINGS_AUTOMATION", Def.GetUISprite("LogicWire", "ui", false).first, 800, new string[]
		{
			"permit_logic_demultiplexer_lightcobalt", "permit_logic_demultiplexer_flamingo", "permit_logic_demultiplexer_lemon", "permit_logic_multiplexer_lightcobalt", "permit_logic_multiplexer_flamingo", "permit_logic_multiplexer_lemon", "permit_logic_filter_lightcobalt", "permit_logic_filter_flamingo", "permit_logic_filter_lemon", "permit_logic_buffer_lightcobalt",
			"permit_logic_buffer_flamingo", "permit_logic_buffer_lemon", "permit_logic_not_lightcobalt", "permit_logic_not_flamingo", "permit_logic_not_lemon", "permit_logic_counter_lightcobalt", "permit_logic_counter_flamingo", "permit_logic_counter_lemon", "permit_logic_or_lightcobalt", "permit_logic_or_flamingo",
			"permit_logic_or_lemon", "permit_logic_and_lightcobalt", "permit_logic_and_flamingo", "permit_logic_and_lemon", "permit_logic_xor_lightcobalt", "permit_logic_xor_flamingo", "permit_logic_xor_lemon", "permit_logic_memory_lightcobalt", "permit_logic_memory_flamingo", "permit_logic_memory_lemon",
			"permit_logic_wires_flamingo", "permit_logic_wires_lemon", "permit_logic_wires_bogey", "permit_logic_ribbon_flamingo", "permit_logic_ribbon_lemon", "permit_logic_ribbon_bogey", "permit_logic_bridge_flamingo", "permit_logic_bridge_lemon", "permit_logic_bridge_bogey", "permit_logic_ribbon_bridge_flamingo",
			"permit_logic_ribbon_bridge_lemon", "permit_logic_ribbon_bridge_bogey"
		});
		InventoryOrganization.AddSubcategory("BUILDINGS_ELECTIC_WIRES", Def.GetUISprite("Wire", "ui", false).first, 800, new string[]
		{
			"permit_utilities_electric_conduct_net_pink", "permit_utilities_electric_conduct_diamond_orchid", "permit_utilities_electric_conduct_scale_lime", "permit_utilityelectricbridgeconductive_scale_lime", "permit_utilityelectricbridgeconductive_net_pink", "permit_utilityelectricbridgeconductive_diamond_orchid", "permit_generatormanual_rock", "permit_generatormanual_a", "permit_generatormanual_b", "permit_generatormanual_cyberpunk",
			"permit_generatormanual_neutronium", "permit_generatorphos_neutronium", "permit_steamturbine2_neutronium", "permit_generatormanual_nautical", "permit_generatormanual_waterbubbles"
		});
		InventoryOrganization.AddSubcategory("BUILDINGS_RESEARCH", Def.GetUISprite("ResearchCenter", "ui", false).first, 800, new string[] { "permit_research_center_cyberpunk", "permit_research_center2_cyberpunk" });
		InventoryOrganization.AddSubcategory("BUILDINGS_WASHROOM", Def.GetUISprite("FlushToilet", "ui", false).first, 800, new string[]
		{
			"FlushToilet_polka_darkpurpleresin", "FlushToilet_polka_darknavynookgreen", "FlushToilet_purple_brainfat", "FlushToilet_yellow_tartar", "FlushToilet_red_rose", "FlushToilet_green_mush", "FlushToilet_blue_babytears", "WashSink_purple_brainfat", "WashSink_blue_babytears", "WashSink_green_mush",
			"WashSink_yellow_tartar", "WashSink_red_rose"
		});
		InventoryOrganization.AddSubcategory("JOY_BALLOON", Db.Get().Permits.BalloonArtistFacades[0].GetPermitPresentationInfo().sprite, 100, new string[]
		{
			"BalloonRedFireEngineLongSparkles", "BalloonYellowLongSparkles", "BalloonBlueLongSparkles", "BalloonGreenLongSparkles", "BalloonPinkLongSparkles", "BalloonPurpleLongSparkles", "BalloonBabyPacuEgg", "BalloonBabyGlossyDreckoEgg", "BalloonBabyHatchEgg", "BalloonBabyPokeshellEgg",
			"BalloonBabyPuftEgg", "BalloonBabyShovoleEgg", "BalloonBabyPipEgg", "BalloonCandyBlueberry", "BalloonCandyGrape", "BalloonCandyLemon", "BalloonCandyLime", "BalloonCandyOrange", "BalloonCandyStrawberry", "BalloonCandyWatermelon",
			"BalloonHandGold", "permit_balloon_babystego_egg", "permit_balloon_babyrhex_egg", "permit_balloon_blowfish_egg"
		});
		InventoryOrganization.AddSubcategory("JOY_STICKER", Db.Get().Permits.StickerBombs[0].GetPermitPresentationInfo().sprite, 200, new string[]
		{
			"a", "b", "c", "d", "e", "f", "g", "h", "rocket", "paperplane",
			"plant", "plantpot", "mushroom", "mermaid", "spacepet", "spacepet2", "spacepet3", "spacepet4", "spacepet5", "unicorn"
		});
		InventoryOrganization.AddSubcategory("PRIMO_GARB", null, 200, new string[]
		{
			"clubshirt", "cummerbund", "decor_02", "decor_03", "decor_04", "decor_05", "gaudysweater", "limone", "mondrian", "overalls",
			"triangles", "workout"
		});
		InventoryOrganization.AddSubcategory("BUILDING_CANVAS_STANDARD", Def.GetUISprite("Canvas", "ui", false).first, 100, new string[]
		{
			"Canvas_Bad", "Canvas_Average", "Canvas_Good", "Canvas_Good2", "Canvas_Good3", "Canvas_Good4", "Canvas_Good5", "Canvas_Good6", "Canvas_Good7", "Canvas_Good8",
			"Canvas_Good9", "Canvas_Good10", "Canvas_Good11", "Canvas_Good13", "Canvas_Good12", "Canvas_Good14", "Canvas_Good15", "Canvas_Good16", "permit_painting_art_ceres_a", "permit_painting_art_stego",
			"permit_painting_art_aquatic"
		});
		InventoryOrganization.AddSubcategory("BUILDING_CANVAS_PORTRAIT", Def.GetUISprite("CanvasTall", "ui", false).first, 200, new string[]
		{
			"CanvasTall_Bad", "CanvasTall_Average", "CanvasTall_Good", "CanvasTall_Good2", "CanvasTall_Good3", "CanvasTall_Good4", "CanvasTall_Good5", "CanvasTall_Good6", "CanvasTall_Good7", "CanvasTall_Good8",
			"CanvasTall_Good9", "CanvasTall_Good11", "CanvasTall_Good10", "CanvasTall_Good14", "permit_painting_tall_art_ceres_a", "permit_painting_tall_art_aquatic"
		});
		InventoryOrganization.AddSubcategory("BUILDING_CANVAS_LANDSCAPE", Def.GetUISprite("CanvasWide", "ui", false).first, 300, new string[]
		{
			"CanvasWide_Bad", "CanvasWide_Average", "CanvasWide_Good", "CanvasWide_Good2", "CanvasWide_Good3", "CanvasWide_Good4", "CanvasWide_Good5", "CanvasWide_Good6", "CanvasWide_Good7", "CanvasWide_Good8",
			"CanvasWide_Good9", "CanvasWide_Good10", "CanvasWide_Good11", "CanvasWide_Good13", "permit_painting_wide_art_ceres_a", "permit_painting_wide_art_rhex", "permit_painting_wide_art_venus"
		});
		InventoryOrganization.AddSubcategory("BUILDING_SCULPTURE", Def.GetUISprite("Sculpture", "ui", false).first, 400, new string[]
		{
			"Sculpture_Bad", "Sculpture_Average", "Sculpture_Good1", "Sculpture_Good2", "Sculpture_Good3", "Sculpture_Good5", "Sculpture_Good6", "SmallSculpture_Bad", "SmallSculpture_Average", "SmallSculpture_Good",
			"SmallSculpture_Good2", "SmallSculpture_Good3", "SmallSculpture_Good5", "SmallSculpture_Good6", "IceSculpture_Bad", "IceSculpture_Average", "MarbleSculpture_Bad", "MarbleSculpture_Average", "MarbleSculpture_Good1", "MarbleSculpture_Good2",
			"MarbleSculpture_Good3", "MetalSculpture_Bad", "MetalSculpture_Average", "MetalSculpture_Good1", "MetalSculpture_Good2", "MetalSculpture_Good3", "MetalSculpture_Good5", "Sculpture_Good4", "SmallSculpture_Good4", "MetalSculpture_Good4",
			"MarbleSculpture_Good4", "MarbleSculpture_Good5", "IceSculpture_Average2", "IceSculpture_Average3", "permit_sculpture_wood_amazing_action_puft", "permit_sculpture_wood_amazing_rear_puft", "permit_sculpture_wood_amazing_rear_shovevole", "permit_sculpture_wood_amazing_action_gulp", "permit_sculpture_wood_amazing_rear_cuddlepip", "permit_sculpture_wood_amazing_rear_drecko",
			"permit_sculpture_wood_okay_mid_one", "permit_sculpture_wood_amazing_action_pacu", "permit_sculpture_wood_crap_low_one", "permit_sculpture_wood_amazing_action_wood_deer", "permit_icesculpture_amazing_idle_seal", "permit_icesculpture_amazing_idle_bammoth", "permit_icesculpture_amazing_idle_wood_deer", "permit_fossilsculpture_idle_stego", "permit_fossilsculpture_idle_rhex", "permit_fossilsculpture_idle_jawbo",
			"permit_fossilsculpture_idle_shellonoidis", "permit_sculpture_marble_swirling_pacus", "permit_sculpture_marble_sequine_squad", "permit_sculpture_marble_atlas_haul"
		});
		InventoryOrganization.AddSubcategory("MONUMENT_BOTTOM", Def.GetUISprite("MonumentBottom", "ui", false).first, 700, new string[]
		{
			"bottom_option_a", "bottom_option_b", "bottom_option_c", "bottom_option_d", "bottom_option_e", "bottom_option_f", "bottom_option_g", "bottom_option_h", "bottom_option_i", "bottom_option_j",
			"bottom_option_k", "bottom_option_l", "bottom_option_m", "bottom_option_n", "bottom_option_o", "bottom_option_p", "bottom_option_q", "bottom_option_r", "bottom_option_s", "bottom_option_t",
			"permit_monument_base_a_frosty", "permit_monument_base_b_frosty", "permit_monument_base_c_frosty", "permit_monument_base_a_bionic", "permit_monument_base_b_bionic", "permit_monument_base_c_bionic", "permit_monument_base_a_aquatic", "permit_monument_base_b_aquatic"
		});
		InventoryOrganization.AddSubcategory("MONUMENT_MIDDLE", Def.GetUISprite("MonumentMiddle", "ui", false).first, 600, new string[]
		{
			"mid_option_a", "mid_option_b", "mid_option_c", "mid_option_d", "mid_option_e", "mid_option_f", "mid_option_g", "mid_option_h", "mid_option_i", "mid_option_j",
			"mid_option_k", "mid_option_l", "mid_option_m", "mid_option_n", "mid_option_o", "permit_monument_mid_a_frosty", "permit_monument_mid_b_frosty", "permit_monument_mid_c_frosty", "permit_monument_mid_a_bionic", "permit_monument_mid_b_bionic",
			"permit_monument_mid_c_bionic", "permit_monument_mid_a_aquatic", "permit_monument_mid_b_aquatic"
		});
		InventoryOrganization.AddSubcategory("MONUMENT_TOP", Def.GetUISprite("MonumentTop", "ui", false).first, 500, new string[]
		{
			"top_option_a", "top_option_b", "top_option_c", "top_option_d", "top_option_e", "top_option_f", "top_option_g", "top_option_h", "top_option_i", "top_option_j",
			"top_option_k", "top_option_l", "top_option_m", "top_option_n", "top_option_o", "top_option_p", "top_option_q", "top_option_r", "top_option_s", "top_option_t",
			"top_option_u", "top_option_v", "top_option_w", "top_option_x", "top_option_y", "top_option_z", "permit_monument_upper_a_frosty", "permit_monument_upper_b_frosty", "permit_monument_upper_c_frosty", "permit_monument_upper_a_bionic",
			"permit_monument_upper_b_bionic", "permit_monument_upper_c_bionic", "permit_monument_upper_a_prehistoric", "permit_monument_upper_b_prehistoric", "permit_monument_upper_a_aquatic", "permit_monument_upper_b_aquatic"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_TOPS_BASIC", Assets.GetSprite("icon_inventory_basic_shirts"), 100, new string[]
		{
			"TopStandardYellow", "TopStandardRed", "TopStandardBlue", "TopStandardGreen", "permit_standard_bionic_top", "permit_top_regal_neutronium", "permit_top_swim_red", "permit_top_swim_red", "permit_top_swim_green", "permit_top_swim_blue",
			"permit_top_swim_yellow", "permit_top_swim_axon", "TopBasicBlack", "TopBasicWhite", "TopBasicRed", "TopBasicOrange", "TopBasicYellow", "TopBasicGreen", "TopBasicAqua", "TopBasicPurple",
			"TopBasicPinkOrchid"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_TOPS_TSHIRT", Assets.GetSprite("icon_inventory_tees"), 300, new string[] { "TopRaglanDeepRed", "TopRaglanCobalt", "TopRaglanFlamingo", "TopRaglanKellyGreen", "TopRaglanCharcoal", "TopRaglanLemon", "TopRaglanSatsuma", "TopTShirtWhite", "TopTShirtMagenta" });
		InventoryOrganization.AddSubcategory("CLOTHING_TOPS_UNDERSHIRT", Assets.GetSprite("icon_inventory_undershirts"), 400, new string[]
		{
			"TopUndershirtExecutive", "TopUndershirtUnderling", "TopUndershirtGroupthink", "TopUndershirtStakeholder", "TopUndershirtAdmin", "TopUndershirtBuzzword", "TopUndershirtSynergy", "TopGinchPinkSaltrock", "TopGinchPurpleDusky", "TopGinchBlueBasin",
			"TopGinchTealBalmy", "TopGinchGreenLime", "TopGinchYellowYellowcake", "TopGinchOrangeAtomic", "TopGinchRedMagma", "TopGinchGreyGrey", "TopGinchGreyCharcoal", "permit_top_gotch_ennui_flint", "permit_top_gotch_gluon_petal", "permit_top_gotch_rivet_electro",
			"permit_top_gotch_basin_wortseed", "permit_top_gotch_vitro_symbio", "permit_top_gotch_nuked_quantum", "permit_top_gotch_mush_peagreen", "permit_top_gotch_flatus_muck", "permit_top_gotch_canker_satsuma", "permit_top_gotch_puce_rusty", "permit_top_gotch_dusky_grape", "permit_top_gotch_ash_charcoal"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_TOPS_FANCY", Assets.GetSprite("icon_inventory_specialty_tops"), 500, new string[]
		{
			"TopResearcher", "TopX1Pinchapeppernutbells", "TopXSporchid", "TopPompomShinebugsPinkPeppernut", "TopSnowflakeBlue", "TopWaistcoatPinstripeSlate", "TopWater", "TopFloralPink", "permit_top_flannel_red", "permit_top_flannel_orange",
			"permit_top_flannel_yellow", "permit_top_flannel_green", "permit_top_flannel_blue_middle", "permit_top_flannel_purple", "permit_top_flannel_pink_orchid", "permit_top_flannel_white", "permit_top_flannel_black", "permit_top_jersey_01", "permit_top_jersey_02", "permit_top_jersey_03",
			"permit_top_jersey_04", "permit_top_jersey_05", "permit_top_jersey_07", "permit_top_jersey_08", "permit_top_jersey_09", "permit_top_jersey_10", "permit_top_jersey_11", "permit_top_jersey_12", "permit_top_vest_puffer_orange", "permit_top_spacetop_white",
			"permit_top_metal_grey", "permit_top_scout_white", "permit_top_sweater_ribbed_rust", "permit_top_sweater_wader_ltblue", "permit_top_nautical_blue_white"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_TOPS_JACKET", Assets.GetSprite("icon_inventory_jackets"), 500, new string[]
		{
			"TopJellypuffJacketBlueberry", "TopJellypuffJacketGrape", "TopJellypuffJacketLemon", "TopJellypuffJacketLime", "TopJellypuffJacketSatsuma", "TopJellypuffJacketStrawberry", "TopJellypuffJacketWatermelon", "TopAthlete", "TopCircuitGreen", "TopDenimBlue",
			"TopRebelGi", "TopJacketSmokingBurgundy", "TopMechanic", "TopVelourBlack", "TopVelourBlue", "TopVelourPink", "TopTweedPinkOrchid", "TopBuilder", "TopKnitPolkadotTurq", "TopFlashy",
			"permit_top_snapjacket_brine"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_TOPS_DRESS", Assets.GetSprite("icon_inventory_dress_fancy"), 500, new string[]
		{
			"DressSleevelessBowBw", "BodysuitBallerinaPink", "permit_dress_futurespace_blue", "PjCloversGlitchKelly", "PjHeartsChilliStrawberry", "permit_jumpsuit_vsuit_stellar", "permit_pj_biocircuit_wildberry", "permit_jumpsuit_romper_tan_frass", "permit_pj_biocircuit_wildberry", "permit_pj_dino",
			"permit_pj_dino2", "permit_pj_dino3", "permit_swimsuit_merm", "permit_swimsuit_wavecrests", "permit_dress_sailor_white_nautical"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_BOTTOMS_BASIC", Assets.GetSprite("icon_inventory_basic_pants"), 100, new string[]
		{
			"BottomBasicBlack", "permit_standard_bionic_bottom", "permit_bottom_regal_neutronium", "permit_pants_swim_black", "BottomBasicWhite", "BottomBasicRed", "BottomBasicOrange", "BottomBasicYellow", "BottomBasicGreen", "BottomBasicAqua",
			"BottomBasicPurple", "BottomBasicPinkOrchid", "PantsBasicRedOrange", "PantsBasicLightBrown", "PantsBasicOrangeSatsuma"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_BOTTOMS_FANCY", Assets.GetSprite("icon_inventory_fancy_pants"), 200, new string[]
		{
			"PantsAthlete", "PantsCircuitGreen", "PantsJeans", "PantsRebelGi", "PantsResearch", "PantsPinstripeSlate", "PantsVelourBlack", "PantsVelourBlue", "PantsVelourPink", "PantsKnitPolkadotTurq",
			"PantsGiBeltWhiteBlack", "PantsBeltKhakiTan", "permit_pants_extendedwaist_blue_wheezewort", "permit_pants_snapjacket_brine", "permit_pants_suspenders_frass", "permit_pants_wader_algae", "permit_pants_swim_black", "permit_pants_nautical_white"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_BOTTOMS_SHORTS", Assets.GetSprite("icon_inventory_shorts"), 300, new string[] { "ShortsBasicDeepRed", "ShortsBasicSatsuma", "ShortsBasicYellowcake", "ShortsBasicKellyGreen", "ShortsBasicBlueCobalt", "ShortsBasicPinkFlamingo", "ShortsBasicCharcoal", "permit_shorts_scout_brown" });
		InventoryOrganization.AddSubcategory("CLOTHING_BOTTOMS_SKIRTS", Assets.GetSprite("icon_inventory_skirts"), 300, new string[]
		{
			"SkirtBasicBlueMiddle", "SkirtBasicPurple", "SkirtBasicGreen", "SkirtBasicOrange", "SkirtBasicPinkOrchid", "SkirtBasicRed", "SkirtBasicYellow", "SkirtBasicPolkadot", "SkirtBasicWatermelon", "SkirtDenimBlue",
			"SkirtLeopardPrintBluePink", "SkirtSparkleBlue", "SkirtBallerinaPink", "SkirtTweedPinkOrchid"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_BOTTOMS_UNDERWEAR", Assets.GetSprite("icon_inventory_underwear"), 300, new string[]
		{
			"BottomBriefsExecutive", "BottomBriefsUnderling", "BottomBriefsGroupthink", "BottomBriefsStakeholder", "BottomBriefsAdmin", "BottomBriefsBuzzword", "BottomBriefsSynergy", "BottomGinchPinkGluon", "BottomGinchPurpleCortex", "BottomGinchBlueFrosty",
			"BottomGinchTealLocus", "BottomGinchGreenGoop", "BottomGinchYellowBile", "BottomGinchOrangeNybble", "BottomGinchRedIronbow", "BottomGinchGreyPhlegm", "BottomGinchGreyObelus", "permit_bottom_gotch_flint_ash", "permit_bottom_gotch_petal_neonpink", "permit_bottom_gotch_electro_sweetle",
			"permit_bottom_gotch_wortseed_chromo", "permit_bottom_gotch_symbio_oakmolt", "permit_bottom_gotch_quantum_radglo", "permit_bottom_gotch_peagreen_glitch", "permit_bottom_gotch_muck_earwax", "permit_bottom_gotch_satsuma_resin", "permit_bottom_gotch_rusty_grubby", "permit_bottom_gotch_grape_alkali", "permit_bottom_gotch_charcoal_obelus"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_GLOVES_BASIC", Assets.GetSprite("icon_inventory_basic_gloves"), 100, new string[]
		{
			"GlovesStandardGold", "permit_standard_bionic_gloves", "GlovesBasicBlack", "GlovesBasicWhite", "GlovesBasicRed", "GlovesBasicOrange", "GlovesBasicYellow", "GlovesBasicGreen", "GlovesBasicAqua", "GlovesBasicPurple",
			"GlovesBasicPinkOrchid", "GlovesBasicSlate", "GlovesBasicBlueGrey", "GlovesBasicBrownKhaki", "GlovesBasicGrey", "GlovesBasicPinksalmon", "GlovesBasicTan", "permit_gloves_basic_blue_wheezewort", "permit_gloves_basic_brown", "permit_gloves_basic_grime",
			"permit_gloves_basic_blue_nautical", "permit_gloves_basic_navy"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_GLOVES_FORMAL", Assets.GetSprite("icon_inventory_fancy_gloves"), 200, new string[]
		{
			"GlovesFormalWhite", "GlovesLongWhite", "Gloves2ToneCreamCharcoal", "GlovesSparkleWhite", "GlovesGinchPinkSaltrock", "GlovesGinchPurpleDusky", "GlovesGinchBlueBasin", "GlovesGinchTealBalmy", "GlovesGinchGreenLime", "GlovesGinchYellowYellowcake",
			"GlovesGinchOrangeAtomic", "GlovesGinchRedMagma", "GlovesGinchGreyGrey", "GlovesGinchGreyCharcoal", "permit_gloves_hockey_01", "permit_gloves_hockey_02", "permit_gloves_hockey_03", "permit_gloves_hockey_04", "permit_gloves_hockey_05", "permit_gloves_hockey_07",
			"permit_gloves_hockey_08", "permit_gloves_hockey_09", "permit_gloves_hockey_10", "permit_gloves_hockey_11", "permit_gloves_hockey_12", "permit_mittens_knit_black_smog", "permit_mittens_knit_white", "permit_mittens_knit_yellowcake", "permit_mittens_knit_orange_tectonic", "permit_mittens_knit_green_enzyme",
			"permit_mittens_knit_blue_azulene", "permit_mittens_knit_purple_astral", "permit_mittens_knit_pink_cosmic", "permit_gloves_gotch_flint_ash", "permit_gloves_gotch_petal_neonpink", "permit_gloves_gotch_electro_sweetle", "permit_gloves_gotch_wortseed_chromo", "permit_gloves_gotch_symbio_oakmolt", "permit_gloves_gotch_quantum_radglo", "permit_gloves_gotch_peagreen_glitch",
			"permit_gloves_gotch_muck_earwax", "permit_gloves_gotch_satsuma_resin", "permit_gloves_gotch_rusty_grubby", "permit_gloves_gotch_grape_alkali", "permit_gloves_gotch_charcoal_obelus", "permit_gloves_swim_wheezy", "permit_gloves_swim_gold", "permit_gloves_swim_merm", "permit_gloves_swim_wavecrests"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_GLOVES_SHORT", Assets.GetSprite("icon_inventory_short_gloves"), 300, new string[] { "GlovesCufflessBlueberry", "GlovesCufflessGrape", "GlovesCufflessLemon", "GlovesCufflessLime", "GlovesCufflessSatsuma", "GlovesCufflessStrawberry", "GlovesCufflessWatermelon", "GlovesCufflessBlack", "permit_gloves_cuffless_shiny_algae" });
		InventoryOrganization.AddSubcategory("CLOTHING_GLOVES_PRINTS", Assets.GetSprite("icon_inventory_specialty_gloves"), 400, new string[]
		{
			"GlovesAthlete", "GlovesCircuitGreen", "GlovesAthleticRedDeep", "GlovesAthleticOrangeSatsuma", "GlovesAthleticYellowLemon", "GlovesAthleticGreenKelly", "GlovesAthleticBlueCobalt", "GlovesAthleticPinkFlamingo", "GlovesAthleticGreyCharcoal", "GlovesDenimBlue",
			"GlovesBallerinaPink", "GlovesKnitGold", "GlovesKnitMagenta", "permit_gloves_futurespace_blue", "permit_gloves_vsuit_stellar", "permit_gloves_snapjacket_brine", "permit_gloves_puffer_orange", "permit_gloves_metal_grey", "permit_gloves_neutronium", "permit_gloves_swim_gold"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_SHOES_BASIC", Assets.GetSprite("icon_inventory_basic_shoes"), 100, new string[]
		{
			"ShoesBasicBlack", "permit_standard_bionic_shoes", "permit_shoes_swim_red", "ShoesBasicWhite", "ShoesBasicRed", "ShoesBasicOrange", "ShoesBasicYellow", "ShoesBasicGreen", "ShoesBasicAqua", "ShoesBasicPurple",
			"ShoesBasicPinkOrchid", "ShoesBasicBlueGrey", "ShoesBasicTan", "ShoesBasicGray", "ShoesDenimBlue", "permit_shoes_basic_blue_wheezy", "permit_shoes_basic_frass", "permit_shoes_basic_blue_nautical", "permit_shoes_basic_blue_navy"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_SHOES_FANCY", Assets.GetSprite("icon_inventory_fancy_shoes"), 200, new string[]
		{
			"ShoesBallerinaPink", "ShoesMaryjaneSocksBw", "ShoesClassicFlatsCreamCharcoal", "ShoesVelourBlue", "ShoesVelourPink", "ShoesVelourBlack", "ShoesFlashy", "permit_shoes_futurespace_blue", "permit_shoes_vsuit_stellar", "permit_shoes_romper_frass_tan",
			"permit_shoes_scout_brown", "permit_shoes_regal_neutronium", "permit_shoes_swim_red", "permit_shoes_swim_green", "permit_shoes_swim_yellow", "permit_shoes_swim_blue", "permit_shoes_swim_wheezy", "permit_shoes_boots_sailor_nautical", "permit_shoes_swim_merm", "permit_shoes_swim_wavecrests"
		});
		InventoryOrganization.AddSubcategory("CLOTHING_SHOE_SOCKS", Assets.GetSprite("icon_inventory_socks"), 500, new string[]
		{
			"SocksAthleticDeepRed", "SocksAthleticOrangeSatsuma", "SocksAthleticYellowLemon", "SocksAthleticGreenKelly", "SocksAthleticBlueCobalt", "SocksAthleticPinkFlamingo", "SocksAthleticGreyCharcoal", "SocksLegwarmersBlueberry", "SocksLegwarmersGrape", "SocksLegwarmersLemon",
			"SocksLegwarmersLime", "SocksLegwarmersSatsuma", "SocksLegwarmersStrawberry", "SocksLegwarmersWatermelon", "SocksGinchPinkSaltrock", "SocksGinchPurpleDusky", "SocksGinchBlueBasin", "SocksGinchTealBalmy", "SocksGinchGreenLime", "SocksGinchYellowYellowcake",
			"SocksGinchOrangeAtomic", "SocksGinchRedMagma", "SocksGinchGreyGrey", "SocksGinchGreyCharcoal", "permit_socks_gotch_flint_ash", "permit_socks_gotch_petal_neonpink", "permit_socks_gotch_electro_sweetle", "permit_socks_gotch_wortseed_chromo", "permit_socks_gotch_symbio_oakmolt", "permit_socks_gotch_quantum_radglo",
			"permit_socks_gotch_peagreen_glitch", "permit_socks_gotch_muck_earwax", "permit_socks_gotch_satsuma_resin", "permit_socks_gotch_rusty_grubby", "permit_socks_gotch_grape_alkali", "permit_socks_gotch_charcoal_obelus"
		});
		InventoryOrganization.AddSubcategory("ATMOSUIT_BODIES_BASIC", Assets.GetSprite("icon_inventory_atmosuit_body"), 200, new string[]
		{
			"AtmoSuitBasicYellow", "AtmoSuitSparkleRed", "AtmoSuitSparkleGreen", "AtmoSuitSparkleBlue", "AtmoSuitSparkleLavender", "AtmoSuitPuft", "AtmoSuitConfetti", "AtmoSuitCrispEggplant", "AtmoSuitBasicNeonPink", "AtmoSuitMultiRedBlack",
			"AtmoSuitRocketmelon", "AtmoSuitMultiBlueGreyBlack", "AtmoSuitMultiBlueYellowRed", "permit_atmosuit_80s", "permit_atmosuit_basic_purple_wildberry", "permit_atmosuit_basic_orange", "permit_atmosuit_raptor", "permit_atmosuit_stego", "permit_atmosuit_chameleo", "permit_atmosuit_paculacanth",
			"permit_atmosuit_neutronium", "permit_atmosuit_diving_khaki_tan", "permit_atmosuit_basic_rubble_electro", "permit_atmosuit_turtle", "permit_atmosuit_tap_01"
		});
		InventoryOrganization.AddSubcategory("ATMOSUIT_HELMETS_BASIC", Assets.GetSprite("icon_inventory_atmosuit_helmet"), 100, new string[]
		{
			"AtmoHelmetLimone", "AtmoHelmetSparkleRed", "AtmoHelmetSparkleGreen", "AtmoHelmetSparkleBlue", "AtmoHelmetSparklePurple", "AtmoHelmetPuft", "AtmoHelmetConfetti", "AtmoHelmetEggplant", "AtmoHelmetCummerbundRed", "AtmoHelmetWorkoutLavender",
			"AtmoHelmetRocketmelon", "AtmoHelmetMondrianBlueRedYellow", "AtmoHelmetOverallsRed", "permit_atmo_helmet_80s", "permit_atmo_helmet_gaudysweater_purple", "permit_atmo_helmet_biocircuit", "permit_atmo_helmet_raptor", "permit_atmo_helmet_stego", "permit_atmo_helmet_chameleo", "permit_atmo_helmet_paculacanth",
			"permit_atmo_helmet_neutronium", "permit_atmo_helmet_diving_khaki_tan", "permit_atmo_helmet_tap_electro_rubble", "permit_atmo_helmet_tap_01", "permit_atmo_helmet_turtle"
		});
		InventoryOrganization.AddSubcategory("ATMOSUIT_GLOVES_BASIC", Assets.GetSprite("icon_inventory_atmosuit_gloves"), 500, new string[]
		{
			"AtmoGlovesLime", "AtmoGlovesSparkleRed", "AtmoGlovesSparkleGreen", "AtmoGlovesSparkleBlue", "AtmoGlovesSparkleLavender", "AtmoGlovesPuft", "AtmoGlovesGold", "AtmoGlovesEggplant", "AtmoGlovesWhite", "AtmoGlovesStripesLavender",
			"AtmoGlovesRocketmelon", "AtmoGlovesBrown", "permit_atmo_gloves_80s", "permit_atmo_gloves_plum", "permit_atmo_gloves_biocircuit", "permit_atmo_gloves_raptor", "permit_atmo_gloves_stego", "permit_atmo_gloves_chameleo", "permit_atmo_gloves_paculacanth", "permit_atmo_gloves_neutronium",
			"permit_atmo_gloves_diving_khaki_tan", "permit_atmo_gloves_electro", "permit_atmo_gloves_tap_01", "permit_atmo_gloves_turtle"
		});
		InventoryOrganization.AddSubcategory("ATMOSUIT_BELTS_BASIC", Assets.GetSprite("icon_inventory_atmosuit_belt"), 700, new string[]
		{
			"AtmoBeltBasicLime", "AtmoBeltSparkleRed", "AtmoBeltSparkleGreen", "AtmoBeltSparkleBlue", "AtmoBeltSparkleLavender", "AtmoBeltPuft", "AtmoBeltBasicGold", "AtmoBeltEggplant", "AtmoBeltBasicGrey", "AtmoBeltBasicNeonPink",
			"AtmoBeltRocketmelon", "AtmoBeltTwoToneBrown", "permit_atmo_belt_80s", "permit_atmo_belt_3tone_purple", "permit_atmo_belt_circuit", "permit_atmo_belt_raptor", "permit_atmo_belt_stego", "permit_atmo_belt_chameleo", "permit_atmo_belt_paculacanth", "permit_atmo_belt_neutronium",
			"permit_atmo_belt_diving_tan", "permit_atmo_belt_floatie", "permit_atmo_belt_duckie_gasball", "permit_atmo_belt_turtle"
		});
		InventoryOrganization.AddSubcategory("ATMOSUIT_SHOES_BASIC", Assets.GetSprite("icon_inventory_atmosuit_boots"), 900, new string[]
		{
			"AtmoShoesBasicYellow", "AtmoShoesSparkleBlack", "AtmoShoesPuft", "AtmoShoesStealth", "AtmoShoesEggplant", "AtmoShoesBasicLavender", "AtmoBootsRocketmelon", "permit_atmo_shoes_80s", "permit_atmo_shoes_basic_green", "permit_atmo_shoes_biocircuit",
			"permit_atmo_shoes_raptor", "permit_atmo_shoes_stego", "permit_atmo_shoes_chameleo", "permit_atmo_shoes_paculacanth", "permit_atmo_shoes_neutronium", "permit_atmo_shoes_diving_tan", "permit_atmo_shoes_basic_electro", "permit_atmo_shoes_tap_01", "permit_atmo_shoes_turtle"
		});
		InventoryOrganization.AddSubcategory("JETSUIT_HELMETS_BASIC", Assets.GetSprite("icon_inventory_jetsuit_helmet"), 1000, new string[] { "permit_jet_helmet_00_torpor_obelus", "permit_jet_helmet_00_charcoal_orange", "permit_jet_helmet_00_magenta_obelus", "permit_jet_helmet_00_red_obelus", "permit_jet_helmet_00_atomic_gloom", "permit_jet_helmet_00_kelly_obelus", "permit_jet_helmet_00_hatchstone_obelus", "permit_jet_helmet_00_ultra_obelus", "permit_jet_helmet_neutronium" });
		InventoryOrganization.AddSubcategory("JETSUIT_BODIES_BASIC", Assets.GetSprite("icon_inventory_jetsuit_body"), 1100, new string[] { "permit_jetsuit_black_hatchstone", "permit_jetsuit_black_kelly", "permit_jetsuit_black_red", "permit_jetsuit_black_torpor", "permit_jetsuit_black_ultra", "permit_jetsuit_white_atomic", "permit_jetsuit_white_charcoal", "permit_jetsuit_white_magenta", "permit_jetsuit_neutronium" });
		InventoryOrganization.AddSubcategory("JETSUIT_GLOVES_BASIC", Assets.GetSprite("icon_inventory_jetsuit_gloves"), 1200, new string[] { "permit_jet_gloves_cosmic", "permit_jet_gloves_gloom", "permit_jet_gloves_noshleaf", "permit_jet_gloves_obelus", "permit_jet_gloves_orange", "permit_jet_gloves_redclay", "permit_jet_gloves_neutronium" });
		InventoryOrganization.AddSubcategory("JETSUIT_SHOES_BASIC", Assets.GetSprite("icon_inventory_jetsuit_boots"), 1300, new string[] { "permit_jet_shoes_basic_black", "permit_jet_shoes_basic_white", "permit_jet_shoes_neutronium" });
	}

	public static Dictionary<string, List<string>> categoryIdToSubcategoryIdsMap = new Dictionary<string, List<string>>();

	public static Dictionary<string, Sprite> categoryIdToIconMap = new Dictionary<string, Sprite>();

	public static Dictionary<string, bool> categoryIdToIsEmptyMap = new Dictionary<string, bool>();

	public static bool initialized = false;

	public static Dictionary<string, List<string>> subcategoryIdToPermitIdsMap = new Dictionary<string, List<string>>
	{
		{
			"UNCATEGORIZED",
			new List<string>()
		},
		{
			"YAML",
			new List<string>()
		}
	};

	public static Dictionary<string, InventoryOrganization.SubcategoryPresentationData> subcategoryIdToPresentationDataMap = new Dictionary<string, InventoryOrganization.SubcategoryPresentationData>
	{
		{
			"UNCATEGORIZED",
			new InventoryOrganization.SubcategoryPresentationData("UNCATEGORIZED", Assets.GetSprite("error_message"), 0)
		},
		{
			"YAML",
			new InventoryOrganization.SubcategoryPresentationData("YAML", Assets.GetSprite("error_message"), 0)
		}
	};

	public class SubcategoryPresentationData
	{
		public SubcategoryPresentationData(string subcategoryID, Sprite icon, int sortKey)
		{
			this.subcategoryID = subcategoryID;
			this.sortKey = sortKey;
			this.icon = icon;
		}

		public string subcategoryID;

		public int sortKey;

		public Sprite icon;
	}

	public static class InventoryPermitCategories
	{
		public const string CLOTHING_TOPS = "CLOTHING_TOPS";

		public const string CLOTHING_BOTTOMS = "CLOTHING_BOTTOMS";

		public const string CLOTHING_GLOVES = "CLOTHING_GLOVES";

		public const string CLOTHING_SHOES = "CLOTHING_SHOES";

		public const string ATMOSUITS = "ATMOSUITS";

		public const string BUILDINGS = "BUILDINGS";

		public const string WALLPAPERS = "WALLPAPERS";

		public const string ARTWORK = "ARTWORK";

		public const string JOY_RESPONSES = "JOY_RESPONSES";

		public const string JETSUITS = "JETSUITS";
	}

	public static class PermitSubcategories
	{
		public const string YAML = "YAML";

		public const string UNCATEGORIZED = "UNCATEGORIZED";

		public const string JOY_BALLOON = "JOY_BALLOON";

		public const string JOY_STICKER = "JOY_STICKER";

		public const string PRIMO_GARB = "PRIMO_GARB";

		public const string CLOTHING_TOPS_BASIC = "CLOTHING_TOPS_BASIC";

		public const string CLOTHING_TOPS_TSHIRT = "CLOTHING_TOPS_TSHIRT";

		public const string CLOTHING_TOPS_FANCY = "CLOTHING_TOPS_FANCY";

		public const string CLOTHING_TOPS_JACKET = "CLOTHING_TOPS_JACKET";

		public const string CLOTHING_TOPS_UNDERSHIRT = "CLOTHING_TOPS_UNDERSHIRT";

		public const string CLOTHING_TOPS_DRESS = "CLOTHING_TOPS_DRESS";

		public const string CLOTHING_BOTTOMS_BASIC = "CLOTHING_BOTTOMS_BASIC";

		public const string CLOTHING_BOTTOMS_FANCY = "CLOTHING_BOTTOMS_FANCY";

		public const string CLOTHING_BOTTOMS_SHORTS = "CLOTHING_BOTTOMS_SHORTS";

		public const string CLOTHING_BOTTOMS_SKIRTS = "CLOTHING_BOTTOMS_SKIRTS";

		public const string CLOTHING_BOTTOMS_UNDERWEAR = "CLOTHING_BOTTOMS_UNDERWEAR";

		public const string CLOTHING_GLOVES_BASIC = "CLOTHING_GLOVES_BASIC";

		public const string CLOTHING_GLOVES_PRINTS = "CLOTHING_GLOVES_PRINTS";

		public const string CLOTHING_GLOVES_SHORT = "CLOTHING_GLOVES_SHORT";

		public const string CLOTHING_GLOVES_FORMAL = "CLOTHING_GLOVES_FORMAL";

		public const string CLOTHING_SHOES_BASIC = "CLOTHING_SHOES_BASIC";

		public const string CLOTHING_SHOES_FANCY = "CLOTHING_SHOES_FANCY";

		public const string CLOTHING_SHOE_SOCKS = "CLOTHING_SHOE_SOCKS";

		public const string ATMOSUIT_HELMETS_BASIC = "ATMOSUIT_HELMETS_BASIC";

		public const string ATMOSUIT_HELMETS_FANCY = "ATMOSUIT_HELMETS_FANCY";

		public const string ATMOSUIT_BODIES_BASIC = "ATMOSUIT_BODIES_BASIC";

		public const string ATMOSUIT_BODIES_FANCY = "ATMOSUIT_BODIES_FANCY";

		public const string ATMOSUIT_GLOVES_BASIC = "ATMOSUIT_GLOVES_BASIC";

		public const string ATMOSUIT_GLOVES_FANCY = "ATMOSUIT_GLOVES_FANCY";

		public const string ATMOSUIT_BELTS_BASIC = "ATMOSUIT_BELTS_BASIC";

		public const string ATMOSUIT_BELTS_FANCY = "ATMOSUIT_BELTS_FANCY";

		public const string ATMOSUIT_SHOES_BASIC = "ATMOSUIT_SHOES_BASIC";

		public const string ATMOSUIT_SHOES_FANCY = "ATMOSUIT_SHOES_FANCY";

		public const string BUILDING_WALLPAPER_BASIC = "BUILDING_WALLPAPER_BASIC";

		public const string BUILDING_WALLPAPER_FANCY = "BUILDING_WALLPAPER_FANCY";

		public const string BUILDING_WALLPAPER_PRINTS = "BUILDING_WALLPAPER_PRINTS";

		public const string BUILDINGS_STORAGE = "BUILDINGS_STORAGE";

		public const string BUILDINGS_INDUSTRIAL = "BUILDINGS_INDUSTRIAL";

		public const string BUILDINGS_FOOD = "BUILDINGS_FOOD";

		public const string BUILDINGS_RANCHING = "BUILDINGS_RANCHING";

		public const string BUILDINGS_WASHROOM = "BUILDINGS_WASHROOM";

		public const string BUILDINGS_RECREATION = "BUILDINGS_RECREATION";

		public const string BUILDINGS_PRINTING_POD = "BUILDINGS_PRINTING_POD";

		public const string BUILDING_CANVAS_STANDARD = "BUILDING_CANVAS_STANDARD";

		public const string BUILDING_CANVAS_PORTRAIT = "BUILDING_CANVAS_PORTRAIT";

		public const string BUILDING_CANVAS_LANDSCAPE = "BUILDING_CANVAS_LANDSCAPE";

		public const string BUILDING_SCULPTURE = "BUILDING_SCULPTURE";

		public const string MONUMENT_BOTTOM = "MONUMENT_BOTTOM";

		public const string MONUMENT_MIDDLE = "MONUMENT_MIDDLE";

		public const string MONUMENT_TOP = "MONUMENT_TOP";

		public const string BUILDINGS_FLOWER_VASE = "BUILDINGS_FLOWER_VASE";

		public const string BUILDINGS_BED_COT = "BUILDINGS_BED_COT";

		public const string BUILDINGS_BED_LUXURY = "BUILDINGS_BED_LUXURY";

		public const string BUILDING_CEILING_LIGHT = "BUILDING_CEILING_LIGHT";

		public const string BUILDINGS_RESEARCH = "BUILDINGS_RESEARCH";

		public const string BUILDINGS_AUTOMATION = "BUILDINGS_AUTOMATION";

		public const string BUILDINGS_ELECTRIC_WIRES = "BUILDINGS_ELECTIC_WIRES";

		public const string JETSUIT_HELMETS_BASIC = "JETSUIT_HELMETS_BASIC";

		public const string JETSUIT_BODIES_BASIC = "JETSUIT_BODIES_BASIC";

		public const string JETSUIT_GLOVES_BASIC = "JETSUIT_GLOVES_BASIC";

		public const string JETSUIT_SHOES_BASIC = "JETSUIT_SHOES_BASIC";
	}
}
