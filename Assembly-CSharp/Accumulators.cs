using System;
using System.Collections.Generic;

public class Accumulators
{
	public Accumulators()
	{
		this.elapsedTime = 0f;
		this.accumulated = new KCompactedVector<float>(0);
		this.average = new KCompactedVector<float>(0);
	}

	public HandleVector<int>.Handle Add(string name, KMonoBehaviour owner)
	{
		HandleVector<int>.Handle handle = this.accumulated.Allocate(0f);
		this.average.Allocate(0f);
		return handle;
	}

	public HandleVector<int>.Handle Remove(HandleVector<int>.Handle handle)
	{
		if (!handle.IsValid())
		{
			return HandleVector<int>.InvalidHandle;
		}
		this.accumulated.Free(handle);
		this.average.Free(handle);
		return HandleVector<int>.InvalidHandle;
	}

	public void Sim200ms(float dt)
	{
		this.elapsedTime += dt;
		if (this.elapsedTime < 3f)
		{
			return;
		}
		this.elapsedTime -= 3f;
		List<float> dataList = this.accumulated.GetDataList();
		List<float> dataList2 = this.average.GetDataList();
		int count = dataList.Count;
		float num = 0.33333334f;
		for (int i = 0; i < count; i++)
		{
			dataList2[i] = dataList[i] * num;
			dataList[i] = 0f;
		}
	}

	public float GetAverageRate(HandleVector<int>.Handle handle)
	{
		return (!handle.IsValid()) ? 0f : this.average.GetData(handle);
	}

	public void Accumulate(HandleVector<int>.Handle handle, float amount)
	{
		float data = this.accumulated.GetData(handle);
		this.accumulated.SetData(handle, data + amount);
	}

	private const float TIME_WINDOW = 3f;

	private float elapsedTime;

	private KCompactedVector<float> accumulated;

	private KCompactedVector<float> average;
}
