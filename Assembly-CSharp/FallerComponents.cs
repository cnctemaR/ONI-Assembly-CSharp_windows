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
		HandleVector<int>.Handle handle = base.GetHandle(go);
		this.OnCleanUpImmediate(handle);
		KComponentManager<FallerComponent>.CleanupInfo cleanupInfo = new KComponentManager<FallerComponent>.CleanupInfo(go, handle);
		if (!KComponentCleanUp.InCleanUpPhase)
		{
			base.AddToCleanupList(cleanupInfo);
			return;
		}
		base.InternalRemoveComponent(cleanupInfo);
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle h)
	{
		FallerComponent data = base.GetData(h);
		Vector3 position = data.transform.GetPosition();
		int num = Grid.PosToCell(position);
		data.cellChangedCB = delegate(object _)
		{
			FallerComponents.OnSolidChanged(h);
		};
		float groundOffset = GravityComponent.GetGroundOffset(data.transform.GetComponent<KCollider2D>());
		int num2 = Grid.PosToCell(new Vector3(position.x, position.y - groundOffset - 0.07f, position.z));
		bool flag = Grid.IsValidCell(num2) && Grid.Solid[num2] && data.initialVelocity.sqrMagnitude == 0f;
		if ((Grid.IsValidCell(num) && Grid.Solid[num]) || flag)
		{
			data.solidChangedCB = delegate(object ev_data)
			{
				FallerComponents.OnSolidChanged(h);
			};
			int num3 = 2;
			Vector2I vector2I = Grid.CellToXY(num);
			vector2I.y--;
			if (vector2I.y < 0)
			{
				vector2I.y = 0;
				num3 = 1;
			}
			else if (vector2I.y == Grid.HeightInCells - 1)
			{
				num3 = 1;
			}
			data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", data.transform.gameObject, vector2I.x, vector2I.y, 1, num3, GameScenePartitioner.Instance.solidChangedLayer, data.solidChangedCB);
			GameComps.Fallers.SetData(h, data);
			return;
		}
		GameComps.Fallers.SetData(h, data);
		FallerComponents.AddGravity(data.transform, data.initialVelocity);
	}

	protected override void OnSpawn(HandleVector<int>.Handle h)
	{
		base.OnSpawn(h);
		FallerComponent data = base.GetData(h);
		data.cellChangedHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(data.transform, data.cellChangedCB, null, "FallerComponent.OnSpawn");
		base.SetData(h, data);
	}

	private void OnCleanUpImmediate(HandleVector<int>.Handle h)
	{
		FallerComponent data = base.GetData(h);
		GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
		if (data.cellChangedCB != null)
		{
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref data.cellChangedHandlerID);
			data.cellChangedCB = null;
		}
		if (GameComps.Gravities.Has(data.transform.gameObject))
		{
			GameComps.Gravities.Remove(data.transform.gameObject);
		}
		base.SetData(h, data);
	}

	private static void AddGravity(Transform transform, Vector2 initial_velocity)
	{
		if (!GameComps.Gravities.Has(transform.gameObject))
		{
			GameComps.Gravities.Add(transform.gameObject, initial_velocity, FallerComponents.OnLandedAction);
			HandleVector<int>.Handle handle = GameComps.Fallers.GetHandle(transform.gameObject);
			FallerComponent data = GameComps.Fallers.GetData(handle);
			if (data.partitionerEntry.IsValid())
			{
				GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
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
			int num = Grid.CellBelow(Grid.PosToCell(transform.GetPosition()));
			GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
			if (Grid.IsValidCell(num))
			{
				data.solidChangedCB = delegate(object ev_data)
				{
					FallerComponents.OnSolidChanged(h);
				};
				data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", transform.gameObject, num, GameScenePartitioner.Instance.solidChangedLayer, data.solidChangedCB);
			}
			GameComps.Fallers.SetData(h, data);
		}
	}

	private static void OnLanded(Transform transform)
	{
		FallerComponents.RemoveGravity(transform);
	}

	private static void OnSolidChanged(HandleVector<int>.Handle handle)
	{
		FallerComponent data = GameComps.Fallers.GetData(handle);
		if (data.transform == null)
		{
			return;
		}
		Vector3 position = data.transform.GetPosition();
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
				return;
			}
			FallerComponents.RemoveGravity(data.transform);
		}
	}

	private const float EPSILON = 0.07f;

	private static Action<Transform> OnLandedAction = new Action<Transform>(FallerComponents.OnLanded);
}
