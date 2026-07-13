using System;
using System.Runtime.CompilerServices;
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
		this.movementStateChangedHandlerId = Singleton<CellChangeMonitor>.Instance.RegisterMovementStateChanged(base.transform, KCollider2D.OnMovementStateChangedDispatcher, this);
		this.MarkDirty(true);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Singleton<CellChangeMonitor>.Instance.UnregisterMovementStateChanged(ref this.movementStateChangedHandlerId);
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
	}

	public void MarkDirty(bool force = false)
	{
		bool flag = force || this.partitionerEntry.IsValid();
		if (!flag)
		{
			return;
		}
		Extents extents = this.GetExtents();
		if (!force && this.cachedExtents.x == extents.x && this.cachedExtents.y == extents.y && this.cachedExtents.width == extents.width && this.cachedExtents.height == extents.height)
		{
			return;
		}
		this.cachedExtents = extents;
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		if (flag)
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add(null, this, this.cachedExtents, GameScenePartitioner.Instance.collisionLayer, null);
		}
	}

	private void OnMovementStateChanged(bool is_moving)
	{
		if (is_moving)
		{
			this.MarkDirty(false);
			SimAndRenderScheduler.instance.Add(this, false);
			return;
		}
		SimAndRenderScheduler.instance.Remove(this);
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

	private HandleVector<int>.Handle partitionerEntry;

	private ulong movementStateChangedHandlerId;

	private static Action<Transform, bool, object> OnMovementStateChangedDispatcher = delegate(Transform transform, bool is_moving, object context)
	{
		Unsafe.As<KCollider2D>(context).OnMovementStateChanged(is_moving);
	};
}
