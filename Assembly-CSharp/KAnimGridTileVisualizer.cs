using System;
using Rendering;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/KAnimGridTileVisualizer")]
public class KAnimGridTileVisualizer : KMonoBehaviour, IBlockTileInfo
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<KAnimGridTileVisualizer>(-1503271301, KAnimGridTileVisualizer.OnSelectionChangedDelegate);
		base.Subscribe<KAnimGridTileVisualizer>(-1201923725, KAnimGridTileVisualizer.OnHighlightChangedDelegate);
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
			TileVisualizer.RefreshCell(num, tileLayer, component.Def.ReplacementLayer);
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

	private static readonly EventSystem.IntraObjectHandler<KAnimGridTileVisualizer> OnSelectionChangedDelegate = new EventSystem.IntraObjectHandler<KAnimGridTileVisualizer>(delegate(KAnimGridTileVisualizer component, object data)
	{
		component.OnSelectionChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<KAnimGridTileVisualizer> OnHighlightChangedDelegate = new EventSystem.IntraObjectHandler<KAnimGridTileVisualizer>(delegate(KAnimGridTileVisualizer component, object data)
	{
		component.OnHighlightChanged(data);
	});
}
