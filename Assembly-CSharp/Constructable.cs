using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Constructable")]
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

	protected override void OnCompleteWork(WorkerBase worker)
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
			this.initialTemperature = Mathf.Clamp(num2 / num, 0f, 318.15f);
		}
		KAnimGraphTileVisualizer component2 = base.GetComponent<KAnimGraphTileVisualizer>();
		UtilityConnections connections = ((component2 == null) ? ((UtilityConnections)0) : component2.Connections);
		bool flag2 = true;
		if (this.IsReplacementTile)
		{
			int num3 = Grid.PosToCell(base.transform.GetLocalPosition());
			GameObject replacementCandidate = this.building.Def.GetReplacementCandidate(num3);
			if (replacementCandidate != null)
			{
				flag2 = false;
				SimCellOccupier component3 = replacementCandidate.GetComponent<SimCellOccupier>();
				if (component3 != null)
				{
					component3.DestroySelf(delegate
					{
						if (this != null && this.gameObject != null)
						{
							this.FinishConstruction(connections, worker);
						}
					});
				}
				else
				{
					Conduit component4 = replacementCandidate.GetComponent<Conduit>();
					if (component4 != null)
					{
						component4.GetFlowManager().MarkForReplacement(num3);
					}
					BuildingComplete component5 = replacementCandidate.GetComponent<BuildingComplete>();
					if (component5 != null)
					{
						component5.Subscribe(-21016276, delegate(object data)
						{
							this.FinishConstruction(connections, worker);
						});
					}
					else
					{
						global::Debug.LogWarning("Why am I trying to replace a: " + replacementCandidate.name);
						this.FinishConstruction(connections, worker);
					}
				}
				KAnimGraphTileVisualizer component6 = replacementCandidate.GetComponent<KAnimGraphTileVisualizer>();
				if (component6 != null)
				{
					component6.skipCleanup = true;
				}
				Deconstructable component7 = replacementCandidate.GetComponent<Deconstructable>();
				if (component7 != null)
				{
					component7.SpawnItemsFromConstruction(worker);
				}
				Boxed<Constructable.ReplaceCallbackParameters> boxed = Boxed<Constructable.ReplaceCallbackParameters>.Get(new Constructable.ReplaceCallbackParameters
				{
					TileLayer = this.building.Def.TileLayer,
					Worker = worker
				});
				replacementCandidate.Trigger(1606648047, boxed);
				Boxed<Constructable.ReplaceCallbackParameters>.Release(boxed);
				replacementCandidate.DeleteObject();
			}
		}
		if (flag2)
		{
			this.FinishConstruction(connections, worker);
		}
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, base.GetComponent<KSelectable>().GetName(), base.transform, 1.5f, false);
	}

	private void FinishConstruction(UtilityConnections connections, WorkerBase workerForGameplayEvent)
	{
		Rotatable component = base.GetComponent<Rotatable>();
		Orientation orientation = ((component != null) ? component.GetOrientation() : Orientation.Neutral);
		int num = Grid.PosToCell(base.transform.GetLocalPosition());
		this.UnmarkArea();
		GameObject gameObject = this.building.Def.Build(num, orientation, this.storage, this.selectedElementsTags, this.initialTemperature, base.GetComponent<BuildingFacade>().CurrentFacade, true, GameClock.Instance.GetTime());
		BonusEvent.GameplayEventData gameplayEventData = new BonusEvent.GameplayEventData();
		gameplayEventData.building = gameObject.GetComponent<BuildingComplete>();
		gameplayEventData.workable = this;
		gameplayEventData.worker = workerForGameplayEvent;
		gameplayEventData.eventTrigger = GameHashes.NewBuilding;
		GameplayEventManager.Instance.Trigger(-1661515756, gameplayEventData);
		gameObject.transform.rotation = base.transform.rotation;
		Rotatable component2 = gameObject.GetComponent<Rotatable>();
		if (component2 != null)
		{
			component2.SetOrientation(orientation);
		}
		KAnimGraphTileVisualizer component3 = base.GetComponent<KAnimGraphTileVisualizer>();
		if (component3 != null)
		{
			gameObject.GetComponent<KAnimGraphTileVisualizer>().Connections = connections;
			component3.skipCleanup = true;
		}
		KSelectable component4 = base.GetComponent<KSelectable>();
		if (component4 != null && component4.IsSelected && gameObject.GetComponent<KSelectable>() != null)
		{
			component4.Unselect();
			if (PlayerController.Instance.ActiveTool.name == "SelectTool")
			{
				((SelectTool)PlayerController.Instance.ActiveTool).SelectNextFrame(gameObject.GetComponent<KSelectable>(), false);
			}
		}
		gameObject.Trigger(2121280625, this);
		this.storage.ConsumeAllIgnoringDisease();
		this.finished = true;
		this.DeleteObject();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.invalidLocation = new Notification(MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null, true, false, false);
		this.faceTargetWhenWorking = true;
		base.Subscribe<Constructable>(-1432940121, Constructable.OnReachableChangedDelegate);
		if (this.rotatable == null)
		{
			this.MarkArea();
		}
		if (Db.Get().TechItems.GetTechTierForItem(this.building.Def.PrefabID) > 2)
		{
			this.requireMinionToWork = true;
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
		this.workingPstComplete = null;
		this.workingPstFailed = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		CellOffset[][] array = OffsetGroups.InvertedStandardTable;
		if (this.building.Def.IsTilePiece)
		{
			array = OffsetGroups.InvertedStandardTableWithCorners;
		}
		CellOffset[] array2 = this.building.Def.PlacementOffsets;
		if (this.rotatable != null)
		{
			array2 = new CellOffset[this.building.Def.PlacementOffsets.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = this.rotatable.GetRotatedCellOffset(this.building.Def.PlacementOffsets[i]);
			}
		}
		CellOffset[][] array3 = OffsetGroups.BuildReachabilityTable(array2, array, this.building.Def.ConstructionOffsetFilter);
		base.SetOffsetTable(array3);
		this.storage.SetOffsetTable(array3);
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
		float num = component.Element.highTemp - 10f;
		component.Temperature = (component.Temperature = Mathf.Min(this.building.Def.Temperature, num));
		foreach (Recipe.Ingredient ingredient in this.Recipe.GetAllIngredients(this.selectedElementsTags))
		{
			this.fetchList.Add(ingredient.tag, null, ingredient.amount, Operational.State.None);
			MaterialNeeds.UpdateNeed(ingredient.tag, ingredient.amount, base.gameObject.GetMyWorldId());
		}
		if (!this.building.Def.IsTilePiece)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Construction");
		}
		this.building.RunOnArea(delegate(int offset_cell)
		{
			if (base.gameObject.GetComponent<ConduitBridge>() == null)
			{
				GameObject gameObject3 = Grid.Objects[offset_cell, 7];
				if (gameObject3 != null)
				{
					gameObject3.DeleteObject();
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
			GameObject gameObject2 = Grid.Objects[num2, (int)this.building.Def.ObjectLayer];
			if (gameObject2 != null)
			{
				Deconstructable component2 = gameObject2.GetComponent<Deconstructable>();
				if (component2 != null)
				{
					component2.CancelDeconstruction();
				}
			}
		}
		bool flag = this.building.Def.BuildingComplete.GetComponent<Ladder>();
		this.waitForFetchesBeforeDigging = flag || this.building.Def.BuildingComplete.GetComponent<SimCellOccupier>() || this.building.Def.BuildingComplete.GetComponent<Door>() || this.building.Def.BuildingComplete.GetComponent<LiquidPumpingStation>();
		if (flag)
		{
			int num4 = 0;
			int num5 = 0;
			Grid.CellToXY(Grid.PosToCell(this), out num4, out num5);
			int num6 = num5 - 3;
			this.ladderDetectionExtents = new Extents(num4, num6, 1, 5);
			this.ladderPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnNearbyBuildingLayerChanged", base.gameObject, this.ladderDetectionExtents, GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnNearbyBuildingLayerChanged));
			this.OnNearbyBuildingLayerChanged(null);
		}
		this.fetchList.Submit(new global::System.Action(this.OnFetchListComplete), true);
		this.PlaceDiggables();
		new ReachabilityMonitor.Instance(this).StartSM();
		base.Subscribe<Constructable>(493375141, Constructable.OnRefreshUserMenuDelegate);
		Prioritizable component3 = base.GetComponent<Prioritizable>();
		Prioritizable prioritizable = component3;
		prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<PrioritySetting>(this.OnPriorityChanged));
		this.OnPriorityChanged(component3.GetMasterPriority());
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
		ObjectLayer objectLayer = (this.IsReplacementTile ? def.ReplacementLayer : def.ObjectLayer);
		def.MarkArea(num, orientation, objectLayer, base.gameObject);
		if (def.IsTilePiece)
		{
			if (Grid.Objects[num, (int)def.TileLayer] == null)
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
		ObjectLayer objectLayer = (this.IsReplacementTile ? this.building.Def.ReplacementLayer : this.building.Def.ObjectLayer);
		def.UnmarkArea(num, this.building.Orientation, objectLayer, base.gameObject);
		if (def.IsTilePiece)
		{
			Grid.IsTileUnderConstruction[num] = false;
		}
		this.ClearPendingUproots();
	}

	private void ClearPendingUproots()
	{
		foreach (Uprootable uprootable in this.pendingUproots)
		{
			if (!uprootable.IsNullOrDestroyed())
			{
				uprootable.Unsubscribe(-216549700, new Action<object>(this.OnSolidChangedOrDigDestroyed));
				uprootable.Unsubscribe(1198393204, new Action<object>(this.OnSolidChangedOrDigDestroyed));
				uprootable.ForceCancelUproot(null);
			}
		}
		this.pendingUproots.Clear();
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
					return;
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
				int num2 = (flag ? building.GetPowerInputCell() : building.GetUtilityInputCell());
				int num3 = (flag ? num2 : building.GetUtilityOutputCell());
				if (num == num2 || num == num3)
				{
					BuildingCellVisualizer component = building.gameObject.GetComponent<BuildingCellVisualizer>();
					if (component != null && (flag ? ((component.addedPorts & (EntityCellVisualizer.Ports.PowerIn | EntityCellVisualizer.Ports.PowerOut)) > (EntityCellVisualizer.Ports)0) : ((component.addedPorts & (EntityCellVisualizer.Ports.GasIn | EntityCellVisualizer.Ports.GasOut | EntityCellVisualizer.Ports.LiquidIn | EntityCellVisualizer.Ports.LiquidOut | EntityCellVisualizer.Ports.SolidIn | EntityCellVisualizer.Ports.SolidOut)) > (EntityCellVisualizer.Ports)0)))
					{
						component.ConnectedEventWithDelay(delay, connectionCount, num, soundName);
						return true;
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
		GameScenePartitioner.Instance.Free(ref this.ladderPartitionerEntry);
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
		HashSetPool<Uprootable, Constructable>.PooledHashSet pooledHashSet = HashSetPool<Uprootable, Constructable>.Allocate();
		foreach (int num2 in this.building.PlacementCells)
		{
			Diggable diggable = Diggable.GetDiggable(num2);
			if (diggable != null)
			{
				diggable.gameObject.DeleteObject();
			}
			Constructable.<OnCleanUp>g__TryAddUprootable|48_0(Grid.Objects[num2, 1], pooledHashSet);
			Constructable.<OnCleanUp>g__TryAddUprootable|48_0(Grid.Objects[num2, 5], pooledHashSet);
		}
		foreach (Uprootable uprootable in pooledHashSet)
		{
			uprootable.Unsubscribe(-216549700, new Action<object>(this.OnSolidChangedOrDigDestroyed));
			uprootable.Unsubscribe(1198393204, new Action<object>(this.OnSolidChangedOrDigDestroyed));
			uprootable.ForceCancelUproot(null);
		}
		pooledHashSet.Recycle();
		base.OnCleanUp();
	}

	private void OnDiggableReachabilityChanged(object _)
	{
		if (!this.IsReplacementTile)
		{
			int diggable_count = 0;
			int unreachable_count = 0;
			this.building.RunOnArea(delegate(int offset_cell)
			{
				Diggable diggable = Diggable.GetDiggable(offset_cell);
				if (diggable != null && diggable.isActiveAndEnabled)
				{
					int num = diggable_count + 1;
					diggable_count = num;
					if (!diggable.GetComponent<KPrefabID>().HasTag(GameTags.Reachable))
					{
						num = unreachable_count + 1;
						unreachable_count = num;
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
		if (!this.solidPartitionerEntry.IsValid())
		{
			Extents validPlacementExtents = this.building.GetValidPlacementExtents();
			this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.PlaceDiggables", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChangedOrDigDestroyed));
			this.digPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.PlaceDiggables", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.digDestroyedLayer, new Action<object>(this.OnSolidChangedOrDigDestroyed));
		}
		bool digs_complete = true;
		if (!this.IsReplacementTile)
		{
			PrioritySetting master_priority = base.GetComponent<Prioritizable>().GetMasterPriority();
			HashSetPool<Uprootable, Constructable>.PooledHashSet uprootables = HashSetPool<Uprootable, Constructable>.Allocate();
			this.building.RunOnArea(delegate(int offset_cell)
			{
				Uprootable uprootable4;
				if (Diggable.IsDiggable(offset_cell))
				{
					digs_complete = false;
					Diggable diggable = Diggable.GetDiggable(offset_cell);
					if (diggable != null && !diggable.isActiveAndEnabled)
					{
						diggable.Unsubscribe(-1432940121, new Action<object>(this.OnDiggableReachabilityChanged));
						diggable = null;
					}
					if (diggable == null)
					{
						diggable = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), Grid.SceneLayer.Move, null, 0).GetComponent<Diggable>();
						diggable.choreTypeIdHash = Db.Get().ChoreTypes.BuildDig.IdHash;
						diggable.gameObject.SetActive(true);
						diggable.transform.SetPosition(Grid.CellToPosCBC(offset_cell, Grid.SceneLayer.Move));
						Grid.Objects[offset_cell, 7] = diggable.gameObject;
						diggable.Subscribe(-1432940121, new Action<object>(this.OnDiggableReachabilityChanged));
					}
					diggable.GetComponent<Prioritizable>().SetMasterPriority(master_priority);
					RenderUtil.EnableRenderer(diggable.transform, false);
					SaveLoadRoot component = diggable.GetComponent<SaveLoadRoot>();
					if (component != null)
					{
						global::UnityEngine.Object.Destroy(component);
						return;
					}
				}
				else if (this.building.Def.ObjectLayer == ObjectLayer.Building && Uprootable.CanUproot(Grid.Objects[offset_cell, 5], out uprootable4))
				{
					digs_complete = false;
					uprootables.Add(uprootable4);
				}
			});
			ListPool<Uprootable, Constructable>.PooledList pooledList = ListPool<Uprootable, Constructable>.Allocate();
			foreach (Uprootable uprootable in this.pendingUproots)
			{
				if (uprootable.IsNullOrDestroyed())
				{
					pooledList.Add(uprootable);
				}
				else if (!uprootables.Contains(uprootable))
				{
					uprootable.Unsubscribe(-216549700, new Action<object>(this.OnSolidChangedOrDigDestroyed));
					uprootable.Unsubscribe(1198393204, new Action<object>(this.OnSolidChangedOrDigDestroyed));
					pooledList.Add(uprootable);
				}
			}
			foreach (Uprootable uprootable2 in pooledList)
			{
				this.pendingUproots.Remove(uprootable2);
			}
			pooledList.Recycle();
			foreach (Uprootable uprootable3 in uprootables)
			{
				bool flag = this.pendingUproots.Add(uprootable3);
				uprootable3.choreTypeIdHash = Db.Get().ChoreTypes.BuildUproot.IdHash;
				uprootable3.MarkForUproot(true);
				if (flag)
				{
					uprootable3.Subscribe(-216549700, new Action<object>(this.OnSolidChangedOrDigDestroyed));
					uprootable3.Subscribe(1198393204, new Action<object>(this.OnSolidChangedOrDigDestroyed));
				}
			}
			uprootables.Recycle();
			this.OnDiggableReachabilityChanged(null);
		}
		bool flag2 = this.building.Def.IsValidBuildLocation(base.gameObject, base.transform.GetPosition(), this.building.Orientation, this.IsReplacementTile);
		if (flag2)
		{
			this.notifier.Remove(this.invalidLocation);
		}
		else
		{
			this.notifier.Add(this.invalidLocation, "");
		}
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidBuildingLocation, !flag2, this);
		bool flag3 = digs_complete && flag2 && this.fetchList == null;
		if (flag3 && this.buildChore == null)
		{
			this.buildChore = new WorkChore<Constructable>(Db.Get().ChoreTypes.Build, this, null, true, new Action<Chore>(this.UpdateBuildState), new Action<Chore>(this.UpdateBuildState), new Action<Chore>(this.UpdateBuildState), true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			this.UpdateBuildState(this.buildChore);
			return;
		}
		if (!flag3 && this.buildChore != null)
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
			MaterialNeeds.UpdateNeed(ingredient.tag, -ingredient.amount, base.gameObject.GetMyWorldId());
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
		StatusItem statusItem = (chore.InProgress() ? Db.Get().BuildingStatusItems.UnderConstruction : Db.Get().BuildingStatusItems.UnderConstructionNoWorker);
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, statusItem, null);
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
		if (((Boxed<bool>)data).value)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable, false);
			if (component != null)
			{
				component.TintColour = Game.Instance.uiColours.Build.validLocation;
				return;
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
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", UI.USERMENUACTIONS.CANCELCONSTRUCTION.NAME, new global::System.Action(this.OnPressCancel), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELCONSTRUCTION.TOOLTIP, true), 1f);
	}

	private void OnPressCancel()
	{
		base.gameObject.Trigger(2127324410, null);
	}

	private void OnCancel(object _ = null)
	{
		DetailsScreen.Instance.Show(false);
		this.ClearMaterialNeeds();
		this.ClearPendingUproots();
	}

	[CompilerGenerated]
	internal static void <OnCleanUp>g__TryAddUprootable|48_0(GameObject plant, HashSet<Uprootable> _uprootables)
	{
		if (plant == null)
		{
			return;
		}
		Uprootable component = plant.GetComponent<Uprootable>();
		if (component == null)
		{
			return;
		}
		_uprootables.Add(component);
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

	private HandleVector<int>.Handle ladderPartitionerEntry;

	private readonly HashSet<Uprootable> pendingUproots = new HashSet<Uprootable>();

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

	public struct ReplaceCallbackParameters
	{
		public ObjectLayer TileLayer;

		public WorkerBase Worker;
	}
}
