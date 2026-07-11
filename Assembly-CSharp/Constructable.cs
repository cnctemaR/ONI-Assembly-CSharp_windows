using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Constructable : Workable, ISaveLoadable
{
	public Recipe Recipe
	{
		get
		{
			return this.building.Def.CraftRecipe;
		}
	}

	public IList<Tag> SelectedElementsTags
	{
		get
		{
			return this.selectedElementsTags;
		}
		set
		{
			if (this.selectedElementsTags == null || this.selectedElementsTags.Length != value.Count)
			{
				this.selectedElementsTags = new Tag[value.Count];
			}
			value.CopyTo(this.selectedElementsTags, 0);
		}
	}

	public override string GetConversationTopic()
	{
		return this.building.Def.PrefabID;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		float num = 0f;
		float num2 = 0f;
		bool flag = true;
		foreach (GameObject gameObject in this.storage.items)
		{
			if (!(gameObject == null))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (!(component == null))
				{
					num += component.Mass;
					num2 += component.Temperature * component.Mass;
					flag = flag && component.HasTag(GameTags.Liquifiable);
				}
			}
		}
		if (num <= 0f)
		{
			DebugUtil.LogWarningArgs(base.gameObject, new object[]
			{
				"uhhh this constructable is about to generate a nan",
				"Item Count: ",
				this.storage.items.Count
			});
			return;
		}
		if (flag)
		{
			this.initialTemperature = Mathf.Min(num2 / num, 318.15f);
		}
		else
		{
			this.initialTemperature = Mathf.Clamp(num2 / num, 288.15f, 318.15f);
		}
		KAnimGraphTileVisualizer component2 = base.GetComponent<KAnimGraphTileVisualizer>();
		UtilityConnections connections = ((!(component2 == null)) ? component2.Connections : ((UtilityConnections)0));
		if (this.IsReplacementTile)
		{
			int num3 = Grid.PosToCell(base.transform.GetLocalPosition());
			GameObject replacementCandidate = this.building.Def.GetReplacementCandidate(num3);
			if (replacementCandidate != null)
			{
				SimCellOccupier component3 = replacementCandidate.GetComponent<SimCellOccupier>();
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
					Conduit component4 = replacementCandidate.GetComponent<Conduit>();
					if (component4 != null)
					{
						ConduitFlow flowManager = component4.GetFlowManager();
						flowManager.MarkForReplacement(num3);
					}
					BuildingComplete component5 = replacementCandidate.GetComponent<BuildingComplete>();
					if (component5 != null)
					{
						component5.Subscribe(-21016276, delegate(object data)
						{
							this.FinishConstruction(connections);
						});
					}
					else
					{
						global::Debug.LogWarning("Why am I trying to replace a: " + replacementCandidate.name);
						this.FinishConstruction(connections);
					}
				}
				KAnimGraphTileVisualizer component6 = replacementCandidate.GetComponent<KAnimGraphTileVisualizer>();
				if (component6 != null)
				{
					component6.skipCleanup = true;
				}
				PrimaryElement component7 = replacementCandidate.GetComponent<PrimaryElement>();
				float mass = component7.Mass;
				float temperature = component7.Temperature;
				byte diseaseIdx = component7.DiseaseIdx;
				int diseaseCount = component7.DiseaseCount;
				global::Debug.Assert(component7.Element != null && component7.Element.tag != null);
				Deconstructable.SpawnItem(component7.transform.GetPosition(), component7.GetComponent<Building>().Def, component7.Element.tag, mass, temperature, diseaseIdx, diseaseCount);
				replacementCandidate.Trigger(1606648047, null);
				replacementCandidate.DeleteObject();
			}
		}
		else
		{
			this.FinishConstruction(connections);
		}
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, base.GetComponent<KSelectable>().GetName(), base.transform, 1.5f, false);
	}

	private void FinishConstruction(UtilityConnections connections)
	{
		Rotatable component = base.GetComponent<Rotatable>();
		Orientation orientation = ((!(component != null)) ? Orientation.Neutral : component.GetOrientation());
		int num = Grid.PosToCell(base.transform.GetLocalPosition());
		this.UnmarkArea();
		GameObject gameObject = this.building.Def.Build(num, orientation, this.storage, this.selectedElementsTags, this.initialTemperature, true);
		gameObject.transform.rotation = base.transform.rotation;
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
		this.storage.ConsumeAllIgnoringDisease();
		this.finished = true;
		this.DeleteObject();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.invalidLocation = new Notification(MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);
		CellOffset[][] array = OffsetGroups.InvertedStandardTable;
		if (this.building.Def.IsTilePiece)
		{
			array = OffsetGroups.InvertedStandardTableWithCorners;
		}
		CellOffset[][] array2 = OffsetGroups.BuildReachabilityTable(this.building.Def.PlacementOffsets, array, this.building.Def.ConstructionOffsetFilter);
		base.SetOffsetTable(array2);
		this.storage.SetOffsetTable(array2);
		this.faceTargetWhenWorking = true;
		base.Subscribe<Constructable>(-1432940121, Constructable.OnReachableChangedDelegate);
		if (this.rotatable == null)
		{
			this.MarkArea();
		}
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Building;
		this.workingStatusItem = null;
		this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.minimumAttributeMultiplier = 0.75f;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
		this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		Prioritizable.AddRef(base.gameObject);
		this.synchronizeAnims = false;
		this.multitoolContext = "build";
		this.multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		this.workingPstComplete = HashedString.Invalid;
		this.workingPstFailed = HashedString.Invalid;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<Constructable>(2127324410, Constructable.OnCancelDelegate);
		if (this.rotatable != null)
		{
			this.MarkArea();
		}
		this.fetchList = new FetchList2(this.storage, Db.Get().ChoreTypes.BuildFetch);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		Element element = ElementLoader.GetElement(this.SelectedElementsTags[0]);
		global::Debug.Assert(element != null, "Missing primary element for Constructable");
		component.ElementID = element.id;
		PrimaryElement primaryElement = component;
		float num = 293.15f;
		component.Temperature = num;
		primaryElement.Temperature = num;
		foreach (Recipe.Ingredient ingredient in this.Recipe.GetAllIngredients(this.selectedElementsTags))
		{
			FetchList2 fetchList = this.fetchList;
			Tag tag = ingredient.tag;
			num = ingredient.amount;
			fetchList.Add(tag, null, null, num, FetchOrder2.OperationalRequirement.None);
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
			int num2 = Grid.PosToCell(base.transform.GetPosition());
			GameObject gameObject = Grid.Objects[num2, (int)this.building.Def.ReplacementLayer];
			if (gameObject == null || gameObject == base.gameObject)
			{
				Grid.Objects[num2, (int)this.building.Def.ReplacementLayer] = base.gameObject;
				if (base.gameObject.GetComponent<SimCellOccupier>() != null)
				{
					int num3 = LayerMask.NameToLayer("Overlay");
					World.Instance.blockTileRenderer.AddBlock(num3, this.building.Def, this.IsReplacementTile, SimHashes.Void, num2);
				}
				TileVisualizer.RefreshCell(num2, this.building.Def.TileLayer, this.building.Def.ReplacementLayer);
			}
			else
			{
				global::Debug.LogError("multiple replacement tiles on the same cell!");
				Util.KDestroyGameObject(base.gameObject);
			}
		}
		bool flag = this.building.Def.BuildingComplete.GetComponent<Ladder>();
		this.waitForFetchesBeforeDigging = flag || this.building.Def.BuildingComplete.GetComponent<SimCellOccupier>() || this.building.Def.BuildingComplete.GetComponent<Door>() || this.building.Def.BuildingComplete.GetComponent<LiquidPumpingStation>();
		if (flag)
		{
			int num4 = 0;
			int num5 = 0;
			int num6 = Grid.PosToCell(this);
			Grid.CellToXY(num6, out num4, out num5);
			int num7 = num5 - 3;
			this.ladderDetectionExtents = new Extents(num4, num7, 1, 5);
			this.ladderParititonerEntry = GameScenePartitioner.Instance.Add("Constructable.OnNearbyBuildingLayerChanged", base.gameObject, this.ladderDetectionExtents, GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnNearbyBuildingLayerChanged));
			this.OnNearbyBuildingLayerChanged(null);
		}
		this.fetchList.Submit(new global::System.Action(this.OnFetchListComplete), true);
		this.PlaceDiggables();
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
		base.Subscribe<Constructable>(493375141, Constructable.OnRefreshUserMenuDelegate);
		Prioritizable component2 = base.GetComponent<Prioritizable>();
		Prioritizable prioritizable = component2;
		prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<PrioritySetting>(this.OnPriorityChanged));
		this.OnPriorityChanged(component2.GetMasterPriority());
	}

	private void OnPriorityChanged(PrioritySetting priority)
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
		int num = Grid.PosToCell(base.transform.GetPosition());
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
					TileVisualizer.RefreshCell(c, def.TileLayer, def.ReplacementLayer);
				});
			}
			Grid.IsTileUnderConstruction[num] = true;
		}
	}

	private void UnmarkArea()
	{
		if (this.unmarked)
		{
			return;
		}
		this.unmarked = true;
		int num = Grid.PosToCell(base.transform.GetPosition());
		BuildingDef def = this.building.Def;
		ObjectLayer objectLayer = ((!this.IsReplacementTile) ? this.building.Def.ObjectLayer : this.building.Def.ReplacementLayer);
		def.UnmarkArea(num, this.building.Orientation, objectLayer, base.gameObject);
		if (def.IsTilePiece)
		{
			Grid.IsTileUnderConstruction[num] = false;
		}
	}

	private void OnNearbyBuildingLayerChanged(object data)
	{
		this.hasLadderNearby = false;
		for (int i = this.ladderDetectionExtents.y; i < this.ladderDetectionExtents.y + this.ladderDetectionExtents.height; i++)
		{
			int num = Grid.OffsetCell(0, this.ladderDetectionExtents.x, i);
			if (Grid.IsValidCell(num))
			{
				GameObject gameObject = null;
				Grid.ObjectLayers[1].TryGetValue(num, out gameObject);
				if (gameObject != null && gameObject.GetComponent<Ladder>() != null)
				{
					this.hasLadderNearby = true;
					break;
				}
			}
		}
	}

	private bool IsWire()
	{
		return this.building.Def.name.Contains("Wire");
	}

	public bool IconConnectionAnimation(float delay, int connectionCount, string defName, string soundName)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
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
				bool flag = this.IsWire();
				int num2 = ((!flag) ? building.GetUtilityInputCell() : building.GetPowerInputCell());
				int num3 = ((!flag) ? building.GetUtilityOutputCell() : num2);
				if (num == num2 || num == num3)
				{
					BuildingCellVisualizer component = building.gameObject.GetComponent<BuildingCellVisualizer>();
					if (component != null)
					{
						bool flag2 = ((!flag) ? component.RequiresUtilityConnection : component.RequiresPower);
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
			int num = Grid.PosToCell(base.transform.GetPosition());
			GameObject gameObject = Grid.Objects[num, (int)this.building.Def.ReplacementLayer];
			if (gameObject == base.gameObject && gameObject.GetComponent<SimCellOccupier>() != null)
			{
				World.Instance.blockTileRenderer.RemoveBlock(this.building.Def, this.IsReplacementTile, SimHashes.Void, num);
			}
		}
		GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref this.digPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref this.ladderParititonerEntry);
		SaveLoadRoot component = base.GetComponent<SaveLoadRoot>();
		if (component != null)
		{
			SaveLoader.Instance.saveManager.Unregister(component);
		}
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("Constructable destroyed");
		}
		this.UnmarkArea();
		foreach (int num2 in this.building.PlacementCells)
		{
			Diggable diggable = Diggable.GetDiggable(num2);
			if (diggable != null)
			{
				diggable.gameObject.DeleteObject();
			}
		}
		base.OnCleanUp();
	}

	private void OnDiggableReachabilityChanged(object data)
	{
		if (!this.IsReplacementTile)
		{
			int diggable_count = 0;
			int unreachable_count = 0;
			this.building.RunOnArea(delegate(int offset_cell)
			{
				Diggable diggable = Diggable.GetDiggable(offset_cell);
				if (diggable != null)
				{
					diggable_count++;
					if (!diggable.GetComponent<KPrefabID>().HasTag(GameTags.Reachable))
					{
						unreachable_count++;
					}
				}
			});
			bool flag = unreachable_count > 0 && unreachable_count == diggable_count;
			if (flag != this.hasUnreachableDigs)
			{
				if (flag)
				{
					base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ConstructableDigUnreachable, null);
				}
				else
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructableDigUnreachable, false);
				}
				this.hasUnreachableDigs = flag;
			}
		}
	}

	private void PlaceDiggables()
	{
		if (this.waitForFetchesBeforeDigging && this.fetchList != null && !this.hasLadderNearby)
		{
			this.OnDiggableReachabilityChanged(null);
			return;
		}
		bool digs_complete = true;
		if (!this.solidPartitionerEntry.IsValid())
		{
			Extents validPlacementExtents = this.building.GetValidPlacementExtents();
			this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChangedOrDigDestroyed));
			this.digPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.digDestroyedLayer, new Action<object>(this.OnSolidChangedOrDigDestroyed));
		}
		if (!this.IsReplacementTile)
		{
			this.building.RunOnArea(delegate(int offset_cell)
			{
				PrioritySetting masterPriority = this.GetComponent<Prioritizable>().GetMasterPriority();
				if (Diggable.IsDiggable(offset_cell))
				{
					digs_complete = false;
					Diggable diggable = Diggable.GetDiggable(offset_cell);
					if (diggable == null)
					{
						diggable = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), Grid.SceneLayer.Move, null, 0).GetComponent<Diggable>();
						diggable.gameObject.SetActive(true);
						diggable.transform.SetPosition(Grid.CellToPosCBC(offset_cell, Grid.SceneLayer.Move));
						diggable.Subscribe(-1432940121, new Action<object>(this.OnDiggableReachabilityChanged));
						Grid.Objects[offset_cell, 7] = diggable.gameObject;
					}
					else
					{
						diggable.Unsubscribe(-1432940121, new Action<object>(this.OnDiggableReachabilityChanged));
						diggable.Subscribe(-1432940121, new Action<object>(this.OnDiggableReachabilityChanged));
					}
					diggable.choreTypeIdHash = Db.Get().ChoreTypes.BuildDig.IdHash;
					diggable.GetComponent<Prioritizable>().SetMasterPriority(masterPriority);
					RenderUtil.EnableRenderer(diggable.transform, false);
					SaveLoadRoot component = diggable.GetComponent<SaveLoadRoot>();
					if (component != null)
					{
						global::UnityEngine.Object.Destroy(component);
					}
				}
			});
			this.OnDiggableReachabilityChanged(null);
		}
		bool flag = this.building.Def.IsValidBuildLocation(base.gameObject, base.transform.GetPosition(), this.building.Orientation);
		if (flag)
		{
			this.notifier.Remove(this.invalidLocation);
		}
		else
		{
			this.notifier.Add(this.invalidLocation, string.Empty);
		}
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidBuildingLocation, !flag, this);
		bool flag2 = digs_complete && flag && this.fetchList == null;
		if (flag2 && this.buildChore == null)
		{
			this.buildChore = new WorkChore<Constructable>(Db.Get().ChoreTypes.Build, this, null, true, new Action<Chore>(this.UpdateBuildState), new Action<Chore>(this.UpdateBuildState), new Action<Chore>(this.UpdateBuildState), true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
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
		this.fetchList = null;
		this.PlaceDiggables();
		this.ClearMaterialNeeds();
	}

	private void ClearMaterialNeeds()
	{
		if (this.materialNeedsCleared)
		{
			return;
		}
		foreach (Recipe.Ingredient ingredient in this.Recipe.GetAllIngredients(this.SelectedElementsTags))
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, -ingredient.amount);
		}
		this.materialNeedsCleared = true;
	}

	private void OnSolidChangedOrDigDestroyed(object data)
	{
		if (this == null || this.finished)
		{
			return;
		}
		this.PlaceDiggables();
	}

	private void UpdateBuildState(Chore chore)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		if (chore.InProgress())
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstruction, null);
		}
		else
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstructionNoWorker, null);
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
			if (this.selectedElementsTags == null)
			{
				this.selectedElementsTags = new Tag[this.ids.Length];
				for (int j = 0; j < this.ids.Length; j++)
				{
					this.selectedElementsTags[j] = ElementLoader.FindElementByHash((SimHashes)this.ids[j]).tag;
				}
			}
			global::Debug.Assert(this.selectedElements.Length == this.selectedElementsTags.Length);
			for (int k = 0; k < this.selectedElements.Length; k++)
			{
				global::Debug.Assert(this.selectedElements[k].tag == this.SelectedElementsTags[k]);
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
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string text = "action_cancel";
		string text2 = UI.USERMENUACTIONS.CANCELCONSTRUCTION.NAME;
		global::System.Action action = new global::System.Action(this.OnPressCancel);
		string text3 = UI.USERMENUACTIONS.CANCELCONSTRUCTION.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
	}

	private void OnPressCancel()
	{
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

	private bool materialNeedsCleared;

	private bool hasUnreachableDigs;

	private bool finished;

	private bool unmarked;

	public bool isDiggingRequired = true;

	private bool waitForFetchesBeforeDigging;

	private bool hasLadderNearby;

	private Extents ladderDetectionExtents;

	[Serialize]
	public bool IsReplacementTile;

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle digPartitionerEntry;

	private HandleVector<int>.Handle ladderParititonerEntry;

	private LoggerFSS log = new LoggerFSS("Constructable", 35);

	[Serialize]
	private Tag[] selectedElementsTags;

	private Element[] selectedElements;

	[Serialize]
	private int[] ids;

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
