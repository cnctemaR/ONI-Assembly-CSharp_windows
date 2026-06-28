using System;

public class ScenePartitionerEntry
{
	public ScenePartitionerEntry(string name, object obj, int x, int y, int width, int height, int masks, ScenePartitioner partitioner, Action<object> event_callback)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		this.masks = masks;
		this.partitioner = partitioner;
		this.eventCallback = event_callback;
		this.obj = obj;
	}

	public void UpdatePosition(int x, int y)
	{
		this.partitioner.UpdatePosition(x, y, this);
	}

	public void Release()
	{
		this.partitioner.Remove(this);
	}

	public int x;

	public int y;

	public int width;

	public int height;

	public int masks;

	public int queryId;

	public ScenePartitioner partitioner;

	public Action<object> eventCallback;

	public object obj;
}
