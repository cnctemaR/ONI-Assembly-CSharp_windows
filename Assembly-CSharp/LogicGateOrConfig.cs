using System;

public class LogicGateOrConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Or;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef("LogicGateOR", "logic_or_kanim", 2, 2);
		buildingDef.HotKey = global::Action.BuildMenuKeyR;
		return buildingDef;
	}

	public const string ID = "LogicGateOR";
}
