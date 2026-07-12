using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class BuildingFacades : ResourceSet<BuildingFacadeResource>
	{
		public BuildingFacades(ResourceSet parent)
			: base("BuildingFacades", parent)
		{
			base.Initialize();
			if (!DlcManager.IsExpansion1Active())
			{
				for (int i = 0; i < BuildingFacades.Infos_All.Length; i++)
				{
					BuildingFacades.Info info = BuildingFacades.Infos_All[i];
					if (info.DLCIds == DlcManager.AVAILABLE_EXPANSION1_ONLY)
					{
						info.AnimFile = "so_kanim";
					}
				}
			}
			foreach (BuildingFacades.Info info2 in BuildingFacades.Infos_All)
			{
				this.Add(info2.Id, info2.Name, info2.Description, info2.Rarity, info2.PrefabID, info2.AnimFile, info2.DLCIds, null);
			}
		}

		public void Add(string id, LocString Name, LocString Desc, PermitRarity rarity, string prefabId, string animFile, Dictionary<string, string> workables = null)
		{
			BuildingFacadeResource buildingFacadeResource = new BuildingFacadeResource(id, Name, Desc, rarity, prefabId, animFile, DlcManager.AVAILABLE_ALL_VERSIONS, workables);
			this.resources.Add(buildingFacadeResource);
		}

		public void Add(string id, LocString Name, LocString Desc, PermitRarity rarity, string prefabId, string animFile, string[] DLCIds, Dictionary<string, string> workables = null)
		{
			BuildingFacadeResource buildingFacadeResource = new BuildingFacadeResource(id, Name, Desc, rarity, prefabId, animFile, DLCIds, workables);
			this.resources.Add(buildingFacadeResource);
		}

		public void PostProcess()
		{
			foreach (BuildingFacadeResource buildingFacadeResource in this.resources)
			{
				buildingFacadeResource.Init();
			}
		}

		public static BuildingFacades.Info[] Infos_Skins = new BuildingFacades.Info[]
		{
			new BuildingFacades.Info("ExteriorWall_basic_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_WHITE.DESC, PermitRarity.Universal, "ExteriorWall", "walls_basic_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVase_retro", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_SUNNY.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_SUNNY.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_yellow_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVase_retro_red", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BOLD.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BOLD.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_red_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVase_retro_white", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_ELEGANT.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_ELEGANT.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVase_retro_green", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BRIGHT.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BRIGHT.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_green_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVase_retro_blue", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_DREAMY.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_DREAMY.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_blue_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("LuxuryBed_boat", BUILDINGS.PREFABS.LUXURYBED.FACADES.BOAT.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.BOAT.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_boat_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("LuxuryBed_bouncy", BUILDINGS.PREFABS.LUXURYBED.FACADES.BOUNCY_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.BOUNCY_BED.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_bouncy_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("LuxuryBed_grandprix", BUILDINGS.PREFABS.LUXURYBED.FACADES.GRANDPRIX.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.GRANDPRIX.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_grandprix_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("LuxuryBed_rocket", BUILDINGS.PREFABS.LUXURYBED.FACADES.ROCKET_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.ROCKET_BED.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_rocket_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("LuxuryBed_puft", BUILDINGS.PREFABS.LUXURYBED.FACADES.PUFT_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.PUFT_BED.DESC, PermitRarity.Loyalty, "LuxuryBed", "elegantbed_puft_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_pastel_pink", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPINK.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPINK.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_pink_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_pastel_yellow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELYELLOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELYELLOW.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_yellow_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_pastel_green", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELGREEN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELGREEN.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_green_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_pastel_blue", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELBLUE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELBLUE.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_blue_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_pastel_purple", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPURPLE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPURPLE.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_purple_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_balm_lily", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BALM_LILY.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BALM_LILY.DESC, PermitRarity.Decent, "ExteriorWall", "walls_balm_lily_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_clouds", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CLOUDS.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CLOUDS.DESC, PermitRarity.Decent, "ExteriorWall", "walls_clouds_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_coffee", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.COFFEE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.COFFEE.DESC, PermitRarity.Decent, "ExteriorWall", "walls_coffee_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_mosaic", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.AQUATICMOSAIC.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.AQUATICMOSAIC.DESC, PermitRarity.Decent, "ExteriorWall", "walls_mosaic_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_mushbar", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.MUSHBAR.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.MUSHBAR.DESC, PermitRarity.Decent, "ExteriorWall", "walls_mushbar_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plaid", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLAID.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLAID.DESC, PermitRarity.Decent, "ExteriorWall", "walls_plaid_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_rain", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAIN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAIN.DESC, PermitRarity.Decent, "ExteriorWall", "walls_rain_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_rainbow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAINBOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAINBOW.DESC, PermitRarity.Decent, "ExteriorWall", "walls_rainbow_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_snow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SNOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SNOW.DESC, PermitRarity.Decent, "ExteriorWall", "walls_snow_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_sun", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SUN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SUN.DESC, PermitRarity.Decent, "ExteriorWall", "walls_sun_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_polka", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.DESC, PermitRarity.Decent, "ExteriorWall", "walls_polka_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_diagonal_red_deep_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_RED_DEEP_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_RED_DEEP_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_red_deep_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_diagonal_orange_satsuma_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_ORANGE_SATSUMA_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_ORANGE_SATSUMA_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_orange_satsuma_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_diagonal_yellow_lemon_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_YELLOW_LEMON_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_YELLOW_LEMON_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_yellow_lemon_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_diagonal_green_kelly_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_GREEN_KELLY_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_GREEN_KELLY_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_green_kelly_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_diagonal_blue_cobalt_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_BLUE_COBALT_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_BLUE_COBALT_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_blue_cobalt_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_diagonal_pink_flamingo_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_PINK_FLAMINGO_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_PINK_FLAMINGO_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_pink_flamingo_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_diagonal_grey_charcoal_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_GREY_CHARCOAL_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_GREY_CHARCOAL_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_grey_charcoal_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_circle_red_deep_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_RED_DEEP_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_RED_DEEP_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_red_deep_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_circle_orange_satsuma_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_ORANGE_SATSUMA_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_ORANGE_SATSUMA_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_orange_satsuma_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_circle_yellow_lemon_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_YELLOW_LEMON_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_YELLOW_LEMON_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_yellow_lemon_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_circle_green_kelly_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_GREEN_KELLY_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_GREEN_KELLY_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_green_kelly_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_circle_blue_cobalt_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_BLUE_COBALT_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_BLUE_COBALT_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_blue_cobalt_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_circle_pink_flamingo_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_PINK_FLAMINGO_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_PINK_FLAMINGO_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_pink_flamingo_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_circle_grey_charcoal_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_GREY_CHARCOAL_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_GREY_CHARCOAL_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_grey_charcoal_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("Bed_star_curtain", BUILDINGS.PREFABS.BED.FACADES.STARCURTAIN.NAME, BUILDINGS.PREFABS.BED.FACADES.STARCURTAIN.DESC, PermitRarity.Nifty, "Bed", "bed_star_curtain_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("Bed_canopy", BUILDINGS.PREFABS.BED.FACADES.CREAKY.NAME, BUILDINGS.PREFABS.BED.FACADES.CREAKY.DESC, PermitRarity.Nifty, "Bed", "bed_canopy_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("Bed_rowan_tropical", BUILDINGS.PREFABS.BED.FACADES.STAYCATION.NAME, BUILDINGS.PREFABS.BED.FACADES.STAYCATION.DESC, PermitRarity.Nifty, "Bed", "bed_rowan_tropical_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("Bed_ada_science_lab", BUILDINGS.PREFABS.BED.FACADES.SCIENCELAB.NAME, BUILDINGS.PREFABS.BED.FACADES.SCIENCELAB.DESC, PermitRarity.Nifty, "Bed", "bed_ada_science_lab_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("CeilingLight_mining", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.MINING.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.MINING.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_mining_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("CeilingLight_flower", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.BLOSSOM.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.BLOSSOM.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_flower_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("CeilingLight_polka_lamp_shade", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.POLKADOT.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.POLKADOT.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_polka_lamp_shade_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("CeilingLight_burt_shower", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.FAUXPIPE.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.FAUXPIPE.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_burt_shower_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("CeilingLight_ada_flask_round", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.LABFLASK.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.LABFLASK.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_ada_flask_round_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseWall_retro_green", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_GREEN.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_GREEN.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_green_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseWall_retro_yellow", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_YELLOW.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_YELLOW.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_yellow_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseWall_retro_red", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_RED.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_RED.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_red_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseWall_retro_blue", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_BLUE.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_BLUE.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_blue_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseWall_retro_white", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_WHITE.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_WHITE.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_basic_blue_cobalt", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_BLUE_COBALT.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_BLUE_COBALT.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_blue_cobalt_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_basic_green_kelly", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_GREEN_KELLY.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_GREEN_KELLY.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_green_kelly_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_basic_grey_charcoal", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_GREY_CHARCOAL.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_GREY_CHARCOAL.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_grey_charcoal_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_basic_orange_satsuma", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_ORANGE_SATSUMA.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_ORANGE_SATSUMA.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_orange_satsuma_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_basic_pink_flamingo", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_PINK_FLAMINGO.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_PINK_FLAMINGO.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_pink_flamingo_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_basic_red_deep", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_RED_DEEP.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_RED_DEEP.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_red_deep_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_basic_yellow_lemon", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_YELLOW_LEMON.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_YELLOW_LEMON.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_yellow_lemon_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_blueberries", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BLUEBERRIES.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BLUEBERRIES.DESC, PermitRarity.Decent, "ExteriorWall", "walls_blueberries_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_grapes", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.GRAPES.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.GRAPES.DESC, PermitRarity.Decent, "ExteriorWall", "walls_grapes_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_lemon", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.LEMON.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.LEMON.DESC, PermitRarity.Decent, "ExteriorWall", "walls_lemon_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_lime", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.LIME.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.LIME.DESC, PermitRarity.Decent, "ExteriorWall", "walls_lime_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_satsuma", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SATSUMA.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SATSUMA.DESC, PermitRarity.Decent, "ExteriorWall", "walls_satsuma_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_strawberry", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRAWBERRY.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRAWBERRY.DESC, PermitRarity.Decent, "ExteriorWall", "walls_strawberry_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_watermelon", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.WATERMELON.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.WATERMELON.DESC, PermitRarity.Decent, "ExteriorWall", "walls_watermelon_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseHanging_retro_red", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_RED.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_RED.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_red_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseHanging_retro_green", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_GREEN.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_GREEN.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_green_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseHanging_retro_blue", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_BLUE.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_BLUE.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_blue_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseHanging_retro_yellow", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_YELLOW.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_YELLOW.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_yellow_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseHanging_retro_white", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_WHITE.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_WHITE.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_toiletpaper", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.TOILETPAPER.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.TOILETPAPER.DESC, PermitRarity.Decent, "ExteriorWall", "walls_toiletpaper_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plunger", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUNGER.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUNGER.DESC, PermitRarity.Decent, "ExteriorWall", "walls_plunger_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_tropical", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.TROPICAL.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.TROPICAL.DESC, PermitRarity.Decent, "ExteriorWall", "walls_tropical_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ItemPedestal_hand", BUILDINGS.PREFABS.ITEMPEDESTAL.FACADES.HAND.NAME, BUILDINGS.PREFABS.ITEMPEDESTAL.FACADES.HAND.DESC, PermitRarity.Decent, "ItemPedestal", "pedestal_hand_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("MassageTable_shiatsu", BUILDINGS.PREFABS.MASSAGETABLE.FACADES.SHIATSU.NAME, BUILDINGS.PREFABS.MASSAGETABLE.FACADES.SHIATSU.DESC, PermitRarity.Splendid, "MassageTable", "masseur_shiatsu_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("RockCrusher_hands", BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.HANDS.NAME, BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.HANDS.DESC, PermitRarity.Splendid, "RockCrusher", "rockrefinery_hands_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("RockCrusher_teeth", BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.TEETH.NAME, BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.TEETH.DESC, PermitRarity.Splendid, "RockCrusher", "rockrefinery_teeth_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("WaterCooler_round_body", BUILDINGS.PREFABS.WATERCOOLER.FACADES.ROUND_BODY.NAME, BUILDINGS.PREFABS.WATERCOOLER.FACADES.ROUND_BODY.DESC, PermitRarity.Splendid, "WaterCooler", "watercooler_round_body_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_stripes_blue", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRIPES_BLUE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRIPES_BLUE.DESC, PermitRarity.Decent, "ExteriorWall", "walls_stripes_blue_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_stripes_diagonal_blue", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRIPES_DIAGONAL_BLUE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRIPES_DIAGONAL_BLUE.DESC, PermitRarity.Decent, "ExteriorWall", "walls_stripes_diagonal_blue_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_stripes_circle_blue", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRIPES_CIRCLE_BLUE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRIPES_CIRCLE_BLUE.DESC, PermitRarity.Decent, "ExteriorWall", "walls_stripes_circle_blue_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_squares_red_deep_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_RED_DEEP_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_RED_DEEP_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_squares_red_deep_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_squares_orange_satsuma_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_ORANGE_SATSUMA_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_ORANGE_SATSUMA_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_squares_orange_satsuma_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_squares_yellow_lemon_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_YELLOW_LEMON_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_YELLOW_LEMON_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_squares_yellow_lemon_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_squares_green_kelly_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_GREEN_KELLY_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_GREEN_KELLY_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_squares_green_kelly_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_squares_blue_cobalt_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_BLUE_COBALT_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_BLUE_COBALT_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_squares_blue_cobalt_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_squares_pink_flamingo_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_PINK_FLAMINGO_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_PINK_FLAMINGO_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_squares_pink_flamingo_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_squares_grey_charcoal_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_GREY_CHARCOAL_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SQUARES_GREY_CHARCOAL_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_squares_grey_charcoal_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("EggCracker_beaker", BUILDINGS.PREFABS.EGGCRACKER.FACADES.BEAKER.NAME, BUILDINGS.PREFABS.EGGCRACKER.FACADES.BEAKER.DESC, PermitRarity.Nifty, "EggCracker", "egg_cracker_beaker_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("EggCracker_flower", BUILDINGS.PREFABS.EGGCRACKER.FACADES.FLOWER.NAME, BUILDINGS.PREFABS.EGGCRACKER.FACADES.FLOWER.DESC, PermitRarity.Nifty, "EggCracker", "egg_cracker_flower_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("EggCracker_hands", BUILDINGS.PREFABS.EGGCRACKER.FACADES.HANDS.NAME, BUILDINGS.PREFABS.EGGCRACKER.FACADES.HANDS.DESC, PermitRarity.Nifty, "EggCracker", "egg_cracker_hands_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("CeilingLight_rubiks", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.RUBIKS.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.RUBIKS.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_rubiks_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseHanging_beaker", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.BEAKER.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.BEAKER.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_beaker_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("FlowerVaseHanging_rubiks", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RUBIKS.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RUBIKS.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_rubiks_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("LuxuryBed_hand", BUILDINGS.PREFABS.LUXURYBED.FACADES.HAND.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.HAND.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_hand_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("LuxuryBed_rubiks", BUILDINGS.PREFABS.LUXURYBED.FACADES.RUBIKS.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.RUBIKS.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_rubiks_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("RockCrusher_roundstamp", BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.ROUNDSTAMP.NAME, BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.ROUNDSTAMP.DESC, PermitRarity.Splendid, "RockCrusher", "rockrefinery_roundstamp_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("RockCrusher_spikebeds", BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.SPIKEBEDS.NAME, BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.SPIKEBEDS.DESC, PermitRarity.Splendid, "RockCrusher", "rockrefinery_spikebeds_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("StorageLocker_green_mush", BUILDINGS.PREFABS.STORAGELOCKER.FACADES.GREEN_MUSH.NAME, BUILDINGS.PREFABS.STORAGELOCKER.FACADES.GREEN_MUSH.DESC, PermitRarity.Nifty, "StorageLocker", "storagelocker_green_mush_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("StorageLocker_red_rose", BUILDINGS.PREFABS.STORAGELOCKER.FACADES.RED_ROSE.NAME, BUILDINGS.PREFABS.STORAGELOCKER.FACADES.RED_ROSE.DESC, PermitRarity.Nifty, "StorageLocker", "storagelocker_red_rose_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("StorageLocker_blue_babytears", BUILDINGS.PREFABS.STORAGELOCKER.FACADES.BLUE_BABYTEARS.NAME, BUILDINGS.PREFABS.STORAGELOCKER.FACADES.BLUE_BABYTEARS.DESC, PermitRarity.Nifty, "StorageLocker", "storagelocker_blue_babytears_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("StorageLocker_purple_brainfat", BUILDINGS.PREFABS.STORAGELOCKER.FACADES.PURPLE_BRAINFAT.NAME, BUILDINGS.PREFABS.STORAGELOCKER.FACADES.PURPLE_BRAINFAT.DESC, PermitRarity.Nifty, "StorageLocker", "storagelocker_purple_brainfat_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("StorageLocker_yellow_tartar", BUILDINGS.PREFABS.STORAGELOCKER.FACADES.YELLOW_TARTAR.NAME, BUILDINGS.PREFABS.STORAGELOCKER.FACADES.YELLOW_TARTAR.DESC, PermitRarity.Nifty, "StorageLocker", "storagelocker_yellow_tartar_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("PlanterBox_mealwood", BUILDINGS.PREFABS.PLANTERBOX.FACADES.MEALWOOD.NAME, BUILDINGS.PREFABS.PLANTERBOX.FACADES.MEALWOOD.DESC, PermitRarity.Common, "PlanterBox", "planterbox_skin_mealwood_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("PlanterBox_bristleblossom", BUILDINGS.PREFABS.PLANTERBOX.FACADES.BRISTLEBLOSSOM.NAME, BUILDINGS.PREFABS.PLANTERBOX.FACADES.BRISTLEBLOSSOM.DESC, PermitRarity.Common, "PlanterBox", "planterbox_skin_bristleblossom_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("PlanterBox_wheezewort", BUILDINGS.PREFABS.PLANTERBOX.FACADES.WHEEZEWORT.NAME, BUILDINGS.PREFABS.PLANTERBOX.FACADES.WHEEZEWORT.DESC, PermitRarity.Decent, "PlanterBox", "planterbox_skin_wheezewort_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("PlanterBox_sleetwheat", BUILDINGS.PREFABS.PLANTERBOX.FACADES.SLEETWHEAT.NAME, BUILDINGS.PREFABS.PLANTERBOX.FACADES.SLEETWHEAT.DESC, PermitRarity.Common, "PlanterBox", "planterbox_skin_sleetwheat_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("PlanterBox_salmon_pink", BUILDINGS.PREFABS.PLANTERBOX.FACADES.SALMON_PINK.NAME, BUILDINGS.PREFABS.PLANTERBOX.FACADES.SALMON_PINK.DESC, PermitRarity.Common, "PlanterBox", "planterbox_skin_salmon_pink_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_lightgold", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.LIGHTGOLD.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.LIGHTGOLD.DESC, PermitRarity.Nifty, "GasReservoir", "gasstorage_lightgold_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_peagreen", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.PEAGREEN.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.PEAGREEN.DESC, PermitRarity.Nifty, "GasReservoir", "gasstorage_peagreen_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_lightcobalt", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.LIGHTCOBALT.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.LIGHTCOBALT.DESC, PermitRarity.Nifty, "GasReservoir", "gasstorage_lightcobalt_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_polka_darkpurpleresin", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.POLKA_DARKPURPLERESIN.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.POLKA_DARKPURPLERESIN.DESC, PermitRarity.Splendid, "GasReservoir", "gasstorage_polka_darkpurpleresin_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_polka_darknavynookgreen", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.POLKA_DARKNAVYNOOKGREEN.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.POLKA_DARKNAVYNOOKGREEN.DESC, PermitRarity.Splendid, "GasReservoir", "gasstorage_polka_darknavynookgreen_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_kitchen_retro1", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.KITCHEN_RETRO1.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.KITCHEN_RETRO1.DESC, PermitRarity.Decent, "ExteriorWall", "walls_kitchen_retro1_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plus_red_deep_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_RED_DEEP_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_RED_DEEP_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_plus_red_deep_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plus_orange_satsuma_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_ORANGE_SATSUMA_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_ORANGE_SATSUMA_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_plus_orange_satsuma_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plus_yellow_lemon_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_YELLOW_LEMON_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_YELLOW_LEMON_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_plus_yellow_lemon_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plus_green_kelly_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_GREEN_KELLY_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_GREEN_KELLY_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_plus_green_kelly_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plus_blue_cobalt_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_BLUE_COBALT_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_BLUE_COBALT_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_plus_blue_cobalt_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plus_pink_flamingo_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_PINK_FLAMINGO_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_PINK_FLAMINGO_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_plus_pink_flamingo_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("ExteriorWall_plus_grey_charcoal_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_GREY_CHARCOAL_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUS_GREY_CHARCOAL_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_plus_grey_charcoal_white_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("RockCrusher_chomp", BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.CHOMP.NAME, BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.CHOMP.DESC, PermitRarity.Splendid, "RockCrusher", "rockrefinery_chomp_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("RockCrusher_gears", BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.GEARS.NAME, BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.GEARS.DESC, PermitRarity.Splendid, "RockCrusher", "rockrefinery_gears_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("RockCrusher_balloon", BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.BALLOON.NAME, BUILDINGS.PREFABS.ROCKCRUSHER.FACADES.BALLOON.DESC, PermitRarity.Splendid, "RockCrusher", "rockrefinery_balloon_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("StorageLocker_polka_darknavynookgreen", BUILDINGS.PREFABS.STORAGELOCKER.FACADES.POLKA_DARKNAVYNOOKGREEN.NAME, BUILDINGS.PREFABS.STORAGELOCKER.FACADES.POLKA_DARKNAVYNOOKGREEN.DESC, PermitRarity.Splendid, "StorageLocker", "storagelocker_polka_darknavynookgreen_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("StorageLocker_polka_darkpurpleresin", BUILDINGS.PREFABS.STORAGELOCKER.FACADES.POLKA_DARKPURPLERESIN.NAME, BUILDINGS.PREFABS.STORAGELOCKER.FACADES.POLKA_DARKPURPLERESIN.DESC, PermitRarity.Splendid, "StorageLocker", "storagelocker_polka_darkpurpleresin_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_blue_babytears", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.BLUE_BABYTEARS.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.BLUE_BABYTEARS.DESC, PermitRarity.Nifty, "GasReservoir", "gasstorage_blue_babytears_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_yellow_tartar", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.YELLOW_TARTAR.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.YELLOW_TARTAR.DESC, PermitRarity.Nifty, "GasReservoir", "gasstorage_yellow_tartar_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_green_mush", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.GREEN_MUSH.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.GREEN_MUSH.DESC, PermitRarity.Nifty, "GasReservoir", "gasstorage_green_mush_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_red_rose", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.RED_ROSE.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.RED_ROSE.DESC, PermitRarity.Nifty, "GasReservoir", "gasstorage_red_rose_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("GasReservoir_purple_brainfat", BUILDINGS.PREFABS.GASRESERVOIR.FACADES.PURPLE_BRAINFAT.NAME, BUILDINGS.PREFABS.GASRESERVOIR.FACADES.PURPLE_BRAINFAT.DESC, PermitRarity.Nifty, "GasReservoir", "gasstorage_purple_brainfat_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("MassageTable_balloon", BUILDINGS.PREFABS.MASSAGETABLE.FACADES.MASSEUR_BALLOON.NAME, BUILDINGS.PREFABS.MASSAGETABLE.FACADES.MASSEUR_BALLOON.DESC, PermitRarity.Splendid, "MassageTable", "masseur_balloon_kanim", DlcManager.AVAILABLE_ALL_VERSIONS),
			new BuildingFacades.Info("WaterCooler_balloon", BUILDINGS.PREFABS.WATERCOOLER.FACADES.BALLOON.NAME, BUILDINGS.PREFABS.WATERCOOLER.FACADES.BALLOON.DESC, PermitRarity.Splendid, "WaterCooler", "watercooler_balloon_kanim", DlcManager.AVAILABLE_ALL_VERSIONS)
		};

		public static BuildingFacades.Info[] Infos_All = BuildingFacades.Infos_Skins;

		public struct Info
		{
			public Info(string Id, string Name, string Description, PermitRarity rarity, string PrefabID, string AnimFile, string[] DLCIds)
			{
				this.Id = Id;
				this.Name = Name;
				this.Description = Description;
				this.Rarity = rarity;
				this.PrefabID = PrefabID;
				this.AnimFile = AnimFile;
				this.DLCIds = DLCIds;
			}

			public string Id;

			public string Name;

			public string Description;

			public PermitRarity Rarity;

			public string PrefabID;

			public string AnimFile;

			public string[] DLCIds;
		}
	}
}
