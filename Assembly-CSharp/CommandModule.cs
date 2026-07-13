using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CommandModule : StateMachineComponent<CommandModule.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.rocketStats = new RocketStats(this);
		this.conditions = base.GetComponent<CommandConditions>();
	}

	public void ReleaseAstronaut(bool fill_bladder)
	{
		if (this.releasingAstronaut || this.robotPilotControlled)
		{
			return;
		}
		this.releasingAstronaut = true;
		MinionStorage component = base.GetComponent<MinionStorage>();
		List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
		for (int i = storedMinionInfo.Count - 1; i >= 0; i--)
		{
			MinionStorage.Info info = storedMinionInfo[i];
			GameObject gameObject = component.DeserializeMinion(info.id, Grid.CellToPos(Grid.PosToCell(base.smi.master.transform.GetPosition())));
			if (!(gameObject == null))
			{
				if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
				{
					gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
				}
				if (fill_bladder)
				{
					AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(gameObject);
					if (amountInstance != null)
					{
						amountInstance.value = amountInstance.GetMax();
					}
				}
			}
		}
		this.releasingAstronaut = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.storage = base.GetComponent<Storage>();
		if (!this.robotPilotControlled)
		{
			this.assignable = base.GetComponent<Assignable>();
			this.assignable.AddAssignPrecondition(new Func<MinionAssignablesProxy, bool>(this.CanAssignTo));
			int num = Grid.PosToCell(base.gameObject);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("CommandModule.gantryChanged", base.gameObject, num, GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnGantryChanged));
			this.OnGantryChanged(null);
		}
		base.smi.StartSM();
	}

	private bool CanAssignTo(MinionAssignablesProxy worker)
	{
		MinionIdentity minionIdentity = worker.target as MinionIdentity;
		if (minionIdentity != null)
		{
			return minionIdentity.GetComponent<MinionResume>().HasPerk(Db.Get().SkillPerks.CanUseRockets);
		}
		StoredMinionIdentity storedMinionIdentity = worker.target as StoredMinionIdentity;
		if (storedMinionIdentity != null)
		{
			if (storedMinionIdentity.model == BionicMinionConfig.MODEL)
			{
				MinionStorageDataHolder component = storedMinionIdentity.GetComponent<MinionStorageDataHolder>();
				if (component != null)
				{
					MinionStorageDataHolder.DataPack dataPack = component.GetDataPack<BionicUpgradesMonitor.Instance>();
					if (dataPack != null)
					{
						MinionStorageDataHolder.DataPackData dataPackData = dataPack.PeekData();
						if (dataPackData != null && dataPackData.Tags != null)
						{
							Tag[] tags = dataPackData.Tags;
							for (int i = 0; i < tags.Length; i++)
							{
								if (tags[i] == "Booster_PilotVanilla1")
								{
									return true;
								}
							}
						}
					}
				}
			}
			return storedMinionIdentity.HasPerk(Db.Get().SkillPerks.CanUseRockets);
		}
		return false;
	}

	private static bool HasValidGantry(GameObject go)
	{
		int num = Grid.OffsetCell(Grid.PosToCell(go), 0, -1);
		return Grid.IsValidCell(num) && Grid.FakeFloor[num];
	}

	private void OnGantryChanged(object data)
	{
		if (base.gameObject != null)
		{
			KSelectable component = base.GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.HasGantry, false);
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.MissingGantry, false);
			if (CommandModule.HasValidGantry(base.smi.master.gameObject))
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.HasGantry, null);
			}
			else
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.MissingGantry, null);
			}
			base.smi.sm.gantryChanged.Trigger(base.smi);
		}
	}

	private Chore CreateWorkChore()
	{
		WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(Db.Get().ChoreTypes.Astronaut, this, null, true, null, null, null, false, null, false, true, Assets.GetAnim("anim_hat_kanim"), false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, true);
		workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRockets);
		workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, this.assignable);
		return workChore;
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		this.partitionerEntry.Clear();
		this.ReleaseAstronaut(false);
		base.smi.StopSM("cleanup");
	}

	public Storage storage;

	public RocketStats rocketStats;

	public CommandConditions conditions;

	private bool releasingAstronaut;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	public Assignable assignable;

	public bool robotPilotControlled;

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

		public bool CheckStoredMinionIsAssignee()
		{
			if (base.smi.master.robotPilotControlled)
			{
				return true;
			}
			foreach (MinionStorage.Info info in base.GetComponent<MinionStorage>().GetStoredMinionInfo())
			{
				if (info.serializedMinion != null)
				{
					KPrefabID kprefabID = info.serializedMinion.Get();
					if (!(kprefabID == null))
					{
						StoredMinionIdentity component = kprefabID.GetComponent<StoredMinionIdentity>();
						if (base.GetComponent<Assignable>().assignee == component.assignableProxy.Get())
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}

	public class States : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded;
			this.grounded.PlayAnim("grounded", KAnim.PlayMode.Loop).DefaultState(this.grounded.awaitingAstronaut).TagTransition(GameTags.RocketNotOnGround, this.spaceborne, false);
			this.grounded.refreshChore.GoTo(this.grounded.awaitingAstronaut);
			this.grounded.awaitingAstronaut.Enter(delegate(CommandModule.StatesInstance smi)
			{
				if (smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(this.grounded.hasAstronaut);
				}
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).EventHandler(GameHashes.AssigneeChanged, delegate(CommandModule.StatesInstance smi)
			{
				if (smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(this.grounded.hasAstronaut);
				}
				else
				{
					smi.GoTo(this.grounded.refreshChore);
				}
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).ToggleChore((CommandModule.StatesInstance smi) => smi.master.CreateWorkChore(), this.grounded.hasAstronaut);
			this.grounded.hasAstronaut.EventHandler(GameHashes.AssigneeChanged, delegate(CommandModule.StatesInstance smi)
			{
				if (!smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(this.grounded.waitingToRelease);
				}
			});
			this.grounded.waitingToRelease.ToggleStatusItem(Db.Get().BuildingStatusItems.DisembarkingDuplicant, null).OnSignal(this.gantryChanged, this.grounded.awaitingAstronaut, delegate(CommandModule.StatesInstance smi)
			{
				if (CommandModule.HasValidGantry(smi.gameObject))
				{
					smi.master.ReleaseAstronaut(this.accumulatedPee.Get(smi));
					this.accumulatedPee.Set(false, smi, false);
					Game.Instance.userMenu.Refresh(smi.gameObject);
					return true;
				}
				return false;
			});
			this.spaceborne.DefaultState(this.spaceborne.launch);
			this.spaceborne.launch.Enter(delegate(CommandModule.StatesInstance smi)
			{
				smi.SetSuspended(true);
			}).GoTo(this.spaceborne.idle);
			this.spaceborne.idle.TagTransition(GameTags.RocketNotOnGround, this.spaceborne.land, true);
			this.spaceborne.land.Enter(delegate(CommandModule.StatesInstance smi)
			{
				smi.SetSuspended(false);
				Game.Instance.userMenu.Refresh(smi.gameObject);
				this.accumulatedPee.Set(true, smi, false);
			}).GoTo(this.grounded.waitingToRelease);
		}

		public StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.Signal gantryChanged;

		public StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.BoolParameter accumulatedPee;

		public CommandModule.States.GroundedStates grounded;

		public CommandModule.States.SpaceborneStates spaceborne;

		public class GroundedStates : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State
		{
			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State refreshChore;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State awaitingAstronaut;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State hasAstronaut;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State waitingToRelease;
		}

		public class SpaceborneStates : GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State
		{
			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State launch;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State idle;

			public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State land;
		}
	}
}
