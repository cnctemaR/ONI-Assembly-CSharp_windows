using System;
using UnityEngine;

public class AccessControlNavMask : NavMask
{
	public AccessControlNavMask(GameObject agent)
	{
		this.agent = agent.GetComponent<Navigator>();
	}

	private bool CanTraverse(int cell, int from_cell)
	{
		if (!Grid.HasAccessDoor[cell])
		{
		}
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
		AccessControl.Permission permission = component.GetPermission(this.agent.gameObject);
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

	public override bool IsTraversable(PathFinder.PotentialPath path, int from_cell, int cost, int transition_id, PathFinderAbilities abilities)
	{
		if (!this.CanTraverse(path.cell, from_cell))
		{
			return false;
		}
		NavGrid.Transition transition = this.agent.NavGrid.transitions[transition_id];
		for (int i = 0; i < transition.voidOffsets.Length; i++)
		{
			int num = Grid.OffsetCell(from_cell, transition.voidOffsets[i]);
			if (!this.CanTraverse(num, from_cell))
			{
				return false;
			}
		}
		return true;
	}

	private Navigator agent;
}
