using System;

public class CropTracker : WorldTracker
{
	public CropTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		foreach (PlantablePlot plantablePlot in Components.PlantablePlots.GetItems(base.WorldID))
		{
			if (!(plantablePlot.plant == null) && plantablePlot.HasDepositTag(GameTags.CropSeed) && !plantablePlot.plant.HasTag(GameTags.Wilting))
			{
				num += 1f;
			}
		}
		base.AddPoint(num);
	}

	public override string FormatValueString(float value)
	{
		return value.ToString() + "%";
	}
}
