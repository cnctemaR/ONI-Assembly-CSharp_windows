using System;
using UnityEngine;

public class ConduitBridge : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.accumulator = Game.Instance.accumulators.Add("Flow", this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		Conduit.GetFlowManager(this.type).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(this.accumulator);
		Conduit.GetFlowManager(this.type).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(this.type);
		if (!flowManager.HasConduit(this.inputCell))
		{
			return;
		}
		ConduitFlow.ConduitContents contents = flowManager.GetContents(this.inputCell);
		if (contents.mass > 0f)
		{
			float num = flowManager.AddElement(this.outputCell, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
			if (num > 0f)
			{
				flowManager.RemoveElement(this.inputCell, num);
				Game.Instance.accumulators.Accumulate(this.accumulator, contents.mass);
			}
		}
	}

	[SerializeField]
	public ConduitType type;

	private int inputCell;

	private int outputCell;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
}
