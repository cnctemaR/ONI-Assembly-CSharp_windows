using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class SolidConduitConsumer : KMonoBehaviour
{
	public bool IsConsuming
	{
		get
		{
			return this.consuming;
		}
	}

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[this.utilityCell, 20];
			return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
		}
	}

	private SolidConduitFlow GetConduitFlow()
	{
		return Game.Instance.solidConduitFlow;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.utilityCell = this.building.GetUtilityInputCell();
		ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[20];
		this.partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, scenePartitionerLayer, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitFlow().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
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
		this.consuming = this.consuming && this.IsConnected;
		base.Trigger(-2094018600, this.IsConnected);
	}

	private void ConduitUpdate(float dt)
	{
		bool flag = false;
		SolidConduitFlow conduitFlow = this.GetConduitFlow();
		if (this.IsConnected)
		{
			SolidConduitFlow.ConduitContents contents = conduitFlow.GetContents(this.utilityCell);
			if (contents.pickupableHandle.IsValid() && (this.alwaysConsume || this.operational.IsOperational))
			{
				float num = ((!(this.capacityTag != GameTags.Any)) ? this.storage.MassStored() : this.storage.GetMassAvailable(this.capacityTag));
				float num2 = Mathf.Min(this.storage.RemainingCapacity(), this.capacityKG - num);
				Pickupable pickupable = conduitFlow.GetPickupable(contents.pickupableHandle);
				if (pickupable.PrimaryElement.Mass <= num2)
				{
					Pickupable pickupable2 = conduitFlow.RemovePickupable(this.utilityCell);
					if (pickupable2)
					{
						this.storage.Store(pickupable2.gameObject, true, false, true, false);
						flag = true;
					}
				}
			}
		}
		this.storage.storageNetworkID = this.GetConnectedNetworkID();
		this.consuming = flag;
	}

	private int GetConnectedNetworkID()
	{
		GameObject gameObject = Grid.Objects[this.utilityCell, 20];
		SolidConduit solidConduit = ((!(gameObject != null)) ? null : gameObject.GetComponent<SolidConduit>());
		UtilityNetwork utilityNetwork = ((!(solidConduit != null)) ? null : solidConduit.GetNetwork());
		return (utilityNetwork == null) ? (-1) : utilityNetwork.id;
	}

	[SerializeField]
	public Tag capacityTag = GameTags.Any;

	[SerializeField]
	public float capacityKG = float.PositiveInfinity;

	[SerializeField]
	public bool alwaysConsume;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private bool consuming;
}
