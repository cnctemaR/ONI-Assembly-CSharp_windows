using System;
using UnityEngine;

public class PlantPreview : KMonoBehaviour
{
	public bool Valid { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("PlantPreview", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnAreaChanged));
		this.buildingPartitionerEntry = GameScenePartitioner.Instance.Add("PlantPreview", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnAreaChanged));
		this.OnAreaChanged(null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.solidPartitionerEntry != null)
		{
			this.solidPartitionerEntry.Release();
			this.solidPartitionerEntry = null;
		}
		if (this.buildingPartitionerEntry != null)
		{
			this.buildingPartitionerEntry.Release();
			this.buildingPartitionerEntry = null;
		}
	}

	public void SetSolid()
	{
		this.occupyArea.ApplyToCells = true;
	}

	private void OnAreaChanged(object obj)
	{
		bool valid = this.Valid;
		this.Valid = this.occupyArea.TestArea(Grid.PosToCell(this), null, (int cell, object data) => !Grid.Solid[cell] && (Grid.Objects[cell, 1] == base.gameObject || Grid.Objects[cell, 1] == null));
		if (this.Valid)
		{
			this.animController.TintColour = Color.white;
		}
		else
		{
			this.animController.TintColour = Color.red;
		}
		if (valid != this.Valid)
		{
			this.Trigger(-1820564715, this.Valid);
		}
	}

	[MyCmpReq]
	private OccupyArea occupyArea;

	[MyCmpReq]
	private KBatchedAnimController animController;

	private GameScenePartitionerEntry solidPartitionerEntry;

	private GameScenePartitionerEntry buildingPartitionerEntry;
}
