using System;
using System.Collections.Generic;
using Database;

public class Blueprints_DlcPack5 : BlueprintProvider
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public override void SetupBlueprints()
	{
		base.AddBuilding("Headquarters", PermitRarity.Universal, "permit_hqbase_aquatic", "hqbase_aquatic_kanim");
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "permit_standard_swim_outfit_red", new string[] { "permit_top_swim_red", "permit_pants_swim_black", "permit_gloves_swim_gold", "permit_shoes_swim_red" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "permit_standard_swim_outfit_blue", new string[] { "permit_top_swim_blue", "permit_pants_swim_black", "permit_gloves_swim_gold", "permit_shoes_swim_blue" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "permit_standard_swim_outfit_green", new string[] { "permit_top_swim_green", "permit_pants_swim_black", "permit_gloves_swim_gold", "permit_shoes_swim_green" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "permit_standard_swim_outfit_yellow", new string[] { "permit_top_swim_yellow", "permit_pants_swim_black", "permit_gloves_swim_gold", "permit_shoes_swim_yellow" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "permit_minnow_swim_outfit", new string[] { "permit_top_swim_axon", "permit_pants_swim_black", "permit_gloves_swim_wheezy", "permit_shoes_swim_wheezy" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_nautical_dress", new string[] { "permit_dress_sailor_white_nautical", "permit_gloves_basic_blue_nautical", "permit_shoes_boots_sailor_nautical" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_nautical_suit", new string[] { "permit_top_nautical_blue_white", "permit_pants_nautical_white", "permit_shoes_basic_blue_navy", "permit_gloves_basic_navy" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_swimsuit_merm", new string[] { "permit_swimsuit_merm", "permit_shoes_swim_merm", "permit_gloves_swim_merm" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_swimsuit_wavecrests", new string[] { "permit_swimsuit_wavecrests", "permit_shoes_swim_wavecrests", "permit_gloves_swim_wavecrests" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "outfit_atmosuit_antique", new string[] { "permit_atmo_helmet_diving_khaki_tan", "permit_atmo_belt_diving_tan", "permit_atmo_gloves_diving_khaki_tan", "permit_atmo_shoes_diving_tan", "permit_atmosuit_diving_khaki_tan" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "outfit_atmosuit_ducky", new string[] { "permit_atmo_helmet_tap_electro_rubble", "permit_atmo_belt_duckie_gasball", "permit_atmo_gloves_electro", "permit_atmo_shoes_basic_electro", "permit_atmosuit_basic_rubble_electro" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "outfit_atmosuit_floatie", new string[] { "permit_atmo_helmet_tap_01", "permit_atmo_belt_floatie", "permit_atmo_gloves_tap_01", "permit_atmo_shoes_tap_01", "permit_atmosuit_tap_01" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "outfit_atmosuit_orehull", new string[] { "permit_atmo_helmet_turtle", "permit_atmo_belt_turtle", "permit_atmo_gloves_turtle", "permit_atmo_shoes_turtle", "permit_atmosuit_turtle" });
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_diving_tan", "atmo_belt_diving_tan_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_floatie", "atmo_belt_floatie_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_duckie_gasball", "atmo_belt_duckie_gasball_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_turtle", "atmo_belt_turtle_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_diving_khaki_tan", "atmosuit_diving_khaki_tan_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_basic_rubble_electro", "atmosuit_basic_rubble_electro_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_tap_01", "atmosuit_tap_01_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_turtle", "atmosuit_turtle_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_diving_tan", "atmo_shoes_diving_tan_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_basic_electro", "atmo_shoes_basic_electro_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_tap_01", "atmo_shoes_tap_01_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_turtle", "atmo_shoes_turtle_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_diving_khaki_tan", "atmo_gloves_diving_khaki_tan_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_electro", "atmo_gloves_electro_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_tap_01", "atmo_gloves_tap_01_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_turtle", "atmo_gloves_turtle_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_diving_khaki_tan", "atmo_helmet_diving_khaki_tan_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_tap_electro_rubble", "atmo_helmet_tap_electro_rubble_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_tap_01", "atmo_helmet_tap_01_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_turtle", "atmo_helmet_turtle_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_swimsuit_merm", "swimsuit_merm_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_swimsuit_wavecrests", "swimsuit_wavecrests_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeBottoms, PermitRarity.Universal, "permit_pants_swim_black", "pants_swim_black_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeBottoms, PermitRarity.Universal, "permit_pants_nautical_white", "pants_nautical_white_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_swim_red", "shoes_swim_red_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_basic_blue_nautical", "shoes_basic_blue_nautical_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_basic_blue_navy", "shoes_basic_blue_navy_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_boots_sailor_nautical", "shoes_boots_sailor_nautical_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_swim_green", "shoes_swim_green_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_swim_yellow", "shoes_swim_yellow_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_swim_blue", "shoes_swim_blue_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_swim_wheezy", "shoes_swim_wheezy_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_swim_merm", "shoes_swim_merm_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_swim_wavecrests", "shoes_swim_wavecrests_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_swim_gold", "gloves_swim_gold_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_basic_blue_nautical", "gloves_basic_blue_nautical_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_basic_navy", "gloves_basic_navy_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_swim_wheezy", "gloves_swim_wheezy_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_swim_merm", "gloves_swim_merm_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_swim_wavecrests", "gloves_swim_wavecrests_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_nautical_blue_white", "top_nautical_blue_white_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_swim_red", "top_swim_red_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_swim_green", "top_swim_green_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_swim_blue", "top_swim_blue_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_swim_yellow", "top_swim_yellow_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_swim_axon", "top_swim_axon_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_dress_sailor_white_nautical", "dress_sailor_white_nautical_kanim");
		base.AddBuilding("BeachChair", PermitRarity.Universal, "permit_beach_chair_beakon", "beach_chair_beakon_kanim");
		base.AddBuilding("BeachChair", PermitRarity.Universal, "permit_beach_chair_pokeshell", "beach_chair_pokeshell_kanim");
		base.AddBuilding("BeachChair", PermitRarity.Universal, "permit_beach_chair_kraken", "beach_chair_kraken_kanim");
		base.AddBuildingWithData("GlassCeilingLight", PermitRarity.Universal, "permit_glassceilinglight_jelly_pink", "glassceilinglight_jelly_pink_kanim", new Dictionary<string, string>
		{
			{ "LightColor", "FF4FA1" },
			{ "LightOverlayColor", "FF61CC" }
		});
		base.AddBuildingWithData("GlassCeilingLight", PermitRarity.Universal, "permit_glassceilinglight_jelly_blue", "glassceilinglight_jelly_blue_kanim", new Dictionary<string, string>
		{
			{ "LightColor", "00C0FF" },
			{ "LightOverlayColor", "0092F5" }
		});
		base.AddBuilding("CornerMoulding", PermitRarity.Universal, "permit_corner_tile_retro_nautical", "corner_tile_retro_nautical_kanim");
		base.AddBuilding("CrownMoulding", PermitRarity.Universal, "permit_crown_moulding_retro_nautical", "crown_moulding_retro_nautical_kanim");
		base.AddBuilding("ManualGenerator", PermitRarity.Universal, "permit_generatormanual_nautical", "generatormanual_nautical_kanim");
		base.AddBuilding("ManualGenerator", PermitRarity.Universal, "permit_generatormanual_waterbubbles", "generatormanual_waterbubbles_kanim");
		base.AddBuilding("ExobaseHeadquarters", PermitRarity.Universal, "permit_porta_pod_y_aquatic", "porta_pod_y_aquatic_kanim");
		base.AddBuilding("UnderwaterCritterCondo", PermitRarity.Universal, "permit_underwater_critter_condo_aquatic", "underwater_critter_condo_aquatic_kanim");
		base.AddBuilding("StorageLocker", PermitRarity.Universal, "permit_storagelocker_stripes_nautical", "storagelocker_stripes_nautical_kanim");
		base.AddBuilding("StorageLocker", PermitRarity.Universal, "permit_storagelocker_waterbubbles", "storagelocker_waterbubbles_kanim");
		base.AddBuilding("GasReservoir", PermitRarity.Universal, "permit_gasstorage_stripes_nautical", "gasstorage_stripes_nautical_kanim");
		base.AddBuilding("GasReservoir", PermitRarity.Universal, "permit_gasstorage_waterbubbles", "gasstorage_waterbubbles_kanim");
		base.AddBuilding("Refrigerator", PermitRarity.Universal, "permit_fridge_stripes_nautical", "fridge_stripes_nautical_kanim");
		base.AddBuilding("Refrigerator", PermitRarity.Universal, "permit_fridge_waterbubbles", "fridge_waterbubbles_kanim");
		base.AddJoyResponse(BlueprintProvider.JoyResponseType.BallonSet, PermitRarity.Universal, "permit_balloon_blowfish_egg", "balloon_blowfish_egg_kanim");
		base.AddMonumentPart(BlueprintProvider.MonumentPart.Bottom, PermitRarity.Universal, "permit_monument_base_a_aquatic", "monument_base_a_aquatic_kanim");
		base.AddMonumentPart(BlueprintProvider.MonumentPart.Bottom, PermitRarity.Universal, "permit_monument_base_b_aquatic", "monument_base_b_aquatic_kanim");
		base.AddMonumentPart(BlueprintProvider.MonumentPart.Middle, PermitRarity.Universal, "permit_monument_mid_a_aquatic", "monument_mid_a_aquatic_kanim");
		base.AddMonumentPart(BlueprintProvider.MonumentPart.Middle, PermitRarity.Universal, "permit_monument_mid_b_aquatic", "monument_mid_b_aquatic_kanim");
		base.AddMonumentPart(BlueprintProvider.MonumentPart.Top, PermitRarity.Universal, "permit_monument_upper_a_aquatic", "monument_upper_a_aquatic_kanim");
		base.AddMonumentPart(BlueprintProvider.MonumentPart.Top, PermitRarity.Universal, "permit_monument_upper_b_aquatic", "monument_upper_b_aquatic_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.Painting, PermitRarity.Universal, "permit_painting_art_aquatic", "painting_art_aquatic_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.PaintingWide, PermitRarity.Universal, "permit_painting_wide_art_venus", "painting_wide_art_venus_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.PaintingTall, PermitRarity.Universal, "permit_painting_tall_art_aquatic", "painting_tall_art_aquatic_kanim");
		base.AddBuilding("LuxuryBed", PermitRarity.Universal, "permit_elegantbed_clam", "elegantbed_clam_kanim");
		base.AddBuilding("LuxuryBed", PermitRarity.Universal, "permit_elegantbed_submarine", "elegantbed_submarine_kanim");
		base.AddBuilding("MechanicalSurfboard", PermitRarity.Universal, "permit_mechanical_surfboard_jaws", "mechanical_surfboard_jaws_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureMarble, PermitRarity.Universal, "permit_sculpture_marble_swirling_pacus", "sculpture_marble_swirling_pacus_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureMarble, PermitRarity.Universal, "permit_sculpture_marble_sequine_squad", "sculpture_marble_sequine_squad_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureMarble, PermitRarity.Universal, "permit_sculpture_marble_atlas_haul", "sculpture_marble_atlas_haul_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_basic_blue_nautical", "walls_basic_blue_nautical_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_diagonal_blue_nautical_white", "walls_diagonal_blue_nautical_white_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_circle_blue_nautical_white", "walls_circle_blue_nautical_white_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_circle_white_blue_nautical", "walls_circle_white_blue_nautical_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_stripes_diagonal_white_nautical", "walls_stripes_diagonal_white_nautical_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_stripes_white_nautical", "walls_stripes_white_nautical_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_stripes_circle_nautical_white", "walls_stripes_circle_nautical_white_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_stripes_circle_white_nautical", "walls_stripes_circle_white_nautical_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_squares_blue_nautical_white", "walls_squares_blue_nautical_white_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_plus_blue_nautical_white", "walls_plus_blue_nautical_white_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_square_blue_nautical_white", "walls_square_blue_nautical_white_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_triangle_white_nautical", "walls_triangle_white_nautical_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_triangle_nautical_white", "walls_triangle_nautical_white_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_x_nautical_white", "walls_x_nautical_white_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_waves", "walls_waves_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_waves_half", "walls_waves_half_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_waves01", "walls_waves01_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_waves02", "walls_waves02_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_wavescrests", "walls_wavescrests_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_wavescrests_half", "walls_wavescrests_half_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_wavescrashing", "walls_wavescrashing_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_sea", "walls_sea_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_sea_anchor", "walls_sea_anchor_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_sea_wheel", "walls_sea_wheel_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_sea_lifepreserver", "walls_sea_lifepreserver_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_sea_compass", "walls_sea_compass_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_wavesswirls", "walls_wavesswirls_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_blank", "walls_aqua_blank_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_bubbles", "walls_aqua_bubbles_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_jellyfish", "walls_aqua_jellyfish_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_silt_top", "walls_aqua_silt_top_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_silt", "walls_aqua_silt_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_kelp_top", "walls_aqua_kelp_top_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_kelp", "walls_aqua_kelp_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_kelp_silt", "walls_aqua_kelp_silt_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_beaker", "walls_aqua_beaker_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_pacu", "walls_aqua_pacu_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_puffer", "walls_aqua_puffer_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_aqua_turtle", "walls_aqua_turtle_kanim");
	}
}
