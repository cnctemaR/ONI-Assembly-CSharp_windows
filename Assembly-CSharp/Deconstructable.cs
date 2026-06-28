using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Deconstructable : Workable
{
	private Deconstructable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.faceTargetWhenWorking = true;
		this.synchronizeAnims = false;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
		this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(-111137758, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(2127324410, new Action<object>(this.OnCancel));
		base.Subscribe(-790448070, new Action<object>(this.OnDeconstruct));
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
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		Building building = base.GetComponent<Building>();
		SimCellOccupier component2 = base.GetComponent<SimCellOccupier>();
		if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(base.gameObject))
		{
			DetailsScreen.Instance.Show(false);
		}
		float mass = component.Mass;
		float temperature = component.Temperature;
		SimHashes element = component.ElementID;
		byte disease_idx = component.DiseaseIdx;
		int disease_count = component.DiseaseCount;
		if (component2 != null)
		{
			if (building.Def.TileLayer != ObjectLayer.NumLayers)
			{
				int num = Grid.PosToCell(base.transform.position);
				if (Grid.Objects[num, (int)building.Def.TileLayer] == base.gameObject)
				{
					Grid.Objects[num, (int)building.Def.ObjectLayer] = null;
					Grid.Objects[num, (int)building.Def.TileLayer] = null;
					Grid.Foundation[num] = false;
					TileVisualizer.RefreshCell(num, building.Def.TileLayer);
				}
			}
			component2.DestroySelf(delegate
			{
				this.TriggerDestroy(building, element, mass, temperature, disease_idx, disease_count);
			});
		}
		else
		{
			this.TriggerDestroy(building, element, mass, temperature, disease_idx, disease_count);
		}
		string sound = GlobalAssets.GetSound("Finish_Deconstruction_" + building.Def.AudioSize, false);
		if (sound != null)
		{
			KMonoBehaviour.PlaySound3DAtLocation(sound, base.gameObject.transform.position);
		}
		base.Trigger(-702296337, this);
	}

	private void TriggerDestroy(Building building, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (this == null || this.destroyed)
		{
			return;
		}
		GameObject gameObject = Deconstructable.SpawnItem(base.transform.position, building.Def, element, mass, temperature, disease_idx, disease_count);
		gameObject.transform.position += Vector3.up * 0.5f;
		int num = Grid.PosToCell(gameObject.transform.position);
		int num2 = Grid.CellAbove(num);
		Vector2 vector;
		if ((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2]))
		{
			vector = Vector2.zero;
		}
		else
		{
			Vector3 vector2;
			gameObject.transform.position.x = vector2.x + (global::UnityEngine.Random.value - 0.5f) * Deconstructable.scale.x;
			vector = Vector2.up * Deconstructable.scale.y;
		}
		if (GameComps.Fallers.Has(gameObject))
		{
			GameComps.Fallers.Remove(gameObject);
		}
		GameComps.Fallers.Add(gameObject, vector);
		this.destroyed = true;
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
				this.chore = new WorkChore<Deconstructable>(Db.Get().ChoreTypes.Deconstruct, this, null, true, null, null, null, true, null, false, default(Tag), null, true, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
				base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, this);
				this.isMarkedForDeconstruction = true;
				Prioritizable.AddRef(base.gameObject);
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

	private static GameObject SpawnItem(Vector3 position, BuildingDef def, SimHashes src_element, float src_mass, float src_temperature, byte disease_idx, int disease_count)
	{
		GameObject gameObject = null;
		int num = Grid.PosToCell(position);
		CellOffset[] placementOffsets = def.PlacementOffsets;
		float num2 = src_mass;
		Element element = ElementLoader.FindElementByHash(src_element);
		int num3 = 0;
		while ((float)num3 < src_mass / 400f)
		{
			int num4 = num3 % def.PlacementOffsets.Length;
			int num5 = Grid.OffsetCell(num, placementOffsets[num4]);
			float num6 = num2;
			if (num2 > 400f)
			{
				num6 = 400f;
				num2 -= 400f;
			}
			gameObject = element.substance.SpawnResource(Grid.CellToPosCBC(num5, Grid.SceneLayer.Ore), num6, src_temperature, disease_idx, disease_count, false, false);
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
			string text = "action_deconstruct";
			string text2 = UI.USERMENUACTIONS.DEMOLISH.NAME;
			global::System.Action action = new global::System.Action(this.OnDeconstruct);
			string text3 = UI.USERMENUACTIONS.DEMOLISH.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text3 = "action_deconstruct";
			string text2 = UI.USERMENUACTIONS.DEMOLISH.NAME_OFF;
			global::System.Action action = new global::System.Action(this.OnDeconstruct);
			string text = UI.USERMENUACTIONS.DEMOLISH.TOOLTIP_OFF;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true), 1f);
		}
	}

	private void CancelDeconstruction()
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Cancelled deconstruction");
			this.chore = null;
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, false);
			base.ShowProgressBar(false);
			this.isMarkedForDeconstruction = false;
			Prioritizable.RemoveRef(base.gameObject);
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

	private static Vector2 scale = new Vector2(0.5f, 4f);

	private bool destroyed;
}
