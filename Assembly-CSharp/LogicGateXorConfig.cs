using System;

public class LogicGateXorConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Xor;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateXOR", "logic_xor_kanim", 2, 2);
	}

	public const string ID = "LogicGateXOR";
}
