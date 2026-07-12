using System;
using Database;

public class Blueprints_DlcPack2 : BlueprintProvider
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public override void SetupBlueprints()
	{
		base.AddBuilding("Headquarters", PermitRarity.Universal, "permit_headquarters_ceres", "hqbase_ice_kanim");
		base.AddBuilding("Bed", PermitRarity.Universal, "permit_bed_jorge", "bed_jorge_kanim");
		base.AddBuilding("StorageLockerSmart", PermitRarity.Universal, "permit_smartstoragelocker_gravitas", "smartstoragelocker_gravitas_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureIce, PermitRarity.Universal, "permit_icesculpture_amazing_idle_bammoth", "icesculpture_idle_bammoth_kanim").Quality(BlueprintProvider.ArtableQuality.LookingOkay);
		base.AddArtable(BlueprintProvider.ArtableType.SculptureIce, PermitRarity.Universal, "permit_icesculpture_amazing_idle_wood_deer", "icesculpture_idle_floof_kanim").Quality(BlueprintProvider.ArtableQuality.LookingOkay);
		base.AddArtable(BlueprintProvider.ArtableType.SculptureIce, PermitRarity.Universal, "permit_icesculpture_amazing_idle_seal", "icesculpture_idle_seal_kanim").Quality(BlueprintProvider.ArtableQuality.LookingOkay);
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_crap_low_one", "sculpture_wood_low_one_kanim").Quality(BlueprintProvider.ArtableQuality.LookingUgly);
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_okay_mid_one", "sculpture_wood_mid_one_kanim").Quality(BlueprintProvider.ArtableQuality.LookingOkay);
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_amazing_action_wood_deer", "sculpture_wood_action_floof_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_amazing_action_gulp", "sculpture_wood_action_gulp_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_amazing_action_pacu", "sculpture_wood_action_pacu_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_amazing_action_puft", "sculpture_wood_action_puft_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_amazing_rear_cuddlepip", "sculpture_wood_rear_cuddlepip_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_amazing_rear_drecko", "sculpture_wood_rear_drecko_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_amazing_rear_puft", "sculpture_wood_rear_puft_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.SculptureWood, PermitRarity.Universal, "permit_sculpture_wood_amazing_rear_shovevole", "sculpture_wood_rear_shovevole_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_red", "top_flannel_red_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_orange", "top_flannel_orange_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_yellow", "top_flannel_yellow_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_green", "top_flannel_green_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_blue_middle", "top_flannel_blue_middle_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_purple", "top_flannel_purple_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_pink_orchid", "top_flannel_pink_orchid_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_white", "top_flannel_white_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_flannel_black", "top_flannel_black_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_80s", "atmo_belt_80s_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_80s", "atmosuit_80s_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_80s", "atmo_shoes_80s_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_80s", "atmo_gloves_80s_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_80s", "atmo_helmet_80s_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_01", "gloves_hockey_01_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_02", "gloves_hockey_02_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_03", "gloves_hockey_03_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_04", "gloves_hockey_04_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_05", "gloves_hockey_05_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_07", "gloves_hockey_07_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_08", "gloves_hockey_08_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_09", "gloves_hockey_09_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_10", "gloves_hockey_10_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_11", "gloves_hockey_11_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_hockey_12", "gloves_hockey_12_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_mittens_knit_black_smog", "mittens_knit_black_smog_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_mittens_knit_white", "mittens_knit_white_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_mittens_knit_yellowcake", "mittens_knit_yellowcake_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_mittens_knit_orange_tectonic", "mittens_knit_orange_tectonic_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_mittens_knit_green_enzyme", "mittens_knit_green_enzyme_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_mittens_knit_blue_azulene", "mittens_knit_blue_azulene_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_mittens_knit_purple_astral", "mittens_knit_purple_astral_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_mittens_knit_pink_cosmic", "mittens_knit_pink_cosmic_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_01", "top_jersey_01_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_02", "top_jersey_02_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_03", "top_jersey_03_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_04", "top_jersey_04_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_05", "top_jersey_05_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_07", "top_jersey_07_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_08", "top_jersey_08_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_09", "top_jersey_09_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_10", "top_jersey_10_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_11", "top_jersey_11_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_jersey_12", "top_jersey_12_kanim");
		base.AddBuilding("Bed", PermitRarity.Universal, "permit_bed_cottage", "bed_cottage_kanim");
		base.AddBuilding("FloorLamp", PermitRarity.Universal, "permit_floorlamp_cottage", "floorlamp_cottage_kanim");
		base.AddBuilding("CookingStation", PermitRarity.Universal, "permit_cookingstation_cottage", "cookstation_cottage_kanim");
		base.AddBuilding("GourmetCookingStation", PermitRarity.Universal, "permit_cookingstation_gourmet_cottage", "cookstation_gourmet_cottage_kanim");
		base.AddBuilding("RanchStation", PermitRarity.Universal, "permit_rancherstation_cottage", "rancherstation_cottage_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_wood_panel", "walls_wood_panel_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_igloo", "walls_igloo_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_forest", "walls_forest_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_southwest", "walls_southwest_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.Painting, PermitRarity.Universal, "permit_painting_art_ceres_a", "painting_art_ceres_a_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.PaintingWide, PermitRarity.Universal, "permit_painting_wide_art_ceres_a", "painting_wide_art_ceres_a_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.PaintingTall, PermitRarity.Universal, "permit_painting_tall_art_ceres_a", "painting_tall_art_ceres_a_kanim");
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_red", new string[] { "permit_top_flannel_red" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_orange", new string[] { "permit_top_flannel_orange" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_yellow", new string[] { "permit_top_flannel_yellow" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_green", new string[] { "permit_top_flannel_green" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_blue_middle", new string[] { "permit_top_flannel_blue_middle" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_purple", new string[] { "permit_top_flannel_purple" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_pink_orchid", new string[] { "permit_top_flannel_pink_orchid" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_white", new string[] { "permit_top_flannel_white" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_flannel_black", new string[] { "permit_top_flannel_black" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_01", new string[] { "permit_gloves_hockey_01", "permit_top_jersey_01" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_02", new string[] { "permit_gloves_hockey_02", "permit_top_jersey_02" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_03", new string[] { "permit_gloves_hockey_03", "permit_top_jersey_03" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_04", new string[] { "permit_gloves_hockey_04", "permit_top_jersey_04" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_05", new string[] { "permit_gloves_hockey_05", "permit_top_jersey_05" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_07", new string[] { "permit_gloves_hockey_07", "permit_top_jersey_07" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_08", new string[] { "permit_gloves_hockey_08", "permit_top_jersey_08" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_09", new string[] { "permit_gloves_hockey_09", "permit_top_jersey_09" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_10", new string[] { "permit_gloves_hockey_10", "permit_top_jersey_10" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_11", new string[] { "permit_gloves_hockey_11", "permit_top_jersey_11" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_top_hockey_12", new string[] { "permit_gloves_hockey_12", "permit_top_jersey_12" });
	}
}
