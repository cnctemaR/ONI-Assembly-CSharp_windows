using System;
using STRINGS;

public class LogicGateAndConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.And;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		return new LogicGate.LogicGateDescriptions
		{
			output = new LogicGate.LogicGateDescriptions.Description
			{
				name = BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateAND", "logic_and_kanim", 2, 2);
	}

	public const string ID = "LogicGateAND";
}
