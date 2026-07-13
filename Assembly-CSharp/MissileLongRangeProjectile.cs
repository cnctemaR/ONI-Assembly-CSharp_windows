using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MissileLongRangeProjectile : GameStateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ParamTransition<GameObject>(this.asteroidTarget, this.launch, (MissileLongRangeProjectile.StatesInstance smi, GameObject target) => !target.IsNullOrDestroyed());
		this.launch.Update("Launch", delegate(MissileLongRangeProjectile.StatesInstance smi, float dt)
		{
			smi.UpdateLaunch(dt);
		}, UpdateRate.SIM_EVERY_TICK, false).ParamTransition<bool>(this.triggeroutofworld, this.leaveworld, GameStateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>.IsTrue).Enter(delegate(MissileLongRangeProjectile.StatesInstance smi)
		{
			Vector3 position = smi.master.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
			smi.smokeTrailFX = Util.KInstantiate(EffectPrefabs.Instance.LongRangeMissileSmokeTrailFX, position);
			smi.smokeTrailFX.transform.SetParent(smi.master.transform);
			smi.smokeTrailFX.SetActive(true);
			smi.StartTakeoff();
			KFMOD.PlayOneShot(GlobalAssets.GetSound("MissileLauncher_Missile_ignite", false), CameraController.Instance.GetVerticallyScaledPosition(position, false), 1f);
		});
		this.leaveworld.Enter(delegate(MissileLongRangeProjectile.StatesInstance smi)
		{
			smi.ExitWorldEnterStarmap();
		});
	}

	public GameStateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>.State launch;

	public GameStateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>.State leaveworld;

	public StateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>.BoolParameter triggeroutofworld = new StateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>.BoolParameter(false);

	public StateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>.ObjectParameter<GameObject> asteroidTarget = new StateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>.ObjectParameter<GameObject>();

	public class Def : StateMachine.BaseDef
	{
	}

	public class StatesInstance : GameStateMachine<MissileLongRangeProjectile, MissileLongRangeProjectile.StatesInstance, IStateMachineTarget, MissileLongRangeProjectile.Def>.GameInstance
	{
		private Vector3 Position
		{
			get
			{
				return base.transform.position + this.animController.Offset;
			}
		}

		public StatesInstance(IStateMachineTarget master, MissileLongRangeProjectile.Def def)
			: base(master, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
		}

		public override void StartSM()
		{
			base.StartSM();
			if (this.launchedTarget.Get() != null)
			{
				base.sm.asteroidTarget.Set(this.launchedTarget.Get().gameObject, this, false);
				this.myWorld = ClusterManager.Instance.GetWorld(this.myWorldId);
			}
		}

		public void StartTakeoff()
		{
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
			base.GetComponent<Pickupable>().handleFallerComponents = false;
		}

		public void UpdateLaunch(float dt)
		{
			float num = MathUtil.AngleSigned(Vector3.up, Vector3.up, Vector3.forward);
			this.animController.Rotation = num;
			int num2 = Grid.PosToCell(this.Position);
			Vector2I vector2I = Grid.CellToXY(num2);
			if (!Grid.IsValidCell(num2))
			{
				base.smi.sm.triggeroutofworld.Set(true, base.smi, false);
				return;
			}
			if (Grid.IsValidCellInWorld(Grid.PosToCell(this.Position), this.myWorldId) && (float)vector2I.y < this.myWorld.maximumBounds.y)
			{
				base.transform.SetPosition(base.transform.position + Vector3.up * (this.launchSpeed * dt));
			}
			else
			{
				this.animController.Offset += Vector3.up * (this.launchSpeed * dt);
			}
			ParticleSystem[] componentsInChildren = base.smi.smokeTrailFX.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.transform.SetPositionAndRotation(this.Position, Quaternion.identity);
			}
		}

		public void PrepareLaunch(GameObject asteroid_target, float speed, Vector3 launchPos, float launchAngle)
		{
			base.gameObject.transform.SetParent(null);
			base.gameObject.layer = LayerMask.NameToLayer("Default");
			launchPos.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
			base.gameObject.transform.SetLocalPosition(launchPos);
			this.animController.Rotation = launchAngle;
			this.animController.Offset = Vector3.back;
			this.animController.SetVisiblity(true);
			FetchableMonitor.Instance smi = base.gameObject.GetSMI<FetchableMonitor.Instance>();
			if (smi != null)
			{
				smi.SetForceUnfetchable(true);
			}
			base.sm.triggeroutofworld.Set(false, base.smi, false);
			base.sm.asteroidTarget.Set(asteroid_target, base.smi, false);
			this.launchedTarget = new Ref<KPrefabID>(asteroid_target.GetComponent<KPrefabID>());
			this.launchSpeed = speed;
			this.myWorld = base.gameObject.GetMyWorld();
			this.myWorldId = this.myWorld.id;
			ClusterGridEntity component = this.myWorld.GetComponent<ClusterGridEntity>();
			if (component != null)
			{
				this.myLocation = component.Location;
			}
		}

		public void ExitWorldEnterStarmap()
		{
			GameObject gameObject = base.sm.asteroidTarget.Get(base.smi);
			if (gameObject != null)
			{
				ClusterGridEntity component = gameObject.GetComponent<ClusterGridEntity>();
				if (component != null)
				{
					GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab("ClusterMapLongRangeMissile"), Grid.SceneLayer.NoLayer, null, 0);
					gameObject2.SetActive(true);
					gameObject2.GetSMI<ClusterMapLongRangeMissile.StatesInstance>().Setup(this.myLocation, component);
				}
				else
				{
					gameObject.Trigger(-2056344675, MissileLongRangeConfig.DamageEventPayload.sharedInstance);
				}
			}
			Util.KDestroyGameObject(base.gameObject);
		}

		public KBatchedAnimController animController;

		[Serialize]
		private float launchSpeed;

		public GameObject smokeTrailFX;

		private WorldContainer myWorld;

		[Serialize]
		private AxialI myLocation;

		[Serialize]
		private int myWorldId = -1;

		[Serialize]
		private Ref<KPrefabID> launchedTarget = new Ref<KPrefabID>();
	}
}
