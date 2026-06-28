using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterBodyProbe : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		WaterBodyProbe.Instance = this;
	}

	public BodyOfWater GetLocationsWorkableBodyOfWater(int locationCell)
	{
		foreach (BodyOfWater bodyOfWater in this.knownBodies)
		{
			if (bodyOfWater.workPoints != null)
			{
				foreach (int num in bodyOfWater.workPoints)
				{
					if (num == locationCell)
					{
						return bodyOfWater;
					}
				}
			}
		}
		return null;
	}

	public BodyOfWater GetBodyIfKnown(int startCell)
	{
		foreach (BodyOfWater bodyOfWater in this.knownBodies)
		{
			if (bodyOfWater.waterCells != null)
			{
				foreach (int num in bodyOfWater.waterCells)
				{
					if (num == startCell)
					{
						return bodyOfWater;
					}
				}
			}
		}
		return null;
	}

	public void MarkBodyDirty(BodyOfWater body)
	{
		if (!this.DirtyBodies.Contains(body))
		{
			this.DirtyBodies.Add(body);
		}
	}

	private void RefreshBody(int cell, List<int> touched_cells, List<int> water_cells, List<int> work_points)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		if (Grid.Solid[cell])
		{
			return;
		}
		if (water_cells.Count > 100)
		{
			return;
		}
		if (touched_cells.Contains(cell))
		{
			return;
		}
		touched_cells.Add(cell);
		if (WaterBodyProbe.isSubstantialLiquid(cell))
		{
			water_cells.Add(cell);
			this.RefreshBody(Grid.CellLeft(cell), touched_cells, water_cells, work_points);
			this.RefreshBody(Grid.CellRight(cell), touched_cells, water_cells, work_points);
			this.RefreshBody(Grid.CellAbove(cell), touched_cells, water_cells, work_points);
			this.RefreshBody(Grid.CellBelow(cell), touched_cells, water_cells, work_points);
		}
		else
		{
			int num = 5;
			for (int i = 1; i < num; i++)
			{
				bool flag = false;
				for (int j = 0; j < i; j++)
				{
					if (Grid.Solid[Grid.OffsetCell(cell, 0, j)])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					int num2 = Grid.OffsetCell(cell, -1, i);
					int num3 = Grid.OffsetCell(cell, 1, i);
					if (Grid.IsValidCell(num3) && !Grid.Solid[num3] && !Grid.IsSubstantialLiquid(num3, 0.35f) && Grid.Solid[Grid.CellBelow(num3)] && !work_points.Contains(num3))
					{
						work_points.Add(num3);
					}
					if (Grid.IsValidCell(num2) && !Grid.Solid[num2] && !Grid.IsSubstantialLiquid(num2, 0.35f) && Grid.Solid[Grid.CellBelow(num2)] && !work_points.Contains(num2))
					{
						work_points.Add(num2);
					}
				}
			}
		}
	}

	public void RefreshBody(BodyOfWater body)
	{
		if (body == null)
		{
			return;
		}
		int num = Grid.InvalidCell;
		for (int i = body.containedObjects.Count - 1; i >= 0; i--)
		{
			if (body.containedObjects[i] != null)
			{
				num = Grid.PosToCell(body.containedObjects[i].transform.position);
				break;
			}
		}
		List<int> waterCells = body.waterCells;
		List<int> workPoints = body.workPoints;
		this.touchedCells.Clear();
		waterCells.Clear();
		workPoints.Clear();
		if (num == Grid.InvalidCell)
		{
			this.RemoveBody(body);
			return;
		}
		this.RefreshBody(num, this.touchedCells, waterCells, workPoints);
		body.Setup(waterCells, workPoints);
		for (int j = 0; j < this.knownBodies.Count; j++)
		{
			this.knownBodies[j].waterCells.Sort();
			for (int k = 0; k < this.knownBodies.Count; k++)
			{
				if (j != k)
				{
					if (!this.BodiesToDestroy.Contains(this.knownBodies[j]) && !this.BodiesToDestroy.Contains(this.knownBodies[k]))
					{
						if (this.knownBodies[j].waterCells[0] == this.knownBodies[k].waterCells[0])
						{
							foreach (GameObject gameObject in this.knownBodies[k].containedObjects)
							{
								if (!this.knownBodies[j].containedObjects.Contains(gameObject))
								{
									this.knownBodies[j].AddObjectToBody(gameObject);
								}
							}
							this.BodiesToDestroy.Add(this.knownBodies[k]);
						}
					}
				}
			}
		}
	}

	private void RemoveBody(BodyOfWater body)
	{
		if (this.knownBodies.Contains(body))
		{
			this.knownBodies.Remove(body);
		}
		Util.KDestroyGameObject(body.gameObject);
	}

	public int[] GetBodyOfWaterWorkCells(GameObject objectInBody)
	{
		int num = Grid.PosToCell(objectInBody);
		BodyOfWater bodyOfWater = this.GetBodyIfKnown(num);
		if (bodyOfWater != null)
		{
			if (!bodyOfWater.containedObjects.Contains(objectInBody))
			{
				bodyOfWater.AddObjectToBody(objectInBody);
			}
			return bodyOfWater.workPoints.ToArray();
		}
		bodyOfWater = this.FindBodyOfWater(objectInBody);
		return bodyOfWater.workPoints.ToArray();
	}

	public BodyOfWater FindBodyOfWater(GameObject objectInBody)
	{
		GameObject gameObject = new GameObject("body_of_water");
		gameObject.transform.parent = base.transform;
		gameObject.transform.SetPosition(objectInBody.transform.position);
		gameObject.AddComponent<KPrefabID>();
		BodyOfWater bodyOfWater = gameObject.AddComponent<BodyOfWater>();
		bodyOfWater.AddObjectToBody(objectInBody);
		this.RefreshBody(bodyOfWater);
		this.knownBodies.Add(bodyOfWater);
		return bodyOfWater;
	}

	private static bool isSubstantialLiquid(int cell)
	{
		return Grid.IsValidCell(cell) && ((Grid.IsValidCell(Grid.CellAbove(cell)) && Grid.IsLiquid(Grid.CellAbove(cell))) || Grid.IsSubstantialLiquid(cell, 0.75f));
	}

	private void SimUpdate(float dt)
	{
		if (this.DirtyBodies.Count > 0)
		{
			foreach (BodyOfWater bodyOfWater in this.DirtyBodies)
			{
				if (!this.BodiesToDestroy.Contains(bodyOfWater))
				{
					this.RefreshBody(bodyOfWater);
				}
			}
			this.DirtyBodies.Clear();
			base.Trigger(-263784810, null);
		}
		if (this.BodiesToDestroy.Count > 0)
		{
			for (int i = this.BodiesToDestroy.Count - 1; i >= 0; i--)
			{
				this.RemoveBody(this.BodiesToDestroy[i]);
			}
			this.BodiesToDestroy.Clear();
			base.Trigger(-263784810, null);
		}
	}

	public BodyOfWater GetBodyContainedIn(GameObject go)
	{
		foreach (BodyOfWater bodyOfWater in this.knownBodies)
		{
			if (bodyOfWater.containedObjects.Contains(go))
			{
				return bodyOfWater;
			}
		}
		return null;
	}

	public static WaterBodyProbe Instance;

	public List<BodyOfWater> DirtyBodies = new List<BodyOfWater>();

	public List<BodyOfWater> BodiesToDestroy = new List<BodyOfWater>();

	public List<BodyOfWater> knownBodies = new List<BodyOfWater>();

	private List<int> touchedCells = new List<int>();
}
