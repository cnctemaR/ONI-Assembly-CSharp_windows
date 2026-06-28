using System;

public class Chattable : KMonoBehaviour, IApproachable
{
	public CellOffset[] GetOffsets()
	{
		return OffsetGroups.Chat;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public bool ShouldPreferPrimaryCell()
	{
		return false;
	}

	public bool ShouldPreferUnreservedCell()
	{
		return false;
	}
}
