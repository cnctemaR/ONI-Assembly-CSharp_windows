using System;
using UnityEngine;

public class AntiCluster : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnCellChange), false);
	}

	protected override void OnCleanUp()
	{
		CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnCellChange), false);
	}

	private void OnCellChange(int previous_cell, int new_cell)
	{
		if (Grid.IsValidCell(previous_cell))
		{
			GameObject gameObject = Grid.Objects[previous_cell, 0];
			if (gameObject == base.gameObject)
			{
				Grid.Objects[previous_cell, 0] = null;
			}
		}
		if (Grid.IsValidCell(new_cell))
		{
			GameObject gameObject2 = Grid.Objects[new_cell, 0];
			if (gameObject2 == null)
			{
				Grid.Objects[new_cell, 0] = base.gameObject;
			}
			else
			{
				KPrefabID component = base.GetComponent<KPrefabID>();
				KPrefabID component2 = gameObject2.GetComponent<KPrefabID>();
				if (component.InstanceID > component2.InstanceID)
				{
					Grid.Objects[new_cell, 0] = base.gameObject;
				}
				this.Trigger(-517744704, component2.gameObject);
				component2.Trigger(-517744704, component.gameObject);
			}
		}
	}
}
