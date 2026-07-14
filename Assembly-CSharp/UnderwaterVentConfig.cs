using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class UnderwaterVentConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "UnderwaterVent";
		string text2 = global::STRINGS.CREATURES.SPECIES.GEYSER.UNDERWATERVENT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.GEYSER.UNDERWATERVENT.DESC;
		float num = 2000f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER1;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("underwater_vent_kanim"), "idle", Grid.SceneLayer.BuildingBack, 4, 4, tier, tier2, SimHashes.Creature, new List<Tag> { GameTags.GeyserFeature }, 293f);
		gameObject.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		gameObject.AddOrGet<EntombVulnerable>();
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Katairite, true);
		component.Temperature = 363.15f;
		gameObject.AddOrGet<Submergable>().GetStatusItem = new Func<StatusItem>(UnderwaterVentConfig.GetSubmergableStatusItem);
		gameObject.AddOrGetDef<UnderwaterVent.Def>().data = UnderwaterVentConfig.Data;
		gameObject.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), GameTags.UnderwaterVentDrill, null)
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.AddOrGet<Submergable>().GetStatusItem = new Func<StatusItem>(UnderwaterVentConfig.GetSubmergableStatusItem);
	}

	public void OnSpawn(GameObject inst)
	{
	}

	private static StatusItem GetSubmergableStatusItem()
	{
		return Db.Get().CreatureStatusItems.NotSubmerged;
	}

	public const string ID = "UnderwaterVent";

	public static readonly UnderwaterVent.Data Data = new UnderwaterVent.Data(new Vector3(1f, 2.5f, 0f), new Vector3(1f, 1.5f, 0f), SimHashes.Methane, 373.15f, 0.083333336f, SimHashes.Sulfur, 1000f, 373.15f, 1200f);
}
