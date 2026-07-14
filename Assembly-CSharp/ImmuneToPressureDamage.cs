using System;

public class ImmuneToPressureDamage : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		foreach (CellOffset cellOffset in this.Cells)
		{
			SimMessages.SetCellProperties(Grid.OffsetCell(Grid.PosToCell(this), cellOffset), 8, -1);
		}
		base.OnPrefabInit();
	}

	protected override void OnCleanUp()
	{
		foreach (CellOffset cellOffset in this.Cells)
		{
			SimMessages.ClearCellProperties(Grid.OffsetCell(Grid.PosToCell(this), cellOffset), 8, -1);
		}
	}

	public CellOffset[] Cells = new CellOffset[]
	{
		new CellOffset(0, 0)
	};
}
