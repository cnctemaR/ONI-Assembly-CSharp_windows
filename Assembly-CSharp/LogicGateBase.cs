using System;
using UnityEngine;

public class LogicGateBase : KMonoBehaviour
{
	private int GetActualCell(CellOffset offset)
	{
		Rotatable component = base.GetComponent<Rotatable>();
		if (component != null)
		{
			offset = component.GetRotatedCellOffset(offset);
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		return Grid.OffsetCell(num, offset);
	}

	public int InputCellOne
	{
		get
		{
			return this.GetActualCell(LogicGateBase.portOffsets[0]);
		}
	}

	public int InputCellTwo
	{
		get
		{
			return this.GetActualCell(LogicGateBase.portOffsets[1]);
		}
	}

	public int OutputCell
	{
		get
		{
			return this.GetActualCell(LogicGateBase.portOffsets[2]);
		}
	}

	public int PortCell(LogicGateBase.PortId port)
	{
		switch (port)
		{
		case LogicGateBase.PortId.InputOne:
			return this.InputCellOne;
		case LogicGateBase.PortId.InputTwo:
			return this.InputCellTwo;
		}
		return this.OutputCell;
	}

	public bool TryGetPortAtCell(int cell, out LogicGateBase.PortId port)
	{
		if (cell == this.InputCellOne)
		{
			port = LogicGateBase.PortId.InputOne;
			return true;
		}
		if (cell == this.InputCellTwo && this.RequiresTwoInputs)
		{
			port = LogicGateBase.PortId.InputTwo;
			return true;
		}
		if (cell == this.OutputCell)
		{
			port = LogicGateBase.PortId.Output;
			return true;
		}
		port = LogicGateBase.PortId.InputOne;
		return false;
	}

	public bool RequiresTwoInputs
	{
		get
		{
			return LogicGateBase.OpRequiresTwoInputs(this.op);
		}
	}

	public static bool OpRequiresTwoInputs(LogicGateBase.Op op)
	{
		return op != LogicGateBase.Op.Not && op != LogicGateBase.Op.CustomSingle;
	}

	public static LogicModeUI uiSrcData;

	[SerializeField]
	public LogicGateBase.Op op;

	public static CellOffset[] portOffsets = new CellOffset[]
	{
		CellOffset.none,
		new CellOffset(0, 1),
		new CellOffset(1, 0)
	};

	public enum PortId
	{
		InputOne,
		InputTwo,
		Output
	}

	public enum Op
	{
		And,
		Or,
		Not,
		Xor,
		CustomSingle
	}
}
