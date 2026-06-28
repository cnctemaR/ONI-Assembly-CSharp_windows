using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyOfWater : KMonoBehaviour
{
	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
		base.OnCleanUp();
	}

	public void Setup(List<int> WaterCells, List<int> WorkPoints)
	{
		this.waterCells = WaterCells;
		this.workPoints = WorkPoints;
		if (this.containedObjects == null || this.waterCells.Count == 0 || this.containedObjects[0] == null)
		{
			WaterBodyProbe.Instance.BodiesToDestroy.Add(this);
		}
		else
		{
			float num = this.containedObjects[0].transform.position.x;
			float num2 = this.containedObjects[0].transform.position.x;
			float num3 = this.containedObjects[0].transform.position.y;
			float num4 = this.containedObjects[0].transform.position.y;
			foreach (int num5 in this.waterCells)
			{
				Vector3 vector = Grid.CellToPos(num5);
				if (vector.x < num)
				{
					num = vector.x;
				}
				if (vector.x > num2)
				{
					num2 = vector.x;
				}
				if (vector.y < num3)
				{
					num3 = vector.y;
				}
				if (vector.y > num4)
				{
					num4 = vector.y;
				}
			}
			this.extents = new Extents((int)num - 1, (int)num3 - 1, (int)(num2 - num) + 1, (int)(num4 - num3) + 1);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("BodyOfWater.Setup", base.gameObject, this.extents, GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.ReevaluateBody));
		}
	}

	public void ToggleFishingTask()
	{
	}

	public Catchable[] GetCatchables()
	{
		List<Catchable> list = new List<Catchable>();
		foreach (GameObject gameObject in this.containedObjects)
		{
			if (!(gameObject == null))
			{
				Catchable component = gameObject.GetComponent<Catchable>();
				if (component != null)
				{
					list.Add(component);
				}
			}
		}
		return list.ToArray();
	}

	public void AddObjectToBody(GameObject go)
	{
		if (!(go == null))
		{
			if (!this.containedObjects.Contains(go))
			{
				this.containedObjects.Add(go);
				if (go.GetComponent<Catchable>())
				{
					go.GetComponent<Catchable>().body = this;
				}
			}
			WaterBodyProbe.Instance.RefreshBody(this);
		}
	}

	public void RemoveObjectFromBody(GameObject go)
	{
		if (this.containedObjects.Contains(go))
		{
			this.containedObjects.Remove(go);
			Catchable component = go.GetComponent<Catchable>();
			if (component != null && component.body == this)
			{
				component.body = null;
			}
		}
		WaterBodyProbe.Instance.RefreshBody(this);
	}

	private void ReevaluateBody(object param)
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		WaterBodyProbe.Instance.MarkBodyDirty(this);
	}

	public static void MakeSplash(Vector3 pos)
	{
		int num = global::UnityEngine.Random.Range(0, BodyOfWater.splashes.Length);
		SpriteSheetAnimManager.instance.Play(BodyOfWater.splashes[num], pos, Vector2.one, Color.white);
	}

	private void OnDrawGizmos()
	{
		if (this.DebugDraw)
		{
			foreach (int num in this.waterCells)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(Grid.CellToPosCCC(num, Grid.SceneLayer.Move), new Vector3(1f, 1f, 1f));
			}
		}
	}

	public List<int> waterCells = new List<int>();

	public List<int> workPoints = new List<int>();

	public List<GameObject> containedObjects = new List<GameObject>();

	public Extents extents;

	public bool DebugDraw = false;

	private GameScenePartitionerEntry partitionerEntry;

	public static string[] splashes = new string[] { "liquid_splash1", "liquid_splash1", "liquid_splash3", "liquid_splash4" };
}
