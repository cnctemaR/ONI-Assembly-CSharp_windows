using System;
using UnityEngine;

public class EntityPreview : KMonoBehaviour
{
	public bool Valid { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.storage != null)
		{
			this.storage.choreType = Db.Get().ChoreTypes.Fetch;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("EntityPreview", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnAreaChanged));
		if (this.objectLayer != ObjectLayer.NumLayers)
		{
			this.objectPartitionerEntry = GameScenePartitioner.Instance.Add("EntityPreview", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.objectLayers[(int)this.objectLayer], new Action<object>(this.OnAreaChanged));
		}
		CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnCellChange), false);
		this.OnAreaChanged(null);
	}

	protected override void OnCleanUp()
	{
		if (this.solidPartitionerEntry != null)
		{
			this.solidPartitionerEntry.Release();
			this.solidPartitionerEntry = null;
		}
		if (this.objectPartitionerEntry != null)
		{
			this.objectPartitionerEntry.Release();
			this.objectPartitionerEntry = null;
		}
		CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnCellChange), false);
		base.OnCleanUp();
	}

	private void OnCellChange(int previous_cell, int current_cell)
	{
		if (this.solidPartitionerEntry != null)
		{
			this.solidPartitionerEntry.UpdatePosition(current_cell);
		}
		if (this.objectPartitionerEntry != null)
		{
			this.objectPartitionerEntry.UpdatePosition(current_cell);
		}
		this.OnAreaChanged(null);
	}

	public void SetSolid()
	{
		this.occupyArea.ApplyToCells = true;
	}

	private void OnAreaChanged(object obj)
	{
		this.UpdateValidity();
	}

	public void UpdateValidity()
	{
		bool valid = this.Valid;
		this.Valid = this.occupyArea.TestArea(Grid.PosToCell(this), null, new Func<int, object, bool>(this.ValidTest));
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

	private bool ValidTest(int cell, object data)
	{
		bool flag = !Grid.Solid[cell];
		if (this.objectLayer != ObjectLayer.NumLayers)
		{
			bool flag2 = Grid.Objects[cell, (int)this.objectLayer] == base.gameObject || Grid.Objects[cell, (int)this.objectLayer] == null;
		}
		return !Grid.Solid[cell] && (this.objectLayer == ObjectLayer.NumLayers || Grid.Objects[cell, (int)this.objectLayer] == base.gameObject || Grid.Objects[cell, (int)this.objectLayer] == null);
	}

	[MyCmpReq]
	private OccupyArea occupyArea;

	[MyCmpReq]
	private KBatchedAnimController animController;

	[MyCmpGet]
	private Storage storage;

	public ObjectLayer objectLayer = ObjectLayer.NumLayers;

	private GameScenePartitionerEntry solidPartitionerEntry;

	private GameScenePartitionerEntry objectPartitionerEntry;
}
