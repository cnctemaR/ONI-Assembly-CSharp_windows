using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Deconstructable : Workable
{
	private Deconstructable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.faceTargetWhenWorking = true;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
		this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		this.Subscribe(-111137758, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		this.Subscribe(2127324410, new EventSystem.EventHandler(this.OnCancel));
		this.Subscribe(-790448070, new EventSystem.EventHandler(this.OnDeconstruct));
		if (this.isMarkedForDeconstruction)
		{
			this.QueueDeconstruction();
		}
	}

	public override float GetWorkTime()
	{
		return base.GetComponent<Building>().Def.ConstructionTime * 0.5f;
	}

	protected override void OnStartWork(Worker worker)
	{
		this.progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("DeconstructBar");
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		PrimaryElement primary_element = base.GetComponent<PrimaryElement>();
		Building building = base.GetComponent<Building>();
		SimCellOccupier component = base.GetComponent<SimCellOccupier>();
		if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(base.gameObject))
		{
			DetailsScreen.Instance.Show(false);
		}
		if (component != null)
		{
			int num = Grid.PosToCell(this.transform.position);
			if (Grid.Objects[num, (int)building.Def.TileLayer] == base.gameObject)
			{
				Grid.Objects[num, (int)building.Def.ObjectLayer] = null;
				Grid.Objects[num, (int)building.Def.TileLayer] = null;
				Grid.Foundation[num] = false;
				TileVisualizer.RefreshCell(num, building.Def.TileLayer);
			}
			component.DestroySelf(delegate
			{
				this.TriggerDestroy(building, primary_element);
			}, false);
		}
		else
		{
			this.TriggerDestroy(building, primary_element);
		}
		this.Trigger(-702296337, this);
	}

	private void TriggerDestroy(Building building, PrimaryElement primary_element)
	{
		GameObject gameObject = Deconstructable.SpawnItem(this.transform.position, building.Def, primary_element);
		gameObject.transform.position += Vector3.up * 0.5f;
		float num = (global::UnityEngine.Random.value - 0.5f) * Deconstructable.scale.x;
		Vector2 vector = Vector2.right * num + Vector2.up * Deconstructable.scale.y;
		if (GameComps.Fallers.Has(gameObject))
		{
			GameComps.Fallers.Remove(gameObject);
		}
		GameComps.Fallers.Add(gameObject, vector);
		base.gameObject.DeleteObject();
	}

	private void QueueDeconstruction()
	{
		if (this.chore == null)
		{
			if (DebugHandler.InstantBuildMode)
			{
				this.OnCompleteWork(null);
			}
			else
			{
				this.chore = new WorkChore<Deconstructable>(Db.Get().ChoreTypes.Deconstruct, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true);
				base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, this);
				this.isMarkedForDeconstruction = true;
			}
		}
	}

	private void OnDeconstruct()
	{
		if (this.chore == null)
		{
			this.QueueDeconstruction();
		}
		else
		{
			this.CancelDeconstruction();
		}
	}

	public bool IsMarkedForDeconstruction()
	{
		return this.chore != null;
	}

	private static GameObject SpawnItem(Vector3 position, BuildingDef def, PrimaryElement primaryElement)
	{
		GameObject gameObject = null;
		int num = Grid.PosToCell(position);
		CellOffset[] placementOffsets = def.PlacementOffsets;
		float num2 = primaryElement.Mass;
		int num3 = 0;
		while ((float)num3 < primaryElement.Mass / 400f)
		{
			int num4 = num3 % def.PlacementOffsets.Length;
			int num5 = Grid.OffsetCell(num, placementOffsets[num4]);
			float num6 = num2;
			if (num2 > 400f)
			{
				num6 = 400f;
				num2 -= 400f;
			}
			gameObject = primaryElement.Element.substance.SpawnResource(Grid.CellToPosCBC(num5, Grid.SceneLayer.Use), num6, primaryElement.Temperature, false, false);
			num3++;
		}
		return gameObject;
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.allowDeconstruction)
		{
			return;
		}
		if (this.chore == null)
		{
			UserMenu userMenu = this.userMenu;
			string text = UI.USERMENUACTIONS.DEMOLISH.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DEMOLISH.NAME, new global::System.Action(this.OnDeconstruct), global::Action.NumActions, null, null, null, null, text));
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text = UI.USERMENUACTIONS.DEMOLISH.TOOLTIP_OFF;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DEMOLISH.NAME_OFF, new global::System.Action(this.OnDeconstruct), global::Action.NumActions, null, null, null, null, text));
		}
	}

	private void CancelDeconstruction()
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Cancelled deconstruction");
			this.chore = null;
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction);
			base.ShowProgressBar(false);
			this.isMarkedForDeconstruction = false;
		}
	}

	private void OnCancel(object data)
	{
		this.CancelDeconstruction();
	}

	private void OnDeconstruct(object data)
	{
		if (this.allowDeconstruction)
		{
			this.QueueDeconstruction();
		}
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "build", EffectPrefabs.Instance.BuildEffect);
		return anim;
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;

	public bool allowDeconstruction = true;

	[Serialize]
	private bool isMarkedForDeconstruction;

	private static Vector2 scale = new Vector2(1f, 4f);
}
