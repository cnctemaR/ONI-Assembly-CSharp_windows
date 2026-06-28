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

	public override bool IsTraversable(int cell, int from_cell, int cost, PathFinderAbilities abilities)
	{
		if (cost > this.maxPathCost)
		{
			return false;
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
		Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.NoLayer) - Grid.CellToPosCCC(from_cell, Grid.SceneLayer.NoLayer);
		return permission == AccessControl.Permission.Both || (permission == AccessControl.Permission.GoLeft && vector.x < 0f) || (permission == AccessControl.Permission.GoRight && vector.x > 0f);
	}

	private GameObject agent;

	private int maxPathCost = int.MaxValue;
}
