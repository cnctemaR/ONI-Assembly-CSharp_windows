using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class EntityTemplates
{
	public static GameObject CreateBasicEntity(string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null, float defaultTemperature = 293f)
	{
		GameObject gameObject = new GameObject("EntityTemplate");
		gameObject.SetActive(false);
		gameObject.name = id;
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
		KPrefabID kprefabID = gameObject.UpdateComponentRequirement<KPrefabID>(true);
		kprefabID.PrefabTag = TagManager.Create(id, name);
		if (additionalTags != null)
		{
			kprefabID.AddPrefabTags(additionalTags);
		}
		KSelectable kselectable = gameObject.UpdateComponentRequirement<KSelectable>(true);
		kselectable.SetName(name);
		gameObject.UpdateComponentRequirement<SaveLoadRoot>(true);
		gameObject.UpdateComponentRequirement<SavedObject>(true);
		gameObject.UpdateComponentRequirement<StateMachineController>(true);
		KBatchedAnimController kbatchedAnimController = gameObject.UpdateComponentRequirement<KBatchedAnimController>(true);
		kbatchedAnimController.AddAnims(new KAnimFile[] { anim });
		kbatchedAnimController.sceneLayer = sceneLayer;
		kbatchedAnimController.initialAnim = initialAnim;
		PrimaryElement primaryElement = gameObject.UpdateComponentRequirement<PrimaryElement>(true);
		primaryElement.ElementID = element;
		primaryElement.Temperature = defaultTemperature;
		if (unitMass)
		{
			primaryElement.MassPerUnit = mass;
			primaryElement.Units = 1f;
			GameTags.DisplayAsUnits.Add(kprefabID.PrefabTag);
		}
		else
		{
			primaryElement.Mass = mass;
		}
		gameObject.UpdateComponentRequirement<SimTemperatureTransfer>(true);
		InfoDescription infoDescription = gameObject.UpdateComponentRequirement<InfoDescription>(true);
		infoDescription.description = desc;
		gameObject.UpdateComponentRequirement<Notifier>(true);
		return gameObject;
	}

	public static GameObject CreatePlacedEntity(string id, string name, string desc, float mass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, int width, int height, EffectorValues decor, EffectorValues noise = default(EffectorValues), SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null, float defaultTemperature = 293f)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, mass, true, anim, initialAnim, sceneLayer, element, additionalTags, defaultTemperature);
		KBoxCollider2D kboxCollider2D = gameObject.UpdateComponentRequirement<KBoxCollider2D>(true);
		kboxCollider2D.size = new Vector2f(width, height);
		float num = 0.5f * (float)((width + 1) % 2);
		kboxCollider2D.offset = new Vector2f(num, (float)height / 2f);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.Offset = new Vector3(num, 0f, 0f);
		OccupyArea occupyArea = gameObject.UpdateComponentRequirement<OccupyArea>(true);
		occupyArea.OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(width, height);
		occupyArea.objectLayer = ObjectLayer.NumLayers;
		gameObject.UpdateComponentRequirement<Modifiers>(true);
		DecorProvider decorProvider = gameObject.UpdateComponentRequirement<DecorProvider>(true);
		decorProvider.SetValues(decor);
		return gameObject;
	}

	public static GameObject MakeHangingOffsets(GameObject template, int width, int height)
	{
		KBoxCollider2D component = template.GetComponent<KBoxCollider2D>();
		if (component)
		{
			component.size = new Vector2f(width, height);
			float num = 0.5f * (float)((width + 1) % 2);
			component.offset = new Vector2f(num, (float)(-(float)height) / 2f + 1f);
		}
		OccupyArea component2 = template.GetComponent<OccupyArea>();
		if (component2)
		{
			component2.OccupiedCellsOffsets = EntityTemplates.GenerateHangingOffsets(width, height);
			component2.objectLayer = ObjectLayer.NumLayers;
		}
		return template;
	}

	public static GameObject ExtendEntityToBasicPlant(GameObject template, float drowning_stamina = 15f, float drowning_regen = 5f, float temperature_lethal_low = 218.15f, float temperature_warning_low = 283.15f, float temperature_perfect_low = 291.15f, float temperature_perfect_high = 295.15f, float temperature_warning_high = 303.15f, float temperature_lethal_high = 398.15f, SimHashes[] safe_elements = null, bool pressure_sensitive = true, float pressure_lethal_low = 0f, float pressure_warning_low = 0.15f, string crop_id = null, bool can_drown = true, bool can_tinker = true)
	{
		template.UpdateComponentRequirement<EntombVulnerable>(true);
		PressureVulnerable pressureVulnerable = template.UpdateComponentRequirement<PressureVulnerable>(true);
		if (pressure_sensitive)
		{
			PressureVulnerable pressureVulnerable2 = pressureVulnerable;
			SimHashes[] safe_elements2 = safe_elements;
			pressureVulnerable2.Configure(pressure_warning_low, pressure_lethal_low, 10f, 30f, 0.75f, 5f, safe_elements2);
		}
		else
		{
			pressureVulnerable.Configure(safe_elements);
		}
		template.UpdateComponentRequirement<WiltCondition>(true);
		template.UpdateComponentRequirement<Uprootable>(true);
		template.UpdateComponentRequirement<UprootedMonitor>(true);
		template.UpdateComponentRequirement<ReceptacleMonitor>(true);
		template.UpdateComponentRequirement<Notifier>(true);
		if (can_drown)
		{
			DrowningMonitor drowningMonitor = template.UpdateComponentRequirement<DrowningMonitor>(true);
			drowningMonitor.Configure(drowning_stamina, drowning_regen, 0.95f);
		}
		TemperatureVulnerable temperatureVulnerable = template.UpdateComponentRequirement<TemperatureVulnerable>(true);
		temperatureVulnerable.Configure(temperature_warning_low, temperature_lethal_low, temperature_warning_high, temperature_lethal_high, temperature_perfect_low, temperature_perfect_high);
		template.UpdateComponentRequirement<OccupyArea>(true).objectLayer = ObjectLayer.Building;
		KPrefabID component = template.GetComponent<KPrefabID>();
		if (crop_id != null)
		{
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, component.PrefabID().ToString());
			Crop.CropVal cropVal = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == crop_id);
			Crop crop = template.UpdateComponentRequirement<Crop>(true);
			crop.Configure(cropVal);
			Growing growing = template.UpdateComponentRequirement<Growing>(true);
			growing.Configure(cropVal.cropDuration);
			template.UpdateComponentRequirement<Harvestable>(true);
		}
		component.prefabInitFn += delegate(GameObject inst)
		{
			PressureVulnerable component2 = inst.GetComponent<PressureVulnerable>();
			if (safe_elements != null)
			{
				foreach (SimHashes simHashes in safe_elements)
				{
					component2.safe_atmospheres.Add(ElementLoader.FindElementByHash(simHashes));
				}
			}
		};
		if (can_tinker)
		{
			Tinkerable.MakeFarmTinkerable(template);
		}
		return template;
	}

	public static GameObject ExtendEntityToBasicCreature(GameObject template, FactionManager.FactionID faction = FactionManager.FactionID.Prey, float HitPoints = 100f, string NavGridName = "HatchNavGrid", NavType navType = NavType.Floor, int max_probing_radius = 32, float moveSpeed = 2f, string onDeathDropID = "Meat", int onDeathDropCount = 1, bool drownVulnerable = true, bool entombVulnerable = true, float drowningStamina = 30f, float warningLowTemperature = 283f, float warningHighTemperature = 294f, float lethalLowTemperature = 243f, float lethalHighTemperature = 343f)
	{
		template.GetComponent<KBatchedAnimController>().isMovable = true;
		template.UpdateComponentRequirement<Health>(true).SetMaxHitPoints(HitPoints);
		template.UpdateComponentRequirement<CharacterOverlay>(true);
		template.UpdateComponentRequirement<RangedAttackable>(true);
		template.UpdateComponentRequirement<FactionAlignment>(true).Alignment = faction;
		template.UpdateComponentRequirement<Prioritizable>(true);
		template.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(warningLowTemperature, lethalLowTemperature, warningHighTemperature, lethalHighTemperature, 0f, 0f);
		if (drownVulnerable)
		{
			template.UpdateComponentRequirement<DrowningMonitor>(true).Configure(drowningStamina, 10f, 0.95f);
		}
		if (entombVulnerable)
		{
			template.UpdateComponentRequirement<EntombVulnerable>(true);
		}
		if (onDeathDropCount > 0 && onDeathDropID != string.Empty)
		{
			string[] array = new string[onDeathDropCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = onDeathDropID;
			}
			template.UpdateComponentRequirement<Butcherable>(true).SetDrops(array);
		}
		Navigator navigator = template.UpdateComponentRequirement<Navigator>(true);
		navigator.NavGridName = NavGridName;
		navigator.CurrentNavType = navType;
		navigator.defaultSpeed = moveSpeed;
		navigator.updateProber = true;
		navigator.maxProbingRadius = max_probing_radius;
		KPrefabID component = template.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			KPrefabID component2 = inst.GetComponent<KPrefabID>();
			new DeathMonitor.Instance(component2);
		};
		component.prefabSpawnFn += delegate(GameObject inst)
		{
			DeathMonitor.Instance smi = inst.GetSMI<DeathMonitor.Instance>();
			smi.StartSM();
		};
		return template;
	}

	public static Tag GetBaggedCreatureTag(Tag tag)
	{
		return TagManager.Create("Bagged" + tag.Name, null);
	}

	public static Tag GetUnbaggedCreatureTag(Tag bagged_tag)
	{
		return TagManager.Create(bagged_tag.Name.Substring(6), null);
	}

	public static string GetBaggedCreatureName(string name)
	{
		return "Bagged" + name;
	}

	public static GameObject CreateAndRegisterBaggedCreature(GameObject creature, string name, string desc, KAnimFile anim, string initialAnim, Tag previewPrefab)
	{
		KPrefabID component = creature.GetComponent<KPrefabID>();
		PrimaryElement component2 = creature.GetComponent<PrimaryElement>();
		string baggedCreatureName = EntityTemplates.GetBaggedCreatureName(component.PrefabTag.Name);
		GameObject gameObject = EntityTemplates.CreateLooseEntity(baggedCreatureName, name, desc, component2.Mass, true, anim, initialAnim, Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.CIRCLE, 1f, 1f, true, SimHashes.Creature, null);
		KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
		component3.AddPrefabTag(GameTags.BagableCreature);
		Assets.AddPrefab(component3);
		Baggable baggable = gameObject.AddComponent<Baggable>();
		baggable.animOverride = Assets.GetAnim("anim_restrain_creature_kanim");
		baggable.creatureTag = creature.GetComponent<KPrefabID>().PrefabTag;
		return gameObject;
	}

	public static GameObject CreateLooseEntity(string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, EntityTemplates.CollisionShape collisionShape, float width = 1f, float height = 1f, bool isPickupable = false, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, element, additionalTags, 293f);
		gameObject = EntityTemplates.AddCollision(gameObject, collisionShape, width, height);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.isMovable = true;
		gameObject.UpdateComponentRequirement<Modifiers>(true);
		if (isPickupable)
		{
			Pickupable pickupable = gameObject.UpdateComponentRequirement<Pickupable>(true);
			pickupable.SetWorkTime(5f);
		}
		return gameObject;
	}

	public static GameObject CreateOreEntity(SimHashes elementID, EntityTemplates.CollisionShape shape, float width, float height, List<Tag> additionalTags = null, float default_temperature = 293f)
	{
		Element element = ElementLoader.FindElementByHash(elementID);
		string text = element.id.ToString();
		GameObject gameObject = new GameObject(text);
		gameObject.SetActive(false);
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
		KPrefabID kprefabID = gameObject.UpdateComponentRequirement<KPrefabID>(true);
		kprefabID.PrefabTag = element.tag;
		if (additionalTags != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			component.AddPrefabTags(additionalTags);
		}
		PrimaryElement primaryElement = gameObject.UpdateComponentRequirement<PrimaryElement>(true);
		primaryElement.SetElement(elementID);
		primaryElement.Mass = 1f;
		primaryElement.Temperature = default_temperature;
		Pickupable pickupable = gameObject.UpdateComponentRequirement<Pickupable>(true);
		pickupable.SetWorkTime(5f);
		KSelectable kselectable = gameObject.UpdateComponentRequirement<KSelectable>(true);
		kselectable.SetName(element.name);
		gameObject.UpdateComponentRequirement<SaveLoadRoot>(true);
		gameObject.UpdateComponentRequirement<SavedObject>(true);
		gameObject.UpdateComponentRequirement<StateMachineController>(true);
		KBatchedAnimController kbatchedAnimController = gameObject.UpdateComponentRequirement<KBatchedAnimController>(true);
		kbatchedAnimController.AddAnims(new KAnimFile[] { element.substance.anim });
		kbatchedAnimController.sceneLayer = Grid.SceneLayer.Front;
		kbatchedAnimController.initialAnim = "idle1";
		kbatchedAnimController.isMovable = true;
		gameObject.UpdateComponentRequirement<SimTemperatureTransfer>(true);
		gameObject.UpdateComponentRequirement<Modifiers>(true);
		OccupyArea occupyArea = gameObject.UpdateComponentRequirement<OccupyArea>(true);
		occupyArea.OccupiedCellsOffsets = Grid.DefaultOffset;
		DecorProvider decorProvider = gameObject.UpdateComponentRequirement<DecorProvider>(true);
		decorProvider.baseDecor = -10f;
		decorProvider.baseRadius = 1f;
		gameObject.UpdateComponentRequirement<ElementChunk>(true);
		return EntityTemplates.AddCollision(gameObject, shape, width, height);
	}

	public static GameObject CreateSolidOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		return EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.CIRCLE, 0.5f, 0.5f, additionalTags, 293f);
	}

	public static GameObject CreateLiquidOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		GameObject gameObject = EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 0.6f, additionalTags, 293f);
		Dumpable dumpable = gameObject.UpdateComponentRequirement<Dumpable>(true);
		dumpable.SetWorkTime(5f);
		gameObject.UpdateComponentRequirement<SubstanceChunk>(true);
		return gameObject;
	}

	public static GameObject CreateGasOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		GameObject gameObject = EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 0.6f, additionalTags, 293f);
		Dumpable dumpable = gameObject.UpdateComponentRequirement<Dumpable>(true);
		dumpable.SetWorkTime(5f);
		gameObject.UpdateComponentRequirement<SubstanceChunk>(true);
		return gameObject;
	}

	public static GameObject ExtendEntityToFood(GameObject template, EdiblesManager.FoodInfo foodInfo, bool canRot = true)
	{
		template.UpdateComponentRequirement<EntitySplitter>(true);
		KPrefabID component = template.GetComponent<KPrefabID>();
		if (foodInfo.CaloriesPerUnit > 0f)
		{
			Edible edible = template.UpdateComponentRequirement<Edible>(true);
			edible.FoodInfo = foodInfo;
			component.instantiateFn += delegate(GameObject go)
			{
				go.GetComponent<Edible>().FoodInfo = foodInfo;
			};
			GameTags.DisplayAsCalories.Add(component.PrefabTag);
			EntityTemplates.CreateAndRegisterCompostableFromEdible(template);
		}
		else
		{
			component.AddPrefabTag(GameTags.CookingIngredient);
			GameTags.DisplayAsUnits.Add(component.PrefabTag);
		}
		if (canRot)
		{
			component.prefabInitFn += delegate(GameObject inst)
			{
				KPrefabID component2 = inst.GetComponent<KPrefabID>();
				Rottable.Instance instance = new Rottable.Instance(component2, foodInfo.SpoilTime, foodInfo.StaleTime);
				instance.RotTemperature = foodInfo.RotTemperature;
			};
			component.prefabSpawnFn += delegate(GameObject inst)
			{
				Rottable.Instance smi = inst.GetSMI<Rottable.Instance>();
				smi.StartSM();
			};
		}
		return template;
	}

	public static GameObject ExtendEntityToMedicine(GameObject template, MedicineInfo medicineInfo)
	{
		KPrefabID component = template.GetComponent<KPrefabID>();
		component.AddPrefabTag(GameTags.Medicine);
		MedicinalPill medicinalPill = template.UpdateComponentRequirement<MedicinalPill>(true);
		medicinalPill.info = medicineInfo;
		return template;
	}

	public static GameObject ExtendPlantToFertilizable(GameObject template, PlantElementAbsorber.ConsumeInfo[] fertilizers)
	{
		HashedString idHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
		foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in fertilizers)
		{
			ManualDeliveryKG manualDeliveryKG = template.AddComponent<ManualDeliveryKG>();
			manualDeliveryKG.RequestedItemTag = consumeInfo.tag;
			manualDeliveryKG.capacity = consumeInfo.massConsumptionRate * 600f * 3f;
			manualDeliveryKG.refillMass = consumeInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.minimumMass = consumeInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
			manualDeliveryKG.choreTags = new Tag[] { GameTags.ChoreTypes.Farming };
			manualDeliveryKG.choreTypeIDHash = idHash;
		}
		KPrefabID component = template.GetComponent<KPrefabID>();
		FertilizationMonitor.Instance.Def def = new FertilizationMonitor.Instance.Def();
		def.wrongFertilizerTestTag = GameTags.Solid;
		def.consumedElements = fertilizers;
		template.GetComponent<StateMachineController>().AddDef(def);
		component.prefabInitFn += delegate(GameObject inst)
		{
			foreach (ManualDeliveryKG manualDeliveryKG2 in inst.GetComponents<ManualDeliveryKG>())
			{
				manualDeliveryKG2.Pause(true, "init");
			}
		};
		return template;
	}

	public static GameObject ExtendPlantToIrrigated(GameObject template, PlantElementAbsorber.ConsumeInfo info)
	{
		return EntityTemplates.ExtendPlantToIrrigated(template, new PlantElementAbsorber.ConsumeInfo[] { info });
	}

	public static GameObject ExtendPlantToIrrigated(GameObject template, PlantElementAbsorber.ConsumeInfo[] liquids)
	{
		foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in liquids)
		{
			ManualDeliveryKG manualDeliveryKG = template.AddComponent<ManualDeliveryKG>();
			manualDeliveryKG.RequestedItemTag = consumeInfo.tag;
			manualDeliveryKG.capacity = consumeInfo.massConsumptionRate * 600f * 3f;
			manualDeliveryKG.refillMass = consumeInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.minimumMass = consumeInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
			manualDeliveryKG.choreTags = new Tag[] { GameTags.ChoreTypes.Farming };
		}
		IrrigationMonitor.Instance.Def def = new IrrigationMonitor.Instance.Def();
		def.wrongIrrigationTestTag = GameTags.Liquid;
		def.consumedElements = liquids;
		template.GetComponent<StateMachineController>().AddDef(def);
		return template;
	}

	public static GameObject CreateAndRegisterCompostableFromEdible(GameObject edible)
	{
		edible.AddComponent<Compostable>();
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(edible);
		string text = "Compost" + gameObject.GetComponent<KPrefabID>().PrefabTag.Name;
		gameObject.GetComponent<KPrefabID>().PrefabTag = new Tag(text);
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
		global::UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Edible>());
		gameObject.name = text;
		gameObject.GetComponent<KSelectable>().SetName(ITEMS.FOOD.COMPOST + " " + gameObject.GetComponent<KSelectable>().GetName());
		gameObject.GetComponent<Compostable>().originalPrefab = edible;
		gameObject.GetComponent<Compostable>().compostPrefab = gameObject;
		edible.GetComponent<Compostable>().originalPrefab = edible;
		edible.GetComponent<Compostable>().compostPrefab = gameObject;
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		return gameObject;
	}

	public static GameObject CreateAndRegisterSeedForPlant(GameObject plant, SeedProducer.ProductionType productionType, string id, string name, string desc, KAnimFile anim, string initialAnim = "object", int numberOfSeeds = 1, List<Tag> additionalTags = null, SingleEntityReceptacle.ReceptacleDirection planterDirection = SingleEntityReceptacle.ReceptacleDirection.Top, Tag replantGroundTag = default(Tag), int sortOrder = 0, string domesticatedDescription = "", EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE, float width = 0.25f, float height = 0.25f, Recipe.Ingredient[] recipe_ingredients = null, string recipe_description = "")
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, true, anim, initialAnim, Grid.SceneLayer.Front, collisionShape, width, height, true, SimHashes.Creature, null);
		gameObject.UpdateComponentRequirement<EntitySplitter>(true);
		PlantableSeed plantableSeed = gameObject.UpdateComponentRequirement<PlantableSeed>(true);
		plantableSeed.PlantID = new Tag(plant.name);
		plantableSeed.replantGroundTag = replantGroundTag;
		plantableSeed.sortOrder = sortOrder;
		plantableSeed.domesticatedDescription = domesticatedDescription;
		plantableSeed.direction = planterDirection;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddPrefabTag(GameTags.Seed);
		if (additionalTags != null)
		{
			component.AddPrefabTags(additionalTags);
		}
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		Assets.AddPrefab(component2);
		SeedProducer seedProducer = plant.UpdateComponentRequirement<SeedProducer>(true);
		seedProducer.Configure(gameObject.name, productionType, numberOfSeeds);
		if (recipe_ingredients != null)
		{
			Recipe recipe = new Recipe(id, 1f, (SimHashes)0, null, recipe_description, 1).SetFabricator("SeedSplicer", FOOD.RECIPES.STANDARD_COOK_TIME);
			foreach (Recipe.Ingredient ingredient in recipe_ingredients)
			{
				recipe.AddIngredient(ingredient);
			}
		}
		return gameObject;
	}

	public static GameObject CreateAndRegisterPreview(string id, KAnimFile anim, string initialAnim, ObjectLayer objectLayer, int width, int height)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, id, id, 1f, anim, initialAnim, Grid.SceneLayer.Front, width, height, global::TUNING.BUILDINGS.DECOR.NONE, default(EffectorValues), SimHashes.Creature, null, 293f);
		gameObject.UpdateComponentRequirement<KSelectable>(false);
		gameObject.UpdateComponentRequirement<SaveLoadRoot>(false);
		gameObject.UpdateComponentRequirement<SavedObject>(false);
		EntityPreview entityPreview = gameObject.UpdateComponentRequirement<EntityPreview>(true);
		entityPreview.objectLayer = objectLayer;
		OccupyArea occupyArea = gameObject.UpdateComponentRequirement<OccupyArea>(true);
		occupyArea.objectLayer = objectLayer;
		occupyArea.ApplyToCells = false;
		gameObject.UpdateComponentRequirement<Storage>(true);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		Assets.AddPrefab(component);
		return gameObject;
	}

	public static GameObject CreateAndRegisterPreviewForPlant(GameObject seed, string id, KAnimFile anim, string initialAnim, int width, int height)
	{
		GameObject gameObject = EntityTemplates.CreateAndRegisterPreview(id, anim, initialAnim, ObjectLayer.Building, width, height);
		PlantableSeed component = seed.GetComponent<PlantableSeed>();
		component.PreviewID = TagManager.Create(id, null);
		return gameObject;
	}

	public static CellOffset[] GenerateOffsets(int width, int height)
	{
		int num = width / 2;
		int num2 = num;
		int num3 = num2 - width + 1;
		int num4 = 0;
		int num5 = height - 1;
		return EntityTemplates.GenerateOffsets(num3, num4, num2, num5);
	}

	private static CellOffset[] GenerateOffsets(int startX, int startY, int endX, int endY)
	{
		List<CellOffset> list = new List<CellOffset>();
		for (int i = startY; i <= endY; i++)
		{
			for (int j = startX; j <= endX; j++)
			{
				list.Add(new CellOffset
				{
					x = j,
					y = i
				});
			}
		}
		return list.ToArray();
	}

	public static CellOffset[] GenerateHangingOffsets(int width, int height)
	{
		int num = width / 2;
		int num2 = num;
		int num3 = num2 - width + 1;
		int num4 = -height + 1;
		int num5 = 0;
		return EntityTemplates.GenerateOffsets(num3, num4, num2, num5);
	}

	public static GameObject AddCollision(GameObject template, EntityTemplates.CollisionShape shape, float width, float height)
	{
		if (shape != EntityTemplates.CollisionShape.RECTANGLE)
		{
			if (shape != EntityTemplates.CollisionShape.POLYGONAL)
			{
				KCircleCollider2D kcircleCollider2D = template.UpdateComponentRequirement<KCircleCollider2D>(true);
				kcircleCollider2D.radius = Mathf.Max(width, height);
			}
			else
			{
				template.UpdateComponentRequirement<PolygonCollider2D>(true);
			}
		}
		else
		{
			KBoxCollider2D kboxCollider2D = template.UpdateComponentRequirement<KBoxCollider2D>(true);
			kboxCollider2D.size = new Vector2f(width, height);
		}
		return template;
	}

	public enum CollisionShape
	{
		CIRCLE,
		RECTANGLE,
		POLYGONAL
	}
}
