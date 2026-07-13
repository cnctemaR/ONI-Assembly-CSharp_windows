using System;
using System.Collections.Generic;

public class PlantElementAbsorbers : KCompactedVector<PlantElementAbsorber>
{
	public HandleVector<int>.Handle Add(Storage storage, PlantElementAbsorber.ConsumeInfo[] consumed_elements)
	{
		if (consumed_elements == null || consumed_elements.Length == 0)
		{
			return HandleVector<int>.InvalidHandle;
		}
		if (consumed_elements.Length == 1)
		{
			return base.Allocate(new PlantElementAbsorber
			{
				storage = storage,
				consumedElements = null,
				localInfo = new PlantElementAbsorber.LocalInfo
				{
					tag = consumed_elements[0].tag,
					massConsumptionRate = consumed_elements[0].massConsumptionRate
				}
			});
		}
		return base.Allocate(new PlantElementAbsorber
		{
			storage = storage,
			consumedElements = consumed_elements,
			localInfo = new PlantElementAbsorber.LocalInfo
			{
				tag = Tag.Invalid,
				massConsumptionRate = 0f
			}
		});
	}

	public HandleVector<int>.Handle Remove(HandleVector<int>.Handle h)
	{
		if (this.updating)
		{
			this.queuedRemoves.Add(h);
		}
		else
		{
			base.Free(h);
		}
		return HandleVector<int>.InvalidHandle;
	}

	public void Sim200ms(float dt)
	{
		int count = this.data.Count;
		this.updating = true;
		ListPool<PlantElementAbsorber.Planner.ConsumeCommand, PlantElementAbsorbers>.PooledList pooledList = ListPool<PlantElementAbsorber.Planner.ConsumeCommand, PlantElementAbsorbers>.Allocate();
		for (int i = 0; i < count; i++)
		{
			PlantElementAbsorber plantElementAbsorber = this.data[i];
			pooledList.Clear();
			if (plantElementAbsorber.PlanConsume(dt, pooledList))
			{
				foreach (PlantElementAbsorber.Planner.ConsumeCommand consumeCommand in pooledList)
				{
					consumeCommand.primaryElement.Mass -= consumeCommand.deltaMass;
					plantElementAbsorber.storage.Trigger(-1697596308, consumeCommand.primaryElement.gameObject);
				}
				this.data[i] = plantElementAbsorber;
			}
		}
		pooledList.Recycle();
		this.updating = false;
		for (int j = 0; j < this.queuedRemoves.Count; j++)
		{
			HandleVector<int>.Handle handle = this.queuedRemoves[j];
			this.Remove(handle);
		}
		this.queuedRemoves.Clear();
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < this.data.Count; i++)
		{
			this.data[i].Clear();
		}
		this.data.Clear();
		this.handles.Clear();
	}

	public PlantElementAbsorbers()
		: base(0)
	{
	}

	private bool updating;

	private List<HandleVector<int>.Handle> queuedRemoves = new List<HandleVector<int>.Handle>();
}
