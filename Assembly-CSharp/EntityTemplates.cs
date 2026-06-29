using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class EntityTemplates
{
	public static GameObject CreateEntity(string id, string name)
	{
		GameObject gameObject = new GameObject("EntityTemplate");
		gameObject.SetActive(false);
		gameObject.name = id;
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		kprefabID.PrefabTag = TagManager.Create(id, name);
		KSelectable kselectable = gameObject.AddOrGet<KSelectable>();
		kselectable.SetName(name);
		return gameObject;
	}

	public static GameObject CreateBasicEntity(string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null, float defaultTemperature = 293f)
	{
		GameObject gameObject = EntityTemplates.CreateEntity(id, name);
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		if (additionalTags != null)
		{
			kprefabID.AddPrefabTags(additionalTags);
		}
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<SavedObject>();
		gameObject.AddOrGet<StateMachineController>();
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.AddAnims(new KAnimFile[] { anim });
		kbatchedAnimController.sceneLayer = sceneLayer;
		kbatchedAnimController.initialAnim = initialAnim;
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
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
		gameObject.AddOrGet<SimTemperatureTransfer>();
		InfoDescription infoDescription = gameObject.AddOrGet<InfoDescription>();
		infoDescription.description = desc;
		gameObject.AddOrGet<Notifier>();
		return gameObject;
	}

	public static GameObject CreatePlacedEntity(string id, string name, string desc, float mass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, int width, int height, EffectorValues decor, EffectorValues noise = default(EffectorValues), SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null, float defaultTemperature = 293f)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, mass, true, anim, initialAnim, sceneLayer, element, additionalTags, defaultTemperature);
		KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kboxCollider2D.size = new Vector2f(width, height);
		float num = 0.5f * (float)((width + 1) % 2);
		kboxCollider2D.offset = new Vector2f(num, (float)height / 2f);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.Offset = new Vector3(num, 0f, 0f);
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(width, height);
		occupyArea.objectLayer = ObjectLayer.NumLayers;
		gameObject.AddOrGet<Modifiers>();
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
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
		template.AddOrGet<EntombVulnerable>();
		PressureVulnerable pressureVulnerable = template.AddOrGet<PressureVulnerable>();
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
		template.AddOrGet<WiltCondition>();
		template.AddOrGet<Uprootable>();
		template.AddOrGet<UprootedMonitor>();
		template.AddOrGet<ReceptacleMonitor>();
		template.AddOrGet<Notifier>();
		if (can_drown)
		{
			DrowningMonitor drowningMonitor = template.AddOrGet<DrowningMonitor>();
			drowningMonitor.Configure(drowning_stamina, drowning_regen, 0.95f);
		}
		TemperatureVulnerable temperatureVulnerable = template.AddOrGet<TemperatureVulnerable>();
		temperatureVulnerable.Configure(temperature_warning_low, temperature_lethal_low, temperature_warning_high, temperature_lethal_high, temperature_perfect_low, temperature_perfect_high);
		template.AddOrGet<OccupyArea>().objectLayer = ObjectLayer.Building;
		KPrefabID component = template.GetComponent<KPrefabID>();
		if (crop_id != null)
		{
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, component.PrefabID().ToString());
			Crop.CropVal cropVal = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == crop_id);
			Crop crop = template.AddOrGet<Crop>();
			crop.Configure(cropVal);
			Growing growing = template.AddOrGet<Growing>();
			growing.Configure(cropVal.cropDuration);
			template.AddOrGet<Harvestable>();
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

	public static GameObject ExtendEntityToFertileCreature(GameObject prefab, string anim, float calorie_loss_per_cycle_when_wild, float calorie_loss_per_cycle_when_tame, int space_required_per_creature)
	{
		FertilityMonitor.Def def = prefab.AddOrGetDef<FertilityMonitor.Def>();
		Tag prefabTag = prefab.GetComponent<KPrefabID>().PrefabTag;
		string text = prefabTag.Name + "Egg";
		EggConfig.CreateEgg(text, prefabTag, anim);
		def.eggPrefab = new Tag(text);
		prefab.AddOrGetDef<RanchableMonitor.Def>();
		prefab.AddOrGetDef<HappinessMonitor.Def>();
		WildnessMonitor.Def def2 = prefab.AddOrGetDef<WildnessMonitor.Def>();
		def2.wildEffect = new Effect("Wild" + prefabTag.Name, global::STRINGS.CREATURES.MODIFIERS.WILD.NAME, global::STRINGS.CREATURES.MODIFIERS.WILD.TOOLTIP, 0f, true, true, false);
		def2.wildEffect.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -calorie_loss_per_cycle_when_wild / 600f, global::STRINGS.CREATURES.MODIFIERS.WILD.NAME, false, false, true));
		def2.wildEffect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, 0.008333334f, global::STRINGS.CREATURES.MODIFIERS.WILD.NAME, false, false, true));
		def2.wildEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -0.033333335f, global::STRINGS.CREATURES.MODIFIERS.WILD.NAME, false, false, true));
		def2.tameEffect = new Effect("Tame" + prefabTag.Name, global::STRINGS.CREATURES.MODIFIERS.TAME.NAME, global::STRINGS.CREATURES.MODIFIERS.TAME.TOOLTIP, 0f, true, true, false);
		def2.tameEffect.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -calorie_loss_per_cycle_when_tame / 600f, global::STRINGS.CREATURES.MODIFIERS.TAME.NAME, false, false, true));
		def2.tameEffect.Add(new AttributeModifier(Db.Get().Amounts.Happiness.deltaAttribute.Id, -0.016666668f, global::STRINGS.CREATURES.MODIFIERS.TAME.NAME, false, false, true));
		OvercrowdingMonitor.Def def3 = prefab.AddOrGetDef<OvercrowdingMonitor.Def>();
		def3.spaceRequiredPerCreature = space_required_per_creature;
		return prefab;
	}

	public static GameObject ExtendEntityToBasicCreature(GameObject template, FactionManager.FactionID faction = FactionManager.FactionID.Prey, string initialTraitID = null, string NavGridName = "HatchNavGrid", NavType navType = NavType.Floor, int max_probing_radius = 32, float moveSpeed = 2f, string onDeathDropID = "Meat", int onDeathDropCount = 1, bool drownVulnerable = true, bool entombVulnerable = true, float drowningStamina = 30f, float warningLowTemperature = 283f, float warningHighTemperature = 294f, float lethalLowTemperature = 243f, float lethalHighTemperature = 343f)
	{
		template.GetComponent<KBatchedAnimController>().isMovable = true;
		AgeMonitor.Def def = template.AddOrGetDef<AgeMonitor.Def>();
		def.minLifeTimeInCycles = 48000f;
		def.maxLifeTimeInCycles = 72000f;
		Modifiers modifiers = template.AddOrGet<Modifiers>();
		if (initialTraitID != null)
		{
			modifiers.initialTraits = new string[] { initialTraitID };
		}
		modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		template.AddOrGet<Traits>();
		template.AddOrGet<Health>();
		template.AddOrGet<CharacterOverlay>();
		template.AddOrGet<RangedAttackable>();
		template.AddOrGet<FactionAlignment>().Alignment = faction;
		template.AddOrGet<Prioritizable>();
		template.AddOrGet<Effects>();
		template.AddOrGetDef<DebugGoToMonitor.Def>();
		template.AddOrGetDef<DeathMonitor.Def>();
		template.AddOrGet<TemperatureVulnerable>().Configure(warningLowTemperature, lethalLowTemperature, warningHighTemperature, lethalHighTemperature, 0f, 0f);
		if (drownVulnerable)
		{
			template.AddOrGet<DrowningMonitor>().Configure(drowningStamina, 10f, 0.95f);
		}
		if (entombVulnerable)
		{
			template.AddOrGet<EntombVulnerable>();
		}
		if (onDeathDropCount > 0 && onDeathDropID != string.Empty)
		{
			string[] array = new string[onDeathDropCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = onDeathDropID;
			}
			template.AddOrGet<Butcherable>().SetDrops(array);
		}
		Navigator navigator = template.AddOrGet<Navigator>();
		navigator.NavGridName = NavGridName;
		navigator.CurrentNavType = navType;
		navigator.defaultSpeed = moveSpeed;
		navigator.updateProber = true;
		navigator.maxProbingRadius = max_probing_radius;
		navigator.sceneLayer = Grid.SceneLayer.Creatures;
		return template;
	}

	public static void AddCreatureBrain(GameObject prefab, ChoreTable.Builder chore_table)
	{
		prefab.AddOrGet<Brain>();
		ChoreConsumer choreConsumer = prefab.AddOrGet<ChoreConsumer>();
		choreConsumer.choreTable = chore_table.CreateTable();
		prefab.AddOrGet<KPrefabID>().AddPrefabTag(GameTags.CreatureBrain);
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity(baggedCreatureName, name, desc, component2.Mass, true, anim, initialAnim, Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.CIRCLE, 0.5f, 0.5f, true, SimHashes.Creature, null);
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
		gameObject.AddOrGet<Modifiers>();
		if (isPickupable)
		{
			Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
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
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		kprefabID.PrefabTag = element.tag;
		if (additionalTags != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			component.AddPrefabTags(additionalTags);
		}
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(elementID);
		primaryElement.Mass = 1f;
		primaryElement.Temperature = default_temperature;
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		pickupable.SetWorkTime(5f);
		KSelectable kselectable = gameObject.AddOrGet<KSelectable>();
		kselectable.SetName(element.name);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<SavedObject>();
		gameObject.AddOrGet<StateMachineController>();
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.AddAnims(new KAnimFile[] { element.substance.anim });
		kbatchedAnimController.sceneLayer = Grid.SceneLayer.Front;
		kbatchedAnimController.initialAnim = "idle1";
		kbatchedAnimController.isMovable = true;
		gameObject.AddOrGet<SimTemperatureTransfer>();
		gameObject.AddOrGet<Modifiers>();
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.OccupiedCellsOffsets = Grid.DefaultOffset;
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.baseDecor = -10f;
		decorProvider.baseRadius = 1f;
		gameObject.AddOrGet<ElementChunk>();
		return EntityTemplates.AddCollision(gameObject, shape, width, height);
	}

	public static GameObject CreateSolidOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		return EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.CIRCLE, 0.5f, 0.5f, additionalTags, 293f);
	}

	public static GameObject CreateLiquidOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		GameObject gameObject = EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 0.6f, additionalTags, 293f);
		Dumpable dumpable = gameObject.AddOrGet<Dumpable>();
		dumpable.SetWorkTime(5f);
		gameObject.AddOrGet<SubstanceChunk>();
		return gameObject;
	}

	public static GameObject CreateGasOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		GameObject gameObject = EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 0.6f, additionalTags, 293f);
		Dumpable dumpable = gameObject.AddOrGet<Dumpable>();
		dumpable.SetWorkTime(5f);
		gameObject.AddOrGet<SubstanceChunk>();
		return gameObject;
	}

	public static GameObject ExtendEntityToFood(GameObject template, EdiblesManager.FoodInfo foodInfo, bool canRot = true)
	{
		template.AddOrGet<EntitySplitter>();
		KPrefabID component = template.GetComponent<KPrefabID>();
		if (foodInfo.CaloriesPerUnit > 0f)
		{
			Edible edible = template.AddOrGet<Edible>();
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
			Rottable.Def def = template.AddOrGetDef<Rottable.Def>();
			def.rotTemperature = foodInfo.RotTemperature;
			def.spoilTime = foodInfo.SpoilTime;
			def.staleTime = foodInfo.StaleTime;
		}
		return template;
	}

	public static GameObject ExtendEntityToMedicine(GameObject template, MedicineInfo medicineInfo)
	{
		template.AddOrGet<EntitySplitter>();
		KPrefabID component = template.GetComponent<KPrefabID>();
		component.AddPrefabTag(GameTags.Medicine);
		MedicinalPill medicinalPill = template.AddOrGet<MedicinalPill>();
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
		FertilizationMonitor.Def def = template.AddOrGetDef<FertilizationMonitor.Def>();
		def.wrongFertilizerTestTag = GameTags.Solid;
		def.consumedElements = fertilizers;
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
		HashedString idHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
		foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in liquids)
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
		IrrigationMonitor.Def def = template.AddOrGetDef<IrrigationMonitor.Def>();
		def.wrongIrrigationTestTag = GameTags.Liquid;
		def.consumedElements = liquids;
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
		gameObject.AddOrGet<EntitySplitter>();
		PlantableSeed plantableSeed = gameObject.AddOrGet<PlantableSeed>();
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
		SeedProducer seedProducer = plant.AddOrGet<SeedProducer>();
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
		EntityPreview entityPreview = gameObject.AddOrGet<EntityPreview>();
		entityPreview.objectLayer = objectLayer;
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.objectLayer = objectLayer;
		occupyArea.ApplyToCells = false;
		gameObject.AddOrGet<Storage>();
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
				KCircleCollider2D kcircleCollider2D = template.AddOrGet<KCircleCollider2D>();
				kcircleCollider2D.radius = Mathf.Max(width, height);
			}
			else
			{
				template.AddOrGet<PolygonCollider2D>();
			}
		}
		else
		{
			KBoxCollider2D kboxCollider2D = template.AddOrGet<KBoxCollider2D>();
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
