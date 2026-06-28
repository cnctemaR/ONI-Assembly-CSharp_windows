using System;
using KSerialization;
using UnityEngine;

public class ConduitJoiner : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.flowAccumulator = new Accumulator("Flow", this, 3f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.type);
		conduitFlowManager.AddConduitUpdater(new Action<float>(this.ConduitUpdate), 0);
		this.outputConduit = new ConduitFlow.BuildingConduit(conduitFlowManager);
		this.outputConduit.cell = this.outputCell;
		conduitFlowManager.AddBuildingConduit(this.outputConduit);
		IUtilityNetworkMgr networkManager = Game.Instance.GetNetworkManager(this.type);
		if (this.itemInput != null)
		{
			networkManager.RemoveFromNetworks(this.itemInput.Cell, this.itemInput);
		}
		if (this.itemOutput != null)
		{
			networkManager.RemoveFromNetworks(this.itemOutput.Cell, this.itemOutput);
		}
		this.itemInput = new FlowUtilityNetwork.NetworkItem(this.type, Vent.Endpoint.Sink, this.inputCell, 1000, null);
		this.itemOutput = new FlowUtilityNetwork.NetworkItem(this.type, Vent.Endpoint.Source, this.outputCell, 1000, null);
		Vent[] components = base.GetComponents<Vent>();
		foreach (Vent vent in components)
		{
			if (vent != null && vent.endpointType == Vent.Endpoint.Source)
			{
				vent.SortKey = 1000;
			}
		}
		networkManager.AddToNetworks(this.inputCell, this.itemInput);
		networkManager.AddToNetworks(this.outputCell, this.itemOutput);
	}

	protected override void OnCleanUp()
	{
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.type);
		conduitFlowManager.RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.type);
		ConduitFlow.Conduit conduit = conduitFlowManager.GetConduit(this.inputCell);
		if (conduit == null)
		{
			return;
		}
		if (this.outputConduit.GetContents().element == SimHashes.Vacuum)
		{
			ConduitFlow.ConduitContents contents = conduit.GetContents();
			if (contents.mass > 0f)
			{
				this.flowAccumulator.Accumulate(contents.mass);
				ConduitFlow.ConduitContents conduitContents = contents;
				conduitContents.mass = contents.mass;
				this.outputConduit.SetContents(conduitContents);
				conduitFlowManager.RemoveElement(this.inputCell, contents.mass);
			}
		}
	}

	[SerializeField]
	public Vent.Transfer type;

	private int inputCell;

	private int outputCell;

	private FlowUtilityNetwork.NetworkItem itemInput;

	private FlowUtilityNetwork.NetworkItem itemOutput;

	[Serialize]
	private ConduitFlow.BuildingConduit outputConduit;

	private HandleVector<ConduitFlow.BuildingConduit>.Handle outputConduitHandle;

	private Accumulator flowAccumulator;
}
