using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Constructable : Workable, ISaveLoadable
{
	private Constructable()
	{
		base.preferPrimaryCell = false;
	}

	public Relocatable Source
	{
		get
		{
			return this.source.Get();
		}
		set
		{
			this.source.Set(value);
		}
	}

	public Recipe Recipe
	{
		get
		{
			return (!this.isRelocating) ? this.building.Def.CraftRecipe : this.building.Def.RelocateRecipe;
		}
	}

	public IList<Element> SelectedElements
	{
		get
		{
			return this.selectedElements;
		}
		set
		{
			if (this.selectedElements == null || this.selectedElements.Length != value.Count)
			{
				this.selectedElements = new Element[value.Count];
			}
			value.CopyTo(this.selectedElements, 0);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		float num = 0f;
		float num2 = 0f;
		foreach (GameObject gameObject in this.storage.items)
		{
			if (!(gameObject == null))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (!(component == null))
				{
					num += component.Mass;
					num2 += component.Temperature * component.Mass;
				}
			}
		}
		if (num <= 0f)
		{
			Output.LogWarningWithObj(base.gameObject, new object[]
			{
				"uhhh this constructable is about to generate a nan",
				"Item Count: ",
				this.storage.items.Count
			});
			return;
		}
		this.initialTemperature = Mathf.Clamp(num2 / num, 288.15f, 318.15f);
		KAnimGraphTileVisualizer component2 = base.GetComponent<KAnimGraphTileVisualizer>();
		UtilityConnections connections = ((!(component2 == null)) ? component2.Connections : ((UtilityConnections)0));
		if (this.IsReplacementTile)
		{
			int num3 = Grid.PosToCell(this.transform.localPosition);
			GameObject gameObject2 = Grid.Objects[num3, (int)this.building.Def.TileLayer];
			if (gameObject2 != null)
			{
				SimCellOccupier component3 = gameObject2.GetComponent<SimCellOccupier>();
				if (component3 != null)
				{
					component3.DestroySelf(delegate
					{
						if (this != null && this.gameObject != null)
						{
							this.FinishConstruction(connections);
						}
					});
				}
				else
				{
					Conduit component4 = gameObject2.GetComponent<Conduit>();
					if (component4 != null)
					{
						ConduitFlow flowManager = component4.GetFlowManager();
						flowManager.MarkForReplacement(num3);
					}
					BuildingComplete component5 = gameObject2.GetComponent<BuildingComplete>();
					if (component5 != null)
					{
						component5.onCleanUp += delegate
						{
							this.FinishConstruction(connections);
						};
					}
					else
					{
						global::Debug.LogWarning("Why am I trying to replace a: " + gameObject2.name, null);
						this.FinishConstruction(connections);
					}
				}
				KAnimGraphTileVisualizer component6 = gameObject2.GetComponent<KAnimGraphTileVisualizer>();
				if (component6 != null)
				{
					component6.skipCleanup = true;
				}
				gameObject2.DeleteObject();
			}
		}
		else
		{
			this.FinishConstruction(connections);
		}
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, base.GetComponent<KSelectable>().GetName(), this.transform, 1.5f, false);
		worker.GetComponent<Effects>().Add("DirtyHands", true);
	}

	private void FinishConstruction(UtilityConnections connections)
	{
		Rotatable component = base.GetComponent<Rotatable>();
		Orientation orientation = ((!(component != null)) ? Orientation.Neutral : component.GetOrientation());
		int num = Grid.PosToCell(this.transform.localPosition);
		GameObject gameObject = this.building.Def.Build(num, orientation, this.storage, this.selectedElements, this.isRelocating);
		gameObject.GetComponent<PrimaryElement>().Temperature = this.initialTemperature;
		gameObject.transform.rotation = this.transform.rotation;
		Rotatable component2 = gameObject.GetComponent<Rotatable>();
		if (component2 != null)
		{
			component2.SetOrientation(orientation);
		}
		KAnimGraphTileVisualizer component3 = base.GetComponent<KAnimGraphTileVisualizer>();
		if (component3 != null)
		{
			KAnimGraphTileVisualizer component4 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
			component4.Connections = connections;
			component3.skipCleanup = true;
		}
		KSelectable component5 = base.GetComponent<KSelectable>();
		if (component5 != null && component5.IsSelected && gameObject.GetComponent<KSelectable>() != null)
		{
			component5.Unselect();
			if (PlayerController.Instance.ActiveTool.name == "SelectTool")
			{
				((SelectTool)PlayerController.Instance.ActiveTool).SelectNextFrame(gameObject.GetComponent<KSelectable>(), false);
			}
		}
		base.GetComponent<Storage>().ConsumeAll();
		this.DeleteObject();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Build;
		this.invalidLocation = new Notification(MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);
		CellOffset[][] array = OffsetGroups.InvertedStandardTable;
		if (this.building.Def.IsTilePiece)
		{
			array = OffsetGroups.InvertedStandardTableWithCorners;
		}
		CellOffset[][] array2 = OffsetGroups.BuildReachabilityTable(this.building.Def.PlacementOffsets, array, this.building.Def.ConstructionOffsetFilter);
		base.SetOffsetTable(array2);
		base.GetComponent<Storage>().SetOffsetTable(array2);
		this.faceTargetWhenWorking = true;
		this.Subscribe(-1432940121, new Action<object>(this.OnReachableChanged));
		if (this.rotatable == null)
		{
			this.MarkArea();
		}
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Building;
		this.workingStatusItem = null;
		this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		this.storage.choreType = Db.Get().ChoreTypes.BuildFetch;
		this.choreType = Db.Get().ChoreTypes.Build;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(2127324410, new Action<object>(this.OnCancel));
		if (this.rotatable != null)
		{
			this.MarkArea();
		}
		this.storage.choreType = Db.Get().ChoreTypes.BuildFetch;
		this.fetchList = new FetchList2(this.storage);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		component.ElementID = this.selectedElements[0].id;
		PrimaryElement primaryElement = component;
		float num = 293.15f;
		component.Temperature = num;
		primaryElement.Temperature = num;
		foreach (Recipe.Ingredient ingredient in this.Recipe.GetAllIngredients(this.selectedElements))
		{
			this.fetchList.Add(ingredient.tag, ingredient.amount, FetchOrder2.OperationalRequirement.None);
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, ingredient.amount);
		}
		if (!this.building.Def.IsTilePiece)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Construction");
		}
		this.building.RunOnArea(delegate(int offset_cell)
		{
			if (base.gameObject.GetComponent<ConduitBridge>() == null)
			{
				GameObject gameObject2 = Grid.Objects[offset_cell, 7];
				if (gameObject2 != null)
				{
					gameObject2.DeleteObject();
				}
			}
		});
		if (this.IsReplacementTile && this.building.Def.ReplacementLayer != ObjectLayer.NumLayers)
		{
			int num2 = Grid.PosToCell(this.transform.position);
			GameObject gameObject = Grid.Objects[num2, (int)this.building.Def.ReplacementLayer];
			if (gameObject == null || gameObject == base.gameObject)
			{
				Grid.Objects[num2, (int)this.building.Def.ReplacementLayer] = base.gameObject;
				if (base.gameObject.GetComponent<SimCellOccupier>() != null)
				{
					int num3 = LayerMask.NameToLayer("Overlay");
					World.Instance.blockTileRenderer.AddBlock(num3, this.building.Def, SimHashes.Void, num2);
				}
				TileVisualizer.RefreshCell(num2, this.building.Def.TileLayer);
			}
			else
			{
				Output.LogError(new object[] { "multiple replacement tiles on the same cell!" });
				Util.KDestroyGameObject(base.gameObject);
			}
		}
		this.fetchList.Submit(new global::System.Action(this.OnFetchListComplete), true);
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		Prioritizable component2 = base.GetComponent<Prioritizable>();
		Prioritizable prioritizable = component2;
		prioritizable.onPriorityChanged = (Action<int>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<int>(this.OnPriorityChanged));
		this.OnPriorityChanged(component2.GetMasterPriority());
		Components.Constructables.Add(this);
	}

	private void OnPriorityChanged(int priority)
	{
		this.building.RunOnArea(delegate(int cell)
		{
			Diggable diggable = Diggable.GetDiggable(cell);
			if (diggable != null)
			{
				diggable.GetComponent<Prioritizable>().SetMasterPriority(priority);
			}
		});
	}

	private void MarkArea()
	{
		int num = Grid.PosToCell(this.transform.position);
		BuildingDef def = this.building.Def;
		Orientation orientation = this.building.Orientation;
		ObjectLayer objectLayer = ((!this.IsReplacementTile) ? def.ObjectLayer : def.ReplacementLayer);
		def.MarkArea(num, orientation, objectLayer, base.gameObject);
		if (def.IsTilePiece)
		{
			GameObject gameObject = Grid.Objects[num, (int)def.TileLayer];
			if (gameObject == null)
			{
				def.MarkArea(num, orientation, def.TileLayer, base.gameObject);
				def.RunOnArea(num, orientation, delegate(int c)
				{
					TileVisualizer.RefreshCell(c, def.TileLayer);
				});
			}
			Grid.IsTileUnderConstruction[num] = true;
		}
	}

	private void UnmarkArea()
	{
		int num = Grid.PosToCell(this.transform.position);
		ObjectLayer objectLayer = ((!this.IsReplacementTile) ? this.building.Def.ObjectLayer : this.building.Def.ReplacementLayer);
		this.building.Def.UnmarkArea(num, this.building.Orientation, objectLayer, base.gameObject);
		if (this.building.Def.IsTilePiece)
		{
			Grid.IsTileUnderConstruction[num] = false;
		}
	}

	public bool IconConnectionAnimation(float delay, int connectionCount, string defName, string soundName)
	{
		int num = Grid.PosToCell(this.transform.position);
		if (this.building.Def.Name.Contains(defName))
		{
			Building building = null;
			GameObject gameObject = Grid.Objects[num, 1];
			if (gameObject != null)
			{
				building = gameObject.GetComponent<Building>();
			}
			if (building != null)
			{
				bool flag = defName.Contains("Wire");
				int num2 = ((!flag) ? building.GetUtilityInputCell() : building.GetPowerInputCell());
				int num3 = ((!flag) ? building.GetUtilityOutputCell() : num2);
				if (num == num2 || num == num3)
				{
					BuildingCellVisualizer component = building.gameObject.GetComponent<BuildingCellVisualizer>();
					if (component != null)
					{
						bool flag2 = ((!flag) ? component.RequiresGasOrLiquid : component.RequiresPower);
						if (flag2)
						{
							component.ConnectedEventWithDelay(delay, connectionCount, num, soundName);
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		if (this.IsReplacementTile && this.building.Def.isKAnimTile)
		{
			int num = Grid.PosToCell(this.transform.position);
			GameObject gameObject = Grid.Objects[num, (int)this.building.Def.ReplacementLayer];
			if (gameObject == base.gameObject && gameObject.GetComponent<SimCellOccupier>() != null)
			{
				World.Instance.blockTileRenderer.RemoveBlock(this.building.Def, SimHashes.Void, num);
			}
		}
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		SaveLoadRoot component = base.GetComponent<SaveLoadRoot>();
		if (component != null)
		{
			SaveLoader.Instance.saveManager.Unregister(component);
		}
		foreach (int num2 in this.building.PlacementCells)
		{
			Diggable diggable = Diggable.GetDiggable(num2);
			if (diggable != null)
			{
				diggable.gameObject.DeleteObject();
			}
		}
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("Constructable destroyed");
		}
		Components.Constructables.Remove(this);
		this.UnmarkArea();
		base.OnCleanUp();
	}

	private void PlaceDiggables()
	{
		bool digs_complete = true;
		if (!this.IsReplacementTile)
		{
			this.building.RunOnArea(delegate(int offset_cell)
			{
				int masterPriority = this.GetComponent<Prioritizable>().GetMasterPriority();
				if (Diggable.IsDiggable(offset_cell))
				{
					digs_complete = false;
					Diggable diggable = Diggable.GetDiggable(offset_cell);
					if (diggable == null)
					{
						diggable = GameUtil.KInstantiate(EntityPrefabs.Instance.DigPlacer, Grid.SceneLayer.Move, Folder.Placers, null, 0).GetComponent<Diggable>();
						diggable.transform.SetPosition(Grid.CellToPosCBC(offset_cell, Grid.SceneLayer.Move));
					}
					diggable.SetChoreType(Db.Get().ChoreTypes.BuildDig);
					diggable.GetComponent<Prioritizable>().SetMasterPriority(masterPriority);
					RenderUtil.EnableRenderer(diggable.transform, false);
					SaveLoadRoot component = diggable.GetComponent<SaveLoadRoot>();
					if (component != null)
					{
						global::UnityEngine.Object.Destroy(component);
					}
				}
			});
		}
		bool flag = this.building.Def.IsValidBuildLocation(this.transform.position, this.building.Orientation);
		if (flag)
		{
			this.notifier.Remove(this.invalidLocation);
		}
		else
		{
			this.notifier.Add(this.invalidLocation, string.Empty);
		}
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidBuildingLocation, !flag, this);
		bool flag2 = digs_complete && flag;
		if (flag2 && this.buildChore == null)
		{
			Action<Chore> action = new Action<Chore>(this.UpdateBuildState);
			Action<Chore> action2 = new Action<Chore>(this.UpdateBuildState);
			this.buildChore = new WorkChore<Constructable>(this.choreType, this, null, true, action, action2, new Action<Chore>(this.UpdateBuildState), true, null, true, default(Tag), null, false, true);
			this.UpdateBuildState(this.buildChore);
		}
		else if (!flag2 && this.buildChore != null)
		{
			this.buildChore.Cancel("Need to dig");
			this.buildChore = null;
		}
	}

	private void OnFetchListComplete()
	{
		this.PlaceDiggables();
		Extents validPlacementExtents = this.building.GetValidPlacementExtents();
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.digDestroyedMask.mask | GameScenePartitioner.Instance.solidChangedMask.mask, new Action<object>(this.OnSolidChangedOrDigDestroyed));
		this.fetchList = null;
		this.ClearMaterialNeeds();
	}

	private void ClearMaterialNeeds()
	{
		if (this.materialNeedsCleared)
		{
			return;
		}
		foreach (Recipe.Ingredient ingredient in this.Recipe.GetAllIngredients(this.SelectedElements))
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, -ingredient.amount);
		}
		this.materialNeedsCleared = true;
	}

	private void OnSolidChangedOrDigDestroyed(object data)
	{
		if (this == null)
		{
			return;
		}
		this.PlaceDiggables();
	}

	private void UpdateBuildState(Chore chore)
	{
		if (chore.InProgress())
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstruction, null);
		}
		else
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstructionNoWorker, null);
		}
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "build", EffectPrefabs.Instance.BuildEffect);
		return anim;
	}

	[OnSerializing]
	internal void OnSerializing()
	{
		if (this.selectedElements != null)
		{
			this.ids = new int[this.selectedElements.Length];
			for (int i = 0; i < this.selectedElements.Length; i++)
			{
				this.ids[i] = (int)this.selectedElements[i].id;
			}
		}
	}

	[OnDeserialized]
	internal void OnDeserialized()
	{
		if (this.ids != null)
		{
			this.selectedElements = new Element[this.ids.Length];
			for (int i = 0; i < this.ids.Length; i++)
			{
				this.selectedElements[i] = ElementLoader.FindElementByHash((SimHashes)this.ids[i]);
			}
		}
	}

	private void OnReachableChanged(object data)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		bool flag = (bool)data;
		if (flag)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable, false);
			if (component != null)
			{
				component.TintColour = Game.Instance.uiColours.Build.validLocation;
			}
		}
		else
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable, this);
			if (component != null)
			{
				component.TintColour = Game.Instance.uiColours.Build.unreachable;
			}
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = this.userMenu;
		string text = UI.USERMENUACTIONS.CANCELCONSTRUCTION.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo("icon_cancel", UI.USERMENUACTIONS.CANCELCONSTRUCTION.NAME, new global::System.Action(this.OnPressCancel), global::Action.NumActions, null, null, null, text, true), 1f);
	}

	private void OnPressCancel()
	{
		if (this.Source != null)
		{
			this.Source.Trigger(2127324410, null);
		}
		base.gameObject.Trigger(2127324410, null);
	}

	private void OnCancel(object data = null)
	{
		DetailsScreen.Instance.Show(false);
		this.ClearMaterialNeeds();
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpAdd]
	private UserMenu userMenu;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Rotatable rotatable;

	private Notification invalidLocation;

	private float initialTemperature = -1f;

	[Serialize]
	private bool isPrioritized;

	private FetchList2 fetchList;

	private Chore buildChore;

	private ChoreType choreType;

	private bool materialNeedsCleared;

	[Serialize]
	private Ref<Relocatable> source = new Ref<Relocatable>();

	[Serialize]
	public bool isRelocating;

	public bool isDiggingRequired = true;

	[Serialize]
	public bool IsReplacementTile;

	private GameScenePartitionerEntry partitionerEntry;

	private LoggerFSS log = new LoggerFSS("Constructable");

	private Element[] selectedElements;

	[Serialize]
	private int[] ids;
}
