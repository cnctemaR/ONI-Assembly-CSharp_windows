using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Klei.AI;
using TUNING;
using UnityEngine;

public class EntityTemplates
{
	public static GameObject CreateBasicEntity(string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null)
	{
		GameObject gameObject = new GameObject("EntityTemplate");
		gameObject.SetActive(false);
		gameObject.name = id;
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.EntityPrefabs).transform;
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
		primaryElement.Temperature = 293f;
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

	public static GameObject CreatePlacedEntity(string id, string name, string desc, float mass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, int width, int height, DecorValues decor, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null)
	{
		Debug.AssertFormat(anim != null, "Cant create [{0}] entity witiout an anim", new object[] { name });
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, mass, true, anim, initialAnim, sceneLayer, element, additionalTags);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.defaultSpawnOffset = CellAlignment.Bottom;
		BoxCollider2D boxCollider2D = gameObject.UpdateComponentRequirement<BoxCollider2D>(true);
		boxCollider2D.size = new Vector2f(width, height);
		float num = 0.5f * (float)((width + 1) % 2);
		boxCollider2D.offset = new Vector2f(num, (float)height / 2f);
		KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
		component2.Offset = new Vector3(num, 0f, 0f);
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
		BoxCollider2D component = template.GetComponent<BoxCollider2D>();
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

