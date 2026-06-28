using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[SkipSerialization]
public class Flatulence : StateMachineComponent<Flatulence.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(1623392196, new EventSystem.EventHandler(this.OnDeath));
		this.Subscribe(-1117766961, new EventSystem.EventHandler(this.OnRevived));
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void Emit(object data)
	{
		GameObject gameObject = (GameObject)data;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		Vector2 vector = gameObject.transform.position;
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if (minionIdentity.gameObject != gameObject.gameObject)
			{
				Vector2 vector2 = minionIdentity.transform.position;
				float num = Vector2.SqrMagnitude(vector - vector2);
				if (num <= 2.25f)
				{
					minionIdentity.Trigger(508119890, Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
					minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
				}
			}
		}
		int num2 = Grid.PosToCell(gameObject.transform.position);
		float value = Db.Get().Amounts.Temperature.Lookup(this).value;
		SimMessages.AddRemoveSubstance(num2, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.0050000004f, value, -1);
		KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("odor_fx", gameObject.transform, true, Grid.SceneLayer.Front);
		kbatchedAnimController.transform.localPosition = Vector3.zero;
		kbatchedAnimController.Play(new string[] { "working_pre", "working_loop", "working_pst" }, KAnim.PlayMode.Once);
		kbatchedAnimController.destroyOnAnimComplete = true;
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

	private const float EmitMass = 0.0050000004f;

	private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;

	private const float EmissionRadius = 1.5f;

	private const float MaxDistanceSq = 2.25f;

	public class StatesInstance : GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence>.GameInstance
	{
		public StatesInstance(Flatulence master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.Enter("ScheduleNextFart", delegate(Flatulence.StatesInstance smi)
			{
				smi.ScheduleGoTo(this.GetNewInterval(), this.emit);
			});
			this.emit.Enter("Fart", delegate(Flatulence.StatesInstance smi)
			{
				smi.master.Emit(smi.master.gameObject);
			}).ToggleExpression(Db.Get().Expressions.Relief, null).ScheduleGoTo(3f, this.idle);
		}

		private float GetNewInterval()
		{
			float num = TRAITS.FLATULENCE_EMIT_INTERVAL_MAX - TRAITS.FLATULENCE_EMIT_INTERVAL_MIN;
			float num2 = Util.GaussianRandom(num, 1f);
			num2 = Mathf.Max(num2, TRAITS.FLATULENCE_EMIT_INTERVAL_MIN);
			return Mathf.Min(num2, TRAITS.FLATULENCE_EMIT_INTERVAL_MAX);
		}

		public GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence>.State idle;

		public GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence>.State emit;
	}
}
