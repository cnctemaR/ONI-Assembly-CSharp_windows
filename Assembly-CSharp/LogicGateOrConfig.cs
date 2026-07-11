using System;
using STRINGS;

public class LogicGateOrConfig : LogicGateBaseConfig
{
	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Or;
	}

	protected override CellOffset[] InputPortOffsets
	{
		get
		{
			return new CellOffset[]
			{
				CellOffset.none,
				new CellOffset(0, 1)
			};
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
				name = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("LogicGateOR", "logic_or_kanim", 2, 2);
	}

	public const string ID = "LogicGateOR";
}
