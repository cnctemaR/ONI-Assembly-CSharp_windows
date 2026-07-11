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

	Transform IApproachable.get_transform()
	{
		return base.transform;
	}
}
