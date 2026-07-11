using System;
using STRINGS;
using UnityEngine;

public class Checkpoint : StateMachineComponent<Checkpoint.SMInstance>
{
	private bool RedLightDesiredState
	{
		get
		{
			return this.hasLogicWire && !this.hasInputHigh && this.operational.IsOperational;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<Checkpoint>(-801688580, Checkpoint.OnLogicValueChangedDelegate);
		base.Subscribe<Checkpoint>(-592767678, Checkpoint.OnOperationalChangedDelegate);
		base.smi.StartSM();
		if (Checkpoint.infoStatusItem_Logic == null)
		{
			Checkpoint.infoStatusItem_Logic = new StatusItem("CheckpointLogic", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			Checkpoint.infoStatusItem_Logic.resolveStringCallback = new Func<string, object, string>(Checkpoint.ResolveInfoStatusItem_Logic);
		}
		this.Refresh(this.redLight);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.ClearReactable();
	}

	public void RefreshLight()
	{
		if (this.redLight != this.RedLightDesiredState)
		{
			this.Refresh(this.RedLightDesiredState);
			this.statusDirty = true;
		}
		if (this.statusDirty)
		{
			this.RefreshStatusItem();
		}
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
		return (!checkpoint.RedLight) ? BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_OPEN : BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_CLOSED;
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
			this.statusDirty = true;
		}
	}

	private void OnOperationalChanged(object data)
	{
		this.statusDirty = true;
	}

	private void RefreshStatusItem()
	{
		bool flag = this.operational.IsOperational && this.hasLogicWire;
		this.selectable.ToggleStatusItem(Checkpoint.infoStatusItem_Logic, flag, this);
		this.statusDirty = false;
	}

	private void Refresh(bool redLightState)
	{
		this.redLight = redLightState;
		this.operational.SetActive(this.operational.IsOperational && this.redLight, false);
		base.smi.sm.redLight.Set(this.redLight, base.smi);
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

	private Checkpoint.CheckpointReactable reactable;

	public static readonly HashedString PORT_ID = "Checkpoint";

	private bool hasLogicWire;

	private bool hasInputHigh;

	private bool redLight;

	private bool statusDirty = true;

	private static readonly EventSystem.IntraObjectHandler<Checkpoint> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<Checkpoint>(delegate(Checkpoint component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Checkpoint> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Checkpoint>(delegate(Checkpoint component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private class CheckpointReactable : Reactable
	{
		public CheckpointReactable(Checkpoint checkpoint)
			: base(checkpoint.gameObject, "CheckpointReactable", Db.Get().ChoreTypes.Checkpoint, 1, 1, false, 0f, 0f, float.PositiveInfinity)
		{
			this.checkpoint = checkpoint;
			this.rotated = this.gameObject.GetComponent<Rotatable>().IsRotated;
			this.preventChoreInterruption = false;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (this.reactor != null)
			{
				return false;
			}
			if (this.checkpoint == null)
			{
				base.Cleanup();
				return false;
			}
			if (!this.checkpoint.RedLight)
			{
				return false;
			}
			if (this.rotated)
			{
				return transition.x < 0;
			}
			return transition.x > 0;
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
					if (!((!this.rotated) ? ((int)nextTransition.x > 0) : ((int)nextTransition.x < 0)))
					{
						base.Cleanup();
					}
				}
			}
		}

		protected override void InternalEnd()
		{
			if (this.reactor != null)
			{
				this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"));
			}
		}

		protected override void InternalCleanup()
		{
		}

		private Checkpoint checkpoint;

		private Navigator reactor_navigator;

		private bool rotated;
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
			this.root.Update("RefreshLight", delegate(Checkpoint.SMInstance smi, float dt)
			{
				smi.master.RefreshLight();
			}, UpdateRate.SIM_200ms, false);
			this.stop.ParamTransition<bool>(this.redLight, this.go, (Checkpoint.SMInstance smi, bool redLight) => !redLight).PlayAnim("red_light");
			this.go.ParamTransition<bool>(this.redLight, this.stop, (Checkpoint.SMInstance smi, bool redLight) => redLight).PlayAnim("green_light");
		}

		public StateMachine<Checkpoint.States, Checkpoint.SMInstance, Checkpoint, object>.BoolParameter redLight;

		public GameStateMachine<Checkpoint.States, Checkpoint.SMInstance, Checkpoint, object>.State stop;

		public GameStateMachine<Checkpoint.States, Checkpoint.SMInstance, Checkpoint, object>.State go;
	}
}
