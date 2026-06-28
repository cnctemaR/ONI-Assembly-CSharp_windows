using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FlutConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "Flut";
		string text2 = global::STRINGS.CREATURES.SPECIES.FLUT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.FLUT.DESC;
		float num = 25f;
		KAnimFile anim = Assets.GetAnim("flut_single_kanim");
		string text4 = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, null, "SwimmerNavGrid", NavType.Swim, 32, 2f, "Meat", 2, false, true, 30f, 283f, 294f, 243f, 343f);
		gameObject.AddOrGet<Catchable>();
		gameObject.AddOrGet<Storage>();
		gameObject.AddOrGet<Operational>();
		ElementConverter elementConverter = gameObject.AddOrGet<ElementConverter>();
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.25f, SimHashes.Fertilizer, 0f, false, 0f, 0.5f, false, 1f, byte.MaxValue, 0)
		};
		gameObject.AddAquaticReproducer("flutEgg".ToTag(), 60f, 100f, 0.08f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const float ReproductiveCylceLength = 60f;

	public const float RandomCycleOffset = 100f;

	public const float MaxmimumPopulationDensityForReproduce = 0.08f;
}
