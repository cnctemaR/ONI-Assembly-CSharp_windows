using System;

public class DecorPlantMonitor : GameStateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.wildPlanted;
		this.wildPlanted.EventTransition(GameHashes.ReceptacleMonitorChange, this.domestic, new StateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.Transition.ConditionCallback(DecorPlantMonitor.IsDomestic));
		this.domestic.EventTransition(GameHashes.ReceptacleMonitorChange, this.wildPlanted, GameStateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.Not(new StateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.Transition.ConditionCallback(DecorPlantMonitor.IsDomestic))).DefaultState(this.domestic.wilted);
		this.domestic.wilted.EventTransition(GameHashes.WiltRecover, this.domestic.healthy, GameStateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.Not(new StateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.Transition.ConditionCallback(DecorPlantMonitor.IsWilted))).Enter(new StateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.State.Callback(DecorPlantMonitor.TriggerRoomRefresh));
		this.domestic.healthy.EventTransition(GameHashes.Wilt, this.domestic.wilted, new StateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.Transition.ConditionCallback(DecorPlantMonitor.IsWilted)).ToggleTag(GameTags.Decoration).Enter(new StateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.State.Callback(DecorPlantMonitor.TriggerRoomRefresh));
	}

	public static bool IsDomestic(DecorPlantMonitor.Instance smi)
	{
		return smi.receptacleMonitor != null && smi.receptacleMonitor.ReceptacleObject != null;
	}

	public static bool IsWilted(DecorPlantMonitor.Instance smi)
	{
		return smi.IsWilted;
	}

	public static void TriggerRoomRefresh(DecorPlantMonitor.Instance smi)
	{
		int num = Grid.PosToCell(smi);
		Game.Instance.roomProber.TriggerBuildingChangedEvent(num, smi.gameObject);
	}

	public GameStateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.State wildPlanted;

	public DecorPlantMonitor.DomesticStates domestic;

	public class Def : StateMachine.BaseDef
	{
	}

	public class DomesticStates : GameStateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.State
	{
		public GameStateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.State healthy;

		public GameStateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.State wilted;
	}

	public new class Instance : GameStateMachine<DecorPlantMonitor, DecorPlantMonitor.Instance, IStateMachineTarget, DecorPlantMonitor.Def>.GameInstance
	{
		public bool IsWilted
		{
			get
			{
				return this.wiltCondition.IsWilting();
			}
		}

		public ReceptacleMonitor.StatesInstance receptacleMonitor
		{
			get
			{
				if (this._receptacleMonitor == null)
				{
					this._receptacleMonitor = base.gameObject.GetSMI<ReceptacleMonitor.StatesInstance>();
				}
				return this._receptacleMonitor;
			}
		}

		public Instance(IStateMachineTarget master, DecorPlantMonitor.Def def)
			: base(master, def)
		{
			this.wiltCondition = base.GetComponent<WiltCondition>();
		}

		private WiltCondition wiltCondition;

		private ReceptacleMonitor.StatesInstance _receptacleMonitor;
	}
}
