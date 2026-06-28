using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitDispenser : KMonoBehaviour, ISaveLoadableJson
{
	public void SetConduitData(ConduitType type)
	{
		this.conduitType = type;
	}

	public ConduitFlow GetConduitManager()
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

	private void OnConduitConnectionChanged(object data)
	{
		this.Trigger(-2094018600, this.IsConnected);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.utilityCell = this.building.GetUtilityOutputCell();
		int mask = GameScenePartitioner.Instance.objectLayerMasks[(this.conduitType != ConduitType.Gas) ? 12 : 10].mask;
		this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, mask, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), 0);
		this.OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		this.operational.SetFlag(ConduitDispenser.outputConduitFlag, this.IsConnected);
		if (this.operational.IsOperational)
		{
			PrimaryElement primaryElement;
			if (this.elementFilter != SimHashes.Vacuum)
			{
				primaryElement = this.storage.FindPrimaryElement(this.elementFilter);
			}
			else
			{
				primaryElement = this.storage.GetAnyChunk();
			}
			if (primaryElement != null && primaryElement.Mass > 0f)
			{
				primaryElement.KeepZeroMassObject = true;
				ConduitFlow conduitManager = this.GetConduitManager();
				float num = conduitManager.AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature);
				primaryElement.Mass -= num;
				this.Trigger(-1697596308, primaryElement.gameObject);
				Debug.Assert(primaryElement.Mass >= 0f);
			}
		}
	}

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 12 : 10];
			return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
		}
	}

	public ConduitType TypeOfConduit
	{
		get
		{
			return this.conduitType;
		}
	}

	public ConduitFlow.ConduitContents ConduitContents
	{
		get
		{
			return this.GetConduitManager().GetContents(this.utilityCell);
		}
	}

	public bool CanDispense
	{
		get
		{
			bool flag = false;
			if (this.IsConnected)
			{
				ConduitFlow conduitManager = this.GetConduitManager();
				flag = conduitManager.GetContents(this.utilityCell).mass <= 0f;
			}
			return flag;
		}
	}

	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public SimHashes elementFilter = SimHashes.Vacuum;

	public static Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Storage storage;

	private GameScenePartitionerEntry partitionerEntry;

	private int utilityCell = -1;
}
