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

	Transform IApproachable.get_transform()
	{
		return base.transform;
	}
}
