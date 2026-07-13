using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using STRINGS;
using UnityEngine;

public class Navigator : StateMachineComponent<Navigator.StatesInstance>, ISaveLoadableDetails
{
	public KMonoBehaviour target { get; set; }

	public CellOffset[] targetOffsets { get; private set; }

	public NavGrid NavGrid { get; private set; }

	public PathGrid PathGrid { get; set; }

	public void Serialize(BinaryWriter writer)
	{
		byte currentNavType = (byte)this.CurrentNavType;
		writer.Write(currentNavType);
		writer.Write(this.distanceTravelledByNavType.Count);
		foreach (KeyValuePair<NavType, int> keyValuePair in this.distanceTravelledByNavType)
		{
			byte key = (byte)keyValuePair.Key;
			writer.Write(key);
			writer.Write(keyValuePair.Value);
		}
	}

	public void Deserialize(IReader reader)
	{
		NavType navType = (NavType)reader.ReadByte();
		if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 11))
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				NavType navType2 = (NavType)reader.ReadByte();
				int num2 = reader.ReadInt32();
				if (this.distanceTravelledByNavType.ContainsKey(navType2))
				{
					this.distanceTravelledByNavType[navType2] = num2;
				}
			}
		}
		bool flag = false;
		NavType[] validNavTypes = this.NavGrid.ValidNavTypes;
		for (int j = 0; j < validNavTypes.Length; j++)
		{
			if (validNavTypes[j] == navType)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.CurrentNavType = navType;
		}
	}

	protected override void OnPrefabInit()
	{
		this.transitionDriver = new TransitionDriver(this);
		this.targetLocator = Util.KInstantiate(Assets.GetPrefab(TargetLocator.ID), null, null).GetComponent<KPrefabID>();
		this.targetLocator.gameObject.SetActive(true);
		this.log = new LoggerFSS("Navigator", 35);
		this.simRenderLoadBalance = true;
		this.autoRegisterSimRender = false;
		this.NavGrid = Pathfinding.Instance.GetNavGrid(this.NavGridName);
		if (this.maxProbeRadiusX != 0 || this.maxProbeRadiusY != 0)
		{
			this.PathGrid = new PathGrid(this.maxProbeRadiusX * 2 + 1, this.maxProbeRadiusY * 2 + 1, true, this.NavGrid.ValidNavTypes);
		}
		else
		{
			this.PathGrid = new PathGrid(Grid.WidthInCells, Grid.HeightInCells, false, this.NavGrid.ValidNavTypes);
		}
		this.distanceTravelledByNavType = new Dictionary<NavType, int>();
		for (int i = 0; i < 11; i++)
		{
			this.distanceTravelledByNavType.Add((NavType)i, 0);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<Navigator>(1623392196, Navigator.OnDefeatedDelegate);
		base.Subscribe<Navigator>(-1506500077, Navigator.OnDefeatedDelegate);
		base.Subscribe<Navigator>(493375141, Navigator.OnRefreshUserMenuDelegate);
		base.Subscribe<Navigator>(-1503271301, Navigator.OnSelectObjectDelegate);
		base.Subscribe<Navigator>(856640610, Navigator.OnStoreDelegate);
		base.Subscribe<Navigator>(1502190696, Navigator.OnQueueDestroyDelegate);
		if (this.updateProber)
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}
		if (this.executePathProbeTaskAsync)
		{
			AsyncPathProber.Instance.Register(this);
		}
		this.cachedCell = Grid.PosToCell(this);
		this.SetCurrentNavType(this.CurrentNavType);
		this.OnBuildingTileChangedAction = new Action<int, object>(this.OnBuildingTileChanged);
		this.SubscribeUnstuckFunctions();
	}

	private void SubscribeUnstuckFunctions()
	{
		if (this.CurrentNavType == NavType.Tube)
		{
			GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], this.OnBuildingTileChangedAction);
		}
	}

	private void UnsubscribeUnstuckFunctions()
	{
		GameScenePartitioner.Instance.RemoveGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], this.OnBuildingTileChangedAction);
	}

	private void OnBuildingTileChanged(int cell, object building)
	{
		if (this.CurrentNavType == NavType.Tube && building == null)
		{
			bool flag = cell == Grid.PosToCell(this);
			if (base.smi != null && flag)
			{
				this.SetCurrentNavType(NavType.Floor);
				this.UnsubscribeUnstuckFunctions();
			}
		}
	}

	protected override void OnCleanUp()
	{
		this.UnsubscribeUnstuckFunctions();
		base.OnCleanUp();
	}

	protected void OnQueueDestroy()
	{
		if (this.executePathProbeTaskAsync)
		{
			AsyncPathProber.Instance.Unregister(this);
		}
		if (this.reportOccupation)
		{
			MinionGroupProber.Get().Vacate(this.occupiedCells);
		}
	}

	public PathGrid TakeResult(ref AsyncPathProber.WorkResult result)
	{
		PathGrid pathGrid = this.PathGrid;
		this.PathGrid = result.pathGrid;
		if (this.reportOccupation)
		{
			List<int> list = this.occupiedCells;
			MinionGroupProber.Get().OccupyST(result.newlyReachableCells);
			MinionGroupProber.Get().VacateST(result.noLongerReachableCells);
			this.occupiedCells = result.reachableCells;
			result.reachableCells = list;
		}
		return pathGrid;
	}

	public bool IsMoving()
	{
		return base.smi.IsInsideState(base.smi.sm.normal.moving);
	}

	public bool GoTo(int cell, CellOffset[] offsets = null)
	{
		if (offsets == null)
		{
			offsets = new CellOffset[1];
		}
		this.targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
		return this.GoTo(this.targetLocator, offsets, NavigationTactics.ReduceTravelDistance);
	}

	public bool GoTo(int cell, CellOffset[] offsets, NavTactic tactic)
	{
		if (offsets == null)
		{
			offsets = new CellOffset[1];
		}
		this.targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
		return this.GoTo(this.targetLocator, offsets, tactic);
	}

	public void UpdateTarget(int cell)
	{
		this.targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
	}

	public bool GoTo(KMonoBehaviour target, CellOffset[] offsets, NavTactic tactic)
	{
		if (tactic == null)
		{
			tactic = NavigationTactics.ReduceTravelDistance;
		}
		base.smi.GoTo(base.smi.sm.normal.moving);
		base.smi.sm.moveTarget.Set(target.gameObject, base.smi, false);
		this.tactic = tactic;
		this.target = target;
		this.targetOffsets = offsets;
		this.ClearReservedCell();
		this.AdvancePath(true);
		return this.IsMoving();
	}

	public void BeginTransition(NavGrid.Transition transition)
	{
		this.transitionDriver.EndTransition();
		base.smi.GoTo(base.smi.sm.normal.moving);
		this.transitionDriver.BeginTransition(this, transition, this.defaultSpeed);
	}

	private bool ValidatePath(ref PathFinder.Path path, out bool atNextNode)
	{
		atNextNode = false;
		bool flag = false;
		if (path.IsValid())
		{
			int num = Grid.PosToCell(this.target);
			flag = this.reservedCell != NavigationReservations.InvalidReservation && this.CanReach(this.reservedCell);
			flag &= Grid.IsCellOffsetOf(this.reservedCell, num, this.targetOffsets);
		}
		if (flag)
		{
			int num2 = Grid.PosToCell(this);
			flag = num2 == path.nodes[0].cell && this.CurrentNavType == path.nodes[0].navType;
			flag |= (atNextNode = num2 == path.nodes[1].cell && this.CurrentNavType == path.nodes[1].navType);
		}
		if (!flag)
		{
			return false;
		}
		PathFinderAbilities currentAbilities = this.GetCurrentAbilities();
		return PathFinder.ValidatePath(this.NavGrid, currentAbilities, ref path, this.flags);
	}

	private bool TryBuildPathFromCache(int cachedCell, int reservedCell, ref PathFinder.Path path)
	{
		bool flag;
		return this.PathGrid.BuildPath(cachedCell, reservedCell, this.CurrentNavType, ref path) && this.ValidatePath(ref path, out flag);
	}

	public void AdvancePath(bool trigger_advance = true)
	{
		this.cachedCell = Grid.PosToCell(this);
		if (this.target == null)
		{
			if (!this.Stop(false, true))
			{
				base.Trigger(-766531887, null);
			}
		}
		else if (this.cachedCell == this.reservedCell && this.CurrentNavType != NavType.Tube)
		{
			this.Stop(true, true);
		}
		else
		{
			bool flag;
			if (!this.ValidatePath(ref this.path, out flag))
			{
				int num = Grid.PosToCell(this.target);
				int cellPreferences = this.tactic.GetCellPreferences(num, this.targetOffsets, this);
				this.SetReservedCell(cellPreferences);
				if (this.reservedCell == NavigationReservations.InvalidReservation)
				{
					this.path.Clear();
				}
				else if (!this.TryBuildPathFromCache(this.cachedCell, this.reservedCell, ref this.path))
				{
					PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(this.cachedCell, this.CurrentNavType, this.flags);
					PathFinder.UpdatePath(this.NavGrid, this.GetCurrentAbilities(), potentialPath, PathFinderQueries.cellQuery.Reset(this.reservedCell), ref this.path);
					if (this.executePathProbeTaskAsync)
					{
						AsyncPathProber.Instance.ApplyNavigationFailedPenalty(this);
					}
				}
			}
			else if (flag)
			{
				this.path.nodes.RemoveAt(0);
			}
			if (this.path.IsValid())
			{
				this.BeginTransition(this.NavGrid.transitions[(int)this.path.nodes[1].transitionId]);
				this.distanceTravelledByNavType[this.CurrentNavType] = Mathf.Max(this.distanceTravelledByNavType[this.CurrentNavType] + 1, this.distanceTravelledByNavType[this.CurrentNavType]);
			}
			else if (this.path.HasArrived())
			{
				this.Stop(true, true);
			}
			else
			{
				this.ClearReservedCell();
				this.Stop(false, true);
			}
		}
		if (trigger_advance)
		{
			base.Trigger(1347184327, null);
		}
	}

	public NavGrid.Transition GetNextTransition()
	{
		return this.NavGrid.transitions[(int)this.path.nodes[1].transitionId];
	}

	public bool Stop(bool arrived_at_destination = false, bool play_idle = true)
	{
		this.target = null;
		this.targetOffsets = null;
		this.path.Clear();
		base.smi.sm.moveTarget.Set(null, base.smi);
		this.transitionDriver.EndTransition();
		if (play_idle)
		{
			HashedString idleAnim = this.NavGrid.GetIdleAnim(this.CurrentNavType);
			this.animController.Play(idleAnim, KAnim.PlayMode.Loop, 1f, 0f);
		}
		if (arrived_at_destination)
		{
			base.smi.GoTo(base.smi.sm.normal.arrived);
			return true;
		}
		if (base.smi.GetCurrentState() == base.smi.sm.normal.moving)
		{
			this.ClearReservedCell();
			base.smi.GoTo(base.smi.sm.normal.failed);
			return true;
		}
		return false;
	}

	private void SimEveryTick(float dt)
	{
		if (this.IsMoving())
		{
			this.transitionDriver.UpdateTransition(dt);
		}
	}

	public void UpdateProbe(bool forceUpdate = false)
	{
		if (forceUpdate || !this.executePathProbeTaskAsync)
		{
			if (this.reportOccupation)
			{
				ListPool<int, Navigator>.PooledList pooledList = ListPool<int, Navigator>.Allocate();
				PathProber.Run(this, pooledList);
				MinionGroupProber.Get().Occupy(pooledList);
				MinionGroupProber.Get().Vacate(this.occupiedCells);
				this.occupiedCells.Clear();
				this.occupiedCells.AddRange(pooledList);
				pooledList.Recycle();
				return;
			}
			PathProber.Run(this, null);
		}
	}

	public void DrawPath()
	{
		if (base.gameObject.activeInHierarchy && this.IsMoving())
		{
			NavPathDrawer.Instance.DrawPath(this.animController.GetPivotSymbolPosition(), this.path);
		}
	}

	public void Pause(string reason)
	{
		base.smi.sm.isPaused.Set(true, base.smi, false);
	}

	public void Unpause(string reason)
	{
		base.smi.sm.isPaused.Set(false, base.smi, false);
	}

	private void OnDefeated(object data)
	{
		this.ClearReservedCell();
		this.Stop(false, false);
	}

	private void ClearReservedCell()
	{
		if (this.reservedCell != NavigationReservations.InvalidReservation)
		{
			NavigationReservations.Instance.RemoveOccupancy(this.reservedCell);
			this.reservedCell = NavigationReservations.InvalidReservation;
		}
	}

	private void SetReservedCell(int cell)
	{
		this.ClearReservedCell();
		this.reservedCell = cell;
		NavigationReservations.Instance.AddOccupancy(cell);
	}

	public int GetReservedCell()
	{
		return this.reservedCell;
	}

	public int GetAnchorCell()
	{
		return this.AnchorCell;
	}

	public bool IsValidNavType(NavType nav_type)
	{
		return this.NavGrid.HasNavTypeData(nav_type);
	}

	public void SetCurrentNavType(NavType nav_type)
	{
		this.CurrentNavType = nav_type;
		this.AnchorCell = NavTypeHelper.GetAnchorCell(nav_type, Grid.PosToCell(this));
		NavGrid.NavTypeData navTypeData = this.NavGrid.GetNavTypeData(this.CurrentNavType);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		Vector2 one = Vector2.one;
		if (navTypeData.flipX)
		{
			one.x = -1f;
		}
		if (navTypeData.flipY)
		{
			one.y = -1f;
		}
		component.navMatrix = Matrix2x3.Translate(navTypeData.animControllerOffset * 200f) * Matrix2x3.Rotate(navTypeData.rotation) * Matrix2x3.Scale(one);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.gameObject.HasTag(GameTags.Dead))
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = ((NavPathDrawer.Instance.GetNavigator() != this) ? new KIconButtonMenu.ButtonInfo("action_navigable_regions", UI.USERMENUACTIONS.DRAWPATHS.NAME, new global::System.Action(this.OnDrawPaths), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP, true) : new KIconButtonMenu.ButtonInfo("action_navigable_regions", UI.USERMENUACTIONS.DRAWPATHS.NAME_OFF, new global::System.Action(this.OnDrawPaths), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP_OFF, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 0.1f);
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_follow_cam", UI.USERMENUACTIONS.FOLLOWCAM.NAME, new global::System.Action(this.OnFollowCam), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.FOLLOWCAM.TOOLTIP, true), 0.3f);
	}

	private void OnFollowCam()
	{
		if (CameraController.Instance.followTarget == base.transform)
		{
			CameraController.Instance.ClearFollowTarget();
			return;
		}
		CameraController.Instance.SetFollowTarget(base.transform);
	}

	private void OnDrawPaths()
	{
		if (NavPathDrawer.Instance.GetNavigator() != this)
		{
			NavPathDrawer.Instance.SetNavigator(this);
			return;
		}
		NavPathDrawer.Instance.ClearNavigator();
	}

	private void OnSelectObject(object data)
	{
		NavPathDrawer.Instance.ClearNavigator();
	}

	public void OnStore(object data)
	{
		if (data is Storage || (data != null && ((Boxed<bool>)data).value))
		{
			this.Stop(false, true);
		}
	}

	public PathFinderAbilities GetCurrentAbilities()
	{
		this.abilities.Refresh();
		return this.abilities;
	}

	public void SetAbilities(PathFinderAbilities abilities)
	{
		this.abilities = abilities;
	}

	public bool CanReach(IApproachable approachable)
	{
		return this.CanReach(approachable.GetCell(), approachable.GetOffsets());
	}

	public bool CanReach(int cell, CellOffset[] offsets)
	{
		foreach (CellOffset cellOffset in offsets)
		{
			int num = Grid.OffsetCell(cell, cellOffset);
			if (this.CanReach(num))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanReach(int cell)
	{
		return this.GetNavigationCost(cell) != -1;
	}

	public int GetNavigationCost(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			return this.PathGrid.GetCost(cell);
		}
		return -1;
	}

	public int GetNavigationCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		return this.PathGrid.GetCostIgnoreProberOffset(cell, offsets);
	}

	public int GetNavigationCost(int cell, CellOffset[] offsets)
	{
		int num = -1;
		int num2 = offsets.Length;
		for (int i = 0; i < num2; i++)
		{
			int num3 = Grid.OffsetCell(cell, offsets[i]);
			int navigationCost = this.GetNavigationCost(num3);
			if (navigationCost != -1 && (num == -1 || navigationCost < num))
			{
				num = navigationCost;
			}
		}
		return num;
	}

	public int GetNavigationCost(int cell, IReadOnlyList<CellOffset> offsets)
	{
		int num = -1;
		int count = offsets.Count;
		for (int i = 0; i < count; i++)
		{
			int num2 = Grid.OffsetCell(cell, offsets[i]);
			int navigationCost = this.GetNavigationCost(num2);
			if (navigationCost != -1 && (num == -1 || navigationCost < num))
			{
				num = navigationCost;
			}
		}
		return num;
	}

	public int GetNavigationCost(IApproachable approachable)
	{
		return this.GetNavigationCost(approachable.GetCell(), approachable.GetOffsets());
	}

	public void RunQuery(PathFinderQuery query)
	{
		int num = Grid.PosToCell(this);
		PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(num, this.CurrentNavType, this.flags);
		PathFinder.Run(this.NavGrid, this.GetCurrentAbilities(), potentialPath, query);
	}

	public void SetFlags(PathFinder.PotentialPath.Flags new_flags)
	{
		this.flags |= new_flags;
	}

	public void ClearFlags(PathFinder.PotentialPath.Flags new_flags)
	{
		this.flags &= ~new_flags;
	}

	[Conditional("ENABLE_DETAILED_NAVIGATOR_PROFILE_INFO")]
	public static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("ENABLE_DETAILED_NAVIGATOR_PROFILE_INFO")]
	public static void EndDetailedSample(string region_name)
	{
	}

	public bool DebugDrawPath;

	[MyCmpAdd]
	public Facing facing;

	public float defaultSpeed = 1f;

	public TransitionDriver transitionDriver;

	public string NavGridName;

	public bool updateProber;

	public int maxProbeRadiusX;

	public int maxProbeRadiusY;

	public PathFinder.PotentialPath.Flags flags;

	private LoggerFSS log;

	public Dictionary<NavType, int> distanceTravelledByNavType;

	public Grid.SceneLayer sceneLayer = Grid.SceneLayer.Move;

	private PathFinderAbilities abilities;

	[MyCmpReq]
	public KBatchedAnimController animController;

	[NonSerialized]
	public PathFinder.Path path;

	public NavType CurrentNavType;

	private int AnchorCell;

	private KPrefabID targetLocator;

	public int cachedCell;

	private int reservedCell = NavigationReservations.InvalidReservation;

	private NavTactic tactic;

	public bool reportOccupation;

	public List<int> occupiedCells = new List<int>();

	private Action<int, object> OnBuildingTileChangedAction;

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnDefeatedDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnDefeated(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnSelectObjectDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnSelectObject(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnStoreDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnStore(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnQueueDestroyDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnQueueDestroy();
	});

	public bool executePathProbeTaskAsync;

	public class ActiveTransition
	{
		public void Init(NavGrid.Transition transition, float default_speed)
		{
			this.x = transition.x;
			this.y = transition.y;
			this.isLooping = transition.isLooping;
			this.start = transition.start;
			this.end = transition.end;
			this.preAnim = transition.preAnim;
			this.anim = transition.anim;
			this.speed = default_speed;
			this.animSpeed = transition.animSpeed;
			this.navGridTransition = transition;
		}

		public void Copy(Navigator.ActiveTransition other)
		{
			this.x = other.x;
			this.y = other.y;
			this.isLooping = other.isLooping;
			this.start = other.start;
			this.end = other.end;
			this.preAnim = other.preAnim;
			this.anim = other.anim;
			this.speed = other.speed;
			this.animSpeed = other.animSpeed;
			this.navGridTransition = other.navGridTransition;
		}

		public int x;

		public int y;

		public bool isLooping;

		public NavType start;

		public NavType end;

		public HashedString preAnim;

		public HashedString anim;

		public float speed;

		public float animSpeed = 1f;

		public Func<bool> isCompleteCB;

		public NavGrid.Transition navGridTransition;
	}

	public class StatesInstance : GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.GameInstance
	{
		public StatesInstance(Navigator master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.normal.stopped;
			this.saveHistory = true;
			this.normal.ParamTransition<bool>(this.isPaused, this.paused, GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.IsTrue);
			this.normal.moving.Enter(delegate(Navigator.StatesInstance smi)
			{
				smi.BoxingTrigger<GameHashes>(1027377649, GameHashes.ObjectMovementWakeUp);
			}).Update("UpdateNavigator", delegate(Navigator.StatesInstance smi, float dt)
			{
				smi.master.SimEveryTick(dt);
			}, UpdateRate.SIM_EVERY_TICK, true).Exit(delegate(Navigator.StatesInstance smi)
			{
				smi.BoxingTrigger<GameHashes>(1027377649, GameHashes.ObjectMovementSleep);
			});
			this.normal.arrived.TriggerOnEnter(GameHashes.DestinationReached, null).GoTo(this.normal.stopped);
			this.normal.failed.TriggerOnEnter(GameHashes.NavigationFailed, null).GoTo(this.normal.stopped);
			this.normal.stopped.Enter(delegate(Navigator.StatesInstance smi)
			{
				smi.master.SubscribeUnstuckFunctions();
			}).DoNothing().Exit(delegate(Navigator.StatesInstance smi)
			{
				smi.master.UnsubscribeUnstuckFunctions();
			});
			this.paused.ParamTransition<bool>(this.isPaused, this.normal, GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.IsFalse);
		}

		public StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.TargetParameter moveTarget;

		public StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.BoolParameter isPaused = new StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.BoolParameter(false);

		public Navigator.States.NormalStates normal;

		public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State paused;

		public class NormalStates : GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State
		{
			public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State moving;

			public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State arrived;

			public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State failed;

			public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State stopped;
		}
	}

	public class Scanner<T> where T : KMonoBehaviour
	{
		public Scanner(int radius, ScenePartitionerLayer layer, Func<T, bool> filterFn)
		{
			this.radius = radius;
			this.layer = layer;
			this.filterFn = filterFn;
			this.offsets = Navigator.Scanner<T>.NO_OFFSETS;
			this.offsetsFn = null;
			this.early_out_threshold = null;
		}

		public void SetConstantOffsets(CellOffset[] offsets)
		{
			this.offsets = offsets;
		}

		public void SetDynamicOffsetsFn(Action<T, List<CellOffset>> offsetsFn)
		{
			this.offsetsFn = offsetsFn;
		}

		public void SetEarlyOutThreshold(int early_out_threshold)
		{
			this.early_out_threshold = new int?(early_out_threshold);
		}

		private int NavCostFromConstantOffsets(Navigator navigator, T destinationObject, CellOffset[] offsets)
		{
			return navigator.GetNavigationCost(Grid.PosToCell(destinationObject.gameObject), offsets);
		}

		private int NavCostFromDynamicOffsets(Navigator navigator, T destinationObject, Action<T, List<CellOffset>> offsetsFn)
		{
			ListPool<CellOffset, Navigator>.PooledList pooledList = ListPool<CellOffset, Navigator>.Allocate();
			offsetsFn(destinationObject, pooledList);
			int navigationCost = navigator.GetNavigationCost(Grid.PosToCell(destinationObject.gameObject), pooledList);
			pooledList.Recycle();
			return navigationCost;
		}

		public T Scan(Vector2I gridPos, Navigator navigator)
		{
			ListPool<ScenePartitionerEntry, Navigator>.PooledList pooledList = ListPool<ScenePartitionerEntry, Navigator>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(gridPos.x - this.radius, gridPos.y - this.radius, this.radius * 2, this.radius * 2, this.layer, pooledList);
			T t = default(T);
			int num = -1;
			if (this.early_out_threshold != null)
			{
				pooledList.Shuffle<ScenePartitionerEntry>();
				if (this.offsetsFn != null)
				{
					for (int i = 0; i < pooledList.Count; i++)
					{
						T t2 = pooledList[i].obj as T;
						if (this.filterFn(t2))
						{
							int num2 = this.NavCostFromDynamicOffsets(navigator, t2, this.offsetsFn);
							if (num2 != -1 && (t == null || num2 < num))
							{
								t = t2;
								num = num2;
								if (num2 <= this.early_out_threshold.Value)
								{
									break;
								}
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < pooledList.Count; j++)
					{
						T t3 = pooledList[j].obj as T;
						if (this.filterFn(t3))
						{
							int num3 = this.NavCostFromConstantOffsets(navigator, t3, this.offsets);
							if (num3 != -1 && (t == null || num3 < num))
							{
								t = t3;
								num = num3;
								if (num3 <= this.early_out_threshold.Value)
								{
									break;
								}
							}
						}
					}
				}
			}
			else if (this.offsetsFn != null)
			{
				for (int k = 0; k < pooledList.Count; k++)
				{
					T t4 = pooledList[k].obj as T;
					if (this.filterFn(t4))
					{
						int num4 = this.NavCostFromDynamicOffsets(navigator, t4, this.offsetsFn);
						if (num4 != -1 && (t == null || num4 < num))
						{
							t = t4;
							num = num4;
						}
					}
				}
			}
			else
			{
				for (int l = 0; l < pooledList.Count; l++)
				{
					T t5 = pooledList[l].obj as T;
					if (this.filterFn(t5))
					{
						int num5 = this.NavCostFromConstantOffsets(navigator, t5, this.offsets);
						if (num5 != -1 && (t == null || num5 < num))
						{
							t = t5;
							num = num5;
						}
					}
				}
			}
			pooledList.Recycle();
			return t;
		}

		private static readonly CellOffset[] NO_OFFSETS = new CellOffset[]
		{
			new CellOffset(0, 0)
		};

		private readonly int radius;

		private readonly ScenePartitionerLayer layer;

		private readonly Func<T, bool> filterFn;

		private CellOffset[] offsets;

		private Action<T, List<CellOffset>> offsetsFn;

		private int? early_out_threshold;
	}
}
