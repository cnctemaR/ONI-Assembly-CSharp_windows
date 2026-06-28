using System;

public class LogicGateAndConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.And;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateAND", "logic_and_kanim", 2, 2);
	}

	public const string ID = "LogicGateAND";
}
