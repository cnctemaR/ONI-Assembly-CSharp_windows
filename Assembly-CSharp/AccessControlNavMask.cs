using System;
using UnityEngine;

internal struct AccessControlNavMask
{
	public AccessControlNavMask(Navigator navigator)
	{
		this.transitionVoidOffsets = new CellOffset[navigator.NavGrid.transitions.Length][];
		for (int i = 0; i < this.transitionVoidOffsets.Length; i++)
		{
			this.transitionVoidOffsets[i] = navigator.NavGrid.transitions[i].voidOffsets;
		}
	}

	private bool CanTraverse(Navigator agent, int cell, int from_cell)
	{
		if (!Grid.HasAccessDoor[cell])
		{
			return true;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject == null)
		{
			return true;
		}
		AccessControl component = gameObject.GetComponent<AccessControl>();
		if (component == null)
		{
			return true;
		}
		AccessControl.Permission permission = component.GetPermission(agent);
		if (permission == AccessControl.Permission.Neither)
		{
			return false;
		}
		if (permission == AccessControl.Permission.Both)
		{
			return true;
		}
		Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.NoLayer) - Grid.CellToPosCCC(from_cell, Grid.SceneLayer.NoLayer);
		Door component2 = component.GetComponent<Door>();
		if (component2.GetComponent<Rotatable>().IsRotated)
		{
			if ((permission == AccessControl.Permission.GoLeft && vector.y > 0f) || (permission == AccessControl.Permission.GoRight && vector.y < 0f))
			{
				return true;
			}
		}
		else if ((permission == AccessControl.Permission.GoLeft && vector.x < 0f) || (permission == AccessControl.Permission.GoRight && vector.x > 0f))
		{
			return true;
		}
		return false;
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, int transition_id)
	{
		if (!this.CanTraverse(agent, path.cell, from_cell))
		{
			return false;
		}
		foreach (CellOffset cellOffset in this.transitionVoidOffsets[transition_id])
		{
			int num = Grid.OffsetCell(from_cell, cellOffset);
			if (!this.CanTraverse(agent, num, from_cell))
			{
				return false;
			}
		}
		return true;
	}

	private CellOffset[][] transitionVoidOffsets;
}
