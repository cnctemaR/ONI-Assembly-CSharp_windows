using System;
using UnityEngine;

public class Polluter : IPolluter
{
	public Polluter(int radius)
	{
		this.radius = radius;
	}

	public int radius
	{
		get
		{
			return this._radius;
		}
		private set
		{
			this._radius = value;
			if (this._radius == 0)
			{
				global::Debug.LogFormat("[{0}] has a 0 radius noise, this will disable it", new object[] { this.GetName() });
				return;
			}
			int num = (2 * this._radius + 5) * (2 * this._radius + 5);
			if (this.cells.Length < num)
			{
				this.cells = new Pair<int, int>[num];
			}
		}
	}

	public void SetAttributes(Vector2 pos, int dB, string name)
	{
		this.position = pos;
		this.sourceName = name;
		this.noise = dB;
	}

	public string GetName()
	{
		return this.sourceName;
	}

	public int GetRadius()
	{
		return this.radius;
	}

	public int GetNoise()
	{
		return this.noise;
	}

	public int GetCellCount()
	{
		return this.cellCount;
	}

	public void AddCell(Pair<int, int> cell)
	{
		this.cells[this.cellCount++] = cell;
	}

	public Pair<int, int> GetCell(int index)
	{
		return this.cells[index];
	}

	public void Clear()
	{
		this.cellCount = 0;
	}

	public Vector2 GetPosition()
	{
		return this.position;
	}

	private int _radius;

	private int noise;

	private int cellCount;

	private Vector2 position;

	private string sourceName;

	private Pair<int, int>[] cells = new Pair<int, int>[0];
}
