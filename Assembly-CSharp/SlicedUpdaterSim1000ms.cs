using System;
using System.Collections.Generic;
using KSerialization;

public abstract class SlicedUpdaterSim1000ms<T> : KMonoBehaviour, ISim200ms where T : KMonoBehaviour, ISlicedSim1000ms
{
	protected override void OnPrefabInit()
	{
		this.InitializeSlices();
		base.OnPrefabInit();
		SlicedUpdaterSim1000ms<T>.instance = this;
	}

	private void InitializeSlices()
	{
		int num = SlicedUpdaterSim1000ms<T>.NUM_200MS_BUCKETS * this.numSlicesPer200ms;
		this.m_slices = new List<SlicedUpdaterSim1000ms<T>.Slice>();
		for (int i = 0; i < num; i++)
		{
			this.m_slices.Add(new SlicedUpdaterSim1000ms<T>.Slice());
		}
		this.m_nextSliceIdx = 0;
	}

	private int GetSliceIdx(T toBeUpdated)
	{
		return toBeUpdated.GetComponent<KPrefabID>().InstanceID % this.m_slices.Count;
	}

	public void RegisterUpdate1000ms(T toBeUpdated)
	{
		SlicedUpdaterSim1000ms<T>.Slice slice = this.m_slices[this.GetSliceIdx(toBeUpdated)];
		slice.Register(toBeUpdated);
		DebugUtil.DevAssert(slice.Count < this.maxUpdatesPer200ms, string.Format("The SlicedUpdaterSim1000ms for {0} wants to update no more than {1} instances per 200ms tick, but a slice has grown more than the SlicedUpdaterSim1000ms can support.", typeof(T).Name, this.maxUpdatesPer200ms));
	}

	public void UnregisterUpdate1000ms(T toBeUpdated)
	{
		this.m_slices[this.GetSliceIdx(toBeUpdated)].Unregister(toBeUpdated);
	}

	public void Sim200ms(float dt)
	{
		foreach (SlicedUpdaterSim1000ms<T>.Slice slice in this.m_slices)
		{
			slice.IncrementDt(dt);
		}
		int num = 0;
		int i = 0;
		while (i < this.numSlicesPer200ms)
		{
			SlicedUpdaterSim1000ms<T>.Slice slice2 = this.m_slices[this.m_nextSliceIdx];
			num += slice2.Count;
			if (num > this.maxUpdatesPer200ms && i > 0)
			{
				break;
			}
			slice2.Update();
			i++;
			this.m_nextSliceIdx = (this.m_nextSliceIdx + 1) % this.m_slices.Count;
		}
	}

	private static int NUM_200MS_BUCKETS = 5;

	public static SlicedUpdaterSim1000ms<T> instance;

	[Serialize]
	public int maxUpdatesPer200ms = 300;

	[Serialize]
	public int numSlicesPer200ms = 3;

	private List<SlicedUpdaterSim1000ms<T>.Slice> m_slices;

	private int m_nextSliceIdx;

	private class Slice
	{
		public void Register(T toBeUpdated)
		{
			if (this.m_timeSinceLastUpdate == 0f)
			{
				this.m_updateList.Add(toBeUpdated);
				return;
			}
			this.m_recentlyAdded[toBeUpdated] = 0f;
		}

		public void Unregister(T toBeUpdated)
		{
			if (!this.m_updateList.Remove(toBeUpdated))
			{
				this.m_recentlyAdded.Remove(toBeUpdated);
			}
		}

		public int Count
		{
			get
			{
				return this.m_updateList.Count + this.m_recentlyAdded.Count;
			}
		}

		public List<T> GetUpdateList()
		{
			List<T> list = new List<T>();
			list.AddRange(this.m_updateList);
			list.AddRange(this.m_recentlyAdded.Keys);
			return list;
		}

		public void Update()
		{
			foreach (T t in this.m_updateList)
			{
				t.SlicedSim1000ms(this.m_timeSinceLastUpdate);
			}
			foreach (KeyValuePair<T, float> keyValuePair in this.m_recentlyAdded)
			{
				keyValuePair.Key.SlicedSim1000ms(keyValuePair.Value);
				this.m_updateList.Add(keyValuePair.Key);
			}
			this.m_recentlyAdded.Clear();
			this.m_timeSinceLastUpdate = 0f;
		}

		public void IncrementDt(float dt)
		{
			this.m_timeSinceLastUpdate += dt;
			if (this.m_recentlyAdded.Count > 0)
			{
				foreach (T t in new List<T>(this.m_recentlyAdded.Keys))
				{
					Dictionary<T, float> recentlyAdded = this.m_recentlyAdded;
					T t2 = t;
					recentlyAdded[t2] += dt;
				}
			}
		}

		private float m_timeSinceLastUpdate;

		private List<T> m_updateList = new List<T>();

		private Dictionary<T, float> m_recentlyAdded = new Dictionary<T, float>();
	}
}
