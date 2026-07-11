using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MinionConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME;
		GameObject gameObject = EntityTemplates.CreateEntity(MinionConfig.ID, text, true);
		gameObject.AddOrGet<StateMachineController>();
		MinionModifiers minionModifiers = gameObject.AddOrGet<MinionModifiers>();
		MinionConfig.AddMinionAmounts(minionModifiers);
		Trait trait = Db.Get().CreateTrait(MinionConfig.MINION_BASE_TRAIT_ID, text, text, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, -0.11666667f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1666.6666f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, 4000000f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Toxicity.deltaAttribute.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.AirConsumptionRate.Id, 0.1f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, 0.16666667f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 100f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.MaxUnderwaterTravelCost.Id, 8f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.ToiletEfficiency.Id, 1f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.RoomTemperaturePreference.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.QualityOfLife.Id, 1f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.025f, text, false, false, true));
		gameObject.AddOrGet<MinionBrain>();
		gameObject.AddOrGet<KPrefabID>().AddTag(GameTags.DupeBrain);
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
		gameObject.AddOrGet<Health>();
		OxygenBreather oxygenBreather = gameObject.AddOrGet<OxygenBreather>();
		oxygenBreather.O2toCO2conversion = 0.02f;
		oxygenBreather.lowOxygenThreshold = 0.52f;
		oxygenBreather.noOxygenThreshold = 0.05f;
		oxygenBreather.mouthOffset = new Vector2f(0.25f, 0.7f);
		oxygenBreather.minCO2ToEmit = 0.02f;
		oxygenBreather.breathableCells = new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1),
			new CellOffset(1, 1),
			new CellOffset(-1, 1),
			new CellOffset(1, 0),
			new CellOffset(-1, 0)
		};
		gameObject.AddOrGet<WarmBlooded>();
		gameObject.AddOrGet<MinionIdentity>();
		GridVisibility gridVisibility = gameObject.AddOrGet<GridVisibility>();
		gridVisibility.radius = 30f;
		gridVisibility.innerRadius = 20f;
		gameObject.AddOrGet<MinionSounds>();
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<AntiCluster>();
		Navigator navigator = gameObject.AddOrGet<Navigator>();
		navigator.NavGridName = "MinionNavGrid";
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
			Assets.GetAnim("anim_loco_hover_kanim"),
			Assets.GetAnim("anim_construction_firepole_kanim")
		};
		KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kboxCollider2D.offset = new Vector2(0f, 0.8f);
		kboxCollider2D.size = new Vector2(1f, 1.5f);
		SnapOn snapOn = gameObject.AddOrGet<SnapOn>();
		snapOn.snapPoints = new List<SnapOn.SnapPoint>(new SnapOn.SnapPoint[]
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
				context = string.Empty,
				buildFile = null,
				overrideSymbol = "snapTo_chest"
			},
			new SnapOn.SnapPoint
			{
				pointName = "build",
				automatic = false,
				context = string.Empty,
				buildFile = null,
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "remote",
				automatic = false,
				context = string.Empty,
				buildFile = null,
				overrideSymbol = "snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "snapTo_neck",
				automatic = false,
				context = string.Empty,
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
			}
		});
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<AttributeLevels>();
		gameObject.AddOrGet<AttributeConverters>();
		gameObject.AddOrGet<Equipment>();
		gameObject.AddOrGet<Ownables>();
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.InternalTemperature = 310.15f;
		primaryElement.MassPerUnit = 30f;
		primaryElement.ElementID = SimHashes.Creature;
		gameObject.AddOrGet<ChoreProvider>();
		gameObject.AddOrGetDef<DebugGoToMonitor.Def>();
		gameObject.AddOrGetDef<SpeechMonitor.Def>();
		gameObject.AddOrGetDef<BlinkMonitor.Def>();
		gameObject.AddOrGetDef<ConversationMonitor.Def>();
		gameObject.AddOrGet<Sensors>();
		gameObject.AddOrGet<Chattable>();
		gameObject.AddOrGet<FaceGraph>();
		gameObject.AddOrGet<Accessorizer>();
		gameObject.AddOrGet<Schedulable>();
		LoopingSounds loopingSounds = gameObject.AddOrGet<LoopingSounds>();
		loopingSounds.updatePosition = true;
		gameObject.AddOrGet<AnimEventHandler>();
		FactionAlignment factionAlignment = gameObject.AddOrGet<FactionAlignment>();
		factionAlignment.Alignment = FactionManager.FactionID.Duplicant;
		gameObject.AddOrGet<Weapon>();
		gameObject.AddOrGet<RangedAttackable>();
		gameObject.AddOrGet<CharacterOverlay>();
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
		gameObject.AddOrGet<DiseaseTrigger>();
		gameObject.AddOrGet<ClothingWearer>();
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.baseRadius = 3f;
		decorProvider.isMovable = true;
		gameObject.AddOrGet<ConsumableConsumer>();
		gameObject.AddOrGet<NoiseListener>();
		gameObject.AddOrGet<MinionResume>();
		DuplicantNoiseLevels.SetupNoiseLevels();
		this.SetupLaserEffects(gameObject);
		SymbolOverrideController symbolOverrideController = SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		symbolOverrideController.applySymbolOverridesEveryFrame = true;
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
			}
		};
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		foreach (MinionConfig.LaserEffect laserEffect in array)
		{
			GameObject gameObject2 = new GameObject(laserEffect.id);
			gameObject2.transform.parent = gameObject.transform;
			KPrefabID kprefabID = gameObject2.AddOrGet<KPrefabID>();
			kprefabID.PrefabTag = new Tag(laserEffect.id);
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
		AmountInstance amountInstance2 = Db.Get().Amounts.Bladder.Lookup(go);
		amountInstance2.value = global::UnityEngine.Random.Range(0f, 10f);
		AmountInstance amountInstance3 = Db.Get().Amounts.Stress.Lookup(go);
		amountInstance3.value = 5f;
		AmountInstance amountInstance4 = Db.Get().Amounts.Temperature.Lookup(go);
		amountInstance4.value = 310.15f;
		AmountInstance amountInstance5 = Db.Get().Amounts.Stamina.Lookup(go);
		amountInstance5.value = amountInstance5.GetMax();
		AmountInstance amountInstance6 = Db.Get().Amounts.Breath.Lookup(go);
		amountInstance6.value = amountInstance6.GetMax();
		AmountInstance amountInstance7 = Db.Get().Amounts.Calories.Lookup(go);
		amountInstance7.value = 0.8875f * amountInstance7.GetMax();
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
		StateMachineController component2 = go.GetComponent<StateMachineController>();
		RationalAi.Instance instance = new RationalAi.Instance(component2);
		instance.StartSM();
		if (go.GetComponent<OxygenBreather>().GetGasProvider() == null)
		{
			go.GetComponent<OxygenBreather>().SetGasProvider(new GasBreatherFromWorldProvider());
		}
		Navigator component3 = go.GetComponent<Navigator>();
		component3.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(component3, 3.325f, 2.5f));
		component3.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(component3));
		component3.transitionDriver.overrideLayers.Add(new TubeTransitionLayer(component3));
		component3.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(component3));
		component3.transitionDriver.overrideLayers.Add(new ReactableTransitionLayer(component3));
		component3.transitionDriver.overrideLayers.Add(new SplashTransitionLayer(component3));
		ThreatMonitor.Instance smi = go.GetSMI<ThreatMonitor.Instance>();
		if (smi != null)
		{
			smi.def.fleethresholdState = Health.HealthState.Critical;
		}
	}

	public static void AddMinionAmounts(Modifiers modifiers)
	{
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
	}

	public static void ConfigureSymbols(GameObject go)
	{
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity("snapto_hat", false);
		component.SetSymbolVisiblity("snapTo_hat_hair", false);
		component.SetSymbolVisiblity("snapto_chest", false);
		component.SetSymbolVisiblity("snapto_neck", false);
		component.SetSymbolVisiblity("snapto_goggles", false);
		component.SetSymbolVisiblity("snapto_pivot", false);
		component.SetSymbolVisiblity("snapTo_rgtHand", false);
	}

	public static string ID = "Minion";

	public static string MINION_BASE_TRAIT_ID = MinionConfig.ID + "BaseTrait";

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
