using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ManualHighEnergyParticleSpawner : StateMachineComponent<ManualHighEnergyParticleSpawner.StatesInstance>, IHighEnergyParticleDirection
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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<ManualHighEnergyParticleSpawner>(-905833192, ManualHighEnergyParticleSpawner.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.radiationEmitter.SetEmitting(false);
		this.directionController = new EightDirectionController(base.GetComponent<KBatchedAnimController>(), "redirector_target", "redirect", EightDirectionController.Offset.Infront);
		this.Direction = this.Direction;
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation, true);
	}

	private void OnCopySettings(object data)
	{
		ManualHighEnergyParticleSpawner component = ((GameObject)data).GetComponent<ManualHighEnergyParticleSpawner>();
		if (component != null)
		{
			this.Direction = component.Direction;
		}
	}

	public void LauncherUpdate()
	{
		if (this.particleStorage.Particles > 0f)
		{
			int highEnergyParticleOutputCell = base.GetComponent<Building>().GetHighEnergyParticleOutputCell();
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2, null, 0);
			gameObject.SetActive(true);
			if (gameObject != null)
			{
				HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
				component.payload = this.particleStorage.ConsumeAndGet(this.particleStorage.Particles);
				component.SetDirection(this.Direction);
				this.directionController.PlayAnim("redirect_send", KAnim.PlayMode.Once);
				this.directionController.controller.Queue("redirect", KAnim.PlayMode.Once, 1f, 0f);
			}
		}
	}

	[MyCmpReq]
	private HighEnergyParticleStorage particleStorage;

	[MyCmpGet]
	private RadiationEmitter radiationEmitter;

	[Serialize]
	private EightDirection _direction;

	private EightDirectionController directionController;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<ManualHighEnergyParticleSpawner> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ManualHighEnergyParticleSpawner>(delegate(ManualHighEnergyParticleSpawner component, object data)
	{
		component.OnCopySettings(data);
	});

	public class StatesInstance : GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.GameInstance
	{
		public StatesInstance(ManualHighEnergyParticleSpawner smi)
			: base(smi)
		{
		}

		public bool IsComplexFabricatorWorkable(object data)
		{
			return data as ComplexFabricatorWorkable != null;
		}
	}

	public class States : GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.inoperational;
			this.inoperational.Enter(delegate(ManualHighEnergyParticleSpawner.StatesInstance smi)
			{
				smi.master.radiationEmitter.SetEmitting(false);
			}).TagTransition(GameTags.Operational, this.ready, false);
			this.ready.DefaultState(this.ready.idle).TagTransition(GameTags.Operational, this.inoperational, true).Update(delegate(ManualHighEnergyParticleSpawner.StatesInstance smi, float dt)
			{
				smi.master.LauncherUpdate();
			}, UpdateRate.SIM_200ms, false);
			this.ready.idle.EventHandlerTransition(GameHashes.WorkableStartWork, this.ready.working, (ManualHighEnergyParticleSpawner.StatesInstance smi, object data) => smi.IsComplexFabricatorWorkable(data));
			this.ready.working.Enter(delegate(ManualHighEnergyParticleSpawner.StatesInstance smi)
			{
				smi.master.radiationEmitter.SetEmitting(true);
			}).EventHandlerTransition(GameHashes.WorkableCompleteWork, this.ready.idle, (ManualHighEnergyParticleSpawner.StatesInstance smi, object data) => smi.IsComplexFabricatorWorkable(data)).EventHandlerTransition(GameHashes.WorkableStopWork, this.ready.idle, (ManualHighEnergyParticleSpawner.StatesInstance smi, object data) => smi.IsComplexFabricatorWorkable(data))
				.Exit(delegate(ManualHighEnergyParticleSpawner.StatesInstance smi)
				{
					smi.master.radiationEmitter.SetEmitting(false);
				});
		}

		public GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State inoperational;

		public ManualHighEnergyParticleSpawner.States.ReadyStates ready;

		public class ReadyStates : GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State
		{
			public GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State idle;

			public GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State working;
		}
	}
}
