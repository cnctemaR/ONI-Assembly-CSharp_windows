using System;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementFilter : KMonoBehaviour, ISaveLoadableJson
{
	public SimHashes FilteredElement
	{
		get
		{
			return this.filteredElem;
		}
	}

	public Vent.Transfer TransferType
	{
		get
		{
			return this.transferType;
		}
	}

	public Vent.Transfer GetTransferType()
	{
		return this.transferType;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vent.Transfer transfer = this.transferType;
		if (transfer != Vent.Transfer.Gas)
		{
			if (transfer == Vent.Transfer.Liquid)
			{
				this.filterable = base.GetComponent<LiquidFilterable>();
			}
		}
		else
		{
			this.filterable = base.GetComponent<GasFilterable>();
			if (this.filteredTag == GameTags.Water)
			{
				this.filteredTag = GameTags.Oxygen;
			}
		}
		this.filteredVent = base.gameObject.AddComponent<Vent>();
		this.filteredVent.transferType = this.transferType;
		this.filteredVent.endpointType = Vent.Endpoint.Source;
		this.filteredVent.DynamicOffset = new CellOffset(-1, 0);
		this.handleOutput = HandleVector<ConduitFlow.BuildingConduit>.InvalidHandle;
		this.handleFilter = HandleVector<ConduitFlow.BuildingConduit>.InvalidHandle;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<ConduitConsumer>().isConsuming = false;
		this.OnFilterChanged(ElementLoader.FindElementByHash(this.filteredElem).tag);
		this.filterable.onFilterChanged += this.OnFilterChanged;
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.transferType);
		conduitFlowManager.AddConduitUpdater(new Action<float>(this.OnConduitTick), 0);
		this.conduitOutput = new ConduitFlow.BuildingConduit(conduitFlowManager);
		this.conduitFilter = new ConduitFlow.BuildingConduit(conduitFlowManager);
		this.Subscribe(-1305509372, new EventSystem.EventHandler(this.OnVentCellUpdated));
	}

	protected override void OnCleanUp()
	{
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.transferType);
		conduitFlowManager.RemoveBuildingConduit(this.handleOutput);
		conduitFlowManager.RemoveBuildingConduit(this.handleFilter);
		IUtilityNetworkMgr networkManager = Game.Instance.GetNetworkManager(this.transferType);
		networkManager.RemoveFromNetworks(this.outputCell, this.itemOutput);
		networkManager.RemoveFromNetworks(this.filteredCell, this.itemFilter);
		conduitFlowManager.RemoveConduitUpdater(new Action<float>(this.OnConduitTick));
		base.OnCleanUp();
	}

	private void OnVentCellUpdated(object data)
	{
		this.UpdateVentCells();
	}

	private void UpdateVentCells()
	{
		this.inputCell = this.building.GetUtilityInputCell();
		this.outputCell = this.building.GetUtilityOutputCell();
		this.filteredCell = this.filteredVent.Cell;
		this.conduitOutput.cell = this.outputCell;
		this.conduitFilter.cell = this.filteredCell;
		IUtilityNetworkMgr networkManager = Game.Instance.GetNetworkManager(this.transferType);
		if (this.itemInput != null)
		{
			networkManager.RemoveFromNetworks(this.itemInput.Cell, this.itemInput);
		}
		if (this.itemOutput != null)
		{
			networkManager.RemoveFromNetworks(this.itemOutput.Cell, this.itemOutput);
		}
		if (this.itemFilter != null)
		{
			networkManager.RemoveFromNetworks(this.itemFilter.Cell, this.itemFilter);
		}
		Vent[] components = base.GetComponents<Vent>();
		Vent input_vent = null;
		Vent output_vent = null;
		foreach (Vent vent in components)
		{
			if (vent.endpointType == Vent.Endpoint.Sink)
			{
				input_vent = vent;
			}
			else if (vent != this.filteredVent)
			{
				output_vent = vent;
			}
		}
		Action<FlowUtilityNetwork> action = delegate(FlowUtilityNetwork network)
		{
			input_vent.network = network;
		};
		this.itemInput = new FlowUtilityNetwork.NetworkItem(this.transferType, Vent.Endpoint.Sink, this.inputCell, 1000, action);
		this.itemOutput = new FlowUtilityNetwork.NetworkItem(this.transferType, Vent.Endpoint.Source, this.outputCell, 0, delegate(FlowUtilityNetwork network)
		{
			output_vent.network = network;
		});
		this.itemFilter = new FlowUtilityNetwork.NetworkItem(this.transferType, Vent.Endpoint.Source, this.filteredCell, 0, delegate(FlowUtilityNetwork network)
		{
			this.filteredVent.network = network;
		});
		networkManager.AddToNetworks(this.inputCell, this.itemInput);
		networkManager.AddToNetworks(this.outputCell, this.itemOutput);
		networkManager.AddToNetworks(this.filteredCell, this.itemFilter);
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.transferType);
		if (!this.handleOutput.IsValid())
		{
			this.handleOutput = conduitFlowManager.AddBuildingConduit(this.conduitOutput);
		}
		if (!this.handleFilter.IsValid())
		{
			this.handleFilter = conduitFlowManager.AddBuildingConduit(this.conduitFilter);
		}
		conduitFlowManager.ForceRebuildNetworks();
	}

	private void OnConduitTick(float dt)
	{
		bool flag = false;
		if (this.operational.IsOperational)
		{
			ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.transferType);
			ConduitFlow.ConduitContents contents = conduitFlowManager.GetContents(this.inputCell);
			if (contents.mass > 0f)
			{
				flag = true;
				ConduitFlow.BuildingConduit buildingConduit = ((contents.element == this.filteredElem) ? this.conduitFilter : this.conduitOutput);
				if (buildingConduit.GetContents().element == SimHashes.Vacuum)
				{
					buildingConduit.SetContents(contents);
					conduitFlowManager.RemoveElement(this.inputCell, contents.mass);
				}
			}
		}
		this.operational.SetActive(flag, false);
	}

	private void OnFilterChanged(Tag tag)
	{
		this.filteredTag = tag;
		Element element = ElementLoader.GetElement(this.filteredTag);
		if (element != null)
		{
			this.filteredElem = element.id;
			if (this.filteredElem == SimHashes.Void || this.filteredElem == SimHashes.Vacuum)
			{
				base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, null);
			}
			else
			{
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected);
			}
			return;
		}
		throw new ArgumentException("Invalid element to filter by");
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		Element element = ElementLoader.GetElement(this.filteredTag);
		if (element != null)
		{
			this.filterable.SelectedTag = this.filteredTag;
		}
	}

	[SerializeField]
	public Vent.Transfer transferType = Vent.Transfer.Liquid;

	[Serialize]
	private Tag filteredTag = GameTags.Water;

	private SimHashes filteredElem = SimHashes.Void;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	public Filterable filterable;

	private Vent filteredVent;

	private int inputCell = -1;

	private int outputCell = -1;

	private int filteredCell = -1;

	[Serialize]
	private ConduitFlow.BuildingConduit conduitOutput;

	[Serialize]
	private ConduitFlow.BuildingConduit conduitFilter;

	private HandleVector<ConduitFlow.BuildingConduit>.Handle handleOutput;

	private HandleVector<ConduitFlow.BuildingConduit>.Handle handleFilter;

	private FlowUtilityNetwork.NetworkItem itemInput;

	private FlowUtilityNetwork.NetworkItem itemOutput;

	private FlowUtilityNetwork.NetworkItem itemFilter;
}
