using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LaunchableRocketCluster : StateMachineComponent<LaunchableRocketCluster.StatesInstance>, ILaunchableRocket
{
	public IList<Ref<RocketModuleCluster>> parts
	{
		get
		{
			return base.GetComponent<RocketModuleCluster>().CraftInterface.ClusterModules;
		}
	}

	public bool isLanding { get; private set; }

	public float rocketSpeed { get; private set; }

	public LaunchableRocketRegisterType registerType
	{
		get
		{
			return LaunchableRocketRegisterType.Clustercraft;
		}
	}

	public GameObject LaunchableGameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public List<GameObject> GetEngines()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Ref<RocketModuleCluster> @ref in this.parts)
		{
			if (@ref.Get().GetComponent<RocketEngineCluster>())
			{
				list.Add(@ref.Get().gameObject);
			}
		}
		return list;
	}

	private int GetRocketHeight()
	{
		int num = 0;
		foreach (Ref<RocketModuleCluster> @ref in this.parts)
		{
			num += @ref.Get().GetComponent<Building>().Def.HeightInCells;
		}
		return num;
	}

	private float InitialFlightAnimOffsetForLanding()
	{
		int num = Grid.PosToCell(base.gameObject);
		return ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[num]).maximumBounds.y - base.gameObject.transform.GetPosition().y + (float)this.GetRocketHeight() + 100f;
	}

	[Serialize]
	private int takeOffLocation;

	private GameObject soundSpeakerObject;

	public class StatesInstance : GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.GameInstance
	{
		private float heightLaunchSpeedRatio
		{
			get
			{
				return Mathf.Pow((float)base.master.GetRocketHeight(), TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().heightSpeedPower) * TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().heightSpeedFactor;
			}
		}

		public float DistanceAboveGround
		{
			get
			{
				return base.sm.distanceAboveGround.Get(this);
			}
			set
			{
				base.sm.distanceAboveGround.Set(value, this, false);
			}
		}

		public StatesInstance(LaunchableRocketCluster master)
			: base(master)
		{
			this.takeoffAccelPowerInv = 1f / TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower;
		}

		public void SetMissionState(Spacecraft.MissionState state)
		{
			global::Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
			SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>()).SetState(state);
		}

		public Spacecraft.MissionState GetMissionState()
		{
			global::Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
			return SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>()).state;
		}

		public bool IsGrounded()
		{
			return base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.Grounded;
		}

		public bool IsNotSpaceBound()
		{
			Clustercraft component = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			return component.Status == Clustercraft.CraftStatus.Grounded || component.Status == Clustercraft.CraftStatus.Landing;
		}

		public bool IsNotGroundBound()
		{
			Clustercraft component = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			return component.Status == Clustercraft.CraftStatus.Launching || component.Status == Clustercraft.CraftStatus.InFlight;
		}

		public void SetupLaunch()
		{
			base.master.isLanding = false;
			base.master.rocketSpeed = 0f;
			base.sm.warmupTimeRemaining.Set(5f, this, false);
			base.sm.distanceAboveGround.Set(0f, this, false);
			if (base.master.soundSpeakerObject == null)
			{
				base.master.soundSpeakerObject = new GameObject("rocketSpeaker");
				base.master.soundSpeakerObject.transform.SetParent(base.master.gameObject.transform);
			}
			foreach (Ref<RocketModuleCluster> @ref in base.master.parts)
			{
				if (@ref != null)
				{
					base.master.takeOffLocation = Grid.PosToCell(base.master.gameObject);
					@ref.Get().Trigger(-1277991738, base.master.gameObject);
				}
			}
			CraftModuleInterface craftInterface = base.master.GetComponent<RocketModuleCluster>().CraftInterface;
			if (craftInterface != null)
			{
				craftInterface.Trigger(-1277991738, base.master.gameObject);
				WorldContainer component = craftInterface.GetComponent<WorldContainer>();
				List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(component.id, false);
				MinionMigrationEventArgs e = new MinionMigrationEventArgs
				{
					prevWorldId = component.id,
					targetWorldId = component.id
				};
				foreach (MinionIdentity minionIdentity in worldItems)
				{
					e.minionId = minionIdentity;
					Game.Instance.Trigger(586301400, e);
				}
			}
			Game.Instance.Trigger(-1277991738, base.gameObject);
			this.constantVelocityPhase_maxSpeed = 0f;
		}

		public void LaunchLoop(float dt)
		{
			base.master.isLanding = false;
			if (this.constantVelocityPhase_maxSpeed == 0f)
			{
				float num = Mathf.Pow((Mathf.Pow(TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance, this.takeoffAccelPowerInv) * this.heightLaunchSpeedRatio - 0.033f) / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
				this.constantVelocityPhase_maxSpeed = (TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance - num) / 0.033f;
			}
			if (base.sm.warmupTimeRemaining.Get(this) > 0f)
			{
				base.sm.warmupTimeRemaining.Delta(-dt, this);
			}
			else if (this.DistanceAboveGround < TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance)
			{
				float num2 = Mathf.Pow(this.DistanceAboveGround, this.takeoffAccelPowerInv) * this.heightLaunchSpeedRatio;
				num2 += dt;
				this.DistanceAboveGround = Mathf.Pow(num2 / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
				float num3 = Mathf.Pow((num2 - 0.033f) / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
				base.master.rocketSpeed = (this.DistanceAboveGround - num3) / 0.033f;
			}
			else
			{
				base.master.rocketSpeed = this.constantVelocityPhase_maxSpeed;
				this.DistanceAboveGround += base.master.rocketSpeed * dt;
			}
			this.UpdateSoundSpeakerObject();
			if (this.UpdatePartsAnimPositionsAndDamage(true) == 0)
			{
				base.smi.GoTo(base.sm.not_grounded.space);
			}
		}

		public void FinalizeLaunch()
		{
			base.master.rocketSpeed = 0f;
			this.DistanceAboveGround = base.sm.distanceToSpace.Get(base.smi);
			foreach (Ref<RocketModuleCluster> @ref in base.master.parts)
			{
				if (@ref != null && !(@ref.Get() == null))
				{
					RocketModuleCluster rocketModuleCluster = @ref.Get();
					rocketModuleCluster.GetComponent<KBatchedAnimController>().Offset = Vector3.up * this.DistanceAboveGround;
					rocketModuleCluster.GetComponent<KBatchedAnimController>().enabled = false;
					rocketModuleCluster.GetComponent<RocketModule>().MoveToSpace();
				}
			}
			base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().SetCraftStatus(Clustercraft.CraftStatus.InFlight);
		}

		public void SetupLanding()
		{
			float num = base.master.InitialFlightAnimOffsetForLanding();
			this.DistanceAboveGround = num;
			base.sm.warmupTimeRemaining.Set(2f, this, false);
			base.master.isLanding = true;
			base.master.rocketSpeed = 0f;
			this.constantVelocityPhase_maxSpeed = 0f;
		}

		public void LandingLoop(float dt)
		{
			base.master.isLanding = true;
			if (this.constantVelocityPhase_maxSpeed == 0f)
			{
				float num = Mathf.Pow((Mathf.Pow(TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance, this.takeoffAccelPowerInv) * this.heightLaunchSpeedRatio - 0.033f) / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
				this.constantVelocityPhase_maxSpeed = (num - TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance) / 0.033f;
			}
			if (this.DistanceAboveGround > TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance)
			{
				base.master.rocketSpeed = this.constantVelocityPhase_maxSpeed;
				this.DistanceAboveGround += base.master.rocketSpeed * dt;
			}
			else if (this.DistanceAboveGround > 0.0025f)
			{
				float num2 = Mathf.Pow(this.DistanceAboveGround, this.takeoffAccelPowerInv) * this.heightLaunchSpeedRatio;
				num2 -= dt;
				this.DistanceAboveGround = Mathf.Pow(num2 / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
				float num3 = Mathf.Pow((num2 - 0.033f) / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
				base.master.rocketSpeed = (this.DistanceAboveGround - num3) / 0.033f;
			}
			else if (base.sm.warmupTimeRemaining.Get(this) > 0f)
			{
				base.sm.warmupTimeRemaining.Delta(-dt, this);
				this.DistanceAboveGround = 0f;
			}
			this.UpdateSoundSpeakerObject();
			this.UpdatePartsAnimPositionsAndDamage(true);
		}

		public void FinalizeLanding()
		{
			base.GetComponent<KSelectable>().IsSelectable = true;
			base.master.rocketSpeed = 0f;
			this.DistanceAboveGround = 0f;
			foreach (Ref<RocketModuleCluster> @ref in base.smi.master.parts)
			{
				if (@ref != null && !(@ref.Get() == null))
				{
					@ref.Get().GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
				}
			}
			base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().SetCraftStatus(Clustercraft.CraftStatus.Grounded);
		}

		private void UpdateSoundSpeakerObject()
		{
			if (base.master.soundSpeakerObject == null)
			{
				base.master.soundSpeakerObject = new GameObject("rocketSpeaker");
				base.master.soundSpeakerObject.transform.SetParent(base.gameObject.transform);
			}
			base.master.soundSpeakerObject.transform.SetLocalPosition(this.DistanceAboveGround * Vector3.up);
		}

		public int UpdatePartsAnimPositionsAndDamage(bool doDamage = true)
		{
			int num = base.gameObject.GetMyWorldId();
			if (num == -1)
			{
				return 0;
			}
			LaunchPad currentPad = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
			if (currentPad != null)
			{
				num = currentPad.GetMyWorldId();
			}
			int num2 = 0;
			foreach (Ref<RocketModuleCluster> @ref in base.master.parts)
			{
				if (@ref != null)
				{
					RocketModuleCluster rocketModuleCluster = @ref.Get();
					KBatchedAnimController component = rocketModuleCluster.GetComponent<KBatchedAnimController>();
					component.Offset = Vector3.up * this.DistanceAboveGround;
					Vector3 positionIncludingOffset = component.PositionIncludingOffset;
					int num3 = Grid.PosToCell(component.transform.GetPosition());
					bool flag = Grid.IsValidCell(num3);
					bool flag2 = flag && (int)Grid.WorldIdx[num3] == num;
					if (component.enabled != flag2)
					{
						component.enabled = flag2;
					}
					if (doDamage && flag)
					{
						num2++;
						LaunchableRocketCluster.States.DoWorldDamage(rocketModuleCluster.gameObject, positionIncludingOffset, num);
					}
				}
			}
			return num2;
		}

		private float takeoffAccelPowerInv;

		private float constantVelocityPhase_maxSpeed;

		public class Tuning : TuningData<LaunchableRocketCluster.StatesInstance.Tuning>
		{
			public float takeoffAccelPower = 4f;

			public float maxAccelerationDistance = 25f;

			public float warmupTime = 5f;

			public float heightSpeedPower = 0.5f;

			public float heightSpeedFactor = 4f;

			public int maxAccelHeight = 40;
		}
	}

	public class States : GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.grounded.EventTransition(GameHashes.DoLaunchRocket, this.not_grounded.launch_setup, null).EnterTransition(this.not_grounded.launch_loop, (LaunchableRocketCluster.StatesInstance smi) => smi.IsNotGroundBound()).Enter(delegate(LaunchableRocketCluster.StatesInstance smi)
			{
				smi.FinalizeLanding();
			});
			this.not_grounded.launch_setup.Enter(delegate(LaunchableRocketCluster.StatesInstance smi)
			{
				smi.SetupLaunch();
				this.distanceToSpace.Set((float)ConditionFlightPathIsClear.PadTopEdgeDistanceToOutOfScreenEdge(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.gameObject), smi, false);
				smi.GoTo(this.not_grounded.launch_loop);
			});
			this.not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, this.not_grounded.landing_setup, null).Enter(delegate(LaunchableRocketCluster.StatesInstance smi)
			{
				smi.UpdatePartsAnimPositionsAndDamage(false);
			}).Update(delegate(LaunchableRocketCluster.StatesInstance smi, float dt)
			{
				smi.LaunchLoop(dt);
			}, UpdateRate.SIM_EVERY_TICK, false)
				.ParamTransition<float>(this.distanceAboveGround, this.not_grounded.launch_pst, (LaunchableRocketCluster.StatesInstance smi, float p) => p >= this.distanceToSpace.Get(smi))
				.TriggerOnEnter(GameHashes.StartRocketLaunch, null)
				.Exit(delegate(LaunchableRocketCluster.StatesInstance smi)
				{
					WorldContainer myWorld = smi.gameObject.GetMyWorld();
					if (myWorld != null)
					{
						myWorld.RevealSurface();
					}
				});
			this.not_grounded.launch_pst.ScheduleGoTo(0f, this.not_grounded.space);
			this.not_grounded.space.EnterTransition(this.not_grounded.landing_setup, (LaunchableRocketCluster.StatesInstance smi) => smi.IsNotSpaceBound()).EventTransition(GameHashes.DoReturnRocket, this.not_grounded.landing_setup, null).Enter(delegate(LaunchableRocketCluster.StatesInstance smi)
			{
				smi.FinalizeLaunch();
			});
			this.not_grounded.landing_setup.Enter(delegate(LaunchableRocketCluster.StatesInstance smi)
			{
				smi.SetupLanding();
				smi.GoTo(this.not_grounded.landing_loop);
			});
			this.not_grounded.landing_loop.Enter(delegate(LaunchableRocketCluster.StatesInstance smi)
			{
				smi.UpdatePartsAnimPositionsAndDamage(false);
			}).Update(delegate(LaunchableRocketCluster.StatesInstance smi, float dt)
			{
				smi.LandingLoop(dt);
			}, UpdateRate.SIM_EVERY_TICK, false).ParamTransition<float>(this.distanceAboveGround, this.not_grounded.land, new StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Parameter<float>.Callback(this.IsFullyLanded<float>))
				.ParamTransition<float>(this.warmupTimeRemaining, this.not_grounded.land, new StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Parameter<float>.Callback(this.IsFullyLanded<float>));
			this.not_grounded.land.TriggerOnEnter(GameHashes.RocketTouchDown, null).Enter(delegate(LaunchableRocketCluster.StatesInstance smi)
			{
				foreach (Ref<RocketModuleCluster> @ref in smi.master.parts)
				{
					if (@ref != null && !(@ref.Get() == null))
					{
						@ref.Get().Trigger(-887025858, smi.gameObject);
					}
				}
				CraftModuleInterface craftInterface = smi.master.GetComponent<RocketModuleCluster>().CraftInterface;
				if (craftInterface != null)
				{
					craftInterface.Trigger(-887025858, smi.gameObject);
					WorldContainer component = craftInterface.GetComponent<WorldContainer>();
					List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(component.id, false);
					MinionMigrationEventArgs e = new MinionMigrationEventArgs
					{
						prevWorldId = component.id,
						targetWorldId = component.id
					};
					foreach (MinionIdentity minionIdentity in worldItems)
					{
						e.minionId = minionIdentity;
						Game.Instance.Trigger(586301400, e);
					}
				}
				Game.Instance.Trigger(-887025858, smi.gameObject);
				if (craftInterface != null)
				{
					craftInterface.GetPassengerModule().RemovePassengersOnOtherWorlds();
				}
				smi.GoTo(this.grounded);
			});
		}

		public bool IsFullyLanded<T>(LaunchableRocketCluster.StatesInstance smi, T p)
		{
			return this.distanceAboveGround.Get(smi) <= 0.0025f && this.warmupTimeRemaining.Get(smi) <= 0f;
		}

		public static void DoWorldDamage(GameObject part, Vector3 apparentPosition, int actualWorld)
		{
			OccupyArea component = part.GetComponent<OccupyArea>();
			component.UpdateOccupiedArea();
			foreach (CellOffset cellOffset in component.OccupiedCellsOffsets)
			{
				int num = Grid.OffsetCell(Grid.PosToCell(apparentPosition), cellOffset);
				if (Grid.IsValidCell(num) && Grid.WorldIdx[num] == Grid.WorldIdx[actualWorld])
				{
					if (Grid.Solid[num])
					{
						WorldDamage.Instance.ApplyDamage(num, 10000f, num, BUILDINGS.DAMAGESOURCES.ROCKET, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
					}
					else if (Grid.FakeFloor[num])
					{
						GameObject gameObject = Grid.Objects[num, 39];
						if (gameObject != null && gameObject.HasTag(GameTags.GantryExtended))
						{
							BuildingHP component2 = gameObject.GetComponent<BuildingHP>();
							if (component2 != null)
							{
								gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
								{
									damage = component2.MaxHitPoints,
									source = BUILDINGS.DAMAGESOURCES.ROCKET,
									popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET
								});
							}
						}
					}
				}
			}
		}

		public StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.FloatParameter warmupTimeRemaining;

		public StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.FloatParameter distanceAboveGround;

		public StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.FloatParameter distanceToSpace;

		public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State grounded;

		public LaunchableRocketCluster.States.NotGroundedStates not_grounded;

		public class NotGroundedStates : GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State
		{
			public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State launch_setup;

			public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State launch_loop;

			public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State launch_pst;

			public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State space;

			public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State landing_setup;

			public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State landing_loop;

			public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State land;
		}
	}
}
