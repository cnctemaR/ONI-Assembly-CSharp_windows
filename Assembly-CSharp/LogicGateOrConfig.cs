using System;

public class LogicGateOrConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Or;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateOR", "logic_or_kanim", 2, 2);
	}

	public const string ID = "LogicGateOR";
}
