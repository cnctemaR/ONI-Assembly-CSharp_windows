using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CommandModule : StateMachineComponent<CommandModule.StatesInstance>, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.characterOverlay = base.gameObject.AddComponent<CharacterOverlay>();
		this.characterOverlay.Register();
		this.rocketStats = new RocketStats(this);
	}

	public void ReleaseAstronaut()
	{
		if (this.releasingAstronaut)
		{
			return;
		}
		this.releasingAstronaut = true;
		MinionStorage component = base.GetComponent<MinionStorage>();
		List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
		for (int i = storedMinionInfo.Count - 1; i >= 0; i--)
		{
			GameObject gameObject = component.DeserializeMinion(storedMinionInfo[i].id, Grid.CellToPos(Grid.PosToCell(base.smi.master.transform.GetPosition())));
			if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
			{
				gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
			}
		}
		this.releasingAstronaut = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.storage = base.GetComponent<Storage>();
		this.assignable = base.GetComponent<Assignable>();
		this.assignable.eligibleFilter = (MinionIdentity identity) => identity.GetComponent<MinionResume>().HasPerk(RoleManager.rolePerks.CanUseRockets);
		base.smi.StartSM();
		int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), 0, -1);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("CommandModule.gantryChanged", base.gameObject, num, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnGantryChanged));
		this.OnGantryChanged(null);
		RocketModule component = base.GetComponent<RocketModule>();
		this.reachable = (ConditionDestinationReachable)component.AddLaunchCondition(new ConditionDestinationReachable(this));
		this.hasAstronaut = (ConditionHasAstronaut)component.AddLaunchCondition(new ConditionHasAstronaut(this));
		this.hasSuit = (ConditionHasAtmoSuit)component.AddLaunchCondition(new ConditionHasAtmoSuit(this));
		this.cargoEmpty = (CargoBayIsEmpty)component.AddLaunchCondition(new CargoBayIsEmpty(this));
		this.flightPathIsClear = (ConditionFlightPathIsClear)component.AddFlightCondition(new ConditionFlightPathIsClear(base.gameObject, 1));
	}

	private void OnGantryChanged(object data)
	{
		if (base.gameObject != null)
		{
			KSelectable component = base.GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.HasGantry, false);
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.MissingGantry, false);
			if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.HasGantry, null);
			}
			else
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.MissingGantry, null);
			}
		}
	}

	private Chore CreateWorkChore()
	{
		ChoreType astronaut = Db.Get().ChoreTypes.Astronaut;
		KAnimFile anim = Assets.GetAnim("anim_hat_kanim");
		WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(astronaut, this, null, null, true, null, null, null, false, null, false, true, anim, false, true, false, PriorityScreen.PriorityClass.emergency, 0, false);
		workChore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanUseRockets);
		workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, this.assignable);
		return workChore;
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		this.partitionerEntry.Clear();
		this.ReleaseAstronaut();
		base.smi.StopSM("cleanup");
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}

	public Storage storage;

	public RocketStats rocketStats;

	private bool releasingAstronaut;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	public ConditionDestinationReachable reachable;

	public ConditionHasAstronaut hasAstronaut;

	public ConditionHasAtmoSuit hasSuit;

	public CargoBayIsEmpty cargoEmpty;

	public ConditionFlightPathIsClear flightPathIsClear;

	public Assignable assignable;

	private CharacterOverlay characterOverlay;

	private HandleVector<int>.Handle partitionerEntry;

	public class StatesInstance : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.GameInstance
	{
		public StatesInstance(CommandModule master)
			: base(master)
		{
		}

		public void SetSuspended(bool suspended)
		{
			Storage component = base.GetComponent<Storage>();
			if (component != null)
			{
				component.allowItemRemoval = !suspended;
			}
			ManualDeliveryKG component2 = base.GetComponent<ManualDeliveryKG>();
			if (component2 != null)
			{
				component2.Pause(suspended, "Rocket is suspended");
			}
		}
	}

	public class States : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded.idle;
			this.grounded.DefaultState(this.grounded.idle).TagTransition(GameTags.RocketNotOnGround, this.spaceborne, false);
			this.grounded.idle.PlayAnim("grounded", KAnim.PlayMode.Loop).GoTo(this.grounded.awaitingAstronaut);
			this.grounded.awaitingAstronaut.PlayAnim("grounded", KAnim.PlayMode.Loop).EnterTransition(this.grounded.hasAstronaut, (CommandModule.StatesInstance smi) => smi.GetComponent<MinionStorage>().GetStoredMinionInfo().Count > 0).ToggleChore((CommandModule.StatesInstance smi) => smi.master.CreateWorkChore(), this.grounded.hasAstronaut);
			this.grounded.hasAstronaut.PlayAnim("grounded", KAnim.PlayMode.Loop).EventHandler(GameHashes.AssigneeChanged, delegate(CommandModule.StatesInstance smi)
			{
				smi.master.ReleaseAstronaut();
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).EventTransition(GameHashes.AssigneeChanged, this.grounded.idle, null);
			this.spaceborne.DefaultState(this.spaceborne.launch);
			this.spaceborne.launch.Enter(delegate(CommandModule.StatesInstance smi)
			{
				smi.SetSuspended(true);
			}).GoTo(this.spaceborne.idle);
			this.spaceborne.idle.TagTransition(GameTags.RocketNotOnGround, this.spaceborne.land, true);
			this.spaceborne.land.Enter(delegate(CommandModule.StatesInstance smi)
			{
				smi.SetSuspended(false);
				smi.master.ReleaseAstronaut();
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).GoTo(this.grounded);
		}

		public CommandModule.States.GroundedStates grounded;

		public CommandModule.States.SpaceborneStates spaceborne;

		public class GroundedStates : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State
		{
			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State idle;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State awaitingAstronaut;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State hasAstronaut;
		}

		public class SpaceborneStates : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State
		{
			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State launch;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State idle;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State land;
		}
	}
}
