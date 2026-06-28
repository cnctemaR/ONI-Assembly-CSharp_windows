using System;
using System.Collections.Generic;

public class ScenePartitioner
{
	public ScenePartitioner(int node_size, int layer_count, int scene_width, int scene_height)
	{
		this.nodeSize = node_size;
		int num = scene_width / node_size;
		int num2 = scene_height / node_size;
		this.nodes = new ScenePartitioner.ScenePartitionerNode[layer_count, num2, num];
		for (int i = 0; i < layer_count; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < num; k++)
				{
					this.nodes[i, j, k].entries = new List<ScenePartitionerEntry>();
				}
			}
		}
	}

	public void FreeResources()
	{
		for (int i = 0; i < this.nodes.GetLength(0); i++)
		{
			for (int j = 0; j < this.nodes.GetLength(1); j++)
			{
				for (int k = 0; k < this.nodes.GetLength(2); k++)
				{
					for (int l = 0; l < this.nodes[i, j, k].entries.Count; l++)
					{
						if (this.nodes[i, j, k].entries[l] != null)
						{
							this.nodes[i, j, k].entries[l].partitioner = null;
							this.nodes[i, j, k].entries[l].obj = null;
						}
					}
					this.nodes[i, j, k].entries.Clear();
				}
			}
		}
		this.nodes = null;
	}

	public ScenePartitionerLayer CreateMask(HashedString name)
	{
		foreach (ScenePartitionerLayer scenePartitionerLayer in this.layers)
		{
			if (scenePartitionerLayer.name == name)
			{
				return scenePartitionerLayer;
			}
		}
		ScenePartitionerLayer scenePartitionerLayer2 = new ScenePartitionerLayer(name, this.layers.Count);
		this.layers.Add(scenePartitionerLayer2);
		DebugUtil.Assert(this.layers.Count <= this.nodes.GetLength(0), "Assert!");
		return scenePartitionerLayer2;
	}

	private int ClampNodeX(int x)
	{
		return Math.Min(Math.Max(x, 0), this.nodes.GetLength(2) - 1);
	}

	private int ClampNodeY(int y)
	{
		return Math.Min(Math.Max(y, 0), this.nodes.GetLength(1) - 1);
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
			Debug.LogWarning("Trying to put null go into scene partitioner", null);
			return;
		}
		Extents nodeExtents = this.GetNodeExtents(entry);
		if (nodeExtents.x + nodeExtents.width > this.nodes.GetLength(2))
		{
			Debug.LogError(string.Concat(new object[]
			{
				entry.obj.ToString(),
				" x/w ",
				nodeExtents.x,
				"/",
				nodeExtents.width,
				" < ",
				this.nodes.GetLength(2)
			}), null);
		}
		if (nodeExtents.y + nodeExtents.height > this.nodes.GetLength(1))
		{
			Debug.LogError(string.Concat(new object[]
			{
				entry.obj.ToString(),
				" y/h ",
				nodeExtents.y,
				"/",
				nodeExtents.height,
				" < ",
				this.nodes.GetLength(1)
			}), null);
		}
		int layer = entry.layer;
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				ScenePartitioner.ScenePartitionerNode scenePartitionerNode = this.nodes[layer, i, j];
				if (!scenePartitionerNode.dirty)
				{
					scenePartitionerNode.dirty = true;
					this.dirtyNodes.Add(new ScenePartitioner.DirtyNode
					{
						layer = layer,
						x = j,
						y = i
					});
					this.nodes[layer, i, j] = scenePartitionerNode;
				}
				scenePartitionerNode.entries.Add(entry);
			}
		}
	}

	private void Widthdraw(ScenePartitionerEntry entry)
	{
		Extents nodeExtents = this.GetNodeExtents(entry);
		if (nodeExtents.x + nodeExtents.width > this.nodes.GetLength(2))
		{
			Debug.LogError(string.Concat(new object[]
			{
				" x/w ",
				nodeExtents.x,
				"/",
				nodeExtents.width,
				" < ",
				this.nodes.GetLength(2)
			}), null);
		}
		if (nodeExtents.y + nodeExtents.height > this.nodes.GetLength(1))
		{
			Debug.LogError(string.Concat(new object[]
			{
				" y/h ",
				nodeExtents.y,
				"/",
				nodeExtents.height,
				" < ",
				this.nodes.GetLength(1)
			}), null);
		}
		int layer = entry.layer;
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				ScenePartitioner.ScenePartitionerNode scenePartitionerNode = this.nodes[layer, i, j];
				List<ScenePartitionerEntry> entries = scenePartitionerNode.entries;
				int count = entries.Count;
				for (int k = 0; k < count; k++)
				{
					if (entries[k] == entry)
					{
						if (!scenePartitionerNode.dirty)
						{
							scenePartitionerNode.dirty = true;
							this.dirtyNodes.Add(new ScenePartitioner.DirtyNode
							{
								layer = layer,
								x = j,
								y = i
							});
							this.nodes[layer, i, j] = scenePartitionerNode;
						}
						entries[k] = null;
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
		entry.x = x;
		entry.y = y;
		this.Insert(entry);
	}

	public void Remove(ScenePartitionerEntry entry)
	{
		Extents nodeExtents = this.GetNodeExtents(entry);
		if (nodeExtents.x + nodeExtents.width > this.nodes.GetLength(2))
		{
			Debug.LogError(string.Concat(new object[]
			{
				" x/w ",
				nodeExtents.x,
				"/",
				nodeExtents.width,
				" < ",
				this.nodes.GetLength(2)
			}), null);
		}
		if (nodeExtents.y + nodeExtents.height > this.nodes.GetLength(1))
		{
			Debug.LogError(string.Concat(new object[]
			{
				" y/h ",
				nodeExtents.y,
				"/",
				nodeExtents.height,
				" < ",
				this.nodes.GetLength(1)
			}), null);
		}
		int layer = entry.layer;
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				ScenePartitioner.ScenePartitionerNode scenePartitionerNode = this.nodes[layer, i, j];
				if (!scenePartitionerNode.dirty)
				{
					scenePartitionerNode.dirty = true;
					this.dirtyNodes.Add(new ScenePartitioner.DirtyNode
					{
						layer = layer,
						x = j,
						y = i
					});
					this.nodes[layer, i, j] = scenePartitionerNode;
				}
			}
		}
		entry.obj = null;
	}

	public void Update()
	{
		for (int i = 0; i < this.dirtyNodes.Count; i++)
		{
			ScenePartitioner.ScenePartitionerNode scenePartitionerNode = this.nodes[this.dirtyNodes[i].layer, this.dirtyNodes[i].y, this.dirtyNodes[i].x];
			scenePartitionerNode.entries.RemoveAll(ScenePartitioner.removeCallback);
			scenePartitionerNode.dirty = false;
			this.nodes[this.dirtyNodes[i].layer, this.dirtyNodes[i].y, this.dirtyNodes[i].x] = scenePartitionerNode;
		}
		this.dirtyNodes.Clear();
	}

	public void TriggerEvent(List<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		List<ScenePartitionerEntry> list = this.ReserveList();
		this.queryId++;
		for (int i = 0; i < cells.Count; i++)
		{
			int num = 0;
			int num2 = 0;
			Grid.CellToXY(cells[i], out num, out num2);
			this.GatherEntries(num, num2, 1, 1, layer, event_data, list, this.queryId);
		}
		this.RunEntries(list, event_data);
		this.ReleaseList(list);
	}

	public void TriggerEvent(HashSet<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		List<ScenePartitionerEntry> list = this.ReserveList();
		this.queryId++;
		foreach (int num in cells)
		{
			int num2 = 0;
			int num3 = 0;
			Grid.CellToXY(num, out num2, out num3);
			this.GatherEntries(num2, num3, 1, 1, layer, event_data, list, this.queryId);
		}
		this.RunEntries(list, event_data);
		this.ReleaseList(list);
	}

	public void TriggerEvent(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data)
	{
		List<ScenePartitionerEntry> list = this.ReserveList();
		this.GatherEntries(x, y, width, height, layer, event_data, list);
		this.RunEntries(list, event_data);
		this.ReleaseList(list);
	}

	private void RunEntries(List<ScenePartitionerEntry> gathered_entries, object event_data)
	{
		for (int i = 0; i < gathered_entries.Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = gathered_entries[i];
			if (scenePartitionerEntry.obj != null && scenePartitionerEntry.eventCallback != null)
			{
				scenePartitionerEntry.eventCallback(event_data);
			}
		}
	}

	public void GatherEntries(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data, List<ScenePartitionerEntry> gathered_entries)
	{
		this.GatherEntries(x, y, width, height, layer, event_data, gathered_entries, ++this.queryId);
	}

	public void GatherEntries(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data, List<ScenePartitionerEntry> gathered_entries, int query_id)
	{
		Extents nodeExtents = this.GetNodeExtents(x, y, width, height);
		int num = Math.Min(nodeExtents.y + nodeExtents.height, this.nodes.GetLength(1));
		int num2 = Math.Max(nodeExtents.y, 0);
		int num3 = Math.Max(nodeExtents.x, 0);
		int num4 = Math.Min(nodeExtents.x + nodeExtents.width, this.nodes.GetLength(2));
		int layer2 = layer.layer;
		for (int i = num2; i < num; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				List<ScenePartitionerEntry> entries = this.nodes[layer2, i, j].entries;
				int count = entries.Count;
				for (int k = 0; k < count; k++)
				{
					ScenePartitionerEntry scenePartitionerEntry = entries[k];
					if (scenePartitionerEntry != null)
					{
						if (scenePartitionerEntry.queryId != this.queryId)
						{
							if (scenePartitionerEntry.obj == null)
							{
								entries[k] = null;
							}
							else if (x + width - 1 >= scenePartitionerEntry.x && x <= scenePartitionerEntry.x + scenePartitionerEntry.width - 1 && y + height - 1 >= scenePartitionerEntry.y && y <= scenePartitionerEntry.y + scenePartitionerEntry.height - 1)
							{
								scenePartitionerEntry.queryId = this.queryId;
								gathered_entries.Add(scenePartitionerEntry);
							}
						}
					}
				}
			}
		}
	}

	public List<ScenePartitionerEntry> ReserveList()
	{
		List<ScenePartitionerEntry> list;
		if (this.freeLists.Count == 0)
		{
			list = new List<ScenePartitionerEntry>();
		}
		else
		{
			list = this.freeLists[this.freeLists.Count - 1];
			this.freeLists.RemoveAt(this.freeLists.Count - 1);
		}
		return list;
	}

	public void ReleaseList(List<ScenePartitionerEntry> list)
	{
		list.Clear();
		this.freeLists.Add(list);
	}

	private List<ScenePartitionerLayer> layers = new List<ScenePartitionerLayer>();

	private int nodeSize;

	private List<ScenePartitioner.DirtyNode> dirtyNodes = new List<ScenePartitioner.DirtyNode>();

	private ScenePartitioner.ScenePartitionerNode[,,] nodes;

	private int queryId;

	private List<List<ScenePartitionerEntry>> freeLists = new List<List<ScenePartitionerEntry>>();

	private static Predicate<ScenePartitionerEntry> removeCallback = (ScenePartitionerEntry entry) => entry == null || entry.obj == null;

	private struct ScenePartitionerNode
	{
		public List<ScenePartitionerEntry> entries;

		public bool dirty;
	}

	private struct DirtyNode
	{
		public int layer;

		public int x;

		public int y;
	}
}
