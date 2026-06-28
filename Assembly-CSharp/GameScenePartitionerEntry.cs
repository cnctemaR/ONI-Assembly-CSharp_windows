using System;

public class GameScenePartitionerEntry : ScenePartitionerEntry
{
	public GameScenePartitionerEntry(string name, object obj, int x, int y, int width, int height, int masks, ScenePartitioner partitioner, Action<object> event_callback)
		: base(name, obj, x, y, width, height, masks, partitioner, event_callback)
	{
	}

	public void UpdatePosition(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		base.UpdatePosition(vector2I.x, vector2I.y);
	}
}
