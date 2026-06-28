using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenePartitioner
{
	public ScenePartitioner(int node_size, int scene_width, int scene_height)
	{
		this.nodeSize = node_size;
		int num = scene_width / node_size;
		int num2 = scene_height / node_size;
		this.nodes = new ScenePartitionerNode[num2, num];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				this.nodes[i, j].entries = new List<ScenePartitionerEntry>();
			}
		}
	}

	public ScenePartitionerMask CreateMask(HashedString name)
	{
		foreach (ScenePartitionerMask scenePartitionerMask in this.masks)
		{
			if (scenePartitionerMask.name == name)
			{
				return scenePartitionerMask;
			}
		}
		int num = 1 << this.masks.Count;
		ScenePartitionerMask scenePartitionerMask2 = new ScenePartitionerMask(name, num);
		this.masks.Add(scenePartitionerMask2);
		return scenePartitionerMask2;
	}

	private int ClampNodeX(int x)
	{
		return Math.Min(Math.Max(x, 0), this.nodes.GetLength(1) - 1);
	}

	private int ClampNodeY(int y)
	{
		return Math.Min(Math.Max(y, 0), this.nodes.GetLength(0) - 1);
	}

	private Extents GetNodeExtents(int x, int y, int width, int height)
	{
		Extents extents = default(Extents);
		extents.x = this.ClampNodeX(x / this.nodeSize);
		extents.y = this.ClampNodeY(y / this.nodeSize);
		extents.width = 1 + this.ClampNodeX((x + width) / this.nodeSize) - extents.x;
		extents.height = 1 + this.ClampNodeY((y + height) / this.nodeSize) - extents.y;
		return extents;
	}

	private Extents GetNodeExtents(ScenePartitionerEntry entry)
	{
		return this.GetNodeExtents(entry.x, entry.y, entry.width, entry.height);
	}

	private void Insert(ScenePartitionerEntry entry)
	{
		if (entry.obj == null)
		{
			Debug.LogWarning("Trying to put null go into scene partitioner");
			return;
		}
		Extents nodeExtents = this.GetNodeExtents(entry);
		if (nodeExtents.x + nodeExtents.width > this.nodes.GetLength(1))
		{
			Debug.LogError(string.Concat(new object[]
			{
				entry.obj.ToString(),
				" x/w ",
				nodeExtents.x,
				"/",
				nodeExtents.width,
				" < ",
				this.nodes.GetLength(1)
			}));
		}
		if (nodeExtents.y + nodeExtents.height > this.nodes.GetLength(0))
		{
			Debug.LogError(string.Concat(new object[]
			{
				entry.obj.ToString(),
				" y/h ",
				nodeExtents.y,
				"/",
				nodeExtents.height,
				" < ",
				this.nodes.GetLength(0)
			}));
		}
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				this.nodes[i, j].entries.Add(entry);
			}
		}
	}

	private void Widthdraw(ScenePartitionerEntry entry)
	{
		Extents nodeExtents = this.GetNodeExtents(entry);
		if (nodeExtents.x + nodeExtents.width > this.nodes.GetLength(1))
		{
			Debug.LogError(string.Concat(new object[]
			{
				" x/w ",
				nodeExtents.x,
				"/",
				nodeExtents.width,
				" < ",
				this.nodes.GetLength(1)
			}));
		}
		if (nodeExtents.y + nodeExtents.height > this.nodes.GetLength(0))
		{
			Debug.LogError(string.Concat(new object[]
			{
				" y/h ",
				nodeExtents.y,
				"/",
				nodeExtents.height,
				" < ",
				this.nodes.GetLength(0)
			}));
		}
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				int count = this.nodes[i, j].entries.Count;
				for (int k = 0; k < count; k++)
				{
					if (this.nodes[i, j].entries[k] == entry)
					{
						this.nodes[i, j].entries[k] = null;
						break;
					}
				}
			}
		}
	}

	public ScenePartitionerEntry Add(ScenePartitionerEntry entry)
	{
		this.Insert(entry);
		return entry;
	}

	public void UpdatePosition(int x, int y, ScenePartitionerEntry entry)
	{
		this.Widthdraw(entry);
		entry.x = x - entry.width / 2;
		entry.y = y - entry.height / 2;
		this.Insert(entry);
	}

	public void Remove(ScenePartitionerEntry entry)
	{
		this.Widthdraw(entry);
	}

	public void TriggerEvent(List<int> cells, int masks, object event_data)
	{
		HashSet<ScenePartitionerEntry> hashSet = null;
		for (int i = 0; i < cells.Count; i++)
		{
			int num = 0;
			int num2 = 0;
			Grid.CellToXY(cells[i], out num, out num2);
			this.GatherEntries(num, num2, 1, 1, masks, event_data, ref hashSet);
		}
		this.RunEntries(hashSet, event_data);
	}

	public void TriggerEvent(int x, int y, int width, int height, int masks, object event_data)
	{
		HashSet<ScenePartitionerEntry> hashSet = null;
		this.GatherEntries(x, y, width, height, masks, event_data, ref hashSet);
		this.RunEntries(hashSet, event_data);
	}

	private void RunEntries(HashSet<ScenePartitionerEntry> entries, object event_data)
	{
		if (entries == null)
		{
			return;
		}
		foreach (ScenePartitionerEntry scenePartitionerEntry in entries)
		{
			if (scenePartitionerEntry.eventCallback != null)
			{
				scenePartitionerEntry.eventCallback(event_data);
			}
		}
	}

	public void GatherEntries(int x, int y, int width, int height, int masks, object event_data, ref HashSet<ScenePartitionerEntry> duplicate_check)
	{
		Extents nodeExtents = this.GetNodeExtents(x, y, width, height);
		int num = Math.Min(nodeExtents.y + nodeExtents.height, this.nodes.GetLength(0));
		int num2 = Math.Max(nodeExtents.y, 0);
		int num3 = Math.Max(nodeExtents.x, 0);
		int num4 = Math.Min(nodeExtents.x + nodeExtents.width, this.nodes.GetLength(1));
		for (int i = num2; i < num; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				List<ScenePartitionerEntry> entries = this.nodes[i, j].entries;
				entries.RemoveAll(ScenePartitioner.removeCallback);
				int count = entries.Count;
				for (int k = 0; k < count; k++)
				{
					ScenePartitionerEntry scenePartitionerEntry = entries[k];
					if (scenePartitionerEntry != null)
					{
						if (scenePartitionerEntry.obj == null)
						{
							entries[k] = null;
						}
						else if ((scenePartitionerEntry.masks & masks) != 0)
						{
							if (scenePartitionerEntry.x < x + width && scenePartitionerEntry.x + scenePartitionerEntry.width >= x && scenePartitionerEntry.y < y + height && scenePartitionerEntry.y + scenePartitionerEntry.height >= y)
							{
								if (duplicate_check == null)
								{
									duplicate_check = new HashSet<ScenePartitionerEntry>();
								}
								duplicate_check.Add(scenePartitionerEntry);
							}
						}
					}
				}
			}
		}
	}

	private List<ScenePartitionerMask> masks = new List<ScenePartitionerMask>();

	private int nodeSize;

	private ScenePartitionerNode[,] nodes;

	private static Predicate<ScenePartitionerEntry> removeCallback = (ScenePartitionerEntry entry) => entry == null;
}
