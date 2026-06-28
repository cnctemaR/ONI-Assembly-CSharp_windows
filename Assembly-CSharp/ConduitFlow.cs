using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Klei;
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
		network_mgr.ConduitFlowManager = this;
		this.Initialize(num_cells);
	}

	public event global::System.Action onConduitsRebuilt;

	public float MaxConduitCapacity
	{
		get
		{
			return this.MaxMass;
		}
	}

	public void AddConduitUpdater(Action<float> callback, ConduitFlow.Priority priority = ConduitFlow.Priority.Default)
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
			this.grid[i].conduitIdx = -1;
			this.grid[i].contents.element = SimHashes.Vacuum;
			this.grid[i].contents.diseaseIdx = byte.MaxValue;
		}
	}

	public void RebuildConnections(IEnumerable<int> root_nodes)
	{
		for (int i = 0; i < this.conduits.Count; i++)
		{
			ConduitFlow.Conduit conduit = this.conduits[i];
			ConduitFlow.ConduitContents contents = conduit.GetContents();
			if (conduit.temperatureHandle.IsValid())
			{
				contents.temperature = Game.Instance.conduitTemperatureManager.GetData(conduit.temperatureHandle).temperature;
			}
			if (conduit.diseaseHandle.IsValid())
			{
				ConduitDiseaseManager.Data data = Game.Instance.conduitDiseaseManager.GetData(conduit.diseaseHandle);
				contents.diseaseIdx = data.diseaseIdx;
				contents.diseaseCount = data.diseaseCount;
			}
			conduit.SetContents(contents);
			conduit.temperatureHandle = Game.Instance.conduitTemperatureManager.Free(conduit.temperatureHandle);
			conduit.diseaseHandle = Game.Instance.conduitDiseaseManager.Free(conduit.diseaseHandle);
			this.grid[conduit.cell].conduitIdx = -1;
		}
		this.conduits.Clear();
		this.pathList.Clear();
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
				this.grid[num].conduitIdx = conduit2.idx;
			}
		}
		foreach (int num2 in root_nodes)
		{
			UtilityConnections connections = this.networkMgr.GetConnections(num2, true);
			if (connections != (UtilityConnections)0)
			{
				if (this.grid[num2].conduitIdx != -1)
				{
					ConduitFlow.Conduit conduit3 = this.conduits[this.grid[num2].conduitIdx];
					int num3 = num2 - 1;
					if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Left) != (UtilityConnections)0)
					{
						conduit3.left = this.grid[num3].conduitIdx;
					}
					num3 = num2 + 1;
					if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Right) != (UtilityConnections)0)
					{
						conduit3.right = this.grid[num3].conduitIdx;
					}
					num3 = num2 - Grid.WidthInCells;
					if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Down) != (UtilityConnections)0)
					{
						conduit3.down = this.grid[num3].conduitIdx;
					}
					num3 = num2 + Grid.WidthInCells;
					if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Up) != (UtilityConnections)0)
					{
						conduit3.up = this.grid[num3].conduitIdx;
					}
				}
			}
		}
		foreach (ConduitFlow.Conduit conduit4 in this.conduits)
		{
			conduit4.initialContents = conduit4.GetContents();
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
		if (gridNode.conduitIdx != -1)
		{
			this.FindSinks(source_idx, sort_key, this.conduits[gridNode.conduitIdx]);
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
			ConduitFlow.Conduit conduitFromDirection = this.path[i].GetConduitFromDirection(flowDirection);
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
		this.TryAdd(pathInfo);
	}

	private void TryAdd(ConduitFlow.PathInfo new_path_info)
	{
		foreach (ConduitFlow.PathInfo pathInfo in this.pathList)
		{
			if (pathInfo.path.Count > new_path_info.path.Count)
			{
				int num = new_path_info.path[new_path_info.path.Count - 1].Cell;
				int num2 = new_path_info.path[0].Cell;
				for (int i = 0; i < pathInfo.path.Count; i++)
				{
					int cell = pathInfo.path[i].Cell;
					if (cell == num)
					{
						num = -1;
					}
					if (cell == num2)
					{
						num2 = -1;
					}
					if (num == -1 && num2 == -1)
					{
						return;
					}
				}
			}
		}
		for (int j = this.pathList.Count - 1; j >= 0; j--)
		{
			int num3 = this.pathList[j].path[this.pathList[j].path.Count - 1].Cell;
			int num4 = this.pathList[j].path[0].Cell;
			for (int k = 0; k < new_path_info.path.Count; k++)
			{
				int cell2 = new_path_info.path[k].Cell;
				if (cell2 == num3)
				{
					num3 = -1;
				}
				if (cell2 == num4)
				{
					num4 = -1;
				}
				if (num3 == -1 && num4 == -1)
				{
					this.pathList.RemoveAt(j);
					break;
				}
			}
		}
		this.pathList.Add(new_path_info);
	}

	public ConduitFlow.ConduitContents GetContents(int cell)
	{
		ConduitFlow.ConduitContents contents = this.grid[cell].contents;
		ConduitFlow.GridNode gridNode = this.grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			ConduitFlow.Conduit conduit = this.conduits[gridNode.conduitIdx];
			Assert.IsTrue(conduit.temperatureHandle.IsValid());
			Assert.IsTrue(conduit.diseaseHandle.IsValid());
			if (conduit.diseaseHandle.IsValid())
			{
				ConduitDiseaseManager.Data data = Game.Instance.conduitDiseaseManager.GetData(conduit.diseaseHandle);
				contents.diseaseIdx = data.diseaseIdx;
				contents.diseaseCount = data.diseaseCount;
			}
			contents.temperature = Game.Instance.conduitTemperatureManager.GetData(conduit.temperatureHandle).temperature;
		}
		if (contents.mass > 0f && contents.temperature <= 0f)
		{
			Output.LogError(new object[] { "unexpected temperature" });
		}
		return contents;
	}

	public void SetContents(int cell, ConduitFlow.ConduitContents contents)
	{
		ConduitFlow.GridNode gridNode = this.grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			ConduitFlow.Conduit conduit = this.conduits[gridNode.conduitIdx];
			conduit.SetContents(contents);
		}
		else
		{
			this.grid[cell].contents = contents;
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

	public static ConduitFlow.FlowDirection InverseFlow(ConduitFlow.FlowDirection direction)
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
			ConduitFlow.ConduitContents contents = conduit.GetContents();
			conduit.initialContents = contents;
			conduit.lastFlowContents = ConduitFlow.ConduitContents.EmptyContents();
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
			this.conduitUpdaters.Sort((ConduitFlow.ConduitUpdater a, ConduitFlow.ConduitUpdater b) => a.priority - b.priority);
		}
		for (int i = 0; i < this.conduitUpdaters.Count; i++)
		{
			this.conduitUpdaters[i].callback(num);
		}
	}

	private void UpdateConduit(ConduitFlow.Conduit conduit)
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
			conduit.lastFlowContents = ConduitFlow.ConduitContents.EmptyContents();
			conduit.TargetDirection = conduit.GetNextFlowTarget();
			return;
		}
		ConduitFlow.Conduit conduitFromDirection = conduit.GetConduitFromDirection(conduit.TargetDirection);
		if (conduitFromDirection == null)
		{
			conduit.TargetDirection = conduit.GetNextFlowTarget();
			return;
		}
		ConduitFlow.ConduitContents contents2 = conduitFromDirection.GetContents();
		if (contents2.element != SimHashes.Vacuum && contents2.element != contents.element)
		{
			conduit.lastFlowContents = ConduitFlow.ConduitContents.EmptyContents();
			conduit.TargetDirection = conduit.GetNextFlowTarget();
			return;
		}
		if ((conduit.PermittedFlowDirections & ConduitFlow.FlowBit(conduit.TargetDirection)) != 0)
		{
			bool flag = false;
			for (int i = 0; i < 5; i++)
			{
				ConduitFlow.Conduit conduitFromDirection2 = conduitFromDirection.GetConduitFromDirection(conduitFromDirection.SrcDirection);
				if (conduitFromDirection2 == conduit)
				{
					flag = true;
					break;
				}
				if (conduitFromDirection2 != null && conduitFromDirection2.GetContents().element != SimHashes.Vacuum)
				{
					break;
				}
				conduitFromDirection.SrcDirection = conduitFromDirection.GetNextFlowSource();
			}
			if (flag)
			{
				float num = Mathf.Max(0f, this.MaxMass - contents2.mass);
				float num2 = Mathf.Min(contents.mass, num);
				if (num2 > 0f)
				{
					int num3 = (int)(num2 / contents.mass * (float)contents.diseaseCount);
					num2 = this.AddElement(conduitFromDirection.Cell, contents.element, num2, contents.temperature, contents.diseaseIdx, num3);
					ConduitFlow.ConduitContents conduitContents = this.RemoveElement(conduit, num2);
					conduit.lastFlowDirection = conduit.TargetDirection;
					conduit.lastFlowContents = conduitContents;
					conduitFromDirection.Updated = true;
					conduitFromDirection.SrcDirection = conduitFromDirection.GetNextFlowSource();
					this.TriggerListeners(conduitFromDirection);
					this.TriggerListeners(conduit);
				}
			}
		}
		conduit.TargetDirection = conduit.GetNextFlowTarget();
	}

	private void TriggerListeners(ConduitFlow.Conduit conduit)
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

	public float AddElement(int cell_idx, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (this.grid[cell_idx].conduitIdx == -1)
		{
			return 0f;
		}
		ConduitFlow.ConduitContents contents = this.GetConduit(cell_idx).GetContents();
		if (contents.element != element && contents.element != SimHashes.Vacuum && mass > 0f)
		{
			return 0f;
		}
		float num = Mathf.Min(mass, this.MaxMass - contents.mass);
		float num2 = num / mass;
		if (num <= 0f)
		{
			return 0f;
		}
		contents.temperature = GameUtil.GetFinalTemperature(temperature, num, contents.temperature, contents.mass);
		contents.mass += num;
		contents.element = element;
		int num3 = (int)(num2 * (float)disease_count);
		if (num3 > 0)
		{
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, num3, contents.diseaseIdx, contents.diseaseCount);
			contents.diseaseIdx = diseaseInfo.idx;
			contents.diseaseCount = diseaseInfo.count;
		}
		this.SetContents(cell_idx, contents);
		return num;
	}

	public ConduitFlow.ConduitContents RemoveElement(int cell, float delta)
	{
		ConduitFlow.Conduit conduit = this.GetConduit(cell);
		if (conduit != null)
		{
			return this.RemoveElement(conduit, delta);
		}
		return ConduitFlow.ConduitContents.EmptyContents();
	}

	public ConduitFlow.ConduitContents RemoveElement(ConduitFlow.Conduit conduit, float delta)
	{
		ConduitFlow.ConduitContents contents = conduit.GetContents();
		ConduitFlow.ConduitContents conduitContents = contents;
		ConduitFlow.ConduitContents conduitContents2 = default(ConduitFlow.ConduitContents);
		conduitContents.mass = Mathf.Min(contents.mass, delta);
		float num = contents.mass - conduitContents.mass;
		if (num <= 0f)
		{
			conduitContents2.mass = 0f;
			conduitContents2.temperature = 0f;
			conduitContents2.element = SimHashes.Vacuum;
			conduitContents2.diseaseIdx = byte.MaxValue;
			conduitContents2.diseaseCount = 0;
		}
		else
		{
			float num2 = num / contents.mass;
			int num3 = (int)(num2 * (float)contents.diseaseCount);
			conduitContents.diseaseCount = contents.diseaseCount - num3;
			conduitContents2.mass = num;
			conduitContents2.temperature = contents.temperature;
			conduitContents2.element = contents.element;
			conduitContents2.diseaseIdx = contents.diseaseIdx;
			conduitContents2.diseaseCount = num3;
			if (num3 <= 0)
			{
				conduitContents2.diseaseIdx = byte.MaxValue;
				conduitContents2.diseaseCount = 0;
			}
		}
		conduit.SetContents(conduitContents2);
		return conduitContents;
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
		int conduitIdx = this.grid[cell].conduitIdx;
		return (conduitIdx == -1) ? null : this.conduits[conduitIdx];
	}

	private void DumpPipeContents(int cell, ConduitFlow.ConduitContents contents)
	{
		if (contents.element != SimHashes.Vacuum && contents.mass > 0f)
		{
			SimMessages.AddRemoveSubstance(cell, contents.element, CellEventLogger.Instance.ConduitFlowEmptyConduit, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount, -1);
			contents.mass = 0f;
			contents.temperature = 0f;
			contents.element = SimHashes.Vacuum;
			contents.diseaseIdx = byte.MaxValue;
			contents.diseaseCount = 0;
			this.SetContents(cell, contents);
		}
	}

	public void EmptyConduit(int cell)
	{
		if (this.replacements.Contains(cell))
		{
			return;
		}
		this.DumpPipeContents(cell, this.grid[cell].contents);
	}

	public void MarkForReplacement(int cell)
	{
		this.replacements.Add(cell);
	}

	public void DeactivateCell(int cell)
	{
		this.grid[cell].conduitIdx = -1;
		ConduitFlow.ConduitContents conduitContents = new ConduitFlow.ConduitContents(SimHashes.Vacuum, 0f, 0f, byte.MaxValue, 0);
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
			this.versionedSerializedContents = new ConduitFlow.SerializedContents[count];
			this.serializedIdx = new int[count];
			for (int i = 0; i < count; i++)
			{
				ConduitFlow.Conduit conduit = this.conduits[i];
				ConduitFlow.ConduitContents contents = conduit.GetContents();
				this.serializedIdx[i] = conduit.cell;
				this.versionedSerializedContents[i] = new ConduitFlow.SerializedContents(contents);
			}
		}
		else
		{
			this.serializedContents = null;
			this.versionedSerializedContents = null;
			this.serializedIdx = null;
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		this.versionedSerializedContents = null;
		this.serializedContents = null;
		this.serializedIdx = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.serializedContents != null)
		{
			this.versionedSerializedContents = new ConduitFlow.SerializedContents[this.serializedContents.Length];
			for (int i = 0; i < this.serializedContents.Length; i++)
			{
				this.versionedSerializedContents[i] = new ConduitFlow.SerializedContents(this.serializedContents[i]);
			}
			this.serializedContents = null;
		}
		if (this.versionedSerializedContents == null)
		{
			return;
		}
		ConduitFlow.ConduitContents conduitContents = default(ConduitFlow.ConduitContents);
		for (int j = 0; j < this.versionedSerializedContents.Length; j++)
		{
			int num = this.serializedIdx[j];
			ConduitFlow.SerializedContents serializedContents = this.versionedSerializedContents[j];
			if (serializedContents.mass <= 0f)
			{
				conduitContents.element = SimHashes.Vacuum;
				conduitContents.mass = 0f;
				conduitContents.temperature = 0f;
			}
			else
			{
				conduitContents.element = serializedContents.element;
				conduitContents.mass = serializedContents.mass;
				conduitContents.temperature = serializedContents.temperature;
			}
			if (serializedContents.diseaseCount <= 0 || serializedContents.diseaseHash == 0)
			{
				conduitContents.diseaseCount = 0;
				conduitContents.diseaseIdx = byte.MaxValue;
			}
			else
			{
				conduitContents.diseaseIdx = Db.Get().Diseases.GetIndex(serializedContents.diseaseHash);
				conduitContents.diseaseCount = ((conduitContents.diseaseIdx != byte.MaxValue) ? serializedContents.diseaseCount : 0);
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
		this.versionedSerializedContents = null;
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
		ConduitFlow.ConduitContents contents = this.grid[cell_idx].contents;
		return this.MaxMass - contents.mass <= 0f;
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

	private ConduitFlow.GridNode[] grid;

	[Serialize]
	public int[] serializedIdx;

	[Serialize]
	public ConduitFlow.ConduitContents[] serializedContents;

	[Serialize]
	public ConduitFlow.SerializedContents[] versionedSerializedContents;

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

	public enum Priority
	{
		First = -100,
		Default = 0,
		Last = 100
	}

	[DebuggerDisplay("{priority} {callback.Target.name} {callback.Target} {callback.Method}")]
	public struct ConduitUpdater
	{
		public ConduitFlow.Priority priority;

		public Action<float> callback;
	}

	public struct GridNode
	{
		public int conduitIdx;

		public ConduitFlow.ConduitContents contents;
	}

	public struct SerializedContents
	{
		public SerializedContents(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
		{
			this.element = element;
			this.mass = mass;
			this.temperature = temperature;
			this.diseaseHash = ((disease_idx == byte.MaxValue) ? 0 : Db.Get().Diseases[(int)disease_idx].id.GetHashCode());
			this.diseaseCount = disease_count;
			if (this.diseaseCount <= 0)
			{
				this.diseaseHash = 0;
			}
		}

		public SerializedContents(ConduitFlow.ConduitContents src)
		{
			this = new ConduitFlow.SerializedContents(src.element, src.mass, src.temperature, src.diseaseIdx, src.diseaseCount);
		}

		public SimHashes element;

		public float mass;

		public float temperature;

		public int diseaseHash;

		public int diseaseCount;
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

	[DebuggerDisplay("{cell}")]
	[Serializable]
	public class Conduit
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
			this.temperatureHandle = Game.Instance.conduitTemperatureManager.Allocate(handle, ref contents);
			this.diseaseHandle = Game.Instance.conduitDiseaseManager.Allocate(this.temperatureHandle, ref contents);
			this.conduitGO = conduit_go;
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

		public ConduitFlow.ConduitContents GetContents()
		{
			ConduitFlow.ConduitContents contents = this.manager.grid[this.cell].contents;
			if (this.temperatureHandle.IsValid())
			{
				contents.temperature = Game.Instance.conduitTemperatureManager.GetData(this.temperatureHandle).temperature;
			}
			if (this.diseaseHandle.IsValid())
			{
				ConduitDiseaseManager.Data data = Game.Instance.conduitDiseaseManager.GetData(this.diseaseHandle);
				contents.diseaseIdx = data.diseaseIdx;
				contents.diseaseCount = data.diseaseCount;
			}
			return contents;
		}

		public void SetContents(ConduitFlow.ConduitContents contents)
		{
			this.manager.grid[this.cell].contents = contents;
			HandleVector<int>.Handle conduitTemperatureHandle = this.GetConduitTemperatureHandle();
			if (conduitTemperatureHandle.IsValid())
			{
				Game.Instance.conduitTemperatureManager.SetData(this.temperatureHandle, conduitTemperatureHandle, ref contents);
			}
			if (this.conduitGO != null)
			{
				PrimaryElement component = this.conduitGO.GetComponent<PrimaryElement>();
				if (component != null)
				{
					component.ForcePermanentDiseaseContainer(contents.diseaseIdx != byte.MaxValue);
				}
			}
			Game.Instance.conduitDiseaseManager.SetData(this.diseaseHandle, ref contents);
		}

		public ConduitFlow.FlowDirection GetNextFlowSource()
		{
			if (this.permittedFlowDirections == -1)
			{
				return ConduitFlow.FlowDirection.Blocked;
			}
			ConduitFlow.FlowDirection flowDirection = this.srcDirection;
			if (flowDirection == ConduitFlow.FlowDirection.None)
			{
				flowDirection = ConduitFlow.FlowDirection.Down;
			}
			for (int i = 0; i < 5; i++)
			{
				int num = flowDirection + i - ConduitFlow.FlowDirection.Left;
				int num2 = (num + 1) % 5;
				ConduitFlow.FlowDirection flowDirection2 = num2 + ConduitFlow.FlowDirection.Left;
				ConduitFlow.Conduit conduitFromDirection = this.GetConduitFromDirection(flowDirection2);
				if (conduitFromDirection != null)
				{
					if (conduitFromDirection.GetContents().element != SimHashes.Vacuum)
					{
						if (conduitFromDirection.PermittedFlowDirections != -1)
						{
							ConduitFlow.FlowDirection flowDirection3 = ConduitFlow.InverseFlow(flowDirection2);
							ConduitFlow.Conduit conduitFromDirection2 = conduitFromDirection.GetConduitFromDirection(flowDirection3);
							if (conduitFromDirection2 != null && (conduitFromDirection.PermittedFlowDirections & ConduitFlow.FlowBit(flowDirection3)) != 0)
							{
								return flowDirection2;
							}
						}
					}
				}
			}
			for (int j = 0; j < 5; j++)
			{
				int num3 = this.targetDirection + j - ConduitFlow.FlowDirection.Left;
				int num4 = (num3 + 1) % 5;
				ConduitFlow.FlowDirection flowDirection4 = num4 + ConduitFlow.FlowDirection.Left;
				ConduitFlow.FlowDirection flowDirection5 = ConduitFlow.InverseFlow(flowDirection4);
				ConduitFlow.Conduit conduitFromDirection3 = this.GetConduitFromDirection(flowDirection4);
				if (conduitFromDirection3 != null)
				{
					if (conduitFromDirection3.PermittedFlowDirections != -1)
					{
						if ((conduitFromDirection3.PermittedFlowDirections & ConduitFlow.FlowBit(flowDirection5)) != 0)
						{
							return flowDirection4;
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
				ConduitFlow.Conduit conduitFromDirection = this.GetConduitFromDirection((ConduitFlow.FlowDirection)num3);
				if (conduitFromDirection != null && (this.permittedFlowDirections & ConduitFlow.FlowBit((ConduitFlow.FlowDirection)num3)) != 0)
				{
					return (ConduitFlow.FlowDirection)num3;
				}
			}
			return ConduitFlow.FlowDirection.Blocked;
		}

		public ConduitFlow.Conduit GetConduitFromDirection(ConduitFlow.FlowDirection direction)
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

		private GameObject conduitGO;

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

		public ConduitFlow.ConduitContents initialContents;

		public ConduitFlow.ConduitContents lastFlowContents;

		public HandleVector<int>.Handle temperatureHandle = HandleVector<int>.InvalidHandle;

		public HandleVector<int>.Handle diseaseHandle = HandleVector<int>.InvalidHandle;
	}

	[DebuggerDisplay("{element},{mass}")]
	public struct ConduitContents
	{
		public ConduitContents(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
		{
			this.element = element;
			this.mass = mass;
			this.temperature = temperature;
			this.diseaseIdx = disease_idx;
			this.diseaseCount = disease_count;
		}

		public static ConduitFlow.ConduitContents EmptyContents()
		{
			return new ConduitFlow.ConduitContents
			{
				element = SimHashes.Vacuum,
				mass = 0f,
				temperature = 0f,
				diseaseIdx = byte.MaxValue,
				diseaseCount = 0
			};
		}

		public SimHashes element;

		public float mass;

		public float temperature;

		public byte diseaseIdx;

		public int diseaseCount;
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
