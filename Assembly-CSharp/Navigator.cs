using System;
using System.IO;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class Navigator : StateMachineComponent<Navigator.StatesInstance>, ISaveLoadableDetails, ISim4000ms
{
	public KMonoBehaviour target { get; set; }

	public CellOffset[] targetOffsets { get; private set; }

	public NavGrid NavGrid { get; private set; }

	public void Serialize(BinaryWriter writer)
	{
		byte b = (byte)this.CurrentNavType;
		writer.Write(b);
	}

	public void Deserialize(IReader reader)
	{
		byte b = reader.ReadByte();
		this.CurrentNavType = (NavType)b;
	}

	protected override void OnPrefabInit()
	{
		this.transitionDriver = new TransitionDriver(this);
		this.targetLocator = new GameObject("TargetLocator").AddComponent<KPrefabID>();
		this.targetLocator.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.Misc).transform;
		this.targetLocator.PrefabTag = new Tag("TargetLocator");
		this.log = new LoggerFS("Navigator");
		this.simRenderLoadBalance = true;
		this.autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.NavGrid = Pathfinding.Instance.GetNavGrid(this.NavGridName);
		base.GetComponent<PathProber>().SetValidNavTypes(this.NavGrid.ValidNavTypes, this.maxProbingRadius);
		base.Subscribe(1623392196, new Action<object>(this.OnDefeated));
		base.Subscribe(-1506500077, new Action<object>(this.OnDefeated));
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(-1503271301, new Action<object>(this.OnSelectObject));
		this.maxUnderwaterTravelCost = Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(this);
		if (this.updateProber)
		{
			SimAndRenderScheduler.instance.Add(this, false);
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
		this.targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
		return this.GoTo(this.targetLocator, offsets, NavigationTactics.ReduceTravelDistance);
	}

	public bool GoTo(int cell, CellOffset[] offsets, NavTactic tactic)
	{
		if (offsets == null)
		{
			offsets = Grid.DefaultOffset;
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
		base.smi.GoTo(base.smi.sm.moving);
		base.smi.sm.moveTarget.Set(target.gameObject, base.smi);
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
		base.smi.GoTo(base.smi.sm.moving);
		Navigator.ActiveTransition activeTransition = new Navigator.ActiveTransition(transition, this.defaultSpeed);
		base.Trigger(-897197316, activeTransition);
		this.transitionDriver.BeginTransition(this, activeTransition);
	}

	private bool ValidatePath(ref PathFinder.Path path)
	{
		PathFinderAbilities currentAbilities = this.GetCurrentAbilities();
		return PathFinder.ValidatePath(this.NavGrid, currentAbilities, ref path);
	}

	public void AdvancePath(bool trigger_advance = true)
	{
		int num = Grid.PosToCell(this);
		if (this.target == null)
		{
			base.Trigger(-766531887, null);
			this.Stop(false);
		}
		else if (num == this.reservedCell && this.CurrentNavType != NavType.Tube)
		{
			this.Stop(true);
		}
		else
		{
			int num2 = Grid.PosToCell(this.target);
			bool flag;
			if (this.reservedCell == NavigationReservations.InvalidReservation)
			{
				flag = true;
			}
			else if (!this.CanReach(this.reservedCell))
			{
				flag = true;
			}
			else if (!Grid.IsCellOffsetOf(this.reservedCell, num2, this.targetOffsets))
			{
				flag = true;
			}
			else if (this.path.IsValid())
			{
				if (num == this.path.nodes[0].cell && this.CurrentNavType == this.path.nodes[0].navType)
				{
					flag = !this.ValidatePath(ref this.path);
				}
				else if (num == this.path.nodes[1].cell && this.CurrentNavType == this.path.nodes[1].navType)
				{
					this.path.nodes.RemoveAt(0);
					flag = !this.ValidatePath(ref this.path);
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				int cellPreferences = this.tactic.GetCellPreferences(num2, this.targetOffsets, this);
				this.SetReservedCell(cellPreferences);
				if (this.reservedCell == NavigationReservations.InvalidReservation)
				{
					this.Stop(false);
				}
				else
				{
					PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(num, this.CurrentNavType, this.flags);
					PathFinder.UpdatePath(this.NavGrid, this.GetCurrentAbilities(), potentialPath, PathFinderQueries.cellQuery.Reset(this.reservedCell), ref this.path);
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
		if (trigger_advance)
		{
			base.Trigger(1347184327, null);
		}
	}

	public NavGrid.Transition GetNextTransition()
	{
		return this.NavGrid.transitions[this.path.nodes[1].transitionId];
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

	private void Sim33ms(float dt)
	{
		if (this.IsMoving())
		{
			this.transitionDriver.UpdateTransition(dt);
		}
	}

	public void Sim4000ms(float dt)
	{
		this.UpdateProbe();
	}

	public void UpdateProbe()
	{
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		this.PathProber.UpdateProbe(this.NavGrid, num, this.CurrentNavType, this.GetCurrentAbilities(), this.flags, true);
	}

	public void DrawPath()
	{
		if (base.gameObject.activeInHierarchy && this.IsMoving())
		{
			NavPathDrawer.Instance.DrawPath(base.GetComponent<KAnimControllerBase>().GetPivotSymbolPosition(), this.path);
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
		if (base.gameObject.HasTag(GameTags.Dead))
		{
			return;
		}
		string text;
		string text2;
		global::System.Action action;
		string text3;
		if (NavPathDrawer.Instance.GetNavigator() != this)
		{
			UserMenu userMenu = this.userMenu;
			text = "action_navigable_regions";
			text2 = UI.USERMENUACTIONS.DRAWPATHS.NAME;
			action = new global::System.Action(this.OnDrawPaths);
			text3 = UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 0.1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			text3 = "action_navigable_regions";
			text2 = UI.USERMENUACTIONS.DRAWPATHS.NAME_OFF;
			action = new global::System.Action(this.OnDrawPaths);
			text = UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP_OFF;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true), 0.1f);
		}
		UserMenu userMenu3 = this.userMenu;
		text = "action_follow_cam";
		text2 = UI.USERMENUACTIONS.FOLLOWCAM.NAME;
		action = new global::System.Action(this.OnFollowCam);
		text3 = UI.USERMENUACTIONS.FOLLOWCAM.TOOLTIP;
		userMenu3.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 0.3f);
	}

	private void OnFollowCam()
	{
		if (CameraController.Instance.followTarget == base.transform)
		{
			CameraController.Instance.ClearFollowTarget();
		}
		else
		{
			CameraController.Instance.SetFollowTarget(base.transform);
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
		PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(num, this.CurrentNavType, this.flags);
		PathFinder.Run(this.NavGrid, this.GetCurrentAbilities(), potentialPath, query);
	}

	public void AddMask(NavMask mask)
	{
		this.abilities.AddMask(mask);
	}

	public void RemoveMask(NavMask mask)
	{
		this.abilities.RemoveMask(mask);
	}

	public void SetFlags(PathFinder.PotentialPath.Flags new_flags)
	{
		this.flags |= new_flags;
	}

	public void ClearFlags(PathFinder.PotentialPath.Flags new_flags)
	{
		this.flags &= ~new_flags;
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

	public PathFinder.PotentialPath.Flags flags;

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
			this.navGridTransition = transition;
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
			default_state = this.stopped;
			this.saveHistory = true;
			this.moving.Enter(delegate(Navigator.StatesInstance smi)
			{
				smi.Trigger(1027377649, GameHashes.ObjectMovementWakeUp);
			}).Update("Log travel time", delegate(Navigator.StatesInstance smi, float dt)
			{
				if (smi.GetComponent<MinionIdentity>() != null)
				{
					Chore currentChore = smi.GetComponent<ChoreDriver>().GetCurrentChore();
					if (currentChore != null)
					{
						ReportManager.Instance.ReportValue(ReportManager.ReportType.TravelTime, dt, currentChore.choreType.Name, currentChore.driver.GetProperName());
						if (currentChore is FetchAreaChore)
						{
							MinionResume component = smi.GetComponent<MinionResume>();
							if (component != null)
							{
								component.AddExperienceIfRole("Hauler", dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
								component.AddExperienceIfRole(MaterialsManager.ID, dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
								component.AddExperienceIfRole(Handyman.ID, dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
							}
						}
					}
				}
			}, UpdateRate.SIM_200ms, false).Update("UpdateNavigator", delegate(Navigator.StatesInstance smi, float dt)
			{
				smi.master.Sim33ms(dt);
			}, UpdateRate.SIM_33ms, true)
				.Exit(delegate(Navigator.StatesInstance smi)
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
