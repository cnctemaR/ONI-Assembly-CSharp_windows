using System;

public class ResourceTracker : WorldTracker
{
	public Tag tag { get; private set; }

	public ResourceTracker(int worldID, Tag materialCategoryTag)
		: base(worldID)
	{
		this.tag = materialCategoryTag;
	}

	public override void UpdateData()
	{
		if (ClusterManager.Instance.GetWorld(base.WorldID).worldInventory == null)
		{
			return;
		}
		base.AddPoint(ClusterManager.Instance.GetWorld(base.WorldID).worldInventory.GetAmount(this.tag, false));
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
	}
}
