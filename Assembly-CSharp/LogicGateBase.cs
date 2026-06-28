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

	public enum Op
	{
		And,
		Or,
		Not,
		Xor,
		CustomSingle
	}
}
