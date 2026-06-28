using System;
using UnityEngine;

public class BuildingUnderConstruction : Building
{
	protected override void OnPrefabInit()
	{
		Vector3 position = this.transform.position;
		position.z = Grid.GetLayerZ(this.Def.SceneLayer);
		this.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Construction"));
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			Vector3 visualizerOffset = this.Def.GetVisualizerOffset();
			Rotatable component2 = base.GetComponent<Rotatable>();
			if (component2 != null)
			{
				component.Rotation = component2.GetVisualizerRotation();
				component.Pivot = component2.GetVisualizerPivot();
			}
			component.Offset = visualizerOffset;
			BoxCollider2D component3 = base.GetComponent<BoxCollider2D>();
			component3.offset += new Vector2(visualizerOffset.x, visualizerOffset.y);
		}
		if (this.Def.IsTilePiece)
		{
			int num = Grid.PosToCell(this.transform.position);
			this.Def.RunOnArea(num, base.Orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, this.Def.TileLayer);
			});
		}
		base.RegisterBlockTileRenderer();
	}

	protected override void OnCleanUp()
	{
		base.UnregisterBlockTileRenderer();
		base.OnCleanUp();
	}

	[MyCmpAdd]
	private KSelectable selectable;

	[MyCmpAdd]
	private SaveLoadRoot saveLoadRoot;

	[MyCmpAdd]
	private KPrefabID kPrefabID;

	[MyCmpAdd]
	private Cancellable cancellable;
}
