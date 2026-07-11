using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SolidConduitBridge")]
public class SolidConduitBridge : KMonoBehaviour
{
	public bool IsDispensing
	{
		get
		{
			return this.dispensing;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		SolidConduit.GetFlowManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
	}

	protected override void OnCleanUp()
	{
		SolidConduit.GetFlowManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		this.dispensing = false;
		if (this.operational && !this.operational.IsOperational)
		{
			return;
		}
		SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
		if (!flowManager.HasConduit(this.inputCell))
		{
			return;
		}
		if (!flowManager.HasConduit(this.outputCell))
		{
			return;
		}
		if (flowManager.IsConduitFull(this.inputCell) && flowManager.IsConduitEmpty(this.outputCell))
		{
			Pickupable pickupable = flowManager.RemovePickupable(this.inputCell);
			if (pickupable)
			{
				flowManager.AddPickupable(this.outputCell, pickupable);
				this.dispensing = true;
			}
		}
	}

	[MyCmpGet]
	private Operational operational;

	private int inputCell;

	private int outputCell;

	private bool dispensing;
}
