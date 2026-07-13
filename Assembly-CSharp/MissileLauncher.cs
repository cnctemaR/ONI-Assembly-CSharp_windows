using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class MissileLauncher : GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.Off;
		this.root.Update(delegate(MissileLauncher.Instance smi, float dt)
		{
			smi.HasLineOfSight();
		}, UpdateRate.SIM_200ms, false);
		this.Off.PlayAnim("inoperational").EventTransition(GameHashes.OperationalChanged, this.On, (MissileLauncher.Instance smi) => smi.Operational.IsOperational).Enter(delegate(MissileLauncher.Instance smi)
		{
			smi.Operational.SetActive(false, false);
		});
		this.On.DefaultState(this.On.opening).EventTransition(GameHashes.OperationalChanged, this.On.shutdown, (MissileLauncher.Instance smi) => !smi.Operational.IsOperational).ParamTransition<bool>(this.fullyBlocked, this.Nosurfacesight, GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.IsTrue)
			.ScheduleGoTo(this.shutdownDuration, this.On.idle)
			.Enter(delegate(MissileLauncher.Instance smi)
			{
				smi.Operational.SetActive(smi.Operational.IsOperational, false);
			});
		this.On.opening.PlayAnim("working_pre").OnAnimQueueComplete(this.On.searching).Target(this.cannonTarget)
			.PlayAnim("Cannon_working_pre");
		this.On.searching.PlayAnim("on", KAnim.PlayMode.Loop).Enter(delegate(MissileLauncher.Instance smi)
		{
			smi.sm.rotationComplete.Set(false, smi, false);
			smi.sm.meteorTarget.Set(null, smi, false);
			smi.cannonRotation = smi.def.scanningAngle;
		}).Update("FindMeteor", delegate(MissileLauncher.Instance smi, float dt)
		{
			smi.Searching(dt);
		}, UpdateRate.SIM_EVERY_TICK, false)
			.EventTransition(GameHashes.OnStorageChange, this.NoAmmo, (MissileLauncher.Instance smi) => smi.MissileStorage.Count <= 0 && smi.LongRangeStorage.Count <= 0)
			.ParamTransition<GameObject>(this.meteorTarget, this.Launch.targeting, (MissileLauncher.Instance smi, GameObject meteor) => meteor != null)
			.ParamTransition<GameObject>(this.longRangeTarget, this.Launch.targetingLongRange, (MissileLauncher.Instance smi, GameObject longrange) => smi.ShouldRotateToLongRange())
			.Exit(delegate(MissileLauncher.Instance smi)
			{
				smi.sm.rotationComplete.Set(false, smi, false);
			});
		this.On.idle.Target(this.masterTarget).PlayAnim("idle", KAnim.PlayMode.Loop).UpdateTransition(this.On, (MissileLauncher.Instance smi, float dt) => smi.Operational.IsOperational && smi.MeteorDetected(), UpdateRate.SIM_200ms, false)
			.EventTransition(GameHashes.ClusterDestinationChanged, this.On.searching, (MissileLauncher.Instance smi) => smi.LongRangeStorage.Count > 0)
			.Target(this.cannonTarget)
			.PlayAnim("Cannon_working_pst");
		this.On.shutdown.Target(this.masterTarget).PlayAnim("working_pst").OnAnimQueueComplete(this.Off)
			.Target(this.cannonTarget)
			.PlayAnim("Cannon_working_pst");
		this.Launch.PlayAnim("target_detected", KAnim.PlayMode.Loop).Update("Rotate", delegate(MissileLauncher.Instance smi, float dt)
		{
			smi.RotateToMeteor(dt);
		}, UpdateRate.SIM_EVERY_TICK, false);
		this.Launch.targeting.Update("Targeting", delegate(MissileLauncher.Instance smi, float dt)
		{
			if (smi.sm.meteorTarget.Get(smi).IsNullOrDestroyed())
			{
				smi.GoTo(this.On.searching);
				return;
			}
			if (smi.cannonAnimController.Rotation < smi.def.maxAngle * -1f || smi.cannonAnimController.Rotation > smi.def.maxAngle)
			{
				smi.sm.meteorTarget.Get(smi).GetComponent<Comet>().Targeted = false;
				smi.sm.meteorTarget.Set(null, smi, false);
				smi.GoTo(this.On.searching);
			}
		}, UpdateRate.SIM_EVERY_TICK, false).ParamTransition<bool>(this.rotationComplete, this.Launch.shoot, GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.IsTrue);
		this.Launch.targetingLongRange.Update("TargetingLongRange", delegate(MissileLauncher.Instance smi, float dt)
		{
		}, UpdateRate.SIM_EVERY_TICK, false).ParamTransition<bool>(this.rotationComplete, this.Launch.shoot, GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.IsTrue);
		this.Launch.shoot.ScheduleGoTo(this.shootDelayDuration, this.Launch.pst).Exit("LaunchMissile", delegate(MissileLauncher.Instance smi)
		{
			if (smi.sm.meteorTarget.Get(smi) != null)
			{
				smi.LaunchMissile();
			}
			else if (smi.sm.longRangeTarget.Get(smi) != null)
			{
				smi.LaunchLongRangeMissile();
			}
			this.cannonTarget.Get(smi).GetComponent<KBatchedAnimController>().Play("Cannon_shooting_pre", KAnim.PlayMode.Once, 1f, 0f);
		});
		this.Launch.pst.Target(this.masterTarget).Enter(delegate(MissileLauncher.Instance smi)
		{
			smi.SetOreChunk();
			KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
			if (smi.GetComponent<Storage>().Count <= 0)
			{
				component.Play("base_shooting_pst_last", KAnim.PlayMode.Once, 1f, 0f);
				return;
			}
			component.Play("base_shooting_pst", KAnim.PlayMode.Once, 1f, 0f);
		}).Target(this.cannonTarget)
			.PlayAnim("Cannon_shooting_pst")
			.OnAnimQueueComplete(this.Cooldown);
		this.Cooldown.Exit(delegate(MissileLauncher.Instance smi)
		{
			smi.SpawnOre();
		}).Enter(delegate(MissileLauncher.Instance smi)
		{
			KAnimControllerBase component2 = smi.GetComponent<KAnimControllerBase>();
			if (smi.GetComponent<Storage>().Count <= 0)
			{
				component2.Play("base_ejecting_last", KAnim.PlayMode.Once, 1f, 0f);
			}
			else
			{
				component2.Play("base_ejecting", KAnim.PlayMode.Once, 1f, 0f);
			}
			smi.sm.rotationComplete.Set(false, smi, false);
			smi.sm.meteorTarget.Set(null, smi, false);
			smi.GoTo(smi.CooldownGoToState);
		});
		this.Cooldown.basic.Update("Rotate", delegate(MissileLauncher.Instance smi, float dt)
		{
			smi.RotateToMeteor(dt);
		}, UpdateRate.SIM_EVERY_TICK, false).OnAnimQueueComplete(this.On.searching);
		this.Cooldown.longrange.QueueAnim("cooldown", true, null).ToggleStatusItem(MissileLauncher.LongRangeCooldown, null).Target(this.cannonTarget)
			.QueueAnim("cooldown_cannon_pre", false, null)
			.QueueAnim("cooldown_cannon", true, null)
			.ScheduleGoTo(MissileLauncher.longrangeCooldownTime, this.On.searching)
			.Exit(delegate(MissileLauncher.Instance smi)
			{
				this.cannonTarget.Get(smi).GetComponent<KBatchedAnimController>().Play("cooldown_cannon_pst", KAnim.PlayMode.Once, 1f, 0f);
			});
		this.Nosurfacesight.Target(this.masterTarget).PlayAnim("working_pst").QueueAnim("error", false, null)
			.ParamTransition<bool>(this.fullyBlocked, this.On, GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.IsFalse)
			.Target(this.cannonTarget)
			.PlayAnim("Cannon_working_pst")
			.Enter(delegate(MissileLauncher.Instance smi)
			{
				smi.Operational.SetActive(false, false);
			});
		this.NoAmmo.PlayAnim("off_open").EventTransition(GameHashes.OnStorageChange, this.On, (MissileLauncher.Instance smi) => smi.MissileStorage.Count > 0 || smi.LongRangeStorage.Count > 0).Enter(delegate(MissileLauncher.Instance smi)
		{
			smi.Operational.SetActive(false, false);
		})
			.Exit(delegate(MissileLauncher.Instance smi)
			{
				smi.GetComponent<KAnimControllerBase>().Play("off_closing", KAnim.PlayMode.Once, 1f, 0f);
			})
			.Target(this.cannonTarget)
			.PlayAnim("Cannon_working_pst");
	}

	private static StatusItem NoSurfaceSight = new StatusItem("MissileLauncher_NoSurfaceSight", "BUILDING", "status_item_no_sky", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);

	private static StatusItem PartiallyBlockedStatus = new StatusItem("MissileLauncher_PartiallyBlocked", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022, null);

	private static StatusItem LongRangeCooldown = new StatusItem("MissileLauncher_LongRangeCooldown", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022, null);

	public float shutdownDuration = 50f;

	public float shootDelayDuration = 0.25f;

	public static float SHELL_MASS = 2.5f;

	public static float SHELL_TEMPERATURE = 353.15f;

	public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.BoolParameter rotationComplete;

	public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.ObjectParameter<GameObject> meteorTarget = new StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.ObjectParameter<GameObject>();

	public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.TargetParameter cannonTarget;

	public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.BoolParameter fullyBlocked;

	public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.ObjectParameter<GameObject> longRangeTarget = new StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.ObjectParameter<GameObject>();

	public static float longrangeCooldownTime = 10f;

	public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State Off;

	public MissileLauncher.OnState On;

	public MissileLauncher.LaunchState Launch;

	public MissileLauncher.CooldownState Cooldown;

	public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State Nosurfacesight;

	public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State NoAmmo;

	public class Def : StateMachine.BaseDef
	{
		public static readonly CellOffset LaunchOffset = new CellOffset(0, 4);

		public float launchSpeed = 30f;

		public float rotationSpeed = 100f;

		public static readonly Vector2I launchRange = new Vector2I(16, 32);

		public float scanningAngle = 50f;

		public float maxAngle = 80f;
	}

	public new class Instance : GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.GameInstance
	{
		public WorldContainer myWorld
		{
			get
			{
				if (this.worldContainer == null)
				{
					this.worldContainer = this.GetMyWorld();
				}
				return this.worldContainer;
			}
		}

		public Instance(IStateMachineTarget master, MissileLauncher.Def def)
			: base(master, def)
		{
			Components.MissileLaunchers.Add(this);
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			string text = component.name + ".cannon";
			base.smi.cannonGameObject = new GameObject(text);
			base.smi.cannonGameObject.SetActive(false);
			base.smi.cannonGameObject.transform.parent = component.transform;
			base.smi.cannonGameObject.AddComponent<KPrefabID>().PrefabTag = new Tag(text);
			base.smi.cannonAnimController = base.smi.cannonGameObject.AddComponent<KBatchedAnimController>();
			base.smi.cannonAnimController.AnimFiles = new KAnimFile[] { component.AnimFiles[0] };
			base.smi.cannonAnimController.initialAnim = "Cannon_off";
			base.smi.cannonAnimController.isMovable = true;
			base.smi.cannonAnimController.SetSceneLayer(Grid.SceneLayer.Building);
			component.SetSymbolVisiblity("cannon_target", false);
			bool flag;
			Vector3 vector = component.GetSymbolTransform(new HashedString("cannon_target"), out flag).GetColumn(3);
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.Building);
			base.smi.cannonGameObject.transform.SetPosition(vector);
			this.launchPosition = vector;
			Grid.PosToXY(this.launchPosition, out this.launchXY);
			base.smi.cannonGameObject.SetActive(true);
			base.smi.sm.cannonTarget.Set(base.smi.cannonGameObject, base.smi, false);
			KAnim.Anim anim = component.AnimFiles[0].GetData().GetAnim("Cannon_shooting_pre");
			if (anim != null)
			{
				this.launchAnimTime = anim.totalTime / 2f;
			}
			else
			{
				global::Debug.LogWarning("MissileLauncher anim data is missing");
				this.launchAnimTime = 1f;
			}
			this.meter = new MeterController(component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			this.longRangemeter = new MeterController(component, "meter_target_longrange", "meter_longrange", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			base.Subscribe(-1201923725, new Action<object>(this.OnHighlight));
			base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
			foreach (Storage storage in base.smi.gameObject.GetComponents<Storage>())
			{
				if (storage.storageID == "MissileBasic")
				{
					this.MissileStorage = storage;
				}
				else if (storage.storageID == "MissileLongRange")
				{
					this.LongRangeStorage = storage;
				}
				else if (storage.storageID == "CondiutStorage")
				{
					this.LoadingStorage = storage;
				}
			}
			base.Subscribe(-1697596308, new Action<object>(this.OnStorage));
			FlatTagFilterable component2 = base.smi.master.GetComponent<FlatTagFilterable>();
			foreach (GameObject gameObject in Assets.GetPrefabsWithTag(GameTags.Comet))
			{
				if (!gameObject.HasTag(GameTags.DeprecatedContent))
				{
					if (!component2.tagOptions.Contains(gameObject.PrefabID()))
					{
						component2.tagOptions.Add(gameObject.PrefabID());
						component2.selectedTags.Add(gameObject.PrefabID());
					}
					component2.selectedTags.Remove(GassyMooCometConfig.ID);
				}
			}
			this.ManualDeliveryKgs = base.smi.gameObject.GetComponents<ManualDeliveryKG>();
		}

		public override void StartSM()
		{
			base.StartSM();
			this.OnStorage(null);
			base.smi.master.GetComponent<FlatTagFilterable>().currentlyUserAssignable = this.AmmunitionIsAllowed("MissileBasic");
			this.clusterDestinationSelector = base.smi.master.GetComponent<EntityClusterDestinationSelector>();
			if (this.clusterDestinationSelector != null)
			{
				this.clusterDestinationSelector.assignable = this.AmmunitionIsAllowed("MissileLongRange");
			}
			this.UpdateAmmunitionDelivery();
			this.UpdateMeterVisibility();
		}

		protected override void OnCleanUp()
		{
			Components.MissileLaunchers.Remove(this);
			base.Unsubscribe(-1201923725, new Action<object>(this.OnHighlight));
			base.OnCleanUp();
		}

		private void OnHighlight(object data)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			base.smi.cannonAnimController.HighlightColour = component.HighlightColour;
		}

		private void OnCopySettings(object data)
		{
			GameObject gameObject = (GameObject)data;
			if (gameObject != null)
			{
				MissileLauncher.Instance smi = gameObject.GetSMI<MissileLauncher.Instance>();
				if (smi != null)
				{
					this.ammunitionPermissions.Clear();
					foreach (KeyValuePair<Tag, bool> keyValuePair in smi.ammunitionPermissions)
					{
						this.ChangeAmmunition(keyValuePair.Key, smi.AmmunitionIsAllowed(keyValuePair.Key));
					}
					base.smi.master.GetComponent<FlatTagFilterable>().currentlyUserAssignable = this.AmmunitionIsAllowed("MissileBasic");
					this.clusterDestinationSelector = base.smi.master.GetComponent<EntityClusterDestinationSelector>();
					if (this.clusterDestinationSelector != null)
					{
						this.clusterDestinationSelector.assignable = this.AmmunitionIsAllowed("MissileLongRange");
					}
					if (smi.sm.longRangeTarget != null)
					{
						base.sm.longRangeTarget.Set(smi.sm.longRangeTarget.Get(smi), this, false);
					}
				}
			}
		}

		private void OnStorage(object data)
		{
			if (this.LoadingStorage.items.Count > 0)
			{
				KPrefabID component = this.LoadingStorage.items[0].GetComponent<KPrefabID>();
				if (this.AmmunitionIsAllowed(component.PrefabTag))
				{
					Pickupable component2 = component.GetComponent<Pickupable>();
					Storage storage = null;
					if (component.PrefabTag == "MissileBasic")
					{
						storage = this.MissileStorage;
					}
					else if (component.PrefabTag == "MissileLongRange")
					{
						storage = this.LongRangeStorage;
					}
					if (storage != null && storage.Capacity() - storage.MassStored() >= component2.PrimaryElement.Mass)
					{
						this.LoadingStorage.Transfer(component2.gameObject, storage, true, true);
					}
				}
			}
			this.meter.SetPositionPercent(Mathf.Clamp01(this.MissileStorage.MassStored() / this.MissileStorage.capacityKg));
			this.longRangemeter.SetPositionPercent(Mathf.Clamp01(this.LongRangeStorage.MassStored() / this.LongRangeStorage.capacityKg));
		}

		private void UpdateMeterVisibility()
		{
			this.meter.gameObject.SetActive(this.AmmunitionIsAllowed("MissileBasic"));
			this.longRangemeter.gameObject.SetActive(this.AmmunitionIsAllowed("MissileLongRange"));
		}

		public void Searching(float dt)
		{
			if (!this.FindMeteor())
			{
				this.FindLongRangeTarget();
			}
			this.RotateCannon(dt, base.def.rotationSpeed / 2f);
			if (base.smi.sm.rotationComplete.Get(base.smi))
			{
				this.cannonRotation *= -1f;
				base.smi.sm.rotationComplete.Set(false, base.smi, false);
			}
		}

		private bool FindMeteor()
		{
			if (this.MissileStorage.items.Count > 0)
			{
				GameObject gameObject = this.ChooseClosestInterceptionPoint(this.myWorld.id);
				if (gameObject != null)
				{
					base.smi.sm.meteorTarget.Set(gameObject, base.smi, false);
					gameObject.GetComponent<Comet>().Targeted = true;
					base.smi.cannonRotation = this.CalculateLaunchAngle(gameObject.transform.position);
					return true;
				}
			}
			return false;
		}

		private bool FindLongRangeTarget()
		{
			if (this.LongRangeStorage.items.Count > 0)
			{
				GameObject gameObject = null;
				if (this.clusterDestinationSelector != null)
				{
					if (this.clusterDestinationSelector.GetDestination() != this.myWorld.GetComponent<ClusterGridEntity>().Location)
					{
						ClusterGridEntity visibleEntityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.clusterDestinationSelector.GetDestination(), EntityLayer.Meteor);
						gameObject = ((visibleEntityOfLayerAtCell != null) ? visibleEntityOfLayerAtCell.gameObject : null);
					}
				}
				else
				{
					GameplayEventInstance gameplayEventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.IdHash, -1);
					if (gameplayEventInstance != null)
					{
						GameObject impactorInstance = ((LargeImpactorEvent.StatesInstance)gameplayEventInstance.smi).impactorInstance;
						gameObject = ((impactorInstance != null) ? impactorInstance.gameObject : null);
					}
				}
				if (gameObject != null)
				{
					Vector3 position = base.transform.position;
					position.y += 50f;
					if (this.IsPathClear(this.launchPosition, position))
					{
						base.smi.sm.longRangeTarget.Set(gameObject, base.smi, false);
						base.smi.cannonRotation = this.CalculateLaunchAngle(position);
						return true;
					}
				}
			}
			return false;
		}

		private float CalculateLaunchAngle(Vector3 targetPosition)
		{
			Vector3 vector = Vector3.Normalize(targetPosition - this.launchPosition);
			return MathUtil.AngleSigned(Vector3.up, vector, Vector3.forward);
		}

		public void LaunchMissile()
		{
			GameObject gameObject = this.MissileStorage.FindFirst("MissileBasic");
			if (gameObject != null)
			{
				Pickupable pickupable = gameObject.GetComponent<Pickupable>();
				if (pickupable.TotalAmount <= 1f)
				{
					this.MissileStorage.Drop(pickupable.gameObject, true);
				}
				else
				{
					pickupable = EntitySplitter.Split(pickupable, 1f, null);
				}
				this.SetMissileElement(gameObject);
				GameObject gameObject2 = base.smi.sm.meteorTarget.Get(base.smi);
				if (!gameObject2.IsNullOrDestroyed())
				{
					pickupable.GetSMI<MissileProjectile.StatesInstance>().PrepareLaunch(gameObject2.GetComponent<Comet>(), base.def.launchSpeed, this.launchPosition, base.smi.cannonRotation);
					this.CooldownGoToState = base.sm.Cooldown.basic;
				}
			}
		}

		public void LaunchLongRangeMissile()
		{
			GameObject gameObject = this.LongRangeStorage.FindFirst("MissileLongRange");
			if (gameObject != null)
			{
				Pickupable pickupable = gameObject.GetComponent<Pickupable>();
				if (pickupable.TotalAmount <= 1f)
				{
					this.LongRangeStorage.Drop(pickupable.gameObject, true);
				}
				else
				{
					pickupable = EntitySplitter.Split(pickupable, 1f, null);
				}
				this.SetMissileElement(gameObject);
				GameObject gameObject2 = base.smi.sm.longRangeTarget.Get(base.smi);
				if (!gameObject2.IsNullOrDestroyed())
				{
					pickupable.GetSMI<MissileLongRangeProjectile.StatesInstance>().PrepareLaunch(gameObject2, base.def.launchSpeed, this.launchPosition, base.smi.cannonRotation);
					this.CooldownGoToState = base.sm.Cooldown.longrange;
					base.smi.sm.longRangeTarget.Set(null, base.smi, false);
				}
			}
		}

		private void SetMissileElement(GameObject missile)
		{
			this.missileElement = missile.GetComponent<PrimaryElement>().Element.tag;
			if (Assets.GetPrefab(this.missileElement) == null)
			{
				global::Debug.LogWarning(string.Format("Missing element {0} for missile launcher. Defaulting to IronOre", this.missileElement));
				this.missileElement = GameTags.IronOre;
			}
		}

		public GameObject ChooseClosestInterceptionPoint(int world_id)
		{
			GameObject gameObject = null;
			List<Comet> items = Components.Meteors.GetItems(world_id);
			float num = (float)MissileLauncher.Def.launchRange.y;
			foreach (Comet comet in items)
			{
				if (!comet.IsNullOrDestroyed() && !comet.Targeted && this.TargetFilter.selectedTags.Contains(comet.typeID))
				{
					Vector3 targetPosition = comet.TargetPosition;
					float num2;
					Vector3 vector = this.CalculateCollisionPoint(targetPosition, comet.Velocity, out num2);
					Grid.PosToCell(vector);
					float num3 = Vector3.Distance(vector, this.launchPosition);
					if (num3 < num && num2 > this.launchAnimTime && this.IsMeteorInRange(vector) && this.IsPathClear(this.launchPosition, targetPosition))
					{
						gameObject = comet.gameObject;
						num = num3;
					}
				}
			}
			return gameObject;
		}

		private bool IsMeteorInRange(Vector3 interception_point)
		{
			Vector2I vector2I;
			Grid.PosToXY(interception_point, out vector2I);
			return Math.Abs(vector2I.X - this.launchXY.X) <= MissileLauncher.Def.launchRange.X && vector2I.Y - this.launchXY.Y > 0 && vector2I.Y - this.launchXY.Y <= MissileLauncher.Def.launchRange.Y;
		}

		public bool IsPathClear(Vector3 startPoint, Vector3 endPoint)
		{
			Vector2I vector2I = Grid.PosToXY(startPoint);
			Vector2I vector2I2 = Grid.PosToXY(endPoint);
			return Grid.TestLineOfSight(vector2I.x, vector2I.y, vector2I2.x, vector2I2.y, new Func<int, bool>(this.IsCellBlockedFromSky), false, true);
		}

		public bool IsCellBlockedFromSky(int cell)
		{
			if (Grid.IsValidCell(cell) && (int)Grid.WorldIdx[cell] == this.myWorld.id)
			{
				return Grid.Solid[cell];
			}
			int num;
			int num2;
			Grid.CellToXY(cell, out num, out num2);
			return num2 <= this.launchXY.Y;
		}

		public Vector3 CalculateCollisionPoint(Vector3 targetPosition, Vector3 targetVelocity, out float timeToCollision)
		{
			Vector3 vector = targetVelocity - base.smi.def.launchSpeed * (targetPosition - this.launchPosition).normalized;
			timeToCollision = (targetPosition - this.launchPosition).magnitude / vector.magnitude;
			return targetPosition + targetVelocity * timeToCollision;
		}

		public void HasLineOfSight()
		{
			bool flag = false;
			bool flag2 = true;
			Extents extents = base.GetComponent<Building>().GetExtents();
			int num = this.launchXY.x - MissileLauncher.Def.launchRange.X;
			int num2 = this.launchXY.x + MissileLauncher.Def.launchRange.X;
			int num3 = extents.y + extents.height;
			int num4 = Grid.XYToCell(Math.Max((int)this.myWorld.minimumBounds.x, num), num3);
			int num5 = Grid.XYToCell(Math.Min((int)this.myWorld.maximumBounds.x, num2), num3);
			for (int i = num4; i <= num5; i++)
			{
				flag = flag || Grid.ExposedToSunlight[i] <= 0;
				flag2 = flag2 && Grid.ExposedToSunlight[i] <= 0;
			}
			this.Selectable.ToggleStatusItem(MissileLauncher.PartiallyBlockedStatus, flag && !flag2, null);
			this.Selectable.ToggleStatusItem(MissileLauncher.NoSurfaceSight, flag2, null);
			base.smi.sm.fullyBlocked.Set(flag2, base.smi, false);
		}

		public bool MeteorDetected()
		{
			return Components.Meteors.GetItems(this.myWorld.id).Count > 0;
		}

		public void SetOreChunk()
		{
			if (!this.missileElement.IsValid)
			{
				global::Debug.LogWarning(string.Format("Missing element {0} for missile launcher. Defaulting to IronOre", this.missileElement));
				this.missileElement = GameTags.IronOre;
			}
			KAnim.Build.Symbol symbolByIndex = Assets.GetPrefab(this.missileElement).GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbolByIndex(0U);
			base.gameObject.GetComponent<SymbolOverrideController>().AddSymbolOverride("Shell", symbolByIndex, 0);
		}

		public void SpawnOre()
		{
			bool flag;
			Vector3 vector = base.GetComponent<KBatchedAnimController>().GetSymbolTransform("Shell", out flag).GetColumn(3);
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			Assets.GetPrefab(this.missileElement).GetComponent<PrimaryElement>().Element.substance.SpawnResource(vector, MissileLauncher.SHELL_MASS, MissileLauncher.SHELL_TEMPERATURE, byte.MaxValue, 0, false, false, false);
		}

		public void RotateCannon(float dt, float rotation_speed)
		{
			float num = this.cannonRotation - this.simpleAngle;
			if (num > 180f)
			{
				num -= 360f;
			}
			else if (num < -180f)
			{
				num += 360f;
			}
			float num2 = rotation_speed * dt;
			if (num > 0f && num2 < num)
			{
				this.simpleAngle += num2;
				this.cannonAnimController.Rotation = this.simpleAngle;
				return;
			}
			if (num < 0f && -num2 > num)
			{
				this.simpleAngle -= num2;
				this.cannonAnimController.Rotation = this.simpleAngle;
				return;
			}
			this.simpleAngle = this.cannonRotation;
			this.cannonAnimController.Rotation = this.simpleAngle;
			base.smi.sm.rotationComplete.Set(true, base.smi, false);
		}

		public bool ShouldRotateToLongRange()
		{
			return !base.smi.sm.longRangeTarget.Get(base.smi).IsNullOrDestroyed() && this.LongRangeStorage.items.Count > 0 && this.IsPathClear(this.launchPosition, this.launchPosition + new Vector3(0f, 50f, 0f));
		}

		public void RotateToMeteor(float dt)
		{
			GameObject gameObject = base.sm.meteorTarget.Get(this);
			float num;
			if (!gameObject.IsNullOrDestroyed())
			{
				num = this.CalculateLaunchAngle(gameObject.transform.position);
			}
			else
			{
				if (!this.ShouldRotateToLongRange())
				{
					return;
				}
				Vector3 position = base.transform.position;
				position.y += 50f;
				num = this.CalculateLaunchAngle(position);
			}
			float num2 = num - this.simpleAngle;
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			else if (num2 < -180f)
			{
				num2 += 360f;
			}
			float num3 = base.def.rotationSpeed * dt;
			if (num2 > 0f && num3 < num2)
			{
				this.simpleAngle += num3;
				this.cannonAnimController.Rotation = this.simpleAngle;
				return;
			}
			if (num2 < 0f && -num3 > num2)
			{
				this.simpleAngle -= num3;
				this.cannonAnimController.Rotation = this.simpleAngle;
				return;
			}
			base.smi.sm.rotationComplete.Set(true, base.smi, false);
		}

		public void ChangeAmmunition(Tag tag, bool allowed)
		{
			if (!this.ammunitionPermissions.ContainsKey(tag))
			{
				this.ammunitionPermissions.Add(tag, false);
			}
			this.ammunitionPermissions[tag] = allowed;
			this.UpdateAmmunitionDelivery();
			this.OnStorage(null);
			this.UpdateMeterVisibility();
		}

		public bool AmmunitionIsAllowed(Tag tag)
		{
			return this.ammunitionPermissions.ContainsKey(tag) && this.ammunitionPermissions[tag];
		}

		private void UpdateAmmunitionDelivery()
		{
			foreach (ManualDeliveryKG manualDeliveryKG in this.ManualDeliveryKgs)
			{
				bool flag = this.AmmunitionIsAllowed(manualDeliveryKG.RequestedItemTag);
				manualDeliveryKG.Pause(!flag, "ammunitionnotallowed");
			}
		}

		[MyCmpReq]
		public Operational Operational;

		public Storage MissileStorage;

		public Storage LongRangeStorage;

		private Storage LoadingStorage;

		public ManualDeliveryKG[] ManualDeliveryKgs;

		[MyCmpReq]
		public KSelectable Selectable;

		[MyCmpReq]
		public FlatTagFilterable TargetFilter;

		private EntityClusterDestinationSelector clusterDestinationSelector;

		[Serialize]
		private Dictionary<Tag, bool> ammunitionPermissions = new Dictionary<Tag, bool> { { "MissileBasic", true } };

		private Vector3 launchPosition;

		private Vector2I launchXY;

		private float launchAnimTime;

		public KBatchedAnimController cannonAnimController;

		public GameObject cannonGameObject;

		public float cannonRotation;

		public float simpleAngle;

		private Tag missileElement;

		private MeterController meter;

		private MeterController longRangemeter;

		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State CooldownGoToState;

		private WorldContainer worldContainer;
	}

	public class OnState : GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State
	{
		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State searching;

		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State opening;

		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State shutdown;

		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State idle;
	}

	public class LaunchState : GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State
	{
		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State targeting;

		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State targetingLongRange;

		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State shoot;

		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State pst;
	}

	public class CooldownState : GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State
	{
		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State longrange;

		public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State basic;
	}
}
