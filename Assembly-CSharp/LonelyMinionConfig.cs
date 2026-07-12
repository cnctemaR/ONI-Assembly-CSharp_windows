using System;
using System.Collections.Generic;
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
		gameObject.AddComponent<Storage>().doDiseaseTransfer = false;
		gameObject.AddComponent<StateMachineController>();
		LonelyMinion.Def def = gameObject.AddOrGetDef<LonelyMinion.Def>();
		Tag tag = new Tag("Jorge");
		string text2 = tag.Name.ToUpper();
		def.Personality = new Personality(text2, Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", text2)), "Male", "Grumpy", "UglyCrier", "BalloonArtist", "", "", 5, 5, -1, 3, 45, tag.GetHash(), Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", text2)), false);
		KAnimFile anim = Assets.GetAnim("body_lonelyminion_kanim");
		List<AccessorySlot> resources = Db.Get().AccessorySlots.resources;
		for (int i = 0; i < resources.Count; i++)
		{
			int count = resources[i].accessories.Count;
			resources[i].AddAccessories(anim, null);
			if (count != resources[i].accessories.Count)
			{
				Resource resource = resources[i].accessories[resources[i].accessories.Count - 1];
				Db.Get().ResourceTable.Add(resource);
			}
		}
		def.Personality.Disabled = true;
		Db.Get().Personalities.Add(def.Personality);
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
		MinionConfig.ConfigureSymbols(gameObject, false);
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

	public static void ApplyAccessoryOverrides(Accessorizer accessorizer)
	{
		int num = Hash.SDBMLower("Jorge");
		accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Neck));
		accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Leg));
		accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Belt));
		accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Pelvis));
		accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Foot));
		accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hand));
		accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Cuff));
		accessorizer.AddAccessory(Db.Get().AccessorySlots.Neck.Lookup(string.Format("neck_{0}", num)));
		accessorizer.AddAccessory(Db.Get().AccessorySlots.Leg.Lookup(string.Format("leg_{0}", num)));
		accessorizer.AddAccessory(Db.Get().AccessorySlots.Belt.Lookup(string.Format("belt_{0}", num)));
		accessorizer.AddAccessory(Db.Get().AccessorySlots.Pelvis.Lookup(string.Format("pelvis_{0}", num)));
		accessorizer.AddAccessory(Db.Get().AccessorySlots.Foot.Lookup(string.Format("foot_{0}", num)));
		accessorizer.AddAccessory(Db.Get().AccessorySlots.Hand.Lookup(string.Format("hand_paint_{0}", num)));
		accessorizer.AddAccessory(Db.Get().AccessorySlots.Cuff.Lookup(string.Format("cuff_{0}", num)));
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
}
