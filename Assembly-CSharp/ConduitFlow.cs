using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{conduitType}")]
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

	private static ConduitFlow.FlowDirections ComputeFlowDirection(int index)
	{
		global::Debug.Assert(0 <= index && index < 4);
		return (ConduitFlow.FlowDirections)(1 << index);
	}

	private static int ComputeIndex(ConduitFlow.FlowDirections flow)
	{
		switch (flow)
		{
		case ConduitFlow.FlowDirections.Down:
			return 0;
		case ConduitFlow.FlowDirections.Left:
			return 1;
		case ConduitFlow.FlowDirections.Right:
			return 2;
		case ConduitFlow.FlowDirections.Up:
			return 3;
		}
		global::Debug.Assert(false, "multiple bits are set in 'flow'...can't compute refuted index");
		return -1;
	}

	private static ConduitFlow.FlowDirections ComputeNextFlowDirection(ConduitFlow.FlowDirections current)
	{
		return (current != ConduitFlow.FlowDirections.None) ? ConduitFlow.ComputeFlowDirection((ConduitFlow.ComputeIndex(current) + 1) % 4) : ConduitFlow.FlowDirections.Down;
	}

	public static ConduitFlow.FlowDirections Invert(ConduitFlow.FlowDirections directions)
	{
		return (ConduitFlow.FlowDirections)(15 & (byte)(~(byte)directions));
	}

	public static ConduitFlow.FlowDirections Opposite(ConduitFlow.FlowDirections directions)
	{
		ConduitFlow.FlowDirections flowDirections = ConduitFlow.FlowDirections.None;
		if ((byte)(directions & ConduitFlow.FlowDirections.Left) != 0)
		{
			flowDirections = ConduitFlow.FlowDirections.Right;
		}
		else if ((byte)(directions & ConduitFlow.FlowDirections.Right) != 0)
		{
			flowDirections = ConduitFlow.FlowDirections.Left;
		}
		else if ((byte)(directions & ConduitFlow.FlowDirections.Up) != 0)
		{
			flowDirections = ConduitFlow.FlowDirections.Down;
		}
		else if ((byte)(directions & ConduitFlow.FlowDirections.Down) != 0)
		{
			flowDirections = ConduitFlow.FlowDirections.Up;
		}
		global::Debug.Assert((ConduitFlow.Invert(flowDirections) & directions) == directions, "computing the Opposite of multiple directions is refutable");
		return flowDirections;
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
		int num = this.networks.Count - networks.Count;
		if (0 < this.networks.Count - networks.Count)
		{
			this.networks.RemoveRange(networks.Count, num);
		}
		global::Debug.Assert(this.networks.Count <= networks.Count);
		for (int num2 = 0; num2 != networks.Count; num2++)
		{
			if (num2 < this.networks.Count)
			{
				this.networks[num2] = new ConduitFlow.Network
				{
					network = (FlowUtilityNetwork)networks[num2],
					cells = this.networks[num2].cells
				};
				this.networks[num2].cells.Clear();
			}
			else
			{
				this.networks.Add(new ConduitFlow.Network
				{
					network = (FlowUtilityNetwork)networks[num2],
					cells = new List<int>()
				});
			}
		}
		this.build_network_job.Reset(this);
		foreach (ConduitFlow.Network network in this.networks)
		{
			this.build_network_job.Add(new ConduitFlow.BuildNetworkTask(network, this.soaInfo.NumEntries));
		}
		GlobalJobManager.Run(this.build_network_job);
		for (int num3 = 0; num3 != this.build_network_job.Count; num3++)
		{
			this.build_network_job.GetWorkItem(num3).Finish();
		}
	}

	private void RebuildConnections(IEnumerable<int> root_nodes)
	{
		ConduitFlow.ConnectContext connectContext = new ConduitFlow.ConnectContext(this);
		this.soaInfo.Clear(this);
		this.replacements.ExceptWith(root_nodes);
		ObjectLayer objectLayer = ((this.conduitType != ConduitType.Gas) ? ObjectLayer.LiquidConduit : ObjectLayer.GasConduit);
		foreach (int num in root_nodes)
		{
			GameObject gameObject = Grid.Objects[num, (int)objectLayer];
			if (!(gameObject == null))
			{
				global::Conduit component = gameObject.GetComponent<global::Conduit>();
				if (!(component != null) || !component.IsDisconnected())
				{
					int num2 = this.soaInfo.AddConduit(this, gameObject, num);
					this.grid[num].conduitIdx = num2;
					connectContext.cells.Add(num);
				}
			}
		}
		Game.Instance.conduitTemperatureManager.Sim200ms(0f);
		this.connect_job.Reset(connectContext);
		int num3 = 256;
		for (int i = 0; i < connectContext.cells.Count; i += num3)
		{
			this.connect_job.Add(new ConduitFlow.ConnectTask(i, Mathf.Min(i + num3, connectContext.cells.Count)));
		}
		GlobalJobManager.Run(this.connect_job);
		connectContext.Finish();
		if (this.onConduitsRebuilt != null)
		{
			this.onConduitsRebuilt();
		}
	}

	private ConduitFlow.FlowDirections GetDirection(ConduitFlow.Conduit conduit, ConduitFlow.Conduit target_conduit)
	{
		global::Debug.Assert(conduit.idx != -1);
		global::Debug.Assert(target_conduit.idx != -1);
		ConduitFlow.ConduitConnections conduitConnections = this.soaInfo.GetConduitConnections(conduit.idx);
		if (conduitConnections.up == target_conduit.idx)
		{
			return ConduitFlow.FlowDirections.Up;
		}
		if (conduitConnections.down == target_conduit.idx)
		{
			return ConduitFlow.FlowDirections.Down;
		}
		if (conduitConnections.left == target_conduit.idx)
		{
			return ConduitFlow.FlowDirections.Left;
		}
		if (conduitConnections.right == target_conduit.idx)
		{
			return ConduitFlow.FlowDirections.Right;
		}
		return ConduitFlow.FlowDirections.None;
	}

	public int ComputeUpdateOrder(int cell)
	{
		foreach (ConduitFlow.Network network in this.networks)
		{
			int num = network.cells.IndexOf(cell);
			if (num != -1)
			{
				return num;
			}
		}
		return -1;
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
			global::Debug.LogError("unexpected temperature");
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

	public static int GetCellFromDirection(int cell, ConduitFlow.FlowDirections direction)
	{
		switch (direction)
		{
		case ConduitFlow.FlowDirections.Down:
			return Grid.CellBelow(cell);
		case ConduitFlow.FlowDirections.Left:
			return Grid.CellLeft(cell);
		case ConduitFlow.FlowDirections.Right:
			return Grid.CellRight(cell);
		case ConduitFlow.FlowDirections.Up:
			return Grid.CellAbove(cell);
		}
		return -1;
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
		this.elapsedTime -= 1f;
		float num = 1f;
		this.lastUpdateTime = Time.time;
		this.soaInfo.BeginFrame(this);
		ListPool<ConduitFlow.UpdateNetworkTask, ConduitFlow>.PooledList pooledList = ListPool<ConduitFlow.UpdateNetworkTask, ConduitFlow>.Allocate();
		pooledList.Capacity = Mathf.Max(pooledList.Capacity, this.networks.Count);
		foreach (ConduitFlow.Network network in this.networks)
		{
			pooledList.Add(new ConduitFlow.UpdateNetworkTask(network));
		}
		for (int num2 = 0; num2 != 4; num2++)
		{
			if (pooledList.Count == 0)
			{
				break;
			}
			this.update_networks_job.Reset(this);
			foreach (ConduitFlow.UpdateNetworkTask updateNetworkTask in pooledList)
			{
				this.update_networks_job.Add(updateNetworkTask);
			}
			GlobalJobManager.Run(this.update_networks_job);
			pooledList.Clear();
			for (int num3 = 0; num3 != this.update_networks_job.Count; num3++)
			{
				ConduitFlow.UpdateNetworkTask workItem = this.update_networks_job.GetWorkItem(num3);
				if (workItem.continue_updating && num2 != 3)
				{
					pooledList.Add(workItem);
				}
				else
				{
					workItem.Finish(this);
				}
			}
		}
		pooledList.Recycle();
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

	private float ComputeMovableMass(ConduitFlow.GridNode grid_node, Dictionary<int, ConduitFlow.Sink> sinks)
	{
		ConduitFlow.ConduitContents contents = grid_node.contents;
		if (contents.element == SimHashes.Vacuum)
		{
			return 0f;
		}
		ConduitFlow.Sink sink;
		return (!sinks.TryGetValue(grid_node.conduitIdx, out sink) || !(sink.consumer != null)) ? contents.movable_mass : Mathf.Max(0f, contents.movable_mass - sink.space_remaining);
	}

	private bool UpdateConduit(ConduitFlow.Conduit conduit, Dictionary<int, ConduitFlow.Sink> sinks)
	{
		bool flag = false;
		int cell = this.soaInfo.GetCell(conduit.idx);
		ConduitFlow.GridNode gridNode = this.grid[cell];
		float num = this.ComputeMovableMass(gridNode, sinks);
		ConduitFlow.FlowDirections permittedFlowDirections = this.soaInfo.GetPermittedFlowDirections(conduit.idx);
		ConduitFlow.FlowDirections flowDirections = this.soaInfo.GetTargetFlowDirection(conduit.idx);
		if (num <= 0f)
		{
			for (int num2 = 0; num2 != 4; num2++)
			{
				flowDirections = ConduitFlow.ComputeNextFlowDirection(flowDirections);
				if ((byte)(permittedFlowDirections & flowDirections) != 0)
				{
					ConduitFlow.Conduit conduitFromDirection = this.soaInfo.GetConduitFromDirection(conduit.idx, flowDirections);
					global::Debug.Assert(conduitFromDirection.idx != -1);
					ConduitFlow.FlowDirections srcFlowDirection = this.soaInfo.GetSrcFlowDirection(conduitFromDirection.idx);
					bool flag2 = (byte)(srcFlowDirection & ConduitFlow.Opposite(flowDirections)) != 0;
					if (flag2)
					{
						this.soaInfo.SetPullDirection(conduitFromDirection.idx, flowDirections);
					}
				}
			}
		}
		else
		{
			for (int num3 = 0; num3 != 4; num3++)
			{
				flowDirections = ConduitFlow.ComputeNextFlowDirection(flowDirections);
				if ((byte)(permittedFlowDirections & flowDirections) != 0)
				{
					ConduitFlow.Conduit conduitFromDirection2 = this.soaInfo.GetConduitFromDirection(conduit.idx, flowDirections);
					global::Debug.Assert(conduitFromDirection2.idx != -1);
					ConduitFlow.FlowDirections srcFlowDirection2 = this.soaInfo.GetSrcFlowDirection(conduitFromDirection2.idx);
					bool flag3 = (byte)(srcFlowDirection2 & ConduitFlow.Opposite(flowDirections)) != 0;
					if (srcFlowDirection2 != ConduitFlow.FlowDirections.None && !flag3)
					{
						flag = true;
					}
					else
					{
						int cell2 = this.soaInfo.GetCell(conduitFromDirection2.idx);
						global::Debug.Assert(cell2 != -1);
						ConduitFlow.ConduitContents contents = this.grid[cell2].contents;
						bool flag4 = contents.element == SimHashes.Vacuum || contents.element == gridNode.contents.element;
						float effectiveCapacity = contents.GetEffectiveCapacity(this.MaxMass);
						bool flag5 = flag4 && effectiveCapacity > 0f;
						float num4 = Mathf.Min(num, effectiveCapacity);
						if (flag3 && flag5)
						{
							this.soaInfo.SetPullDirection(conduitFromDirection2.idx, flowDirections);
						}
						if (num4 > 0f)
						{
							if (flag5)
							{
								this.soaInfo.SetTargetFlowDirection(conduit.idx, flowDirections);
								global::Debug.Assert(gridNode.contents.temperature > 0f);
								contents.temperature = GameUtil.GetFinalTemperature(gridNode.contents.temperature, num4, contents.temperature, contents.mass);
								contents.AddMass(num4);
								contents.element = gridNode.contents.element;
								float num5 = num4 / gridNode.contents.mass;
								int num6 = (int)(num5 * (float)gridNode.contents.diseaseCount);
								if (num6 != 0)
								{
									SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(gridNode.contents.diseaseIdx, num6, contents.diseaseIdx, contents.diseaseCount);
									contents.diseaseIdx = diseaseInfo.idx;
									contents.diseaseCount = diseaseInfo.count;
								}
								this.grid[cell2].contents = contents;
								global::Debug.Assert(num4 <= gridNode.contents.mass);
								float num7 = gridNode.contents.mass - num4;
								num -= num4;
								if (num7 <= 0f)
								{
									global::Debug.Assert(num <= 0f);
									this.soaInfo.SetLastFlowInfo(conduit.idx, flowDirections, ref gridNode.contents);
									gridNode.contents = ConduitFlow.ConduitContents.Empty;
								}
								else
								{
									float num8 = num7 / gridNode.contents.mass;
									int num9 = (int)(num8 * (float)gridNode.contents.diseaseCount);
									global::Debug.Assert(num9 >= 0);
									ConduitFlow.ConduitContents contents2 = gridNode.contents;
									contents2.RemoveMass(num7);
									contents2.diseaseCount -= num9;
									gridNode.contents.RemoveMass(num4);
									gridNode.contents.diseaseCount = num9;
									if (num9 == 0)
									{
										gridNode.contents.diseaseIdx = byte.MaxValue;
									}
									this.soaInfo.SetLastFlowInfo(conduit.idx, flowDirections, ref contents2);
								}
								this.grid[cell].contents = gridNode.contents;
								flag = 0f < this.ComputeMovableMass(gridNode, sinks);
								break;
							}
						}
					}
				}
			}
		}
		ConduitFlow.FlowDirections srcFlowDirection3 = this.soaInfo.GetSrcFlowDirection(conduit.idx);
		ConduitFlow.FlowDirections pullDirection = this.soaInfo.GetPullDirection(conduit.idx);
		if (srcFlowDirection3 == ConduitFlow.FlowDirections.None || (byte)(ConduitFlow.Opposite(srcFlowDirection3) & pullDirection) != 0)
		{
			this.soaInfo.SetPullDirection(conduit.idx, ConduitFlow.FlowDirections.None);
			this.soaInfo.SetSrcFlowDirection(conduit.idx, ConduitFlow.FlowDirections.None);
			for (int num10 = 0; num10 != 2; num10++)
			{
				ConduitFlow.FlowDirections flowDirections2 = srcFlowDirection3;
				for (int num11 = 0; num11 != 4; num11++)
				{
					flowDirections2 = ConduitFlow.ComputeNextFlowDirection(flowDirections2);
					ConduitFlow.Conduit conduitFromDirection3 = this.soaInfo.GetConduitFromDirection(conduit.idx, flowDirections2);
					if (conduitFromDirection3.idx != -1)
					{
						ConduitFlow.FlowDirections permittedFlowDirections2 = this.soaInfo.GetPermittedFlowDirections(conduitFromDirection3.idx);
						if ((byte)(permittedFlowDirections2 & ConduitFlow.Opposite(flowDirections2)) != 0)
						{
							int cell3 = this.soaInfo.GetCell(conduitFromDirection3.idx);
							ConduitFlow.ConduitContents contents3 = this.grid[cell3].contents;
							float num12 = ((num10 != 0) ? contents3.mass : contents3.movable_mass);
							if (0f < num12)
							{
								this.soaInfo.SetSrcFlowDirection(conduit.idx, flowDirections2);
								break;
							}
						}
					}
				}
				if (this.soaInfo.GetSrcFlowDirection(conduit.idx) != ConduitFlow.FlowDirections.None)
				{
					break;
				}
			}
		}
		return flag;
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
		contents.AddMass(num);
		contents.element = element;
		contents.ConsolidateMass();
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
		return (conduit.idx == -1) ? ConduitFlow.ConduitContents.Empty : this.RemoveElement(conduit, delta);
	}

	public ConduitFlow.ConduitContents RemoveElement(ConduitFlow.Conduit conduit, float delta)
	{
		ConduitFlow.ConduitContents contents = conduit.GetContents(this);
		float num = Mathf.Min(contents.mass, delta);
		float num2 = contents.mass - num;
		if (num2 <= 0f)
		{
			conduit.SetContents(this, ConduitFlow.ConduitContents.Empty);
			return contents;
		}
		ConduitFlow.ConduitContents conduitContents = contents;
		conduitContents.RemoveMass(num2);
		float num3 = num2 / contents.mass;
		int num4 = (int)(num3 * (float)contents.diseaseCount);
		conduitContents.diseaseCount = contents.diseaseCount - num4;
		ConduitFlow.ConduitContents conduitContents2 = contents;
		conduitContents2.RemoveMass(num);
		conduitContents2.diseaseCount = num4;
		if (num4 <= 0)
		{
			conduitContents2.diseaseIdx = byte.MaxValue;
			conduitContents2.diseaseCount = 0;
		}
		conduit.SetContents(this, conduitContents2);
		return conduitContents;
	}

	public ConduitFlow.FlowDirections GetPermittedFlow(int cell)
	{
		ConduitFlow.Conduit conduit = this.GetConduit(cell);
		if (conduit.idx == -1)
		{
			return ConduitFlow.FlowDirections.None;
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
			this.SetContents(cell, ConduitFlow.ConduitContents.Empty);
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
		this.SetContents(cell, ConduitFlow.ConduitContents.Empty);
	}

	[Conditional("CHECK_NAN")]
	private void Validate(ConduitFlow.ConduitContents contents)
	{
		if (contents.mass > 0f && contents.temperature <= 0f)
		{
			global::Debug.LogError("zero degree pipe contents");
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
		for (int j = 0; j < this.versionedSerializedContents.Length; j++)
		{
			int num = this.serializedIdx[j];
			ConduitFlow.SerializedContents serializedContents = this.versionedSerializedContents[j];
			ConduitFlow.ConduitContents conduitContents = ((serializedContents.mass > 0f) ? new ConduitFlow.ConduitContents(serializedContents.element, Math.Min(this.MaxMass, serializedContents.mass), serializedContents.temperature, byte.MaxValue, 0) : ConduitFlow.ConduitContents.Empty);
			if (0 < serializedContents.diseaseCount || serializedContents.diseaseHash != 0)
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

	private HashSet<int> replacements = new HashSet<int>();

	private const int FLOW_DIRECTION_COUNT = 4;

	private List<ConduitFlow.Network> networks = new List<ConduitFlow.Network>();

	private WorkItemCollection<ConduitFlow.BuildNetworkTask, ConduitFlow> build_network_job = new WorkItemCollection<ConduitFlow.BuildNetworkTask, ConduitFlow>();

	private WorkItemCollection<ConduitFlow.ConnectTask, ConduitFlow.ConnectContext> connect_job = new WorkItemCollection<ConduitFlow.ConnectTask, ConduitFlow.ConnectContext>();

	private WorkItemCollection<ConduitFlow.UpdateNetworkTask, ConduitFlow> update_networks_job = new WorkItemCollection<ConduitFlow.UpdateNetworkTask, ConduitFlow>();

	[DebuggerDisplay("{NumEntries}")]
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
			this.lastFlowInfo.Add(ConduitFlow.ConduitFlowInfo.DEFAULT);
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(conduit_go);
			HandleVector<int>.Handle handle2 = Game.Instance.conduitTemperatureManager.Allocate(manager.conduitType, count, handle, ref contents);
			HandleVector<int>.Handle handle3 = Game.Instance.conduitDiseaseManager.Allocate(handle2, ref contents);
			this.cells.Add(cell);
			this.diseaseContentsVisible.Add(false);
			this.structureTemperatureHandles.Add(handle);
			this.temperatureHandles.Add(handle2);
			this.diseaseHandles.Add(handle3);
			this.conduitGOs.Add(conduit_go);
			this.permittedFlowDirections.Add(ConduitFlow.FlowDirections.None);
			this.srcFlowDirections.Add(ConduitFlow.FlowDirections.None);
			this.pullDirections.Add(ConduitFlow.FlowDirections.None);
			this.targetFlowDirections.Add(ConduitFlow.FlowDirections.None);
			return count;
		}

		public void Clear(ConduitFlow manager)
		{
			if (this.clearJob.Count == 0)
			{
				this.clearJob.Reset(this);
				this.clearJob.Add<ConduitFlow.SOAInfo.PublishTemperatureToSim>(this.publishTemperatureToSim);
				this.clearJob.Add<ConduitFlow.SOAInfo.PublishDiseaseToSim>(this.publishDiseaseToSim);
				this.clearJob.Add<ConduitFlow.SOAInfo.ResetConduit>(this.resetConduit);
			}
			this.clearPermanentDiseaseContainer.Initialize(this.conduits.Count, manager);
			this.publishTemperatureToSim.Initialize(this.conduits.Count, manager);
			this.publishDiseaseToSim.Initialize(this.conduits.Count, manager);
			this.resetConduit.Initialize(this.conduits.Count, manager);
			this.clearPermanentDiseaseContainer.Run(this);
			GlobalJobManager.Run(this.clearJob);
			for (int num = 0; num != this.conduits.Count; num++)
			{
				Game.Instance.conduitDiseaseManager.Free(this.diseaseHandles[num]);
			}
			for (int num2 = 0; num2 != this.conduits.Count; num2++)
			{
				Game.Instance.conduitTemperatureManager.Free(this.temperatureHandles[num2]);
			}
			this.cells.Clear();
			this.diseaseContentsVisible.Clear();
			this.permittedFlowDirections.Clear();
			this.srcFlowDirections.Clear();
			this.pullDirections.Clear();
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
			float temperature = Game.Instance.conduitTemperatureManager.GetTemperature(handle);
			global::Debug.Assert(!float.IsNaN(temperature));
			return temperature;
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

		public ConduitFlow.Conduit GetConduitFromDirection(int idx, ConduitFlow.FlowDirections direction)
		{
			ConduitFlow.Conduit conduit = ConduitFlow.Conduit.Invalid();
			ConduitFlow.ConduitConnections conduitConnections = this.conduitConnections[idx];
			switch (direction)
			{
			case ConduitFlow.FlowDirections.Down:
				conduit = ((conduitConnections.down == -1) ? ConduitFlow.Conduit.Invalid() : this.conduits[conduitConnections.down]);
				break;
			case ConduitFlow.FlowDirections.Left:
				conduit = ((conduitConnections.left == -1) ? ConduitFlow.Conduit.Invalid() : this.conduits[conduitConnections.left]);
				break;
			case ConduitFlow.FlowDirections.Right:
				conduit = ((conduitConnections.right == -1) ? ConduitFlow.Conduit.Invalid() : this.conduits[conduitConnections.right]);
				break;
			case ConduitFlow.FlowDirections.Up:
				conduit = ((conduitConnections.up == -1) ? ConduitFlow.Conduit.Invalid() : this.conduits[conduitConnections.up]);
				break;
			}
			return conduit;
		}

		public void BeginFrame(ConduitFlow manager)
		{
			if (this.beginFrameJob.Count == 0)
			{
				this.beginFrameJob.Reset(this);
				this.beginFrameJob.Add<ConduitFlow.SOAInfo.InitializeContentsTask>(this.initializeContents);
				this.beginFrameJob.Add<ConduitFlow.SOAInfo.InvalidateLastFlow>(this.invalidateLastFlow);
			}
			this.initializeContents.Initialize(this.conduits.Count, manager);
			this.invalidateLastFlow.Initialize(this.conduits.Count, manager);
			GlobalJobManager.Run(this.beginFrameJob);
		}

		public void EndFrame(ConduitFlow manager)
		{
			if (this.endFrameJob.Count == 0)
			{
				this.endFrameJob.Reset(this);
				this.endFrameJob.Add<ConduitFlow.SOAInfo.PublishDiseaseToGame>(this.publishDiseaseToGame);
			}
			this.publishTemperatureToGame.Initialize(this.conduits.Count, manager);
			this.publishDiseaseToGame.Initialize(this.conduits.Count, manager);
			this.publishTemperatureToGame.Run(this);
			GlobalJobManager.Run(this.endFrameJob);
		}

		public void UpdateFlowDirection(ConduitFlow manager)
		{
			if (this.updateFlowDirectionJob.Count == 0)
			{
				this.updateFlowDirectionJob.Reset(this);
				this.updateFlowDirectionJob.Add<ConduitFlow.SOAInfo.FlowThroughVacuum>(this.flowThroughVacuum);
			}
			this.flowThroughVacuum.Initialize(this.conduits.Count, manager);
			GlobalJobManager.Run(this.updateFlowDirectionJob);
		}

		public void ResetLastFlowInfo(int idx)
		{
			this.lastFlowInfo[idx] = ConduitFlow.ConduitFlowInfo.DEFAULT;
		}

		public void SetLastFlowInfo(int idx, ConduitFlow.FlowDirections direction, ref ConduitFlow.ConduitContents contents)
		{
			if (this.lastFlowInfo[idx].direction == ConduitFlow.FlowDirections.None)
			{
				this.lastFlowInfo[idx] = new ConduitFlow.ConduitFlowInfo
				{
					direction = direction,
					contents = contents
				};
			}
		}

		public ConduitFlow.ConduitContents GetInitialContents(int idx)
		{
			return this.initialContents[idx];
		}

		public ConduitFlow.ConduitFlowInfo GetLastFlowInfo(int idx)
		{
			return this.lastFlowInfo[idx];
		}

		public ConduitFlow.FlowDirections GetPermittedFlowDirections(int idx)
		{
			return this.permittedFlowDirections[idx];
		}

		public void SetPermittedFlowDirections(int idx, ConduitFlow.FlowDirections permitted)
		{
			this.permittedFlowDirections[idx] = permitted;
		}

		public ConduitFlow.FlowDirections AddPermittedFlowDirections(int idx, ConduitFlow.FlowDirections delta)
		{
			List<ConduitFlow.FlowDirections> list;
			return (list = this.permittedFlowDirections)[idx] = list[idx] | delta;
		}

		public ConduitFlow.FlowDirections RemovePermittedFlowDirections(int idx, ConduitFlow.FlowDirections delta)
		{
			List<ConduitFlow.FlowDirections> list;
			return (list = this.permittedFlowDirections)[idx] = list[idx] & ~delta;
		}

		public ConduitFlow.FlowDirections GetTargetFlowDirection(int idx)
		{
			return this.targetFlowDirections[idx];
		}

		public void SetTargetFlowDirection(int idx, ConduitFlow.FlowDirections directions)
		{
			this.targetFlowDirections[idx] = directions;
		}

		public ConduitFlow.FlowDirections GetSrcFlowDirection(int idx)
		{
			return this.srcFlowDirections[idx];
		}

		public void SetSrcFlowDirection(int idx, ConduitFlow.FlowDirections directions)
		{
			this.srcFlowDirections[idx] = directions;
		}

		public ConduitFlow.FlowDirections GetPullDirection(int idx)
		{
			return this.pullDirections[idx];
		}

		public void SetPullDirection(int idx, ConduitFlow.FlowDirections directions)
		{
			this.pullDirections[idx] = directions;
		}

		public int GetCell(int idx)
		{
			return this.cells[idx];
		}

		public void SetCell(int idx, int cell)
		{
			this.cells[idx] = cell;
		}

		private List<ConduitFlow.Conduit> conduits = new List<ConduitFlow.Conduit>();

		private List<ConduitFlow.ConduitConnections> conduitConnections = new List<ConduitFlow.ConduitConnections>();

		private List<ConduitFlow.ConduitFlowInfo> lastFlowInfo = new List<ConduitFlow.ConduitFlowInfo>();

		private List<ConduitFlow.ConduitContents> initialContents = new List<ConduitFlow.ConduitContents>();

		private List<GameObject> conduitGOs = new List<GameObject>();

		private List<bool> diseaseContentsVisible = new List<bool>();

		private List<int> cells = new List<int>();

		private List<ConduitFlow.FlowDirections> permittedFlowDirections = new List<ConduitFlow.FlowDirections>();

		private List<ConduitFlow.FlowDirections> srcFlowDirections = new List<ConduitFlow.FlowDirections>();

		private List<ConduitFlow.FlowDirections> pullDirections = new List<ConduitFlow.FlowDirections>();

		private List<ConduitFlow.FlowDirections> targetFlowDirections = new List<ConduitFlow.FlowDirections>();

		private List<HandleVector<int>.Handle> structureTemperatureHandles = new List<HandleVector<int>.Handle>();

		private List<HandleVector<int>.Handle> temperatureHandles = new List<HandleVector<int>.Handle>();

		private List<HandleVector<int>.Handle> diseaseHandles = new List<HandleVector<int>.Handle>();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.ClearPermanentDiseaseContainer> clearPermanentDiseaseContainer = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.ClearPermanentDiseaseContainer>();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishTemperatureToSim> publishTemperatureToSim = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishTemperatureToSim>();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishDiseaseToSim> publishDiseaseToSim = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishDiseaseToSim>();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.ResetConduit> resetConduit = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.ResetConduit>();

		private ConduitFlow.SOAInfo.ConduitJob clearJob = new ConduitFlow.SOAInfo.ConduitJob();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.InitializeContentsTask> initializeContents = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.InitializeContentsTask>();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.InvalidateLastFlow> invalidateLastFlow = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.InvalidateLastFlow>();

		private ConduitFlow.SOAInfo.ConduitJob beginFrameJob = new ConduitFlow.SOAInfo.ConduitJob();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishTemperatureToGame> publishTemperatureToGame = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishTemperatureToGame>();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishDiseaseToGame> publishDiseaseToGame = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishDiseaseToGame>();

		private ConduitFlow.SOAInfo.ConduitJob endFrameJob = new ConduitFlow.SOAInfo.ConduitJob();

		private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.FlowThroughVacuum> flowThroughVacuum = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.FlowThroughVacuum>();

		private ConduitFlow.SOAInfo.ConduitJob updateFlowDirectionJob = new ConduitFlow.SOAInfo.ConduitJob();

		private abstract class ConduitTask : DivisibleTask<ConduitFlow.SOAInfo>
		{
			public ConduitTask(string name)
				: base(name)
			{
			}

			public ConduitFlow manager;
		}

		private class ConduitTaskDivision<Task> : TaskDivision<Task, ConduitFlow.SOAInfo> where Task : ConduitFlow.SOAInfo.ConduitTask, new()
		{
			public void Initialize(int conduitCount, ConduitFlow manager)
			{
				base.Initialize(conduitCount);
				foreach (Task task in this.tasks)
				{
					task.manager = manager;
				}
			}
		}

		private class ConduitJob : WorkItemCollection<ConduitFlow.SOAInfo.ConduitTask, ConduitFlow.SOAInfo>
		{
			public void Add<Task>(ConduitFlow.SOAInfo.ConduitTaskDivision<Task> taskDivision) where Task : ConduitFlow.SOAInfo.ConduitTask, new()
			{
				foreach (Task task in taskDivision.tasks)
				{
					base.Add(task);
				}
			}
		}

		private class ClearPermanentDiseaseContainer : ConduitFlow.SOAInfo.ConduitTask
		{
			public ClearPermanentDiseaseContainer()
				: base("ClearPermanentDiseaseContainer")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					soaInfo.ForcePermanentDiseaseContainer(num, false);
				}
			}
		}

		private class PublishTemperatureToSim : ConduitFlow.SOAInfo.ConduitTask
		{
			public PublishTemperatureToSim()
				: base("PublishTemperatureToSim")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					HandleVector<int>.Handle handle = soaInfo.temperatureHandles[num];
					if (handle.IsValid())
					{
						float temperature = Game.Instance.conduitTemperatureManager.GetTemperature(handle);
						this.manager.grid[soaInfo.cells[num]].contents.temperature = temperature;
					}
				}
			}
		}

		private class PublishDiseaseToSim : ConduitFlow.SOAInfo.ConduitTask
		{
			public PublishDiseaseToSim()
				: base("PublishDiseaseToSim")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					HandleVector<int>.Handle handle = soaInfo.diseaseHandles[num];
					if (handle.IsValid())
					{
						ConduitDiseaseManager.Data data = Game.Instance.conduitDiseaseManager.GetData(handle);
						int num2 = soaInfo.cells[num];
						this.manager.grid[num2].contents.diseaseIdx = data.diseaseIdx;
						this.manager.grid[num2].contents.diseaseCount = data.diseaseCount;
					}
				}
			}
		}

		private class ResetConduit : ConduitFlow.SOAInfo.ConduitTask
		{
			public ResetConduit()
				: base("ResetConduitTask")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					this.manager.grid[soaInfo.cells[num]].conduitIdx = -1;
				}
			}
		}

		private class InitializeContentsTask : ConduitFlow.SOAInfo.ConduitTask
		{
			public InitializeContentsTask()
				: base("SetInitialContents")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					int num2 = soaInfo.cells[num];
					ConduitFlow.ConduitContents conduitContents = soaInfo.conduits[num].GetContents(this.manager);
					if (conduitContents.mass <= 0f)
					{
						conduitContents = ConduitFlow.ConduitContents.Empty;
					}
					soaInfo.initialContents[num] = conduitContents;
					this.manager.grid[num2].contents = conduitContents;
				}
			}
		}

		private class InvalidateLastFlow : ConduitFlow.SOAInfo.ConduitTask
		{
			public InvalidateLastFlow()
				: base("InvalidateLastFlow")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					soaInfo.lastFlowInfo[num] = ConduitFlow.ConduitFlowInfo.DEFAULT;
				}
			}
		}

		private class PublishTemperatureToGame : ConduitFlow.SOAInfo.ConduitTask
		{
			public PublishTemperatureToGame()
				: base("PublishTemperatureToGame")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					Game.Instance.conduitTemperatureManager.SetData(soaInfo.temperatureHandles[num], ref this.manager.grid[soaInfo.cells[num]].contents);
				}
			}
		}

		private class PublishDiseaseToGame : ConduitFlow.SOAInfo.ConduitTask
		{
			public PublishDiseaseToGame()
				: base("PublishDiseaseToGame")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					Game.Instance.conduitDiseaseManager.SetData(soaInfo.diseaseHandles[num], ref this.manager.grid[soaInfo.cells[num]].contents);
				}
			}
		}

		private class FlowThroughVacuum : ConduitFlow.SOAInfo.ConduitTask
		{
			public FlowThroughVacuum()
				: base("FlowThroughVacuum")
			{
			}

			protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
			{
				for (int num = this.start; num != this.end; num++)
				{
					ConduitFlow.Conduit conduit = soaInfo.conduits[num];
					int cell = conduit.GetCell(this.manager);
					if (this.manager.grid[cell].contents.element == SimHashes.Vacuum)
					{
						soaInfo.srcFlowDirections[conduit.idx] = ConduitFlow.FlowDirections.None;
					}
				}
			}
		}
	}

	[DebuggerDisplay("{priority} {callback.Target.name} {callback.Target} {callback.Method}")]
	public struct ConduitUpdater
	{
		public ConduitFlowPriority priority;

		public Action<float> callback;
	}

	[DebuggerDisplay("conduit {conduitIdx}:{contents.element}")]
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

	[Flags]
	public enum FlowDirections : byte
	{
		None = 0,
		Down = 1,
		Left = 2,
		Right = 4,
		Up = 8,
		All = 15
	}

	[DebuggerDisplay("conduits l:{left}, r:{right}, u:{up}, d:{down}")]
	public struct ConduitConnections
	{
		public int left;

		public int right;

		public int up;

		public int down;

		public static readonly ConduitFlow.ConduitConnections DEFAULT = new ConduitFlow.ConduitConnections
		{
			left = -1,
			right = -1,
			up = -1,
			down = -1
		};
	}

	[DebuggerDisplay("{direction}:{contents.element}")]
	public struct ConduitFlowInfo
	{
		public ConduitFlow.FlowDirections direction;

		public ConduitFlow.ConduitContents contents;

		public static readonly ConduitFlow.ConduitFlowInfo DEFAULT = new ConduitFlow.ConduitFlowInfo
		{
			direction = ConduitFlow.FlowDirections.None,
			contents = ConduitFlow.ConduitContents.Empty
		};
	}

	[DebuggerDisplay("conduit {idx}")]
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

		public ConduitFlow.FlowDirections GetPermittedFlowDirections(ConduitFlow manager)
		{
			return manager.soaInfo.GetPermittedFlowDirections(this.idx);
		}

		public void SetPermittedFlowDirections(ConduitFlow.FlowDirections permitted, ConduitFlow manager)
		{
			manager.soaInfo.SetPermittedFlowDirections(this.idx, permitted);
		}

		public ConduitFlow.FlowDirections GetTargetFlowDirection(ConduitFlow manager)
		{
			return manager.soaInfo.GetTargetFlowDirection(this.idx);
		}

		public void SetTargetFlowDirection(ConduitFlow.FlowDirections directions, ConduitFlow manager)
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
			global::Debug.Assert(!float.IsNaN(temperature));
			this.element = element;
			this.initial_mass = mass;
			this.added_mass = 0f;
			this.removed_mass = 0f;
			this.temperature = temperature;
			this.diseaseIdx = disease_idx;
			this.diseaseCount = disease_count;
		}

		public float mass
		{
			get
			{
				return this.initial_mass + this.added_mass - this.removed_mass;
			}
		}

		public float movable_mass
		{
			get
			{
				return this.initial_mass - this.removed_mass;
			}
		}

		public void ConsolidateMass()
		{
			this.initial_mass += this.added_mass;
			this.added_mass = 0f;
			this.initial_mass -= this.removed_mass;
			this.removed_mass = 0f;
		}

		public float GetEffectiveCapacity(float maximum_capacity)
		{
			float mass = this.mass;
			DebugUtil.DevAssert(mass <= maximum_capacity, "Effective mass cannot be greater than capacity!");
			return Mathf.Max(0f, maximum_capacity - mass);
		}

		public void AddMass(float amount)
		{
			global::Debug.Assert(0f <= amount);
			this.added_mass += amount;
		}

		public float RemoveMass(float amount)
		{
			global::Debug.Assert(0f <= amount);
			float num = 0f;
			float num2 = this.mass - amount;
			if (num2 < 0f)
			{
				amount += num2;
				num = -num2;
				global::Debug.Assert(false);
			}
			this.removed_mass += amount;
			return num;
		}

		public SimHashes element;

		private float initial_mass;

		private float added_mass;

		private float removed_mass;

		public float temperature;

		public byte diseaseIdx;

		public int diseaseCount;

		public static readonly ConduitFlow.ConduitContents Empty = new ConduitFlow.ConduitContents
		{
			element = SimHashes.Vacuum,
			initial_mass = 0f,
			added_mass = 0f,
			removed_mass = 0f,
			temperature = 0f,
			diseaseIdx = byte.MaxValue,
			diseaseCount = 0
		};
	}

	[DebuggerDisplay("{network.ConduitType}:{cells.Count}")]
	private struct Network
	{
		public List<int> cells;

		public FlowUtilityNetwork network;
	}

	private struct BuildNetworkTask : IWorkItem<ConduitFlow>
	{
		public BuildNetworkTask(ConduitFlow.Network network, int conduit_count)
		{
			this.network = network;
			this.distance_nodes = QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.Allocate();
			this.distances_via_sources = DictionaryPool<int, int, ConduitFlow>.Allocate();
			this.from_sources = ListPool<KeyValuePair<int, int>, ConduitFlow>.Allocate();
			this.distances_via_sinks = DictionaryPool<int, int, ConduitFlow>.Allocate();
			this.from_sinks = ListPool<KeyValuePair<int, int>, ConduitFlow>.Allocate();
			this.from_sources_graph = new ConduitFlow.BuildNetworkTask.Graph(network.network);
			this.from_sinks_graph = new ConduitFlow.BuildNetworkTask.Graph(network.network);
		}

		public void Finish()
		{
			this.distances_via_sinks.Recycle();
			this.distances_via_sources.Recycle();
			this.distance_nodes.Recycle();
			this.from_sources.Recycle();
			this.from_sinks.Recycle();
			this.from_sources_graph.Recycle();
			this.from_sinks_graph.Recycle();
		}

		private void ComputeFlow(ConduitFlow outer)
		{
			this.from_sources_graph.Build(outer, this.network.network.sources, this.network.network.sinks, true);
			this.from_sinks_graph.Build(outer, this.network.network.sinks, this.network.network.sources, false);
			this.from_sources_graph.Merge(this.from_sinks_graph);
			this.from_sources_graph.BreakCycles();
			this.from_sources_graph.WriteFlow(false);
			this.from_sinks_graph.WriteFlow(true);
		}

		private void ComputeOrder(ConduitFlow outer)
		{
			foreach (int num in this.from_sources_graph.sources)
			{
				this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
				{
					cell = num,
					distance = 0
				});
			}
			foreach (int num2 in this.from_sources_graph.dead_ends)
			{
				this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
				{
					cell = num2,
					distance = 0
				});
			}
			while (this.distance_nodes.Count != 0)
			{
				ConduitFlow.BuildNetworkTask.DistanceNode distanceNode = this.distance_nodes.Dequeue();
				int conduitIdx = outer.grid[distanceNode.cell].conduitIdx;
				if (conduitIdx != -1)
				{
					this.distances_via_sources[distanceNode.cell] = distanceNode.distance;
					ConduitFlow.ConduitConnections conduitConnections = outer.soaInfo.GetConduitConnections(conduitIdx);
					ConduitFlow.FlowDirections permittedFlowDirections = outer.soaInfo.GetPermittedFlowDirections(conduitIdx);
					if ((byte)(permittedFlowDirections & ConduitFlow.FlowDirections.Up) != 0)
					{
						this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections.up),
							distance = distanceNode.distance + 1
						});
					}
					if ((byte)(permittedFlowDirections & ConduitFlow.FlowDirections.Down) != 0)
					{
						this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections.down),
							distance = distanceNode.distance + 1
						});
					}
					if ((byte)(permittedFlowDirections & ConduitFlow.FlowDirections.Left) != 0)
					{
						this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections.left),
							distance = distanceNode.distance + 1
						});
					}
					if ((byte)(permittedFlowDirections & ConduitFlow.FlowDirections.Right) != 0)
					{
						this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections.right),
							distance = distanceNode.distance + 1
						});
					}
				}
			}
			this.from_sources.AddRange(this.distances_via_sources);
			this.from_sources.Sort((KeyValuePair<int, int> a, KeyValuePair<int, int> b) => b.Value - a.Value);
			this.distance_nodes.Clear();
			foreach (int num3 in this.from_sinks_graph.sources)
			{
				this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
				{
					cell = num3,
					distance = 0
				});
			}
			foreach (int num4 in this.from_sinks_graph.dead_ends)
			{
				this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
				{
					cell = num4,
					distance = 0
				});
			}
			while (this.distance_nodes.Count != 0)
			{
				ConduitFlow.BuildNetworkTask.DistanceNode distanceNode2 = this.distance_nodes.Dequeue();
				int conduitIdx2 = outer.grid[distanceNode2.cell].conduitIdx;
				if (conduitIdx2 != -1)
				{
					if (!this.distances_via_sources.ContainsKey(distanceNode2.cell))
					{
						this.distances_via_sinks[distanceNode2.cell] = distanceNode2.distance;
					}
					ConduitFlow.ConduitConnections conduitConnections2 = outer.soaInfo.GetConduitConnections(conduitIdx2);
					if (conduitConnections2.up != -1 && (byte)(outer.soaInfo.GetPermittedFlowDirections(conduitConnections2.up) & ConduitFlow.FlowDirections.Down) != 0)
					{
						this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections2.up),
							distance = distanceNode2.distance + 1
						});
					}
					if (conduitConnections2.down != -1 && (byte)(outer.soaInfo.GetPermittedFlowDirections(conduitConnections2.down) & ConduitFlow.FlowDirections.Up) != 0)
					{
						this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections2.down),
							distance = distanceNode2.distance + 1
						});
					}
					if (conduitConnections2.left != -1 && (byte)(outer.soaInfo.GetPermittedFlowDirections(conduitConnections2.left) & ConduitFlow.FlowDirections.Right) != 0)
					{
						this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections2.left),
							distance = distanceNode2.distance + 1
						});
					}
					if (conduitConnections2.right != -1 && (byte)(outer.soaInfo.GetPermittedFlowDirections(conduitConnections2.right) & ConduitFlow.FlowDirections.Left) != 0)
					{
						this.distance_nodes.Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections2.right),
							distance = distanceNode2.distance + 1
						});
					}
				}
			}
			this.from_sinks.AddRange(this.distances_via_sinks);
			this.from_sinks.Sort((KeyValuePair<int, int> a, KeyValuePair<int, int> b) => a.Value - b.Value);
			this.network.cells.Capacity = Mathf.Max(this.network.cells.Capacity, this.from_sources.Count + this.from_sinks.Count);
			foreach (KeyValuePair<int, int> keyValuePair in this.from_sources)
			{
				this.network.cells.Add(keyValuePair.Key);
			}
			foreach (KeyValuePair<int, int> keyValuePair2 in this.from_sinks)
			{
				this.network.cells.Add(keyValuePair2.Key);
			}
		}

		public void Run(ConduitFlow outer)
		{
			this.ComputeFlow(outer);
			this.ComputeOrder(outer);
		}

		private ConduitFlow.Network network;

		private QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distance_nodes;

		private DictionaryPool<int, int, ConduitFlow>.PooledDictionary distances_via_sources;

		private ListPool<KeyValuePair<int, int>, ConduitFlow>.PooledList from_sources;

		private DictionaryPool<int, int, ConduitFlow>.PooledDictionary distances_via_sinks;

		private ListPool<KeyValuePair<int, int>, ConduitFlow>.PooledList from_sinks;

		private ConduitFlow.BuildNetworkTask.Graph from_sources_graph;

		private ConduitFlow.BuildNetworkTask.Graph from_sinks_graph;

		[DebuggerDisplay("cell {cell}:{distance}")]
		private struct DistanceNode
		{
			public int cell;

			public int distance;
		}

		[DebuggerDisplay("vertices:{vertex_cells.Count}, edges:{edges.Count}")]
		private struct Graph
		{
			public Graph(FlowUtilityNetwork network)
			{
				this.conduit_flow = null;
				this.vertex_cells = HashSetPool<int, ConduitFlow>.Allocate();
				this.edges = ListPool<ConduitFlow.BuildNetworkTask.Graph.Edge, ConduitFlow>.Allocate();
				this.cycles = ListPool<ConduitFlow.BuildNetworkTask.Graph.Edge, ConduitFlow>.Allocate();
				this.bfs_traversal = QueuePool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.Allocate();
				this.visited = HashSetPool<int, ConduitFlow>.Allocate();
				this.pseudo_sources = ListPool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.Allocate();
				this.sources = HashSetPool<int, ConduitFlow>.Allocate();
				this.sinks = HashSetPool<int, ConduitFlow>.Allocate();
				this.dfs_path = HashSetPool<ConduitFlow.BuildNetworkTask.Graph.DFSNode, ConduitFlow>.Allocate();
				this.dfs_traversal = ListPool<ConduitFlow.BuildNetworkTask.Graph.DFSNode, ConduitFlow>.Allocate();
				this.dead_ends = HashSetPool<int, ConduitFlow>.Allocate();
				this.cycle_vertices = ListPool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.Allocate();
			}

			public void Recycle()
			{
				this.vertex_cells.Recycle();
				this.edges.Recycle();
				this.cycles.Recycle();
				this.bfs_traversal.Recycle();
				this.visited.Recycle();
				this.pseudo_sources.Recycle();
				this.sources.Recycle();
				this.sinks.Recycle();
				this.dfs_path.Recycle();
				this.dfs_traversal.Recycle();
				this.dead_ends.Recycle();
				this.cycle_vertices.Recycle();
			}

			public void Build(ConduitFlow conduit_flow, List<FlowUtilityNetwork.IItem> sources, List<FlowUtilityNetwork.IItem> sinks, bool are_dead_ends_pseudo_sources)
			{
				this.conduit_flow = conduit_flow;
				this.sources.Clear();
				for (int i = 0; i < sources.Count; i++)
				{
					int cell = sources[i].Cell;
					if (conduit_flow.grid[cell].conduitIdx != -1)
					{
						this.sources.Add(cell);
					}
				}
				this.sinks.Clear();
				for (int j = 0; j < sinks.Count; j++)
				{
					int cell2 = sinks[j].Cell;
					if (conduit_flow.grid[cell2].conduitIdx != -1)
					{
						this.sinks.Add(cell2);
					}
				}
				global::Debug.Assert(this.bfs_traversal.Count == 0);
				this.visited.Clear();
				foreach (int num in this.sources)
				{
					this.bfs_traversal.Enqueue(new ConduitFlow.BuildNetworkTask.Graph.Vertex
					{
						cell = num,
						direction = ConduitFlow.FlowDirections.None
					});
					this.visited.Add(num);
				}
				this.pseudo_sources.Clear();
				this.dead_ends.Clear();
				this.cycles.Clear();
				while (this.bfs_traversal.Count != 0)
				{
					ConduitFlow.BuildNetworkTask.Graph.Vertex node = this.bfs_traversal.Dequeue();
					this.vertex_cells.Add(node.cell);
					ConduitFlow.FlowDirections flowDirections = ConduitFlow.FlowDirections.None;
					int num2 = 4;
					if (node.direction != ConduitFlow.FlowDirections.None)
					{
						flowDirections = ConduitFlow.Opposite(node.direction);
						num2 = 3;
					}
					int conduitIdx = conduit_flow.grid[node.cell].conduitIdx;
					for (int num3 = 0; num3 != num2; num3++)
					{
						flowDirections = ConduitFlow.ComputeNextFlowDirection(flowDirections);
						ConduitFlow.BuildNetworkTask.Graph.Vertex new_node = this.WalkPath(conduitIdx, conduit_flow.soaInfo.GetConduitFromDirection(conduitIdx, flowDirections).idx, flowDirections, are_dead_ends_pseudo_sources);
						if (new_node.is_valid)
						{
							ConduitFlow.BuildNetworkTask.Graph.Edge edge2 = new ConduitFlow.BuildNetworkTask.Graph.Edge
							{
								vertices = new ConduitFlow.BuildNetworkTask.Graph.Vertex[]
								{
									new ConduitFlow.BuildNetworkTask.Graph.Vertex
									{
										cell = node.cell,
										direction = flowDirections
									},
									new_node
								}
							};
							if (new_node.cell == node.cell)
							{
								this.cycles.Add(edge2);
							}
							else if (!this.edges.Any<ConduitFlow.BuildNetworkTask.Graph.Edge>((ConduitFlow.BuildNetworkTask.Graph.Edge edge) => edge.vertices[0].cell == new_node.cell && edge.vertices[1].cell == node.cell))
							{
								if (!this.edges.Contains(edge2))
								{
									this.edges.Add(edge2);
									if (this.visited.Add(new_node.cell))
									{
										if (this.IsSink(new_node.cell))
										{
											this.pseudo_sources.Add(new_node);
										}
										else
										{
											this.bfs_traversal.Enqueue(new_node);
										}
									}
								}
							}
						}
					}
					if (this.bfs_traversal.Count == 0)
					{
						foreach (ConduitFlow.BuildNetworkTask.Graph.Vertex vertex in this.pseudo_sources)
						{
							this.bfs_traversal.Enqueue(vertex);
						}
						this.pseudo_sources.Clear();
					}
				}
			}

			private bool IsEndpoint(int cell)
			{
				global::Debug.Assert(cell != -1);
				ConduitFlow.GridNode gridNode = this.conduit_flow.grid[cell];
				return gridNode.conduitIdx == -1 || this.sources.Contains(cell) || this.sinks.Contains(cell) || this.dead_ends.Contains(cell);
			}

			private bool IsSink(int cell)
			{
				return this.sinks.Contains(cell);
			}

			private bool IsJunction(int cell)
			{
				global::Debug.Assert(cell != -1);
				ConduitFlow.GridNode gridNode = this.conduit_flow.grid[cell];
				global::Debug.Assert(gridNode.conduitIdx != -1);
				ConduitFlow.ConduitConnections conduitConnections = this.conduit_flow.soaInfo.GetConduitConnections(gridNode.conduitIdx);
				return 2 < this.JunctionValue(conduitConnections.down) + this.JunctionValue(conduitConnections.left) + this.JunctionValue(conduitConnections.up) + this.JunctionValue(conduitConnections.right);
			}

			private int JunctionValue(int conduit)
			{
				return (conduit != -1) ? 1 : 0;
			}

			private ConduitFlow.BuildNetworkTask.Graph.Vertex WalkPath(int root_conduit, int conduit, ConduitFlow.FlowDirections direction, bool are_dead_ends_pseudo_sources)
			{
				if (conduit == -1)
				{
					return ConduitFlow.BuildNetworkTask.Graph.Vertex.INVALID;
				}
				int cell;
				for (;;)
				{
					cell = this.conduit_flow.soaInfo.GetCell(conduit);
					if (this.IsEndpoint(cell) || this.IsJunction(cell))
					{
						break;
					}
					direction = ConduitFlow.Opposite(direction);
					bool flag = true;
					for (int num = 0; num != 3; num++)
					{
						direction = ConduitFlow.ComputeNextFlowDirection(direction);
						ConduitFlow.Conduit conduitFromDirection = this.conduit_flow.soaInfo.GetConduitFromDirection(conduit, direction);
						if (conduitFromDirection.idx != -1)
						{
							conduit = conduitFromDirection.idx;
							flag = false;
							break;
						}
					}
					if (flag)
					{
						goto Block_4;
					}
				}
				return new ConduitFlow.BuildNetworkTask.Graph.Vertex
				{
					cell = cell,
					direction = direction
				};
				Block_4:
				if (are_dead_ends_pseudo_sources)
				{
					this.pseudo_sources.Add(new ConduitFlow.BuildNetworkTask.Graph.Vertex
					{
						cell = cell,
						direction = ConduitFlow.ComputeNextFlowDirection(direction)
					});
					this.dead_ends.Add(cell);
					return ConduitFlow.BuildNetworkTask.Graph.Vertex.INVALID;
				}
				ConduitFlow.BuildNetworkTask.Graph.Vertex vertex = default(ConduitFlow.BuildNetworkTask.Graph.Vertex);
				vertex.cell = cell;
				direction = (vertex.direction = ConduitFlow.Opposite(ConduitFlow.ComputeNextFlowDirection(direction)));
				return vertex;
			}

			public void Merge(ConduitFlow.BuildNetworkTask.Graph inverted_graph)
			{
				using (List<ConduitFlow.BuildNetworkTask.Graph.Edge>.Enumerator enumerator = inverted_graph.edges.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ConduitFlow.BuildNetworkTask.Graph.<Merge>c__AnonStorey2 <Merge>c__AnonStorey = new ConduitFlow.BuildNetworkTask.Graph.<Merge>c__AnonStorey2();
						<Merge>c__AnonStorey.inverted_edge = enumerator.Current;
						ConduitFlow.BuildNetworkTask.Graph.Edge candidate = <Merge>c__AnonStorey.inverted_edge.Invert();
						if (!this.edges.Any<ConduitFlow.BuildNetworkTask.Graph.Edge>((ConduitFlow.BuildNetworkTask.Graph.Edge edge) => edge.Equals(<Merge>c__AnonStorey.inverted_edge) || edge.Equals(candidate)))
						{
							this.edges.Add(candidate);
							this.vertex_cells.Add(candidate.vertices[0].cell);
							this.vertex_cells.Add(candidate.vertices[1].cell);
						}
					}
				}
				int num = 1000;
				for (int num2 = 0; num2 != num; num2++)
				{
					global::Debug.Assert(num2 != num - 1);
					bool flag = false;
					using (HashSet<int>.Enumerator enumerator2 = this.vertex_cells.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							int cell = enumerator2.Current;
							if (!this.IsSink(cell))
							{
								bool flag2 = this.edges.Any<ConduitFlow.BuildNetworkTask.Graph.Edge>((ConduitFlow.BuildNetworkTask.Graph.Edge edge) => edge.vertices[0].cell == cell);
								if (!flag2)
								{
									int num3 = inverted_graph.edges.FindIndex((ConduitFlow.BuildNetworkTask.Graph.Edge inverted_edge) => inverted_edge.vertices[1].cell == cell);
									if (num3 != -1)
									{
										ConduitFlow.BuildNetworkTask.Graph.Edge edge3 = inverted_graph.edges[num3];
										for (int num4 = 0; num4 != this.edges.Count; num4++)
										{
											ConduitFlow.BuildNetworkTask.Graph.Edge edge2 = this.edges[num4];
											if (edge2.vertices[0].cell == edge3.vertices[0].cell && edge2.vertices[1].cell == edge3.vertices[1].cell)
											{
												this.edges[num4] = edge2.Invert();
											}
										}
										flag = true;
										break;
									}
								}
							}
						}
					}
					if (!flag)
					{
						break;
					}
				}
			}

			public void BreakCycles()
			{
				this.visited.Clear();
				foreach (int num in this.vertex_cells)
				{
					if (!this.visited.Contains(num))
					{
						this.dfs_path.Clear();
						this.dfs_traversal.Clear();
						this.dfs_traversal.Add(new ConduitFlow.BuildNetworkTask.Graph.DFSNode
						{
							cell = num,
							parent = null
						});
						while (this.dfs_traversal.Count != 0)
						{
							ConduitFlow.BuildNetworkTask.Graph.DFSNode dfsnode = this.dfs_traversal[this.dfs_traversal.Count - 1];
							this.dfs_traversal.RemoveAt(this.dfs_traversal.Count - 1);
							bool flag = false;
							for (ConduitFlow.BuildNetworkTask.Graph.DFSNode dfsnode2 = dfsnode.parent; dfsnode2 != null; dfsnode2 = dfsnode2.parent)
							{
								if (dfsnode2.cell == dfsnode.cell)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								for (int num2 = this.edges.Count - 1; num2 != -1; num2--)
								{
									ConduitFlow.BuildNetworkTask.Graph.Edge edge = this.edges[num2];
									if (edge.vertices[0].cell == dfsnode.parent.cell && edge.vertices[1].cell == dfsnode.cell)
									{
										this.cycles.Add(edge);
										this.edges.RemoveAt(num2);
									}
								}
							}
							else if (this.visited.Add(dfsnode.cell))
							{
								foreach (ConduitFlow.BuildNetworkTask.Graph.Edge edge2 in this.edges)
								{
									if (edge2.vertices[0].cell == dfsnode.cell)
									{
										this.dfs_traversal.Add(new ConduitFlow.BuildNetworkTask.Graph.DFSNode
										{
											cell = edge2.vertices[1].cell,
											parent = dfsnode
										});
									}
								}
							}
						}
					}
				}
			}

			public void WriteFlow(bool cycles_only = false)
			{
				if (!cycles_only)
				{
					foreach (ConduitFlow.BuildNetworkTask.Graph.Edge edge in this.edges)
					{
						ConduitFlow.BuildNetworkTask.Graph.Edge.VertexIterator vertexIterator = edge.Iter(this.conduit_flow);
						while (vertexIterator.IsValid())
						{
							this.conduit_flow.soaInfo.AddPermittedFlowDirections(this.conduit_flow.grid[vertexIterator.cell].conduitIdx, vertexIterator.direction);
							vertexIterator.Next();
						}
					}
				}
				foreach (ConduitFlow.BuildNetworkTask.Graph.Edge edge2 in this.cycles)
				{
					this.cycle_vertices.Clear();
					ConduitFlow.BuildNetworkTask.Graph.Edge.VertexIterator vertexIterator2 = edge2.Iter(this.conduit_flow);
					vertexIterator2.Next();
					while (vertexIterator2.IsValid())
					{
						this.cycle_vertices.Add(new ConduitFlow.BuildNetworkTask.Graph.Vertex
						{
							cell = vertexIterator2.cell,
							direction = vertexIterator2.direction
						});
						vertexIterator2.Next();
					}
					if (this.cycle_vertices.Count > 1)
					{
						int i = 0;
						int num = this.cycle_vertices.Count - 1;
						ConduitFlow.FlowDirections flowDirections = edge2.vertices[0].direction;
						while (i <= num)
						{
							ConduitFlow.BuildNetworkTask.Graph.Vertex vertex = this.cycle_vertices[i];
							this.conduit_flow.soaInfo.AddPermittedFlowDirections(this.conduit_flow.grid[vertex.cell].conduitIdx, ConduitFlow.Opposite(flowDirections));
							flowDirections = vertex.direction;
							i++;
							ConduitFlow.BuildNetworkTask.Graph.Vertex vertex2 = this.cycle_vertices[num];
							this.conduit_flow.soaInfo.AddPermittedFlowDirections(this.conduit_flow.grid[vertex2.cell].conduitIdx, vertex2.direction);
							num--;
						}
						this.dead_ends.Add(this.cycle_vertices[i].cell);
						this.dead_ends.Add(this.cycle_vertices[num].cell);
					}
				}
			}

			private ConduitFlow conduit_flow;

			private HashSetPool<int, ConduitFlow>.PooledHashSet vertex_cells;

			private ListPool<ConduitFlow.BuildNetworkTask.Graph.Edge, ConduitFlow>.PooledList edges;

			private ListPool<ConduitFlow.BuildNetworkTask.Graph.Edge, ConduitFlow>.PooledList cycles;

			private QueuePool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.PooledQueue bfs_traversal;

			private HashSetPool<int, ConduitFlow>.PooledHashSet visited;

			private ListPool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.PooledList pseudo_sources;

			public HashSetPool<int, ConduitFlow>.PooledHashSet sources;

			private HashSetPool<int, ConduitFlow>.PooledHashSet sinks;

			private HashSetPool<ConduitFlow.BuildNetworkTask.Graph.DFSNode, ConduitFlow>.PooledHashSet dfs_path;

			private ListPool<ConduitFlow.BuildNetworkTask.Graph.DFSNode, ConduitFlow>.PooledList dfs_traversal;

			public HashSetPool<int, ConduitFlow>.PooledHashSet dead_ends;

			private ListPool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.PooledList cycle_vertices;

			[DebuggerDisplay("{cell}:{direction}")]
			public struct Vertex : IEquatable<ConduitFlow.BuildNetworkTask.Graph.Vertex>
			{
				public bool is_valid
				{
					get
					{
						return this.cell != -1;
					}
				}

				public bool Equals(ConduitFlow.BuildNetworkTask.Graph.Vertex rhs)
				{
					return this.direction == rhs.direction && this.cell == rhs.cell;
				}

				public ConduitFlow.FlowDirections direction;

				public int cell;

				public static ConduitFlow.BuildNetworkTask.Graph.Vertex INVALID = new ConduitFlow.BuildNetworkTask.Graph.Vertex
				{
					direction = ConduitFlow.FlowDirections.None,
					cell = -1
				};
			}

			[DebuggerDisplay("{vertices[0].cell}:{vertices[0].direction} -> {vertices[1].cell}:{vertices[1].direction}")]
			public struct Edge : IEquatable<ConduitFlow.BuildNetworkTask.Graph.Edge>
			{
				public bool is_valid
				{
					get
					{
						return this.vertices != null;
					}
				}

				public bool Equals(ConduitFlow.BuildNetworkTask.Graph.Edge rhs)
				{
					if (this.vertices == null)
					{
						return rhs.vertices == null;
					}
					return rhs.vertices != null && (this.vertices.Length == rhs.vertices.Length && this.vertices.Length == 2 && this.vertices[0].Equals(rhs.vertices[0])) && this.vertices[1].Equals(rhs.vertices[1]);
				}

				public ConduitFlow.BuildNetworkTask.Graph.Edge Invert()
				{
					return new ConduitFlow.BuildNetworkTask.Graph.Edge
					{
						vertices = new ConduitFlow.BuildNetworkTask.Graph.Vertex[]
						{
							new ConduitFlow.BuildNetworkTask.Graph.Vertex
							{
								cell = this.vertices[1].cell,
								direction = ConduitFlow.Opposite(this.vertices[1].direction)
							},
							new ConduitFlow.BuildNetworkTask.Graph.Vertex
							{
								cell = this.vertices[0].cell,
								direction = ConduitFlow.Opposite(this.vertices[0].direction)
							}
						}
					};
				}

				public ConduitFlow.BuildNetworkTask.Graph.Edge.VertexIterator Iter(ConduitFlow conduit_flow)
				{
					return new ConduitFlow.BuildNetworkTask.Graph.Edge.VertexIterator(conduit_flow, this);
				}

				public ConduitFlow.BuildNetworkTask.Graph.Vertex[] vertices;

				public static readonly ConduitFlow.BuildNetworkTask.Graph.Edge INVALID = new ConduitFlow.BuildNetworkTask.Graph.Edge
				{
					vertices = null
				};

				[DebuggerDisplay("{cell}:{direction}")]
				public struct VertexIterator
				{
					public VertexIterator(ConduitFlow conduit_flow, ConduitFlow.BuildNetworkTask.Graph.Edge edge)
					{
						this.conduit_flow = conduit_flow;
						this.edge = edge;
						this.cell = edge.vertices[0].cell;
						this.direction = edge.vertices[0].direction;
					}

					public void Next()
					{
						int conduitIdx = this.conduit_flow.grid[this.cell].conduitIdx;
						ConduitFlow.Conduit conduitFromDirection = this.conduit_flow.soaInfo.GetConduitFromDirection(conduitIdx, this.direction);
						global::Debug.Assert(conduitFromDirection.idx != -1);
						this.cell = conduitFromDirection.GetCell(this.conduit_flow);
						if (this.cell == this.edge.vertices[1].cell)
						{
							return;
						}
						this.direction = ConduitFlow.Opposite(this.direction);
						bool flag = false;
						for (int num = 0; num != 3; num++)
						{
							this.direction = ConduitFlow.ComputeNextFlowDirection(this.direction);
							if (this.conduit_flow.soaInfo.GetConduitFromDirection(conduitFromDirection.idx, this.direction).idx != -1)
							{
								flag = true;
								break;
							}
						}
						global::Debug.Assert(flag);
						if (!flag)
						{
							this.cell = this.edge.vertices[1].cell;
						}
					}

					public bool IsValid()
					{
						return this.cell != this.edge.vertices[1].cell;
					}

					public int cell;

					public ConduitFlow.FlowDirections direction;

					private ConduitFlow conduit_flow;

					private ConduitFlow.BuildNetworkTask.Graph.Edge edge;
				}
			}

			[DebuggerDisplay("cell:{cell}, parent:{parent == null ? -1 : parent.cell}")]
			private class DFSNode
			{
				public int cell;

				public ConduitFlow.BuildNetworkTask.Graph.DFSNode parent;
			}
		}
	}

	private struct ConnectContext
	{
		public ConnectContext(ConduitFlow outer)
		{
			this.outer = outer;
			this.cells = ListPool<int, ConduitFlow>.Allocate();
			this.cells.Capacity = Mathf.Max(this.cells.Capacity, outer.soaInfo.NumEntries);
		}

		public void Finish()
		{
			this.cells.Recycle();
		}

		public ListPool<int, ConduitFlow>.PooledList cells;

		public ConduitFlow outer;
	}

	private struct ConnectTask : IWorkItem<ConduitFlow.ConnectContext>
	{
		public ConnectTask(int start, int end)
		{
			this.start = start;
			this.end = end;
		}

		public void Run(ConduitFlow.ConnectContext context)
		{
			for (int num = this.start; num != this.end; num++)
			{
				int num2 = context.cells[num];
				int conduitIdx = context.outer.grid[num2].conduitIdx;
				if (conduitIdx != -1)
				{
					UtilityConnections connections = context.outer.networkMgr.GetConnections(num2, true);
					if (connections != (UtilityConnections)0)
					{
						ConduitFlow.ConduitConnections @default = ConduitFlow.ConduitConnections.DEFAULT;
						int num3 = num2 - 1;
						if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Left) != (UtilityConnections)0)
						{
							@default.left = context.outer.grid[num3].conduitIdx;
						}
						num3 = num2 + 1;
						if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Right) != (UtilityConnections)0)
						{
							@default.right = context.outer.grid[num3].conduitIdx;
						}
						num3 = num2 - Grid.WidthInCells;
						if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Down) != (UtilityConnections)0)
						{
							@default.down = context.outer.grid[num3].conduitIdx;
						}
						num3 = num2 + Grid.WidthInCells;
						if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Up) != (UtilityConnections)0)
						{
							@default.up = context.outer.grid[num3].conduitIdx;
						}
						context.outer.soaInfo.SetConduitConnections(conduitIdx, @default);
					}
				}
			}
		}

		private int start;

		private int end;
	}

	private struct Sink
	{
		public Sink(FlowUtilityNetwork.IItem sink)
		{
			this.consumer = ((!(sink.GameObject != null)) ? null : sink.GameObject.GetComponent<ConduitConsumer>());
			this.space_remaining = ((!(this.consumer != null) || !this.consumer.operational.IsOperational) ? 0f : this.consumer.space_remaining_kg);
		}

		public ConduitConsumer consumer;

		public float space_remaining;
	}

	private class UpdateNetworkTask : IWorkItem<ConduitFlow>
	{
		public UpdateNetworkTask(ConduitFlow.Network network)
		{
			this.continue_updating = true;
			this.network = network;
			this.sinks = DictionaryPool<int, ConduitFlow.Sink, ConduitFlow>.Allocate();
			foreach (FlowUtilityNetwork.IItem item in network.network.sinks)
			{
				this.sinks.Add(item.Cell, new ConduitFlow.Sink(item));
			}
		}

		public bool continue_updating { get; private set; }

		public void Run(ConduitFlow conduit_flow)
		{
			global::Debug.Assert(this.continue_updating);
			this.continue_updating = false;
			foreach (int num in this.network.cells)
			{
				int conduitIdx = conduit_flow.grid[num].conduitIdx;
				if (conduit_flow.UpdateConduit(conduit_flow.soaInfo.GetConduit(conduitIdx), this.sinks))
				{
					this.continue_updating = true;
				}
			}
		}

		public void Finish(ConduitFlow conduit_flow)
		{
			foreach (int num in this.network.cells)
			{
				ConduitFlow.ConduitContents contents = conduit_flow.grid[num].contents;
				contents.ConsolidateMass();
				conduit_flow.grid[num].contents = contents;
			}
			this.sinks.Recycle();
		}

		private ConduitFlow.Network network;

		private DictionaryPool<int, ConduitFlow.Sink, ConduitFlow>.PooledDictionary sinks;
	}
}
