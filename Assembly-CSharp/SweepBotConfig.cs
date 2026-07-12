using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class SweepBotConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = "SweepBot";
		string text2 = this.name;
		string text3 = this.desc;
		float mass = SweepBotConfig.MASS;
		EffectorValues none = global::TUNING.BUILDINGS.DECOR.NONE;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, mass, Assets.GetAnim("sweep_bot_kanim"), "idle", Grid.SceneLayer.Creatures, 1, 1, none, default(EffectorValues), SimHashes.Creature, null, 293f);
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.GetComponent<KBatchedAnimController>().isMovable = true;
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		kprefabID.AddTag(GameTags.Creature, false);
		gameObject.AddComponent<Pickupable>();
		gameObject.AddOrGet<Clearable>().isClearable = false;
		Trait trait = Db.Get().CreateTrait("SweepBotBaseTrait", this.name, this.name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.maxAttribute.Id, 9000f, this.name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.deltaAttribute.Id, -17.142857f, this.name, false, false, true));
		Modifiers modifiers = gameObject.AddOrGet<Modifiers>();
		modifiers.initialTraits.Add("SweepBotBaseTrait");
		modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.InternalBattery.Id);
		gameObject.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_pivot", false);
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGetDef<AnimInterruptMonitor.Def>();
		gameObject.AddOrGetDef<StorageUnloadMonitor.Def>();
		RobotBatteryMonitor.Def def = gameObject.AddOrGetDef<RobotBatteryMonitor.Def>();
		def.batteryAmountId = Db.Get().Amounts.InternalBattery.Id;
		def.canCharge = true;
		def.lowBatteryWarningPercent = 0.5f;
		gameObject.AddOrGetDef<SweepBotReactMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<SweepBotTrappedMonitor.Def>();
		gameObject.AddOrGet<AnimEventHandler>();
		gameObject.AddOrGet<SnapOn>().snapPoints = new List<SnapOn.SnapPoint>(new SnapOn.SnapPoint[]
		{
			new SnapOn.SnapPoint
			{
				pointName = "carry",
				automatic = false,
				context = "",
				buildFile = null,
				overrideSymbol = "snapTo_ornament"
			}
		});
		SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		gameObject.AddComponent<Storage>();
		Storage storage = gameObject.AddComponent<Storage>();
		storage.capacityKg = 500f;
		storage.storageFXOffset = new Vector3(0f, 0.5f, 0f);
		gameObject.AddOrGet<OrnamentReceptacle>().AddDepositTag(GameTags.PedestalDisplayable);
		gameObject.AddOrGet<DecorProvider>();
		gameObject.AddOrGet<UserNameable>();
		gameObject.AddOrGet<CharacterOverlay>();
		gameObject.AddOrGet<ItemPedestal>();
		Navigator navigator = gameObject.AddOrGet<Navigator>();
		navigator.NavGridName = "WalkerBabyNavGrid";
		navigator.CurrentNavType = NavType.Floor;
		navigator.defaultSpeed = 1f;
		navigator.updateProber = true;
		navigator.maxProbingRadius = 32;
		navigator.sceneLayer = Grid.SceneLayer.Creatures;
		kprefabID.AddTag(GameTags.Creatures.Walker, false);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new FallStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new SweepBotTrappedStates.Def(), true, -1)
			.Add(new DeliverToSweepLockerStates.Def(), true, -1)
			.Add(new ReturnToChargeStationStates.Def(), true, -1)
			.Add(new SweepStates.Def(), true, -1)
			.Add(new IdleStates.Def(), true, -1);
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Robots.Models.SweepBot, null);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		StorageUnloadMonitor.Instance smi = inst.GetSMI<StorageUnloadMonitor.Instance>();
		smi.sm.internalStorage.Set(inst.GetComponents<Storage>()[1], smi);
		inst.GetComponent<OrnamentReceptacle>();
		inst.GetSMI<CreatureFallMonitor.Instance>().anim = "idle_loop";
	}

	public const string ID = "SweepBot";

	public const string BASE_TRAIT_ID = "SweepBotBaseTrait";

	public const float STORAGE_CAPACITY = 500f;

	public const float BATTERY_CAPACITY = 9000f;

	public const float BATTERY_DEPLETION_RATE = 17.142857f;

	public const float MAX_SWEEP_AMOUNT = 10f;

	public const float MOP_SPEED = 10f;

	private string name = global::STRINGS.ROBOTS.MODELS.SWEEPBOT.NAME;

	private string desc = global::STRINGS.ROBOTS.MODELS.SWEEPBOT.DESC;

	public static float MASS = 25f;
}
