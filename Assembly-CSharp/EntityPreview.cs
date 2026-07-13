using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntityPreview")]
public class EntityPreview : KMonoBehaviour
{
	public bool Valid { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("EntityPreview", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnAreaChanged));
		if (this.objectLayer != ObjectLayer.NumLayers)
		{
			this.objectPartitionerEntry = GameScenePartitioner.Instance.Add("EntityPreview", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.objectLayers[(int)this.objectLayer], new Action<object>(this.OnAreaChanged));
		}
		this.cellChangeHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, EntityPreview.OnCellChangeDispatcher, this, "EntityPreview.OnSpawn");
		this.OnAreaChanged(null);
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref this.objectPartitionerEntry);
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref this.cellChangeHandlerID);
		base.OnCleanUp();
	}

	private void OnCellChange()
	{
		GameScenePartitioner.Instance.UpdatePosition(this.solidPartitionerEntry, this.occupyArea.GetExtents());
		GameScenePartitioner.Instance.UpdatePosition(this.objectPartitionerEntry, this.occupyArea.GetExtents());
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
		this.Valid = this.occupyArea.TestArea(Grid.PosToCell(this), this, EntityPreview.ValidTestDelegate);
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
			base.Trigger(-1820564715, BoxedBools.Box(this.Valid));
		}
	}

	private static bool ValidTest(int cell, object data)
	{
		EntityPreview entityPreview = (EntityPreview)data;
		return Grid.IsValidCell(cell) && !Grid.Solid[cell] && (entityPreview.objectLayer == ObjectLayer.NumLayers || Grid.Objects[cell, (int)entityPreview.objectLayer] == entityPreview.gameObject || Grid.Objects[cell, (int)entityPreview.objectLayer] == null);
	}

	[MyCmpReq]
	private OccupyArea occupyArea;

	[MyCmpReq]
	private KBatchedAnimController animController;

	[MyCmpGet]
	private Storage storage;

	public ObjectLayer objectLayer = ObjectLayer.NumLayers;

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle objectPartitionerEntry;

	private ulong cellChangeHandlerID;

	private static readonly Action<object> OnCellChangeDispatcher = delegate(object obj)
	{
		Unsafe.As<EntityPreview>(obj).OnCellChange();
	};

	private static readonly Func<int, object, bool> ValidTestDelegate = (int cell, object data) => EntityPreview.ValidTest(cell, data);
}
