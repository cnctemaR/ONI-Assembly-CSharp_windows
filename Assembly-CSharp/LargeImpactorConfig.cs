using System;
using STRINGS;
using UnityEngine;

public class LargeImpactorConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return new string[] { "EXPANSION1_ID", "DLC4_ID" };
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	GameObject IEntityConfig.CreatePrefab()
	{
		GameObject gameObject = LargeImpactorVanillaConfig.ConfigCommon("LargeImpactor", this.NAME);
		gameObject.AddOrGet<InfoDescription>().description = this.DESC;
		ClusterDestinationSelector clusterDestinationSelector = gameObject.AddOrGet<ClusterDestinationSelector>();
		clusterDestinationSelector.assignable = false;
		clusterDestinationSelector.canNavigateFogOfWar = true;
		clusterDestinationSelector.requireAsteroidDestination = true;
		clusterDestinationSelector.requireLaunchPadOnAsteroidDestination = false;
		clusterDestinationSelector.dodgesHiddenAsteroids = true;
		ClusterMapMeteorShowerVisualizer clusterMapMeteorShowerVisualizer = gameObject.AddOrGet<ClusterMapMeteorShowerVisualizer>();
		clusterMapMeteorShowerVisualizer.p_name = this.NAME;
		clusterMapMeteorShowerVisualizer.clusterAnimName = "shower_cluster_demolior_kanim";
		clusterMapMeteorShowerVisualizer.revealed = true;
		clusterMapMeteorShowerVisualizer.forceRevealed = true;
		clusterMapMeteorShowerVisualizer.isWorldEntity = true;
		ClusterTraveler clusterTraveler = gameObject.AddOrGet<ClusterTraveler>();
		clusterTraveler.revealsFogOfWarAsItTravels = true;
		clusterTraveler.peekRadius = 0;
		clusterTraveler.quickTravelToAsteroidIfInOrbit = false;
		ClusterMapLargeImpactor.Def def = gameObject.AddOrGetDef<ClusterMapLargeImpactor.Def>();
		def.name = this.NAME;
		def.description = this.DESC;
		def.eventID = "LargeImpactor";
		return gameObject;
	}

	void IEntityConfig.OnPrefabInit(GameObject inst)
	{
	}

	void IEntityConfig.OnSpawn(GameObject inst)
	{
		inst.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.ImpactorStatus, inst.GetComponent<ClusterTraveler>());
		LargeImpactorVanillaConfig.SpawnCommon(inst);
	}

	public const string ID = "LargeImpactor";

	public string NAME = UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.NAME;

	public string DESC = UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.DESCRIPTION;

	public const string ANIMFILE = "shower_cluster_demolior_kanim";

	public const int HEALTH = 1000;
}
