using System;
using System.Collections.Generic;
using System.Linq;
using Generated;
using UnityEngine;

public class Excavator : KMonoBehaviour
{
	public BuildingDef Def
	{
		get
		{
			return this.building.Def;
		}
	}

	private void Update()
	{
		if (!Grid.IsValidCell(Grid.PosToCell(this)))
		{
			this.DeleteObject();
		}
		if (this.armTimer > 0f)
		{
			this.armTimer -= Time.deltaTime;
			return;
		}
		float num = float.MaxValue;
		for (int i = 0; i < Components.MinionIdentities.Count; i++)
		{
			float num2 = Vector3.Distance(Components.MinionIdentities[i].gameObject.transform.position, this.transform.position);
			num = Mathf.Min(num, num2);
		}
		if (num >= this.maxRadius)
		{
			if (num <= this.maxRadius || num >= this.maxRadius + this.minSafeDistance)
			{
				if (num > this.maxRadius + this.minSafeDistance)
				{
					this.StartExplosion();
				}
			}
		}
	}

	private void PlayExplosion(Vector3 pos)
	{
		GameUtil.KInstantiate(EffectPrefabs.Instance.Explosion, pos, Grid.SceneLayer.Front, Folder.FX, null, 0);
	}

	private void StartExplosion()
	{
		this.exploding = true;
		this.startTime = Time.time + this.countDown;
	}

	private List<Vector2> DoCircularExplosion(float x, float y, float radius)
	{
		List<Vector2> circle = global::Generated.Util.GetCircle(new Vector2(x, y), (int)Mathf.Floor(radius));
		for (int i = 0; i < circle.Count; i++)
		{
			Vector3 vector = new Vector3(circle[i].x, circle[i].y, -Grid.CellSizeInMeters * 0.5f);
			int cell = Grid.PosToCell(vector);
			if (Grid.IsValidCell(cell) && Grid.Element[cell].id != SimHashes.Unobtanium)
			{
				this.PlayExplosion(vector);
				int num = -1;
				if (Grid.Solid[cell])
				{
					global::System.Action action = delegate
					{
						Substance substance = Grid.Element[cell].substance;
						Element element = ElementLoader.FindElementByHash(substance.elementID);
						if (element.IsSolid)
						{
							substance.SpawnResource(Grid.CellToPosCCC(cell, Grid.SceneLayer.Use), element.defaultValues.mass, element.defaultValues.temperature, false, false);
						}
					};
					num = Game.Instance.callbackManager.Add(action, "Excavator").index;
				}
				SimMessages.ReplaceElement(cell, SimHashes.Oxygen, CellEventLogger.Instance.Excavator, 8000f, 2000f, num);
			}
		}
		return circle;
	}

	private List<int> GetNeighbors(int cell)
	{
		List<int> list = new List<int>();
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				if (Vector3.Distance(Grid.CellToPos(cell, (float)i, (float)j, 0f), this.startLocation) < this.maxRadius * this.maxRadius)
				{
					CellOffset cellOffset = new CellOffset(i, j);
					int num = Grid.OffsetCell(cell, cellOffset);
					bool flag = false;
					for (int k = 0; k < this.visitedPoints.Count; k++)
					{
						if (this.visitedPoints[k].Key == num)
						{
							flag = true;
							break;
						}
					}
					if (Grid.IsValidCell(num))
					{
						if (Grid.Element[num].id != SimHashes.Unobtanium)
						{
							if (!flag)
							{
								list.Add(num);
							}
						}
					}
				}
			}
		}
		return list;
	}

	private void DoShockwaveExplosion(float x, float y)
	{
		if (this.shockwavePoints == null)
		{
			this.startLocation = new Vector2(x, y);
			List<Vector2> list = this.DoCircularExplosion(x, y, 1f);
			this.visitedPoints = new List<KeyValuePair<int, float>>();
			this.shockwavePoints = new List<int>();
			for (int i = 0; i < list.Count; i++)
			{
				int num = Grid.PosToCell(list[i]);
				if (Grid.Element[num].id != SimHashes.Unobtanium)
				{
					this.shockwavePoints.Add(num);
				}
			}
			return;
		}
		List<int> list2 = this.shockwavePoints.Distinct<int>().ToList<int>();
		this.shockwavePoints = new List<int>();
		for (int j = 0; j < list2.Count; j++)
		{
			int num2 = list2[j];
			if (Grid.Element[num2].id != SimHashes.Unobtanium)
			{
				float mass = Grid.Cell[num2].mass;
				this.visitedPoints.Add(new KeyValuePair<int, float>(num2, mass));
				if (this.step < this.maxHeatSteps)
				{
					SimMessages.ModifyEnergy(num2, 2000f, SimMessages.EnergySourceID.Excavator);
				}
				if (mass < this.minMassForShockwave)
				{
					WorldDamage.Instance.DestroyCell(num2);
					List<int> neighbors = this.GetNeighbors(num2);
					this.shockwavePoints.AddRange(neighbors);
				}
				else
				{
					SimMessages.ModifyMass(num2, this.maxMassRemoval, CellEventLogger.Instance.ExcavatorShockwave, -1f, SimHashes.Vacuum);
				}
			}
		}
		this.step++;
	}

	private void SimUpdate(float dt)
	{
		if (!this.exploding)
		{
			return;
		}
		this.nextExplosion -= Time.deltaTime;
		if (this.nextExplosion < 0f)
		{
			this.currentRadius += 1f;
			Collider componentInChildren = base.GetComponentInChildren<Collider>();
			Bounds bounds;
			if (componentInChildren != null)
			{
				bounds = componentInChildren.bounds;
			}
			else
			{
				bounds = base.GetComponentInChildren<Collider2D>().bounds;
			}
			float x = bounds.center.x;
			float y = bounds.center.y;
			switch (this.type)
			{
			case Excavator.ExplosionType.CircularFilled:
				this.DoCircularExplosion(x, y, this.currentRadius);
				break;
			case Excavator.ExplosionType.Shockwave:
				this.DoShockwaveExplosion(x, y);
				break;
			}
			this.nextExplosion = 0.01f;
		}
		if (this.currentRadius > this.maxRadius || Time.time - this.startTime > 0.3f)
		{
			int num = Grid.PosToCell(this);
			foreach (CellOffset cellOffset in this.Def.PlacementOffsets)
			{
				int num2 = Grid.OffsetCell(num, cellOffset);
				if (Grid.Element[num2].id != SimHashes.Unobtanium)
				{
					Vector3 vector = Grid.CellToPosCCC(num2, Grid.SceneLayer.Building);
					vector.z = -Grid.CellSizeInMeters * 0.5f;
					this.PlayExplosion(vector);
				}
			}
			base.PlaySound3D(Sounds.Instance.BlowUp_GenericMigrated);
			Vector3 vector2 = Grid.CellToPosCCC(num, Grid.SceneLayer.Move);
			GameUtil.CreateExplosion(vector2);
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	private float startTime = float.MaxValue;

	public float armTimer = 10f;

	public float minSafeDistance = 5f;

	public float maxRadius = 5f;

	public float currentRadius;

	public float countDown = 3f;

	public float nextExplosion = 0.01f;

	public float minMassForShockwave = 250f;

	public bool detectMinion = true;

	public bool exploding;

	public Excavator.ExplosionType type;

	private List<KeyValuePair<int, float>> visitedPoints;

	private List<int> shockwavePoints;

	private Vector3 startLocation;

	private int step;

	public int maxHeatSteps = 10;

	public float maxMassRemoval = 800f;

	public enum ExplosionType
	{
		CircularFilled,
		CircularCenterOnly,
		Shockwave
	}
}
