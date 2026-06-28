using System;
using UnityEngine;

public class AntiCluster : KMonoBehaviour, ISim200ms
{
	public void Sim200ms(float dt)
	{
		int num = Grid.PosToCell(this);
		this.UpdateCell(this.previousCell, num);
		if (this.previousCell != Grid.InvalidCell)
		{
			this.UpdateCell(Grid.CellAbove(this.previousCell), Grid.CellAbove(num));
		}
		else
		{
			this.UpdateCell(this.previousCell, Grid.CellAbove(num));
		}
		this.previousCell = num;
	}

	private void UpdateCell(int previous_cell, int current_cell)
	{
		if (previous_cell != Grid.InvalidCell && previous_cell != current_cell && Grid.Objects[previous_cell, 0] == base.gameObject)
		{
			Grid.Objects[previous_cell, 0] = null;
		}
		GameObject gameObject = Grid.Objects[current_cell, 0];
		if (gameObject == null)
		{
			Grid.Objects[current_cell, 0] = base.gameObject;
		}
		else
		{
			KPrefabID component = base.GetComponent<KPrefabID>();
			KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
			if (component.InstanceID > component2.InstanceID)
			{
				Grid.Objects[current_cell, 0] = base.gameObject;
			}
		}
	}

	private int previousCell = Grid.InvalidCell;
}
