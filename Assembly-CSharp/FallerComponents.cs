using System;
using UnityEngine;

public class FallerComponents : KGameObjectComponentManager<FallerComponent>
{
	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity)
	{
		return base.Add(go, new FallerComponent(go.transform, initial_velocity));
	}

	public override void Remove(GameObject go)
	{
		if (!KComponentCleanUp.InCleanUpPhase)
		{
			this.cleanupList.Add(go);
		}
		else
		{
			HandleVector<int>.Handle handle = base.GetHandle(go);
			FallerComponents.RemoveGravity(base.GetData(handle).transform);
			base.InternalRemoveComponent(go);
		}
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle h)
	{
		FallerComponent data = base.GetData(h);
		int num = Grid.PosToCell(data.transform.position);
		if (Grid.Solid[num])
		{
			data.solidChangedCB = delegate(object ev_data)
			{
				FallerComponents.OnSolidChanged(h, ev_data);
			};
			data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", data.transform.gameObject, num, GameScenePartitioner.Instance.solidChangedMask.mask, data.solidChangedCB);
			GameComps.Fallers.SetData(h, data);
		}
		else
		{
			FallerComponents.AddGravity(data.transform, data.initialVelocity);
		}
	}

	protected override void OnSpawn(HandleVector<int>.Handle h)
	{
		base.OnSpawn(h);
		FallerComponent data = base.GetData(h);
		data.transform.gameObject.Subscribe(1088554450, data.solidChangedCB);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		FallerComponent data = base.GetData(h);
		if (data.partitionerEntry != null)
		{
			data.partitionerEntry.Release();
			data.partitionerEntry = null;
			base.SetData(h, data);
		}
		if (data.solidChangedCB != null && data.transform != null)
		{
			data.transform.gameObject.Unsubscribe(1088554450, data.solidChangedCB);
			data.solidChangedCB = null;
		}
		FallerComponents.RemoveGravity(data.transform);
		base.SetData(h, data);
	}

	private static void AddGravity(Transform transform, Vector2 initial_velocity)
	{
		if (!GameComps.Gravities.Has(transform.gameObject))
		{
			GameComps.Gravities.Add(transform.gameObject, initial_velocity, delegate
			{
				FallerComponents.OnLanded(transform);
			});
			HandleVector<int>.Handle handle = GameComps.Fallers.GetHandle(transform.gameObject);
			FallerComponent data = GameComps.Fallers.GetData(handle);
			if (data.partitionerEntry != null)
			{
				data.partitionerEntry.Release();
				data.partitionerEntry = null;
				GameComps.Fallers.SetData(handle, data);
			}
		}
	}

	private static void RemoveGravity(Transform transform)
	{
		if (GameComps.Gravities.Has(transform.gameObject))
		{
			GameComps.Gravities.Remove(transform.gameObject);
			HandleVector<int>.Handle h = GameComps.Fallers.GetHandle(transform.gameObject);
			FallerComponent data = GameComps.Fallers.GetData(h);
			Action<object> action = delegate(object ev_data)
			{
				FallerComponents.OnSolidChanged(h, ev_data);
			};
			int num = Grid.PosToCell(transform.position);
			int num2 = Grid.CellBelow(num);
			if (data.partitionerEntry != null)
			{
				data.partitionerEntry.Release();
			}
			data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", transform.gameObject, num2, GameScenePartitioner.Instance.solidChangedMask.mask, action);
			GameComps.Fallers.SetData(h, data);
		}
	}

	private static void OnLanded(Transform transform)
	{
		FallerComponents.RemoveGravity(transform);
	}

	private static void OnSolidChanged(HandleVector<int>.Handle handle, object ev_data)
	{
		FallerComponent data = GameComps.Fallers.GetData(handle);
		Vector3 position = data.transform.position;
		position.y = position.y - data.offset - 0.1f;
		int num = Grid.PosToCell(position);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		bool flag = !Grid.Solid[num];
		if (flag != data.isFalling)
		{
			data.isFalling = flag;
			if (flag)
			{
				FallerComponents.AddGravity(data.transform, Vector2.zero);
			}
			else
			{
				FallerComponents.RemoveGravity(data.transform);
			}
		}
	}
}
