using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MinnowImperativePOIAConfig : IEntityConfig, IHasDlcRestrictions
{
	public Tag RequiredDeliveryTag
	{
		get
		{
			return SimHashes.Pearl.CreateTag();
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
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("MinnowImperativePOIA", global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_A.NAME, global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_A.DESC, 30f, Assets.GetAnim("minnow_imperative_poi_a_kanim"), "off", Grid.SceneLayer.Creatures, 3, 3, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NONE, SimHashes.Creature, null, 293f);
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		storage.capacityKg = 200f;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = gameObject.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = this.RequiredDeliveryTag;
		manualDeliveryKG.capacity = 200f;
		manualDeliveryKG.refillMass = 200f;
		manualDeliveryKG.MinimumMass = 200f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
		manualDeliveryKG.enabled = false;
		gameObject.AddOrGet<Prioritizable>();
		Prioritizable.AddRef(gameObject);
		MinnowImperativePOIStates.Def def = gameObject.AddOrGetDef<MinnowImperativePOIStates.Def>();
		def.requestedTag = this.RequiredDeliveryTag;
		def.requiredMass = 200f;
		def.minnowPOIIdentity = MinnowImperativePOIStates.MinnowPOIIdentity.POI_A;
		string text = "MINNOW";
		Db.Get().Personalities.Get(text).Disabled = true;
		gameObject.GetComponent<KBatchedAnimController>().materialType = KAnimBatchGroup.MaterialType.DefaultInsideVistas;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "MinnowImperativePOIA";

	public const float RequiredDeliveryMass = 200f;
}
