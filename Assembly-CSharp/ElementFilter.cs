using System;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementFilter : KMonoBehaviour, ISaveLoadable
{
	public SimHashes FilteredElement
	{
		get
		{
			return this.filteredElem;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConduitType conduitType = this.conduitType;
		if (conduitType != ConduitType.Gas)
		{
			if (conduitType == ConduitType.Liquid)
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
		this.InitializeStatusItems();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.inputCell = this.building.GetUtilityInputCell();
		this.outputCell = this.building.GetUtilityOutputCell();
		CellOffset rotatedOffset = this.building.GetRotatedOffset(this.filterOffset);
		this.filteredCell = Grid.OffsetCell(this.inputCell, rotatedOffset);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
		this.itemFilter = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Source, this.filteredCell);
		networkManager.AddToNetworks(this.filteredCell, this.itemFilter, true);
		base.GetComponent<ConduitConsumer>().isConsuming = false;
		this.OnFilterChanged(ElementLoader.FindElementByHash(this.filteredElem).tag);
		this.filterable.onFilterChanged += this.OnFilterChanged;
		ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
		flowManager.AddConduitUpdater(new Action<float>(this.OnConduitTick), ConduitFlowPriority.Default);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ElementFilter.filterStatusItem, this);
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
		networkManager.RemoveFromNetworks(this.filteredCell, this.itemFilter, true);
		ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
		flowManager.RemoveConduitUpdater(new Action<float>(this.OnConduitTick));
		base.OnCleanUp();
	}

	private void OnConduitTick(float dt)
	{
		bool flag = false;
		if (this.operational.IsOperational)
		{
			ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
			ConduitFlow.ConduitContents contents = flowManager.GetContents(this.inputCell);
			int num = ((contents.element != this.filteredElem) ? this.outputCell : this.filteredCell);
			ConduitFlow.ConduitContents contents2 = flowManager.GetContents(num);
			if (contents.mass > 0f && contents2.mass <= 0f)
			{
				flag = true;
				float num2 = flowManager.AddElement(num, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
				if (num2 > 0f)
				{
					flowManager.RemoveElement(this.inputCell, num2);
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
			bool flag = this.filteredElem == SimHashes.Void || this.filteredElem == SimHashes.Vacuum;
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, flag, null);
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

	private void InitializeStatusItems()
	{
		if (ElementFilter.filterStatusItem == null)
		{
			ElementFilter.filterStatusItem = new StatusItem("Filter", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.LiquidVentMap, true, 63486);
			ElementFilter.filterStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				if (this.filteredElem == SimHashes.Void)
				{
					str = string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, BUILDINGS.PREFABS.GASFILTER.ELEMENT_NOT_SPECIFIED);
				}
				else
				{
					Element element = ElementLoader.FindElementByHash(this.filteredElem);
					str = string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, element.name);
				}
				return str;
			};
			ElementFilter.filterStatusItem.conditionalOverlayCallback = new Func<SimViewMode, object, bool>(this.ShowInUtilityOverlay);
		}
	}

	private bool ShowInUtilityOverlay(SimViewMode mode, object data)
	{
		bool flag = false;
		ElementFilter elementFilter = (ElementFilter)data;
		ConduitType conduitType = elementFilter.conduitType;
		if (conduitType != ConduitType.Gas)
		{
			if (conduitType == ConduitType.Liquid)
			{
				flag = mode == SimViewMode.LiquidVentMap;
			}
		}
		else
		{
			flag = mode == SimViewMode.GasVentMap;
		}
		return flag;
	}

	[SerializeField]
	public ConduitType conduitType = ConduitType.Liquid;

	[SerializeField]
	public CellOffset filterOffset;

	[Serialize]
	private Tag filteredTag = GameTags.Water;

	private SimHashes filteredElem = SimHashes.Void;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	public Filterable filterable;

	private int inputCell = -1;

	private int outputCell = -1;

	private int filteredCell = -1;

	private FlowUtilityNetwork.NetworkItem itemInput;

	private FlowUtilityNetwork.NetworkItem itemOutput;

	private FlowUtilityNetwork.NetworkItem itemFilter;

	private static StatusItem filterStatusItem;
}
