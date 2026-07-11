using System;
using UnityEngine;

public interface IApproachable
{
	CellOffset[] GetOffsets();

	int GetCell();

	Transform transform { get; }
}
