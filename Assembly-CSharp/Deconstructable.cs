using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Deconstructable")]
public class Deconstructable : Workable
{
	private CellOffset[] placementOffsets
	{
		get
		{
			Building component = base.GetComponent<Building>();
			if (component != null)
			{
				CellOffset[] array = component.Def.PlacementOffsets;
				Rotatable component2 = component.GetComponent<Rotatable>();
				if (component2 != null)
				{
					array = new CellOffset[component.Def.PlacementOffsets.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = component2.GetRotatedCellOffset(component.Def.PlacementOffsets[i]);
					}
				}
				return array;
			}
			OccupyArea component3 = base.GetComponent<OccupyArea>();
			if (component3 != null)
			{
				return component3.OccupiedCellsOffsets;
			}
			if (this.looseEntityDeconstructable)
			{
				return new CellOffset[]
				{
					new CellOffset(0, 0)
				};
			}
			global::Debug.Assert(false, "Ack! We put a Deconstructable on something that's neither a Building nor OccupyArea!", this);
			return null;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.faceTargetWhenWorking = true;
		this.synchronizeAnims = false;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
		this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.minimumAttributeMultiplier = 0.75f;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
		this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		this.multitoolContext = "build";
		this.multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		this.workingPstComplete = null;
		this.workingPstFailed = null;
		if (this.customWorkTime > 0f)
		{
			base.SetWorkTime(this.customWorkTime);
			return;
		}
		Building component = base.GetComponent<Building>();
		if (component != null && component.Def.IsTilePiece)
		{
			base.SetWorkTime(component.Def.ConstructionTime * 0.5f);
			return;
		}
		base.SetWorkTime(30f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		CellOffset[] array = null;
		CellOffset[][] array2 = OffsetGroups.InvertedStandardTable;
		Building component = base.GetComponent<Building>();
		if (component != null && component.Def.IsTilePiece)
		{
			array2 = OffsetGroups.InvertedStandardTableWithCorners;
			array = component.Def.ConstructionOffsetFilter;
		}
		CellOffset[][] array3 = OffsetGroups.BuildReachabilityTable(this.placementOffsets, array2, array);
		base.SetOffsetTable(array3);
		base.Subscribe<Deconstructable>(493375141, Deconstructable.OnRefreshUserMenuDelegate);
		base.Subscribe<Deconstructable>(-111137758, Deconstructable.OnRefreshUserMenuDelegate);
		base.Subscribe<Deconstructable>(2127324410, Deconstructable.OnCancelDelegate);
		base.Subscribe<Deconstructable>(-790448070, Deconstructable.OnDeconstructDelegate);
		if (this.constructionElements == null || this.constructionElements.Length == 0)
		{
			this.constructionElements = new Tag[1];
			this.constructionElements[0] = base.GetComponent<PrimaryElement>().Element.tag;
		}
		if (this.isMarkedForDeconstruction)
		{
			this.QueueDeconstruction(false);
		}
		this.reconstructable = base.GetComponent<Reconstructable>();
	}

	protected override void OnStartWork(Worker worker)
	{
		this.progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("DeconstructBar");
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, false);
		base.Trigger(1830962028, this);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.Trigger(-702296337, this);
		if (this.reconstructable != null)
		{
			this.reconstructable.TryCommenceReconstruct();
		}
		Building component = base.GetComponent<Building>();
		SimCellOccupier component2 = base.GetComponent<SimCellOccupier>();
		if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(base.gameObject))
		{
			DetailsScreen.Instance.Show(false);
		}
		PrimaryElement component3 = base.GetComponent<PrimaryElement>();
		float temperature = component3.Temperature;
		byte disease_idx = component3.DiseaseIdx;
		int disease_count = component3.DiseaseCount;
		if (component2 != null)
		{
			if (component.Def.TileLayer != ObjectLayer.NumLayers)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				if (Grid.Objects[num, (int)component.Def.TileLayer] == base.gameObject)
				{
					Grid.Objects[num, (int)component.Def.ObjectLayer] = null;
					Grid.Objects[num, (int)component.Def.TileLayer] = null;
					Grid.Foundation[num] = false;
					TileVisualizer.RefreshCell(num, component.Def.TileLayer, component.Def.ReplacementLayer);
				}
			}
			component2.DestroySelf(delegate
			{
				this.TriggerDestroy(temperature, disease_idx, disease_count, worker);
			});
		}
		else
		{
			this.TriggerDestroy(temperature, disease_idx, disease_count);
		}
		if (component == null || component.Def.PlayConstructionSounds)
		{
			string sound = GlobalAssets.GetSound("Finish_Deconstruction_" + ((!this.audioSize.IsNullOrWhiteSpace()) ? this.audioSize : component.Def.AudioSize), false);
			if (sound != null)
			{
				KMonoBehaviour.PlaySound3DAtLocation(sound, base.gameObject.transform.GetPosition());
			}
		}
	}

	public bool HasBeenDestroyed
	{
		get
		{
			return this.destroyed;
		}
	}

	public List<GameObject> ForceDestroyAndGetMaterials()
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		float temperature = component.Temperature;
		byte diseaseIdx = component.DiseaseIdx;
		int diseaseCount = component.DiseaseCount;
		return this.TriggerDestroy(temperature, diseaseIdx, diseaseCount);
	}

	private List<GameObject> TriggerDestroy(float temperature, byte disease_idx, int disease_count, Worker tile_worker)
	{
		if (this == null || this.destroyed)
		{
			return null;
		}
		List<GameObject> list = this.SpawnItemsFromConstruction(temperature, disease_idx, disease_count, tile_worker);
		this.destroyed = true;
		base.gameObject.DeleteObject();
		return list;
	}

	private List<GameObject> TriggerDestroy(float temperature, byte disease_idx, int disease_count)
	{
		return this.TriggerDestroy(temperature, disease_idx, disease_count, base.worker);
	}

	public void QueueDeconstruction(bool userTriggered)
	{
		if (userTriggered && DebugHandler.InstantBuildMode)
		{
			this.OnCompleteWork(null);
			return;
		}
		if (this.chore == null)
		{
			BuildingComplete component = base.GetComponent<BuildingComplete>();
			if (component != null && component.Def.ReplacementLayer != ObjectLayer.NumLayers)
			{
				int num = Grid.PosToCell(component);
				if (Grid.Objects[num, (int)component.Def.ReplacementLayer] != null)
				{
					return;
				}
			}
			Prioritizable.AddRef(base.gameObject);
			this.chore = new WorkChore<Deconstructable>(Db.Get().ChoreTypes.Deconstruct, this, null, true, null, null, null, true, null, false, false, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, this);
			this.isMarkedForDeconstruction = true;
			base.Trigger(2108245096, "Deconstruct");
		}
	}

	private void QueueDeconstruction()
	{
		this.QueueDeconstruction(true);
	}

	private void OnDeconstruct()
	{
		if (this.chore == null)
		{
			this.QueueDeconstruction();
			return;
		}
		this.CancelDeconstruction();
	}

	public bool IsMarkedForDeconstruction()
	{
		return this.chore != null;
	}

	public void SetAllowDeconstruction(bool allow)
	{
		this.allowDeconstruction = allow;
		if (!this.allowDeconstruction)
		{
			this.CancelDeconstruction();
		}
	}

	public void SpawnItemsFromConstruction(Worker chore_worker)
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		float temperature = component.Temperature;
		byte diseaseIdx = component.DiseaseIdx;
		int diseaseCount = component.DiseaseCount;
		this.SpawnItemsFromConstruction(temperature, diseaseIdx, diseaseCount, chore_worker);
	}

	private List<GameObject> SpawnItemsFromConstruction(float temperature, byte disease_idx, int disease_count, Worker construction_worker)
	{
		List<GameObject> list = new List<GameObject>();
		if (!this.allowDeconstruction)
		{
			return list;
		}
		Building component = base.GetComponent<Building>();
		float[] array;
		if (component != null)
		{
			array = component.Def.Mass;
		}
		else
		{
			array = new float[] { base.GetComponent<PrimaryElement>().Mass };
		}
		int num = 0;
		while (num < this.constructionElements.Length && array.Length > num)
		{
			GameObject gameObject = this.SpawnItem(base.transform.GetPosition(), this.constructionElements[num], array[num], temperature, disease_idx, disease_count, construction_worker);
			int num2 = Grid.PosToCell(gameObject.transform.GetPosition());
			int num3 = Grid.CellAbove(num2);
			Vector2 zero;
			if ((Grid.IsValidCell(num2) && Grid.Solid[num2]) || (Grid.IsValidCell(num3) && Grid.Solid[num3]))
			{
				zero = Vector2.zero;
			}
			else
			{
				zero = new Vector2(global::UnityEngine.Random.Range(-1f, 1f) * Deconstructable.INITIAL_VELOCITY_RANGE.x, Deconstructable.INITIAL_VELOCITY_RANGE.y);
			}
			if (GameComps.Fallers.Has(gameObject))
			{
				GameComps.Fallers.Remove(gameObject);
			}
			GameComps.Fallers.Add(gameObject, zero);
			list.Add(gameObject);
			num++;
		}
		return list;
	}

	public GameObject SpawnItem(Vector3 position, Tag src_element, float src_mass, float src_temperature, byte disease_idx, int disease_count, Worker chore_worker)
	{
		GameObject gameObject = null;
		int num = Grid.PosToCell(position);
		CellOffset[] placementOffsets = this.placementOffsets;
		Element element = ElementLoader.GetElement(src_element);
		if (element != null)
		{
			float num2 = src_mass;
			int num3 = 0;
			while ((float)num3 < src_mass / 400f)
			{
				int num4 = num3 % placementOffsets.Length;
				int num5 = Grid.OffsetCell(num, placementOffsets[num4]);
				float num6 = num2;
				if (num2 > 400f)
				{
					num6 = 400f;
					num2 -= 400f;
				}
				gameObject = element.substance.SpawnResource(Grid.CellToPosCBC(num5, Grid.SceneLayer.Ore), num6, src_temperature, disease_idx, disease_count, false, false, false);
				gameObject.Trigger(580035959, chore_worker);
				num3++;
			}
		}
		else
		{
			int num7 = 0;
			while ((float)num7 < src_mass)
			{
				int num8 = num7 % placementOffsets.Length;
				int num9 = Grid.OffsetCell(num, placementOffsets[num8]);
				gameObject = GameUtil.KInstantiate(Assets.GetPrefab(src_element), Grid.CellToPosCBC(num9, Grid.SceneLayer.Ore), Grid.SceneLayer.Ore, null, 0);
				gameObject.SetActive(true);
				gameObject.Trigger(580035959, chore_worker);
				num7++;
			}
		}
		return gameObject;
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.allowDeconstruction)
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = ((this.chore == null) ? new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DECONSTRUCT.NAME, new global::System.Action(this.OnDeconstruct), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP, true) : new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DECONSTRUCT.NAME_OFF, new global::System.Action(this.OnDeconstruct), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP_OFF, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 0f);
	}

	public void CancelDeconstruction()
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Cancelled deconstruction");
			this.chore = null;
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, false);
			base.ShowProgressBar(false);
			this.isMarkedForDeconstruction = false;
			Prioritizable.RemoveRef(base.gameObject);
			Reconstructable component = base.GetComponent<Reconstructable>();
			if (component != null)
			{
				component.CancelReconstructOrder();
			}
		}
	}

	private void OnCancel(object data)
	{
		this.CancelDeconstruction();
	}

	private void OnDeconstruct(object data)
	{
		if (this.allowDeconstruction || DebugHandler.InstantBuildMode)
		{
			this.QueueDeconstruction();
		}
	}

	public Chore chore;

	public bool allowDeconstruction = true;

	public string audioSize;

	public float customWorkTime = -1f;

	private Reconstructable reconstructable;

	[Serialize]
	private bool isMarkedForDeconstruction;

	[Serialize]
	public Tag[] constructionElements;

	public bool looseEntityDeconstructable;

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnDeconstructDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnDeconstruct(data);
	});

	private static readonly Vector2 INITIAL_VELOCITY_RANGE = new Vector2(0.5f, 4f);

	private bool destroyed;
}
