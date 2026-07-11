using System;
using STRINGS;
using UnityEngine;

public class LogicGateBufferConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.CustomSingle;
	}

	protected override CellOffset[] InputPortOffsets
	{
		get
		{
			return new CellOffset[] { CellOffset.none };
		}
	}

	protected override CellOffset[] OutputPortOffsets
	{
		get
		{
			return new CellOffset[]
			{
				new CellOffset(1, 0)
			};
		}
	}

	protected override CellOffset[] ControlPortOffsets
	{
		get
		{
			return null;
		}
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		return new LogicGate.LogicGateDescriptions
		{
			outputOne = new LogicGate.LogicGateDescriptions.Description
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
		logicGateBuffer.inputPortOffsets = this.InputPortOffsets;
		logicGateBuffer.outputPortOffsets = this.OutputPortOffsets;
		logicGateBuffer.controlPortOffsets = this.ControlPortOffsets;
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			game_object.GetComponent<LogicGateBuffer>().SetPortDescriptions(this.GetDescriptions());
		};
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
	}

	public const string ID = "LogicGateBUFFER";
}
