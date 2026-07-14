using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CrabShellConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("CrabShell", ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME, ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.DESC, 1f, false, Assets.GetAnim("crabshell_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.Organics,
			GameTags.MoltShell
		});
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		gameObject.AddComponent<EntitySizeVisualizer>().TierSetType = OreSizeVisualizerComponents.TiersSetType.PokeShells;
		EntityTemplates.CreateAndRegisterCompostableFromPrefab(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<Compostable>().OnDeserializeCb = delegate(KMonoBehaviour inst)
		{
			if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 36))
			{
				inst.GetComponent<PrimaryElement>();
				PrimaryElement component = inst.GetComponent<PrimaryElement>();
				if (component != null)
				{
					component.MassPerUnit = 1f;
					component.Mass = component.Units * 60f;
				}
				KPrefabID component2 = inst.GetComponent<KPrefabID>();
				if (component2 != null)
				{
					component2.RemoveTag(GameTags.IndustrialIngredient);
				}
			}
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CrabShell";

	public static readonly Tag TAG = TagManager.Create("CrabShell");

	public const float ADULT_MASS = 60f;
}
