using System;
using UnityEngine;

public interface IApproachable
{
	CellOffset[] GetOffsets();

	int GetCell();

	bool ShouldPreferPrimaryCell();

	bool ShouldPreferUnreservedCell();

	Transform transform { get; }
}
