using System;
using UnityEngine;

public class FishingStation : Harvestable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.showProgressBar = false;
		base.SetWorkTime(float.PositiveInfinity);
		int num = Grid.PosToCell(base.transform.GetPosition());
		int num2 = Grid.CellLeft(num);
		int num3 = Grid.CellRight(num);
		this.faceTargetWhenWorking = false;
		SimMessages.ReplaceElement(num2, this.primaryElement.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, this.primaryElement.Mass, Grid.Temperature[num2], Grid.Disease[num2].diseaseIdx, Grid.Disease[num2].elementCount, -1);
		SimMessages.ReplaceElement(num3, this.primaryElement.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, this.primaryElement.Mass, Grid.Temperature[num3], Grid.Disease[num3].diseaseIdx, Grid.Disease[num3].elementCount, -1);
		Grid.RenderedByWorld[num2] = false;
		Grid.RenderedByWorld[num3] = false;
		SimMessages.SetStrength(num2, 0, 1f);
		SimMessages.SetStrength(num3, 0, 1f);
		this.RefreshOffsets();
		this.userMenu.Refresh();
		this.RefreshLure();
		GameObject gameObject = new GameObject("Line_Renderer");
		gameObject.transform.SetPosition(base.transform.GetPosition());
		gameObject.transform.parent = base.transform;
		this.lineRenderer = gameObject.AddComponent<KBatchedAnimController>();
		this.lineRenderer.materialType = KAnimBatchGroup.MaterialType.Simple;
		this.lineRenderer.AddAnims(new KAnimFile[] { this.lineAnimation });
		this.lineRenderer.Play("line", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void ForceCatch(Catchable Fish)
	{
		if (base.worker != null && this.chore != null)
		{
			Fish.Caught();
			this.lure.ReelIn();
		}
	}

	private void DropLine(object param)
	{
		int num = 0;
		if (base.worker != null)
		{
			for (int i = 1; i < this.LineRange; i++)
			{
				int num2 = Grid.PosToCell(base.transform.GetPosition() + Vector3.down * (float)i);
				num = i;
				if (Grid.IsLiquid(num2))
				{
					for (int j = 0; j < 3; j++)
					{
						int num3 = Grid.OffsetCell(num2, 0, -j);
						int num4 = Grid.CellBelow(num3);
						if (Grid.IsLiquid(num4) && Grid.IsLiquid(Grid.CellBelow(num4)))
						{
							num++;
						}
					}
					break;
				}
			}
		}
		else
		{
			num = 1;
		}
		this.lure.moveTarget = base.transform.GetPosition() + Vector3.down * (float)num;
	}

	private void StopWork(object param)
	{
		this.lure.ReelIn();
	}

	private void RefreshLure()
	{
		if (this.lure == null)
		{
			GameObject gameObject = new GameObject("Fishing_Lure");
			gameObject.transform.parent = base.transform;
			gameObject.AddComponent<KBatchedAnimController>().AddAnims(new KAnimFile[] { this.lineAnimation });
			this.lure = gameObject.AddComponent<FishingLure>();
			this.lure.station = this;
			gameObject.transform.SetPosition(base.transform.GetPosition());
			this.lure.SetupLine(this, this.lineAnimation);
			this.lure.ReelIn();
		}
		else
		{
			this.lure.station = this;
		}
	}

	public void RefreshOffsets()
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 1),
			new CellOffset(-1, 1),
			new CellOffset(1, 1)
		};
		base.SetOffsets(array);
	}

	public void RemoveFromHook(GameObject caught)
	{
		Vector3 vector = this.CaughtDepositLocation();
		caught.transform.SetPosition(vector);
		caught.GetComponent<Catchable>().Caught();
		if (this.autoRestartTask)
		{
			this.workTimeRemaining = float.PositiveInfinity;
			this.DropLine(null);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.lure != null)
		{
			Util.KDestroyGameObject(this.lure.gameObject);
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		int num2 = Grid.CellLeft(num);
		int num3 = Grid.CellRight(num);
		SimMessages.ReplaceElement(num2, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierDestroySelf, 0f, 0f, byte.MaxValue, 0, -1);
		SimMessages.ReplaceElement(num3, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierDestroySelf, 0f, 0f, byte.MaxValue, 0, -1);
		base.OnCleanUp();
	}

	public override void OnRefreshUserMenu(object data)
	{
		if (!this.canBeHarvested)
		{
			return;
		}
		if (this.isMarkedForHarvest)
		{
		}
	}

	public override void MarkForHarvest()
	{
		if (this.chore == null)
		{
			this.chore = new WorkChore<FishingStation>(Db.Get().ChoreTypes.Harvest, this, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue, false);
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingFish, null);
		}
		this.isMarkedForHarvest = true;
	}

	public override void ForceCancelHarvest(object data = null)
	{
		this.OnCancel(null);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingFish, false);
		this.userMenu.Refresh();
	}

	protected override void OnCancel(object data)
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Cancel fishing");
			this.chore = null;
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingFish, false);
		}
		this.isMarkedForHarvest = false;
	}

	public virtual Vector3 CaughtDepositLocation()
	{
		Vector3 vector = new Vector3(0f, 0.5f, -2f);
		Vector3 vector2 = base.transform.GetPosition() + Vector3.up;
		vector2.z = -2f;
		for (int i = 0; i < 3; i++)
		{
			Vector3 vector3 = new Vector3((float)i, 0f, 0f);
			if (this.depositLocationPriority(vector2, vector3) == 2)
			{
				return new Vector3(vector3.x + vector2.x, vector3.y + vector2.y, 0f) + vector;
			}
			if (this.depositLocationPriority(vector2, -vector3) == 2)
			{
				return new Vector3(-vector3.x + vector2.x, -vector3.y + vector2.y, 0f) + vector;
			}
		}
		for (int j = 0; j < 3; j++)
		{
			Vector3 vector3 = new Vector3((float)j, 0f, 0f);
			if (this.depositLocationPriority(vector2, vector3) == 1)
			{
				return new Vector3(vector3.x + vector2.x, vector3.y + vector2.y, 0f) + vector;
			}
			if (this.depositLocationPriority(vector2, -vector3) == 1)
			{
				return new Vector3(-vector3.x + vector2.x, -vector3.y + vector2.y, 0f) + vector;
			}
		}
		return vector2;
	}

	private int depositLocationPriority(Vector3 rootLocation, Vector3 offsetLocation)
	{
		int num = Grid.PosToCell(rootLocation + offsetLocation);
		if (Grid.Solid[num] || !Grid.Solid[Grid.CellBelow(num)])
		{
			return 0;
		}
		if (!Grid.Objects[num, 5])
		{
			return 2;
		}
		return 1;
	}

	private void Update()
	{
		if (this.lineRenderer != null)
		{
			float num = 0f;
			float num2 = Vector3.Distance(base.transform.GetPosition(), this.lure.transform.GetPosition()) - num;
			this.lineRenderer.GetBatchInstanceData().SetClipRadius(base.transform.GetPosition().x, base.transform.GetPosition().y, num2 * num2, true);
			this.lineRenderer.SetDirty();
		}
	}

	public void EmptyWater()
	{
	}

	private int LineRange = 7;

	public KAnimFile lineAnimation;

	public KBatchedAnimController lineRenderer;

	[MyCmpAdd]
	private PrimaryElement primaryElement;

	private bool autoRestartTask = true;

	public FishingLure lure;
}
