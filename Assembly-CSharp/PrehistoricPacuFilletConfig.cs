using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrehistoricPacuFilletConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC4;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("PrehistoricPacuFillet", global::STRINGS.ITEMS.FOOD.PREHISTORICPACUFILLET.NAME, global::STRINGS.ITEMS.FOOD.PREHISTORICPACUFILLET.DESC, 1f, false, Assets.GetAnim("jawboFillet_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.JAWBOFILLET);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PrehistoricPacuFillet";
}
