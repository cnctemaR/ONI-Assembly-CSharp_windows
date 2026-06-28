using System;
using UnityEngine;

public class RequireOutputs : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), 10);
	}

	private ConduitFlow GetConduitManager()
	{
		ConduitType outputConduitType = this.building.Def.OutputConduitType;
		if (outputConduitType == ConduitType.Gas)
		{
			return Game.Instance.gasConduitFlow;
		}
		if (outputConduitType != ConduitType.Liquid)
		{
			return null;
		}
		return Game.Instance.liquidConduitFlow;
	}

	protected override void OnCleanUp()
	{
		this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		base.OnCleanUp();
	}

	private bool IsConnected(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, (this.building.Def.OutputConduitType != ConduitType.Gas) ? 12 : 10];
		return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
	}

	private void ConduitUpdate(float dt)
	{
		int utilityOutputCell = this.building.GetUtilityOutputCell();
		bool flag = this.IsConnected(utilityOutputCell);
		this.operational.SetFlag(RequireOutputs.outputConnectedFlag, flag);
		if (flag != this.previouslyConnected)
		{
			this.previouslyConnected = flag;
			StatusItem statusItem = null;
			ConduitType conduitType = this.building.Def.OutputConduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					statusItem = Db.Get().BuildingStatusItems.NeedLiquidOut;
				}
			}
			else
			{
				statusItem = Db.Get().BuildingStatusItems.NeedGasOut;
			}
			if (statusItem != null)
			{
				this.selectable.ToggleStatusItem(statusItem, !flag, this);
			}
		}
		bool flag2;
		if (flag)
		{
			ConduitFlow conduitManager = this.GetConduitManager();
			flag2 = conduitManager.GetContents(utilityOutputCell).mass <= 0f;
		}
		else
		{
			flag2 = true;
		}
		this.operational.SetFlag(RequireOutputs.pipesHaveRoomFlag, flag2);
		if (flag2 != this.previouslyHadRoom)
		{
			this.previouslyHadRoom = flag2;
			StatusItem statusItem2 = null;
			ConduitType conduitType = this.building.Def.OutputConduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					statusItem2 = Db.Get().BuildingStatusItems.LiquidPipeObstructed;
				}
			}
			else
			{
				statusItem2 = Db.Get().BuildingStatusItems.GasPipeObstructed;
			}
			this.selectable.ToggleStatusItem(statusItem2, !flag2, null);
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private Building building;

	private ConduitDispenser dispenser;

	private static Operational.Flag outputConnectedFlag = new Operational.Flag("output_connected", Operational.Flag.Type.Requirement);

	private static Operational.Flag pipesHaveRoomFlag = new Operational.Flag("pipesHaveRoom", Operational.Flag.Type.Requirement);

	private bool previouslyConnected = true;

	private bool previouslyHadRoom = true;
}
