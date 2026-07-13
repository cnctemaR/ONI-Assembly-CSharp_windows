using System;

public class SimpleDoorController : GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inactive;
		this.inactive.TagTransition(GameTags.RocketOnGround, this.active, false);
		this.active.DefaultState(this.active.closed).TagTransition(GameTags.RocketOnGround, this.inactive, true).Enter(delegate(SimpleDoorController.StatesInstance smi)
		{
			smi.Register();
		})
			.Exit(delegate(SimpleDoorController.StatesInstance smi)
			{
				smi.Unregister();
			});
		this.active.closed.PlayAnim((SimpleDoorController.StatesInstance smi) => smi.GetDefaultAnim(), KAnim.PlayMode.Loop).ParamTransition<int>(this.numOpens, this.active.opening, (SimpleDoorController.StatesInstance smi, int p) => p > 0);
		this.active.opening.PlayAnim("enter_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.active.open);
		this.active.open.PlayAnim("enter_loop", KAnim.PlayMode.Loop).ParamTransition<int>(this.numOpens, this.active.closedelay, (SimpleDoorController.StatesInstance smi, int p) => p == 0);
		this.active.closedelay.ParamTransition<int>(this.numOpens, this.active.open, (SimpleDoorController.StatesInstance smi, int p) => p > 0).ScheduleGoTo(0.5f, this.active.closing);
		this.active.closing.PlayAnim("enter_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.active.closed);
	}

	public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State inactive;

	public SimpleDoorController.ActiveStates active;

	public StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.IntParameter numOpens;

	public class Def : StateMachine.BaseDef
	{
	}

	public class ActiveStates : GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State
	{
		public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State closed;

		public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State opening;

		public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State open;

		public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State closedelay;

		public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State closing;
	}

	public class StatesInstance : GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.GameInstance, INavDoor
	{
		public StatesInstance(IStateMachineTarget master, SimpleDoorController.Def def)
			: base(master, def)
		{
		}

		public string GetDefaultAnim()
		{
			KBatchedAnimController component = base.master.GetComponent<KBatchedAnimController>();
			if (component != null && !component.initialAnim.IsNullOrWhiteSpace())
			{
				return component.initialAnim;
			}
			return "idle_loop";
		}

		public void Register()
		{
			int num = Grid.PosToCell(base.gameObject.transform.GetPosition());
			Grid.HasDoor[num] = true;
		}

		public void Unregister()
		{
			int num = Grid.PosToCell(base.gameObject.transform.GetPosition());
			Grid.HasDoor[num] = false;
		}

		public bool isSpawned
		{
			get
			{
				return base.master.gameObject.GetComponent<KMonoBehaviour>().isSpawned;
			}
		}

		public void Close()
		{
			base.sm.numOpens.Delta(-1, base.smi);
		}

		public bool IsOpen()
		{
			return base.IsInsideState(base.sm.active.open) || base.IsInsideState(base.sm.active.closedelay);
		}

		public void Open()
		{
			base.sm.numOpens.Delta(1, base.smi);
		}
	}
}
