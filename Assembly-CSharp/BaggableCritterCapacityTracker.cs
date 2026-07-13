using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class BaggableCritterCapacityTracker : KMonoBehaviour, ISim1000ms, IUserControlledCapacity
{
	[Serialize]
	public int creatureLimit { get; set; } = 20;

	public int storedCreatureCount { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		this.cavityCell = Grid.OffsetCell(num, this.cavityOffset);
		this.filter = base.GetComponent<TreeFilterable>();
		TreeFilterable treeFilterable = this.filter;
		treeFilterable.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<HashSet<Tag>>(this.RefreshCreatureCount));
		base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
		if (this.requireLiquidOffset)
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("BaggableCritterCapacityTracker.OnSpawn", base.gameObject, new Extents(this.cavityCell, new CellOffset[]
			{
				new CellOffset(0, 0)
			}), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnLiquidChanged));
			this.OnLiquidChanged(null);
			return;
		}
		base.Subscribe(144050788, new Action<object>(this.RefreshCreatureCount));
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (BaggableCritterCapacityTracker.capacityStatusItem == null)
		{
			BaggableCritterCapacityTracker.capacityStatusItem = new StatusItem("CritterCapacity", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			BaggableCritterCapacityTracker.capacityStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				IUserControlledCapacity userControlledCapacity = (IUserControlledCapacity)data;
				string text = Util.FormatWholeNumber(Mathf.Floor(userControlledCapacity.AmountStored));
				string text2 = Util.FormatWholeNumber(userControlledCapacity.UserMaxCapacity);
				str = str.Replace("{Stored}", text).Replace("{StoredUnits}", ((int)userControlledCapacity.AmountStored == 1) ? BUILDING.STATUSITEMS.CRITTERCAPACITY.UNIT : BUILDING.STATUSITEMS.CRITTERCAPACITY.UNITS).Replace("{Capacity}", text2)
					.Replace("{CapacityUnits}", ((int)userControlledCapacity.UserMaxCapacity == 1) ? BUILDING.STATUSITEMS.CRITTERCAPACITY.UNIT : BUILDING.STATUSITEMS.CRITTERCAPACITY.UNITS);
				return str;
			};
		}
		this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, BaggableCritterCapacityTracker.capacityStatusItem, this);
	}

	protected override void OnCleanUp()
	{
		TreeFilterable treeFilterable = this.filter;
		treeFilterable.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Remove(treeFilterable.OnFilterChanged, new Action<HashSet<Tag>>(this.RefreshCreatureCount));
		base.Unsubscribe(144050788);
		base.OnCleanUp();
	}

	private void OnLiquidChanged(object data)
	{
		if (this.requireLiquidOffset)
		{
			bool flag = Grid.IsLiquid(this.cavityCell);
			if (flag)
			{
				this.RefreshCreatureCount(null);
			}
			this.operational.SetFlag(BaggableCritterCapacityTracker.isInLiquid, flag);
			this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NotSubmerged, !flag, this);
			this.selectable.ToggleStatusItem(BaggableCritterCapacityTracker.capacityStatusItem, flag, this);
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		BaggableCritterCapacityTracker component = gameObject.GetComponent<BaggableCritterCapacityTracker>();
		if (component == null)
		{
			return;
		}
		this.creatureLimit = component.creatureLimit;
	}

	public void RefreshCreatureCount(object data = null)
	{
		int storedCreatureCount = this.storedCreatureCount;
		if (this.requireLiquidOffset)
		{
			this.storedCreatureCount = this.RefreshSwimmingCreatureCount();
		}
		else
		{
			this.storedCreatureCount = this.RefreshOtherCreatureCount();
		}
		if (this.onCountChanged != null && this.storedCreatureCount != storedCreatureCount)
		{
			this.onCountChanged();
		}
	}

	private int RefreshOtherCreatureCount()
	{
		int num = 0;
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(this.cavityCell);
		if (cavityForCell != null)
		{
			foreach (KPrefabID kprefabID in cavityForCell.creatures)
			{
				if (!kprefabID.HasTag(GameTags.Creatures.Bagged) && !kprefabID.HasTag(GameTags.Trapped) && (!this.filteredCount || this.filter.AcceptedTags.Contains(kprefabID.PrefabTag)))
				{
					num++;
				}
			}
		}
		return num;
	}

	private int RefreshSwimmingCreatureCount()
	{
		return FishOvercrowingManager.Instance.GetFishCavityCount(this.cavityCell, this.filter.AcceptedTags);
	}

	public void Sim1000ms(float dt)
	{
		this.RefreshCreatureCount(null);
	}

	float IUserControlledCapacity.UserMaxCapacity
	{
		get
		{
			return (float)this.creatureLimit;
		}
		set
		{
			this.creatureLimit = Mathf.RoundToInt(value);
			if (this.onCountChanged != null)
			{
				this.onCountChanged();
			}
		}
	}

	float IUserControlledCapacity.AmountStored
	{
		get
		{
			return (float)this.storedCreatureCount;
		}
	}

	float IUserControlledCapacity.MinCapacity
	{
		get
		{
			return 0f;
		}
	}

	float IUserControlledCapacity.MaxCapacity
	{
		get
		{
			return (float)this.maximumCreatures;
		}
	}

	bool IUserControlledCapacity.WholeValues
	{
		get
		{
			return true;
		}
	}

	LocString IUserControlledCapacity.CapacityUnits
	{
		get
		{
			return UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.UNITS_SUFFIX;
		}
	}

	public int maximumCreatures = 40;

	public bool requireLiquidOffset;

	public CellOffset cavityOffset;

	public bool filteredCount;

	public global::System.Action onCountChanged;

	private int cavityCell;

	[MyCmpReq]
	private TreeFilterable filter;

	[MyCmpGet]
	private Operational operational;

	private static readonly Operational.Flag isInLiquid = new Operational.Flag("isInLiquid", Operational.Flag.Type.Requirement);

	[MyCmpGet]
	private KSelectable selectable;

	private static StatusItem capacityStatusItem;

	private HandleVector<int>.Handle partitionerEntry;
}
