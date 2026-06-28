using System;

public class LogicGateXorConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Xor;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef("LogicGateXOR", "logic_xor_kanim", 2, 2);
		buildingDef.HotKey = global::Action.BuildMenuKeyX;
		return buildingDef;
	}

	public const string ID = "LogicGateXOR";
}
