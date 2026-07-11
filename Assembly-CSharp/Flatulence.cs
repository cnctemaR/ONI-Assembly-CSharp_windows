using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Flatulence : StateMachineComponent<Flatulence.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.Subscribe<Flatulence>(1623392196, Flatulence.OnDeathDelegate);
		base.Subscribe<Flatulence>(-1117766961, Flatulence.OnRevivedDelegate);
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void Emit(object data)
	{
		GameObject gameObject = (GameObject)data;
		float value = Db.Get().Amounts.Temperature.Lookup(this).value;
		Equippable equippable = base.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
		if (equippable != null)
		{
			equippable.GetComponent<Storage>().AddGasChunk(SimHashes.Methane, 0.1f, value, byte.MaxValue, 0, false, true);
		}
		else
		{
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
						minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
					}
				}
			}
			int num2 = Grid.PosToCell(gameObject.transform.GetPosition());
			SimMessages.AddRemoveSubstance(num2, SimHashes.Methane, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.1f, value, byte.MaxValue, 0, true, -1);
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("odor_fx_kanim", gameObject.transform.GetPosition(), gameObject.transform, true, Grid.SceneLayer.Front, false);
			kbatchedAnimController.Play(Flatulence.WorkLoopAnims, KAnim.PlayMode.Once);
			kbatchedAnimController.destroyOnAnimComplete = true;
		}
		GameObject gameObject2 = gameObject;
		bool flag = SoundEvent.ObjectIsSelectedAndVisible(gameObject2);
		Vector3 vector3 = gameObject2.GetComponent<Transform>().GetPosition();
		vector3.z = 0f;
		float num3 = 1f;
		if (flag)
		{
			vector3 = SoundEvent.AudioHighlightListenerPosition(vector3);
			num3 = SoundEvent.GetVolume(flag);
		}
		else
		{
			vector3.z = 0f;
		}
		KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence", false), vector3, num3);
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

	private const float EmitMass = 0.1f;

	private const SimHashes EmitElement = SimHashes.Methane;

	private const float EmissionRadius = 1.5f;

	private const float MaxDistanceSq = 2.25f;

	private static readonly EventSystem.IntraObjectHandler<Flatulence> OnDeathDelegate = new EventSystem.IntraObjectHandler<Flatulence>(delegate(Flatulence component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Flatulence> OnRevivedDelegate = new EventSystem.IntraObjectHandler<Flatulence>(delegate(Flatulence component, object data)
	{
		component.OnRevived(data);
	});

	private static readonly HashedString[] WorkLoopAnims = new HashedString[] { "working_pre", "working_loop", "working_pst" };

	public class StatesInstance : GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.GameInstance
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

		public GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.State idle;

		public GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.State emit;
	}
}
