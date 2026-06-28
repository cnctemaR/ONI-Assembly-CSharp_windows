using System;
using UnityEngine;

public class ConduitBridge : KMonoBehaviour
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
		Conduit.GetFlowManager(this.type).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlow.Priority.Default);
	}

	protected override void OnCleanUp()
	{
		Conduit.GetFlowManager(this.type).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(this.type);
		ConduitFlow.Conduit conduit = flowManager.GetConduit(this.inputCell);
		if (conduit == null)
		{
			return;
		}
		ConduitFlow.ConduitContents contents = conduit.GetContents();
		if (contents.mass > 0f)
		{
			float num = flowManager.AddElement(this.outputCell, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
			if (num > 0f)
			{
				flowManager.RemoveElement(this.inputCell, num);
				this.flowAccumulator.Accumulate(contents.mass);
			}
		}
	}

	[SerializeField]
	public ConduitType type;

	private int inputCell;

	private int outputCell;

	private Accumulator flowAccumulator;
}
