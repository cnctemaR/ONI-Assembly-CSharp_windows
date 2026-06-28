using System;

public class BucketUpdater<DataType> : UpdateBucketWithUpdater<DataType>.IUpdater
{
	public BucketUpdater(Action<DataType, float> callback)
	{
		this.callback = callback;
	}

	public void Update(DataType data, float dt)
	{
		this.callback(data, dt);
	}

	private Action<DataType, float> callback;
}
