using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class SlimeSickness : Sickness
	{
		public SlimeSickness()
			: base("SlimeSickness", Sickness.SicknessType.Pathogen, Sickness.Severity.Minor, 0.00025f, new List<Sickness.InfectionVector> { Sickness.InfectionVector.Inhalation }, 2220f, "SlimeSicknessRecovery")
		{
			base.AddSicknessComponent(new CommonSickEffectSickness());
			base.AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[]
			{
				new AttributeModifier("BreathDelta", -1.1363636f, DUPLICANTS.DISEASES.SLIMESICKNESS.NAME, false, false, true),
				new AttributeModifier("Athletics", -3f, DUPLICANTS.DISEASES.SLIMESICKNESS.NAME, false, false, true)
			}));
			base.AddSicknessComponent(new AnimatedSickness(new HashedString[] { "anim_idle_sick_kanim" }, Db.Get().Expressions.Sick));
			base.AddSicknessComponent(new PeriodicEmoteSickness(Db.Get().Emotes.Minion.Sick, 50f));
			base.AddSicknessComponent(new SlimeSickness.SlimeLungComponent());
		}

		private const float COUGH_FREQUENCY = 20f;

		private const float COUGH_MASS = 0.1f;

		private const int DISEASE_AMOUNT = 1000;

		public const string ID = "SlimeSickness";

		public const string RECOVERY_ID = "SlimeSicknessRecovery";

		public class SlimeLungComponent : Sickness.SicknessComponent
		{
			public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
			{
				SlimeSickness.SlimeLungComponent.StatesInstance statesInstance = new SlimeSickness.SlimeLungComponent.StatesInstance(diseaseInstance);
				statesInstance.StartSM();
				return statesInstance;
			}

			public override void OnCure(GameObject go, object instance_data)
			{
				((SlimeSickness.SlimeLungComponent.StatesInstance)instance_data).StopSM("Cured");
			}

			public override List<Descriptor> GetSymptoms()
			{
				return new List<Descriptor>
				{
					new Descriptor(DUPLICANTS.DISEASES.SLIMESICKNESS.COUGH_SYMPTOM, DUPLICANTS.DISEASES.SLIMESICKNESS.COUGH_SYMPTOM_TOOLTIP, Descriptor.DescriptorType.SymptomAidable, false)
				};
			}

			public class StatesInstance : GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.GameInstance
			{
				public StatesInstance(SicknessInstance master)
					: base(master)
				{
				}

				public Reactable GetReactable()
				{
					Emote cough = Db.Get().Emotes.Minion.Cough;
					SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.master.gameObject, "SlimeLungCough", Db.Get().ChoreTypes.Cough, 0f, 0f, float.PositiveInfinity, 0f);
					selfEmoteReactable.SetEmote(cough);
					selfEmoteReactable.RegisterEmoteStepCallbacks("react", null, new Action<GameObject>(this.FinishedCoughing));
					return selfEmoteReactable;
				}

				private void ProduceSlime(GameObject cougher)
				{
					AmountInstance amountInstance = Db.Get().Amounts.Temperature.Lookup(cougher);
					int num = Grid.PosToCell(cougher);
					string id = Db.Get().Diseases.SlimeGerms.Id;
					Equippable equippable = base.master.gameObject.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
					if (equippable != null)
					{
						equippable.GetComponent<Storage>().AddGasChunk(SimHashes.ContaminatedOxygen, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex(id), 1000, false, true);
					}
					else
					{
						SimMessages.AddRemoveSubstance(num, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.Cough, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex(id), 1000, true, -1);
					}
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, base.master.modifier.Name, 1000), cougher.transform, 1.5f, false);
				}

				private void FinishedCoughing(GameObject cougher)
				{
					this.ProduceSlime(cougher);
					base.sm.coughFinished.Trigger(this);
				}

				public float lastCoughTime;
			}

			public class States : GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance>
			{
				public override void InitializeStates(out StateMachine.BaseState default_state)
				{
					default_state = this.breathing;
					this.breathing.DefaultState(this.breathing.normal).TagTransition(GameTags.NoOxygen, this.notbreathing, false);
					this.breathing.normal.Enter("SetCoughTime", delegate(SlimeSickness.SlimeLungComponent.StatesInstance smi)
					{
						if (smi.lastCoughTime < Time.time)
						{
							smi.lastCoughTime = Time.time;
						}
					}).Update("Cough", delegate(SlimeSickness.SlimeLungComponent.StatesInstance smi, float dt)
					{
						if (!smi.master.IsDoctored && Time.time - smi.lastCoughTime > 20f)
						{
							smi.GoTo(this.breathing.cough);
						}
					}, UpdateRate.SIM_4000ms, false);
					this.breathing.cough.ToggleReactable((SlimeSickness.SlimeLungComponent.StatesInstance smi) => smi.GetReactable()).OnSignal(this.coughFinished, this.breathing.normal);
					this.notbreathing.TagTransition(new Tag[] { GameTags.NoOxygen }, this.breathing, true);
				}

				public StateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.Signal coughFinished;

				public SlimeSickness.SlimeLungComponent.States.BreathingStates breathing;

				public GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State notbreathing;

				public class BreathingStates : GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State
				{
					public GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State normal;

					public GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State cough;
				}
			}
		}
	}
}
