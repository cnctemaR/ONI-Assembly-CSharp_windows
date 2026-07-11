using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SetLockerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "SetLocker";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.SETLOCKER.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.SETLOCKER.DESC;
		float num = 100f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("setpiece_locker_kanim"), "on", Grid.SceneLayer.Building, 1, 2, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.overrideAnim = "anim_interacts_clothingfactory_kanim";
		setLocker.dropOffset = new Vector2I(0, 1);
		setLocker.possible_contents_ids = new string[] { "Warm_Vest", "Cool_Vest", "Funky_Vest" };
		gameObject.AddOrGet<LoreBearer>();
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
