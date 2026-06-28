using System;
using System.Collections.Generic;

public class LiquidSourceDetector2 : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		LiquidSourceDetector2.Instance = this;
		for (int i = 0; i < Grid.CellCount; i++)
		{
			if (Grid.Element[i].IsLiquid)
			{
				this.potentialCells.Add(i);
			}
		}
		World instance = World.Instance;
		instance.OnLiquidChanged = (Action<int>)Delegate.Combine(instance.OnLiquidChanged, new Action<int>(this.OnLiquidChanged));
	}

	private void OnLiquidChanged(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		if (Grid.Element[cell].IsLiquid && !this.potentialCells.Contains(cell))
		{
			this.potentialCells.Add(cell);
		}
	}

	private void RemoveSource(int cell)
	{
		LiquidSource liquidSource = null;
		this.sources.TryGetValue(cell, out liquidSource);
		if (liquidSource != null)
		{
			LiquidSourceManager.Instance.DestroySource(liquidSource);
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
			LiquidSource liquidSource = null;
			this.sources.TryGetValue(num3, out liquidSource);
			if (Grid.Element[num3].IsLiquid)
			{
				OffsetTableTracker.GetOffsets(num3, OffsetGroups.InvertedStandardTable, navGrid, this.tempOffsets);
				if (this.tempOffsets.Count > 0)
				{
					bool flag = false;
					for (int j = 0; j < this.tempOffsets.Count; j++)
					{
						if (pathProber.GetCost(Grid.OffsetCell(num3, this.tempOffsets[j])) != PathProber.InvalidCost)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
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
			else
			{
				for (int l = 0; l < this.potentialCells.Count; l++)
				{
					if (this.potentialCells[l] == num3)
					{
						this.potentialCells[l] = this.potentialCells[this.potentialCells.Count - 1];
						this.potentialCells.RemoveAt(this.potentialCells.Count - 1);
						break;
					}
				}
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

	public bool HasSource(int cell)
	{
		return this.sources.ContainsKey(cell);
	}

	private List<int> potentialCells = new List<int>();

	public Dictionary<int, LiquidSource> sources = new Dictionary<int, LiquidSource>();

	public static LiquidSourceDetector2 Instance;

	private int lastUpdateIdx;

	private List<CellOffset> tempOffsets = new List<CellOffset>();
}
