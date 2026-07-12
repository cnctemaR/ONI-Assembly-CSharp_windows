using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;

namespace Database
{
	public class PlantMutations : ResourceSet<PlantMutation>
	{
		public PlantMutation AddPlantMutation(string id)
		{
			StringEntry stringEntry = Strings.Get(new StringKey("STRINGS.CREATURES.PLANT_MUTATIONS." + id.ToUpper() + ".NAME"));
			StringEntry stringEntry2 = Strings.Get(new StringKey("STRINGS.CREATURES.PLANT_MUTATIONS." + id.ToUpper() + ".DESCRIPTION"));
			PlantMutation plantMutation = new PlantMutation(id, stringEntry, stringEntry2);
			base.Add(plantMutation);
			return plantMutation;
		}

		public PlantMutations(ResourceSet parent)
			: base("PlantMutations", parent)
		{
			this.moderatelyLoose = this.AddPlantMutation("moderatelyLoose").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().PlantAttributes.WiltTempRangeMod, 0.5f, true)
				.AttributeModifier(Db.Get().PlantAttributes.YieldAmount, -0.25f, true)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, -0.5f, true)
				.VisualTint(-0.4f, -0.4f, -0.4f);
			this.moderatelyTight = this.AddPlantMutation("moderatelyTight").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().PlantAttributes.WiltTempRangeMod, -0.5f, true)
				.AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 0.5f, true)
				.VisualTint(0.2f, 0.2f, 0.2f);
			this.extremelyTight = this.AddPlantMutation("extremelyTight").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().PlantAttributes.WiltTempRangeMod, -0.8f, true)
				.AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 1f, true)
				.VisualTint(0.3f, 0.3f, 0.3f)
				.VisualBGFX("mutate_glow_fx_kanim");
			this.bonusLice = this.AddPlantMutation("bonusLice").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, true)
				.BonusCrop("BasicPlantFood", 1f)
				.VisualSymbolOverride("snapTo_mutate1", "mutate_snaps_kanim", "meal_lice_mutate1")
				.VisualSymbolOverride("snapTo_mutate2", "mutate_snaps_kanim", "meal_lice_mutate2")
				.AddSoundEvent(GlobalAssets.GetSound("Plant_mutation_MealLice", false));
			this.sunnySpeed = this.AddPlantMutation("sunnySpeed").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().PlantAttributes.MinLightLux, 1000f, false)
				.AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, -0.5f, true)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, true)
				.VisualSymbolOverride("snapTo_mutate1", "mutate_snaps_kanim", "leaf_mutate1")
				.VisualSymbolOverride("snapTo_mutate2", "mutate_snaps_kanim", "leaf_mutate2")
				.AddSoundEvent(GlobalAssets.GetSound("Plant_mutation_Leaf", false));
			this.slowBurn = this.AddPlantMutation("slowBurn").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, -0.9f, true)
				.AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, 3.5f, true)
				.VisualTint(-0.3f, -0.3f, -0.5f);
			this.blooms = this.AddPlantMutation("blooms").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().BuildingAttributes.Decor, 20f, false)
				.VisualSymbolOverride("snapTo_mutate1", "mutate_snaps_kanim", "blossom_mutate1")
				.VisualSymbolOverride("snapTo_mutate2", "mutate_snaps_kanim", "blossom_mutate2")
				.AddSoundEvent(GlobalAssets.GetSound("Plant_mutation_PrickleFlower", false));
			this.loadedWithFruit = this.AddPlantMutation("loadedWithFruit").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 1f, true)
				.AttributeModifier(Db.Get().PlantAttributes.HarvestTime, 4f, true)
				.AttributeModifier(Db.Get().PlantAttributes.MinLightLux, 200f, false)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.2f, true)
				.VisualSymbolScale("swap_crop01", 1.3f)
				.VisualSymbolScale("swap_crop02", 1.3f);
			this.rottenHeaps = this.AddPlantMutation("rottenHeaps").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, -0.75f, true)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.5f, true)
				.BonusCrop(RotPileConfig.ID, 4f)
				.AddDiseaseToHarvest(Db.Get().Diseases.GetIndex(Db.Get().Diseases.FoodGerms.Id), 10000)
				.ForcePrefersDarkness()
				.VisualFGFX("mutate_stink_fx_kanim")
				.VisualSymbolTint("swap_crop01", -0.2f, -0.1f, -0.5f)
				.VisualSymbolTint("swap_crop02", -0.2f, -0.1f, -0.5f);
			this.heavyFruit = this.AddPlantMutation("heavyFruit").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 250f, false).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, true)
				.ForceSelfHarvestOnGrown()
				.VisualSymbolTint("swap_crop01", -0.1f, -0.5f, -0.5f)
				.VisualSymbolTint("swap_crop02", -0.1f, -0.5f, -0.5f);
		}

		public List<string> GetNamesForMutations(List<string> mutationIDs)
		{
			List<string> list = new List<string>(mutationIDs.Count);
			foreach (string text in mutationIDs)
			{
				list.Add(base.Get(text).Name);
			}
			return list;
		}

		public PlantMutation GetRandomMutation(string targetPlantPrefabID)
		{
			return this.resources.Where<PlantMutation>((PlantMutation m) => !m.originalMutation && !m.restrictedPrefabIDs.Contains(targetPlantPrefabID) && (m.requiredPrefabIDs.Count == 0 || m.requiredPrefabIDs.Contains(targetPlantPrefabID))).ToList<PlantMutation>().GetRandom<PlantMutation>();
		}

		public PlantMutation moderatelyLoose;

		public PlantMutation moderatelyTight;

		public PlantMutation extremelyTight;

		public PlantMutation bonusLice;

		public PlantMutation sunnySpeed;

		public PlantMutation slowBurn;

		public PlantMutation blooms;

		public PlantMutation loadedWithFruit;

		public PlantMutation heavyFruit;

		public PlantMutation rottenHeaps;
	}
}
