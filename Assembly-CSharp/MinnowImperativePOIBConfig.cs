using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MinnowImperativePOIBConfig : IEntityConfig, IHasDlcRestrictions
{
	public static Tag RequiredDeliveryTag
	{
		get
		{
			return "Maki".ToTag();
		}
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("MinnowImperativePOIB", global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_B.NAME, global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_B.DESC, 30f, Assets.GetAnim("minnow_imperative_poi_b_kanim"), "off", Grid.SceneLayer.Creatures, 3, 3, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NONE, SimHashes.Creature, null, 293f);
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		storage.capacityKg = 10f;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = gameObject.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = MinnowImperativePOIBConfig.RequiredDeliveryTag;
		manualDeliveryKG.capacity = 10f;
		manualDeliveryKG.refillMass = 10f;
		manualDeliveryKG.MinimumMass = 10f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
		manualDeliveryKG.enabled = false;
		gameObject.AddOrGet<Prioritizable>();
		Prioritizable.AddRef(gameObject);
		MinnowImperativePOIStates.Def def = gameObject.AddOrGetDef<MinnowImperativePOIStates.Def>();
		def.requestedTag = MinnowImperativePOIBConfig.RequiredDeliveryTag;
		def.requiredMass = 10f;
		def.minnowPOIIdentity = MinnowImperativePOIStates.MinnowPOIIdentity.POI_B;
		gameObject.GetComponent<KBatchedAnimController>().materialType = KAnimBatchGroup.MaterialType.DefaultInsideVistas;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "MinnowImperativePOIB";

	public const float RequiredDeliveryMass = 10f;
}
