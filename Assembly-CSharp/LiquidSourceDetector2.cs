using System;
using System.Collections.Generic;

public class LiquidSourceDetector2 : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		LiquidSourceDetector2.Instance = this;
		for (int i = 0; i < Grid.CellCount; i++)
		{
			this.Refresh(i);
		}
		World instance = World.Instance;
		instance.OnLiquidChanged = (Action<int>)Delegate.Combine(instance.OnLiquidChanged, new Action<int>(this.OnLiquidChanged));
	}

	private void OnLiquidChanged(int cell)
	{
		this.Refresh(cell);
		for (int i = 0; i < LiquidSourceDetector2.workTestOffsets.Length; i++)
		{
			int num = Grid.OffsetCell(cell, LiquidSourceDetector2.workTestOffsets[i]);
			if (Grid.IsValidCell(num))
			{
				this.Refresh(num);
			}
		}
	}

	private void Refresh(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		if (Grid.Element[cell].IsLiquid)
		{
			for (int i = 0; i < LiquidSourceDetector2.workTestOffsets.Length; i++)
			{
				int num = Grid.OffsetCell(cell, LiquidSourceDetector2.workTestOffsets[i]);
				if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.Element[num].IsLiquid)
				{
					if (!this.potentialCells.Contains(cell))
					{
						this.potentialCells.Add(cell);
					}
					break;
				}
			}
		}
		else
		{
			this.potentialCells.Remove(cell);
			this.RemoveSource(cell);
		}
	}

	private void RemoveSource(int cell)
	{
		LiquidSource liquidSource = null;
		this.sources.TryGetValue(cell, out liquidSource);
		if (liquidSource != null)
		{
			liquidSource.gameObject.DeleteObject();
			this.sources.Remove(cell);
		}
	}

	private void Update()
	{
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		PathProber pathProber = MinionGroupProber.Get().GetPathProber();
		int num = Math.Min(100, this.potentialCells.Count);
		for (int i = 0; i < num; i++)
		{
			int num2 = this.lastUpdateIdx++ % this.potentialCells.Count;
			int num3 = this.potentialCells[num2];
			this.tempOffsets.Clear();
			OffsetTableTracker.GetOffsets(num3, OffsetGroups.InvertedStandardTable, navGrid, this.tempOffsets);
			if (this.tempOffsets.Count > 0)
			{
				bool flag = false;
				if (Grid.Element[num3].IsLiquid)
				{
					for (int j = 0; j < this.tempOffsets.Count; j++)
					{
						if (pathProber.GetCost(Grid.OffsetCell(num3, this.tempOffsets[j])) != PathProber.InvalidCost)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					LiquidSource liquidSource = null;
					this.sources.TryGetValue(num3, out liquidSource);
					if (liquidSource == null)
					{
						liquidSource = LiquidSourceManager.Instance.CreateSource(Grid.Element[num3]);
						liquidSource.transform.position = Grid.CellToPosCBC(num3, Grid.SceneLayer.Move);
						this.sources[num3] = liquidSource;
						liquidSource.GetComponent<Pickupable>().SetOffsets(this.tempOffsets.ToArray());
					}
					else
					{
						Pickupable component = liquidSource.GetComponent<Pickupable>();
						CellOffset[] offsets = component.GetOffsets();
						if (offsets.Length != this.tempOffsets.Count)
						{
							component.SetOffsets(this.tempOffsets.ToArray());
						}
						else
						{
							for (int k = 0; k < offsets.Length; k++)
							{
								if (offsets[k] != this.tempOffsets[k])
								{
									component.SetOffsets(this.tempOffsets.ToArray());
									break;
								}
							}
						}
					}
				}
				else
				{
					this.RemoveSource(num3);
				}
			}
			else
			{
				this.RemoveSource(num3);
			}
		}
	}

	public bool IsLiquidAccessible(Element element)
	{
		foreach (KeyValuePair<int, LiquidSource> keyValuePair in this.sources)
		{
			if (Grid.Element[keyValuePair.Key].id == element.id)
			{
				return true;
			}
		}
		return false;
	}

	private static CellOffset[] workTestOffsets = new CellOffset[]
	{
		new CellOffset(-1, 0),
		new CellOffset(0, -1),
		new CellOffset(1, 0),
		new CellOffset(0, 1)
	};

	private List<int> potentialCells = new List<int>();

	public Dictionary<int, LiquidSource> sources = new Dictionary<int, LiquidSource>();

	public static LiquidSourceDetector2 Instance;

	private int lastUpdateIdx;

	private List<CellOffset> tempOffsets = new List<CellOffset>();
}
