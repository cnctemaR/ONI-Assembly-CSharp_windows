using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitFlow : IConduitFlow
{
	public ConduitFlow(ConduitType conduit_type, int num_cells, IUtilityNetworkMgr network_mgr, float max_conduit_mass, float initial_elapsed_time)
	{
		this.elapsedTime = initial_elapsed_time;
		this.conduitType = conduit_type;
		this.networkMgr = network_mgr;
		this.MaxMass = max_conduit_mass;
		this.Initialize(num_cells);
		network_mgr.AddNetworksRebuiltListener(new Action<IList<UtilityNetwork>, ICollection<int>>(this.OnUtilityNetworksRebuilt));
	}

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onConduitsRebuilt;

	public void AddConduitUpdater(Action<float> callback, ConduitFlowPriority priority = ConduitFlowPriority.Default)
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

	private void OnUtilityNetworksRebuilt(IList<UtilityNetwork> networks, ICollection<int> root_nodes)
	{
		this.RebuildConnections(root_nodes);
		foreach (UtilityNetwork utilityNetwork in networks)
		{
			FlowUtilityNetwork flowUtilityNetwork = (FlowUtilityNetwork)utilityNetwork;
			this.ScanNetworkSources(flowUtilityNetwork);
		}
		this.RefreshPaths();
	}

	private void RebuildConnections(IEnumerable<int> root_nodes)
	{
		this.soaInfo.Clear(this);
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
				global::Conduit component = gameObject.GetComponent<global::Conduit>();
				if (!(component != null) || !component.IsDisconnected())
				{
					int num2 = this.soaInfo.AddConduit(this, gameObject, num);
					this.grid[num].conduitIdx = num2;
				}
			}
		}
		Game.Instance.conduitTemperatureManager.Sim200ms(0f);
		foreach (int num3 in root_nodes)
		{
			UtilityConnections connections = this.networkMgr.GetConnections(num3, true);
			if (connections != (UtilityConnections)0)
			{
				if (this.grid[num3].conduitIdx != -1)
				{
					int conduitIdx = this.grid[num3].conduitIdx;
					ConduitFlow.ConduitConnections conduitConnections = this.soaInfo.GetConduitConnections(conduitIdx);
					int num4 = num3 - 1;
					if (Grid.IsValidCell(num4) && (connections & UtilityConnections.Left) != (UtilityConnections)0)
					{
						conduitConnections.left = this.grid[num4].conduitIdx;
					}
					num4 = num3 + 1;
					if (Grid.IsValidCell(num4) && (connections & UtilityConnections.Right) != (UtilityConnections)0)
					{
						conduitConnections.right = this.grid[num4].conduitIdx;
					}
					num4 = num3 - Grid.WidthInCells;
					if (Grid.IsValidCell(num4) && (connections & UtilityConnections.Down) != (UtilityConnections)0)
					{
						conduitConnections.down = this.grid[num4].conduitIdx;
					}
					num4 = num3 + Grid.WidthInCells;
					if (Grid.IsValidCell(num4) && (connections & UtilityConnections.Up) != (UtilityConnections)0)
					{
						conduitConnections.up = this.grid[num4].conduitIdx;
					}
					this.soaInfo.SetConduitConnections(conduitIdx, conduitConnections);
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
		for (int i = 0; i < network.sources.Count; i++)
		{
			FlowUtilityNetwork.IItem item = network.sources[i];
			this.path.Clear();
			this.visited.Clear();
			this.FindSinks(i, item.Cell);
		}
	}

	public void RefreshPaths()
	{
		foreach (List<ConduitFlow.Conduit> list in this.pathList)
		{
			for (int i = 0; i < list.Count - 1; i++)
			{
				ConduitFlow.Conduit conduit = list[i];
				ConduitFlow.Conduit conduit2 = list[i + 1];
				if (conduit.GetTargetFlowDirection(this) == ConduitFlow.FlowDirection.None)
				{
					ConduitFlow.FlowDirection direction = this.GetDirection(conduit, conduit2);
					conduit.SetTargetFlowDirection(direction, this);
				}
			}
		}
	}

	private void FindSinks(int source_idx, int cell)
	{
		ConduitFlow.GridNode gridNode = this.grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			this.FindSinksInternal(source_idx, gridNode.conduitIdx);
		}
	}

	private void FindSinksInternal(int source_idx, int conduit_idx)
	{
		if (this.visited.Contains(conduit_idx))
		{
			return;
		}
		this.visited.Add(conduit_idx);
		ConduitFlow.Conduit conduit = this.soaInfo.GetConduit(conduit_idx);
		int permittedFlowDirections = conduit.GetPermittedFlowDirections(this);
		if (permittedFlowDirections == -1)
		{
			return;
		}
		this.path.Add(conduit);
		FlowUtilityNetwork.IItem item = (FlowUtilityNetwork.IItem)this.networkMgr.GetEndpoint(this.soaInfo.GetCell(conduit_idx));
		if (item != null && item.EndpointType == Endpoint.Sink)
		{
			this.FoundSink(source_idx);
		}
		ConduitFlow.ConduitConnections conduitConnections = this.soaInfo.GetConduitConnections(conduit_idx);
		if (conduitConnections.down != -1)
		{
			this.FindSinksInternal(source_idx, conduitConnections.down);
		}
		if (conduitConnections.left != -1)
		{
			this.FindSinksInternal(source_idx, conduitConnections.left);
		}
		if (conduitConnections.right != -1)
		{
			this.FindSinksInternal(source_idx, conduitConnections.right);
		}
		if (conduitConnections.up != -1)
		{
			this.FindSinksInternal(source_idx, conduitConnections.up);
		}
		if (this.path.Count > 0)
		{
			this.path.RemoveAt(this.path.Count - 1);
		}
	}

	private ConduitFlow.FlowDirection GetDirection(ConduitFlow.Conduit conduit, ConduitFlow.Conduit target_conduit)
	{
		ConduitFlow.ConduitConnections conduitConnections = this.soaInfo.GetConduitConnections(conduit.idx);
		if (conduitConnections.up == target_conduit.idx)
		{
			return ConduitFlow.FlowDirection.Up;
		}
		if (conduitConnections.down == target_conduit.idx)
		{
			return ConduitFlow.FlowDirection.Down;
		}
		if (conduitConnections.left == target_conduit.idx)
		{
			return ConduitFlow.FlowDirection.Left;
		}
		if (conduitConnections.right == target_conduit.idx)
		{
			return ConduitFlow.FlowDirection.Right;
		}
		return ConduitFlow.FlowDirection.None;
	}

	private void FoundSink(int source_idx)
	{
		for (int i = 0; i < this.path.Count - 1; i++)
		{
			ConduitFlow.FlowDirection direction = this.GetDirection(this.path[i], this.path[i + 1]);
			ConduitFlow.FlowDirection flowDirection = ConduitFlow.InverseFlow(direction);
			int cellFromDirection = ConduitFlow.GetCellFromDirection(this.soaInfo.GetCell(this.path[i].idx), flowDirection);
			ConduitFlow.Conduit conduitFromDirection = this.soaInfo.GetConduitFromDirection(this.path[i].idx, flowDirection);
			if (i == 0 || (this.path[i].GetPermittedFlowDirections(this) & ConduitFlow.FlowBit(flowDirection)) == 0 || (cellFromDirection != this.soaInfo.GetCell(this.path[i - 1].idx) && (this.soaInfo.GetSrcFlowIdx(this.path[i].idx) == source_idx || (conduitFromDirection.GetPermittedFlowDirections(this) & ConduitFlow.FlowBit(flowDirection)) == 0)))
			{
				int permittedFlowDirections = this.path[i].GetPermittedFlowDirections(this);
				this.soaInfo.SetSrcFlowIdx(this.path[i].idx, source_idx);
				this.path[i].SetPermittedFlowDirections(permittedFlowDirections | ConduitFlow.FlowBit(direction), this);
				this.path[i].SetTargetFlowDirection(direction, this);
			}
		}
		for (int j = 1; j < this.path.Count; j++)
		{
			ConduitFlow.FlowDirection direction2 = this.GetDirection(this.path[j], this.path[j - 1]);
			this.soaInfo.SetSrcFlowDirection(this.path[j].idx, direction2);
		}
		List<ConduitFlow.Conduit> list = new List<ConduitFlow.Conduit>(this.path);
		list.Reverse();
		this.TryAdd(list);
	}

	private static int FindIndex(List<ConduitFlow.Conduit> path, int idx)
	{
		for (int i = 0; i < path.Count; i++)
		{
			if (path[i].idx == idx)
			{
				return i;
			}
		}
		return -1;
	}

	private void TryAdd(List<ConduitFlow.Conduit> new_path)
	{
		foreach (List<ConduitFlow.Conduit> list in this.pathList)
		{
			if (list.Count >= new_path.Count)
			{
				bool flag = false;
				int num = ConduitFlow.FindIndex(list, new_path[0].idx);
				int num2 = ConduitFlow.FindIndex(list, new_path[new_path.Count - 1].idx);
				if (num != -1 && num2 != -1)
				{
					flag = true;
					int i = num;
					int num3 = 0;
					while (i < num2)
					{
						if (list[i].idx != new_path[num3].idx)
						{
							flag = false;
							break;
						}
						i++;
						num3++;
					}
				}
				if (flag)
				{
					return;
				}
			}
		}
		for (int j = this.pathList.Count - 1; j >= 0; j--)
		{
			if (this.pathList[j].Count <= 0)
			{
				this.pathList.RemoveAt(j);
			}
		}
		for (int k = this.pathList.Count - 1; k >= 0; k--)
		{
			List<ConduitFlow.Conduit> list2 = this.pathList[k];
			if (new_path.Count >= list2.Count)
			{
				bool flag2 = false;
				int num4 = ConduitFlow.FindIndex(new_path, list2[0].idx);
				int num5 = ConduitFlow.FindIndex(new_path, list2[list2.Count - 1].idx);
				if (num4 != -1 && num5 != -1)
				{
					flag2 = true;
					int l = num4;
					int num6 = 0;
					while (l < num5)
					{
						if (new_path[l].idx != list2[num6].idx)
						{
							flag2 = false;
							break;
						}
						l++;
						num6++;
					}
				}
				if (flag2)
				{
					this.pathList.RemoveAt(k);
				}
			}
		}
		foreach (List<ConduitFlow.Conduit> list3 in this.pathList)
		{
			for (int m = new_path.Count - 1; m >= 0; m--)
			{
				ConduitFlow.Conduit conduit = new_path[m];
				int num7 = ConduitFlow.FindIndex(list3, conduit.idx);
				if (num7 != -1)
				{
					int permittedFlowDirections = this.soaInfo.GetPermittedFlowDirections(conduit.idx);
					if (Mathf.IsPowerOfTwo(permittedFlowDirections))
					{
						new_path.RemoveAt(m);
					}
				}
			}
		}
		this.pathList.Add(new_path);
	}

	public ConduitFlow.ConduitContents GetContents(int cell)
	{
		ConduitFlow.ConduitContents conduitContents = this.grid[cell].contents;
		ConduitFlow.GridNode gridNode = this.grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			conduitContents = this.soaInfo.GetConduit(gridNode.conduitIdx).GetContents(this);
		}
		if (conduitContents.mass > 0f && conduitContents.temperature <= 0f)
		{
			Output.LogError(new object[] { "unexpected temperature" });
		}
		return conduitContents;
	}

	public void SetContents(int cell, ConduitFlow.ConduitContents contents)
	{
		ConduitFlow.GridNode gridNode = this.grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			this.soaInfo.GetConduit(gridNode.conduitIdx).SetContents(this, contents);
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

	public void Sim200ms(float dt)
	{
		if (dt <= 0f)
		{
			return;
		}
		this.elapsedTime += dt;
		if (this.elapsedTime < 1f)
		{
			return;
		}
		float num = 1f;
		this.elapsedTime -= 1f;
		this.lastUpdateTime = Time.time;
		this.soaInfo.BeginFrame(this);
		foreach (List<ConduitFlow.Conduit> list in this.pathList)
		{
			foreach (ConduitFlow.Conduit conduit in list)
			{
				this.UpdateConduit(conduit);
			}
		}
		if (this.dirtyConduitUpdaters)
		{
			this.conduitUpdaters.Sort((ConduitFlow.ConduitUpdater a, ConduitFlow.ConduitUpdater b) => a.priority - b.priority);
		}
		this.soaInfo.EndFrame(this);
		for (int i = 0; i < this.conduitUpdaters.Count; i++)
		{
			this.conduitUpdaters[i].callback(num);
		}
	}

	private void UpdateConduit(ConduitFlow.Conduit conduit)
	{
		if (this.soaInfo.GetUpdated(conduit.idx))
		{
			return;
		}
		if (this.soaInfo.GetSrcFlowDirection(conduit.idx) == ConduitFlow.FlowDirection.None)
		{
			this.soaInfo.SetSrcFlowDirection(conduit.idx, conduit.GetNextFlowSource(this));
		}
		int cell = this.soaInfo.GetCell(conduit.idx);
		ConduitFlow.ConduitContents contents = this.grid[cell].contents;
		if (contents.element == SimHashes.Vacuum)
		{
			return;
		}
		if (contents.mass <= 0f)
		{
			this.soaInfo.MarkConduitEmpty(conduit.idx, this);
			return;
		}
		ConduitFlow.FlowDirection targetFlowDirection = this.soaInfo.GetTargetFlowDirection(conduit.idx);
		ConduitFlow.Conduit conduitFromDirection = this.soaInfo.GetConduitFromDirection(conduit.idx, targetFlowDirection);
		if (conduitFromDirection.idx == -1)
		{
			this.soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
			return;
		}
		int cell2 = this.soaInfo.GetCell(conduitFromDirection.idx);
		ConduitFlow.ConduitContents contents2 = this.grid[cell2].contents;
		if (contents2.element != SimHashes.Vacuum && contents2.element != contents.element)
		{
			this.soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
			return;
		}
		int permittedFlowDirections = this.soaInfo.GetPermittedFlowDirections(conduit.idx);
		if ((permittedFlowDirections & ConduitFlow.FlowBit(targetFlowDirection)) != 0)
		{
			bool flag = false;
			for (int i = 0; i < 5; i++)
			{
				ConduitFlow.Conduit conduitFromDirection2 = this.soaInfo.GetConduitFromDirection(conduitFromDirection.idx, this.soaInfo.GetSrcFlowDirection(conduitFromDirection.idx));
				if (conduitFromDirection2.idx == conduit.idx)
				{
					flag = true;
					break;
				}
				if (conduitFromDirection2.idx != -1)
				{
					int cell3 = this.soaInfo.GetCell(conduitFromDirection2.idx);
					ConduitFlow.ConduitContents contents3 = this.grid[cell3].contents;
					if (contents3.element != SimHashes.Vacuum)
					{
						break;
					}
				}
				this.soaInfo.SetSrcFlowDirection(conduitFromDirection.idx, conduitFromDirection.GetNextFlowSource(this));
			}
			if (flag)
			{
				float num = Mathf.Max(0f, this.MaxMass - contents2.mass);
				float num2 = Mathf.Min(contents.mass, num);
				if (num2 > 0f)
				{
					int num3 = (int)(num2 / contents.mass * (float)contents.diseaseCount);
					num2 = this.AddElementToGrid(cell2, contents.element, num2, contents.temperature, contents.diseaseIdx, num3);
					ConduitFlow.ConduitContents conduitContents = this.RemoveElementFromGrid(conduit, num2);
					this.soaInfo.SetLastFlowInfo(conduit.idx, this.soaInfo.GetTargetFlowDirection(conduit.idx), ref conduitContents);
					this.soaInfo.SetUpdated(conduitFromDirection.idx, true);
					this.soaInfo.SetSrcFlowDirection(conduitFromDirection.idx, conduitFromDirection.GetNextFlowSource(this));
				}
			}
		}
		this.soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
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
		ConduitFlow.ConduitContents contents = this.GetConduit(cell_idx).GetContents(this);
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

	private float AddElementToGrid(int cell_idx, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		ConduitFlow.ConduitContents contents = this.grid[cell_idx].contents;
		if (contents.element != element && contents.element != SimHashes.Vacuum && mass > 0f)
		{
			return 0f;
		}
		float num = Mathf.Min(mass, this.MaxMass - contents.mass);
		if (num <= 0f)
		{
			return 0f;
		}
		contents.temperature = GameUtil.GetFinalTemperature(temperature, num, contents.temperature, contents.mass);
		contents.mass += num;
		contents.element = element;
		float num2 = num / mass;
		int num3 = (int)(num2 * (float)disease_count);
		if (num3 > 0)
		{
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, num3, contents.diseaseIdx, contents.diseaseCount);
			contents.diseaseIdx = diseaseInfo.idx;
			contents.diseaseCount = diseaseInfo.count;
		}
		this.grid[cell_idx].contents = contents;
		return num;
	}

	public ConduitFlow.ConduitContents RemoveElement(int cell, float delta)
	{
		ConduitFlow.Conduit conduit = this.GetConduit(cell);
		if (conduit.idx != -1)
		{
			return this.RemoveElement(conduit, delta);
		}
		return ConduitFlow.ConduitContents.EmptyContents();
	}

	public ConduitFlow.ConduitContents RemoveElement(ConduitFlow.Conduit conduit, float delta)
	{
		ConduitFlow.ConduitContents contents = conduit.GetContents(this);
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
		conduit.SetContents(this, conduitContents2);
		return conduitContents;
	}

	private ConduitFlow.ConduitContents RemoveElementFromGrid(ConduitFlow.Conduit conduit, float delta)
	{
		int cell = this.soaInfo.GetCell(conduit.idx);
		ConduitFlow.ConduitContents contents = this.grid[cell].contents;
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
		this.grid[cell].contents = conduitContents2;
		return conduitContents;
	}

	public int GetPermittedFlow(int cell)
	{
		ConduitFlow.Conduit conduit = this.GetConduit(cell);
		if (conduit.idx == -1)
		{
			return 0;
		}
		return this.soaInfo.GetPermittedFlowDirections(conduit.idx);
	}

	public bool HasConduit(int cell)
	{
		return this.grid[cell].conduitIdx != -1;
	}

	public ConduitFlow.Conduit GetConduit(int cell)
	{
		int conduitIdx = this.grid[cell].conduitIdx;
		return (conduitIdx == -1) ? ConduitFlow.Conduit.Invalid() : this.soaInfo.GetConduit(conduitIdx);
	}

	private void DumpPipeContents(int cell, ConduitFlow.ConduitContents contents)
	{
		if (contents.element != SimHashes.Vacuum && contents.mass > 0f)
		{
			SimMessages.AddRemoveSubstance(cell, contents.element, CellEventLogger.Instance.ConduitFlowEmptyConduit, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount, true, -1);
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
		if (contents.mass > 0f && contents.temperature <= 0f)
		{
			Output.LogError(new object[] { "zero degree pipe contents" });
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		int numEntries = this.soaInfo.NumEntries;
		if (numEntries > 0)
		{
			this.versionedSerializedContents = new ConduitFlow.SerializedContents[numEntries];
			this.serializedIdx = new int[numEntries];
			for (int i = 0; i < numEntries; i++)
			{
				ConduitFlow.Conduit conduit = this.soaInfo.GetConduit(i);
				ConduitFlow.ConduitContents contents = conduit.GetContents(this);
				this.serializedIdx[i] = this.soaInfo.GetCell(conduit.idx);
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
			if (float.IsNaN(conduitContents.temperature) || (conduitContents.temperature <= 0f && conduitContents.element != SimHashes.Vacuum) || 10000f < conduitContents.temperature)
			{
				Vector2I vector2I = Grid.CellToXY(num);
				DeserializeWarnings.Instance.PipeContentsTemperatureIsNan.Warn(string.Format("Invalid pipe content temperature of {0} detected. Resetting temperature. (x={1}, y={2}, cell={3})", new object[] { conduitContents.temperature, vector2I.x, vector2I.y, num }), null);
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
		int cell = this.soaInfo.GetCell(conduit.idx);
		return this.networkMgr.GetNetworkForCell(cell);
	}

	public void ForceRebuildNetworks()
	{
		this.networkMgr.ForceRebuildNetworks();
	}

	public bool IsConduitFull(int cell_idx)
	{
		ConduitFlow.ConduitContents contents = this.grid[cell_idx].contents;
		return this.MaxMass - contents.mass <= 0f;
	}

	public bool IsConduitEmpty(int cell_idx)
	{
		ConduitFlow.ConduitContents contents = this.grid[cell_idx].contents;
		return contents.mass <= 0f;
	}

	public void FreezeConduitContents(int conduit_idx)
	{
		GameObject conduitGO = this.soaInfo.GetConduitGO(conduit_idx);
		if (conduitGO != null && this.soaInfo.GetConduit(conduit_idx).GetContents(this).mass > this.MaxMass * 0.1f)
		{
			conduitGO.Trigger(-700727624, null);
		}
	}

	public void MeltConduitContents(int conduit_idx)
	{
		GameObject conduitGO = this.soaInfo.GetConduitGO(conduit_idx);
		if (conduitGO != null && this.soaInfo.GetConduit(conduit_idx).GetContents(this).mass > this.MaxMass * 0.1f)
		{
			conduitGO.Trigger(-1152799878, null);
		}
	}

	private ConduitType conduitType;

	private float MaxMass = 10f;

	private const float PERCENT_MAX_MASS_FOR_STATE_CHANGE_DAMAGE = 0.1f;

	public const float TickRate = 1f;

	public const float WaitTime = 1f;

	private float elapsedTime;

	private float lastUpdateTime = float.NegativeInfinity;

	public ConduitFlow.SOAInfo soaInfo = new ConduitFlow.SOAInfo();

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

	private HashSet<int> visited = new HashSet<int>();

	private HashSet<int> replacements = new HashSet<int>();

	private List<ConduitFlow.Conduit> path = new List<ConduitFlow.Conduit>();

	private List<List<ConduitFlow.Conduit>> pathList = new List<List<ConduitFlow.Conduit>>();

	public static readonly ConduitFlow.ConduitContents emptyContents = new ConduitFlow.ConduitContents
	{
		element = SimHashes.Vacuum,
		mass = 0f,
		temperature = 0f,
		diseaseIdx = byte.MaxValue,
		diseaseCount = 0
	};

	public class SOAInfo
	{
		public int NumEntries
		{
			get
			{
				return this.conduits.Count;
			}
		}

		public int AddConduit(ConduitFlow manager, GameObject conduit_go, int cell)
		{
			int count = this.conduitConnections.Count;
			ConduitFlow.Conduit conduit = new ConduitFlow.Conduit(count);
			this.conduits.Add(conduit);
			this.conduitConnections.Add(new ConduitFlow.ConduitConnections
			{
				left = -1,
				right = -1,
				up = -1,
				down = -1
			});
			ConduitFlow.ConduitContents contents = manager.grid[cell].contents;
			this.initialContents.Add(contents);
			this.lastFlowInfo.Add(new ConduitFlow.ConduitFlowInfo
			{
				direction = ConduitFlow.FlowDirection.None,
				contents = ConduitFlow.ConduitContents.EmptyContents()
			});
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(conduit_go);
			HandleVector<int>.Handle handle2 = Game.Instance.conduitTemperatureManager.Allocate(manager.conduitType, count, handle, ref contents);
			HandleVector<int>.Handle handle3 = Game.Instance.conduitDiseaseManager.Allocate(handle2, ref contents);
			this.cells.Add(cell);
			this.updated.Add(false);
			this.diseaseContentsVisible.Add(false);
			this.structureTemperatureHandles.Add(handle);
			this.temperatureHandles.Add(handle2);
			this.diseaseHandles.Add(handle3);
			this.conduitGOs.Add(conduit_go);
			this.srcFlowIdx.Add(-1);
			this.permittedFlowDirections.Add(0);
			this.srcFlowDirections.Add(ConduitFlow.FlowDirection.None);
			this.targetFlowDirections.Add(ConduitFlow.FlowDirection.None);
			return count;
		}

		public void Clear(ConduitFlow manager)
		{
			for (int i = 0; i < this.conduits.Count; i++)
			{
				this.ForcePermanentDiseaseContainer(i, false);
				int num = this.cells[i];
				ConduitFlow.ConduitContents contents = manager.grid[num].contents;
				HandleVector<int>.Handle handle = this.temperatureHandles[i];
				if (handle.IsValid())
				{
					float temperature = Game.Instance.conduitTemperatureManager.GetTemperature(handle);
					contents.temperature = temperature;
					Game.Instance.conduitTemperatureManager.Free(handle);
				}
				HandleVector<int>.Handle handle2 = this.diseaseHandles[i];
				if (handle2.IsValid())
				{
					ConduitDiseaseManager.Data data = Game.Instance.conduitDiseaseManager.GetData(handle2);
					contents.diseaseIdx = data.diseaseIdx;
					contents.diseaseCount = data.diseaseCount;
					Game.Instance.conduitDiseaseManager.Free(handle2);
				}
				manager.grid[num].contents = contents;
				manager.grid[num].conduitIdx = -1;
			}
			this.cells.Clear();
			this.updated.Clear();
			this.diseaseContentsVisible.Clear();
			this.srcFlowIdx.Clear();
			this.permittedFlowDirections.Clear();
			this.srcFlowDirections.Clear();
			this.targetFlowDirections.Clear();
			this.conduitGOs.Clear();
			this.diseaseHandles.Clear();
			this.temperatureHandles.Clear();
			this.structureTemperatureHandles.Clear();
			this.initialContents.Clear();
			this.lastFlowInfo.Clear();
			this.conduitConnections.Clear();
			this.conduits.Clear();
		}

		public ConduitFlow.Conduit GetConduit(int idx)
		{
			return this.conduits[idx];
		}

		public ConduitFlow.ConduitConnections GetConduitConnections(int idx)
		{
			return this.conduitConnections[idx];
		}

		public void SetConduitConnections(int idx, ConduitFlow.ConduitConnections data)
		{
			this.conduitConnections[idx] = data;
		}

		public float GetConduitTemperature(int idx)
		{
			HandleVector<int>.Handle handle = this.temperatureHandles[idx];
			return Game.Instance.conduitTemperatureManager.GetTemperature(handle);
		}

		public void SetConduitTemperatureData(int idx, ref ConduitFlow.ConduitContents contents)
		{
			HandleVector<int>.Handle handle = this.temperatureHandles[idx];
			Game.Instance.conduitTemperatureManager.SetData(handle, ref contents);
		}

		public ConduitDiseaseManager.Data GetDiseaseData(int idx)
		{
			HandleVector<int>.Handle handle = this.diseaseHandles[idx];
			return Game.Instance.conduitDiseaseManager.GetData(handle);
		}

		public void SetDiseaseData(int idx, ref ConduitFlow.ConduitContents contents)
		{
			HandleVector<int>.Handle handle = this.diseaseHandles[idx];
			Game.Instance.conduitDiseaseManager.SetData(handle, ref contents);
		}

		public GameObject GetConduitGO(int idx)
		{
			return this.conduitGOs[idx];
		}

		public void ForcePermanentDiseaseContainer(int idx, bool force_on)
		{
			bool flag = this.diseaseContentsVisible[idx];
			if (flag != force_on)
			{
				this.diseaseContentsVisible[idx] = force_on;
				GameObject gameObject = this.conduitGOs[idx];
				if (gameObject == null)
				{
					return;
				}
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				component.ForcePermanentDiseaseContainer(force_on);
			}
		}

		public ConduitFlow.Conduit GetConduitFromDirection(int idx, ConduitFlow.FlowDirection direction)
		{
			ConduitFlow.Conduit conduit = ConduitFlow.Conduit.Invalid();
			ConduitFlow.ConduitConnections conduitConnections = this.conduitConnections[idx];
			switch (direction)
			{
			case ConduitFlow.FlowDirection.Left:
				conduit = ((conduitConnections.left == -1) ? ConduitFlow.Conduit.Invalid() : this.conduits[conduitConnections.left]);
				break;
			case ConduitFlow.FlowDirection.Right:
				conduit = ((conduitConnections.right == -1) ? ConduitFlow.Conduit.Invalid() : this.conduits[conduitConnections.right]);
				break;
			case ConduitFlow.FlowDirection.Up:
				conduit = ((conduitConnections.up == -1) ? ConduitFlow.Conduit.Invalid() : this.conduits[conduitConnections.up]);
				break;
			case ConduitFlow.FlowDirection.Down:
				conduit = ((conduitConnections.down == -1) ? ConduitFlow.Conduit.Invalid() : this.conduits[conduitConnections.down]);
				break;
			}
			return conduit;
		}

		public void BeginFrame(ConduitFlow manager)
		{
			for (int i = 0; i < this.conduits.Count; i++)
			{
				this.updated[i] = false;
				ConduitFlow.ConduitContents contents = this.conduits[i].GetContents(manager);
				this.initialContents[i] = contents;
				this.lastFlowInfo[i] = new ConduitFlow.ConduitFlowInfo
				{
					direction = ConduitFlow.FlowDirection.None,
					contents = ConduitFlow.ConduitContents.EmptyContents()
				};
				int num = this.cells[i];
				manager.grid[num].contents = contents;
			}
		}

		public void EndFrame(ConduitFlow manager)
		{
			for (int i = 0; i < this.conduits.Count; i++)
			{
				int num = this.cells[i];
				ConduitFlow.ConduitContents contents = manager.grid[num].contents;
				HandleVector<int>.Handle handle = this.temperatureHandles[i];
				HandleVector<int>.Handle handle2 = this.diseaseHandles[i];
				Game.Instance.conduitTemperatureManager.SetData(handle, ref contents);
				Game.Instance.conduitDiseaseManager.SetData(handle2, ref contents);
			}
		}

		public void UpdateFlowDirection(ConduitFlow manager)
		{
			for (int i = 0; i < this.conduits.Count; i++)
			{
				ConduitFlow.Conduit conduit = this.conduits[i];
				if (!this.updated[i])
				{
					int cell = conduit.GetCell(manager);
					ConduitFlow.ConduitContents contents = manager.grid[cell].contents;
					if (contents.element == SimHashes.Vacuum)
					{
						this.srcFlowDirections[conduit.idx] = conduit.GetNextFlowSource(manager);
					}
				}
			}
		}

		public void MarkConduitEmpty(int idx, ConduitFlow manager)
		{
			if (this.lastFlowInfo[idx].direction != ConduitFlow.FlowDirection.None)
			{
				this.lastFlowInfo[idx] = new ConduitFlow.ConduitFlowInfo
				{
					direction = ConduitFlow.FlowDirection.None,
					contents = ConduitFlow.ConduitContents.EmptyContents()
				};
				ConduitFlow.Conduit conduit = this.conduits[idx];
				this.targetFlowDirections[idx] = conduit.GetNextFlowTarget(manager);
				int num = this.cells[idx];
				manager.grid[num].contents = ConduitFlow.ConduitContents.EmptyContents();
			}
		}

		public void ResetLastFlowInfo(int idx)
		{
			this.lastFlowInfo[idx] = new ConduitFlow.ConduitFlowInfo
			{
				direction = ConduitFlow.FlowDirection.None,
				contents = ConduitFlow.ConduitContents.EmptyContents()
			};
		}

		public void SetLastFlowInfo(int idx, ConduitFlow.FlowDirection direction, ref ConduitFlow.ConduitContents contents)
		{
			this.lastFlowInfo[idx] = new ConduitFlow.ConduitFlowInfo
			{
				direction = direction,
				contents = contents
			};
		}

		public ConduitFlow.ConduitContents GetInitialContents(int idx)
		{
			return this.initialContents[idx];
		}

		public ConduitFlow.ConduitFlowInfo GetLastFlowInfo(int idx)
		{
			return this.lastFlowInfo[idx];
		}

		public int GetPermittedFlowDirections(int idx)
		{
			return this.permittedFlowDirections[idx];
		}

		public void SetPermittedFlowDirections(int idx, int permitted)
		{
			this.permittedFlowDirections[idx] = permitted;
		}

		public ConduitFlow.FlowDirection GetTargetFlowDirection(int idx)
		{
			return this.targetFlowDirections[idx];
		}

		public void SetTargetFlowDirection(int idx, ConduitFlow.FlowDirection directions)
		{
			this.targetFlowDirections[idx] = directions;
		}

		public int GetSrcFlowIdx(int idx)
		{
			return this.srcFlowIdx[idx];
		}

		public void SetSrcFlowIdx(int idx, int new_src_idx)
		{
			this.srcFlowIdx[idx] = new_src_idx;
		}

		public ConduitFlow.FlowDirection GetSrcFlowDirection(int idx)
		{
			return this.srcFlowDirections[idx];
		}

		public void SetSrcFlowDirection(int idx, ConduitFlow.FlowDirection directions)
		{
			this.srcFlowDirections[idx] = directions;
		}

		public int GetCell(int idx)
		{
			return this.cells[idx];
		}

		public void SetCell(int idx, int cell)
		{
			this.cells[idx] = cell;
		}

		public bool GetUpdated(int idx)
		{
			return this.updated[idx];
		}

		public void SetUpdated(int idx, bool is_updated)
		{
			this.updated[idx] = is_updated;
		}

		private List<ConduitFlow.Conduit> conduits = new List<ConduitFlow.Conduit>();

		private List<ConduitFlow.ConduitConnections> conduitConnections = new List<ConduitFlow.ConduitConnections>();

		private List<ConduitFlow.ConduitFlowInfo> lastFlowInfo = new List<ConduitFlow.ConduitFlowInfo>();

		private List<ConduitFlow.ConduitContents> initialContents = new List<ConduitFlow.ConduitContents>();

		private List<HandleVector<int>.Handle> structureTemperatureHandles = new List<HandleVector<int>.Handle>();

		private List<HandleVector<int>.Handle> temperatureHandles = new List<HandleVector<int>.Handle>();

		private List<HandleVector<int>.Handle> diseaseHandles = new List<HandleVector<int>.Handle>();

		private List<GameObject> conduitGOs = new List<GameObject>();

		private List<bool> diseaseContentsVisible = new List<bool>();

		private List<bool> updated = new List<bool>();

		private List<int> cells = new List<int>();

		private List<int> permittedFlowDirections = new List<int>();

		private List<int> srcFlowIdx = new List<int>();

		private List<ConduitFlow.FlowDirection> srcFlowDirections = new List<ConduitFlow.FlowDirection>();

		private List<ConduitFlow.FlowDirection> targetFlowDirections = new List<ConduitFlow.FlowDirection>();
	}

	[DebuggerDisplay("{priority} {callback.Target.name} {callback.Target} {callback.Method}")]
	public struct ConduitUpdater
	{
		public ConduitFlowPriority priority;

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

	public struct ConduitConnections
	{
		public int left;

		public int right;

		public int up;

		public int down;
	}

	public struct ConduitFlowInfo
	{
		public ConduitFlow.FlowDirection direction;

		public ConduitFlow.ConduitContents contents;
	}

	[Serializable]
	public struct Conduit : IEquatable<ConduitFlow.Conduit>
	{
		public Conduit(int idx)
		{
			this.idx = idx;
		}

		public static ConduitFlow.Conduit Invalid()
		{
			return new ConduitFlow.Conduit(-1);
		}

		public int GetPermittedFlowDirections(ConduitFlow manager)
		{
			return manager.soaInfo.GetPermittedFlowDirections(this.idx);
		}

		public void SetPermittedFlowDirections(int permitted, ConduitFlow manager)
		{
			manager.soaInfo.SetPermittedFlowDirections(this.idx, permitted);
		}

		public ConduitFlow.FlowDirection GetTargetFlowDirection(ConduitFlow manager)
		{
			return manager.soaInfo.GetTargetFlowDirection(this.idx);
		}

		public void SetTargetFlowDirection(ConduitFlow.FlowDirection directions, ConduitFlow manager)
		{
			manager.soaInfo.SetTargetFlowDirection(this.idx, directions);
		}

		public ConduitFlow.ConduitContents GetContents(ConduitFlow manager)
		{
			int cell = manager.soaInfo.GetCell(this.idx);
			ConduitFlow.ConduitContents contents = manager.grid[cell].contents;
			ConduitFlow.SOAInfo soaInfo = manager.soaInfo;
			contents.temperature = soaInfo.GetConduitTemperature(this.idx);
			ConduitDiseaseManager.Data diseaseData = soaInfo.GetDiseaseData(this.idx);
			contents.diseaseIdx = diseaseData.diseaseIdx;
			contents.diseaseCount = diseaseData.diseaseCount;
			return contents;
		}

		public void SetContents(ConduitFlow manager, ConduitFlow.ConduitContents contents)
		{
			int cell = manager.soaInfo.GetCell(this.idx);
			manager.grid[cell].contents = contents;
			ConduitFlow.SOAInfo soaInfo = manager.soaInfo;
			soaInfo.SetConduitTemperatureData(this.idx, ref contents);
			soaInfo.ForcePermanentDiseaseContainer(this.idx, contents.diseaseIdx != byte.MaxValue);
			soaInfo.SetDiseaseData(this.idx, ref contents);
		}

		public ConduitFlow.FlowDirection GetNextFlowSource(ConduitFlow manager)
		{
			int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(this.idx);
			if (permittedFlowDirections == -1)
			{
				return ConduitFlow.FlowDirection.Blocked;
			}
			ConduitFlow.FlowDirection flowDirection = manager.soaInfo.GetSrcFlowDirection(this.idx);
			if (flowDirection == ConduitFlow.FlowDirection.None)
			{
				flowDirection = ConduitFlow.FlowDirection.Down;
			}
			for (int i = 0; i < 5; i++)
			{
				int num = flowDirection + i - ConduitFlow.FlowDirection.Left;
				int num2 = (num + 1) % 5;
				ConduitFlow.FlowDirection flowDirection2 = num2 + ConduitFlow.FlowDirection.Left;
				ConduitFlow.Conduit conduitFromDirection = manager.soaInfo.GetConduitFromDirection(this.idx, flowDirection2);
				if (conduitFromDirection.idx != -1)
				{
					ConduitFlow.ConduitContents contents = manager.grid[conduitFromDirection.GetCell(manager)].contents;
					if (contents.element != SimHashes.Vacuum)
					{
						int permittedFlowDirections2 = manager.soaInfo.GetPermittedFlowDirections(conduitFromDirection.idx);
						if (permittedFlowDirections2 != -1)
						{
							ConduitFlow.FlowDirection flowDirection3 = ConduitFlow.InverseFlow(flowDirection2);
							if (manager.soaInfo.GetConduitFromDirection(conduitFromDirection.idx, flowDirection3).idx != -1 && (permittedFlowDirections2 & ConduitFlow.FlowBit(flowDirection3)) != 0)
							{
								return flowDirection2;
							}
						}
					}
				}
			}
			for (int j = 0; j < 5; j++)
			{
				ConduitFlow.FlowDirection targetFlowDirection = manager.soaInfo.GetTargetFlowDirection(this.idx);
				int num3 = targetFlowDirection + j - ConduitFlow.FlowDirection.Left;
				int num4 = (num3 + 1) % 5;
				ConduitFlow.FlowDirection flowDirection4 = num4 + ConduitFlow.FlowDirection.Left;
				ConduitFlow.FlowDirection flowDirection5 = ConduitFlow.InverseFlow(flowDirection4);
				ConduitFlow.Conduit conduitFromDirection2 = manager.soaInfo.GetConduitFromDirection(this.idx, flowDirection4);
				if (conduitFromDirection2.idx != -1)
				{
					int permittedFlowDirections3 = manager.soaInfo.GetPermittedFlowDirections(conduitFromDirection2.idx);
					if (permittedFlowDirections3 != -1)
					{
						if ((permittedFlowDirections3 & ConduitFlow.FlowBit(flowDirection5)) != 0)
						{
							return flowDirection4;
						}
					}
				}
			}
			return ConduitFlow.FlowDirection.None;
		}

		public ConduitFlow.FlowDirection GetNextFlowTarget(ConduitFlow manager)
		{
			int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(this.idx);
			if (permittedFlowDirections == -1)
			{
				return ConduitFlow.FlowDirection.Blocked;
			}
			for (int i = 0; i < 5; i++)
			{
				ConduitFlow.FlowDirection targetFlowDirection = manager.soaInfo.GetTargetFlowDirection(this.idx);
				int num = targetFlowDirection + i - ConduitFlow.FlowDirection.Left;
				int num2 = (num + 1) % 5;
				int num3 = num2 + 1;
				if (manager.soaInfo.GetConduitFromDirection(this.idx, (ConduitFlow.FlowDirection)num3).idx != -1 && (permittedFlowDirections & ConduitFlow.FlowBit((ConduitFlow.FlowDirection)num3)) != 0)
				{
					return (ConduitFlow.FlowDirection)num3;
				}
			}
			return ConduitFlow.FlowDirection.Blocked;
		}

		public ConduitFlow.ConduitFlowInfo GetLastFlowInfo(ConduitFlow manager)
		{
			return manager.soaInfo.GetLastFlowInfo(this.idx);
		}

		public ConduitFlow.ConduitContents GetInitialContents(ConduitFlow manager)
		{
			return manager.soaInfo.GetInitialContents(this.idx);
		}

		public int GetCell(ConduitFlow manager)
		{
			return manager.soaInfo.GetCell(this.idx);
		}

		public bool Equals(ConduitFlow.Conduit other)
		{
			return this.idx == other.idx;
		}

		public int idx;
	}

	[DebuggerDisplay("{element} M:{mass} T:{temperature}")]
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
}
