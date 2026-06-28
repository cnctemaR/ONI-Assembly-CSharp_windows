using System;
using UnityEngine;

public class BuildingUnderConstruction : Building
{
	protected override void OnPrefabInit()
	{
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(this.Def.SceneLayer);
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Construction"));
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		Rotatable component2 = base.GetComponent<Rotatable>();
		if (component != null && component2 == null)
		{
			component.Offset = this.Def.GetVisualizerOffset() + this.Def.placementPivot;
		}
		KBoxCollider2D component3 = base.GetComponent<KBoxCollider2D>();
		if (component3 != null)
		{
			Vector3 visualizerOffset = this.Def.GetVisualizerOffset();
			component3.offset += new Vector2(visualizerOffset.x, visualizerOffset.y);
		}
		if (this.Def.IsTilePiece)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
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
