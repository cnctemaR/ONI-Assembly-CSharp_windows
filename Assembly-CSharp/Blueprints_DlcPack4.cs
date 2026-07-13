using System;
using Database;

public class Blueprints_DlcPack4 : BlueprintProvider
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC4;
	}

	public override void SetupBlueprints()
	{
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_raptor", "atmo_belt_raptor_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_stego", "atmo_belt_stego_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_chameleo", "atmo_belt_chameleo_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_paculacanth", "atmo_belt_paculacanth_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_raptor", "atmosuit_raptor_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_stego", "atmosuit_stego_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_chameleo", "atmosuit_chameleo_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_paculacanth", "atmosuit_paculacanth_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_raptor", "atmo_shoes_raptor_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_stego", "atmo_shoes_stego_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_chameleo", "atmo_shoes_chameleo_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_paculacanth", "atmo_shoes_paculacanth_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_raptor", "atmo_gloves_raptor_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_stego", "atmo_gloves_stego_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_chameleo", "atmo_gloves_chameleo_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_paculacanth", "atmo_gloves_paculacanth_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_raptor", "atmo_helmet_raptor_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_stego", "atmo_helmet_stego_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_chameleo", "atmo_helmet_chameleo_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_paculacanth", "atmo_helmet_paculacanth_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_jumpsuit_romper_tan_frass", "jumpsuit_romper_tan_frass_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_pj_dino", "pj_dino_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_pj_dino2", "pj_dino2_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_pj_dino3", "pj_dino3_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeBottoms, PermitRarity.Universal, "permit_pants_suspenders_frass", "pants_suspenders_frass_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeBottoms, PermitRarity.Universal, "permit_pants_wader_algae", "pants_wader_algae_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeBottoms, PermitRarity.Universal, "permit_shorts_scout_brown", "shorts_scout_brown_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_basic_frass", "shoes_basic_frass_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_scout_brown", "shoes_scout_brown_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_romper_frass_tan", "shoes_romper_frass_tan_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_basic_brown", "gloves_basic_brown_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_basic_grime", "gloves_basic_grime_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_cuffless_shiny_algae", "gloves_cuffless_shiny_algae_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_sweater_ribbed_rust", "top_sweater_ribbed_rust_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_sweater_wader_ltblue", "top_sweater_wader_ltblue_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_scout_white", "top_scout_white_kanim");
		base.AddBuilding("Bed", PermitRarity.Universal, "permit_bed_rock", "bed_rock_kanim");
		base.AddBuilding("ManualGenerator", PermitRarity.Universal, "permit_generatormanual_rock", "generatormanual_rock_kanim");
		base.AddBuilding("RanchStation", PermitRarity.Universal, "permit_rancherstation_dino", "rancherstation_dino_kanim");
		base.AddBuilding("GasReservoir", PermitRarity.Universal, "permit_gasstorage_dartle", "gasstorage_dartle_kanim");
		base.AddBuilding("GasReservoir", PermitRarity.Universal, "permit_gasstorage_lumb", "gasstorage_lumb_kanim");
		base.AddBuilding("MilkPress", PermitRarity.Universal, "permit_milkpress_stego", "milkpress_stego_kanim");
		base.AddJoyResponse(BlueprintProvider.JoyResponseType.BallonSet, PermitRarity.Universal, "permit_balloon_babystego_egg", "balloon_babystego_egg_kanim");
		base.AddJoyResponse(BlueprintProvider.JoyResponseType.BallonSet, PermitRarity.Universal, "permit_balloon_babyrhex_egg", "balloon_babyrhex_egg_kanim");
		base.AddMonumentPart(BlueprintProvider.MonumentPart.Top, PermitRarity.Universal, "permit_monument_upper_a_prehistoric", "monument_upper_a_prehistoric_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.Painting, PermitRarity.Universal, "permit_painting_art_stego", "painting_art_stego_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.PaintingWide, PermitRarity.Universal, "permit_painting_wide_art_rhex", "painting_wide_art_rhex_kanim");
		base.AddBuilding("MassageTable", PermitRarity.Universal, "permit_masseur_prehistoric", "masseur_prehistoric_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_chameleo", "walls_chameleo_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_paculacanth", "walls_paculacanth_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_raptor", "walls_raptor_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_stego", "walls_stego_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_fossil_chameleo", "walls_fossil_chameleo_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_fossil_paculacanth", "walls_fossil_paculacanth_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_fossil_raptor", "walls_fossil_raptor_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_fossil_stego", "walls_fossil_stego_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_silhouette_prehistoriccritters", "walls_silhouette_prehistoriccritters_kanim");
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_romper_tan", new string[] { "permit_jumpsuit_romper_tan_frass", "permit_shoes_romper_frass_tan", "permit_gloves_basic_brown" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_scout", new string[] { "permit_shorts_scout_brown", "permit_shoes_scout_brown", "permit_gloves_basic_grime", "permit_top_scout_white" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_wader", new string[] { "permit_pants_wader_algae", "permit_gloves_cuffless_shiny_algae", "permit_top_sweater_wader_ltblue" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_suspenders", new string[] { "permit_pants_suspenders_frass", "permit_gloves_basic_brown", "permit_top_sweater_ribbed_rust", "permit_shoes_basic_frass" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_pj_dino", new string[] { "permit_pj_dino" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_pj_dino2", new string[] { "permit_pj_dino2" });
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "outfit_pj_dino3", new string[] { "permit_pj_dino3" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "outfit_atmosuit_raptor", new string[] { "permit_atmo_helmet_raptor", "permit_atmo_belt_raptor", "permit_atmo_gloves_raptor", "permit_atmo_shoes_raptor", "permit_atmosuit_raptor" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "outfit_atmosuit_stego", new string[] { "permit_atmo_helmet_stego", "permit_atmo_belt_stego", "permit_atmo_gloves_stego", "permit_atmo_shoes_stego", "permit_atmosuit_stego" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "outfit_atmosuit_chameleo", new string[] { "permit_atmo_helmet_chameleo", "permit_atmo_belt_chameleo", "permit_atmo_gloves_chameleo", "permit_atmo_shoes_chameleo", "permit_atmosuit_chameleo" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "outfit_atmosuit_paculacanth", new string[] { "permit_atmo_helmet_paculacanth", "permit_atmo_belt_paculacanth", "permit_atmo_gloves_paculacanth", "permit_atmo_shoes_paculacanth", "permit_atmosuit_paculacanth" });
		base.AddBuilding("Headquarters", PermitRarity.Universal, "permit_hqbase_dino", "hqbase_dino_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.FossilSculpture, PermitRarity.Universal, "permit_fossilsculpture_idle_rhex", "fossilsculpture_idle_rhex_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.FossilSculpture, PermitRarity.Universal, "permit_fossilsculpture_idle_stego", "fossilsculpture_idle_stego_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.CeilingFossilSculpture, PermitRarity.Universal, "permit_fossilsculpture_idle_jawbo", "fossilsculpture_idle_jawbo_kanim");
		base.AddArtable(BlueprintProvider.ArtableType.CeilingFossilSculpture, PermitRarity.Universal, "permit_fossilsculpture_idle_shellonoidis", "fossilsculpture_idle_shellonoidis_kanim");
	}
}
