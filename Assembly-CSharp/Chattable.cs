using System;
using UnityEngine;

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

	Transform IApproachable.get_transform()
	{
		return base.transform;
	}
}
