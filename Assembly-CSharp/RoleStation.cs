using System;
using System.Collections.Generic;

public class RoleStation : Workable, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<RoleStation>(-1503271301, RoleStation.OnSelectObjectDelegate);
		Components.RoleStations.Add(this);
		this.smi = new RoleStation.RoleStationSM.Instance(this);
		this.smi.StartSM();
		base.SetWorkTime(2f);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	private Chore CreateWorkChore()
	{
		ChoreType switchRole = Db.Get().ChoreTypes.SwitchRole;
		KAnimFile anim = Assets.GetAnim("anim_hat_kanim");
		return new WorkChore<RoleStation>(switchRole, this, null, null, true, null, null, null, false, null, false, true, anim, false, true, false, PriorityScreen.PriorityClass.emergency, 0, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		new PutOnHatChore(worker, Db.Get().ChoreTypes.SwitchHat);
	}

	private void ClearRolesScreen()
	{
		if (this.rolesScreen != null)
		{
			this.rolesScreen.Deactivate();
			this.rolesScreen = null;
		}
	}

	private void OnSelectRolesClick()
	{
		DetailsScreen.Instance.Show(false);
		if (this.rolesScreen == null)
		{
			ManagementMenu.Instance.ToggleRoles();
		}
		else
		{
			this.ClearRolesScreen();
		}
	}

	private void OnSelectObject(object data)
	{
		this.ClearRolesScreen();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.RoleStations.Remove(this);
		this.ClearRolesScreen();
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return new List<Descriptor>();
	}

	private Chore chore;

	private RolesScreen rolesScreen;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Operational operational;

	private RoleStation.RoleStationSM.Instance smi;

	private static readonly EventSystem.IntraObjectHandler<RoleStation> OnSelectObjectDelegate = new EventSystem.IntraObjectHandler<RoleStation>(delegate(RoleStation component, object data)
	{
		component.OnSelectObject(data);
	});

	public class RoleStationSM : GameStateMachine<RoleStation.RoleStationSM, RoleStation.RoleStationSM.Instance, RoleStation>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (RoleStation.RoleStationSM.Instance smi) => smi.GetComponent<Operational>().IsOperational);
			this.operational.ToggleChore((RoleStation.RoleStationSM.Instance smi) => smi.master.CreateWorkChore(), this.unoperational);
		}

		public GameStateMachine<RoleStation.RoleStationSM, RoleStation.RoleStationSM.Instance, RoleStation, object>.State unoperational;

		public GameStateMachine<RoleStation.RoleStationSM, RoleStation.RoleStationSM.Instance, RoleStation, object>.State operational;

		public new class Instance : GameStateMachine<RoleStation.RoleStationSM, RoleStation.RoleStationSM.Instance, RoleStation, object>.GameInstance
		{
			public Instance(RoleStation master)
				: base(master)
			{
			}
		}
	}
}
