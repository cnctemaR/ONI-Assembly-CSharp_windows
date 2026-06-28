using System;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSplat : IUniformGridObject
{
	public NoiseSplat(NoisePolluter setProvider, float death_time = 0f)
	{
		this.deathTime = death_time;
		this.dB = 0;
		this.radius = 5;
		if (setProvider.dB != null)
		{
			this.dB = (int)setProvider.dB.GetTotalValue();
		}
		int num = Grid.PosToCell(setProvider.gameObject);
		if (!NoisePolluter.IsNoiseableCell(num))
		{
			this.dB = 0;
		}
		if (this.dB != 0)
		{
			setProvider.Clear();
			OccupyArea occupyArea = setProvider.occupyArea;
			this.baseExtents = occupyArea.GetExtents();
			this.provider = setProvider;
			this.position = setProvider.transform.position;
			if (setProvider.dBRadius != null)
			{
				this.radius = (int)setProvider.dBRadius.GetTotalValue();
			}
			if (this.radius != 0)
			{
				int num2 = 0;
				int num3 = 0;
				Grid.CellToXY(num, out num2, out num3);
				int widthInCells = occupyArea.GetWidthInCells();
				int heightInCells = occupyArea.GetHeightInCells();
				Vector2I vector2I = new Vector2I(num2 - this.radius, num3 - this.radius);
				Vector2I vector2I2 = vector2I + new Vector2I(this.radius * 2 + widthInCells, this.radius * 2 + heightInCells);
				vector2I = Vector2I.Max(vector2I, Vector2I.zero);
				vector2I2 = Vector2I.Min(vector2I2, new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1));
				this.effectExtents = new Extents(vector2I.x, vector2I.y, vector2I2.x - vector2I.x, vector2I2.y - vector2I.y);
				this.partitionerEntry = GameScenePartitioner.Instance.Add("NoiseSplat.SplatCollectNoisePolluters", setProvider.gameObject, this.effectExtents, GameScenePartitioner.Instance.noisePolluterLayer, setProvider.onCollectNoisePollutersCallback);
				this.solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("NoiseSplat.SplatSolidCheck", setProvider.gameObject, this.effectExtents, GameScenePartitioner.Instance.solidChangedLayer, setProvider.refreshPartionerCallback);
			}
		}
	}

	public NoiseSplat(IPolluter setProvider, float death_time = 0f)
	{
		this.deathTime = death_time;
		this.provider = setProvider;
		this.provider.Clear();
		this.position = this.provider.GetPosition();
		this.dB = this.provider.GetNoise();
		int num = Grid.PosToCell(this.position);
		if (!NoisePolluter.IsNoiseableCell(num))
		{
			this.dB = 0;
		}
		if (this.dB != 0)
		{
			this.radius = this.provider.GetRadius();
			if (this.radius != 0)
			{
				int num2 = 0;
				int num3 = 0;
				Grid.CellToXY(num, out num2, out num3);
				Vector2I vector2I = new Vector2I(num2 - this.radius, num3 - this.radius);
				Vector2I vector2I2 = vector2I + new Vector2I(this.radius * 2, this.radius * 2);
				vector2I = Vector2I.Max(vector2I, Vector2I.zero);
				vector2I2 = Vector2I.Min(vector2I2, new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1));
				this.effectExtents = new Extents(vector2I.x, vector2I.y, vector2I2.x - vector2I.x, vector2I2.y - vector2I.y);
				this.baseExtents = new Extents(num2, num3, 1, 1);
				this.AddNoise();
			}
		}
	}

	public int dB { get; private set; }

	public float deathTime { get; private set; }

	public string GetName()
	{
		return this.provider.GetName();
	}

	public IPolluter GetProvider()
	{
		return this.provider;
	}

	public Vector2 PosMin()
	{
		return new Vector2(this.position.x - (float)this.radius, this.position.y - (float)this.radius);
	}

	public Vector2 PosMax()
	{
		return new Vector2(this.position.x + (float)this.radius, this.position.y + (float)this.radius);
	}

	public void Clear()
	{
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
		this.RemoveNoise();
	}

	private void AddNoise()
	{
		int num = Grid.PosToCell(this.position);
		int num2 = this.effectExtents.x + this.effectExtents.width;
		int num3 = this.effectExtents.y + this.effectExtents.height;
		int num4 = this.effectExtents.x;
		int num5 = this.effectExtents.y;
		int num6 = 0;
		int num7 = 0;
		Grid.CellToXY(num, out num6, out num7);
		num2 = Math.Min(num2, Grid.WidthInCells);
		num3 = Math.Min(num3, Grid.HeightInCells);
		num4 = Math.Max(0, num4);
		num5 = Math.Max(0, num5);
		for (int i = num5; i < num3; i++)
		{
			for (int j = num4; j < num2; j++)
			{
				if (Grid.VisibilityTest(num6, num7, j, i))
				{
					int num8 = Grid.XYToCell(j, i);
					float dbforCell = this.GetDBForCell(num8);
					if (dbforCell > 0f)
					{
						float num9 = AudioEventManager.DBToLoudness(dbforCell);
						Grid.Loudness[num8] += num9;
						Pair<int, float> pair = new Pair<int, float>(num8, num9);
						this.decibels.Add(pair);
					}
				}
			}
		}
	}

	public float GetDBForCell(int cell)
	{
		Vector2 vector = Grid.CellToPos2D(cell);
		float num = Mathf.Floor(Vector2.Distance(this.position, vector));
		if (vector.x >= (float)this.baseExtents.x && vector.x < (float)(this.baseExtents.x + this.baseExtents.width) && vector.y >= (float)this.baseExtents.y && vector.y < (float)(this.baseExtents.y + this.baseExtents.height))
		{
			num = 0f;
		}
		float num2 = (float)this.dB - (float)this.dB * num * 0.05f;
		return Mathf.Round(num2);
	}

	private void RemoveNoise()
	{
		for (int i = 0; i < this.decibels.Count; i++)
		{
			Pair<int, float> pair = this.decibels[i];
			float num = Math.Max(0f, Grid.Loudness[pair.first] - pair.second);
			Grid.Loudness[pair.first] = ((num >= 1f) ? num : 0f);
		}
		this.decibels.Clear();
	}

	public float GetLoudness(int cell)
	{
		float num = 0f;
		for (int i = 0; i < this.decibels.Count; i++)
		{
			Pair<int, float> pair = this.decibels[i];
			if (pair.first == cell)
			{
				num = pair.second;
				break;
			}
		}
		return num;
	}

	public const float noiseFalloff = 0.05f;

	private IPolluter provider;

	private Vector2 position;

	private int radius;

	private Extents effectExtents;

	private Extents baseExtents;

	private GameScenePartitionerEntry partitionerEntry;

	private GameScenePartitionerEntry solidChangedPartitionerEntry;

	private List<Pair<int, float>> decibels = new List<Pair<int, float>>();
}
