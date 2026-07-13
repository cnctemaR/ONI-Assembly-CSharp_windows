using System;
using Database;

public class Blueprints_CosmeticPack1 : BlueprintProvider
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.COSMETIC1;
	}

	public override void SetupBlueprints()
	{
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBelt, PermitRarity.Universal, "permit_atmo_belt_neutronium", "atmo_belt_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitBody, PermitRarity.Universal, "permit_atmosuit_neutronium", "atmosuit_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitShoes, PermitRarity.Universal, "permit_atmo_shoes_neutronium", "atmo_shoes_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitGloves, PermitRarity.Universal, "permit_atmo_gloves_neutronium", "atmo_gloves_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.AtmoSuitHelmet, PermitRarity.Universal, "permit_atmo_helmet_neutronium", "atmo_helmet_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeBottoms, PermitRarity.Universal, "permit_bottom_regal_neutronium", "pants_regal_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeShoes, PermitRarity.Universal, "permit_shoes_regal_neutronium", "shoes_regal_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeGloves, PermitRarity.Universal, "permit_gloves_neutronium", "gloves_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.DupeTops, PermitRarity.Universal, "permit_top_regal_neutronium", "top_regal_neutronium_kanim");
		base.AddBuilding("CrownMoulding", PermitRarity.Universal, "permit_crown_moulding_neutronium", "crown_moulding_neutronium_kanim");
		base.AddBuilding("CornerMoulding", PermitRarity.Universal, "permit_corner_tile_neutronium", "corner_tile_neutronium_kanim");
		base.AddBuilding("ItemPedestal", PermitRarity.Universal, "permit_pedestal_neutronium", "pedestal_neutronium_kanim");
		base.AddBuilding("ManualGenerator", PermitRarity.Universal, "permit_generatormanual_neutronium", "generatormanual_neutronium_kanim");
		base.AddBuilding("Headquarters", PermitRarity.Universal, "permit_hqbase_neutronium", "hqbase_neutronium_kanim");
		base.AddBuilding("ExobaseHeadquarters", PermitRarity.Universal, "permit_porta_pod_y_neutronium", "porta_pod_y_neutronium_kanim");
		base.AddBuilding("RanchStation", PermitRarity.Universal, "permit_rancherstation_neutronium", "rancherstation_neutronium_kanim");
		base.AddBuilding("StorageLocker", PermitRarity.Universal, "permit_storagelocker_neutronium", "storagelocker_neutronium_kanim");
		base.AddBuilding("Refrigerator", PermitRarity.Universal, "permit_fridge_neutronium", "fridge_neutronium_kanim");
		base.AddBuilding("MineralDeoxidizer", PermitRarity.Universal, "permit_mineraldeoxidizer_neutronium", "mineraldeoxidizer_neutronium_kanim");
		base.AddBuilding("LiquidPumpingStation", PermitRarity.Universal, "permit_waterpump_neutronium", "waterpump_neutronium_kanim");
		base.AddBuilding("Generator", PermitRarity.Universal, "permit_generatorphos_neutronium", "generatorphos_neutronium_kanim");
		base.AddBuilding("SteamTurbine2", PermitRarity.Universal, "permit_steamturbine2_neutronium", "steamturbine2_neutronium_kanim");
		base.AddBuilding("CO2Scrubber", PermitRarity.Universal, "permit_co2scrubber_neutronium", "co2scrubber_neutronium_kanim");
		base.AddBuilding("Electrolyzer", PermitRarity.Universal, "permit_electrolyzer_neutronium", "electrolyzer_neutronium_kanim");
		base.AddBuilding("ResetSkillsStation", PermitRarity.Universal, "permit_respeccer_neutronium", "respeccer_neutronium_kanim");
		base.AddBuilding("WaterPurifier", PermitRarity.Universal, "permit_waterpurifier_neutronium", "waterpurifier_neutronium_kanim");
		base.AddBuilding("MetalRefinery", PermitRarity.Universal, "permit_metalrefinery_neutronium", "metalrefinery_neutronium_kanim");
		base.AddBuilding("LiquidHeater", PermitRarity.Universal, "permit_boiler_neutronium", "boiler_neutronium_kanim");
		base.AddBuilding("LiquidConditioner", PermitRarity.Universal, "permit_liquidconditioner_neutronium", "liquidconditioner_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.JetSuitBody, PermitRarity.Universal, "permit_jetsuit_neutronium", "jetsuit_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.JetSuitShoes, PermitRarity.Universal, "permit_jet_shoes_neutronium", "jet_shoes_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.JetSuitGloves, PermitRarity.Universal, "permit_jet_gloves_neutronium", "jet_gloves_neutronium_kanim");
		base.AddClothing(BlueprintProvider.ClothingType.JetSuitHelmet, PermitRarity.Universal, "permit_jet_helmet_neutronium", "jet_helmet_neutronium_kanim");
		base.AddBuilding("ExteriorWall", PermitRarity.Universal, "permit_walls_stripes_neutronium", "walls_stripes_neutronium_kanim");
		base.AddOutfit(BlueprintProvider.OutfitType.Clothing, "permit_standard_regal_neutronium_outfit", new string[] { "permit_bottom_regal_neutronium", "permit_shoes_regal_neutronium", "permit_gloves_neutronium", "permit_top_regal_neutronium" });
		base.AddOutfit(BlueprintProvider.OutfitType.AtmoSuit, "neutronium_atmo_outfit", new string[] { "permit_atmo_helmet_neutronium", "permit_atmosuit_neutronium", "permit_atmo_gloves_neutronium", "permit_atmo_shoes_neutronium", "permit_atmo_belt_neutronium" });
		base.AddOutfit(BlueprintProvider.OutfitType.JetSuit, "neutronium_jetsuit_outfit", new string[] { "permit_jet_helmet_neutronium", "permit_jetsuit_neutronium", "permit_jet_gloves_neutronium", "permit_jet_shoes_neutronium" });
	}
}
