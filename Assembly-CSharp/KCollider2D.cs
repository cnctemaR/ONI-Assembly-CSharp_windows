using System;
using UnityEngine;

public abstract class KCollider2D : KMonoBehaviour, IRenderEveryTick
{
	public Vector2 offset
	{
		get
		{
			return this._offset;
		}
		set
		{
			this._offset = value;
			this.MarkDirty(false);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		CellChangeMonitor.Instance.RegisterMovementStateChanged(base.transform, new Action<bool>(this.OnMovementStateChanged));
		this.MarkDirty(true);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		CellChangeMonitor.Instance.UnregisterMovementStateChanged(base.transform, new Action<bool>(this.OnMovementStateChanged));
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
	}

	protected void MarkDirty(bool force = false)
	{
		bool flag = force || this.partitionerEntry != null;
		if (!flag)
		{
			return;
		}
		Extents extents = this.GetExtents();
		if (this.cachedExtents.x == extents.x && this.cachedExtents.y == extents.y && this.cachedExtents.width == extents.width && this.cachedExtents.height == extents.height)
		{
			return;
		}
		this.cachedExtents = this.GetExtents();
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		if (flag)
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add(base.name, this, this.cachedExtents, GameScenePartitioner.Instance.collisionLayer, null);
		}
	}

	private void OnMovementStateChanged(bool is_moving)
	{
		if (is_moving)
		{
			this.MarkDirty(false);
			SimAndRenderScheduler.instance.Add(this, false);
		}
		else
		{
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	public void RenderEveryTick(float dt)
	{
		this.MarkDirty(false);
	}

	public abstract bool Intersects(Vector2 pos);

	public abstract Extents GetExtents();

	public abstract Bounds bounds { get; }

	[SerializeField]
	public Vector2 _offset;

	private Extents cachedExtents;

	private GameScenePartitionerEntry partitionerEntry;
}
