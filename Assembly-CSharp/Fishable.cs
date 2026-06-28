using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Fishable : Harvestable, ISaveLoadableJson
{
	protected Fishable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.SetWorkTime(60f);
		base.gameObject.Subscribe(-1358696400, new EventSystem.EventHandler(this.PositionProgressBar));
		base.gameObject.Subscribe(1272413801, new EventSystem.EventHandler(this.OnCatchComplete));
		base.Subscribe(WaterBodyProbe.Instance.gameObject, -263784810, new EventSystem.EventHandler(this.RefreshBodyOfWater));
	}

	public virtual void RefreshBodyOfWater(object param)
	{
		if (this.bodyOfWater == null)
		{
			Debug.Log("force cancel body null");
			this.ForceCancelHarvest(null);
		}
	}

	private void PositionProgressBar(object data)
	{
		if (this.progressBar == null)
		{
			return;
		}
		this.progressBar.transform.SetPosition(new Vector3(this.transform.position.x - 0.5f, this.transform.position.y + 1f, 0f));
		if (base.worker != null && this.positionToWorker)
		{
			this.progressBar.transform.SetPosition(base.worker.transform.position - new Vector3(0.5f, 0f, 0f));
		}
	}

	public override void OnRefreshUserMenu(object data)
	{
		if (this.bodyOfWater == null)
		{
			return;
		}
		if (!this.canBeHarvested)
		{
			return;
		}
		if (this.bodyOfWater.GetCatchables().Length <= 0)
		{
			return;
		}
		if (this.isMarkedForHarvest)
		{
			this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_fish", "Cancel Fishing", new global::System.Action(base.OnClickCancelHarvest), global::Action.NumActions, null, null, null, null, string.Empty));
		}
		else
		{
			this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_fish", "Fish", new global::System.Action(this.OnClickHarvest), global::Action.NumActions, null, null, null, null, string.Empty));
		}
	}

	public virtual void RefreshOffsets()
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		base.SetOffsets(array);
	}

	public void OnCatchComplete(object param)
	{
		Catchable[] catchables = this.bodyOfWater.GetCatchables();
		if (catchables == null || catchables.Length == 0)
		{
			return;
		}
		GameObject gameObject = this.RollCatch();
		if (gameObject == null)
		{
			return;
		}
		gameObject.transform.SetPosition(this.CaughtDepositLocation());
		this.bodyOfWater.RemoveObjectFromBody(gameObject);
		if (this.bodyOfWater == null || this.bodyOfWater.containedObjects.Count == 0)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, "Water Depleted", this.transform, 1.5f, false);
		}
		gameObject.GetComponent<Catchable>().Caught();
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, "Caught " + gameObject.GetComponent<KPrefabID>().GetProperName(), gameObject.transform, 1.5f, false);
		if (this.AutoRestartWorkTask)
		{
			this.MarkForHarvest();
		}
		this.RefreshBodyOfWater(null);
	}

	public virtual Vector3 CaughtDepositLocation()
	{
		Vector3 vector;
		if (base.GetWorker() != null && this.positionToWorker)
		{
			vector = base.GetWorker().transform.position;
		}
		else
		{
			vector = this.transform.position + Vector3.up;
		}
		for (int i = 0; i < 3; i++)
		{
			Vector3 vector2 = new Vector3((float)i, 0f, 0f);
			if (this.depositLocationPriority(vector, vector2) == 2)
			{
				return new Vector3(vector2.x + vector.x, vector2.y + vector.y, -1.5f);
			}
			if (this.depositLocationPriority(vector, -vector2) == 2)
			{
				return new Vector3(-vector2.x + vector.x, -vector2.y + vector.y, -1.5f);
			}
		}
		for (int j = 0; j < 3; j++)
		{
			Vector3 vector2 = new Vector3((float)j, 0f, 0f);
			if (this.depositLocationPriority(vector, vector2) == 1)
			{
				return new Vector3(vector2.x + vector.x, vector2.y + vector.y, -1.5f);
			}
			if (this.depositLocationPriority(vector, -vector2) == 1)
			{
				return new Vector3(-vector2.x + vector.x, -vector2.y + vector.y, -1.5f);
			}
		}
		return vector;
	}

	private int depositLocationPriority(Vector3 rootLocation, Vector3 offsetLocation)
	{
		int num = Grid.PosToCell(rootLocation + offsetLocation);
		if (Grid.Solid[num] || !Grid.Solid[Grid.CellBelow(num)])
		{
			return 0;
		}
		if (!Grid.Objects[num, 22])
		{
			return 2;
		}
		return 1;
	}

	private GameObject RollCatch()
	{
		GameObject gameObject = null;
		int num = 0;
		Catchable[] catchables = this.bodyOfWater.GetCatchables();
		foreach (Catchable catchable in catchables)
		{
			num += catchable.Weight;
		}
		int num2 = global::UnityEngine.Random.Range(0, num + 1);
		int num3 = 0;
		for (int j = 0; j < catchables.Length; j++)
		{
			num3 += catchables[j].Weight;
			if (num3 >= num2)
			{
				gameObject = catchables[j].gameObject;
			}
		}
		return gameObject;
	}

	public override void MarkForHarvest()
	{
		if (this.chore == null)
		{
			this.chore = new WorkChore<Fishable>(Db.Get().ChoreTypes.Harvest, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
		}
		this.isMarkedForHarvest = true;
	}

	public override void ForceCancelHarvest(object data = null)
	{
		this.OnCancel(null);
		this.userMenu.Refresh();
	}

	public bool AutoRestartWorkTask = true;

	public BodyOfWater bodyOfWater;

	public bool positionToWorker;

	private int[] worldWorkCells;
}
