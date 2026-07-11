using System;
using STRINGS;

public class LogicGateNotConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Not;
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
				name = BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateNOT", "logic_not_kanim", 2, 1);
	}

	public const string ID = "LogicGateNOT";
}
