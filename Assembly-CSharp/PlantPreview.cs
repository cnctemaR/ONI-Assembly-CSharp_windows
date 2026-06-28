using System;
using UnityEngine;

public class PlantPreview : KMonoBehaviour
{
	public bool Valid { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.partitionerEntry = GameScenePartitioner.Instance.Add("PlantPreview", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedMask.mask | GameScenePartitioner.Instance.objectLayerMasks[1].mask, new Action<object>(this.OnAreaChanged));
		this.OnAreaChanged(null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
	}

	public void SetSolid()
	{
		this.occupyArea.ApplyToCells = true;
	}

	private void OnAreaChanged(object obj)
	{
		bool valid = this.Valid;
		this.Valid = this.occupyArea.TestArea(Grid.PosToCell(this), (int cell) => !Grid.Solid[cell] && (Grid.Objects[cell, 1] == base.gameObject || Grid.Objects[cell, 1] == null));
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

	private GameScenePartitionerEntry partitionerEntry;
}
