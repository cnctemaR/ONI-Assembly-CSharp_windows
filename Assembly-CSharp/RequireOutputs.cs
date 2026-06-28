using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class RequireOutputs : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		ScenePartitionerLayer scenePartitionerLayer = null;
		Building component = base.GetComponent<Building>();
		this.utilityCell = component.GetUtilityOutputCell();
		this.conduitType = component.Def.OutputConduitType;
		ConduitType outputConduitType = component.Def.OutputConduitType;
		if (outputConduitType != ConduitType.Gas)
		{
			if (outputConduitType == ConduitType.Liquid)
			{
				scenePartitionerLayer = GameScenePartitioner.Instance.liquidConduitsLayer;
			}
		}
		else
		{
			scenePartitionerLayer = GameScenePartitioner.Instance.gasConduitsLayer;
		}
		this.UpdateConnectionState(true);
		this.UpdatePipeRoomState(true);
		if (scenePartitionerLayer != null)
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("RequireOutputs", base.gameObject, this.utilityCell, scenePartitionerLayer, delegate(object data)
			{
				this.UpdateConnectionState(false);
			});
		}
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.UpdatePipeState), ConduitFlow.Priority.First);
	}

	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.UpdatePipeState));
		base.OnCleanUp();
	}

	private void UpdateConnectionState(bool force_update = false)
	{
		this.connected = this.IsConnected(this.utilityCell);
		if (this.connected != this.previouslyConnected || force_update)
		{
			this.operational.SetFlag(RequireOutputs.outputConnectedFlag, this.connected);
			this.previouslyConnected = this.connected;
			StatusItem statusItem = null;
			ConduitType conduitType = this.conduitType;
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
			this.selectable.ToggleStatusItem(statusItem, !this.connected, this);
		}
	}

	private bool OutputPipeIsEmpty()
	{
		bool flag;
		if (this.connected)
		{
			ConduitFlow conduitManager = this.GetConduitManager();
			flag = conduitManager.GetContents(this.utilityCell).mass <= 0f;
		}
		else
		{
			flag = true;
		}
		return flag;
	}

	private void UpdatePipeState(float dt)
	{
		this.UpdatePipeRoomState(false);
	}

	private void UpdatePipeRoomState(bool force_update = false)
	{
		bool flag = this.OutputPipeIsEmpty();
		if (flag != this.previouslyHadRoom || force_update)
		{
			this.operational.SetFlag(RequireOutputs.pipesHaveRoomFlag, flag);
			this.previouslyHadRoom = flag;
			StatusItem statusItem = null;
			ConduitType conduitType = this.conduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					statusItem = Db.Get().BuildingStatusItems.LiquidPipeObstructed;
				}
			}
			else
			{
				statusItem = Db.Get().BuildingStatusItems.GasPipeObstructed;
			}
			this.selectable.ToggleStatusItem(statusItem, !flag, null);
		}
	}

	private ConduitFlow GetConduitManager()
	{
		ConduitType conduitType = this.conduitType;
		if (conduitType == ConduitType.Gas)
		{
			return Game.Instance.gasConduitFlow;
		}
		if (conduitType != ConduitType.Liquid)
		{
			return null;
		}
		return Game.Instance.liquidConduitFlow;
	}

	private bool IsConnected(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, (this.conduitType != ConduitType.Gas) ? 15 : 11];
		return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Operational operational;

	private int utilityCell;

	private ConduitType conduitType;

	private static Operational.Flag outputConnectedFlag = new Operational.Flag("output_connected", Operational.Flag.Type.Requirement);

	private static Operational.Flag pipesHaveRoomFlag = new Operational.Flag("pipesHaveRoom", Operational.Flag.Type.Requirement);

	private bool previouslyConnected = true;

	private bool previouslyHadRoom = true;

	private bool connected;

	private GameScenePartitionerEntry partitionerEntry;
}
