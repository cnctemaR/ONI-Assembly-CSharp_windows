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
}
