using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;
using UnityEngine.Assertions;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitFlow
{
	public ConduitFlow(ConduitType conduit_type, int num_cells, IUtilityNetworkMgr network_mgr, float max_conduit_mass)
	{
		this.conduitType = conduit_type;
		this.networkMgr = network_mgr;
		this.MaxMass = max_conduit_mass;
		float num = 1f;
		this.temperatureManager = new ConduitTemperatureManager(num);
		network_mgr.ConduitFlowManager = this;
		this.Initialize(num_cells);
	}

	public event global::System.Action onConduitsRebuilt;

	public void AddConduitUpdater(Action<float> callback, int priority = 0)
	{
		this.conduitUpdaters.Add(new ConduitFlow.ConduitUpdater
		{
			priority = priority,
			callback = callback
		});
		this.dirtyConduitUpdaters = true;
	}

	public void RemoveConduitUpdater(Action<float> callback)
	{
		for (int i = 0; i < this.conduitUpdaters.Count; i++)
		{
			if (this.conduitUpdaters[i].callback == callback)
			{
				this.conduitUpdaters.RemoveAt(i);
				this.dirtyConduitUpdaters = true;
				break;
			}
		}
	}

	public static int FlowBit(ConduitFlow.FlowDirection direction)
	{
		return 1 << direction - ConduitFlow.FlowDirection.Left;
	}

	public void Initialize(int num_cells)
	{
		this.grid = new ConduitFlow.GridNode[num_cells];
		for (int i = 0; i < num_cells; i++)
		{
			this.grid[i].frontIdx = -1;
			this.grid[i].frontContents.element = SimHashes.Vacuum;
		}
	}

	public void RebuildConnections(IEnumerable<int> root_nodes)
	{
		for (int i = 0; i < this.conduits.Count; i++)
		{
			ConduitFlow.Conduit conduit = this.conduits[i];
			HandleVector<int>.Handle temperatureHandle = conduit.temperatureHandle;
			if (temperatureHandle.IsValid())
			{
				ConduitTemperatureManager.Data data = this.temperatureManager.GetData(temperatureHandle);
				ConduitFlow.ConduitContents contents = conduit.GetContents();
				contents.temperature = data.temperature;
				conduit.SetContents(contents);
				conduit.temperatureHandle.Clear();
				this.temperatureManager.Free(temperatureHandle);
			}
			this.grid[conduit.cell].frontIdx = -1;
		}
		this.conduits.Clear();
		this.pathList.Clear();
		this.temperatureManager.Clear();
		ObjectLayer objectLayer = ((this.conduitType != ConduitType.Gas) ? ObjectLayer.LiquidConduit : ObjectLayer.GasConduit);
		foreach (int num in root_nodes)
		{
			if (this.replacements.Contains(num))
			{
				this.replacements.Remove(num);
			}
			GameObject gameObject = Grid.Objects[num, (int)objectLayer];
			if (!(gameObject == null))
			{
				ConduitFlow.Conduit conduit2 = new ConduitFlow.Conduit(this, gameObject, num, this.conduits.Count);
				this.conduits.Add(conduit2);
				this.grid[num].frontIdx = conduit2.idx;
			}
		}
		foreach (int num2 in root_nodes)
		{
			UtilityConnections connections = this.networkMgr.GetConnections(num2, true);
			if (connections != (UtilityConnections)0)
			{
				if (this.grid[num2].frontIdx != -1)
				{
					ConduitFlow.Conduit conduit3 = this.conduits[this.grid[num2].frontIdx];
					int num3 = num2 - 1;
					if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Left) != (UtilityConnections)0)
					{
						conduit3.left = this.grid[num3].frontIdx;
					}
					num3 = num2 + 1;
					if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Right) != (UtilityConnections)0)
					{
						conduit3.right = this.grid[num3].frontIdx;
					}
					num3 = num2 - Grid.WidthInCells;
					if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Down) != (UtilityConnections)0)
					{
						conduit3.down = this.grid[num3].frontIdx;
					}
					num3 = num2 + Grid.WidthInCells;
					if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Up) != (UtilityConnections)0)
					{
						conduit3.up = this.grid[num3].frontIdx;
					}
				}
			}
		}
		if (this.onConduitsRebuilt != null)
		{
			this.onConduitsRebuilt();
		}
	}

	public void ScanNetworkSources(FlowUtilityNetwork network)
	{
		if (network == null)
		{
			return;
		}
		network.sources.Sort((FlowUtilityNetwork.IItem x, FlowUtilityNetwork.IItem y) => x.SortKey.CompareTo(y.SortKey));
		for (int i = 0; i < network.sources.Count; i++)
		{
			FlowUtilityNetwork.IItem item = network.sources[i];
			this.path.Clear();
			this.visited.Clear();
			this.FindSinks(i, item.SortKey, item.Cell);
		}
	}

	public void RefreshPaths()
	{
		this.pathList.Sort((ConduitFlow.PathInfo x, ConduitFlow.PathInfo y) => x.sortKey.CompareTo(y.sortKey));
		foreach (ConduitFlow.PathInfo pathInfo in this.pathList)
		{
			List<ConduitFlow.Conduit> list = pathInfo.path;
			for (int i = 0; i < list.Count - 1; i++)
			{
				ConduitFlow.Conduit conduit = list[i];
				ConduitFlow.Conduit conduit2 = list[i + 1];
				if (conduit.targetDirection == ConduitFlow.FlowDirection.None)
				{
					conduit.targetDirection = this.GetDirection(conduit, conduit2);
				}
			}
		}
	}

	private ConduitFlow.FlowDirection GetDirection(ConduitFlow.Conduit conduit, ConduitFlow.Conduit target_conduit)
	{
		if (conduit.up == target_conduit.idx)
		{
			return ConduitFlow.FlowDirection.Up;
		}
		if (conduit.down == target_conduit.idx)
		{
			return ConduitFlow.FlowDirection.Down;
		}
		if (conduit.left == target_conduit.idx)
		{
			return ConduitFlow.FlowDirection.Left;
		}
		if (conduit.right == target_conduit.idx)
		{
			return ConduitFlow.FlowDirection.Right;
		}
		return ConduitFlow.FlowDirection.None;
	}

	private void FindSinks(int source_idx, int sort_key, int cell)
	{
		ConduitFlow.GridNode gridNode = this.grid[cell];
		if (gridNode.frontIdx != -1)
		{
			this.FindSinks(source_idx, sort_key, this.conduits[gridNode.frontIdx]);
		}
	}

	private void FindSinks(int source_idx, int sort_key, ConduitFlow.Conduit conduit)
	{
		if (conduit == null || this.visited.Contains(conduit))
		{
			return;
		}
		this.visited.Add(conduit);
		if (conduit.permittedFlowDirections == -1)
		{
			return;
		}
		this.path.Add(conduit);
		FlowUtilityNetwork.IItem item = (FlowUtilityNetwork.IItem)this.networkMgr.GetEndpoint(conduit.cell);
		if (item != null && item.EndpointType == Endpoint.Sink)
		{
			this.FoundSink(source_idx, sort_key);
		}
		if (conduit.down != -1)
		{
			this.FindSinks(source_idx, sort_key, this.conduits[conduit.down]);
		}
		if (conduit.left != -1)
		{
			this.FindSinks(source_idx, sort_key, this.conduits[conduit.left]);
		}
		if (conduit.right != -1)
		{
			this.FindSinks(source_idx, sort_key, this.conduits[conduit.right]);
		}
		if (conduit.up != -1)
		{
			this.FindSinks(source_idx, sort_key, this.conduits[conduit.up]);
		}
		if (this.path.Count > 0)
		{
			this.path.RemoveAt(this.path.Count - 1);
		}
	}

	private void FoundSink(int source_idx, int sort_key)
	{
		for (int i = 0; i < this.path.Count - 1; i++)
		{
			ConduitFlow.FlowDirection direction = this.GetDirection(this.path[i], this.path[i + 1]);
			ConduitFlow.FlowDirection flowDirection = ConduitFlow.InverseFlow(direction);
			int cellFromDirection = ConduitFlow.GetCellFromDirection(this.path[i].cell, flowDirection);
			ConduitFlow.IConduit conduitFromDirection = this.path[i].GetConduitFromDirection(flowDirection);
			if (i == 0 || (this.path[i].permittedFlowDirections & ConduitFlow.FlowBit(flowDirection)) == 0 || (cellFromDirection != this.path[i - 1].cell && (this.path[i].sourceIdx == source_idx || (conduitFromDirection.PermittedFlowDirections & ConduitFlow.FlowBit(flowDirection)) == 0)))
			{
				this.path[i].sourceIdx = source_idx;
				this.path[i].permittedFlowDirections |= ConduitFlow.FlowBit(direction);
				this.path[i].targetDirection = direction;
			}
		}
		for (int j = 1; j < this.path.Count; j++)
		{
			ConduitFlow.FlowDirection direction2 = this.GetDirection(this.path[j], this.path[j - 1]);
			this.path[j].srcDirection = direction2;
		}
		ConduitFlow.PathInfo pathInfo = new ConduitFlow.PathInfo();
		pathInfo.sortKey = sort_key;
		pathInfo.path.AddRange(this.path);
		pathInfo.path.Reverse();
		this.pathList.Add(pathInfo);
	}

	public ConduitFlow.ConduitContents GetContents(int cell)
	{
		ConduitFlow.ConduitContents frontContents = this.grid[cell].frontContents;
		ConduitFlow.GridNode gridNode = this.grid[cell];
		if (gridNode.frontIdx != -1)
		{
			ConduitFlow.Conduit conduit = this.conduits[gridNode.frontIdx];
			frontContents.temperature = this.temperatureManager.GetData(conduit.temperatureHandle).temperature;
		}
		if (frontContents.mass > 0f && frontContents.temperature <= 0f)
		{
			Output.LogError(new object[] { "unexpected temperature" });
		}
		return frontContents;
	}

	public void SetContents(int cell, ConduitFlow.ConduitContents contents)
	{
		ConduitFlow.GridNode gridNode = this.grid[cell];
		if (gridNode.frontIdx != -1)
		{
			ConduitFlow.Conduit conduit = this.conduits[gridNode.frontIdx];
			conduit.SetContents(contents);
		}
		else
		{
			this.grid[cell].frontContents = contents;
		}
	}

	public static int GetCellFromDirection(int cell, ConduitFlow.FlowDirection direction)
	{
		switch (direction)
		{
		case ConduitFlow.FlowDirection.Left:
			return Grid.CellLeft(cell);
		case ConduitFlow.FlowDirection.Right:
			return Grid.CellRight(cell);
		case ConduitFlow.FlowDirection.Up:
			return Grid.CellAbove(cell);
		case ConduitFlow.FlowDirection.Down:
			return Grid.CellBelow(cell);
		default:
			return -1;
		}
	}

	private static ConduitFlow.FlowDirection InverseFlow(ConduitFlow.FlowDirection direction)
	{
		switch (direction)
		{
		case ConduitFlow.FlowDirection.Left:
			return ConduitFlow.FlowDirection.Right;
		case ConduitFlow.FlowDirection.Right:
			return ConduitFlow.FlowDirection.Left;
		case ConduitFlow.FlowDirection.Up:
			return ConduitFlow.FlowDirection.Down;
		case ConduitFlow.FlowDirection.Down:
			return ConduitFlow.FlowDirection.Up;
		default:
			return ConduitFlow.FlowDirection.None;
		}
	}

	public void Update(float dt)
	{
		this.elapsedTime += dt;
		this.temperatureManager.SimUpdate(dt);
		if (this.elapsedTime < 1f)
		{
			return;
		}
		float num = 1f;
		this.elapsedTime -= 1f;
		this.lastUpdateTime = Time.time;
		foreach (ConduitFlow.Conduit conduit in this.conduits)
		{
			conduit.updated = false;
			conduit.lastFlowElement = SimHashes.Vacuum;
			conduit.lastFlowDirection = ConduitFlow.FlowDirection.None;
			conduit.initialElement = conduit.GetContents().element;
		}
		foreach (ConduitFlow.PathInfo pathInfo in this.pathList)
		{
			List<ConduitFlow.Conduit> list = pathInfo.path;
			foreach (ConduitFlow.Conduit conduit2 in list)
			{
				this.UpdateConduit(conduit2);
			}
		}
		foreach (ConduitFlow.Conduit conduit3 in this.conduits)
		{
			if (!conduit3.updated)
			{
				if (conduit3.GetContents().element == SimHashes.Vacuum)
				{
					conduit3.srcDirection = conduit3.GetNextFlowSource();
				}
			}
		}
		if (this.dirtyConduitUpdaters)
		{
			this.conduitUpdaters.Sort((ConduitFlow.ConduitUpdater a, ConduitFlow.ConduitUpdater b) => b.priority.CompareTo(a.priority));
		}
		for (int i = 0; i < this.conduitUpdaters.Count; i++)
		{
			this.conduitUpdaters[i].callback(num);
		}
	}

	private void UpdateConduit(ConduitFlow.IConduit conduit)
	{
		if (conduit.Updated)
		{
			return;
		}
		if (conduit.SrcDirection == ConduitFlow.FlowDirection.None)
		{
			conduit.SrcDirection = conduit.GetNextFlowSource();
		}
		ConduitFlow.ConduitContents contents = conduit.GetContents();
		if (contents.element == SimHashes.Vacuum)
		{
			return;
		}
		if (contents.mass <= 0f)
		{
			contents.element = SimHashes.Vacuum;
			contents.mass = 0f;
			contents.temperature = 0f;
			conduit.SetContents(contents);
			conduit.LastFlowElement = SimHashes.Vacuum;
			conduit.TargetDirection = conduit.GetNextFlowTarget();
			return;
		}
		ConduitFlow.IConduit conduitFromDirection = conduit.GetConduitFromDirection(conduit.TargetDirection);
		if (conduitFromDirection == null)
		{
			conduit.TargetDirection = conduit.GetNextFlowTarget();
			return;
		}
		ConduitFlow.ConduitContents contents2 = conduitFromDirection.GetContents();
		if (contents2.element != SimHashes.Vacuum && contents2.element != contents.element)
		{
			conduit.LastFlowElement = SimHashes.Vacuum;
			conduit.TargetDirection = conduit.GetNextFlowTarget();
			return;
		}
		if ((conduit.PermittedFlowDirections & ConduitFlow.FlowBit(conduit.TargetDirection)) != 0 && conduitFromDirection.GetConduitFromDirection(conduitFromDirection.SrcDirection) == conduit)
		{
			float num = Mathf.Max(0f, this.MaxMass - contents2.mass);
			float num2 = Mathf.Min(contents.mass, num);
			if (num2 > 0f)
			{
				num2 = this.AddElement(conduitFromDirection.Cell, contents.element, num2, contents.temperature);
				float num3;
				this.RemoveElement(conduit, num2, out num3);
				conduitFromDirection.LastFlowDirection = ConduitFlow.InverseFlow(conduit.TargetDirection);
				conduitFromDirection.LastFlowElement = contents.element;
				conduitFromDirection.Updated = true;
				conduitFromDirection.SrcDirection = conduitFromDirection.GetNextFlowSource();
				this.TriggerListeners(conduitFromDirection);
				this.TriggerListeners(conduit);
			}
		}
		conduit.TargetDirection = conduit.GetNextFlowTarget();
	}

	private void TriggerListeners(ConduitFlow.IConduit conduit)
	{
		List<GameObject> list = null;
		if (this.conduitContentListeners.TryGetValue(conduit.Cell, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = list[i];
				EventSystem.Trigger(gameObject, 853768572, null);
			}
		}
	}

	public float ContinuousLerpPercent
	{
		get
		{
			return Mathf.Clamp01((Time.time - this.lastUpdateTime) / 1f);
		}
	}

	public float DiscreteLerpPercent
	{
		get
		{
			return Mathf.Clamp01(this.elapsedTime / 1f);
		}
	}

	public float AddElement(int cell_idx, SimHashes element, float mass, float temperature)
	{
		if (this.grid[cell_idx].frontIdx == -1)
		{
			return 0f;
		}
		ConduitFlow.ConduitContents frontContents = this.grid[cell_idx].frontContents;
		if (frontContents.element != element && frontContents.element != SimHashes.Vacuum && mass > 0f)
		{
			return 0f;
		}
		float num = Mathf.Min(mass, this.MaxMass - frontContents.mass);
		frontContents.temperature = GameUtil.GetFinalTemperature(temperature, mass, frontContents.temperature, frontContents.mass);
		frontContents.mass += num;
		frontContents.element = element;
		this.SetContents(cell_idx, frontContents);
		return num;
	}

	public float RemoveElement(int cell_idx, float delta)
	{
		if (this.grid[cell_idx].frontIdx == -1)
		{
			return 0f;
		}
		ConduitFlow.Conduit conduit = this.GetConduit(cell_idx);
		float num;
		this.RemoveElement(conduit, delta, out num);
		return num;
	}

	private ConduitFlow.ConduitContents RemoveElement(ConduitFlow.IConduit conduit, float delta, out float amount_removed)
	{
		ConduitFlow.ConduitContents contents = conduit.GetContents();
		amount_removed = Mathf.Min(contents.mass, delta);
		float num = contents.mass - amount_removed;
		if (num <= 0f)
		{
			contents.mass = 0f;
			contents.temperature = 0f;
			contents.element = SimHashes.Vacuum;
		}
		else
		{
			contents.mass = num;
		}
		conduit.SetContents(contents);
		return contents;
	}

	public int GetPermittedFlow(int cell)
	{
		ConduitFlow.Conduit conduit = this.GetConduit(cell);
		if (conduit == null)
		{
			return 0;
		}
		return conduit.permittedFlowDirections;
	}

	public ConduitFlow.Conduit GetConduit(int cell)
	{
		int frontIdx = this.grid[cell].frontIdx;
		return (frontIdx == -1) ? null : this.conduits[frontIdx];
	}

	private void DumpPipeContents(int cell, ConduitFlow.ConduitContents contents)
	{
		if (contents.element != SimHashes.Vacuum && contents.mass > 0f)
		{
			SimMessages.AddRemoveSubstance(cell, contents.element, CellEventLogger.Instance.ConduitFlowEmptyConduit, contents.mass, contents.temperature, -1);
			contents.mass = 0f;
			contents.temperature = 0f;
			contents.element = SimHashes.Vacuum;
			this.SetContents(cell, contents);
		}
	}

	public void EmptyConduit(int cell)
	{
		if (this.replacements.Contains(cell))
		{
			return;
		}
		this.DumpPipeContents(cell, this.grid[cell].frontContents);
	}

	public void MarkForReplacement(int cell)
	{
		this.replacements.Add(cell);
	}

	public void DeactivateCell(int cell)
	{
		this.grid[cell].frontIdx = -1;
		ConduitFlow.ConduitContents conduitContents = new ConduitFlow.ConduitContents(SimHashes.Vacuum, 0f, 0f);
		this.SetContents(cell, conduitContents);
	}

	[Conditional("CHECK_NAN")]
	private void Validate(ConduitFlow.ConduitContents contents)
	{
		Assert.IsTrue(!float.IsNaN(contents.temperature));
		Assert.IsTrue(!float.IsPositiveInfinity(contents.temperature));
		Assert.IsTrue(!float.IsNegativeInfinity(contents.temperature));
		Assert.IsTrue(contents.mass == 0f || contents.temperature > 0f);
		if (contents.mass > 0f && contents.temperature <= 0f)
		{
			Output.LogError(new object[] { "zero degree pipe contents" });
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		int count = this.conduits.Count;
		if (count > 0)
		{
			this.serializedContents = new ConduitFlow.ConduitContents[count];
			this.serializedIdx = new int[count];
			for (int i = 0; i < count; i++)
			{
				ConduitFlow.Conduit conduit = this.conduits[i];
				ConduitFlow.ConduitContents contents = conduit.GetContents();
				this.Validate(contents);
				this.serializedIdx[i] = conduit.cell;
				this.serializedContents[i] = contents;
			}
		}
		else
		{
			this.serializedContents = null;
			this.serializedIdx = null;
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		this.serializedContents = null;
		this.serializedIdx = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.serializedContents == null)
		{
			return;
		}
		for (int i = 0; i < this.serializedContents.Length; i++)
		{
			int num = this.serializedIdx[i];
			ConduitFlow.ConduitContents conduitContents = this.serializedContents[i];
			if (conduitContents.mass <= 0f)
			{
				conduitContents.mass = 0f;
				conduitContents.temperature = 0f;
				conduitContents.element = SimHashes.Vacuum;
			}
			if (float.IsNaN(conduitContents.temperature) || (conduitContents.temperature <= 0f && conduitContents.element != SimHashes.Vacuum))
			{
				Vector2I vector2I = Grid.CellToXY(num);
				DeserializeWarnings.Instance.PipeContentsTemperatureIsNan.Warn(string.Format("NaN pipe content temperature detected. Resetting temperature. (x={0}, y={1}, cell={2})", vector2I.x, vector2I.y, num), null);
				conduitContents.temperature = ElementLoader.FindElementByHash(conduitContents.element).defaultValues.temperature;
			}
			conduitContents.mass = Math.Min(this.MaxMass, conduitContents.mass);
			this.SetContents(num, conduitContents);
		}
		this.serializedContents = null;
		this.serializedIdx = null;
	}

	public UtilityNetwork GetNetwork(ConduitFlow.Conduit conduit)
	{
		return this.networkMgr.GetNetworkForCell(conduit.cell);
	}

	public IEnumerator<ConduitFlow.Conduit> VisibleConduitsEnumerator(Vector2I min, Vector2I max)
	{
		return new ConduitFlow.VisibleConduitIterator(min, max, this.conduits);
	}

	public void ForceRebuildNetworks()
	{
		this.networkMgr.ForceRebuildNetworks();
	}

	public void RegisterContentListener(int cell, GameObject listener)
	{
		List<GameObject> list = null;
		if (!this.conduitContentListeners.TryGetValue(cell, out list))
		{
			list = new List<GameObject>();
			this.conduitContentListeners[cell] = list;
		}
		list.Add(listener);
	}

	public void UnregisterContentListener(int cell, GameObject listener)
	{
		List<GameObject> list = null;
		if (this.conduitContentListeners.TryGetValue(cell, out list))
		{
			list.Remove(listener);
		}
	}

	public bool IsConduitFull(int cell_idx)
	{
		ConduitFlow.ConduitContents frontContents = this.grid[cell_idx].frontContents;
		return this.MaxMass - frontContents.mass <= 0f;
	}

	public const float TickRate = 1f;

	public const float WaitTime = 1f;

	private ConduitType conduitType;

	private float MaxMass = 10f;

	private float elapsedTime;

	private float lastUpdateTime = float.NegativeInfinity;

	private List<ConduitFlow.Conduit> conduits = new List<ConduitFlow.Conduit>();

	private bool dirtyConduitUpdaters;

	private List<ConduitFlow.ConduitUpdater> conduitUpdaters = new List<ConduitFlow.ConduitUpdater>();

	private ConduitTemperatureManager temperatureManager;

	private ConduitFlow.GridNode[] grid;

	[Serialize]
	public int[] serializedIdx;

	[Serialize]
	public ConduitFlow.ConduitContents[] serializedContents;

	private IUtilityNetworkMgr networkMgr;

	private HashSet<ConduitFlow.Conduit> visited = new HashSet<ConduitFlow.Conduit>();

	private HashSet<int> replacements = new HashSet<int>();

	private List<ConduitFlow.Conduit> path = new List<ConduitFlow.Conduit>();

	private List<ConduitFlow.PathInfo> pathList = new List<ConduitFlow.PathInfo>();

	public static readonly ConduitFlow.ConduitContents emptyContents = new ConduitFlow.ConduitContents
	{
		element = SimHashes.Vacuum,
		mass = 0f,
		temperature = 0f
	};

	private Dictionary<int, List<GameObject>> conduitContentListeners = new Dictionary<int, List<GameObject>>();

	public struct ConduitUpdater
	{
		public int priority;

		public Action<float> callback;
	}

	public struct GridNode
	{
		public int frontIdx;

		public ConduitFlow.ConduitContents frontContents;
	}

	private class PathInfo
	{
		public int sortKey;

		public List<ConduitFlow.Conduit> path = new List<ConduitFlow.Conduit>();
	}

	public enum FlowDirection
	{
		Blocked = -1,
		None,
		Left,
		Right,
		Up,
		Down,
		Num
	}

	public interface IConduit
	{
		ConduitFlow.ConduitContents GetContents();

		void SetContents(ConduitFlow.ConduitContents contents);

		ConduitFlow.FlowDirection GetNextFlowTarget();

		ConduitFlow.FlowDirection GetNextFlowSource();

		ConduitFlow.IConduit GetConduitFromDirection(ConduitFlow.FlowDirection direction);

		int Cell { get; }

		int PermittedFlowDirections { get; set; }

		bool Updated { get; set; }

		ConduitFlow.FlowDirection SrcDirection { get; set; }

		ConduitFlow.FlowDirection TargetDirection { get; set; }

		ConduitFlow.FlowDirection LastFlowDirection { get; set; }

		SimHashes LastFlowElement { get; set; }
	}

	[DebuggerDisplay("{cell}")]
	[Serializable]
	public class Conduit : ConduitFlow.IConduit
	{
		public Conduit(ConduitFlow manager, GameObject conduit_go, int cell, int idx)
		{
			this.manager = manager;
			this.cell = cell;
			this.idx = idx;
			this.left = (this.right = (this.up = (this.down = -1)));
			this.updated = false;
			this.permittedFlowDirections = 0;
			this.sourceIdx = -1;
			ConduitFlow.ConduitContents contents = this.GetContents();
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(conduit_go);
			this.temperatureHandle = manager.temperatureManager.Allocate(handle, ref contents);
		}

		private HandleVector<int>.Handle GetConduitTemperatureHandle()
		{
			ObjectLayer objectLayer = ((this.manager.conduitType != ConduitType.Gas) ? ObjectLayer.LiquidConduit : ObjectLayer.GasConduit);
			GameObject gameObject = Grid.Objects[this.cell, (int)objectLayer];
			return (!(gameObject != null)) ? HandleVector<int>.InvalidHandle : GameComps.StructureTemperatures.GetHandle(gameObject);
		}

		public int Cell
		{
			get
			{
				return this.cell;
			}
		}

		public int PermittedFlowDirections
		{
			get
			{
				return this.permittedFlowDirections;
			}
			set
			{
				this.permittedFlowDirections = value;
			}
		}

		public ConduitFlow.FlowDirection SrcDirection
		{
			get
			{
				return this.srcDirection;
			}
			set
			{
				this.srcDirection = value;
			}
		}

		public ConduitFlow.FlowDirection TargetDirection
		{
			get
			{
				return this.targetDirection;
			}
			set
			{
				this.targetDirection = value;
			}
		}

		public bool Updated
		{
			get
			{
				return this.updated;
			}
			set
			{
				this.updated = value;
			}
		}

		public ConduitFlow.FlowDirection LastFlowDirection
		{
			get
			{
				return this.lastFlowDirection;
			}
			set
			{
				this.lastFlowDirection = value;
			}
		}

		public SimHashes LastFlowElement
		{
			get
			{
				return this.lastFlowElement;
			}
			set
			{
				this.lastFlowElement = value;
			}
		}

		public ConduitFlow.ConduitContents GetContents()
		{
			return this.manager.grid[this.cell].frontContents;
		}

		public void SetContents(ConduitFlow.ConduitContents contents)
		{
			this.manager.grid[this.cell].frontContents = contents;
			HandleVector<int>.Handle conduitTemperatureHandle = this.GetConduitTemperatureHandle();
			if (conduitTemperatureHandle.IsValid())
			{
				this.manager.temperatureManager.SetData(this.temperatureHandle, conduitTemperatureHandle, ref contents);
			}
		}

		public ConduitFlow.FlowDirection GetNextFlowSource()
		{
			if (this.permittedFlowDirections == -1)
			{
				return ConduitFlow.FlowDirection.Blocked;
			}
			if (this.srcDirection == ConduitFlow.FlowDirection.None)
			{
			}
			for (int i = 0; i < 5; i++)
			{
				int num = this.targetDirection + i - ConduitFlow.FlowDirection.Left;
				int num2 = (num + 1) % 5;
				ConduitFlow.FlowDirection flowDirection = num2 + ConduitFlow.FlowDirection.Left;
				ConduitFlow.IConduit conduitFromDirection = this.GetConduitFromDirection(flowDirection);
				if (conduitFromDirection != null)
				{
					if (conduitFromDirection.GetContents().element != SimHashes.Vacuum)
					{
						if (conduitFromDirection.PermittedFlowDirections != -1)
						{
							ConduitFlow.FlowDirection flowDirection2 = ConduitFlow.InverseFlow(flowDirection);
							ConduitFlow.IConduit conduitFromDirection2 = conduitFromDirection.GetConduitFromDirection(flowDirection2);
							if (conduitFromDirection2 != null && (conduitFromDirection.PermittedFlowDirections & ConduitFlow.FlowBit(flowDirection2)) != 0)
							{
								return flowDirection;
							}
						}
					}
				}
			}
			for (int j = 0; j < 5; j++)
			{
				int num3 = this.targetDirection + j - ConduitFlow.FlowDirection.Left;
				int num4 = (num3 + 1) % 5;
				ConduitFlow.FlowDirection flowDirection3 = num4 + ConduitFlow.FlowDirection.Left;
				ConduitFlow.FlowDirection flowDirection4 = ConduitFlow.InverseFlow(flowDirection3);
				ConduitFlow.IConduit conduitFromDirection3 = this.GetConduitFromDirection(flowDirection3);
				if (conduitFromDirection3 != null)
				{
					if (conduitFromDirection3.PermittedFlowDirections != -1)
					{
						if ((conduitFromDirection3.PermittedFlowDirections & ConduitFlow.FlowBit(flowDirection4)) != 0)
						{
							return flowDirection3;
						}
					}
				}
			}
			return ConduitFlow.FlowDirection.None;
		}

		public ConduitFlow.FlowDirection GetNextFlowTarget()
		{
			if (this.permittedFlowDirections == -1)
			{
				return ConduitFlow.FlowDirection.Blocked;
			}
			for (int i = 0; i < 5; i++)
			{
				int num = this.targetDirection + i - ConduitFlow.FlowDirection.Left;
				int num2 = (num + 1) % 5;
				int num3 = num2 + 1;
				ConduitFlow.IConduit conduitFromDirection = this.GetConduitFromDirection((ConduitFlow.FlowDirection)num3);
				if (conduitFromDirection != null && (this.permittedFlowDirections & ConduitFlow.FlowBit((ConduitFlow.FlowDirection)num3)) != 0)
				{
					return (ConduitFlow.FlowDirection)num3;
				}
			}
			return ConduitFlow.FlowDirection.Blocked;
		}

		public ConduitFlow.IConduit GetConduitFromDirection(ConduitFlow.FlowDirection direction)
		{
			switch (direction)
			{
			case ConduitFlow.FlowDirection.Left:
				return (this.left == -1) ? null : this.manager.conduits[this.left];
			case ConduitFlow.FlowDirection.Right:
				return (this.right == -1) ? null : this.manager.conduits[this.right];
			case ConduitFlow.FlowDirection.Up:
				return (this.up == -1) ? null : this.manager.conduits[this.up];
			case ConduitFlow.FlowDirection.Down:
				return (this.down == -1) ? null : this.manager.conduits[this.down];
			default:
				return null;
			}
		}

		public float Capacity
		{
			get
			{
				return this.manager.MaxMass;
			}
		}

		private ConduitFlow manager;

		public int cell;

		public int idx;

		public int left;

		public int right;

		public int up;

		public int down;

		public int permittedFlowDirections;

		public int sourceIdx;

		public bool updated;

		public ConduitFlow.FlowDirection srcDirection;

		public ConduitFlow.FlowDirection targetDirection;

		public ConduitFlow.FlowDirection lastFlowDirection;

		public SimHashes lastFlowElement;

		public SimHashes initialElement;

		public HandleVector<int>.Handle temperatureHandle = HandleVector<int>.InvalidHandle;
	}

	[DebuggerDisplay("{element},{mass}")]
	public struct ConduitContents
	{
		public ConduitContents(SimHashes element, float mass, float temperature)
		{
			this.element = element;
			this.mass = mass;
			this.temperature = temperature;
		}

		public SimHashes element;

		public float mass;

		public float temperature;
	}

	private class VisibleConduitIterator : IDisposable, IEnumerator, IEnumerator<ConduitFlow.Conduit>
	{
		public VisibleConduitIterator(Vector2I min, Vector2I max, IList<ConduitFlow.Conduit> conduits)
		{
			this.min = min;
			this.max = max;
			this.conduits = conduits;
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public ConduitFlow.Conduit Current
		{
			get
			{
				return this.conduits[this.idx];
			}
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			this.idx++;
			while (this.idx < this.conduits.Count)
			{
				int cell = this.conduits[this.idx].cell;
				Vector2I vector2I = new Vector2I(cell % Grid.WidthInCells, cell / Grid.WidthInCells);
				if (this.min <= vector2I && vector2I <= this.max)
				{
					return true;
				}
				this.idx++;
			}
			return false;
		}

		public void Reset()
		{
			this.idx = -1;
		}

		public int idx = -1;

		private Vector2I min;

		private Vector2I max;

		private IList<ConduitFlow.Conduit> conduits;
	}
}
