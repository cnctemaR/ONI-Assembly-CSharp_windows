using System;
using STRINGS;
using UnityEngine;

public class Checkpoint : StateMachineComponent<Checkpoint.SMInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-801688580, new Action<object>(this.OnLogicValueChanged));
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		base.smi.StartSM();
		if (Checkpoint.infoStatusItem_Logic == null)
		{
			Checkpoint.infoStatusItem_Logic = new StatusItem("CheckpointLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			Checkpoint.infoStatusItem_Logic.resolveStringCallback = new Func<string, object, string>(Checkpoint.ResolveInfoStatusItem_Logic);
		}
		if (Checkpoint.infoStatusItem_Wire == null)
		{
			Checkpoint.infoStatusItem_Wire = new StatusItem("CheckpointDisconnected", BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_DISCONNECTED, BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_DISCONNECTED, "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.Logic, 30718);
		}
		this.Refresh();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.ClearReactable();
	}

	private LogicCircuitNetwork GetNetwork()
	{
		LogicPorts component = base.GetComponent<LogicPorts>();
		int portCell = component.GetPortCell(Checkpoint.PORT_ID);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		return logicCircuitManager.GetNetworkForCell(portCell);
	}

	private static string ResolveInfoStatusItem_Logic(string format_str, object data)
	{
		Checkpoint checkpoint = (Checkpoint)data;
		string text;
		if (!checkpoint.hasLogicWire)
		{
			text = BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_DISCONNECTED;
		}
		else
		{
			text = ((!checkpoint.RedLight) ? BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_OPEN : BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_CLOSED);
		}
		return text;
	}

	private void CreateNewReactable()
	{
		if (this.reactable == null)
		{
			this.reactable = new Checkpoint.CheckpointReactable(this);
		}
	}

	private void OrphanReactable()
	{
		this.reactable = null;
	}

	private void ClearReactable()
	{
		if (this.reactable != null)
		{
			this.reactable.Cleanup();
			this.reactable = null;
		}
	}

	public bool RedLight
	{
		get
		{
			return this.redLight;
		}
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == Checkpoint.PORT_ID)
		{
			this.hasInputHigh = logicValueChanged.newValue > 0;
			this.hasLogicWire = this.GetNetwork() != null;
			this.Refresh();
		}
	}

	private void OnOperationalChanged(object data)
	{
		this.Refresh();
	}

	private void Refresh()
	{
		this.redLight = this.hasLogicWire && !this.hasInputHigh && this.operational.IsOperational;
		this.operational.SetActive(this.redLight, false);
		base.smi.sm.redLight.Set(this.redLight, base.smi);
		this.selectable.ToggleStatusItem(Checkpoint.infoStatusItem_Logic, this.operational.IsOperational && this.hasLogicWire, this);
		this.selectable.ToggleStatusItem(Checkpoint.infoStatusItem_Wire, !this.hasLogicWire, null);
		if (this.redLight)
		{
			this.CreateNewReactable();
		}
		else
		{
			this.ClearReactable();
		}
	}

	[MyCmpReq]
	public Operational operational;

	[MyCmpReq]
	private KSelectable selectable;

	private static StatusItem infoStatusItem_Logic;

	private static StatusItem infoStatusItem_Wire;

	private Checkpoint.CheckpointReactable reactable;

	public static readonly HashedString PORT_ID = "Checkpoint";

	private bool hasLogicWire;

	private bool hasInputHigh;

	private bool redLight;

	private class CheckpointReactable : Reactable
	{
		public CheckpointReactable(Checkpoint checkpoint)
			: base(checkpoint.gameObject, Db.Get().ChoreTypes.Checkpoint, 1, 1, false)
		{
			this.checkpoint = checkpoint;
			this.rotated = this.gameObject.GetComponent<Rotatable>().IsRotated;
			this.preventChoreInterruption = false;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			bool flag;
			if (this.reactor != null)
			{
				flag = false;
			}
			else if (this.checkpoint == null)
			{
				base.Cleanup();
				flag = false;
			}
			else if (!this.checkpoint.RedLight)
			{
				flag = false;
			}
			else if (this.rotated)
			{
				flag = transition.x < 0;
			}
			else
			{
				flag = transition.x > 0;
			}
			return flag;
		}

		protected override void InternalBegin()
		{
			this.reactor_navigator = this.reactor.GetComponent<Navigator>();
			KBatchedAnimController component = this.reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"), 1f);
			component.Play("idle_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
			this.checkpoint.OrphanReactable();
			this.checkpoint.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			if (this.checkpoint == null || !this.checkpoint.RedLight || this.reactor_navigator == null)
			{
				base.Cleanup();
			}
			else
			{
				this.reactor_navigator.AdvancePath(false);
				if (!this.reactor_navigator.path.IsValid())
				{
					base.Cleanup();
				}
				else
				{
					NavGrid.Transition nextTransition = this.reactor_navigator.GetNextTransition();
					if (!((!this.rotated) ? (nextTransition.x > 0) : (nextTransition.x < 0)))
					{
						base.Cleanup();
					}
				}
			}
		}

		protected override void InternalEnd()
		{
			this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"));
		}

		protected override void InternalCleanup()
		{
		}

		private Checkpoint checkpoint;

		private Navigator reactor_navigator;

		private bool rotated = false;
	}

	public class SMInstance : GameStateMachine<Checkpoint.States, Checkpoint.SMInstance, Checkpoint, object>.GameInstance
	{
		public SMInstance(Checkpoint master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Checkpoint.States, Checkpoint.SMInstance, Checkpoint>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.go;
			this.stop.ParamTransition<bool>(this.redLight, this.go, (Checkpoint.SMInstance smi, bool redLight) => !redLight).PlayAnim("red_light");
			this.go.ParamTransition<bool>(this.redLight, this.stop, (Checkpoint.SMInstance smi, bool redLight) => redLight).PlayAnim("green_light");
		}

		public StateMachine<Checkpoint.States, Checkpoint.SMInstance, Checkpoint, object>.BoolParameter redLight;

		public GameStateMachine<Checkpoint.States, Checkpoint.SMInstance, Checkpoint, object>.State stop;

		public GameStateMachine<Checkpoint.States, Checkpoint.SMInstance, Checkpoint, object>.State go;
	}
}
