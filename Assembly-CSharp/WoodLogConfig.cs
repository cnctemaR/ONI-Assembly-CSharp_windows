using System;
using UnityEngine;

public class WoodLogConfig : IOreConfig
{
	public SimHashes ElementID
	{
		get
		{
			return SimHashes.WoodLog;
		}
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(this.ElementID, null);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += this.OnInit;
		component.prefabSpawnFn += this.OnSpawn;
		component.RemoveTag(GameTags.HideFromSpawnTool);
		return gameObject;
	}

	public void OnInit(GameObject inst)
	{
		PrimaryElement component = inst.GetComponent<PrimaryElement>();
		component.SetElement(this.ElementID, true);
		Element element = component.Element;
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<PrimaryElement>().SetElement(this.ElementID, true);
	}

	public const string ID = "WoodLog";

	public const float C02MassEmissionWhenBurned = 0.142f;

	public const float HeatWhenBurned = 7500f;

	public const float EnergyWhenBurned = 250f;

	public static readonly Tag TAG = TagManager.Create("WoodLog");
}
