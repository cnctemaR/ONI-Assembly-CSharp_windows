using System;

public class LogicGateNotConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Not;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateNOT", "logic_not_kanim", 2, 1);
	}

	public const string ID = "LogicGateNOT";
}
