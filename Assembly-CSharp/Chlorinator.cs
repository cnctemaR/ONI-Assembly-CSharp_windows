using System;
using Klei;
using UnityEngine;

public class Chlorinator : GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.inoperational.TagTransition(GameTags.Operational, this.ready, false);
		this.ready.TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.ready.idle);
		this.ready.idle.EventTransition(GameHashes.OnStorageChange, this.ready.wait, (Chlorinator.StatesInstance smi) => smi.CanEmit()).EnterTransition(this.ready.wait, (Chlorinator.StatesInstance smi) => smi.CanEmit()).Target(this.hopper)
			.PlayAnim("hopper_idle_loop");
		this.ready.wait.ScheduleGoTo(new Func<Chlorinator.StatesInstance, float>(Chlorinator.GetPoppingDelay), this.ready.popPre).EnterTransition(this.ready.idle, (Chlorinator.StatesInstance smi) => !smi.CanEmit()).Target(this.hopper)
			.PlayAnim("hopper_idle_loop");
		this.ready.popPre.Target(this.hopper).PlayAnim("meter_hopper_pre").OnAnimQueueComplete(this.ready.pop);
		this.ready.pop.Enter(delegate(Chlorinator.StatesInstance smi)
		{
			smi.TryEmit();
		}).Target(this.hopper).PlayAnim("meter_hopper_loop")
			.OnAnimQueueComplete(this.ready.popPst);
		this.ready.popPst.Target(this.hopper).PlayAnim("meter_hopper_pst").OnAnimQueueComplete(this.ready.wait);
	}

	public static float GetPoppingDelay(Chlorinator.StatesInstance smi)
	{
		return smi.def.popWaitRange.Get();
	}

	private GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.State inoperational;

	private Chlorinator.ReadyStates ready;

	public StateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.TargetParameter hopper;

	public class Def : StateMachine.BaseDef
	{
		public MathUtil.MinMax popWaitRange = new MathUtil.MinMax(0.2f, 0.8f);

		public Tag primaryOreTag;

		public float primaryOreMassPerOre;

		public MathUtil.MinMaxInt primaryOreCount = new MathUtil.MinMaxInt(1, 1);

		public Tag secondaryOreTag;

		public float secondaryOreMassPerOre;

		public MathUtil.MinMaxInt secondaryOreCount = new MathUtil.MinMaxInt(1, 1);

		public Vector3 offset = Vector3.zero;

		public MathUtil.MinMax initialVelocity = new MathUtil.MinMax(1f, 3f);

		public MathUtil.MinMax initialDirectionHalfAngleDegreesRange = new MathUtil.MinMax(160f, 20f);
	}

	public class ReadyStates : GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.State
	{
		public GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.State idle;

		public GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.State wait;

		public GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.State popPre;

		public GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.State pop;

		public GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.State popPst;
	}

	public class StatesInstance : GameStateMachine<Chlorinator, Chlorinator.StatesInstance, IStateMachineTarget, Chlorinator.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, Chlorinator.Def def)
			: base(master, def)
		{
			this.storage = base.GetComponent<ComplexFabricator>().outStorage;
			KAnimControllerBase component = master.GetComponent<KAnimControllerBase>();
			this.hopperMeter = new MeterController(component, "meter_target", "meter_hopper_pre", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[] { "meter_target" });
			base.sm.hopper.Set(this.hopperMeter.gameObject, this, false);
		}

		public bool CanEmit()
		{
			return !this.storage.IsEmpty();
		}

		public void TryEmit()
		{
			this.TryEmit(base.smi.def.primaryOreCount.Get(), base.def.primaryOreTag, base.def.primaryOreMassPerOre);
			this.TryEmit(base.smi.def.secondaryOreCount.Get(), base.def.secondaryOreTag, base.def.secondaryOreMassPerOre);
		}

		private void TryEmit(int oreSpawnCount, Tag emitTag, float amount)
		{
			GameObject gameObject = this.storage.FindFirst(emitTag);
			if (gameObject == null)
			{
				return;
			}
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			Substance substance = component.Element.substance;
			float num;
			SimUtil.DiseaseInfo diseaseInfo;
			float num2;
			this.storage.ConsumeAndGetDisease(emitTag, amount, out num, out diseaseInfo, out num2);
			if (num <= 0f)
			{
				return;
			}
			float num3 = num * component.MassPerUnit / (float)oreSpawnCount;
			Vector3 vector = base.smi.gameObject.transform.position;
			vector += base.def.offset;
			bool flag = global::UnityEngine.Random.value >= 0.5f;
			for (int i = 0; i < oreSpawnCount; i++)
			{
				float num4 = base.def.initialDirectionHalfAngleDegreesRange.Get() * 3.1415927f / 180f;
				Vector2 normalized = new Vector2(-Mathf.Cos(num4), Mathf.Sin(num4));
				if (flag)
				{
					normalized.x = -normalized.x;
				}
				flag = !flag;
				normalized = normalized.normalized;
				Vector3 vector2 = normalized * base.def.initialVelocity.Get();
				Vector3 vector3 = vector;
				vector3 += normalized * 0.1f;
				GameObject gameObject2 = substance.SpawnResource(vector3, num3, num2, diseaseInfo.idx, diseaseInfo.count / oreSpawnCount, false, false, false);
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Chlorinator_popping", false), CameraController.Instance.GetVerticallyScaledPosition(vector3, false), 1f);
				if (GameComps.Fallers.Has(gameObject2))
				{
					GameComps.Fallers.Remove(gameObject2);
				}
				GameComps.Fallers.Add(gameObject2, vector2);
			}
		}

		public Storage storage;

		public MeterController hopperMeter;
	}
}
