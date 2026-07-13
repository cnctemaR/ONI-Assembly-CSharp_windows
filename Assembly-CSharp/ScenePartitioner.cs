using System;
using System.Collections.Generic;

public class ScenePartitioner : ISim1000ms
{
	public ScenePartitioner(int node_size, int layer_count, int scene_width, int scene_height)
	{
		this.nodeSize = node_size;
		int num = scene_width / node_size;
		int num2 = scene_height / node_size;
		this.nodes = new ScenePartitioner.ScenePartitionerNode[layer_count, num2, num];
		for (int i = 0; i < this.nodes.GetLength(0); i++)
		{
			for (int j = 0; j < this.nodes.GetLength(1); j++)
			{
				for (int k = 0; k < this.nodes.GetLength(2); k++)
				{
					this.nodes[i, j, k].entries = new HybridListHashSet<HandleVector<int>.Handle>();
				}
			}
		}
		SimAndRenderScheduler.instance.Add(this, false);
	}

	public void FreeResources()
	{
		this.nodes = null;
	}

	[Obsolete]
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
		DebugUtil.Assert(this.layers.Count <= this.nodes.GetLength(0));
		return scenePartitionerLayer2;
	}

	public ScenePartitionerLayer CreateMask(string name)
	{
		foreach (ScenePartitionerLayer scenePartitionerLayer in this.layers)
		{
			if (scenePartitionerLayer.name == name)
			{
				return scenePartitionerLayer;
			}
		}
		HashCache.Get().Add(name);
		ScenePartitionerLayer scenePartitionerLayer2 = new ScenePartitionerLayer(name, this.layers.Count);
		this.layers.Add(scenePartitionerLayer2);
		DebugUtil.Assert(this.layers.Count <= this.nodes.GetLength(0));
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

	private void Insert(HandleVector<int>.Handle handle)
	{
		ScenePartitionerEntry scenePartitionerEntry;
		if (!GameScenePartitioner.Instance.Lookup(handle, out scenePartitionerEntry))
		{
			Debug.LogWarning("Trying to put invalid handle go into scene partitioner");
			return;
		}
		if (scenePartitionerEntry.obj == null)
		{
			Debug.LogWarning("Trying to put null go into scene partitioner");
			return;
		}
		Extents nodeExtents = this.GetNodeExtents(scenePartitionerEntry);
		if (nodeExtents.x + nodeExtents.width > this.nodes.GetLength(2))
		{
			Debug.LogError(string.Concat(new string[]
			{
				scenePartitionerEntry.obj.ToString(),
				" x/w ",
				nodeExtents.x.ToString(),
				"/",
				nodeExtents.width.ToString(),
				" < ",
				this.nodes.GetLength(2).ToString()
			}));
		}
		if (nodeExtents.y + nodeExtents.height > this.nodes.GetLength(1))
		{
			Debug.LogError(string.Concat(new string[]
			{
				scenePartitionerEntry.obj.ToString(),
				" y/h ",
				nodeExtents.y.ToString(),
				"/",
				nodeExtents.height.ToString(),
				" < ",
				this.nodes.GetLength(1).ToString()
			}));
		}
		int layer = scenePartitionerEntry.layer;
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				if (!this.nodes[layer, i, j].dirty)
				{
					this.nodes[layer, i, j].dirty = true;
					this.dirtyNodes.Add(new ScenePartitioner.DirtyNode
					{
						layer = layer,
						x = j,
						y = i
					});
				}
				this.nodes[layer, i, j].entries.Add(handle);
			}
		}
	}

	private void Withdraw(int layer, Extents extents, HandleVector<int>.Handle handle)
	{
		if (extents.x + extents.width > this.nodes.GetLength(2))
		{
			Debug.LogError(string.Concat(new string[]
			{
				" x/w ",
				extents.x.ToString(),
				"/",
				extents.width.ToString(),
				" < ",
				this.nodes.GetLength(2).ToString()
			}));
		}
		if (extents.y + extents.height > this.nodes.GetLength(1))
		{
			Debug.LogError(string.Concat(new string[]
			{
				" y/h ",
				extents.y.ToString(),
				"/",
				extents.height.ToString(),
				" < ",
				this.nodes.GetLength(1).ToString()
			}));
		}
		for (int i = extents.y; i < extents.y + extents.height; i++)
		{
			for (int j = extents.x; j < extents.x + extents.width; j++)
			{
				this.nodes[layer, i, j].entries.Remove(handle);
			}
		}
	}

	public void Add(HandleVector<int>.Handle entry)
	{
		this.Insert(entry);
	}

	public void UpdatePosition(int x, int y, HandleVector<int>.Handle handle)
	{
		ScenePartitionerEntry scenePartitionerEntry;
		if (GameScenePartitioner.Instance.Lookup(handle, out scenePartitionerEntry))
		{
			this.Withdraw(scenePartitionerEntry.layer, this.GetNodeExtents(scenePartitionerEntry), handle);
			scenePartitionerEntry.x = x;
			scenePartitionerEntry.y = y;
			this.Insert(handle);
		}
	}

	public void UpdatePosition(Extents e, HandleVector<int>.Handle handle)
	{
		ScenePartitionerEntry scenePartitionerEntry;
		if (GameScenePartitioner.Instance.Lookup(handle, out scenePartitionerEntry))
		{
			this.Withdraw(scenePartitionerEntry.layer, this.GetNodeExtents(scenePartitionerEntry), handle);
			scenePartitionerEntry.x = e.x;
			scenePartitionerEntry.y = e.y;
			scenePartitionerEntry.width = e.width;
			scenePartitionerEntry.height = e.height;
			this.Insert(handle);
		}
	}

	public void Remove(HandleVector<int>.Handle handle)
	{
		ScenePartitionerEntry scenePartitionerEntry;
		if (!GameScenePartitioner.Instance.Lookup(handle, out scenePartitionerEntry))
		{
			return;
		}
		Extents nodeExtents = this.GetNodeExtents(scenePartitionerEntry);
		if (nodeExtents.x + nodeExtents.width > this.nodes.GetLength(2))
		{
			Debug.LogError(string.Concat(new string[]
			{
				" x/w ",
				nodeExtents.x.ToString(),
				"/",
				nodeExtents.width.ToString(),
				" < ",
				this.nodes.GetLength(2).ToString()
			}));
		}
		if (nodeExtents.y + nodeExtents.height > this.nodes.GetLength(1))
		{
			Debug.LogError(string.Concat(new string[]
			{
				" y/h ",
				nodeExtents.y.ToString(),
				"/",
				nodeExtents.height.ToString(),
				" < ",
				this.nodes.GetLength(1).ToString()
			}));
		}
		int layer = scenePartitionerEntry.layer;
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				if (!this.nodes[layer, i, j].dirty)
				{
					this.nodes[layer, i, j].dirty = true;
					this.dirtyNodes.Add(new ScenePartitioner.DirtyNode
					{
						layer = layer,
						x = j,
						y = i
					});
				}
			}
		}
		scenePartitionerEntry.obj = null;
	}

	public void Sim1000ms(float dt)
	{
		foreach (ScenePartitioner.DirtyNode dirtyNode in this.dirtyNodes)
		{
			HybridListHashSet<HandleVector<int>.Handle> entries = this.nodes[dirtyNode.layer, dirtyNode.y, dirtyNode.x].entries;
			for (int i = entries.Count - 1; i >= 0; i--)
			{
				ScenePartitionerEntry scenePartitionerEntry;
				if (!GameScenePartitioner.Instance.Lookup(entries[i], out scenePartitionerEntry))
				{
					entries.Remove(entries[i]);
				}
			}
			this.nodes[dirtyNode.layer, dirtyNode.y, dirtyNode.x].dirty = false;
		}
		this.dirtyNodes.Clear();
	}

	public void TriggerEvent(IEnumerable<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		this.queryId++;
		this.RunLayerGlobalEvent(cells, layer, event_data);
		foreach (int num in cells)
		{
			int num2 = 0;
			int num3 = 0;
			Grid.CellToXY(num, out num2, out num3);
			this.TriggerEventInternal(num2, num3, 1, 1, layer, event_data);
		}
	}

	public void TriggerEvent(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data)
	{
		this.queryId++;
		this.RunLayerGlobalEvent(x, y, width, height, layer, event_data);
		this.TriggerEventInternal(x, y, width, height, layer, event_data);
	}

	private void TriggerEventInternal(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data)
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
				HybridListHashSet<HandleVector<int>.Handle> entries = this.nodes[layer2, i, j].entries;
				for (int k = entries.Count - 1; k >= 0; k--)
				{
					ScenePartitionerEntry scenePartitionerEntry;
					if (GameScenePartitioner.Instance.Lookup(entries[k], out scenePartitionerEntry))
					{
						if (x + width - 1 >= scenePartitionerEntry.x && x <= scenePartitionerEntry.x + scenePartitionerEntry.width - 1 && y + height - 1 >= scenePartitionerEntry.y && y <= scenePartitionerEntry.y + scenePartitionerEntry.height - 1 && scenePartitionerEntry.queryId != this.queryId && scenePartitionerEntry.eventCallback != null && scenePartitionerEntry.obj != null)
						{
							scenePartitionerEntry.queryId = this.queryId;
							scenePartitionerEntry.eventCallback(event_data);
						}
					}
					else
					{
						entries.Remove(entries[k]);
					}
				}
			}
		}
	}

	private void RunLayerGlobalEvent(IEnumerable<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		if (layer.OnEvent != null)
		{
			foreach (int num in cells)
			{
				layer.OnEvent(num, event_data);
			}
		}
	}

	private void RunLayerGlobalEvent(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data)
	{
		if (layer.OnEvent != null)
		{
			for (int i = y; i < y + height; i++)
			{
				for (int j = x; j < x + width; j++)
				{
					int num = Grid.XYToCell(j, i);
					if (Grid.IsValidCell(num))
					{
						layer.OnEvent(num, event_data);
					}
				}
			}
		}
	}

	public void GatherEntries(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data, List<ScenePartitionerEntry> gathered_entries)
	{
		int num = this.queryId + 1;
		this.queryId = num;
		this.GatherEntries(x, y, width, height, layer, event_data, gathered_entries, num);
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
				HybridListHashSet<HandleVector<int>.Handle> entries = this.nodes[layer2, i, j].entries;
				for (int k = entries.Count - 1; k >= 0; k--)
				{
					ScenePartitionerEntry scenePartitionerEntry;
					if (GameScenePartitioner.Instance.Lookup(entries[k], out scenePartitionerEntry))
					{
						if (x + width - 1 >= scenePartitionerEntry.x && x <= scenePartitionerEntry.x + scenePartitionerEntry.width - 1 && y + height - 1 >= scenePartitionerEntry.y && y <= scenePartitionerEntry.y + scenePartitionerEntry.height - 1 && scenePartitionerEntry.queryId != this.queryId)
						{
							scenePartitionerEntry.queryId = this.queryId;
							gathered_entries.Add(scenePartitionerEntry);
						}
					}
					else
					{
						entries.Remove(entries[k]);
					}
				}
			}
		}
	}

	public void VisitEntries<ContextType>(int x, int y, int width, int height, ScenePartitionerLayer layer, Func<object, ContextType, Util.IterationInstruction> visitor, ContextType context) where ContextType : class
	{
		Extents nodeExtents = this.GetNodeExtents(x, y, width, height);
		this.queryId++;
		int num = Math.Min(nodeExtents.y + nodeExtents.height, this.nodes.GetLength(1));
		int num2 = Math.Max(nodeExtents.y, 0);
		int num3 = Math.Max(nodeExtents.x, 0);
		int num4 = Math.Min(nodeExtents.x + nodeExtents.width, this.nodes.GetLength(2));
		int layer2 = layer.layer;
		for (int i = num2; i < num; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				HybridListHashSet<HandleVector<int>.Handle> entries = this.nodes[layer2, i, j].entries;
				for (int k = entries.Count - 1; k >= 0; k--)
				{
					ScenePartitionerEntry scenePartitionerEntry;
					if (GameScenePartitioner.Instance.Lookup(entries[k], out scenePartitionerEntry))
					{
						if (x + width - 1 >= scenePartitionerEntry.x && x <= scenePartitionerEntry.x + scenePartitionerEntry.width - 1 && y + height - 1 >= scenePartitionerEntry.y && y <= scenePartitionerEntry.y + scenePartitionerEntry.height - 1 && scenePartitionerEntry.queryId != this.queryId)
						{
							scenePartitionerEntry.queryId = this.queryId;
							if (visitor(scenePartitionerEntry.obj, context) == Util.IterationInstruction.Halt)
							{
								return;
							}
						}
					}
					else
					{
						entries.Remove(entries[k]);
					}
				}
			}
		}
	}

	public void VisitEntries<ContextType>(int x, int y, int width, int height, ScenePartitionerLayer layer, GameScenePartitioner.VisitorRef<ContextType> visitor, ref ContextType context) where ContextType : struct
	{
		Extents nodeExtents = this.GetNodeExtents(x, y, width, height);
		this.queryId++;
		int num = Math.Min(nodeExtents.y + nodeExtents.height, this.nodes.GetLength(1));
		int num2 = Math.Max(nodeExtents.y, 0);
		int num3 = Math.Max(nodeExtents.x, 0);
		int num4 = Math.Min(nodeExtents.x + nodeExtents.width, this.nodes.GetLength(2));
		int layer2 = layer.layer;
		for (int i = num2; i < num; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				HybridListHashSet<HandleVector<int>.Handle> entries = this.nodes[layer2, i, j].entries;
				for (int k = entries.Count - 1; k >= 0; k--)
				{
					ScenePartitionerEntry scenePartitionerEntry;
					if (GameScenePartitioner.Instance.Lookup(entries[k], out scenePartitionerEntry))
					{
						if (x + width - 1 >= scenePartitionerEntry.x && x <= scenePartitionerEntry.x + scenePartitionerEntry.width - 1 && y + height - 1 >= scenePartitionerEntry.y && y <= scenePartitionerEntry.y + scenePartitionerEntry.height - 1 && scenePartitionerEntry.queryId != this.queryId)
						{
							scenePartitionerEntry.queryId = this.queryId;
							if (visitor(scenePartitionerEntry.obj, ref context) == Util.IterationInstruction.Halt)
							{
								return;
							}
						}
					}
					else
					{
						entries.Remove(entries[k]);
					}
				}
			}
		}
	}

	public void ReadonlyVisitEntries<ContextType>(int x, int y, int width, int height, ScenePartitionerLayer layer, Func<object, ContextType, Util.IterationInstruction> visitor, ContextType context) where ContextType : class
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
				HybridListHashSet<HandleVector<int>.Handle> entries = this.nodes[layer2, i, j].entries;
				for (int k = entries.Count - 1; k >= 0; k--)
				{
					ScenePartitionerEntry scenePartitionerEntry;
					if (GameScenePartitioner.Instance.Lookup(entries[k], out scenePartitionerEntry) && x + width - 1 >= scenePartitionerEntry.x && x <= scenePartitionerEntry.x + scenePartitionerEntry.width - 1 && y + height - 1 >= scenePartitionerEntry.y && y <= scenePartitionerEntry.y + scenePartitionerEntry.height - 1 && visitor(scenePartitionerEntry.obj, context) == Util.IterationInstruction.Halt)
					{
						return;
					}
				}
			}
		}
	}

	public void ReadonlyVisitEntries<ContextType>(int x, int y, int width, int height, ScenePartitionerLayer layer, GameScenePartitioner.VisitorRef<ContextType> visitor, ref ContextType context) where ContextType : struct
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
				HybridListHashSet<HandleVector<int>.Handle> entries = this.nodes[layer2, i, j].entries;
				for (int k = entries.Count - 1; k >= 0; k--)
				{
					ScenePartitionerEntry scenePartitionerEntry;
					if (GameScenePartitioner.Instance.Lookup(entries[k], out scenePartitionerEntry) && x + width - 1 >= scenePartitionerEntry.x && x <= scenePartitionerEntry.x + scenePartitionerEntry.width - 1 && y + height - 1 >= scenePartitionerEntry.y && y <= scenePartitionerEntry.y + scenePartitionerEntry.height - 1 && visitor(scenePartitionerEntry.obj, ref context) == Util.IterationInstruction.Halt)
					{
						return;
					}
				}
			}
		}
	}

	public void Cleanup()
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	private static Util.IterationInstruction checkForAnyObjectHelper(object obj, ref bool found)
	{
		found = true;
		return Util.IterationInstruction.Halt;
	}

	public bool DoDebugLayersContainItemsOnCell(int cell)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		List<ScenePartitionerEntry> list = new List<ScenePartitionerEntry>();
		foreach (ScenePartitionerLayer scenePartitionerLayer in this.toggledLayers)
		{
			list.Clear();
			bool flag = false;
			GameScenePartitioner.Instance.VisitEntries<bool>(num, num2, 1, 1, scenePartitionerLayer, new GameScenePartitioner.VisitorRef<bool>(ScenePartitioner.checkForAnyObjectHelper), ref flag);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	public List<ScenePartitionerLayer> layers = new List<ScenePartitionerLayer>();

	private int nodeSize;

	private List<ScenePartitioner.DirtyNode> dirtyNodes = new List<ScenePartitioner.DirtyNode>();

	private ScenePartitioner.ScenePartitionerNode[,,] nodes;

	private int queryId;

	public HashSet<ScenePartitionerLayer> toggledLayers = new HashSet<ScenePartitionerLayer>();

	private struct ScenePartitionerNode
	{
		public HybridListHashSet<HandleVector<int>.Handle> entries;

		public bool dirty;
	}

	private struct DirtyNode
	{
		public int layer;

		public int x;

		public int y;
	}
}
