using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DisposableElectrobankConfig : IMultiEntityConfig
{
	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(this.CreateDisposableElectrobank("DisposableElectrobank_RawMetal", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_METAL_ORE.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_METAL_ORE.DESC, 20f, SimHashes.Cuprite, "electrobank_popcan_kanim", DlcManager.DLC3, null, "object"));
		list.Add(this.CreateDisposableElectrobank("DisposableElectrobank_BasicSingleHarvestPlant", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_MUCKROOT.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_MUCKROOT.DESC, 20f, SimHashes.Creature, "electrobank_muckroot_kanim", DlcManager.DLC3, null, "object"));
		list.Add(this.CreateDisposableElectrobank("DisposableElectrobank_LightBugEgg", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_LIGHTBUGEGG.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_LIGHTBUGEGG.DESC, 20f, SimHashes.Creature, "electrobank_shinebug_egg_kanim", DlcManager.DLC3, null, "object"));
		list.Add(this.CreateDisposableElectrobank("DisposableElectrobank_Sucrose", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_SUCROSE.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_SUCROSE.DESC, 20f, SimHashes.Sucrose, "electrobank_sucrose_kanim", DlcManager.DLC3, null, "object"));
		list.Add(this.CreateDisposableElectrobank("DisposableElectrobank_UraniumOre", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_URANIUM_ORE.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_URANIUM_ORE.DESC, 20f, SimHashes.UraniumOre, "electrobank_uranium_kanim", DlcManager.DLC3.Append<string>(DlcManager.EXPANSION1), null, "object"));
		list.RemoveAll((GameObject t) => t == null);
		return list;
	}

	private GameObject CreateDisposableElectrobank(string id, LocString name, LocString description, float mass, SimHashes element, string animName, string[] requiredDlcIDs = null, string[] forbiddenDlcIds = null, string initialAnim = "object")
	{
		if (!DlcManager.IsCorrectDlcSubscribed(requiredDlcIDs, forbiddenDlcIds))
		{
			return null;
		}
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, description, mass, true, Assets.GetAnim(animName), initialAnim, Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 0.8f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.ChargedPortableBattery,
			GameTags.PedestalDisplayable
		});
		gameObject.GetComponent<KCollider2D>();
		gameObject.AddComponent<Electrobank>();
		gameObject.AddOrGet<OccupyArea>().SetCellOffsets(EntityTemplates.GenerateOffsets(1, 1));
		gameObject.AddOrGet<DecorProvider>().SetValues(DECOR.PENALTY.TIER0);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "DisposableElectrobank_";

	public const float MASS = 20f;

	public static Dictionary<Tag, ComplexRecipe> recipes = new Dictionary<Tag, ComplexRecipe>();

	public const string ID_METAL_ORE = "DisposableElectrobank_RawMetal";

	public const string ID_MUCKROOT = "DisposableElectrobank_BasicSingleHarvestPlant";

	public const string ID_LIGHTBUG_EGG = "DisposableElectrobank_LightBugEgg";

	public const string ID_SUCROSE = "DisposableElectrobank_Sucrose";

	public const string ID_URANIUM_ORE = "DisposableElectrobank_UraniumOre";
}
