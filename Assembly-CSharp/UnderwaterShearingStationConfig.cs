using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class UnderwaterShearingStationConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "UnderwaterShearingStation";
		int num = 3;
		int num2 = 3;
		string text2 = "shearing_station_aquatic_kanim";
		int num3 = 30;
		float num4 = 60f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnBackWall;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.CanUseRanchStation.Id;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStationType, false);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		go.AddOrGet<BuildingSubmergable>();
		go.AddComponent<UnderwaterShearingStaion>();
		go.AddOrGet<MultiSkillPerkMissingComplainer>().requiredSkillPerks = new string[]
		{
			Db.Get().SkillPerks.CanUseRanchStation.Id,
			Db.Get().SkillPerks.CanSwim.Id
		};
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.IsCritterEligibleToBeRanchedCb = delegate(GameObject creature_go, RanchStation.Instance ranch_station_smi)
		{
			if (creature_go.GetSMI<FlopMonitor.Instance>() == null)
			{
				return false;
			}
			IShearable smi2 = creature_go.GetSMI<IShearable>();
			return smi2 != null && smi2.IsFullyGrown();
		};
		def.RancherInteractAnim = "anim_interacts_shearingstation_aquatic_kanim";
		def.RancherCallingAndWipeBrowAnim = "anim_interacts_rancherstation_aquatic_kanim";
		def.RanchedPreAnim = "shearing_pre";
		def.RanchedLoopAnim = "shearing_loop";
		def.RanchedPstAnim = "shearing_pst";
		def.CreatureRanchingStatusItem = Db.Get().CreatureStatusItems.GettingRanched;
		def.RancherWipesBrowAnim = false;
		def.GetTargetRanchCell = delegate(RanchStation.Instance smi)
		{
			int num = Grid.InvalidCell;
			if (!smi.IsNullOrStopped())
			{
				num = Grid.PosToCell(smi.transform.GetPosition());
			}
			return num;
		};
		def.OnRanchCompleteCb = delegate(GameObject creature_go, WorkerBase rancher_wb)
		{
			Attributes attributes = rancher_wb.GetAttributes();
			float num2 = ((attributes != null) ? attributes.Get(Db.Get().Attributes.Ranching.Id).GetTotalValue() : 0f);
			RanchableMonitor.Instance smi3 = creature_go.GetSMI<RanchableMonitor.Instance>();
			IShearable smi4 = creature_go.GetSMI<IShearable>();
			if (smi4 != null)
			{
				global::Tuple<Tag, float> itemDroppedOnShear = smi4.GetItemDroppedOnShear();
				this.StoreShearable(smi3.TargetRanchStation.gameObject, creature_go, itemDroppedOnShear.first, itemDroppedOnShear.second);
				smi4.Shear();
			}
			UnderwaterShearingStaion component = smi3.TargetRanchStation.GetComponent<UnderwaterShearingStaion>();
			if (component != null)
			{
				component.HideShearableSymbol();
			}
		};
		def.OnRanchWorkBegins = delegate(RanchedStates.Instance creature, Workable workable)
		{
			UnderwaterShearingStaion component2 = workable.GetComponent<UnderwaterShearingStaion>();
			if (component2 != null)
			{
				global::Tuple<Tag, float> itemDroppedOnShear2 = creature.gameObject.GetSMI<IShearable>().GetItemDroppedOnShear();
				component2.UpdateShearableSymbol(itemDroppedOnShear2.first);
			}
		};
	}

	private void StoreShearable(GameObject station, GameObject critter, Tag item_dropped, float mass)
	{
		PrimaryElement component = critter.GetComponent<PrimaryElement>();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(item_dropped), null, null);
		int num = Grid.CellRight(Grid.PosToCell(critter));
		gameObject.transform.SetPosition(Grid.CellToPosCCC(num, Grid.SceneLayer.Ore));
		PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
		component2.Temperature = component.Temperature;
		component2.Mass = mass;
		component2.AddDisease(component.DiseaseIdx, component.DiseaseCount, "Shearing");
		gameObject.SetActive(true);
		Vector2 vector = new Vector2(global::UnityEngine.Random.Range(-1f, 1f) * 1f, global::UnityEngine.Random.value * 2f + 2f);
		if (GameComps.Fallers.Has(gameObject))
		{
			GameComps.Fallers.Remove(gameObject);
		}
		GameComps.Fallers.Add(gameObject, vector);
	}

	public const string ID = "UnderwaterShearingStation";
}
