using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class ScoutRoverConfig : IEntityConfig, IHasDlcRestrictions
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
		return BaseRoverConfig.BaseRover("ScoutRover", global::STRINGS.ROBOTS.MODELS.SCOUT.NAME, GameTags.Robots.Models.ScoutRover, global::STRINGS.ROBOTS.MODELS.SCOUT.DESC, "scout_bot_kanim", 100f, 1f, 2f, global::TUNING.ROBOTS.SCOUTBOT.CARRY_CAPACITY, global::TUNING.ROBOTS.SCOUTBOT.DIGGING, global::TUNING.ROBOTS.SCOUTBOT.CONSTRUCTION, global::TUNING.ROBOTS.SCOUTBOT.ATHLETICS, global::TUNING.ROBOTS.SCOUTBOT.HIT_POINTS, global::TUNING.ROBOTS.SCOUTBOT.BATTERY_CAPACITY, global::TUNING.ROBOTS.SCOUTBOT.BATTERY_DEPLETION_RATE, Db.Get().Amounts.InternalChemicalBattery, false);
	}

	public void OnPrefabInit(GameObject inst)
	{
		BaseRoverConfig.OnPrefabInit(inst, Db.Get().Amounts.InternalChemicalBattery);
	}

	public void OnSpawn(GameObject inst)
	{
		BaseRoverConfig.OnSpawn(inst);
		Effects effects = inst.GetComponent<Effects>();
		if (inst.transform.parent == null)
		{
			if (effects.HasEffect("ScoutBotCharging"))
			{
				effects.Remove("ScoutBotCharging");
			}
		}
		else if (!effects.HasEffect("ScoutBotCharging"))
		{
			effects.Add("ScoutBotCharging", false);
		}
		inst.Subscribe(856640610, delegate(object _)
		{
			if (inst.transform.parent == null)
			{
				if (effects.HasEffect("ScoutBotCharging"))
				{
					effects.Remove("ScoutBotCharging");
					return;
				}
			}
			else if (!effects.HasEffect("ScoutBotCharging"))
			{
				effects.Add("ScoutBotCharging", false);
			}
		});
	}

	public const string ID = "ScoutRover";

	public const float MASS = 100f;

	private const float WIDTH = 1f;

	private const float HEIGHT = 2f;

	public const int MAXIMUM_TECH_CONSTRUCTION_TIER = 2;
}
