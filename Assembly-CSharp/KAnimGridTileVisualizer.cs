using System;
using Rendering;
using UnityEngine;

[SkipSaveFileSerialization]
public class KAnimGridTileVisualizer : KMonoBehaviour, IBlockTileInfo
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-1503271301, new Action<object>(this.OnSelectionChanged));
		base.Subscribe(-1201923725, new Action<object>(this.OnHighlightChanged));
	}

	protected override void OnCleanUp()
	{
		Building component = base.GetComponent<Building>();
		if (component != null)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			ObjectLayer tileLayer = component.Def.TileLayer;
			if (Grid.Objects[num, (int)tileLayer] == base.gameObject)
			{
				Grid.Objects[num, (int)tileLayer] = null;
			}
			TileVisualizer.RefreshCell(num, tileLayer);
		}
		base.OnCleanUp();
	}

	private void OnSelectionChanged(object data)
	{
		bool flag = (bool)data;
		World.Instance.blockTileRenderer.SelectCell(Grid.PosToCell(base.transform.GetPosition()), flag);
	}

	private void OnHighlightChanged(object data)
	{
		bool flag = (bool)data;
		World.Instance.blockTileRenderer.HighlightCell(Grid.PosToCell(base.transform.GetPosition()), flag);
	}

	public int GetBlockTileConnectorID()
	{
		return this.blockTileConnectorID;
	}

	[SerializeField]
	public int blockTileConnectorID;
}
