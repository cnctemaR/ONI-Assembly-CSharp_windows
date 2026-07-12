using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MinionConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME;
		GameObject gameObject = EntityTemplates.CreateEntity(MinionConfig.ID, text, true);
		gameObject.AddOrGet<StateMachineController>();
		MinionModifiers minionModifiers = gameObject.AddOrGet<MinionModifiers>();
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGet<AttributeLevels>();
		gameObject.AddOrGet<AttributeConverters>();
		MinionConfig.AddMinionAmounts(minionModifiers);
		MinionConfig.AddMinionTraits(text, minionModifiers);
		gameObject.AddOrGet<MinionBrain>();
		gameObject.AddOrGet<KPrefabID>().AddTag(GameTags.DupeBrain, false);
		gameObject.AddOrGet<Worker>();
		gameObject.AddOrGet<ChoreConsumer>();
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.fxPrefix = Storage.FXPrefix.PickedUp;
		storage.dropOnLoad = true;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Seal
		});
		gameObject.AddTag(GameTags.CorrosionProof);
		gameObject.AddOrGet<Health>();
		OxygenBreather oxygenBreather = gameObject.AddOrGet<OxygenBreather>();
		oxygenBreather.O2toCO2conversion = 0.02f;
		oxygenBreather.lowOxygenThreshold = 0.52f;
		oxygenBreather.noOxygenThreshold = 0.05f;
		oxygenBreather.mouthOffset = new Vector2f(0.25f, 0.97f);
		oxygenBreather.minCO2ToEmit = 0.02f;
		oxygenBreather.breathableCells = OxygenBreather.DEFAULT_BREATHABLE_OFFSETS;
		gameObject.AddOrGet<WarmBlooded>();
		gameObject.AddOrGet<MinionIdentity>();
		GridVisibility gridVisibility = gameObject.AddOrGet<GridVisibility>();
		gridVisibility.radius = 30;
		gridVisibility.innerRadius = 20f;
		gameObject.AddOrGet<MiningSounds>();
		gameObject.AddOrGet<SaveLoadRoot>();
		MoverLayerOccupier moverLayerOccupier = gameObject.AddOrGet<MoverLayerOccupier>();
		moverLayerOccupier.objectLayers = new ObjectLayer[]
		{
			ObjectLayer.Minion,
			ObjectLayer.Mover
		};
		moverLayerOccupier.cellOffsets = new CellOffset[]
		{
			CellOffset.none,
			new CellOffset(0, 1)
		};
		Navigator navigator = gameObject.AddOrGet<Navigator>();
		navigator.NavGridName = MinionConfig.MINION_NAV_GRID_NAME;
		navigator.CurrentNavType = NavType.Floor;
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.isMovable = true;
		kbatchedAnimController.sceneLayer = Grid.SceneLayer.Move;
		kbatchedAnimController.AnimFiles = new KAnimFile[]
		{
			Assets.GetAnim("body_comp_default_kanim"),
			Assets.GetAnim("anim_construction_default_kanim"),
			Assets.GetAnim("anim_idles_default_kanim"),
			Assets.GetAnim("anim_loco_firepole_kanim"),
			Assets.GetAnim("anim_loco_new_kanim"),
			Assets.GetAnim("anim_loco_tube_kanim"),
			Assets.GetAnim("anim_construction_firepole_kanim"),
			Assets.GetAnim("anim_construction_jetsuit_kanim")
		};
		KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kboxCollider2D.offset = new Vector2(0f, 0.75f);
		kboxCollider2D.size = new Vector2(1f, 1.5f);
		gameObject.AddOrGet<SnapOn>().snapPoints = new List<SnapOn.SnapPoint>(new SnapOn.SnapPoint[]
		{
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "dig",
				buildFile = Assets.GetAnim("excavator_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "build",
				buildFile = Assets.GetAnim("constructor_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "fetchliquid",
				buildFile = Assets.GetAnim("water_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "paint",
				buildFile = Assets.GetAnim("painting_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "harvest",
				buildFile = Assets.GetAnim("plant_harvester_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "capture",
				buildFile = Assets.GetAnim("net_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "attack",
				buildFile = Assets.GetAnim("attack_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "pickup",
				buildFile = Assets.GetAnim("pickupdrop_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "store",
				buildFile = Assets.GetAnim("pickupdrop_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "disinfect",
				buildFile = Assets.GetAnim("plant_spray_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "tend",
				buildFile = Assets.GetAnim("plant_harvester_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "carry",
				automatic = false,
				context = "",
				buildFile = null,
				overrideSymbol = "snapTo_chest"
			},
			new SnapOn.SnapPoint
			{
				pointName = "build",
				automatic = false,
				context = "",
				buildFile = null,
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "remote",
				automatic = false,
				context = "",
				buildFile = null,
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "snapTo_neck",
				automatic = false,
				context = "",
				buildFile = Assets.GetAnim("helm_oxygen_kanim"),
				overrideSymbol = "snapTo_neck"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "powertinker",
				buildFile = Assets.GetAnim("electrician_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "specialistdig",
				buildFile = Assets.GetAnim("excavator_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "mask_oxygen",
				automatic = false,
				context = "",
				buildFile = Assets.GetAnim("mask_oxygen_kanim"),
				overrideSymbol = "snapTo_goggles"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = "demolish",
				buildFile = Assets.GetAnim("poi_demolish_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			}
		});
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.InternalTemperature = 310.15f;
		primaryElement.MassPerUnit = 30f;
		primaryElement.ElementID = SimHashes.Creature;
		gameObject.AddOrGet<ChoreProvider>();
		gameObject.AddOrGetDef<DebugGoToMonitor.Def>();
		gameObject.AddOrGet<Sensors>();
		gameObject.AddOrGet<Chattable>();
		gameObject.AddOrGet<FaceGraph>();
		gameObject.AddOrGet<Accessorizer>();
		gameObject.AddOrGet<Schedulable>();
		gameObject.AddOrGet<LoopingSounds>().updatePosition = true;
		gameObject.AddOrGet<AnimEventHandler>();
		gameObject.AddOrGet<FactionAlignment>().Alignment = FactionManager.FactionID.Duplicant;
		gameObject.AddOrGet<Weapon>();
		gameObject.AddOrGet<RangedAttackable>();
		gameObject.AddOrGet<CharacterOverlay>().shouldShowName = true;
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.objectLayers = new ObjectLayer[1];
		occupyArea.ApplyToCells = false;
		occupyArea.OccupiedCellsOffsets = new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		};
		gameObject.AddOrGet<Pickupable>();
		CreatureSimTemperatureTransfer creatureSimTemperatureTransfer = gameObject.AddOrGet<CreatureSimTemperatureTransfer>();
		creatureSimTemperatureTransfer.SurfaceArea = 10f;
		creatureSimTemperatureTransfer.Thickness = 0.01f;
		gameObject.AddOrGet<SicknessTrigger>();
		gameObject.AddOrGet<ClothingWearer>();
		gameObject.AddOrGet<SuitEquipper>();
		gameObject.AddOrGet<DecorProvider>().baseRadius = 3f;
		gameObject.AddOrGet<ConsumableConsumer>();
		gameObject.AddOrGet<NoiseListener>();
		gameObject.AddOrGet<MinionResume>();
		DuplicantNoiseLevels.SetupNoiseLevels();
		this.SetupLaserEffects(gameObject);
		SymbolOverrideControllerUtil.AddToPrefab(gameObject).applySymbolOverridesEveryFrame = true;
		MinionConfig.ConfigureSymbols(gameObject);
		return gameObject;
	}

	private void SetupLaserEffects(GameObject prefab)
	{
		GameObject gameObject = new GameObject("LaserEffect");
		gameObject.transform.parent = prefab.transform;
		KBatchedAnimEventToggler kbatchedAnimEventToggler = gameObject.AddComponent<KBatchedAnimEventToggler>();
		kbatchedAnimEventToggler.eventSource = prefab;
		kbatchedAnimEventToggler.enableEvent = "LaserOn";
		kbatchedAnimEventToggler.disableEvent = "LaserOff";
		kbatchedAnimEventToggler.entries = new List<KBatchedAnimEventToggler.Entry>();
		MinionConfig.LaserEffect[] array = new MinionConfig.LaserEffect[]
		{
			new MinionConfig.LaserEffect
			{
				id = "DigEffect",
				animFile = "laser_kanim",
				anim = "idle",
				context = "dig"
			},
			new MinionConfig.LaserEffect
			{
				id = "BuildEffect",
				animFile = "construct_beam_kanim",
				anim = "loop",
				context = "build"
			},
			new MinionConfig.LaserEffect
			{
				id = "FetchLiquidEffect",
				animFile = "hose_fx_kanim",
				anim = "loop",
				context = "fetchliquid"
			},
			new MinionConfig.LaserEffect
			{
				id = "PaintEffect",
				animFile = "paint_beam_kanim",
				anim = "loop",
				context = "paint"
			},
			new MinionConfig.LaserEffect
			{
				id = "HarvestEffect",
				animFile = "plant_harvest_beam_kanim",
				anim = "loop",
				context = "harvest"
			},
			new MinionConfig.LaserEffect
			{
				id = "CaptureEffect",
				animFile = "net_gun_fx_kanim",
				anim = "loop",
				context = "capture"
			},
			new MinionConfig.LaserEffect
			{
				id = "AttackEffect",
				animFile = "attack_beam_fx_kanim",
				anim = "loop",
				context = "attack"
			},
			new MinionConfig.LaserEffect
			{
				id = "PickupEffect",
				animFile = "vacuum_fx_kanim",
				anim = "loop",
				context = "pickup"
			},
			new MinionConfig.LaserEffect
			{
				id = "StoreEffect",
				animFile = "vacuum_reverse_fx_kanim",
				anim = "loop",
				context = "store"
			},
			new MinionConfig.LaserEffect
			{
				id = "DisinfectEffect",
				animFile = "plant_spray_beam_kanim",
				anim = "loop",
				context = "disinfect"
			},
			new MinionConfig.LaserEffect
			{
				id = "TendEffect",
				animFile = "plant_tending_beam_fx_kanim",
				anim = "loop",
				context = "tend"
			},
			new MinionConfig.LaserEffect
			{
				id = "PowerTinkerEffect",
				animFile = "electrician_beam_fx_kanim",
				anim = "idle",
				context = "powertinker"
			},
			new MinionConfig.LaserEffect
			{
				id = "SpecialistDigEffect",
				animFile = "senior_miner_beam_fx_kanim",
				anim = "idle",
				context = "specialistdig"
			},
			new MinionConfig.LaserEffect
			{
				id = "DemolishEffect",
				animFile = "poi_demolish_fx_kanim",
				anim = "idle",
				context = "demolish"
			}
		};
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		foreach (MinionConfig.LaserEffect laserEffect in array)
		{
			GameObject gameObject2 = new GameObject(laserEffect.id);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.AddOrGet<KPrefabID>().PrefabTag = new Tag(laserEffect.id);
			KBatchedAnimTracker kbatchedAnimTracker = gameObject2.AddOrGet<KBatchedAnimTracker>();
			kbatchedAnimTracker.controller = component;
			kbatchedAnimTracker.symbol = new HashedString("snapTo_rgtHand");
			kbatchedAnimTracker.offset = new Vector3(195f, -35f, 0f);
			kbatchedAnimTracker.useTargetPoint = true;
			KBatchedAnimController kbatchedAnimController = gameObject2.AddOrGet<KBatchedAnimController>();
			kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim(laserEffect.animFile) };
			KBatchedAnimEventToggler.Entry entry = new KBatchedAnimEventToggler.Entry
			{
				anim = laserEffect.anim,
				context = laserEffect.context,
				controller = kbatchedAnimController
			};
			kbatchedAnimEventToggler.entries.Add(entry);
			gameObject2.AddOrGet<LoopingSounds>();
		}
	}

	public void OnPrefabInit(GameObject go)
	{
		AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(go);
		amountInstance.value = amountInstance.GetMax();
		Db.Get().Amounts.Bladder.Lookup(go).value = global::UnityEngine.Random.Range(0f, 10f);
		Db.Get().Amounts.Stress.Lookup(go).value = 5f;
		Db.Get().Amounts.Temperature.Lookup(go).value = 310.15f;
		AmountInstance amountInstance2 = Db.Get().Amounts.Stamina.Lookup(go);
		amountInstance2.value = amountInstance2.GetMax();
		AmountInstance amountInstance3 = Db.Get().Amounts.Breath.Lookup(go);
		amountInstance3.value = amountInstance3.GetMax();
		AmountInstance amountInstance4 = Db.Get().Amounts.Calories.Lookup(go);
		amountInstance4.value = 0.8875f * amountInstance4.GetMax();
	}

	public void OnSpawn(GameObject go)
	{
		Sensors component = go.GetComponent<Sensors>();
		component.Add(new PathProberSensor(component));
		component.Add(new SafeCellSensor(component));
		component.Add(new IdleCellSensor(component));
		component.Add(new PickupableSensor(component));
		component.Add(new ClosestEdibleSensor(component));
		component.Add(new BreathableAreaSensor(component));
		component.Add(new AssignableReachabilitySensor(component));
		component.Add(new ToiletSensor(component));
		component.Add(new MingleCellSensor(component));
		component.Add(new BalloonStandCellSensor(component));
		new RationalAi.Instance(go.GetComponent<StateMachineController>()).StartSM();
		if (go.GetComponent<OxygenBreather>().GetGasProvider() == null)
		{
			go.GetComponent<OxygenBreather>().SetGasProvider(new GasBreatherFromWorldProvider());
		}
		Navigator component2 = go.GetComponent<Navigator>();
		component2.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(component2, 3.325f, 2.5f));
		component2.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(component2));
		component2.transitionDriver.overrideLayers.Add(new TubeTransitionLayer(component2));
		component2.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(component2));
		component2.transitionDriver.overrideLayers.Add(new ReactableTransitionLayer(component2));
		component2.transitionDriver.overrideLayers.Add(new NavTeleportTransitionLayer(component2));
		component2.transitionDriver.overrideLayers.Add(new SplashTransitionLayer(component2));
		ThreatMonitor.Instance smi = go.GetSMI<ThreatMonitor.Instance>();
		if (smi != null)
		{
			smi.def.fleethresholdState = Health.HealthState.Critical;
		}
	}

	public static void AddMinionAmounts(Modifiers modifiers)
	{
		modifiers.initialAttributes.Add(Db.Get().Attributes.AirConsumptionRate.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.MaxUnderwaterTravelCost.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.DecorExpectation.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.FoodExpectation.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.ToiletEfficiency.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.RoomTemperaturePreference.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.CarryAmount.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.QualityOfLife.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.SpaceNavigation.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.Sneezyness.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.RadiationResistance.Id);
		modifiers.initialAttributes.Add(Db.Get().Attributes.RadiationRecovery.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Stamina.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Calories.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.ImmuneLevel.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Breath.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Stress.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Toxicity.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Bladder.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Temperature.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.ExternalTemperature.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Decor.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.RadiationBalance.Id);
	}

	public static void AddMinionTraits(string name, Modifiers modifiers)
	{
		Trait trait = Db.Get().CreateTrait(MinionConfig.MINION_BASE_TRAIT_ID, name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Attributes.AirConsumptionRate.Id, 0.1f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.MaxUnderwaterTravelCost.Id, 8f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.ToiletEfficiency.Id, 1f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.RoomTemperaturePreference.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.QualityOfLife.Id, 1f, name, false, false, true));
		if (!DlcManager.IsExpansion1Active())
		{
			trait.Add(new AttributeModifier(Db.Get().Attributes.SpaceNavigation.Id, 1f, name, false, false, true));
		}
		trait.Add(new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, -0.11666667f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1666.6666f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, 4000000f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Toxicity.deltaAttribute.Id, 0f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, 0.16666667f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 100f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.025f, name, false, false, true));
		modifiers.initialTraits.Add(MinionConfig.MINION_BASE_TRAIT_ID);
	}

	public static void ConfigureSymbols(GameObject go)
	{
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity("snapto_hat", false);
		component.SetSymbolVisiblity("snapTo_hat_hair", false);
		component.SetSymbolVisiblity("snapTo_headfx", false);
		component.SetSymbolVisiblity("snapto_chest", false);
		component.SetSymbolVisiblity("snapto_neck", false);
		component.SetSymbolVisiblity("snapto_goggles", false);
		component.SetSymbolVisiblity("snapto_pivot", false);
		component.SetSymbolVisiblity("snapTo_rgtHand", false);
	}

	public static string ID = "Minion";

	public static string MINION_BASE_TRAIT_ID = MinionConfig.ID + "BaseTrait";

	public static string MINION_NAV_GRID_NAME = "MinionNavGrid";

	public const int MINION_BASE_SYMBOL_LAYER = 0;

	public const int MINION_HAIR_ALWAYS_HACK_LAYER = 1;

	public const int MINION_EXPRESSION_SYMBOL_LAYER = 2;

	public const int MINION_MOUTH_FLAP_LAYER = 3;

	public const int MINION_CLOTHING_SYMBOL_LAYER = 4;

	public const int MINION_PICKUP_SYMBOL_LAYER = 5;

	public const int MINION_SUIT_SYMBOL_LAYER = 6;

	public struct LaserEffect
	{
		public string id;

		public string animFile;

		public string anim;

		public HashedString context;
	}
}
