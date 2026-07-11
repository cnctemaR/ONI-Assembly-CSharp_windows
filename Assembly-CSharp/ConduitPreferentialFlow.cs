using System;
using UnityEngine;

public class ConduitPreferentialFlow : KMonoBehaviour, ISecondaryInput
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		int num = Grid.PosToCell(base.transform.GetPosition());
		CellOffset rotatedOffset = component.GetRotatedOffset(this.portInfo.offset);
		int num2 = Grid.OffsetCell(num, rotatedOffset);
		Conduit.GetFlowManager(this.portInfo.conduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
		this.secondaryInput = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Sink, num2, base.gameObject);
		networkManager.AddToNetworks(this.secondaryInput.Cell, this.secondaryInput, true);
	}

	protected override void OnCleanUp()
	{
		Conduit.GetNetworkManager(this.portInfo.conduitType).RemoveFromNetworks(this.secondaryInput.Cell, this.secondaryInput, true);
		Conduit.GetFlowManager(this.portInfo.conduitType).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(this.portInfo.conduitType);
		if (!flowManager.HasConduit(this.outputCell))
		{
			return;
		}
		int cell = this.inputCell;
		ConduitFlow.ConduitContents conduitContents = flowManager.GetContents(cell);
		if (conduitContents.mass <= 0f)
		{
			cell = this.secondaryInput.Cell;
			conduitContents = flowManager.GetContents(cell);
		}
		if (conduitContents.mass > 0f)
		{
			float num = flowManager.AddElement(this.outputCell, conduitContents.element, conduitContents.mass, conduitContents.temperature, conduitContents.diseaseIdx, conduitContents.diseaseCount);
			if (num > 0f)
			{
				flowManager.RemoveElement(cell, num);
			}
		}
	}

	public ConduitType GetSecondaryConduitType()
	{
		return this.portInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset()
	{
		return this.portInfo.offset;
	}

	[SerializeField]
	public ConduitPortInfo portInfo;

	private int inputCell;

	private int outputCell;

	private FlowUtilityNetwork.NetworkItem secondaryInput;
}
