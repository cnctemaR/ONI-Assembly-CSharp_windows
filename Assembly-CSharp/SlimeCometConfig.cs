using System;
using STRINGS;
using UnityEngine;

public class SlimeCometConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		float mass = ElementLoader.FindElementByHash(SimHashes.SlimeMold).defaultValues.mass;
		GameObject gameObject = BaseCometConfig.BaseComet(SlimeCometConfig.ID, UI.SPACEDESTINATIONS.COMETS.SLIMECOMET.NAME, "meteor_slime_kanim", SimHashes.SlimeMold, new Vector2(mass * 0.8f * 2f, mass * 1.2f * 2f), new Vector2(310.15f, 323.15f), "Meteor_slimeball_Impact", 7, SimHashes.ContaminatedOxygen, SpawnFXHashes.MeteorImpactSlime, 0.6f);
		Comet component = gameObject.GetComponent<Comet>();
		component.entityDamage = 0;
		component.totalTileDamage = 0f;
		component.explosionOreCount = new Vector2I(1, 2);
		component.explosionSpeedRange = new Vector2(4f, 7f);
		component.addTiles = 2;
		component.addTilesMinHeight = 1;
		component.addTilesMaxHeight = 2;
		component.diseaseIdx = Db.Get().Diseases.GetIndex("SlimeLung");
		component.addDiseaseCount = (int)(component.EXHAUST_RATE * 100000f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
		go.GetComponent<PrimaryElement>().AddDisease(Db.Get().Diseases.GetIndex("SlimeLung"), (int)(global::UnityEngine.Random.Range(0.9f, 1.2f) * 50f * 100000f), "Meteor");
	}

	public static string ID = "SlimeComet";

	public const int ADDED_CELLS = 2;

	private const SimHashes element = SimHashes.SlimeMold;
}
