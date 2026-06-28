using System;
using System.Collections.Generic;

public class ExosuitRegion : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.region.OnRegionChanged += this.RegionChanged;
		this.RegionChanged();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.region.OnRegionChanged -= this.RegionChanged;
	}

	private void RegionChanged()
	{
		this.SetCellState(false);
		this.cells.Clear();
		this.cells.AddRange(this.region.Cells);
		this.SetCellState(true);
	}

	private void SetCellState(bool state)
	{
		foreach (int num in this.cells)
		{
			Grid.SuitRequired[num] = state;
		}
	}

	[MyCmpGet]
	private Region region;

	private List<int> cells = new List<int>();
}
