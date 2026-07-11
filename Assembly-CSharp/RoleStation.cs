using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RoleStation")]
public class RoleStation : Workable, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = true;
		this.UpdateStatusItemDelegate = new Action<object>(this.UpdateSkillPointAvailableStatusItem);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.RoleStations.Add(this);
		this.smi = new RoleStation.RoleStationSM.Instance(this);
		this.smi.StartSM();
		base.SetWorkTime(7.53f);
		this.resetProgressOnStop = true;
		this.subscriptions.Add(Game.Instance.Subscribe(-1523247426, this.UpdateStatusItemDelegate));
		this.subscriptions.Add(Game.Instance.Subscribe(1505456302, this.UpdateStatusItemDelegate));
		this.UpdateSkillPointAvailableStatusItem(null);
	}

	protected override void OnStopWork(Worker worker)
	{
		Telepad.StatesInstance statesInstance = this.GetSMI<Telepad.StatesInstance>();
		statesInstance.sm.idlePortal.Trigger(statesInstance);
	}

	private void UpdateSkillPointAvailableStatusItem(object data = null)
	{
		foreach (object obj in Components.MinionResumes)
		{
			MinionResume minionResume = (MinionResume)obj;
			if (!minionResume.HasTag(GameTags.Dead) && minionResume.TotalSkillPointsGained - minionResume.SkillsMastered > 0)
			{
				if (this.skillPointAvailableStatusItem == Guid.Empty)
				{
					this.skillPointAvailableStatusItem = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable, null);
				}
				return;
			}
		}
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable, false);
		this.skillPointAvailableStatusItem = Guid.Empty;
	}

	private Chore CreateWorkChore()
	{
		return new WorkChore<RoleStation>(Db.Get().ChoreTypes.LearnSkill, this, null, true, null, null, null, false, null, false, true, Assets.GetAnim("anim_hat_kanim"), false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
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

	private Action<object> UpdateStatusItemDelegate;

	private List<int> subscriptions = new List<int>();

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
