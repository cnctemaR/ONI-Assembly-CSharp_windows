using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MissileProjectile : GameStateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ParamTransition<Comet>(this.meteorTarget, this.launch, (MissileProjectile.StatesInstance smi, Comet comet) => comet != null);
		this.launch.Update("Launch", delegate(MissileProjectile.StatesInstance smi, float dt)
		{
			smi.UpdateLaunch(dt);
		}, UpdateRate.SIM_EVERY_TICK, false).ParamTransition<bool>(this.triggerexplode, this.explode, GameStateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>.IsTrue).Enter(delegate(MissileProjectile.StatesInstance smi)
		{
			Vector3 position = smi.master.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
			smi.smokeTrailFX = Util.KInstantiate(EffectPrefabs.Instance.MissileSmokeTrailFX, position);
			smi.smokeTrailFX.transform.SetParent(smi.master.transform);
			smi.smokeTrailFX.SetActive(true);
			smi.StartTakeoff();
			KFMOD.PlayOneShot(GlobalAssets.GetSound("MissileLauncher_Missile_ignite", false), CameraController.Instance.GetVerticallyScaledPosition(position, false), 1f);
		});
		this.explode.Enter(delegate(MissileProjectile.StatesInstance smi)
		{
			smi.TriggerExplosion();
			ParticleSystem[] componentsInChildren = smi.smokeTrailFX.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].emission.enabled = false;
			}
		});
	}

	public GameStateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>.State launch;

	public GameStateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>.State explode;

	public StateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>.BoolParameter triggerexplode = new StateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>.BoolParameter(false);

	public StateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>.ObjectParameter<Comet> meteorTarget = new StateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>.ObjectParameter<Comet>();

	public class Def : StateMachine.BaseDef
	{
		public float ExplosionRange = 2f;

		public string explosionEffectAnim = "missile_explosion_kanim";
	}

	public class StatesInstance : GameStateMachine<MissileProjectile, MissileProjectile.StatesInstance, IStateMachineTarget, MissileProjectile.Def>.GameInstance
	{
		private Vector3 Position
		{
			get
			{
				return base.transform.position + this.animController.Offset;
			}
		}

		public StatesInstance(IStateMachineTarget master, MissileProjectile.Def def)
			: base(master, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
		}

		public void StartTakeoff()
		{
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
		}

		public void UpdateLaunch(float dt)
		{
			int myWorldId = base.gameObject.GetMyWorldId();
			Comet comet = base.sm.meteorTarget.Get(base.smi);
			if (!comet.IsNullOrDestroyed())
			{
				Vector3 targetPosition = comet.TargetPosition;
				base.sm.triggerexplode.Set(this.InExplosionRange(targetPosition, this.Position), base.smi, false);
				Vector3 vector = Vector3.Normalize(targetPosition - this.Position);
				Vector3 normalized = (targetPosition - this.Position).normalized;
				float num = MathUtil.AngleSigned(Vector3.up, vector, Vector3.forward);
				this.animController.Rotation = num;
				if (Grid.IsValidCellInWorld(Grid.PosToCell(this.Position), myWorldId))
				{
					base.transform.SetPosition(base.transform.position + normalized * (this.launchSpeed * dt));
				}
				else
				{
					this.animController.Offset += normalized * (this.launchSpeed * dt);
				}
				ParticleSystem[] componentsInChildren = base.smi.smokeTrailFX.GetComponentsInChildren<ParticleSystem>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.transform.SetPositionAndRotation(this.Position, Quaternion.identity);
				}
				return;
			}
			if (!base.sm.triggerexplode.Get(base.smi))
			{
				if (!base.smi.smokeTrailFX.IsNullOrDestroyed())
				{
					Util.KDestroyGameObject(base.smi.smokeTrailFX);
				}
				if (!GameComps.Fallers.Has(base.gameObject))
				{
					GameComps.Fallers.Add(base.gameObject, Vector2.down);
				}
				base.gameObject.GetComponent<KSelectable>().enabled = true;
				base.smi.GoTo("root");
			}
		}

		public void PrepareLaunch(Comet meteor_target, float speed, Vector3 launchPos, float launchAngle)
		{
			base.gameObject.transform.SetParent(null);
			base.gameObject.layer = LayerMask.NameToLayer("Default");
			launchPos.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
			base.gameObject.transform.SetLocalPosition(launchPos);
			this.animController.Rotation = launchAngle;
			this.animController.Offset = Vector3.back;
			this.animController.SetVisiblity(true);
			base.sm.triggerexplode.Set(false, base.smi, false);
			base.sm.meteorTarget.Set(meteor_target, base.smi, false);
			this.launchSpeed = speed;
		}

		public void TriggerExplosion()
		{
			if (!base.smi.sm.meteorTarget.IsNullOrDestroyed())
			{
				Util.KDestroyGameObject(base.smi.sm.meteorTarget.Get(base.smi));
			}
			this.Explode();
		}

		private void Explode()
		{
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
			Vector3 position = base.gameObject.transform.position;
			position.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
			this.SpawnExplosionFX(base.def.explosionEffectAnim, position, this.animController.Offset);
			this.animController.SetSymbolVisiblity("missile_body", false);
			this.animController.SetSymbolVisiblity("missile_head", false);
		}

		private bool InExplosionRange(Vector3 target_pos, Vector3 current_pos)
		{
			return Vector2.Distance(target_pos, current_pos) <= base.def.ExplosionRange;
		}

		private void SpawnExplosionFX(string anim, Vector3 pos, Vector3 offset)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect(anim, pos, base.gameObject.transform, false, Grid.SceneLayer.FXFront2, false);
			kbatchedAnimController.Offset = offset;
			kbatchedAnimController.Play("idle", KAnim.PlayMode.Once, 1f, 0f);
			kbatchedAnimController.onAnimComplete += delegate(HashedString obj)
			{
				Util.KDestroyGameObject(base.gameObject);
			};
		}

		public KBatchedAnimController animController;

		private float launchSpeed;

		public GameObject smokeTrailFX;
	}
}