	public static GameObject ExtendEntityToBasicPlant(GameObject template, float drowning_stamina = 15f, float drowning_regen = 5f, float temperature_lethal_low = 218.15f, float temperature_warning_low = 283.15f, float temperature_perfect_low = 291.15f, float temperature_perfect_high = 295.15f, float temperature_warning_high = 303.15f, float temperature_lethal_high = 398.15f, float pressure_lethal_low = 0f, float pressure_warning_low = 0.15f, float grow_time = 1f, string crop_id = null)
	{
		template.UpdateComponentRequirement<EntombVulnerable>(true);
		PressureVulnerable pressureVulnerable = template.UpdateComponentRequirement<PressureVulnerable>(true);
		pressureVulnerable.Configure(pressure_warning_low, pressure_lethal_low, 10f, 30f, 0.75f, 5f);
		template.UpdateComponentRequirement<WiltCondition>(true);
		template.UpdateComponentRequirement<Uprootable>(true);
		template.UpdateComponentRequirement<UprootedMonitor>(true);
		DrowningMonitor drowningMonitor = template.UpdateComponentRequirement<DrowningMonitor>(true);
		drowningMonitor.Configure(drowning_stamina, drowning_regen, 0.95f);
		TemperatureVulnerable temperatureVulnerable = template.UpdateComponentRequirement<TemperatureVulnerable>(true);
		temperatureVulnerable.Configure(temperature_warning_low, temperature_lethal_low, temperature_warning_high, temperature_lethal_high, temperature_perfect_low, temperature_perfect_high);
		template.UpdateComponentRequirement<OccupyArea>(true).objectLayer = ObjectLayer.Building;
		if (crop_id != null)
		{
			Crop.CropVal cropVal = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == crop_id);
			Crop crop = template.UpdateComponentRequirement<Crop>(true);
			crop.Configure(cropVal);
			Growing growing = template.UpdateComponentRequirement<Growing>(true);
			growing.Configure(cropVal.cropDuration, cropVal.regrowDuration);
			template.UpdateComponentRequirement<Harvestable>(true);
		}
		template.UpdateComponentRequirement<Prioritizable>(true);
		return template;
	}

	public static GameObject ExtendEntityToBasicCreature(GameObject template, FactionManager.FactionID faction = FactionManager.FactionID.Prey, float HitPoints = 100f, string NavGridName = "HatchNavGrid", NavType navType = NavType.Floor, float moveSpeed = 2f, string onDeathDropID = "Meat", int onDeathDropCount = 1, bool drownVulnerable = true, bool entombVulnerable = true, float drowningStamina = 30f, float warningLowTemperature = 283f, float warningHighTemperature = 294f, float lethalLowTemperature = 243f, float lethalHighTemperature = 343f)
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
		navigator.maxProbingRadius = 100;
		return template;
	}

	public static GameObject CreateLooseEntity(string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, EntityTemplates.CollisionShape collisionShape, float width = 1f, float height = 1f, bool isPickupable = false, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, element, additionalTags);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.defaultSpawnOffset = CellAlignment.RandomInternal;
		if (collisionShape != EntityTemplates.CollisionShape.RECTANGLE)
		{
			if (collisionShape != EntityTemplates.CollisionShape.POLYGONAL)
			{
				CircleCollider2D circleCollider2D = gameObject.UpdateComponentRequirement<CircleCollider2D>(true);
				circleCollider2D.radius = Mathf.Max(width, height);
			}
			else
			{
				gameObject.UpdateComponentRequirement<PolygonCollider2D>(true);
			}
		}
		else
		{
			BoxCollider2D boxCollider2D = gameObject.UpdateComponentRequirement<BoxCollider2D>(true);
			boxCollider2D.size = new Vector2f(width, height);
		}
		KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
		component2.isMovable = true;
		gameObject.UpdateComponentRequirement<Modifiers>(true);
		if (isPickupable)
		{
			Pickupable pickupable = gameObject.UpdateComponentRequirement<Pickupable>(true);
			pickupable.SetWorkTime(5f);
		}
		return gameObject;
	}

	public static GameObject CreateOreEntity(SimHashes elementID, List<Tag> additionalTags = null)
	{
		Element element = ElementLoader.elementTable[elementID];
		string text = element.id.ToString();
		GameObject gameObject = new GameObject(text);
		gameObject.SetActive(false);
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.EntityPrefabs).transform;
		KPrefabID kprefabID = gameObject.UpdateComponentRequirement<KPrefabID>(true);
		kprefabID.PrefabTag = TagManager.Create(text, element.name);
		kprefabID.defaultSpawnOffset = CellAlignment.RandomInternal;
		if (additionalTags != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			component.AddPrefabTags(additionalTags);
		}
		PrimaryElement primaryElement = gameObject.UpdateComponentRequirement<PrimaryElement>(true);
		primaryElement.SetElement(elementID);
		primaryElement.Mass = 1f;
		primaryElement.Temperature = 293f;
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
		CircleCollider2D circleCollider2D = gameObject.UpdateComponentRequirement<CircleCollider2D>(true);
		circleCollider2D.radius = 0.25f;
		return gameObject;
	}

	public static GameObject ExtendEntityToFood(GameObject template, EdiblesManager.FoodInfo foodInfo, bool canRot = true)
	{
		template.UpdateComponentRequirement<EntitySplitter>(true);
		PrimaryElement component = template.GetComponent<PrimaryElement>();
		component.CountableUnits = true;
		KPrefabID component2 = template.GetComponent<KPrefabID>();
		if (foodInfo.CaloriesPerUnit > 0f)
		{
			Edible edible = template.UpdateComponentRequirement<Edible>(true);
			edible.FoodInfo = foodInfo;
			component2.instantiateFn += delegate(GameObject go)
			{
				go.GetComponent<Edible>().FoodInfo = foodInfo;
			};
			GameTags.DisplayAsCalories.Add(component2.PrefabTag);
		}
		else
		{
			component2.AddPrefabTag(GameTags.CookingIngredient);
			GameTags.DisplayAsUnits.Add(component2.PrefabTag);
		}
		if (canRot)
		{
			component2.prefabInitFn += delegate(GameObject inst)
			{
				KPrefabID component3 = inst.GetComponent<KPrefabID>();
				Rottable.Instance instance = new Rottable.Instance(component3, foodInfo.SpoilTime, foodInfo.StaleTime);
				instance.RotTemperature = foodInfo.RotTemperature;
			};
			component2.prefabSpawnFn += delegate(GameObject inst)
			{
				Rottable.Instance smi = inst.GetSMI<Rottable.Instance>();
				smi.StartSM();
			};
		}
		return template;
	}

	public static GameObject ExtendEntityToMedicine(GameObject template, MedicineInfo medicineInfo)
	{
		MedicinalPill medicinalPill = template.UpdateComponentRequirement<MedicinalPill>(true);
		medicinalPill.curedDiseases = medicineInfo.curedDiseases;
		medicinalPill.boostMultipliers = medicineInfo.boostMultipliers;
		medicinalPill.medicineType = medicineInfo.medicineType;
		PrimaryElement component = template.GetComponent<PrimaryElement>();
		component.CountableUnits = true;
		return template;
	}

	public static GameObject ExtendPlantToFertilizable(GameObject template, FertilizationMonitor.FertilizerInfo[] fertilizers)
	{
		foreach (FertilizationMonitor.FertilizerInfo fertilizerInfo in fertilizers)
		{
			ManualDeliveryKG manualDeliveryKG = template.AddComponent<ManualDeliveryKG>();
			manualDeliveryKG.RequestedItemTag = fertilizerInfo.tag;
			manualDeliveryKG.capacity = fertilizerInfo.massConsumptionRate * 600f * 3f;
			manualDeliveryKG.refillMass = fertilizerInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.minimumMass = fertilizerInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
		}
		KPrefabID component = template.GetComponent<KPrefabID>();
		FertilizationMonitor.Instance.Def def = new FertilizationMonitor.Instance.Def();
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

	public static GameObject ExtendPlantToIrrigated(GameObject template, FertilizationMonitor.FertilizerInfo[] fertilizers)
	{
		IrrigationMonitorInstance.Def def = new IrrigationMonitorInstance.Def();
		def.consumedElements = fertilizers;
		template.GetComponent<StateMachineController>().AddDef(def);
		return template;
	}

	public static GameObject ExtendPlantWithYield(GameObject template, IYieldEffect[] midYieldEffects, IYieldEffect[] highYieldEffects)
	{
		KPrefabID component = template.GetComponent<KPrefabID>();
		Crop component2 = template.GetComponent<Crop>();
		component2.SetYieldModifiers(midYieldEffects, highYieldEffects);
		component.prefabInitFn += delegate(GameObject inst)
		{
			Crop component3 = inst.GetComponent<Crop>();
			component3.SetYieldModifiers(midYieldEffects, highYieldEffects);
		};
		return template;
	}

	public static GameObject CreateAndRegisterSeedForPlant(GameObject plant, SeedProducer.ProductionType productionType, string id, string name, string desc, KAnimFile anim, string initialAnim = "object", int numberOfSeeds = 1, List<Tag> additionalTags = null, SingleEntityReceptacle.ReceptacleDirection planterDirection = SingleEntityReceptacle.ReceptacleDirection.Top, [Optional] Tag replantGroundTag, int sortOrder = 0, string domesticatedDescription = "", EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE, float width = 0.25f, float height = 0.25f)
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, true, anim, initialAnim, Grid.SceneLayer.Front, collisionShape, width, height, true, SimHashes.Creature, null);
		gameObject.UpdateComponentRequirement<EntitySplitter>(true);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.CountableUnits = true;
		PlantableSeed plantableSeed = gameObject.UpdateComponentRequirement<PlantableSeed>(true);
		plantableSeed.PlantID = new Tag(plant.name);
		plantableSeed.replantGroundTag = replantGroundTag;
		plantableSeed.sortOrder = sortOrder;
		plantableSeed.domesticatedDescription = domesticatedDescription;
		plantableSeed.direction = planterDirection;
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		component2.AddPrefabTag(GameTags.Seed);
		if (additionalTags != null)
		{
			component2.AddPrefabTags(additionalTags);
		}
		KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
		Assets.AddPrefab(component3);
		SeedProducer seedProducer = plant.UpdateComponentRequirement<SeedProducer>(true);
		seedProducer.Configure(gameObject.name, productionType, numberOfSeeds);
		return gameObject;
	}

	public static GameObject CreateAndRegisterPreviewForPlant(GameObject plant, GameObject seed, string id, KAnimFile anim, string initialAnim, int width, int height)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, id, id, 1f, anim, initialAnim, Grid.SceneLayer.Front, width, height, BUILDINGS.DECOR.NONE, SimHashes.Creature, null);
		gameObject.UpdateComponentRequirement<KSelectable>(false);
		gameObject.UpdateComponentRequirement<SaveLoadRoot>(false);
		gameObject.UpdateComponentRequirement<SavedObject>(false);
		gameObject.UpdateComponentRequirement<PlantPreview>(true);
		OccupyArea occupyArea = gameObject.UpdateComponentRequirement<OccupyArea>(true);
		occupyArea.objectLayer = ObjectLayer.Building;
		occupyArea.ApplyToCells = false;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		Assets.AddPrefab(component);
		PlantableSeed component2 = seed.GetComponent<PlantableSeed>();
		component2.PreviewID = TagManager.Create(id, null);
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

	public enum CollisionShape
	{
		CIRCLE,
		RECTANGLE,
		POLYGONAL
	}
}
