using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Stinky : StateMachineComponent<Stinky.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(1623392196, new Action<object>(this.OnDeath));
		base.Subscribe(-1117766961, new Action<object>(this.OnRevived));
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void Emit(object data)
	{
		GameObject gameObject = (GameObject)data;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		Vector2 vector = gameObject.transform.GetPosition();
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if (minionIdentity.gameObject != gameObject.gameObject)
			{
				Vector2 vector2 = minionIdentity.transform.GetPosition();
				float num = Vector2.SqrMagnitude(vector - vector2);
				if (num <= 2.25f)
				{
					minionIdentity.Trigger(508119890, Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
					minionIdentity.GetComponent<Effects>().Add("SmelledStinky", true);
					minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
				}
			}
		}
		int num2 = Grid.PosToCell(gameObject.transform.GetPosition());
		float value = Db.Get().Amounts.Temperature.Lookup(this).value;
		SimMessages.AddRemoveSubstance(num2, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.0025000002f, value, byte.MaxValue, 0, -1);
		KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence", false), base.transform.GetPosition());
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}

	public void ModifyTrait(Trait t)
	{
	}

	private const float EmitMass = 0.0025000002f;

	private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;

	private const float EmissionRadius = 1.5f;

	private const float MaxDistanceSq = 2.25f;

	private KBatchedAnimController stinkyController;

	private static readonly HashedString[] WorkLoopAnims = new HashedString[] { "working_pre", "working_loop", "working_pst" };

	public class StatesInstance : GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.GameInstance
	{
		public StatesInstance(Stinky master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.Enter(delegate(Stinky.StatesInstance smi)
			{
				KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("odor_fx_kanim", smi.master.gameObject.transform.GetPosition(), smi.master.gameObject.transform, true, Grid.SceneLayer.Front, false);
				kbatchedAnimController.Play(Stinky.WorkLoopAnims, KAnim.PlayMode.Once);
				smi.master.stinkyController = kbatchedAnimController;
			}).Update("StinkyFX", delegate(Stinky.StatesInstance smi, float dt)
			{
				if (smi.master.stinkyController != null)
				{
					smi.master.stinkyController.Play(Stinky.WorkLoopAnims, KAnim.PlayMode.Once);
				}
			}, UpdateRate.SIM_4000ms, false);
			this.idle.Enter("ScheduleNextFart", delegate(Stinky.StatesInstance smi)
			{
				smi.ScheduleGoTo(this.GetNewInterval(), this.emit);
			});
			this.emit.Enter("Fart", delegate(Stinky.StatesInstance smi)
			{
				smi.master.Emit(smi.master.gameObject);
			}).ToggleExpression(Db.Get().Expressions.Relief, null).ScheduleGoTo(3f, this.idle);
		}

		private float GetNewInterval()
		{
			float num = TRAITS.STINKY_EMIT_INTERVAL_MAX - TRAITS.STINKY_EMIT_INTERVAL_MIN;
			float num2 = Util.GaussianRandom(num, 1f);
			num2 = Mathf.Max(num2, TRAITS.STINKY_EMIT_INTERVAL_MIN);
			return Mathf.Min(num2, TRAITS.STINKY_EMIT_INTERVAL_MAX);
		}

		public GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.State idle;

		public GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.State emit;
	}
}
