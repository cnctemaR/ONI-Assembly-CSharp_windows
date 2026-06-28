using System;

[SkipSaveFileSerialization]
public class Approachable : KMonoBehaviour, IApproachable
{
	public CellOffset[] GetOffsets()
	{
		return OffsetGroups.Use;
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
		return this.preferUnreservedCell;
	}

	public bool preferUnreservedCell;
}
