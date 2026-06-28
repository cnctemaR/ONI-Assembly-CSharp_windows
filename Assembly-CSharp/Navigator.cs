using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class Navigator : StateMachineComponent<Navigator.StatesInstance>
{
	public KMonoBehaviour target { get; set; }

	public CellOffset[] targetOffsets { get; private set; }

	public NavGrid NavGrid { get; private set; }

	protected override void OnPrefabInit()
	{
		this.transitionDriver = new TransitionDriver(this);
		this.targetLocator = new GameObject("TargetLocator").AddComponent<KPrefabID>();
		this.targetLocator.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.Misc).transform;
		this.targetLocator.PrefabTag = new Tag("TargetLocator");
		this.log = new LoggerFS("Navigator");
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.NavGrid = Pathfinding.Instance.GetNavGrid(this.NavGridName);
		base.GetComponent<PathProber>().SetValidNavTypes(this.NavGrid.ValidNavTypes, this.maxProbingRadius);
		this.Subscribe(1623392196, new Action<object>(this.OnDefeated));
		this.Subscribe(-1506500077, new Action<object>(this.OnDefeated));
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.Subscribe(-1503271301, new Action<object>(this.OnSelectObject));
		this.maxUnderwaterTravelCost = Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(this);
		if (this.updateProber)
		{
			PathProberScheduler.Instance.Add(this);
		}
	}

	public bool IsMoving()
	{
		return base.smi.IsInsideState(base.smi.sm.moving);
	}

	public bool GoTo(int cell, CellOffset[] offsets = null)
	{
		if (offsets == null)
		{
			offsets = Grid.DefaultOffset;
		}
		this.targetLocator.transform.position = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
		return this.GoTo(this.targetLocator, offsets, NavigationTactics.ReduceTravelDistance);
	}

	public bool GoTo(int cell, CellOffset[] offsets, NavTactic tactic)
	{
		if (offsets == null)
		{
			offsets = Grid.DefaultOffset;
		}
		this.targetLocator.transform.position = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
		return this.GoTo(this.targetLocator, offsets, tactic);
	}

	public void UpdateTarget(int cell)
	{
		this.targetLocator.transform.position = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
	}

	public bool GoTo(KMonoBehaviour target, CellOffset[] offsets, NavTactic tactic)
	{
		if (tactic == null)
		{
			tactic = NavigationTactics.ReduceTravelDistance;
		}
		base.smi.GoTo(base.smi.sm.moving);
		base.smi.sm.moveTarget.Set(target.gameObject, base.smi);
		this.tactic = tactic;
		this.target = target;
		this.targetOffsets = offsets;
		this.ClearReservedCell();
		this.AdvancePath();
		return this.IsMoving();
	}

	public void BeginTransition(NavGrid.Transition transition)
	{
		this.transitionDriver.EndTransition();
		base.smi.GoTo(base.smi.sm.moving);
		Navigator.ActiveTransition activeTransition = new Navigator.ActiveTransition(transition, this.defaultSpeed);
		this.Trigger(-897197316, activeTransition);
		this.transitionDriver.BeginTransition(this, activeTransition);
	}

	public void AdvancePath()
	{
		int num = Grid.PosToCell(this);
		if (this.target == null)
		{
			this.Trigger(-766531887, null);
			this.Stop(false);
		}
		else if (num == this.reservedCell)
		{
			this.Stop(true);
		}
		else
		{
			bool flag = false;
			int num2 = Grid.PosToCell(this.target);
			if (this.reservedCell == NavigationReservations.InvalidReservation || !this.CanReach(this.reservedCell))
			{
				this.ClearReservedCell();
				flag = true;
			}
			else if (Grid.IsCellOffsetOf(this.reservedCell, num2, this.targetOffsets))
			{
				PathFinder.UpdatePath(this.NavGrid, this.GetCurrentAbilities(), num, this.CurrentNavType, PathFinderQueries.cellOffsetQuery.Reset(new int[] { this.reservedCell }), ref this.path);
				if (this.path.IsValid())
				{
					flag = false;
					this.SetReservedCell(this.reservedCell);
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.ClearReservedCell();
				int[] cellPreferences = this.tactic.GetCellPreferences(num2, this.targetOffsets, this);
				for (int i = 0; i < cellPreferences.Length; i++)
				{
					if (this.CanReach(cellPreferences[i]))
					{
						this.SetReservedCell(cellPreferences[i]);
						break;
					}
				}
				if (this.reservedCell == NavigationReservations.InvalidReservation)
				{
					this.path.cost = 0;
					this.Stop(false);
				}
				else
				{
					this.checkCellArray[0] = this.reservedCell;
					PathFinder.UpdatePath(this.NavGrid, this.GetCurrentAbilities(), num, this.CurrentNavType, PathFinderQueries.cellOffsetQuery.Reset(this.checkCellArray), ref this.path);
				}
			}
			if (this.path.IsValid())
			{
				this.BeginTransition(this.NavGrid.transitions[this.path.nodes[1].transitionId]);
			}
			else if (this.path.HasArrived())
			{
				this.Stop(true);
			}
			else
			{
				this.ClearReservedCell();
				this.Stop(false);
			}
		}
	}

	public void Stop(bool arrived_at_destination = false)
	{
		this.target = null;
		this.targetOffsets = null;
		this.path.Clear();
		base.smi.sm.moveTarget.Set(null, base.smi);
		this.transitionDriver.EndTransition();
		if (arrived_at_destination)
		{
			base.smi.GoTo(base.smi.sm.arrived);
		}
		else if (base.smi.GetCurrentState() == base.smi.sm.moving)
		{
			this.ClearReservedCell();
			base.smi.GoTo(base.smi.sm.failed);
		}
	}

	private bool AllTargetOffsetsReserved(int targetCell, CellOffset[] targetOffsets)
	{
		for (int i = 0; i < targetOffsets.Length; i++)
		{
			int num = Grid.OffsetCell(targetCell, targetOffsets[i]);
			if (num != this.reservedCell)
			{
				if (!NavigationReservations.Instance.isReserved(num))
				{
					return false;
				}
			}
		}
		return true;
	}

	private void FixedUpdate()
	{
		if (this.IsMoving())
		{
			this.transitionDriver.UpdateTransition(Time.fixedDeltaTime);
		}
	}

	public void UpdateProbe()
	{
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		this.PathProber.UpdateProbe(this.NavGrid, num, this.CurrentNavType, this.GetCurrentAbilities(), true);
	}

	private void LateUpdate()
	{
		if (this.IsMoving() && this.selectable.IsSelected)
		{
			NavPathDrawer.Instance.DrawPath(base.GetComponent<KAnimControllerBase>().GetPivotSymbolPosition(), this.path);
		}
		if (this.DebugDrawPath || this.NavGrid.DebugViewAllPaths)
		{
			NavGrid.DebugDrawPath(this.path);
		}
	}

	private void OnDefeated(object data)
	{
		this.ClearReservedCell();
		this.Stop(false);
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

	public void SetAbilityFlag(PathFinderFlags flag)
	{
		this.abilities.flags = this.abilities.flags | flag;
	}

	public void ClearAbilityFlag(PathFinderFlags flag)
	{
		this.abilities.flags = this.abilities.flags & ~flag;
	}

	public int GetAnchorCell()
	{
		return this.AnchorCell;
	}

	public void SetCurrentNavType(NavType nav_type)
	{
		this.CurrentNavType = nav_type;
		this.AnchorCell = NavTypeHelper.GetAnchorCell(nav_type, Grid.PosToCell(this));
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.GetComponent<Health>().IsDead())
		{
			return;
		}
		string text;
		if (NavPathDrawer.Instance.GetNavigator() != this)
		{
			UserMenu userMenu = this.userMenu;
			text = UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_navigable_regions", UI.USERMENUACTIONS.DRAWPATHS.NAME, new global::System.Action(this.OnDrawPaths), global::Action.NumActions, null, null, null, text, true), 0.1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			text = UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP_OFF;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_navigable_regions", UI.USERMENUACTIONS.DRAWPATHS.NAME_OFF, new global::System.Action(this.OnDrawPaths), global::Action.NumActions, null, null, null, text, true), 0.1f);
		}
		UserMenu userMenu3 = this.userMenu;
		text = UI.USERMENUACTIONS.FOLLOWCAM.TOOLTIP;
		userMenu3.AddButton(new KIconButtonMenu.ButtonInfo("action_follow_cam", UI.USERMENUACTIONS.FOLLOWCAM.NAME, new global::System.Action(this.OnFollowCam), global::Action.NumActions, null, null, null, text, true), 0.3f);
	}

	private void OnFollowCam()
	{
		if (CameraController.Instance.followTarget == this.transform)
		{
			CameraController.Instance.ClearFollowTarget();
		}
		else
		{
			CameraController.Instance.SetFollowTarget(this.transform);
		}
	}

	private void OnDrawPaths()
	{
		if (NavPathDrawer.Instance.GetNavigator() != this)
		{
			NavPathDrawer.Instance.SetNavigator(this);
		}
		else
		{
			NavPathDrawer.Instance.ClearNavigator();
		}
	}

	private void OnSelectObject(object data)
	{
		NavPathDrawer.Instance.ClearNavigator();
	}

	public PathFinderAbilities GetCurrentAbilities()
	{
		PathFinderAbilities pathFinderAbilities = this.abilities;
		if (this.maxUnderwaterTravelCost != null)
		{
			pathFinderAbilities.maxUnderwaterCost = (int)this.maxUnderwaterTravelCost.GetTotalValue();
		}
		else
		{
			pathFinderAbilities.maxUnderwaterCost = int.MaxValue;
		}
		int num = Grid.PosToCell(this);
		if (Grid.SuitRequired[num])
		{
			pathFinderAbilities.flags |= PathFinderFlags.SuitRequired;
		}
		if (PathFinder.IsSubmerged(num))
		{
			pathFinderAbilities.maxUnderwaterCost = int.MaxValue;
		}
		return pathFinderAbilities;
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
		return this.GetNavigationCost(cell) != PathProber.InvalidCost;
	}

	public int GetNavigationCost(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			return this.PathProber.GetCost(cell);
		}
		return PathProber.InvalidCost;
	}

	public int GetNavigationCost(int cell, CellOffset[] offsets)
	{
		int num = PathProber.InvalidCost;
		foreach (CellOffset cellOffset in offsets)
		{
			int num2 = Grid.OffsetCell(cell, cellOffset);
			int navigationCost = this.GetNavigationCost(num2);
			if (num == PathProber.InvalidCost)
			{
				num = navigationCost;
			}
			else if (navigationCost != PathProber.InvalidCost && navigationCost < num)
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
		PathFinder.Run(this.NavGrid, this.GetCurrentAbilities(), num, this.CurrentNavType, query);
	}

	public void AddMask(NavMask mask)
	{
		this.abilities.AddMask(mask);
	}

	public void RemoveMask(NavMask mask)
	{
		this.abilities.RemoveMask(mask);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.updateProber)
		{
			PathProberScheduler.Instance.Remove(this);
		}
	}

	public bool DebugDrawPath;

	[MyCmpAdd]
	public PathProber PathProber;

	[MyCmpAdd]
	private UserMenu userMenu;

	[MyCmpAdd]
	private Facing facing;

	public float defaultSpeed = 1f;

	public TransitionDriver transitionDriver;

	public string NavGridName;

	public bool updateProber;

	public int maxProbingRadius;

	private AttributeInstance maxUnderwaterTravelCost;

	private LoggerFS log;

	private PathFinderAbilities abilities = default(PathFinderAbilities);

	[MyCmpReq]
	private KSelectable selectable;

	[NonSerialized]
	public PathFinder.Path path;

	public NavType CurrentNavType;

	private int AnchorCell;

	private KPrefabID targetLocator;

	private int reservedCell = NavigationReservations.InvalidReservation;

	private NavTactic tactic;

	private int[] checkCellArray = new int[1];

	public class ActiveTransition
	{
		public ActiveTransition(NavGrid.Transition transition, float default_speed)
		{
			this.x = transition.x;
			this.y = transition.y;
			this.isLooping = transition.isLooping;
			this.start = transition.start;
			this.end = transition.end;
			this.preAnim = transition.preAnim;
			this.anim = transition.anim;
			this.speed = default_speed;
		}

		public int x;

		public int y;

		public bool isLooping;

		public NavType start;

		public NavType end;

		public string preAnim;

		public string anim;

		public float speed;

		public float animSpeed = 1f;

		public Func<bool> isCompleteCB;
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
			default_state = this.stopped;
			this.saveHistory = true;
			this.moving.Enter(delegate(Navigator.StatesInstance smi)
			{
				smi.Trigger(1027377649, GameHashes.ObjectMovementWakeUp);
			}).ToggleSchedulePeriodic("Log travel time", 1f, delegate(Navigator.StatesInstance smi)
			{
				if (smi.GetComponent<MinionIdentity>() != null)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.TravelTime, 1f, null);
				}
			}).Enter(delegate(Navigator.StatesInstance smi)
			{
				smi.Trigger(1027377649, GameHashes.ObjectMovementSleep);
			});
			this.arrived.TriggerOnEnter(GameHashes.DestinationReached, null).GoTo(this.stopped);
			this.failed.TriggerOnEnter(GameHashes.NavigationFailed, null).GoTo(this.stopped);
			this.stopped.DoNothing();
		}

		public StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.TargetParameter moveTarget;

		public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State moving;

		public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State arrived;

		public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State failed;

		public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State stopped;
	}
}
