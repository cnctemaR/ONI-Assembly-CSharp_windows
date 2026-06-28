using System;
using UnityEngine;

public class LogicGateFilterConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.CustomSingle;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef("LogicGateFILTER", "logic_filter_kanim", 2, 1);
		buildingDef.HotKey = global::Action.BuildMenuKeyB;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateFilter logicGateFilter = go.AddComponent<LogicGateFilter>();
		logicGateFilter.op = this.GetLogicOp();
	}

	public const string ID = "LogicGateFILTER";
}
