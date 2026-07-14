using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectConfigs : IMultiEntityConfig
{
	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		List<EffectConfigs.EffectTemplate> list2 = new List<EffectConfigs.EffectTemplate>
		{
			new EffectConfigs.EffectTemplate
			{
				id = EffectConfigs.EffectTemplateId,
				animFiles = new string[0],
				initialAnim = "",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = false
			},
			new EffectConfigs.EffectTemplate
			{
				id = EffectConfigs.EffectTemplateOverrideId,
				animFiles = new string[0],
				initialAnim = "",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = false
			},
			new EffectConfigs.EffectTemplate
			{
				id = EffectConfigs.AttackSplashId,
				animFiles = new string[] { "attack_beam_contact_fx_kanim" },
				initialAnim = "loop",
				initialMode = KAnim.PlayMode.Loop,
				destroyOnAnimComplete = false
			},
			new EffectConfigs.EffectTemplate
			{
				id = EffectConfigs.OreAbsorbId,
				animFiles = new string[] { "ore_collision_kanim" },
				initialAnim = "idle",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = true
			},
			new EffectConfigs.EffectTemplate
			{
				id = EffectConfigs.PlantDeathId,
				animFiles = new string[] { "plant_death_fx_kanim" },
				initialAnim = "plant_death",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = true
			},
			new EffectConfigs.EffectTemplate
			{
				id = EffectConfigs.BuildSplashId,
				animFiles = new string[] { "sparks_radial_build_kanim" },
				initialAnim = "loop",
				initialMode = KAnim.PlayMode.Loop,
				destroyOnAnimComplete = false
			},
			new EffectConfigs.EffectTemplate
			{
				id = EffectConfigs.DemolishSplashId,
				animFiles = new string[] { "poi_demolish_impact_kanim" },
				initialAnim = "POI_demolish_impact",
				initialMode = KAnim.PlayMode.Loop,
				destroyOnAnimComplete = false
			}
		};
		if (DlcManager.IsContentSubscribed("DLC5_ID"))
		{
			list2.Add(new EffectConfigs.EffectTemplate
			{
				id = EffectConfigs.SquidAttackId,
				animFiles = new string[] { "squid_ink_fx_kanim" },
				initialAnim = "loop",
				initialMode = KAnim.PlayMode.Once,
				destroyOnAnimComplete = true
			});
		}
		foreach (EffectConfigs.EffectTemplate effectTemplate in list2)
		{
			GameObject gameObject = EntityTemplates.CreateEntity(effectTemplate.id, effectTemplate.id, false);
			KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
			kbatchedAnimController.materialType = KAnimBatchGroup.MaterialType.Simple;
			kbatchedAnimController.initialAnim = effectTemplate.initialAnim;
			kbatchedAnimController.initialMode = effectTemplate.initialMode;
			kbatchedAnimController.isMovable = true;
			kbatchedAnimController.destroyOnAnimComplete = effectTemplate.destroyOnAnimComplete;
			if (effectTemplate.id == EffectConfigs.EffectTemplateOverrideId)
			{
				SymbolOverrideControllerUtil.AddToPrefab(gameObject);
			}
			if (effectTemplate.animFiles.Length != 0)
			{
				KAnimFile[] array = new KAnimFile[effectTemplate.animFiles.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Assets.GetAnim(effectTemplate.animFiles[i]);
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

	public static string EffectTemplateOverrideId = "EffectTemplateOverrideFx";

	public static string AttackSplashId = "AttackSplashFx";

	public static string OreAbsorbId = "OreAbsorbFx";

	public static string PlantDeathId = "PlantDeathFx";

	public static string BuildSplashId = "BuildSplashFx";

	public static string DemolishSplashId = "DemolishSplashFx";

	public static string SquidAttackId = "SquidAttackFx";

	public struct EffectTemplate
	{
		public string id;

		public string[] animFiles;

		public string initialAnim;

		public KAnim.PlayMode initialMode;

		public bool destroyOnAnimComplete;
	}
}
