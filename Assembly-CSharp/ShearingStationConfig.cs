using System;
using TUNING;
using UnityEngine;

public class ShearingStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ShearingStation";
		int num = 3;
		int num2 = 3;
		string text2 = "shearing_station_kanim";
		int num3 = 100;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.Floodable = true;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.DefaultAnimState = "on";
		buildingDef.ShowInBuildMenu = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStationType, false);
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.IsCritterEligibleToBeRanchedCb = delegate(GameObject creature_go, RanchStation.Instance ranch_station_smi)
		{
			IShearable smi = creature_go.GetSMI<IShearable>();
			return smi != null && smi.IsFullyGrown();
		};
		def.OnRanchCompleteCb = delegate(GameObject creature_go, WorkerBase rancher_wb)
		{
			IShearable smi2 = creature_go.GetSMI<IShearable>();
			global::Tuple<Tag, float> itemDroppedOnShear = smi2.GetItemDroppedOnShear();
			this.DropShearable(rancher_wb.gameObject, creature_go, itemDroppedOnShear.first, itemDroppedOnShear.second);
			smi2.Shear();
		};
		def.RancherInteractAnim = "anim_interacts_shearingstation_kanim";
		def.WorkTime = 12f;
		def.RanchedPreAnim = "shearing_pre";
		def.RanchedLoopAnim = "shearing_loop";
		def.RanchedPstAnim = "shearing_pst";
		def.RancherWipesBrowAnim = false;
		go.AddOrGet<SkillPerkMissingComplainer>().requiredSkillPerk = Db.Get().SkillPerks.CanUseRanchStation.Id;
		Prioritizable.AddRef(go);
	}

	private void DropShearable(GameObject go, GameObject critter, Tag item_dropped, float mass)
	{
		PrimaryElement component = critter.GetComponent<PrimaryElement>();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(item_dropped), null, null);
		int num = Grid.CellLeft(Grid.PosToCell(go));
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

	public const string ID = "ShearingStation";
}
