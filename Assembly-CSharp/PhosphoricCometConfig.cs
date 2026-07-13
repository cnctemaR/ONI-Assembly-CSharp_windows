using System;
using STRINGS;
using UnityEngine;

public class PhosphoricCometConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = BaseCometConfig.BaseComet(PhosphoricCometConfig.ID, UI.SPACEDESTINATIONS.COMETS.PHOSPHORICCOMET.NAME, "meteor_phosphoric_kanim", SimHashes.Phosphorite, new Vector2(3f, 20f), new Vector2(310.15f, 323.15f), "Meteor_dust_heavy_Impact", 0, SimHashes.Void, SpawnFXHashes.MeteorImpactPhosphoric, 0.3f);
		Comet component = gameObject.GetComponent<Comet>();
		component.explosionOreCount = new Vector2I(1, 2);
		component.explosionSpeedRange = new Vector2(4f, 7f);
		component.entityDamage = 0;
		component.totalTileDamage = 0f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "PhosphoricComet";
}
