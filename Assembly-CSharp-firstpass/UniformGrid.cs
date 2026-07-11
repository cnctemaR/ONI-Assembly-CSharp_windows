using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformGrid<T> where T : IUniformGridObject
{
	public UniformGrid()
	{
	}

	public UniformGrid(int width, int height, int cellWidth, int cellHeight)
	{
		this.Reset(width, height, cellWidth, cellHeight);
	}

	public void Reset(int width, int height, int cellWidth, int cellHeight)
	{
		this.cellWidth = cellWidth;
		this.cellHeight = cellHeight;
		this.numXCells = (int)Math.Ceiling((double)((float)width / (float)cellWidth));
		this.numYCells = (int)Math.Ceiling((double)((float)height / (float)cellHeight));
		int num = this.numXCells * this.numYCells;
		this.cells = new List<T>[num];
		this.items = new List<T>();
	}

	public void Clear()
	{
		this.cellWidth = 0;
		this.cellHeight = 0;
		this.numXCells = 0;
		this.numYCells = 0;
		this.cells = null;
	}

	public void Add(T item)
	{
		ref Vector2 ptr = item.PosMin();
		Vector2 vector = item.PosMax();
		int num = (int)Math.Max(ptr.x / (float)this.cellWidth, 0f);
		int num2 = (int)Math.Max(vector.y / (float)this.cellHeight, 0f);
		int num3 = Math.Min(this.numXCells - 1, (int)Math.Ceiling((double)(vector.x / (float)this.cellWidth)));
		int num4 = Math.Min(this.numYCells - 1, (int)Math.Ceiling((double)(vector.y / (float)this.cellHeight)));
		for (int i = num2; i <= num4; i++)
		{
			for (int j = num; j <= num3; j++)
			{
				int num5 = i * this.numXCells + j;
				List<T> list = this.cells[num5];
				if (list == null)
				{
					list = new List<T>();
					this.cells[num5] = list;
				}
				list.Add(item);
				this.items.Add(item);
			}
		}
	}

	public void Remove(T item)
	{
		ref Vector2 ptr = item.PosMin();
		Vector2 vector = item.PosMax();
		int num = (int)Math.Max(ptr.x / (float)this.cellWidth, 0f);
		int num2 = (int)Math.Max(vector.y / (float)this.cellHeight, 0f);
		int num3 = Math.Min(this.numXCells - 1, (int)Math.Ceiling((double)(vector.x / (float)this.cellWidth)));
		int num4 = Math.Min(this.numYCells - 1, (int)Math.Ceiling((double)(vector.y / (float)this.cellHeight)));
		for (int i = num2; i <= num4; i++)
		{
			for (int j = num; j <= num3; j++)
			{
				List<T> list = this.cells[i * this.numXCells + j];
				if (list != null && list.IndexOf(item) != -1)
				{
					list.Remove(item);
					this.items.Remove(item);
				}
			}
		}
	}

	public IEnumerable GetAllIntersecting(IUniformGridObject item)
	{
		Vector2 vector = item.PosMin();
		Vector2 vector2 = item.PosMax();
		return this.GetAllIntersecting(vector, vector2);
	}

	public IEnumerable GetAllIntersecting(Vector2 pos)
	{
		return this.GetAllIntersecting(pos, pos);
	}

	public IEnumerable GetAllIntersecting(Vector2 min, Vector2 max)
	{
		HashSet<T> hashSet = new HashSet<T>();
		this.GetAllIntersecting(min, max, hashSet);
		return hashSet;
	}

	public void GetAllIntersecting(Vector2 min, Vector2 max, ICollection<T> results)
	{
		int num = Math.Max(0, Math.Min((int)(min.x / (float)this.cellWidth), this.numXCells - 1));
		int num2 = Math.Max(0, Math.Min((int)Math.Ceiling((double)(max.x / (float)this.cellWidth)), this.numXCells - 1));
		int num3 = Math.Max(0, Math.Min((int)(min.y / (float)this.cellHeight), this.numYCells - 1));
		int num4 = Math.Max(0, Math.Min((int)Math.Ceiling((double)(max.y / (float)this.cellHeight)), this.numYCells - 1));
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				List<T> list = this.cells[i * this.numXCells + j];
				if (list != null)
				{
					for (int k = 0; k < list.Count; k++)
					{
						results.Add(list[k]);
					}
				}
			}
		}
	}

	public ICollection<T> GetAllItems()
	{
		return this.items;
	}

	private List<T>[] cells;

	private List<T> items;

	private int cellWidth;

	private int cellHeight;

	private int numXCells;

	private int numYCells;
}
