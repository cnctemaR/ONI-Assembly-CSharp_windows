using System;

public class OutputPoint : KMonoBehaviour
{
	public int GetOutputCell()
	{
		return Grid.CellBelow(Grid.CellBelow(Grid.PosToCell(this)));
	}

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Operational operational;

	public static Operational.Flag outputClearFlag = new Operational.Flag("output_clear", Operational.Flag.Type.Requirement);
}
