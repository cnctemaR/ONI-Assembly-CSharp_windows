using System;
using UnityEngine;

public struct NoiseSplat
{
	public NoiseSplat(NoisePolluter setProvider, long id, float death_time = 0f)
	{
		this.id = id;
		this.deathTime = death_time;
		this.dB = 0;
		this.radius = 5;
	}

	public NoiseSplat(IPolluter setProvider, long id, float death_time = 0f)
	{
		this.id = id;
		this.deathTime = death_time;
		this.provider = setProvider;
		this.provider.Clear();
		this.position = this.provider.GetPosition();
		this.dB = 0;
	}

	public int dB { get; private set; }

	public long id { get; private set; }

	public float deathTime { get; private set; }

	public string GetName()
	{
		return this.provider.GetName();
	}

	public IPolluter GetProvider()
	{
		return this.provider;
	}

	public void Clear()
	{
		if (this.dB == 0 || this.radius == 0)
		{
			return;
		}
		this.RemoveNoise();
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		if (this.solidChangedPartitionerEntry != null)
		{
			this.solidChangedPartitionerEntry.Release();
			this.solidChangedPartitionerEntry = null;
		}
	}

	private void AddNoise()
	{
		int num = Grid.PosToCell(this.position);
		int num2 = this.effectExtence.x + this.effectExtence.width;
		int num3 = this.effectExtence.y + this.effectExtence.height;
		int num4 = this.effectExtence.x;
		int num5 = this.effectExtence.y;
		int num6 = 0;
		int num7 = 0;
		Grid.CellToXY(num, out num6, out num7);
		num2 = Math.Min(num2, Grid.WidthInCells);
		num3 = Math.Min(num3, Grid.HeightInCells);
		num4 = Math.Max(0, num4);
		num5 = Math.Max(0, num5);
		for (int i = num4; i < num2; i++)
		{
			for (int j = num5; j < num3; j++)
			{
				if (Grid.VisibilityTest(num6, num7, i, j))
				{
					int num8 = Grid.XYToCell(i, j);
					float num9 = Mathf.Floor(Vector2.Distance(this.position, Grid.CellToPos2D(num8)));
					if (i >= this.baseExtence.x && i < this.baseExtence.x + this.baseExtence.width && j >= this.baseExtence.y && j < this.baseExtence.y + this.baseExtence.height)
					{
						num9 = 0f;
					}
					int num10 = Mathf.CeilToInt((float)this.dB - (float)this.dB * num9 * 0.05f);
					if (num10 > 0)
					{
						AudioEventManager.Get().AddPollution(this.id, num8, num10);
						this.provider.AddCell(new Pair<int, int>(num8, num10));
					}
				}
			}
		}
	}

	private void RemoveNoise()
	{
		for (int i = 0; i < this.provider.GetCellCount(); i++)
		{
			Pair<int, int> cell = this.provider.GetCell(i);
			AudioEventManager.Get().RemovePollution(this.id, cell.first);
		}
	}

	public const float noiseFalloff = 0.05f;

	private IPolluter provider;

	private Vector2 position;

	private int radius;

	private Extents effectExtence;

	private Extents baseExtence;

	private GameScenePartitionerEntry partitionerEntry;

	private GameScenePartitionerEntry solidChangedPartitionerEntry;
}
