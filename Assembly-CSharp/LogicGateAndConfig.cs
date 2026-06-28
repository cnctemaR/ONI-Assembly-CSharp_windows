using System;

public class LogicGateAndConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.And;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef("LogicGateAND", "logic_and_kanim", 2, 2);
		buildingDef.HotKey = global::Action.BuildMenuKeyA;
		return buildingDef;
	}

	public const string ID = "LogicGateAND";
}
