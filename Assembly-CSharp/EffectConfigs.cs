using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectConfigs : IMultiEntityConfig
{
	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		var anon = new <>f__AnonymousType0<string, string[], string, KAnim.PlayMode, bool>[]
		{
			new
			{
				id = EffectConfigs.EffectTemplateId,
				animFiles = new string[0],
				initialAnim = "",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = false
			},
			new
			{
				id = EffectConfigs.AttackSplashId,
				animFiles = new string[] { "attack_beam_contact_fx_kanim" },
				initialAnim = "loop",
				initialMode = KAnim.PlayMode.Loop,
				destroyOnAnimComplete = false
			},
			new
			{
				id = EffectConfigs.OreAbsorbId,
				animFiles = new string[] { "ore_collision_kanim" },
				initialAnim = "idle",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = true
			},
			new
			{
				id = EffectConfigs.PlantDeathId,
				animFiles = new string[] { "plant_death_fx_kanim" },
				initialAnim = "plant_death",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = true
			},
			new
			{
				id = EffectConfigs.BuildSplashId,
				animFiles = new string[] { "sparks_radial_build_kanim" },
				initialAnim = "loop",
				initialMode = KAnim.PlayMode.Loop,
				destroyOnAnimComplete = false
			}
		};
		for (int i = 0; i < anon.Length; i++)
		{
			var anon2 = anon[i];
			GameObject gameObject = EntityTemplates.CreateEntity(anon2.id, anon2.id, false);
			KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
			kbatchedAnimController.materialType = KAnimBatchGroup.MaterialType.Simple;
			kbatchedAnimController.initialAnim = anon2.initialAnim;
			kbatchedAnimController.initialMode = anon2.initialMode;
			kbatchedAnimController.isMovable = true;
			kbatchedAnimController.destroyOnAnimComplete = anon2.destroyOnAnimComplete;
			if (anon2.animFiles.Length != 0)
			{
				KAnimFile[] array = new KAnimFile[anon2.animFiles.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = Assets.GetAnim(anon2.animFiles[j]);
				}
				kbatchedAnimController.AnimFiles = array;
			}
			gameObject.AddOrGet<LoopingSounds>();
			list.Add(gameObject);
		}
		return list;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string EffectTemplateId = "EffectTemplateFx";

	public static string AttackSplashId = "AttackSplashFx";

	public static string OreAbsorbId = "OreAbsorbFx";

	public static string PlantDeathId = "PlantDeathFx";

	public static string BuildSplashId = "BuildSplashFx";
}
