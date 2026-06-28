using System;
using Klei;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Puft", global::STRINGS.CREATURES.SPECIES.PUFT.NAME, global::STRINGS.CREATURES.SPECIES.PUFT.DESC, 50f, Assets.GetAnim("puft_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, 25f, "FlyerNavGrid1x1", NavType.Hover, 2f, "Meat", 1, true, true, 30f, 302f, 318f, 243f, 343f);
		Puft puft = gameObject.UpdateComponentRequirement<Puft>(true);
		puft.consumedElement = SimHashes.ContaminatedOxygen;
		puft.minimumApproachMass = 0.1f;
		puft.emitDiseaseIdx = Db.Get().Diseases.GetIndex("SlimeLung");
		puft.emitDiseasePerKg = 1000;
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		gameObject.UpdateComponentRequirement<SimpleMover>(true);
		ElementEmitter elementEmitter = gameObject.AddElementEmitter(SimHashes.SlimeMold, 0f, 0f, SimUtil.DiseaseInfo.Invalid);
		ElementConsumer elementConsumer = gameObject.AddElementConsumer(SimHashes.ContaminatedOxygen, 0.25f, 0.05f, 3);
		elementEmitter.showDescriptor = false;
		elementConsumer.showDescriptor = false;
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_air_intake", NOISE_POLLUTION.CREATURES.TIER4);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_toot", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_air_inflated", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Puft";

	public const SimHashes consumeElement = SimHashes.ContaminatedOxygen;

	public const SimHashes emitElement = SimHashes.SlimeMold;

	public const string emitDisease = "SlimeLung";

	public const int emitDiseasePerKg = 1000;

	public const float minimumApproachMass = 0.1f;
}
