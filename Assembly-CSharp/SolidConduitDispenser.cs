using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitDispenser : KMonoBehaviour, ISaveLoadable
{
	public SolidConduitFlow.ConduitContents ConduitContents
	{
		get
		{
			return this.GetConduitFlow().GetContents(this.utilityCell);
		}
	}

	public bool IsDispensing
	{
		get
		{
			return this.dispensing;
		}
	}

	public SolidConduitFlow GetConduitFlow()
	{
		return Game.Instance.solidConduitFlow;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.utilityCell = base.GetComponent<Building>().GetUtilityOutputCell();
		ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[20];
		this.partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, scenePartitionerLayer, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitFlow().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Dispense);
		this.OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		this.GetConduitFlow().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object data)
	{
		this.dispensing = this.dispensing && this.IsConnected;
		base.Trigger(-2094018600, this.IsConnected);
	}

	private void ConduitUpdate(float dt)
	{
		bool flag = false;
		this.operational.SetFlag(SolidConduitDispenser.outputConduitFlag, this.IsConnected);
		if (this.operational.IsOperational || this.alwaysDispense)
		{
			SolidConduitFlow conduitFlow = this.GetConduitFlow();
			if (conduitFlow.HasConduit(this.utilityCell) && conduitFlow.IsConduitEmpty(this.utilityCell))
			{
				Pickupable pickupable = this.FindSuitableItem();
				if (pickupable)
				{
					if (pickupable.PrimaryElement.Mass > 20f)
					{
						pickupable = pickupable.Take(20f);
					}
					conduitFlow.AddPickupable(this.utilityCell, pickupable);
					flag = true;
				}
			}
		}
		this.storage.storageNetworkID = this.GetConnectedNetworkID();
		this.dispensing = flag;
	}

	private Pickupable FindSuitableItem()
	{
		List<GameObject> items = this.storage.items;
		if (items.Count < 1)
		{
			return null;
		}
		this.round_robin_index %= items.Count;
		GameObject gameObject = items[this.round_robin_index];
		this.round_robin_index++;
		return (!gameObject) ? null : gameObject.GetComponent<Pickupable>();
	}

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[this.utilityCell, 20];
			return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
		}
	}

	private int GetConnectedNetworkID()
	{
		GameObject gameObject = Grid.Objects[this.utilityCell, 20];
		SolidConduit solidConduit = ((!(gameObject != null)) ? null : gameObject.GetComponent<SolidConduit>());
		UtilityNetwork utilityNetwork = ((!(solidConduit != null)) ? null : solidConduit.GetNetwork());
		return (utilityNetwork == null) ? (-1) : utilityNetwork.id;
	}

	[SerializeField]
	public SimHashes[] elementFilter;

	[SerializeField]
	public bool invertElementFilter;

	[SerializeField]
	public bool alwaysDispense;

	private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private bool dispensing;

	private int round_robin_index;

	private const float MaxMass = 20f;
}
