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
			if (outputConduitType != ConduitType.Liquid)
			{
				if (outputConduitType == ConduitType.Solid)
				{
					scenePartitionerLayer = GameScenePartitioner.Instance.solidConduitsLayer;
				}
			}
			else
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
		this.GetConduitFlow().AddConduitUpdater(new Action<float>(this.UpdatePipeState), ConduitFlowPriority.First);
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		IConduitFlow conduitFlow = this.GetConduitFlow();
		if (conduitFlow != null)
		{
			conduitFlow.RemoveConduitUpdater(new Action<float>(this.UpdatePipeState));
		}
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
			if (conduitType != ConduitType.Liquid)
			{
				if (conduitType != ConduitType.Gas)
				{
					if (conduitType == ConduitType.Solid)
					{
						statusItem = Db.Get().BuildingStatusItems.NeedSolidOut;
					}
				}
				else
				{
					statusItem = Db.Get().BuildingStatusItems.NeedGasOut;
				}
			}
			else
			{
				statusItem = Db.Get().BuildingStatusItems.NeedLiquidOut;
			}
			this.hasPipeGuid = this.selectable.ToggleStatusItem(statusItem, this.hasPipeGuid, !this.connected, this);
		}
	}

	private bool OutputPipeIsEmpty()
	{
		if (this.ignoreFullPipe)
		{
			return true;
		}
		bool flag = true;
		if (this.connected)
		{
			IConduitFlow conduitFlow = this.GetConduitFlow();
			flag = conduitFlow.IsConduitEmpty(this.utilityCell);
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
			StatusItem conduitBlockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
			this.pipeBlockedGuid = this.selectable.ToggleStatusItem(conduitBlockedMultiples, this.pipeBlockedGuid, !flag, null);
		}
	}

	private IConduitFlow GetConduitFlow()
	{
		switch (this.conduitType)
		{
		case ConduitType.Gas:
			return Game.Instance.gasConduitFlow;
		case ConduitType.Liquid:
			return Game.Instance.liquidConduitFlow;
		case ConduitType.Solid:
			return Game.Instance.solidConduitFlow;
		default:
			global::Debug.LogWarning("GetConduitFlow() called with unexpected conduitType: " + this.conduitType.ToString(), null);
			return null;
		}
	}

	private bool IsConnected(int cell)
	{
		return RequireOutputs.IsConnected(cell, this.conduitType);
	}

	public static bool IsConnected(int cell, ConduitType conduitType)
	{
		ObjectLayer objectLayer = ObjectLayer.NumLayers;
		if (conduitType != ConduitType.Gas)
		{
			if (conduitType != ConduitType.Liquid)
			{
				if (conduitType == ConduitType.Solid)
				{
					objectLayer = ObjectLayer.SolidConduit;
				}
			}
			else
			{
				objectLayer = ObjectLayer.LiquidConduit;
			}
		}
		else
		{
			objectLayer = ObjectLayer.GasConduit;
		}
		GameObject gameObject = Grid.Objects[cell, (int)objectLayer];
		return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Operational operational;

	public bool ignoreFullPipe;

	private int utilityCell;

	private ConduitType conduitType;

	private static readonly Operational.Flag outputConnectedFlag = new Operational.Flag("output_connected", Operational.Flag.Type.Requirement);

	private static readonly Operational.Flag pipesHaveRoomFlag = new Operational.Flag("pipesHaveRoom", Operational.Flag.Type.Requirement);

	private bool previouslyConnected = true;

	private bool previouslyHadRoom = true;

	private bool connected;

	private Guid hasPipeGuid;

	private Guid pipeBlockedGuid;

	private HandleVector<int>.Handle partitionerEntry;
}
