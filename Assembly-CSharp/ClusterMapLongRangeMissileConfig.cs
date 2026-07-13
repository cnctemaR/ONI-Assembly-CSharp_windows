using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ClusterMapLongRangeMissileConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateBasicEntity("ClusterMapLongRangeMissile", ITEMS.MISSILE_LONGRANGE.NAME, ITEMS.MISSILE_LONGRANGE.DESC, 2000f, true, Assets.GetAnim("longrange_missile_clustermap_kanim"), "object", Grid.SceneLayer.Front, SimHashes.Creature, new List<Tag>
		{
			GameTags.IgnoreMaterialCategory,
			GameTags.Experimental
		}, 293f);
		gameObject.AddOrGetDef<ClusterMapLongRangeMissile.Def>();
		gameObject.AddComponent<LoopingSounds>();
		gameObject.AddOrGet<KSelectable>().IsSelectable = true;
		ClusterMapLongRangeMissileGridEntity clusterMapLongRangeMissileGridEntity = gameObject.AddOrGet<ClusterMapLongRangeMissileGridEntity>();
		clusterMapLongRangeMissileGridEntity.clusterAnimName = "longrange_missile_clustermap_kanim";
		clusterMapLongRangeMissileGridEntity.isWorldEntity = false;
		clusterMapLongRangeMissileGridEntity.nameKey = new StringKey("STRINGS.ITEMS.MISSILE_LONGRANGE.NAME");
		clusterMapLongRangeMissileGridEntity.keepRotationWhenSpacingOutInHex = true;
		ClusterDestinationSelector clusterDestinationSelector = gameObject.AddOrGet<ClusterDestinationSelector>();
		clusterDestinationSelector.canNavigateFogOfWar = false;
		clusterDestinationSelector.dodgesHiddenAsteroids = true;
		clusterDestinationSelector.requireAsteroidDestination = false;
		clusterDestinationSelector.requireLaunchPadOnAsteroidDestination = false;
		clusterDestinationSelector.assignable = false;
		clusterDestinationSelector.shouldPointTowardsPath = true;
		gameObject.AddOrGet<ClusterTraveler>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ClusterMapLongRangeMissile";

	public const float MASS = 2000f;

	public const float STARMAP_SPEED = 10f;
}
