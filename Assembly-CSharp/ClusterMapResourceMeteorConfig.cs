using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClusterMapResourceMeteorConfig : IMultiEntityConfig
{
	public static string GetFullID(string id)
	{
		return "ClusterMapResourceMeteor_" + id;
	}

	public static string GetReverseFullID(string fullID)
	{
		return fullID.Replace("ClusterMapResourceMeteor_", "");
	}

	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		if (!DlcManager.IsExpansion1Active())
		{
			return list;
		}
		list.Add(ClusterMapResourceMeteorConfig.CreateClusterResourceMeteor("Copper", "ClusterCopperMeteor", UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.COPPER.NAME, UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.COPPER.DESCRIPTION, "shower_cluster_copper_kanim", "idle_loop", "ui", DlcManager.EXPANSION1, null, SimHashes.Unobtanium));
		list.Add(ClusterMapResourceMeteorConfig.CreateClusterResourceMeteor("Iron", "ClusterIronMeteor", UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.IRON.NAME, UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.IRON.DESCRIPTION, "shower_cluster_iron_kanim", "idle_loop", "ui", DlcManager.EXPANSION1, null, SimHashes.Unobtanium));
		list.RemoveAll((GameObject x) => x == null);
		return list;
	}

	public static GameObject CreateClusterResourceMeteor(string id, string meteorEventID, string name, string desc, string animFile, string initial_anim = "idle_loop", string ui_anim = "ui", string[] requiredDlcIds = null, string[] forbiddenDlcIds = null, SimHashes element = SimHashes.Unobtanium)
	{
		if (!DlcManager.IsCorrectDlcSubscribed(requiredDlcIds, forbiddenDlcIds))
		{
			return null;
		}
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ClusterMapResourceMeteorConfig.GetFullID(id), name, desc, 2000f, true, Assets.GetAnim(animFile), initial_anim, Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 1f, 1f, false, SORTORDER.KEEPSAKES, element, new List<Tag>());
		gameObject.AddOrGet<KSelectable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.GetComponent<KBatchedAnimController>().initialMode = KAnim.PlayMode.Loop;
		ClusterDestinationSelector clusterDestinationSelector = gameObject.AddOrGet<ClusterDestinationSelector>();
		clusterDestinationSelector.assignable = false;
		clusterDestinationSelector.canNavigateFogOfWar = true;
		clusterDestinationSelector.requireAsteroidDestination = true;
		clusterDestinationSelector.requireLaunchPadOnAsteroidDestination = false;
		clusterDestinationSelector.dodgesHiddenAsteroids = true;
		ClusterMapMeteorShowerVisualizer clusterMapMeteorShowerVisualizer = gameObject.AddOrGet<ClusterMapMeteorShowerVisualizer>();
		clusterMapMeteorShowerVisualizer.p_name = name;
		clusterMapMeteorShowerVisualizer.clusterAnimName = animFile;
		ClusterTraveler clusterTraveler = gameObject.AddOrGet<ClusterTraveler>();
		clusterTraveler.revealsFogOfWarAsItTravels = false;
		clusterTraveler.quickTravelToAsteroidIfInOrbit = false;
		ClusterMapMeteorShower.Def def = gameObject.AddOrGetDef<ClusterMapMeteorShower.Def>();
		def.name = name;
		def.description = desc;
		def.name_Hidden = UI.SPACEDESTINATIONS.CLUSTERMAPMETEORSHOWERS.UNIDENTIFIED.NAME;
		def.description_Hidden = UI.SPACEDESTINATIONS.CLUSTERMAPMETEORSHOWERS.UNIDENTIFIED.DESCRIPTION;
		def.eventID = meteorEventID;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.requiredDlcIds = requiredDlcIds;
		component.forbiddenDlcIds = forbiddenDlcIds;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string IDENTIFY_AUDIO_NAME = "ClusterMapMeteor_Reveal";

	public const string ID_SIGNATURE = "ClusterMapResourceMeteor_";
}
