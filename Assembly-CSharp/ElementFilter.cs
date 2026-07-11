using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementFilter : KMonoBehaviour, ISaveLoadable, ISecondaryOutput
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.filterable = base.GetComponent<Filterable>();
		this.InitializeStatusItems();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.inputCell = this.building.GetUtilityInputCell();
		this.outputCell = this.building.GetUtilityOutputCell();
		int num = Grid.PosToCell(base.transform.GetPosition());
		CellOffset rotatedOffset = this.building.GetRotatedOffset(this.portInfo.offset);
		this.filteredCell = Grid.OffsetCell(num, rotatedOffset);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
		this.itemFilter = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Source, this.filteredCell, base.gameObject);
		networkManager.AddToNetworks(this.filteredCell, this.itemFilter, true);
		base.GetComponent<ConduitConsumer>().isConsuming = false;
		this.OnFilterChanged(this.filterable.SelectedTag);
		this.filterable.onFilterChanged += this.OnFilterChanged;
		Conduit.GetFlowManager(this.portInfo.conduitType).AddConduitUpdater(new Action<float>(this.OnConduitTick), ConduitFlowPriority.Default);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ElementFilter.filterStatusItem, this);
		this.UpdateConduitExistsStatus();
		this.UpdateConduitBlockedStatus();
		ScenePartitionerLayer scenePartitionerLayer = null;
		switch (this.portInfo.conduitType)
		{
		case ConduitType.Gas:
			scenePartitionerLayer = GameScenePartitioner.Instance.gasConduitsLayer;
			break;
		case ConduitType.Liquid:
			scenePartitionerLayer = GameScenePartitioner.Instance.liquidConduitsLayer;
			break;
		case ConduitType.Solid:
			scenePartitionerLayer = GameScenePartitioner.Instance.solidConduitsLayer;
			break;
		}
		if (scenePartitionerLayer != null)
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("ElementFilterConduitExists", base.gameObject, this.filteredCell, scenePartitionerLayer, delegate(object data)
			{
				this.UpdateConduitExistsStatus();
			});
		}
	}

	protected override void OnCleanUp()
	{
		Conduit.GetNetworkManager(this.portInfo.conduitType).RemoveFromNetworks(this.filteredCell, this.itemFilter, true);
		Conduit.GetFlowManager(this.portInfo.conduitType).RemoveConduitUpdater(new Action<float>(this.OnConduitTick));
		if (this.partitionerEntry.IsValid() && GameScenePartitioner.Instance != null)
		{
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		}
		base.OnCleanUp();
	}

	private void OnConduitTick(float dt)
	{
		bool flag = false;
		this.UpdateConduitBlockedStatus();
		if (this.operational.IsOperational)
		{
			ConduitFlow flowManager = Conduit.GetFlowManager(this.portInfo.conduitType);
			ConduitFlow.ConduitContents contents = flowManager.GetContents(this.inputCell);
			int num = ((contents.element == this.filteredElem) ? this.filteredCell : this.outputCell);
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

	private void UpdateConduitExistsStatus()
	{
		bool flag = RequireOutputs.IsConnected(this.filteredCell, this.portInfo.conduitType);
		StatusItem statusItem;
		switch (this.portInfo.conduitType)
		{
		case ConduitType.Gas:
			statusItem = Db.Get().BuildingStatusItems.NeedGasOut;
			break;
		case ConduitType.Liquid:
			statusItem = Db.Get().BuildingStatusItems.NeedLiquidOut;
			break;
		case ConduitType.Solid:
			statusItem = Db.Get().BuildingStatusItems.NeedSolidOut;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		bool flag2 = this.needsConduitStatusItemGuid != Guid.Empty;
		if (flag == flag2)
		{
			this.needsConduitStatusItemGuid = this.selectable.ToggleStatusItem(statusItem, this.needsConduitStatusItemGuid, !flag, null);
		}
	}

	private void UpdateConduitBlockedStatus()
	{
		bool flag = Conduit.GetFlowManager(this.portInfo.conduitType).IsConduitEmpty(this.filteredCell);
		StatusItem conduitBlockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
		bool flag2 = this.conduitBlockedStatusItemGuid != Guid.Empty;
		if (flag == flag2)
		{
			this.conduitBlockedStatusItemGuid = this.selectable.ToggleStatusItem(conduitBlockedMultiples, this.conduitBlockedStatusItemGuid, !flag, null);
		}
	}

	private void OnFilterChanged(Tag tag)
	{
		bool flag = true;
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			this.filteredElem = element.id;
			flag = this.filteredElem == SimHashes.Void || this.filteredElem == SimHashes.Vacuum;
		}
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, flag, null);
	}

	private void InitializeStatusItems()
	{
		if (ElementFilter.filterStatusItem == null)
		{
			ElementFilter.filterStatusItem = new StatusItem("Filter", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID, true, 129022);
			ElementFilter.filterStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				ElementFilter elementFilter = (ElementFilter)data;
				if (elementFilter.filteredElem == SimHashes.Void)
				{
					str = string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, BUILDINGS.PREFABS.GASFILTER.ELEMENT_NOT_SPECIFIED);
				}
				else
				{
					Element element = ElementLoader.FindElementByHash(elementFilter.filteredElem);
					str = string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, element.name);
				}
				return str;
			};
			ElementFilter.filterStatusItem.conditionalOverlayCallback = new Func<HashedString, object, bool>(this.ShowInUtilityOverlay);
		}
	}

	private bool ShowInUtilityOverlay(HashedString mode, object data)
	{
		bool flag = false;
		ConduitType conduitType = ((ElementFilter)data).portInfo.conduitType;
		if (conduitType != ConduitType.Gas)
		{
			if (conduitType == ConduitType.Liquid)
			{
				flag = mode == OverlayModes.LiquidConduits.ID;
			}
		}
		else
		{
			flag = mode == OverlayModes.GasConduits.ID;
		}
		return flag;
	}

	public ConduitType GetSecondaryConduitType()
	{
		return this.portInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset()
	{
		return this.portInfo.offset;
	}

	public int GetFilteredCell()
	{
		return this.filteredCell;
	}

	[SerializeField]
	public ConduitPortInfo portInfo;

	private SimHashes filteredElem = SimHashes.Void;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private KSelectable selectable;

	public Filterable filterable;

	private Guid needsConduitStatusItemGuid;

	private Guid conduitBlockedStatusItemGuid;

	private int inputCell = -1;

	private int outputCell = -1;

	private int filteredCell = -1;

	private FlowUtilityNetwork.NetworkItem itemFilter;

	private HandleVector<int>.Handle partitionerEntry;

	private static StatusItem filterStatusItem;
}
