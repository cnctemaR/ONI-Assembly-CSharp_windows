using System;
using System.Collections.Generic;

public class MissionControl : GameStateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.Inoperational;
		this.Inoperational.EventTransition(GameHashes.OperationalChanged, this.Operational, new StateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.Transition.ConditionCallback(this.ValidateOperationalTransition)).EventTransition(GameHashes.UpdateRoom, this.Operational, new StateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.Transition.ConditionCallback(this.ValidateOperationalTransition));
		this.Operational.EventTransition(GameHashes.OperationalChanged, this.Inoperational, new StateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.Transition.ConditionCallback(this.ValidateOperationalTransition)).EventTransition(GameHashes.UpdateRoom, this.Operational.WrongRoom, GameStateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.Not(new StateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.Transition.ConditionCallback(this.IsInLabRoom))).Enter(new StateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.State.Callback(this.OnEnterOperational))
			.DefaultState(this.Operational.NoRockets)
			.Update(delegate(MissionControl.Instance smi, float dt)
			{
				smi.UpdateWorkableRockets(null);
			}, UpdateRate.SIM_1000ms, false);
		this.Operational.WrongRoom.EventTransition(GameHashes.UpdateRoom, this.Operational.NoRockets, new StateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.Transition.ConditionCallback(this.IsInLabRoom));
		this.Operational.NoRockets.ToggleStatusItem(Db.Get().BuildingStatusItems.NoRocketsToMissionControlBoost, null).ParamTransition<bool>(this.WorkableRocketsAreInRange, this.Operational.HasRockets, (MissionControl.Instance smi, bool inRange) => this.WorkableRocketsAreInRange.Get(smi));
		this.Operational.HasRockets.ParamTransition<bool>(this.WorkableRocketsAreInRange, this.Operational.NoRockets, (MissionControl.Instance smi, bool inRange) => !this.WorkableRocketsAreInRange.Get(smi)).ToggleChore(new Func<MissionControl.Instance, Chore>(this.CreateChore), this.Operational);
	}

	private Chore CreateChore(MissionControl.Instance smi)
	{
		MissionControlWorkable component = smi.master.gameObject.GetComponent<MissionControlWorkable>();
		Chore chore = new WorkChore<MissionControlWorkable>(Db.Get().ChoreTypes.Research, component, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		Spacecraft randomBoostableSpacecraft = smi.GetRandomBoostableSpacecraft();
		component.TargetSpacecraft = randomBoostableSpacecraft;
		return chore;
	}

	private void OnEnterOperational(MissionControl.Instance smi)
	{
		smi.UpdateWorkableRockets(null);
		if (this.WorkableRocketsAreInRange.Get(smi))
		{
			smi.GoTo(this.Operational.HasRockets);
			return;
		}
		smi.GoTo(this.Operational.NoRockets);
	}

	private bool ValidateOperationalTransition(MissionControl.Instance smi)
	{
		Operational component = smi.GetComponent<Operational>();
		bool flag = smi.IsInsideState(smi.sm.Operational);
		return component != null && flag != component.IsOperational;
	}

	private bool IsInLabRoom(MissionControl.Instance smi)
	{
		return smi.roomTracker.IsInCorrectRoom();
	}

	public GameStateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.State Inoperational;

	public MissionControl.OperationalState Operational;

	public StateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.BoolParameter WorkableRocketsAreInRange;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, MissionControl.Def def)
			: base(master, def)
		{
		}

		public void UpdateWorkableRockets(object data)
		{
			this.boostableSpacecraft.Clear();
			for (int i = 0; i < SpacecraftManager.instance.GetSpacecraft().Count; i++)
			{
				if (this.CanBeBoosted(SpacecraftManager.instance.GetSpacecraft()[i]))
				{
					bool flag = false;
					foreach (object obj in Components.MissionControlWorkables)
					{
						MissionControlWorkable missionControlWorkable = (MissionControlWorkable)obj;
						if (!(missionControlWorkable.gameObject == base.gameObject) && missionControlWorkable.TargetSpacecraft == SpacecraftManager.instance.GetSpacecraft()[i])
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.boostableSpacecraft.Add(SpacecraftManager.instance.GetSpacecraft()[i]);
					}
				}
			}
			base.sm.WorkableRocketsAreInRange.Set(this.boostableSpacecraft.Count > 0, base.smi, false);
		}

		public Spacecraft GetRandomBoostableSpacecraft()
		{
			return this.boostableSpacecraft.GetRandom<Spacecraft>();
		}

		private bool CanBeBoosted(Spacecraft spacecraft)
		{
			return spacecraft.controlStationBuffTimeRemaining == 0f && spacecraft.state == Spacecraft.MissionState.Underway;
		}

		public void ApplyEffect(Spacecraft spacecraft)
		{
			spacecraft.controlStationBuffTimeRemaining = 600f;
		}

		private List<Spacecraft> boostableSpacecraft = new List<Spacecraft>();

		[MyCmpReq]
		public RoomTracker roomTracker;
	}

	public class OperationalState : GameStateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.State
	{
		public GameStateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.State WrongRoom;

		public GameStateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.State NoRockets;

		public GameStateMachine<MissionControl, MissionControl.Instance, IStateMachineTarget, MissionControl.Def>.State HasRockets;
	}
}
