using System;
using STRINGS;
using UnityEngine;

public class LogicGateBufferConfig : LogicGateBaseConfig
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
				name = BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateBUFFER", "logic_buffer_kanim", 2, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateBuffer logicGateBuffer = go.AddComponent<LogicGateBuffer>();
		logicGateBuffer.op = this.GetLogicOp();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LogicGateBuffer component = game_object.GetComponent<LogicGateBuffer>();
			component.SetPortDescriptions(this.GetDescriptions());
		};
	}

	public const string ID = "LogicGateBUFFER";
}
