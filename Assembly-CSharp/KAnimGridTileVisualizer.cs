using System;
using Rendering;
using UnityEngine;

[SkipSaveFileSerialization]
public class KAnimGridTileVisualizer : KMonoBehaviour, IBlockTileInfo
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(-1503271301, new Action<object>(this.OnSelectionChanged));
		this.Subscribe(-1201923725, new Action<object>(this.OnHighlightChanged));
	}

	protected override void OnCleanUp()
	{
		Building component = base.GetComponent<Building>();
		if (component != null)
		{
			int num = Grid.PosToCell(this.transform.position);
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
		World.Instance.blockTileRenderer.SelectCell(Grid.PosToCell(this.transform.position), flag);
	}

	private void OnHighlightChanged(object data)
	{
		bool flag = (bool)data;
		World.Instance.blockTileRenderer.HighlightCell(Grid.PosToCell(this.transform.position), flag);
	}

	public int GetBlockTileConnectorID()
	{
		return this.blockTileConnectorID;
	}

	[SerializeField]
	public int blockTileConnectorID;
}
