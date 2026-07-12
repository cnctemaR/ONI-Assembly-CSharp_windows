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
			foreach (BuildingFacades.Info info in BuildingFacades.Infos_All)
			{
				this.Add(info.Id, info.Name, info.Description, info.Rarity, info.PrefabID, info.AnimFile, null);
			}
		}

		public void Add(string id, LocString Name, LocString Desc, PermitRarity rarity, string prefabId, string animFile, Dictionary<string, string> workables = null)
		{
			BuildingFacadeResource buildingFacadeResource = new BuildingFacadeResource(id, Name, Desc, rarity, prefabId, animFile, workables);
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
			new BuildingFacades.Info("ExteriorWall_basic_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_WHITE.DESC, PermitRarity.Universal, "ExteriorWall", "walls_basic_white_kanim"),
			new BuildingFacades.Info("FlowerVase_retro", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_SUNNY.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_SUNNY.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_yellow_kanim"),
			new BuildingFacades.Info("FlowerVase_retro_red", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BOLD.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BOLD.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_red_kanim"),
			new BuildingFacades.Info("FlowerVase_retro_white", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_ELEGANT.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_ELEGANT.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_white_kanim"),
			new BuildingFacades.Info("FlowerVase_retro_green", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BRIGHT.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BRIGHT.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_green_kanim"),
			new BuildingFacades.Info("FlowerVase_retro_blue", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_DREAMY.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_DREAMY.DESC, PermitRarity.Common, "FlowerVase", "flowervase_retro_blue_kanim"),
			new BuildingFacades.Info("LuxuryBed_boat", BUILDINGS.PREFABS.LUXURYBED.FACADES.BOAT.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.BOAT.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_boat_kanim"),
			new BuildingFacades.Info("LuxuryBed_bouncy", BUILDINGS.PREFABS.LUXURYBED.FACADES.BOUNCY_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.BOUNCY_BED.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_bouncy_kanim"),
			new BuildingFacades.Info("LuxuryBed_grandprix", BUILDINGS.PREFABS.LUXURYBED.FACADES.GRANDPRIX.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.GRANDPRIX.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_grandprix_kanim"),
			new BuildingFacades.Info("LuxuryBed_rocket", BUILDINGS.PREFABS.LUXURYBED.FACADES.ROCKET_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.ROCKET_BED.DESC, PermitRarity.Splendid, "LuxuryBed", "elegantbed_rocket_kanim"),
			new BuildingFacades.Info("LuxuryBed_puft", BUILDINGS.PREFABS.LUXURYBED.FACADES.PUFT_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.PUFT_BED.DESC, PermitRarity.Loyalty, "LuxuryBed", "elegantbed_puft_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_pink", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPINK.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPINK.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_pink_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_yellow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELYELLOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELYELLOW.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_yellow_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_green", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELGREEN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELGREEN.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_green_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_blue", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELBLUE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELBLUE.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_blue_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_purple", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPURPLE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPURPLE.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_purple_kanim"),
			new BuildingFacades.Info("ExteriorWall_balm_lily", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BALM_LILY.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BALM_LILY.DESC, PermitRarity.Decent, "ExteriorWall", "walls_balm_lily_kanim"),
			new BuildingFacades.Info("ExteriorWall_clouds", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CLOUDS.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CLOUDS.DESC, PermitRarity.Decent, "ExteriorWall", "walls_clouds_kanim"),
			new BuildingFacades.Info("ExteriorWall_coffee", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.COFFEE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.COFFEE.DESC, PermitRarity.Decent, "ExteriorWall", "walls_coffee_kanim"),
			new BuildingFacades.Info("ExteriorWall_mosaic", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.AQUATICMOSAIC.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.AQUATICMOSAIC.DESC, PermitRarity.Decent, "ExteriorWall", "walls_mosaic_kanim"),
			new BuildingFacades.Info("ExteriorWall_mushbar", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.MUSHBAR.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.MUSHBAR.DESC, PermitRarity.Decent, "ExteriorWall", "walls_mushbar_kanim"),
			new BuildingFacades.Info("ExteriorWall_plaid", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLAID.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLAID.DESC, PermitRarity.Decent, "ExteriorWall", "walls_plaid_kanim"),
			new BuildingFacades.Info("ExteriorWall_rain", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAIN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAIN.DESC, PermitRarity.Decent, "ExteriorWall", "walls_rain_kanim"),
			new BuildingFacades.Info("ExteriorWall_rainbow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAINBOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAINBOW.DESC, PermitRarity.Decent, "ExteriorWall", "walls_rainbow_kanim"),
			new BuildingFacades.Info("ExteriorWall_snow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SNOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SNOW.DESC, PermitRarity.Decent, "ExteriorWall", "walls_snow_kanim"),
			new BuildingFacades.Info("ExteriorWall_sun", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SUN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SUN.DESC, PermitRarity.Decent, "ExteriorWall", "walls_sun_kanim"),
			new BuildingFacades.Info("ExteriorWall_polka", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.DESC, PermitRarity.Decent, "ExteriorWall", "walls_polka_kanim"),
			new BuildingFacades.Info("ExteriorWall_diagonal_red_deep_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_RED_DEEP_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_RED_DEEP_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_red_deep_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_diagonal_orange_satsuma_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_ORANGE_SATSUMA_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_ORANGE_SATSUMA_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_orange_satsuma_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_diagonal_yellow_lemon_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_YELLOW_LEMON_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_YELLOW_LEMON_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_yellow_lemon_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_diagonal_green_kelly_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_GREEN_KELLY_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_GREEN_KELLY_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_green_kelly_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_diagonal_blue_cobalt_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_BLUE_COBALT_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_BLUE_COBALT_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_blue_cobalt_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_diagonal_pink_flamingo_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_PINK_FLAMINGO_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_PINK_FLAMINGO_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_pink_flamingo_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_diagonal_grey_charcoal_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_GREY_CHARCOAL_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.DIAGONAL_GREY_CHARCOAL_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_diagonal_grey_charcoal_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_circle_red_deep_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_RED_DEEP_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_RED_DEEP_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_red_deep_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_circle_orange_satsuma_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_ORANGE_SATSUMA_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_ORANGE_SATSUMA_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_orange_satsuma_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_circle_yellow_lemon_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_YELLOW_LEMON_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_YELLOW_LEMON_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_yellow_lemon_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_circle_green_kelly_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_GREEN_KELLY_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_GREEN_KELLY_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_green_kelly_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_circle_blue_cobalt_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_BLUE_COBALT_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_BLUE_COBALT_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_blue_cobalt_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_circle_pink_flamingo_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_PINK_FLAMINGO_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_PINK_FLAMINGO_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_pink_flamingo_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_circle_grey_charcoal_white", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_GREY_CHARCOAL_WHITE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CIRCLE_GREY_CHARCOAL_WHITE.DESC, PermitRarity.Common, "ExteriorWall", "walls_circle_grey_charcoal_white_kanim"),
			new BuildingFacades.Info("Bed_star_curtain", BUILDINGS.PREFABS.BED.FACADES.STARCURTAIN.NAME, BUILDINGS.PREFABS.BED.FACADES.STARCURTAIN.DESC, PermitRarity.Nifty, "Bed", "bed_star_curtain_kanim"),
			new BuildingFacades.Info("Bed_canopy", BUILDINGS.PREFABS.BED.FACADES.CREAKY.NAME, BUILDINGS.PREFABS.BED.FACADES.CREAKY.DESC, PermitRarity.Nifty, "Bed", "bed_canopy_kanim"),
			new BuildingFacades.Info("Bed_rowan_tropical", BUILDINGS.PREFABS.BED.FACADES.STAYCATION.NAME, BUILDINGS.PREFABS.BED.FACADES.STAYCATION.DESC, PermitRarity.Nifty, "Bed", "bed_rowan_tropical_kanim"),
			new BuildingFacades.Info("Bed_ada_science_lab", BUILDINGS.PREFABS.BED.FACADES.SCIENCELAB.NAME, BUILDINGS.PREFABS.BED.FACADES.SCIENCELAB.DESC, PermitRarity.Nifty, "Bed", "bed_ada_science_lab_kanim"),
			new BuildingFacades.Info("CeilingLight_mining", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.MINING.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.MINING.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_mining_kanim"),
			new BuildingFacades.Info("CeilingLight_flower", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.BLOSSOM.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.BLOSSOM.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_flower_kanim"),
			new BuildingFacades.Info("CeilingLight_polka_lamp_shade", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.POLKADOT.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.POLKADOT.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_polka_lamp_shade_kanim"),
			new BuildingFacades.Info("CeilingLight_burt_shower", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.FAUXPIPE.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.FAUXPIPE.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_burt_shower_kanim"),
			new BuildingFacades.Info("CeilingLight_ada_flask_round", BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.LABFLASK.NAME, BUILDINGS.PREFABS.CEILINGLIGHT.FACADES.LABFLASK.DESC, PermitRarity.Common, "CeilingLight", "ceilinglight_ada_flask_round_kanim"),
			new BuildingFacades.Info("FlowerVaseWall_retro_green", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_GREEN.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_GREEN.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_green_kanim"),
			new BuildingFacades.Info("FlowerVaseWall_retro_yellow", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_YELLOW.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_YELLOW.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_yellow_kanim"),
			new BuildingFacades.Info("FlowerVaseWall_retro_red", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_RED.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_RED.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_red_kanim"),
			new BuildingFacades.Info("FlowerVaseWall_retro_blue", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_BLUE.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_BLUE.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_blue_kanim"),
			new BuildingFacades.Info("FlowerVaseWall_retro_white", BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_WHITE.NAME, BUILDINGS.PREFABS.FLOWERVASEWALL.FACADES.RETRO_WHITE.DESC, PermitRarity.Common, "FlowerVaseWall", "flowervase_wall_retro_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_basic_blue_cobalt", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_BLUE_COBALT.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_BLUE_COBALT.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_blue_cobalt_kanim"),
			new BuildingFacades.Info("ExteriorWall_basic_green_kelly", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_GREEN_KELLY.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_GREEN_KELLY.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_green_kelly_kanim"),
			new BuildingFacades.Info("ExteriorWall_basic_grey_charcoal", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_GREY_CHARCOAL.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_GREY_CHARCOAL.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_grey_charcoal_kanim"),
			new BuildingFacades.Info("ExteriorWall_basic_orange_satsuma", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_ORANGE_SATSUMA.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_ORANGE_SATSUMA.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_orange_satsuma_kanim"),
			new BuildingFacades.Info("ExteriorWall_basic_pink_flamingo", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_PINK_FLAMINGO.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_PINK_FLAMINGO.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_pink_flamingo_kanim"),
			new BuildingFacades.Info("ExteriorWall_basic_red_deep", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_RED_DEEP.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_RED_DEEP.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_red_deep_kanim"),
			new BuildingFacades.Info("ExteriorWall_basic_yellow_lemon", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_YELLOW_LEMON.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BASIC_YELLOW_LEMON.DESC, PermitRarity.Common, "ExteriorWall", "walls_basic_yellow_lemon_kanim"),
			new BuildingFacades.Info("ExteriorWall_blueberries", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BLUEBERRIES.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BLUEBERRIES.DESC, PermitRarity.Decent, "ExteriorWall", "walls_blueberries_kanim"),
			new BuildingFacades.Info("ExteriorWall_grapes", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.GRAPES.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.GRAPES.DESC, PermitRarity.Decent, "ExteriorWall", "walls_grapes_kanim"),
			new BuildingFacades.Info("ExteriorWall_lemon", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.LEMON.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.LEMON.DESC, PermitRarity.Decent, "ExteriorWall", "walls_lemon_kanim"),
			new BuildingFacades.Info("ExteriorWall_lime", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.LIME.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.LIME.DESC, PermitRarity.Decent, "ExteriorWall", "walls_lime_kanim"),
			new BuildingFacades.Info("ExteriorWall_satsuma", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SATSUMA.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SATSUMA.DESC, PermitRarity.Decent, "ExteriorWall", "walls_satsuma_kanim"),
			new BuildingFacades.Info("ExteriorWall_strawberry", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRAWBERRY.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.STRAWBERRY.DESC, PermitRarity.Decent, "ExteriorWall", "walls_strawberry_kanim"),
			new BuildingFacades.Info("ExteriorWall_watermelon", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.WATERMELON.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.WATERMELON.DESC, PermitRarity.Decent, "ExteriorWall", "walls_watermelon_kanim"),
			new BuildingFacades.Info("FlowerVaseHanging_retro_red", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_RED.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_RED.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_red_kanim"),
			new BuildingFacades.Info("FlowerVaseHanging_retro_green", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_GREEN.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_GREEN.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_green_kanim"),
			new BuildingFacades.Info("FlowerVaseHanging_retro_blue", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_BLUE.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_BLUE.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_blue_kanim"),
			new BuildingFacades.Info("FlowerVaseHanging_retro_yellow", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_YELLOW.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_YELLOW.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_yellow_kanim"),
			new BuildingFacades.Info("FlowerVaseHanging_retro_white", BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_WHITE.NAME, BUILDINGS.PREFABS.FLOWERVASEHANGING.FACADES.RETRO_WHITE.DESC, PermitRarity.Common, "FlowerVaseHanging", "flowervase_hanging_retro_white_kanim"),
			new BuildingFacades.Info("ExteriorWall_toiletpaper", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.TOILETPAPER.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.TOILETPAPER.DESC, PermitRarity.Decent, "ExteriorWall", "walls_toiletpaper_kanim"),
			new BuildingFacades.Info("ExteriorWall_plunger", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUNGER.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLUNGER.DESC, PermitRarity.Decent, "ExteriorWall", "walls_plunger_kanim"),
			new BuildingFacades.Info("ExteriorWall_tropical", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.TROPICAL.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.TROPICAL.DESC, PermitRarity.Decent, "ExteriorWall", "walls_tropical_kanim")
		};

		public static BuildingFacades.Info[] Infos_All = BuildingFacades.Infos_Skins;

		public struct Info
		{
			public Info(string Id, string Name, string Description, PermitRarity rarity, string PrefabID, string AnimFile)
			{
				this.Id = Id;
				this.Name = Name;
				this.Description = Description;
				this.Rarity = rarity;
				this.PrefabID = PrefabID;
				this.AnimFile = AnimFile;
			}

			public string Id;

			public string Name;

			public string Description;

			public PermitRarity Rarity;

			public string PrefabID;

			public string AnimFile;
		}
	}
}
