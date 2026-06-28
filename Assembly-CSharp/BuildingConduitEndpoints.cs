using System;

public class BuildingConduitEndpoints : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		BuildingDef def = component.Def;
		if (def.InputConduitType != ConduitType.None)
		{
			int utilityInputCell = component.GetUtilityInputCell();
			this.itemInput = new FlowUtilityNetwork.NetworkItem(def.InputConduitType, Endpoint.Sink, utilityInputCell, 1000);
			Conduit.GetNetworkManager(def.InputConduitType).AddToNetworks(utilityInputCell, this.itemInput, true);
		}
		if (def.OutputConduitType != ConduitType.None)
		{
			int utilityOutputCell = component.GetUtilityOutputCell();
			this.itemOutput = new FlowUtilityNetwork.NetworkItem(def.OutputConduitType, Endpoint.Source, utilityOutputCell, 1000);
			Conduit.GetNetworkManager(def.OutputConduitType).AddToNetworks(utilityOutputCell, this.itemOutput, true);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.itemInput != null)
		{
			Conduit.GetNetworkManager(this.itemInput.ConduitType).RemoveFromNetworks(this.itemInput.Cell, this.itemInput, true);
		}
		if (this.itemOutput != null)
		{
			Conduit.GetNetworkManager(this.itemOutput.ConduitType).RemoveFromNetworks(this.itemOutput.Cell, this.itemOutput, true);
		}
		base.OnCleanUp();
	}

	private FlowUtilityNetwork.NetworkItem itemInput;

	private FlowUtilityNetwork.NetworkItem itemOutput;
}
