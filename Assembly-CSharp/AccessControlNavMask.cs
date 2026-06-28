using System;
using UnityEngine;

public class AccessControlNavMask : NavMask
{
	public AccessControlNavMask(GameObject agent, int max_path_cost)
	{
		this.agent = agent;
		this.maxPathCost = max_path_cost;
	}

	public void SetMaxPathCost(int max_path_cost)
	{
		this.maxPathCost = max_path_cost;
	}

	public override bool IsTraversable(PathFinder.PotentialPath path, int from_cell, int cost, PathFinderAbilities abilities)
	{
		if (cost > this.maxPathCost)
		{
			return false;
		}
		int cell = path.cell;
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
		AccessControl.Permission permission = component.GetPermission(this.agent);
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

	private GameObject agent;

	private int maxPathCost = int.MaxValue;
}
