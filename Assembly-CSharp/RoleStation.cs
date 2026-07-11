using System;
using System.Collections;
using System.Collections.Generic;

public class RoleStation : Workable, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.RoleStations.Add(this);
		this.smi = new RoleStation.RoleStationSM.Instance(this);
		this.smi.StartSM();
		base.SetWorkTime(7.53f);
		this.resetProgressOnStop = true;
		this.subscriptions.Add(base.Subscribe<RoleStation>(-1523247426, RoleStation.OnUpdateDelegate));
		this.subscriptions.Add(base.Subscribe<RoleStation>(1505456302, RoleStation.OnUpdateDelegate));
		this.UpdateSkillPointAvailableStatusItem(null);
	}

	protected override void OnStopWork(Worker worker)
	{
		Telepad.StatesInstance statesInstance = this.GetSMI<Telepad.StatesInstance>();
		statesInstance.sm.idlePortal.Trigger(statesInstance);
	}

	private void UpdateSkillPointAvailableStatusItem(object data = null)
	{
		IEnumerator enumerator = Components.MinionResumes.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				MinionResume minionResume = (MinionResume)obj;
				if (minionResume.TotalSkillPointsGained - minionResume.SkillsMastered > 0)
				{
					if (this.skillPointAvailableStatusItem == Guid.Empty)
					{
						this.skillPointAvailableStatusItem = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable, null);
					}
					return;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable, false);
		this.skillPointAvailableStatusItem = Guid.Empty;
	}

	private Chore CreateWorkChore()
	{
		ChoreType learnSkill = Db.Get().ChoreTypes.LearnSkill;
		KAnimFile anim = Assets.GetAnim("anim_hat_kanim");
		return new WorkChore<RoleStation>(learnSkill, this, null, true, null, null, null, false, null, false, true, anim, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		worker.GetComponent<MinionResume>().SkillLearned();
	}

	private void OnSelectRolesClick()
	{
		DetailsScreen.Instance.Show(false);
		ManagementMenu.Instance.ToggleSkills();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (int num in this.subscriptions)
		{
			Game.Instance.Unsubscribe(num);
		}
		Components.RoleStations.Remove(this);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return new List<Descriptor>();
	}

	private Chore chore;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Operational operational;

	private RoleStation.RoleStationSM.Instance smi;

	private Guid skillPointAvailableStatusItem;

	private List<int> subscriptions = new List<int>();

	private static readonly EventSystem.IntraObjectHandler<RoleStation> OnUpdateDelegate = new EventSystem.IntraObjectHandler<RoleStation>(delegate(RoleStation component, object data)
	{
		component.UpdateSkillPointAvailableStatusItem(data);
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
