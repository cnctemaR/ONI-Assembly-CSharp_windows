using System;

public class RequiresRegion : KMonoBehaviour
{
	public event Action<Region> OnOwnerRegionSet;

	public Region OwnerRegion
	{
		get
		{
			return this.ownerRegion;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.wasContained = this.IsContainedByRegion(true);
		this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedsRegion, !this.wasContained, this);
		this.schedulerHandle = UIScheduler.Instance.SchedulePeriodic("CheckOwnerRegion", 1f, new Action<object>(this.CheckOwnerRegion), null, null);
		RegionManager regionManager = Game.Instance.RegionManager;
		regionManager.OnRegionChanged = (global::System.Action)Delegate.Combine(regionManager.OnRegionChanged, new global::System.Action(this.OnRegionChanged));
	}

	protected override void OnCleanUp()
	{
		RegionManager regionManager = Game.Instance.RegionManager;
		regionManager.OnRegionChanged = (global::System.Action)Delegate.Remove(regionManager.OnRegionChanged, new global::System.Action(this.OnRegionChanged));
		base.OnCleanUp();
		this.schedulerHandle.Clear();
	}

	private void CheckOwnerRegion(object data)
	{
		bool flag = this.IsContainedByRegion(false);
		if (this.wasContained != flag)
		{
			bool flag2 = !flag;
			this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedsRegion, flag2, this);
			this.wasRegionValid = this.IsRegionValid(false);
			this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedsValidRegion, !flag2 && !this.wasRegionValid, this);
			this.wasContained = flag;
		}
		if (flag)
		{
			bool flag3 = this.IsRegionValid(false);
			if (this.wasRegionValid != flag3)
			{
				this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedsValidRegion, !flag3, this);
				this.wasRegionValid = flag3;
			}
		}
	}

	public void SetRegion(Region region, bool notify_changed)
	{
		if (notify_changed && this.OnOwnerRegionSet != null)
		{
			this.OnOwnerRegionSet(region);
		}
		this.ownerRegion = region;
	}

	public bool IsContainedByRegion(bool force_refresh = false)
	{
		return true;
	}

	public bool IsRegionValid(bool refresh = false)
	{
		return true;
	}

	private void OnRegionChanged()
	{
		this.IsContainedByRegion(true);
		this.CheckOwnerRegion(null);
		this.Trigger(-1601261024, this.ownerRegion);
	}

	[MyCmpGet]
	private KSelectable selectable;

	public TagSet RequiredRegions = new TagSet();

	private Region ownerRegion;

	private bool wasContained;

	private bool wasRegionValid = true;

	private SchedulerHandle schedulerHandle;
}
