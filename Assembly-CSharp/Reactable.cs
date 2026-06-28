using System;
using UnityEngine;

public class Reactable
{
	public Reactable(GameObject gameObject, int range_width = 15, int range_height = 8)
	{
		this.range_height = range_height;
		this.range_width = range_width;
		this.gameObject = gameObject;
		this.UpdateLocation(null);
		this.update_locataion_scheduler_handle = GameScheduler.Instance.SchedulePeriodic("UpdateReactableLocation", 1f, new Action<object>(this.UpdateLocation), null, null, 0f, null);
	}

	public GameObject sourceGameObject
	{
		get
		{
			return this.gameObject;
		}
	}

	public void OnCleanUp()
	{
		this.update_locataion_scheduler_handle.Clear();
		if (this.partitioner_entry != null)
		{
			this.partitioner_entry.Release();
			this.partitioner_entry = null;
		}
	}

	public void UpdateLocation(object data = null)
	{
		if (this.partitioner_entry != null)
		{
			this.partitioner_entry.Release();
			this.partitioner_entry = null;
		}
		if (this.gameObject != null)
		{
			this.CellPosition = Grid.PosToCell(this.gameObject);
			Extents extents = new Extents(Grid.PosToXY(this.gameObject.transform.position).x - this.range_width / 2, Grid.PosToXY(this.gameObject.transform.position).y - this.range_height / 2, this.range_width, this.range_height);
			this.partitioner_entry = GameScenePartitioner.Instance.Add("Reactable", this, extents, GameScenePartitioner.Instance.objectLayerMasks[0].mask, null);
		}
	}

	public Expression expression = Db.Get().Expressions.Uncomfortable;

	public Thought thought = Db.Get().Thoughts.Unhappy;

	private GameScenePartitionerEntry partitioner_entry;

	private GameObject gameObject;

	public int CellPosition;

	private int range_width;

	private int range_height;

	private SchedulerHandle update_locataion_scheduler_handle;
}
