using System;
using STRINGS;
using UnityEngine;

public class LonelyMinionConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME;
		GameObject gameObject = EntityTemplates.CreateEntity(LonelyMinionConfig.ID, text, true);
		gameObject.AddComponent<Accessorizer>();
		gameObject.AddOrGet<WearableAccessorizer>();
		gameObject.AddComponent<Storage>().doDiseaseTransfer = false;
		gameObject.AddComponent<StateMachineController>();
		LonelyMinion.Def def = gameObject.AddOrGetDef<LonelyMinion.Def>();
		def.Personality = Db.Get().Personalities.Get(LonelyMinionConfig.MinionName);
		def.Personality.Disabled = true;
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.defaultAnim = "idle_default";
		kbatchedAnimController.initialAnim = "idle_default";
		kbatchedAnimController.initialMode = KAnim.PlayMode.Loop;
		kbatchedAnimController.AnimFiles = new KAnimFile[]
		{
			Assets.GetAnim("body_comp_default_kanim"),
			Assets.GetAnim("anim_idles_default_kanim"),
			Assets.GetAnim("anim_interacts_lonely_dupe_kanim")
		};
		this.ConfigurePackageOverride(gameObject);
		SymbolOverrideController symbolOverrideController = SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		symbolOverrideController.applySymbolOverridesEveryFrame = true;
		symbolOverrideController.AddSymbolOverride("snapto_cheek", Assets.GetAnim("head_swap_kanim").GetData().build.GetSymbol(string.Format("cheek_00{0}", def.Personality.headShape)), 1);
		MinionConfig.ConfigureSymbols(gameObject, true);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	private void ConfigurePackageOverride(GameObject go)
	{
		GameObject gameObject = new GameObject("PackageSnapPoint");
		gameObject.transform.SetParent(go.transform);
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.transform.position = Vector3.forward * -0.1f;
		kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("mushbar_kanim") };
		kbatchedAnimController.initialAnim = "object";
		component.SetSymbolVisiblity(LonelyMinionConfig.PARCEL_SNAPTO, false);
		KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddOrGet<KBatchedAnimTracker>();
		kbatchedAnimTracker.controller = component;
		kbatchedAnimTracker.symbol = LonelyMinionConfig.PARCEL_SNAPTO;
	}

	public static string ID = "LonelyMinion";

	public const int VOICE_IDX = -2;

	public const int STARTING_SKILL_POINTS = 3;

	public const int BASE_ATTRIBUTE_LEVEL = 7;

	public const int AGE_MIN = 2190;

	public const int AGE_MAX = 3102;

	public const float MIN_IDLE_DELAY = 20f;

	public const float MAX_IDLE_DELAY = 40f;

	public const string IDLE_PREFIX = "idle_blinds";

	public static readonly HashedString GreetingCriteraId = "Neighbor";

	public static readonly HashedString FoodCriteriaId = "FoodQuality";

	public static readonly HashedString DecorCriteriaId = "Decor";

	public static readonly HashedString PowerCriteriaId = "SuppliedPower";

	public static readonly HashedString CHECK_MAIL = "mail_pre";

	public static readonly HashedString CHECK_MAIL_SUCCESS = "mail_success_pst";

	public static readonly HashedString CHECK_MAIL_FAILURE = "mail_failure_pst";

	public static readonly HashedString CHECK_MAIL_DUPLICATE = "mail_duplicate_pst";

	public static readonly HashedString FOOD_SUCCESS = "food_like_loop";

	public static readonly HashedString FOOD_FAILURE = "food_dislike_loop";

	public static readonly HashedString FOOD_DUPLICATE = "food_duplicate_loop";

	public static readonly HashedString FOOD_IDLE = "idle_food_quest";

	public static readonly HashedString DECOR_IDLE = "idle_decor_quest";

	public static readonly HashedString POWER_IDLE = "idle_power_quest";

	public static readonly HashedString BLINDS_IDLE_0 = "idle_blinds_0";

	public static readonly HashedString PARCEL_SNAPTO = "parcel_snapTo";

	public static readonly string MinionName = "JORGE";

	public static readonly string BodyAnimFile = "body_lonelyminion_kanim";
}
