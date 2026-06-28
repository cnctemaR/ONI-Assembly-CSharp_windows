using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class EntityTemplates
{
	public static GameObject CreateBasicEntity(string id, string name, string desc, float mass, string anim, string initialAnim, Grid.SceneLayer sceneLayer)
	{
		GameObject gameObject = new GameObject("EntityTemplate");
		gameObject.SetActive(false);
		gameObject.name = id;
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.EntityPrefabs).transform;
		KPrefabID kprefabID = gameObject.UpdateComponentRequirement<KPrefabID>(true);
		kprefabID.PrefabTag = TagManager.Create(id, null);
		KSelectable kselectable = gameObject.UpdateComponentRequirement<KSelectable>(true);
		kselectable.SetName(name);
		gameObject.UpdateComponentRequirement<SaveLoadRoot>(true);
		gameObject.UpdateComponentRequirement<SavedObject>(true);
		gameObject.UpdateComponentRequirement<StateMachineController>(true);
		KBatchedAnimController kbatchedAnimController = gameObject.UpdateComponentRequirement<KBatchedAnimController>(true);
		kbatchedAnimController.AddAnims(new KAnimFile[] { Assets.GetAnim(anim) });
		kbatchedAnimController.sceneLayer = sceneLayer;
		kbatchedAnimController.initialAnim = initialAnim;
		PrimaryElement primaryElement = gameObject.UpdateComponentRequirement<PrimaryElement>(true);
		primaryElement.ElementID = SimHashes.Carbon;
		primaryElement.Temperature = 293f;
		primaryElement.Mass = mass;
		gameObject.UpdateComponentRequirement<SimTemperatureTransfer>(true);
		InfoDescription infoDescription = gameObject.UpdateComponentRequirement<InfoDescription>(true);
		infoDescription.description = desc;
		gameObject.UpdateComponentRequirement<InfraredVisualizer>(true);
		return gameObject;
	}

	public static GameObject CreatePlacedEntity(string id, string name, string desc, float mass, string anim, string initialAnim, Grid.SceneLayer sceneLayer, int width, int height, DecorValues decor)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, mass, anim, initialAnim, sceneLayer);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.defaultSpawnOffset = CellAlignment.Bottom;
		BoxCollider2D boxCollider2D = gameObject.UpdateComponentRequirement<BoxCollider2D>(true);
		boxCollider2D.size = new Vector2f(width, height);
		boxCollider2D.offset = new Vector2f(0f, (float)height / 2f);
		OccupyArea occupyArea = gameObject.UpdateComponentRequirement<OccupyArea>(true);
		occupyArea.OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(width, height);
		occupyArea.objectLayer = ObjectLayer.NumLayers;
		gameObject.UpdateComponentRequirement<Modifiers>(true);
		DecorProvider decorProvider = gameObject.UpdateComponentRequirement<DecorProvider>(true);
		decorProvider.SetValues(decor);
		return gameObject;
	}

	public static GameObject ExtendEntityToBasicPlant(GameObject template, bool two_tiles_tall, float drowning_stamina = 15f, float drowning_regen = 5f, float temperature_lethal_low = 273f, float temperature_warning_low = 283f, float temperature_warning_high = 303f, float temperature_lethal_high = 315f, float pressure_lethal_low = 0f, float pressure_warning_low = 0.15f, float grow_time = 1f, string crop_id = null)
	{
		EntombVulnerable entombVulnerable = template.UpdateComponentRequirement<EntombVulnerable>(true);
		entombVulnerable.Configure(two_tiles_tall);
		PressureVulnerable pressureVulnerable = template.UpdateComponentRequirement<PressureVulnerable>(true);
		pressureVulnerable.Configure(pressure_lethal_low, pressure_warning_low);
		template.UpdateComponentRequirement<WiltCondition>(true);
		template.UpdateComponentRequirement<Uprootable>(true);
		template.UpdateComponentRequirement<UprootedMonitor>(true);
		DrowningMonitor drowningMonitor = template.UpdateComponentRequirement<DrowningMonitor>(true);
		drowningMonitor.Configure(drowning_stamina, drowning_regen, 0.95f);
		TemperatureVulnerable temperatureVulnerable = template.UpdateComponentRequirement<TemperatureVulnerable>(true);
		temperatureVulnerable.Configure(temperature_warning_low, temperature_lethal_low, temperature_warning_high, temperature_lethal_high, 0.3f);
		template.UpdateComponentRequirement<OccupyArea>(true).objectLayer = ObjectLayer.Building;
		Growing growing = template.UpdateComponentRequirement<Growing>(true);
		growing.Configure(grow_time, grow_time);
		if (crop_id != null)
		{
			CROPS.CropVal cropVal = CROPS.CROP_TYPES.Find((CROPS.CropVal m) => m.crop_id == crop_id);
			Crop crop = template.UpdateComponentRequirement<Crop>(true);
			crop.Configure(cropVal);
			growing.Configure(cropVal.crop_duration, cropVal.regrow_duration);
			template.UpdateComponentRequirement<Harvestable>(true);
			template.UpdateComponentRequirement<Prioritizable>(true);
		}
		return template;
	}

	public static GameObject ExtendEntityToBasicCreature(GameObject template, bool two_tiles_tall = true, FactionManager.FactionID faction = FactionManager.FactionID.Prey, float HitPoints = 100f, string NavGridName = "HatchNavGrid", NavType navType = NavType.Floor, float moveSpeed = 2f, string onDeathDropID = "Meat", int onDeathDropCount = 1, bool drownVulnerable = true, bool entombVulnerable = true, float drowningStamina = 30f, float warningLowTemperature = 283f, float warningHighTemperature = 294f, float lethalLowTemperature = 273f, float lethalHighTemperature = 315f)
	{
		template.GetComponent<KBatchedAnimController>().isMovable = true;
		template.UpdateComponentRequirement<Health>(true).SetMaxHitPoints(HitPoints);
		template.UpdateComponentRequirement<CharacterOverlay>(true);
		template.UpdateComponentRequirement<RangedAttackable>(true);
		template.UpdateComponentRequirement<FactionAlignment>(true).Alignment = faction;
		template.UpdateComponentRequirement<Prioritizable>(true);
		template.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(warningLowTemperature, lethalLowTemperature, warningHighTemperature, lethalHighTemperature, 0.3f);
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
		return template;
	}

	public static GameObject CreateLooseEntity(string id, string name, string desc, float mass, string anim, string initialAnim, Grid.SceneLayer sceneLayer, EntityTemplates.CollisionShape collisionShape, float width = 1f, float height = 1f, bool isPickupable = false)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, mass, anim, initialAnim, sceneLayer);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.defaultSpawnOffset = CellAlignment.RandomInternal;
		if (collisionShape != EntityTemplates.CollisionShape.RECTANGLE)
		{
			if (collisionShape != EntityTemplates.CollisionShape.POLYGONAL)
			{
				CircleCollider2D circleCollider2D = gameObject.UpdateComponentRequirement<CircleCollider2D>(true);
				circleCollider2D.radius = width;
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
		if (isPickupable)
		{
			Pickupable pickupable = gameObject.UpdateComponentRequirement<Pickupable>(true);
			pickupable.SetWorkTime(5f);
		}
		return gameObject;
	}

	public static GameObject ExtendEntityToFood(GameObject template, EdiblesManager.FoodInfo foodInfo)
	{
		Edible edible = template.UpdateComponentRequirement<Edible>(true);
		edible.FoodInfo = foodInfo;
		template.UpdateComponentRequirement<EntitySplitter>(true);
		KPrefabID component = template.GetComponent<KPrefabID>();
		component.instantiateFn += delegate(GameObject go)
		{
			go.GetComponent<Edible>().FoodInfo = foodInfo;
		};
		component.prefabSpawnFn += delegate(GameObject inst)
		{
			KPrefabID component2 = inst.GetComponent<KPrefabID>();
			Rottable.Instance instance = new Rottable.Instance(component2, foodInfo);
			instance.StartSM();
		};
		return template;
	}

	public static GameObject ExtendEntityToMedicine(GameObject template, MedicineInfo medicineInfo)
	{
		MedicinalPill medicinalPill = template.UpdateComponentRequirement<MedicinalPill>(true);
		medicinalPill.curedDiseases = medicineInfo.curedDiseases;
		medicinalPill.boostMultipliers = medicineInfo.boostMultipliers;
		medicinalPill.medicineType = medicineInfo.medicineType;
		return template;
	}

	public static GameObject ExtendPlantToFertilizable(GameObject template, ElementConverter.ConsumedElement[] fertilizers)
	{
		foreach (ElementConverter.ConsumedElement consumedElement in fertilizers)
		{
			ManualDeliveryKG manualDeliveryKG = template.UpdateComponentRequirement<ManualDeliveryKG>(true);
			manualDeliveryKG.RequestedItemTag = consumedElement.tag;
			manualDeliveryKG.Pause(true, "init");
		}
		KPrefabID component = template.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			Amounts amounts = inst.GetAmounts();
			amounts.Add(new AmountInstance(Db.Get().Amounts.Fertilization, inst));
		};
		component.prefabSpawnFn += delegate(GameObject inst)
		{
			KPrefabID component3 = inst.GetComponent<KPrefabID>();
			FertilizationMonitor.Instance instance = new FertilizationMonitor.Instance(component3, fertilizers);
			instance.StartSM();
		};
		Crop component2 = template.GetComponent<Crop>();
		if (component2 != null)
		{
			component2.EffectDescription.Add(UI.UISIDESCREENS.PLANTERSIDESCREEN.REQUIRESFERTILIZER);
		}
		return template;
	}

	public static GameObject CreateAndRegisterSeedForPlant(GameObject plant, string id, string name, string desc, string anim, List<Tag> additionalTags = null, [Optional] Tag replantGroundTag, int sortOrder = 0, string domesticatedDescription = "")
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, anim, "idle", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 1f, true);
		gameObject.UpdateComponentRequirement<EntitySplitter>(true);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.WholeUnitsOnly = true;
		PlantableSeed plantableSeed = gameObject.UpdateComponentRequirement<PlantableSeed>(true);
		plantableSeed.PlantID = new Tag(plant.name);
		plantableSeed.replantGroundTag = replantGroundTag;
		plantableSeed.sortOrder = sortOrder;
		plantableSeed.domesticatedDescription = domesticatedDescription;
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		component2.AddPrefabTag(GameTags.Seed);
		component2.AddPrefabTags(additionalTags);
		KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
		Assets.AddPrefab(component3);
		SeedProducer seedProducer = plant.UpdateComponentRequirement<SeedProducer>(true);
		seedProducer.Configure(gameObject.name);
		return gameObject;
	}

	private static CellOffset[] GenerateOffsets(int width, int height)
	{
		List<CellOffset> list = new List<CellOffset>();
		list.Add(new CellOffset(0, 0));
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (i != 0 || j != 0)
				{
					int num = (j / 2 - j) * ((j + 1) % 2) + j % 2 * (j / 2 + j % 2);
					CellOffset cellOffset = new CellOffset(num, i);
					list.Add(cellOffset);
				}
			}
		}
		return list.ToArray();
	}

	public static void SetDescriptionOrder(GameObject go)
	{
		IGameObjectEffectDescriptor[] components = go.GetComponents<IGameObjectEffectDescriptor>();
		foreach (IGameObjectEffectDescriptor gameObjectEffectDescriptor in components)
		{
			int num = 9999;
			gameObjectEffectDescriptor.DescriptionOrder = num;
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)gameObjectEffectDescriptor;
			string name = kmonoBehaviour.GetType().Name;
			for (int j = 0; j < global::TUNING.BUILDINGS.GAMEOBJECT_COMPONENT_DESCRIPTION_ORDER.Length; j++)
			{
				if (global::TUNING.BUILDINGS.GAMEOBJECT_COMPONENT_DESCRIPTION_ORDER[j] == name)
				{
					gameObjectEffectDescriptor.DescriptionOrder = j;
					break;
				}
			}
			if (gameObjectEffectDescriptor.DescriptionOrder == num)
			{
				Debug.LogWarning("Missing Effect Descriptor Order: " + name);
			}
		}
	}

	public enum CollisionShape
	{
		CIRCLE,
		RECTANGLE,
		POLYGONAL
	}
}
