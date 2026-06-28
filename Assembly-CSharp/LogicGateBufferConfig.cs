using System;
using UnityEngine;

public class LogicGateBufferConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.CustomSingle;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateBUFFER", "logic_buffer_kanim", 2, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateBuffer logicGateBuffer = go.AddComponent<LogicGateBuffer>();
		logicGateBuffer.op = this.GetLogicOp();
	}

	public const string ID = "LogicGateBUFFER";
}
