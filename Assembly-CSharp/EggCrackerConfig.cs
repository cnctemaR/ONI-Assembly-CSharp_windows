using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class EggCrackerConfig : IBuildingConfig
{
	public static void RegisterEgg(Tag eggPrefabTag, string name, string description, float mass, string[] requiredDLC, string[] forbiddenDLC)
	{
		EggCrackerConfig.RegisterEgg(eggPrefabTag, name, description, mass, requiredDLC, forbiddenDLC, null);
	}

	public static void RegisterEgg(Tag eggPrefabTag, string name, string description, float mass, string[] requiredDLC, string[] forbiddenDLC, global::Tuple<Tag, float>[] customDrops)
	{
		EggCrackerConfig.RegisterEgg(eggPrefabTag, name, description, mass, requiredDLC, forbiddenDLC, customDrops, true);
	}

	public static void RegisterEgg(Tag eggPrefabTag, string name, string description, float mass, string[] requiredDLC, string[] forbiddenDLC, global::Tuple<Tag, float>[] customDrops, bool allowCrackerRecipeCreation = true)
	{
		EggCrackerConfig.EggData eggData = new EggCrackerConfig.EggData(eggPrefabTag, name, description, mass, requiredDLC, forbiddenDLC, allowCrackerRecipeCreation);
		eggData.customOutput = customDrops;
		EggCrackerConfig.uncategorizedEggData.Add(eggData);
	}

	public static void CategorizeEggs()
	{
		foreach (EggCrackerConfig.EggData eggData in EggCrackerConfig.uncategorizedEggData)
		{
			Tag spawnedCreature = Assets.GetPrefab(eggData.id).GetDef<IncubationMonitor.Def>().spawnedCreature;
			Tag species = Assets.GetPrefab(spawnedCreature).GetComponent<CreatureBrain>().species;
			eggData.isBaseMorph = Assets.GetPrefab(spawnedCreature).HasTag(GameTags.OriginalCreature);
			if (!EggCrackerConfig.EggsBySpecies.ContainsKey(species))
			{
				EggCrackerConfig.EggsBySpecies.Add(species, new List<EggCrackerConfig.EggData>());
			}
			EggCrackerConfig.EggsBySpecies[species].Add(eggData);
		}
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "EggCracker";
		int num = 2;
		int num2 = 2;
		string text2 = "egg_cracker_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, none, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.AddSearchTerms(SEARCH_TERMS.FOOD);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_egg", false);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.labelByResult = false;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		workable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_egg_cracker_kanim") };
		complexFabricator.outputOffset = new Vector3(1f, 1f, 0f);
		Prioritizable.AddRef(go);
		go.AddOrGet<EggCracker>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
	}

	public override void ConfigurePost(BuildingDef def)
	{
		base.ConfigurePost(def);
		this.MakeRecipes();
	}

	public void MakeRecipes()
	{
		EggCrackerConfig.CategorizeEggs();
		foreach (KeyValuePair<Tag, List<EggCrackerConfig.EggData>> keyValuePair in EggCrackerConfig.EggsBySpecies)
		{
			Tag[] array = new Tag[keyValuePair.Value.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = keyValuePair.Value[i].id;
			}
			EggCrackerConfig.EggData eggData = keyValuePair.Value[0];
			if (eggData.hasCrackerRecipe)
			{
				string text = string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RESULT_DESCRIPTION, eggData.name);
				ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(array, 1f)
					{
						material = array[0]
					}
				};
				List<ComplexRecipe.RecipeElement> list = new List<ComplexRecipe.RecipeElement>
				{
					new ComplexRecipe.RecipeElement("RawEgg", 0.5f * eggData.mass, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
					new ComplexRecipe.RecipeElement("EggShell", 0.5f * eggData.mass, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
				};
				if (eggData.customOutput != null)
				{
					foreach (global::Tuple<Tag, float> tuple in eggData.customOutput)
					{
						list.Add(new ComplexRecipe.RecipeElement(tuple.first, tuple.second, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false));
					}
				}
				ComplexRecipe.RecipeElement[] array3 = list.ToArray();
				string text2 = ComplexRecipeManager.MakeObsoleteRecipeID("EggCracker", "RawEgg");
				string text3 = ComplexRecipeManager.MakeRecipeID("EggCracker", array2, array3);
				ComplexRecipe complexRecipe = new ComplexRecipe(text3, array2, array3, eggData.requiredDlcIds, eggData.forbiddenDlcIds);
				complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, eggData.name, text);
				complexRecipe.fabricators = new List<Tag> { "EggCracker" };
				complexRecipe.time = 5f;
				complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
				complexRecipe.customName = keyValuePair.Key.ProperName();
				complexRecipe.customSpritePrefabID = ((array2[0].material != null) ? array2[0].material.Name : array2[0].possibleMaterials[0].Name);
				ComplexRecipeManager.Get().AddObsoleteIDMapping(text2, text3);
			}
		}
	}

	public const string ID = "EggCracker";

	public static Dictionary<Tag, List<EggCrackerConfig.EggData>> EggsBySpecies = new Dictionary<Tag, List<EggCrackerConfig.EggData>>();

	private static List<EggCrackerConfig.EggData> uncategorizedEggData = new List<EggCrackerConfig.EggData>();

	public class EggData : IHasDlcRestrictions
	{
		public EggData(Tag id, string name, string description, float mass, string[] requiredDLC, string[] forbiddenDLC)
		{
			this.Config(id, name, description, mass, requiredDLC, forbiddenDLC, true);
		}

		public EggData(Tag id, string name, string description, float mass, string[] requiredDLC, string[] forbiddenDLC, bool hasCrackerRecipe = true)
		{
			this.Config(id, name, description, mass, requiredDLC, forbiddenDLC, hasCrackerRecipe);
		}

		private void Config(Tag id, string name, string description, float mass, string[] requiredDLC, string[] forbiddenDLC, bool hasCrackerRecipe = true)
		{
			this.id = id;
			this.name = name;
			this.description = description;
			this.mass = mass;
			this.requiredDlcIds = requiredDLC;
			this.forbiddenDlcIds = forbiddenDLC;
			this.hasCrackerRecipe = hasCrackerRecipe;
		}

		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		public Tag id;

		public float mass;

		public string name;

		public string description;

		public string[] requiredDlcIds;

		public string[] forbiddenDlcIds;

		public bool hasCrackerRecipe;

		public global::Tuple<Tag, float>[] customOutput;

		public bool isBaseMorph;
	}
}
