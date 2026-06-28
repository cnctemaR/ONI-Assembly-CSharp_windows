using System;
using UnityEngine;

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

	Transform IApproachable.get_transform()
	{
		return base.transform;
	}

	public bool preferUnreservedCell = false;
}
