using System;
using STRINGS;
using UnityEngine;

public class LogicGateFilterConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.CustomSingle;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		return new LogicGate.LogicGateDescriptions
		{
			output = new LogicGate.LogicGateDescriptions.Description
			{
				name = BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateFILTER", "logic_filter_kanim", 2, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateFilter logicGateFilter = go.AddComponent<LogicGateFilter>();
		logicGateFilter.op = this.GetLogicOp();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LogicGateFilter component = game_object.GetComponent<LogicGateFilter>();
			component.SetPortDescriptions(this.GetDescriptions());
		};
	}

	public const string ID = "LogicGateFILTER";
}
