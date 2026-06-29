using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class UpdateBucketWithUpdater<DataType> : StateMachineUpdater.BaseUpdateBucket
{
	public UpdateBucketWithUpdater(string name)
		: base(name)
	{
	}

	public override int count
	{
		get
		{
			return this.entries.Count;
		}
	}

	public HandleVector<int>.Handle Add(DataType data, float last_update_time, UpdateBucketWithUpdater<DataType>.IUpdater updater)
	{
		UpdateBucketWithUpdater<DataType>.Entry entry = default(UpdateBucketWithUpdater<DataType>.Entry);
		entry.data = data;
		entry.lastUpdateTime = last_update_time;
		entry.updater = updater;
		HandleVector<int>.Handle handle = this.entries.Allocate(entry);
		this.entries.SetData(handle, entry);
		return handle;
	}

	public override void Remove(HandleVector<int>.Handle handle)
	{
		this.pendingRemovals.Add(handle);
		UpdateBucketWithUpdater<DataType>.Entry data = this.entries.GetData(handle);
		data.updater = null;
		this.entries.SetData(handle, data);
	}

	public override void Update(float dt)
	{
		List<UpdateBucketWithUpdater<DataType>.Entry> dataList = this.entries.GetDataList();
		int count = dataList.Count;
		for (int i = 0; i < count; i++)
		{
			UpdateBucketWithUpdater<DataType>.Entry entry = dataList[i];
			if (entry.updater != null)
			{
				entry.updater.Update(entry.data, dt - entry.lastUpdateTime);
				entry.lastUpdateTime = 0f;
				dataList[i] = entry;
			}
		}
		foreach (HandleVector<int>.Handle handle in this.pendingRemovals)
		{
			this.entries.Free(handle);
		}
		this.pendingRemovals.Clear();
	}

	private KCompactedVector<UpdateBucketWithUpdater<DataType>.Entry> entries = new KCompactedVector<UpdateBucketWithUpdater<DataType>.Entry>(0);

	private List<HandleVector<int>.Handle> pendingRemovals = new List<HandleVector<int>.Handle>();

	private float accumulatedtime;

	public struct Entry
	{
		public DataType data;

		public float lastUpdateTime;

		public UpdateBucketWithUpdater<DataType>.IUpdater updater;
	}

	public interface IUpdater
	{
		void Update(DataType smi, float dt);
	}
}
