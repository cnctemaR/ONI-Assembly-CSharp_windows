using System;
using Klei.AI;
using UnityEngine;

public class PollinationVFXMonitor : GameStateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.idle.EventTransition(GameHashes.EffectAdded, this.pollinated, new StateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>.Transition.ConditionCallback(PollinationVFXMonitor.IsPollinated));
		this.pollinated.EventTransition(GameHashes.EffectRemoved, this.idle, GameStateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>.Not(new StateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>.Transition.ConditionCallback(PollinationVFXMonitor.IsPollinated))).Toggle("Toggle Pollination VFX", new StateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>.State.Callback(PollinationVFXMonitor.CreatePollinationEffect), new StateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>.State.Callback(PollinationVFXMonitor.DestroyPollinationEffect));
	}

	private static bool IsPollinated(PollinationVFXMonitor.Instance smi)
	{
		return smi.IsPollinated();
	}

	private static void DestroyPollinationEffect(PollinationVFXMonitor.Instance smi)
	{
		smi.DestroyPollinationEffect();
	}

	private static void CreatePollinationEffect(PollinationVFXMonitor.Instance smi)
	{
		smi.CreatePollinationEffect();
	}

	private GameStateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>.State idle;

	private GameStateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>.State pollinated;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<PollinationVFXMonitor, PollinationVFXMonitor.Instance, IStateMachineTarget, PollinationVFXMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, PollinationVFXMonitor.Def def)
			: base(master, def)
		{
			this.effects = base.GetComponent<Effects>();
			this.occupyArea = base.GetComponent<OccupyArea>();
		}

		public override void StartSM()
		{
			this.isHangingPlant = base.gameObject.HasTag(GameTags.Hanging);
			base.StartSM();
		}

		public bool IsPollinated()
		{
			if (this.effects == null)
			{
				return false;
			}
			foreach (HashedString hashedString in PollinationMonitor.PollinationEffects)
			{
				if (this.effects.HasEffect(hashedString))
				{
					return true;
				}
			}
			return false;
		}

		public void CreatePollinationEffect()
		{
			this.DestroyPollinationEffect();
			Vector4 vector = new Vector4(float.MaxValue, float.MinValue, float.MaxValue, float.MinValue);
			foreach (CellOffset cellOffset in this.occupyArea.OccupiedCellsOffsets)
			{
				if ((float)cellOffset.x < vector.x)
				{
					vector.x = (float)cellOffset.x;
				}
				if ((float)cellOffset.x > vector.y)
				{
					vector.y = (float)cellOffset.x;
				}
				if ((float)cellOffset.y < vector.z)
				{
					vector.z = (float)cellOffset.y;
				}
				if ((float)cellOffset.y > vector.w)
				{
					vector.w = (float)cellOffset.y;
				}
			}
			int num = 1 + (int)Mathf.Clamp(vector.y - vector.x, 0f, 2.1474836E+09f);
			int num2 = 1 + (int)Mathf.Clamp(vector.w - vector.z, 0f, 2.1474836E+09f);
			Vector3 vector2 = Grid.CellToPosCBC(this.occupyArea.GetOffsetCellWithRotation(new CellOffset(0, this.isHangingPlant ? (-num2 + 1) : 0)), Grid.SceneLayer.BuildingFront);
			GameObject gameObject = Util.KInstantiate(EffectPrefabs.Instance.PlantPollinated, vector2, Quaternion.identity, base.gameObject, "PollinationVFX", true, 0);
			this.pollinationEffect = gameObject.GetComponent<ParticleSystem>();
			ParticleSystem.ShapeModule shape = this.pollinationEffect.shape;
			Vector3 scale = shape.scale;
			Vector3 position = shape.position;
			scale.x = (float)num;
			scale.y = (float)num2;
			position.y = (float)num2 * 0.5f;
			shape.scale = scale;
			shape.position = position;
		}

		public void DestroyPollinationEffect()
		{
			if (this.pollinationEffect != null)
			{
				this.pollinationEffect.DeleteObject();
				this.pollinationEffect = null;
			}
		}

		private Effects effects;

		private ParticleSystem pollinationEffect;

		private OccupyArea occupyArea;

		private bool isHangingPlant;
	}
}
