using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class HighEnergyParticleRedirector : StateMachineComponent<HighEnergyParticleRedirector.StatesInstance>, IHighEnergyParticleDirection
{
	public EightDirection Direction
	{
		get
		{
			return this._direction;
		}
		set
		{
			this._direction = value;
			if (this.directionController != null)
			{
				this.directionController.SetRotation((float)(45 * EightDirectionUtil.GetDirectionIndex(this._direction)));
				this.directionController.controller.enabled = false;
				this.directionController.controller.enabled = true;
			}
		}
	}

	private void OnCopySettings(object data)
	{
		HighEnergyParticleRedirector component = ((GameObject)data).GetComponent<HighEnergyParticleRedirector>();
		if (component != null)
		{
			this.Direction = component.Direction;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<HighEnergyParticleRedirector>(-905833192, HighEnergyParticleRedirector.OnCopySettingsDelegate);
		base.Subscribe<HighEnergyParticleRedirector>(-801688580, HighEnergyParticleRedirector.OnLogicValueChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		HighEnergyParticlePort component = base.GetComponent<HighEnergyParticlePort>();
		if (component)
		{
			HighEnergyParticlePort highEnergyParticlePort = component;
			highEnergyParticlePort.onParticleCaptureAllowed = (HighEnergyParticlePort.OnParticleCaptureAllowed)Delegate.Combine(highEnergyParticlePort.onParticleCaptureAllowed, new HighEnergyParticlePort.OnParticleCaptureAllowed(this.OnParticleCaptureAllowed));
		}
		if (HighEnergyParticleRedirector.infoStatusItem_Logic == null)
		{
			HighEnergyParticleRedirector.infoStatusItem_Logic = new StatusItem("HEPRedirectorLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			HighEnergyParticleRedirector.infoStatusItem_Logic.resolveStringCallback = new Func<string, object, string>(HighEnergyParticleRedirector.ResolveInfoStatusItem);
			HighEnergyParticleRedirector.infoStatusItem_Logic.resolveTooltipCallback = new Func<string, object, string>(HighEnergyParticleRedirector.ResolveInfoStatusItemTooltip);
		}
		this.selectable.AddStatusItem(HighEnergyParticleRedirector.infoStatusItem_Logic, this);
		this.directionController = new EightDirectionController(base.GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", EightDirectionController.Offset.Infront);
		this.Direction = this.Direction;
		base.smi.StartSM();
	}

	private bool OnParticleCaptureAllowed(HighEnergyParticle particle)
	{
		return this.AllowIncomingParticles;
	}

	private void LaunchParticle()
	{
		if (base.smi.master.storage.Particles < 0.1f)
		{
			base.smi.master.storage.ConsumeAll();
			return;
		}
		int highEnergyParticleOutputCell = base.GetComponent<Building>().GetHighEnergyParticleOutputCell();
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2, null, 0);
		gameObject.SetActive(true);
		if (gameObject != null)
		{
			HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
			component.payload = base.smi.master.storage.ConsumeAll();
			component.payload -= 0.1f;
			component.capturedBy = this.port;
			component.SetDirection(this.Direction);
			this.directionController.PlayAnim("redirector_send", KAnim.PlayMode.Once);
			this.directionController.controller.Queue("redirector", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		HighEnergyParticlePort component = base.GetComponent<HighEnergyParticlePort>();
		if (component != null)
		{
			HighEnergyParticlePort highEnergyParticlePort = component;
			highEnergyParticlePort.onParticleCaptureAllowed = (HighEnergyParticlePort.OnParticleCaptureAllowed)Delegate.Remove(highEnergyParticlePort.onParticleCaptureAllowed, new HighEnergyParticlePort.OnParticleCaptureAllowed(this.OnParticleCaptureAllowed));
		}
	}

	public bool AllowIncomingParticles
	{
		get
		{
			return !this.hasLogicWire || (this.hasLogicWire && this.isLogicActive);
		}
	}

	public bool HasLogicWire
	{
		get
		{
			return this.hasLogicWire;
		}
	}

	public bool IsLogicActive
	{
		get
		{
			return this.isLogicActive;
		}
	}

	private LogicCircuitNetwork GetNetwork()
	{
		int portCell = base.GetComponent<LogicPorts>().GetPortCell(HighEnergyParticleRedirector.PORT_ID);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == HighEnergyParticleRedirector.PORT_ID)
		{
			this.isLogicActive = logicValueChanged.newValue > 0;
			this.hasLogicWire = this.GetNetwork() != null;
		}
	}

	private static string ResolveInfoStatusItem(string format_str, object data)
	{
		HighEnergyParticleRedirector highEnergyParticleRedirector = (HighEnergyParticleRedirector)data;
		if (!highEnergyParticleRedirector.HasLogicWire)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.NORMAL;
		}
		if (highEnergyParticleRedirector.IsLogicActive)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_CONTROLLED_ACTIVE;
		}
		return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_CONTROLLED_STANDBY;
	}

	private static string ResolveInfoStatusItemTooltip(string format_str, object data)
	{
		HighEnergyParticleRedirector highEnergyParticleRedirector = (HighEnergyParticleRedirector)data;
		if (!highEnergyParticleRedirector.HasLogicWire)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.NORMAL;
		}
		if (highEnergyParticleRedirector.IsLogicActive)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.LOGIC_CONTROLLED_ACTIVE;
		}
		return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.LOGIC_CONTROLLED_STANDBY;
	}

	public static readonly HashedString PORT_ID = "HEPRedirector";

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private HighEnergyParticleStorage storage;

	[MyCmpGet]
	private HighEnergyParticlePort port;

	public float directorDelay;

	public bool directionControllable = true;

	[Serialize]
	private EightDirection _direction;

	private EightDirectionController directionController;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleRedirector> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleRedirector>(delegate(HighEnergyParticleRedirector component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleRedirector> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleRedirector>(delegate(HighEnergyParticleRedirector component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private bool hasLogicWire;

	private bool isLogicActive;

	private static StatusItem infoStatusItem_Logic;

	public class StatesInstance : GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.GameInstance
	{
		public StatesInstance(HighEnergyParticleRedirector smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.inoperational;
			this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.ready, false);
			this.ready.PlayAnim("on").TagTransition(GameTags.Operational, this.inoperational, true).EventTransition(GameHashes.OnParticleStorageChanged, this.redirect, null);
			this.redirect.PlayAnim("working_pre").QueueAnim("working_loop", false, null).QueueAnim("working_pst", false, null)
				.ScheduleGoTo((HighEnergyParticleRedirector.StatesInstance smi) => smi.master.directorDelay, this.ready)
				.Exit(delegate(HighEnergyParticleRedirector.StatesInstance smi)
				{
					smi.master.LaunchParticle();
				});
		}

		public GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State inoperational;

		public GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State ready;

		public GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State redirect;

		public GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State launch;
	}
}
