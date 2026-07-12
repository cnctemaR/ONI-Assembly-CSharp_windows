using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SolidConduitDispenser")]
public class SolidConduitDispenser : KMonoBehaviour, ISaveLoadable, IConduitDispenser
{
	public Storage Storage
	{
		get
		{
			return this.storage;
		}
	}

	public ConduitType ConduitType
	{
		get
		{
			return ConduitType.Solid;
		}
	}

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
		this.utilityCell = this.GetOutputCell();
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

	private bool isSolid(GameObject o)
	{
		PrimaryElement component = o.GetComponent<PrimaryElement>();
		return component == null || component.Element.IsLiquid || component.Element.IsGas;
	}

	private Pickupable FindSuitableItem()
	{
		List<GameObject> list = this.storage.items;
		if (this.solidOnly)
		{
			List<GameObject> list2 = new List<GameObject>(list);
			list2.RemoveAll(new Predicate<GameObject>(this.isSolid));
			list = list2;
		}
		if (list.Count < 1)
		{
			return null;
		}
		this.round_robin_index %= list.Count;
		GameObject gameObject = list[this.round_robin_index];
		this.round_robin_index++;
		if (!gameObject)
		{
			return null;
		}
		return gameObject.GetComponent<Pickupable>();
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
		SolidConduit solidConduit = ((gameObject != null) ? gameObject.GetComponent<SolidConduit>() : null);
		UtilityNetwork utilityNetwork = ((solidConduit != null) ? solidConduit.GetNetwork() : null);
		if (utilityNetwork == null)
		{
			return -1;
		}
		return utilityNetwork.id;
	}

	private int GetOutputCell()
	{
		Building component = base.GetComponent<Building>();
		if (this.useSecondaryOutput)
		{
			foreach (ISecondaryOutput secondaryOutput in base.GetComponents<ISecondaryOutput>())
			{
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Solid))
				{
					return Grid.OffsetCell(component.NaturalBuildingCell(), secondaryOutput.GetSecondaryConduitOffset(ConduitType.Solid));
				}
			}
			return Grid.OffsetCell(component.NaturalBuildingCell(), CellOffset.none);
		}
		return component.GetUtilityOutputCell();
	}

	[SerializeField]
	public SimHashes[] elementFilter;

	[SerializeField]
	public bool invertElementFilter;

	[SerializeField]
	public bool alwaysDispense;

	[SerializeField]
	public bool useSecondaryOutput;

	[SerializeField]
	public bool solidOnly;

	private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private bool dispensing;

	private int round_robin_index;
}
