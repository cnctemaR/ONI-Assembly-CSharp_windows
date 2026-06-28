using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformGrid<T> where T : IUniformGridObject
{
	public void Reset(int width, int height, int cellWidth, int cellHeight)
	{
		this.cellWidth = cellWidth;
		this.cellHeight = cellHeight;
		this.numXCells = (int)Math.Ceiling((double)((float)width / (float)cellWidth));
		this.numYCells = (int)Math.Ceiling((double)((float)height / (float)cellHeight));
		int num = this.numXCells * this.numYCells;
		this.cells = new List<T>[num];
	}

	public void Add(T item)
	{
		Vector2 vector = item.PosMin();
		Vector2 vector2 = item.PosMax();
		int num = (int)(vector.x / (float)this.cellWidth);
		int num2 = (int)(vector2.y / (float)this.cellHeight);
		int num3 = Math.Min(this.numXCells - 1, (int)Math.Ceiling((double)(vector2.x / (float)this.cellWidth)));
		int num4 = Math.Min(this.numYCells - 1, (int)Math.Ceiling((double)(vector2.y / (float)this.cellHeight)));
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
		Vector2 vector = item.PosMin();
		Vector2 vector2 = item.PosMax();
		int num = (int)(vector.x / (float)this.cellWidth);
		int num2 = (int)(vector2.y / (float)this.cellHeight);
		int num3 = Math.Min(this.numXCells - 1, (int)Math.Ceiling((double)(vector2.x / (float)this.cellWidth)));
		int num4 = Math.Min(this.numYCells - 1, (int)Math.Ceiling((double)(vector2.y / (float)this.cellHeight)));
		for (int i = num2; i <= num4; i++)
		{
			for (int j = num; j <= num3; j++)
			{
				List<T> list = this.cells[i * this.numXCells + j];
				list.Remove(item);
				this.items.Remove(item);
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
		int num = Math.Min((int)(min.x / (float)this.cellWidth), this.numXCells - 1);
		int num2 = Math.Min((int)Math.Ceiling((double)(max.x / (float)this.cellWidth)), this.numXCells - 1);
		int num3 = Math.Min((int)(min.y / (float)this.cellHeight), this.numYCells - 1);
		int num4 = Math.Min((int)Math.Ceiling((double)(max.y / (float)this.cellHeight)), this.numYCells - 1);
		HashSet<T> hashSet = new HashSet<T>();
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				List<T> list = this.cells[i * this.numXCells + j];
				if (list != null)
				{
					for (int k = 0; k < list.Count; k++)
					{
						hashSet.Add(list[k]);
					}
				}
			}
		}
		return hashSet;
	}

	public ICollection<T> GetAllItems()
	{
		return this.items;
	}

	private List<T>[] cells;

	private List<T> items = new List<T>();

	private int cellWidth;

	private int cellHeight;

	private int numXCells;

	private int numYCells;
}
