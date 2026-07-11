using System;
using KSerialization;
using STRINGS;
using TUNING;
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
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.multitoolContext = "build";
		this.multitoolHitEffectTag = EffectConfigs.BuildSplashId;
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

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
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
				int num = Grid.PosToCell(base.transform.GetPosition());
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
			KMonoBehaviour.PlaySound3DAtLocation(sound, base.gameObject.transform.GetPosition());
		}
		base.Trigger(-702296337, this);
	}

	private void TriggerDestroy(Building building, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (this == null || this.destroyed)
		{
			return;
		}
		GameObject gameObject = Deconstructable.SpawnItem(base.transform.GetPosition(), building.Def, element, mass, temperature, disease_idx, disease_count);
		gameObject.transform.SetPosition(gameObject.transform.GetPosition() + Vector3.up * 0.5f);
		int num = Grid.PosToCell(gameObject.transform.GetPosition());
		int num2 = Grid.CellAbove(num);
		Vector2 zero;
		if ((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2]))
		{
			zero = Vector2.zero;
		}
		else
		{
			Vector3 vector;
			gameObject.transform.GetPosition().x = vector.x + (global::UnityEngine.Random.value - 0.5f) * 0.5f;
			zero = new Vector2(global::UnityEngine.Random.Range(-1f, 1f) * Deconstructable.INITIAL_VELOCITY_RANGE.x, Deconstructable.INITIAL_VELOCITY_RANGE.y);
		}
		if (GameComps.Fallers.Has(gameObject))
		{
			GameComps.Fallers.Remove(gameObject);
		}
		GameComps.Fallers.Add(gameObject, zero);
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
				Prioritizable.AddRef(base.gameObject);
				this.chore = new WorkChore<Deconstructable>(Db.Get().ChoreTypes.Deconstruct, this, null, null, true, null, null, null, true, null, false, false, null, true, true, true, PriorityScreen.PriorityClass.basic, 0, true);
				base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, this);
				this.isMarkedForDeconstruction = true;
				base.Trigger(2108245096, "Deconstruct");
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

	public static GameObject SpawnItem(Vector3 position, BuildingDef def, SimHashes src_element, float src_mass, float src_temperature, byte disease_idx, int disease_count)
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
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (this.chore == null)
		{
			string text = "action_deconstruct";
			string text2 = UI.USERMENUACTIONS.DEMOLISH.NAME;
			global::System.Action action = new global::System.Action(this.OnDeconstruct);
			string text3 = UI.USERMENUACTIONS.DEMOLISH.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_deconstruct";
			string text2 = UI.USERMENUACTIONS.DEMOLISH.NAME_OFF;
			global::System.Action action = new global::System.Action(this.OnDeconstruct);
			string text = UI.USERMENUACTIONS.DEMOLISH.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
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

	private Chore chore;

	public bool allowDeconstruction = true;

	[Serialize]
	private bool isMarkedForDeconstruction;

	private static readonly Vector2 INITIAL_VELOCITY_RANGE = new Vector2(0.5f, 4f);

	private bool destroyed;
}
