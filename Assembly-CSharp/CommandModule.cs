using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CommandModule : StateMachineComponent<CommandModule.StatesInstance>, IEffectDescriptor
{
	public bool IsSuspended
	{
		get
		{
			return this.isSuspended;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.characterOverlay = base.gameObject.AddComponent<CharacterOverlay>();
		this.characterOverlay.Register();
	}

	public void SetSuspended(bool state)
	{
		this.isSuspended = state;
		base.smi.GetComponent<KSelectable>().IsSelectable = !this.isSuspended;
	}

	public void ReleaseAstronaut(object data)
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
			GameObject gameObject = component.DeserializeMinion(storedMinionInfo[i].id, Grid.CellToPos(Grid.PosToCell(base.smi.master.transform.position)));
			if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
			{
				gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
			}
		}
		this.releasingAstronaut = false;
	}

	public void OnSuspend(object data)
	{
		Storage component = base.GetComponent<Storage>();
		if (component != null)
		{
			component.capacityKg = component.MassStored();
			component.allowItemRemoval = false;
		}
		if (base.GetComponent<ManualDeliveryKG>() != null)
		{
			global::UnityEngine.Object.Destroy(base.GetComponent<ManualDeliveryKG>());
		}
		this.SetSuspended(true);
		this.ClearFloor(null);
	}

	public float GetRocketMaxDistance()
	{
		float totalMass = this.GetTotalMass();
		float totalThrust = this.GetTotalThrust();
		return Mathf.Max(0f, totalThrust - ROCKETRY.CalculateMassWithPenalty(totalMass));
	}

	public float GetTotalThrust()
	{
		float num = 0f;
		foreach (Tuple<RocketModule, float> tuple in this.GetModuleThrustContributions(base.GetComponent<LaunchableRocket>()))
		{
			num += tuple.second;
		}
		return num;
	}

	public float GetTotalMass()
	{
		float num = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = gameObject.GetComponent<RocketModule>();
			if (component != null)
			{
				num += component.GetComponent<PrimaryElement>().Mass;
			}
		}
		return num;
	}

	public List<Tuple<RocketModule, float>> GetModuleThrustContributions(LaunchableRocket rocket)
	{
		this.moduleThrustContribution.Clear();
		RocketEngine rocketEngine = null;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
		{
			rocketEngine = gameObject.GetComponent<RocketEngine>();
			if (rocketEngine != null)
			{
				break;
			}
		}
		if (rocketEngine != null)
		{
			foreach (GameObject gameObject2 in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
			{
				if (gameObject2.GetComponent<FuelTank>())
				{
					this.moduleThrustContribution.Add(new Tuple<RocketModule, float>(gameObject2.GetComponent<RocketModule>(), rocketEngine.thrustAmount));
				}
			}
		}
		return this.moduleThrustContribution;
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
		if (this.IsSuspended)
		{
			base.GetComponent<KSelectable>().IsSelectable = false;
		}
		else
		{
			base.GetComponent<KSelectable>().IsSelectable = true;
		}
		if (this.reachable == null)
		{
			this.reachable = new ConditionDestinationReachable(this);
			base.GetComponent<RocketModule>().AddCondition(this.reachable);
		}
		if (this.hasAstronaut == null)
		{
			this.hasAstronaut = new ConditionHasAstronaut(this);
			base.GetComponent<RocketModule>().AddCondition(this.hasAstronaut);
		}
		base.Subscribe(-1056989049, new Action<object>(this.OnSuspend));
		base.Subscribe(684616645, new Action<object>(this.OnAssigneeChanged));
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

	private void OnAssigneeChanged(object data)
	{
		if (base.GetComponent<MinionStorage>().GetStoredMinionInfo().Count > 0)
		{
			this.ReleaseAstronaut(null);
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
	}

	private void ClearFloor(object data)
	{
		BuildingDef def = base.GetComponent<BuildingComplete>().Def;
		int num = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int num2 = i - (def.WidthInCells - 1) / 2;
			int num3 = Grid.OffsetCell(num, new CellOffset(num2, 0));
			if (num3 == Grid.PosToCell(base.gameObject))
			{
				num3 = Grid.OffsetCell(num3, 0, -1);
			}
			SimMessages.ClearCellProperties(num3, 23);
			Grid.Foundation[num3] = false;
			Grid.PreviousSolid[num3] = Grid.Solid[num3];
			Grid.SetSolid(num3, false, CellEventLogger.Instance.SimCellOccupierForceSolid);
			World.Instance.OnSolidChanged(num3);
			GameScenePartitioner.Instance.TriggerEvent(num3, GameScenePartitioner.Instance.solidChangedLayer, null);
			Grid.RenderedByWorld[num3] = true;
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		this.partitionerEntry.Clear();
		this.ReleaseAstronaut(null);
		base.smi.StopSM("cleanup");
		this.ClearFloor(null);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}

	public Storage storage;

	[Serialize]
	private bool isSuspended;

	private bool releasingAstronaut;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)23;

	public ConditionDestinationReachable reachable;

	public ConditionHasAstronaut hasAstronaut;

	public Assignable assignable;

	private CharacterOverlay characterOverlay;

	private HandleVector<int>.Handle partitionerEntry;

	private List<Tuple<RocketModule, float>> moduleThrustContribution = new List<Tuple<RocketModule, float>>();

	public class StatesInstance : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.GameInstance
	{
		public StatesInstance(CommandModule smi)
			: base(smi)
		{
			smi.gameObject.Subscribe(238242047, delegate(object data)
			{
				smi.SetSuspended(false);
				smi.ReleaseAstronaut(null);
			});
		}
	}

	public class States : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.PlayAnim("grounded", KAnim.PlayMode.Loop).GoTo(this.awaitingAstronaut);
			this.awaitingAstronaut.PlayAnim("grounded", KAnim.PlayMode.Loop).ToggleChore((CommandModule.StatesInstance smi) => smi.master.CreateWorkChore(), this.hasAstronaut);
			this.hasAstronaut.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.LandRocket, this.idle, null).EventTransition(GameHashes.AssigneeChanged, this.idle, null);
		}

		public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State idle;

		public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State awaitingAstronaut;

		public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State hasAstronaut;
	}
}
