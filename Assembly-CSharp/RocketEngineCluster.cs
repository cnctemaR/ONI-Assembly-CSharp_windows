using System;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RocketEngineCluster : StateMachineComponent<RocketEngineCluster.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (this.mainEngine)
		{
			base.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new RequireAttachedComponent(base.gameObject.GetComponent<AttachableBuilding>(), typeof(IFuelTank), UI.STARMAP.COMPONENT.FUEL_TANK));
			if (this.requireOxidizer)
			{
				base.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new RequireAttachedComponent(base.gameObject.GetComponent<AttachableBuilding>(), typeof(OxidizerTank), UI.STARMAP.COMPONENT.OXIDIZER_TANK));
			}
			base.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionRocketHeight(this));
		}
	}

	private void ConfigureFlameLight()
	{
		this.flameLight = base.gameObject.AddOrGet<Light2D>();
		this.flameLight.Color = Color.white;
		this.flameLight.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
		this.flameLight.Range = 10f;
		this.flameLight.Angle = 0f;
		this.flameLight.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
		this.flameLight.Offset = LIGHT2D.LIGHTBUG_OFFSET;
		this.flameLight.shape = global::LightShape.Circle;
		this.flameLight.drawOverlay = true;
		this.flameLight.Lux = 80000;
		this.flameLight.emitter.RemoveFromGrid();
		base.gameObject.AddOrGet<LightSymbolTracker>().targetSymbol = base.GetComponent<KBatchedAnimController>().CurrentAnim.rootSymbol;
		this.flameLight.enabled = false;
	}

	private void UpdateFlameLight(int cell)
	{
		base.smi.master.flameLight.RefreshShapeAndPosition();
		if (Grid.IsValidCell(cell))
		{
			if (!base.smi.master.flameLight.enabled && base.smi.timeinstate > 3f)
			{
				base.smi.master.flameLight.enabled = true;
				return;
			}
		}
		else
		{
			base.smi.master.flameLight.enabled = false;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public float exhaustEmitRate = 50f;

	public float exhaustTemperature = 1500f;

	public SpawnFXHashes explosionEffectHash;

	public SimHashes exhaustElement = SimHashes.CarbonDioxide;

	public Tag fuelTag;

	public float efficiency = 1f;

	public bool requireOxidizer = true;

	public int maxModules = 32;

	public int maxHeight;

	public bool mainEngine = true;

	public byte exhaustDiseaseIdx = byte.MaxValue;

	public int exhaustDiseaseCount;

	public bool emitRadiation;

	[MyCmpGet]
	private RadiationEmitter radiationEmitter;

	[MyCmpGet]
	private Generator powerGenerator;

	[MyCmpReq]
	private KBatchedAnimController animController;

	public Light2D flameLight;

	public class StatesInstance : GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.GameInstance
	{
		public StatesInstance(RocketEngineCluster smi)
			: base(smi)
		{
			if (smi.emitRadiation)
			{
				DebugUtil.Assert(smi.radiationEmitter != null, "emitRadiation enabled but no RadiationEmitter component");
				this.radiationEmissionBaseOffset = smi.radiationEmitter.emissionOffset;
			}
		}

		public void BeginBurn()
		{
			if (base.smi.master.emitRadiation)
			{
				base.smi.master.radiationEmitter.SetEmitting(true);
			}
			LaunchPad currentPad = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
			if (currentPad != null)
			{
				this.pad_cell = Grid.PosToCell(currentPad.gameObject.transform.GetPosition());
				if (base.smi.master.exhaustDiseaseIdx != 255)
				{
					currentPad.GetComponent<PrimaryElement>().AddDisease(base.smi.master.exhaustDiseaseIdx, base.smi.master.exhaustDiseaseCount, "rocket exhaust");
					return;
				}
			}
			else
			{
				global::Debug.LogWarning("RocketEngineCluster missing LaunchPad for burn.");
				this.pad_cell = Grid.InvalidCell;
			}
		}

		public void DoBurn(float dt)
		{
			int num = Grid.PosToCell(base.smi.master.gameObject.transform.GetPosition() + base.smi.master.animController.Offset);
			if (Grid.AreCellsInSameWorld(num, this.pad_cell))
			{
				SimMessages.EmitMass(num, ElementLoader.GetElementIndex(base.smi.master.exhaustElement), dt * base.smi.master.exhaustEmitRate, base.smi.master.exhaustTemperature, base.smi.master.exhaustDiseaseIdx, base.smi.master.exhaustDiseaseCount, -1);
			}
			if (base.smi.master.emitRadiation)
			{
				Vector3 emissionOffset = base.smi.master.radiationEmitter.emissionOffset;
				base.smi.master.radiationEmitter.emissionOffset = base.smi.radiationEmissionBaseOffset + base.smi.master.animController.Offset;
				if (Grid.AreCellsInSameWorld(base.smi.master.radiationEmitter.GetEmissionCell(), this.pad_cell))
				{
					base.smi.master.radiationEmitter.Refresh();
				}
				else
				{
					base.smi.master.radiationEmitter.emissionOffset = emissionOffset;
					base.smi.master.radiationEmitter.SetEmitting(false);
				}
			}
			int num2 = 10;
			for (int i = 1; i < num2; i++)
			{
				int num3 = Grid.OffsetCell(num, -1, -i);
				int num4 = Grid.OffsetCell(num, 0, -i);
				int num5 = Grid.OffsetCell(num, 1, -i);
				if (Grid.AreCellsInSameWorld(num3, this.pad_cell))
				{
					if (base.smi.master.exhaustDiseaseIdx != 255)
					{
						SimMessages.ModifyDiseaseOnCell(num3, base.smi.master.exhaustDiseaseIdx, (int)((float)base.smi.master.exhaustDiseaseCount / ((float)i + 1f)));
					}
					SimMessages.ModifyEnergy(num3, base.smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
				}
				if (Grid.AreCellsInSameWorld(num4, this.pad_cell))
				{
					if (base.smi.master.exhaustDiseaseIdx != 255)
					{
						SimMessages.ModifyDiseaseOnCell(num4, base.smi.master.exhaustDiseaseIdx, (int)((float)base.smi.master.exhaustDiseaseCount / (float)i));
					}
					SimMessages.ModifyEnergy(num4, base.smi.master.exhaustTemperature / (float)i, 3200f, SimMessages.EnergySourceID.Burner);
				}
				if (Grid.AreCellsInSameWorld(num5, this.pad_cell))
				{
					if (base.smi.master.exhaustDiseaseIdx != 255)
					{
						SimMessages.ModifyDiseaseOnCell(num5, base.smi.master.exhaustDiseaseIdx, (int)((float)base.smi.master.exhaustDiseaseCount / ((float)i + 1f)));
					}
					SimMessages.ModifyEnergy(num5, base.smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
				}
			}
		}

		public void EndBurn()
		{
			if (base.smi.master.emitRadiation)
			{
				base.smi.master.radiationEmitter.emissionOffset = base.smi.radiationEmissionBaseOffset;
				base.smi.master.radiationEmitter.SetEmitting(false);
			}
			this.pad_cell = Grid.InvalidCell;
		}

		public Vector3 radiationEmissionBaseOffset;

		private int pad_cell;
	}

	public class States : GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.initializing.load;
			this.initializing.load.ScheduleGoTo(0f, this.initializing.decide);
			this.initializing.decide.Transition(this.space, new StateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.Transition.ConditionCallback(this.IsRocketInSpace), UpdateRate.SIM_200ms).Transition(this.burning, new StateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.Transition.ConditionCallback(this.IsRocketAirborne), UpdateRate.SIM_200ms).Transition(this.idle, new StateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.Transition.ConditionCallback(this.IsRocketGrounded), UpdateRate.SIM_200ms);
			this.idle.DefaultState(this.idle.grounded).EventTransition(GameHashes.RocketLaunched, this.burning_pre, null);
			this.idle.grounded.EventTransition(GameHashes.LaunchConditionChanged, this.idle.ready, new StateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.Transition.ConditionCallback(this.IsReadyToLaunch)).QueueAnim("grounded", true, null);
			this.idle.ready.EventTransition(GameHashes.LaunchConditionChanged, this.idle.grounded, GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.Not(new StateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.Transition.ConditionCallback(this.IsReadyToLaunch))).PlayAnim("pre_ready_to_launch", KAnim.PlayMode.Once).QueueAnim("ready_to_launch", true, null)
				.Exit(delegate(RocketEngineCluster.StatesInstance smi)
				{
					KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
					if (component != null)
					{
						component.Play("pst_ready_to_launch", KAnim.PlayMode.Once, 1f, 0f);
					}
				});
			this.burning_pre.PlayAnim("launch_pre").OnAnimQueueComplete(this.burning);
			this.burning.EventTransition(GameHashes.RocketLanded, this.burnComplete, null).PlayAnim("launch_loop", KAnim.PlayMode.Loop).Enter(delegate(RocketEngineCluster.StatesInstance smi)
			{
				smi.BeginBurn();
			})
				.Update(delegate(RocketEngineCluster.StatesInstance smi, float dt)
				{
					smi.DoBurn(dt);
				}, UpdateRate.SIM_200ms, false)
				.Exit(delegate(RocketEngineCluster.StatesInstance smi)
				{
					smi.EndBurn();
				})
				.TagTransition(GameTags.RocketInSpace, this.space, false);
			this.space.EventTransition(GameHashes.DoReturnRocket, this.burning, null);
			this.burnComplete.PlayAnim("launch_pst", KAnim.PlayMode.Loop).GoTo(this.idle);
		}

		private bool IsReadyToLaunch(RocketEngineCluster.StatesInstance smi)
		{
			return smi.GetComponent<RocketModuleCluster>().CraftInterface.CheckPreppedForLaunch();
		}

		public bool IsRocketAirborne(RocketEngineCluster.StatesInstance smi)
		{
			return smi.master.HasTag(GameTags.RocketNotOnGround) && !smi.master.HasTag(GameTags.RocketInSpace);
		}

		public bool IsRocketGrounded(RocketEngineCluster.StatesInstance smi)
		{
			return smi.master.HasTag(GameTags.RocketOnGround);
		}

		public bool IsRocketInSpace(RocketEngineCluster.StatesInstance smi)
		{
			return smi.master.HasTag(GameTags.RocketInSpace);
		}

		public RocketEngineCluster.States.InitializingStates initializing;

		public RocketEngineCluster.States.IdleStates idle;

		public GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State burning_pre;

		public GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State burning;

		public GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State burnComplete;

		public GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State space;

		public class InitializingStates : GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State
		{
			public GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State load;

			public GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State decide;
		}

		public class IdleStates : GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State
		{
			public GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State grounded;

			public GameStateMachine<RocketEngineCluster.States, RocketEngineCluster.StatesInstance, RocketEngineCluster, object>.State ready;
		}
	}
}
