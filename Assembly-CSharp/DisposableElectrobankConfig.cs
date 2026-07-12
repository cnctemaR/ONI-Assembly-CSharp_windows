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
		if (!DlcManager.IsContentSubscribed("DLC3_ID"))
		{
			return list;
		}
		list.Add(this.CreateDisposableElectrobank("DisposableElectrobank_RawMetal", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_METAL_ORE.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_METAL_ORE.DESC, 20f, SimHashes.Cuprite, "electrobank_popcan_kanim", DlcManager.DLC3, null, "object"));
		if (DlcManager.IsExpansion1Active())
		{
			GameObject gameObject = this.CreateDisposableElectrobank("DisposableElectrobank_UraniumOre", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_URANIUM_ORE.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_URANIUM_ORE.DESC, 10f, SimHashes.UraniumOre, "electrobank_uranium_kanim", DlcManager.DLC3, null, "object");
			RadiationEmitter radiationEmitter = gameObject.AddOrGet<RadiationEmitter>();
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emitRadiusX = 5;
			radiationEmitter.emitRadiusY = radiationEmitter.emitRadiusX;
			radiationEmitter.emitRads = 60f;
			radiationEmitter.emissionOffset = new Vector3(0f, 0f, 0f);
			list.Add(gameObject);
			gameObject.GetComponent<Electrobank>().radioactivityTuning = radiationEmitter.emitRads;
		}
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
			GameTags.PedestalDisplayable,
			GameTags.DisposablePortableBattery
		});
		if (!Assets.IsTagCountable(GameTags.ChargedPortableBattery))
		{
			Assets.AddCountableTag(GameTags.ChargedPortableBattery);
		}
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

	public const string ID_URANIUM_ORE = "DisposableElectrobank_UraniumOre";
}
