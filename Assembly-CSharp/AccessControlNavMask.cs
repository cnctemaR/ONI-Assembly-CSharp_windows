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
		bool flag;
		if (!Grid.HasAccessDoor[cell])
		{
			flag = true;
		}
		else
		{
			GameObject gameObject = Grid.Objects[cell, 1];
			if (gameObject == null)
			{
				flag = true;
			}
			else
			{
				AccessControl component = gameObject.GetComponent<AccessControl>();
				if (component == null)
				{
					flag = true;
				}
				else
				{
					AccessControl.Permission permission = component.GetPermission(this.agent.gameObject);
					if (permission == AccessControl.Permission.Neither)
					{
						flag = false;
					}
					else if (permission == AccessControl.Permission.Both)
					{
						flag = true;
					}
					else
					{
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
						flag = false;
					}
				}
			}
		}
		return flag;
	}

	public override bool IsTraversable(PathFinder.PotentialPath path, int from_cell, int cost, int transition_id, PathFinderAbilities abilities)
	{
		bool flag;
		if (!this.CanTraverse(path.cell, from_cell))
		{
			flag = false;
		}
		else
		{
			NavGrid.Transition transition = this.agent.NavGrid.transitions[transition_id];
			for (int i = 0; i < transition.voidOffsets.Length; i++)
			{
				int num = Grid.OffsetCell(from_cell, transition.voidOffsets[i]);
				if (!this.CanTraverse(num, from_cell))
				{
					return false;
				}
			}
			flag = true;
		}
		return flag;
	}

	private Navigator agent;
}
