using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class SlimeLung : Disease
	{
		public SlimeLung()
			: base("SlimeLung", Disease.DiseaseType.Pathogen, Disease.Severity.Critical, 0.00025f, new List<Disease.InfectionVector> { Disease.InfectionVector.Inhalation }, 2400f, 1, new Disease.RangeInfo(283.15f, 293.15f, 363.15f, 373.15f), new Disease.RangeInfo(10f, 1200f, 1200f, 10f), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent())
		{
			this.doctorRequired = true;
			this.fatalityDuration = 6000f;
			base.AddDiseaseComponent(new CommonSickEffectDisease());
			base.AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[]
			{
				new AttributeModifier("BreathDelta", -1.1363636f, DUPLICANTS.DISEASES.SLIMELUNG.NAME, false, false),
				new AttributeModifier("Athletics", -3f, DUPLICANTS.DISEASES.SLIMELUNG.NAME, false, false)
			}));
			base.AddDiseaseComponent(new SlimeLung.SlimeLungComponent());
		}

		protected override void PopulateElemGrowthInfo()
		{
			base.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			base.AddGrowthRule(new Disease.GrowthRule
			{
				underPopulationDeathRate = new float?(2.6666667f),
				minCount = new int?(50),
				populationHalfLife = new float?(12000f),
				maxCount = new int?(1000000),
				overPopulationHalfLife = new float?(1200f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = 1
			});
			base.AddGrowthRule(new Disease.StateGrowthRule(Element.State.Solid)
			{
				populationHalfLife = new float?(3000f),
				overPopulationHalfLife = new float?(1200f),
				diffusionScale = new float?(1E-06f),
				minDiffusionCount = new int?(1000000)
			});
			base.AddGrowthRule(new Disease.ElementGrowthRule(SimHashes.SlimeMold)
			{
				underPopulationDeathRate = new float?(0f),
				populationHalfLife = new float?(-3000f),
				overPopulationHalfLife = new float?(3000f),
				maxCount = new int?(1100000),
				diffusionScale = new float?(0.05f)
			});
			base.AddGrowthRule(new Disease.ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			base.AddGrowthRule(new Disease.StateGrowthRule(Element.State.Gas)
			{
				populationHalfLife = new float?(12000f),
				overPopulationHalfLife = new float?(1200f),
				maxCount = new int?(15000),
				minDiffusionCount = new int?(5100),
				diffusionScale = new float?(0.005f)
			});
			base.AddGrowthRule(new Disease.ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				underPopulationDeathRate = new float?(0f),
				populationHalfLife = new float?(-300f),
				overPopulationHalfLife = new float?(1200f)
			});
			base.AddGrowthRule(new Disease.ElementGrowthRule(SimHashes.Oxygen)
			{
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(10f)
			});
			base.AddGrowthRule(new Disease.ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			base.AddGrowthRule(new Disease.StateGrowthRule(Element.State.Liquid)
			{
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(300f),
				maxCount = new int?(100000),
				diffusionScale = new float?(0.01f)
			});
			base.InitializeElemGrowthArray(ref this.elemExposureInfo, Disease.DEFAULT_GROWTH_INFO);
			base.AddExposureRule(new Disease.GrowthRule
			{
				underPopulationDeathRate = new float?(0f),
				minCount = new int?(100),
				populationHalfLife = new float?(float.PositiveInfinity),
				maxCount = new int?(1000000),
				overPopulationHalfLife = new float?(float.PositiveInfinity)
			});
			base.AddExposureRule(new Disease.ElementGrowthRule(SimHashes.DirtyWater)
			{
				populationHalfLife = new float?(-12000f)
			});
			base.AddExposureRule(new Disease.ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = new float?(-12000f)
			});
			base.AddExposureRule(new Disease.ElementGrowthRule(SimHashes.Oxygen)
			{
				populationHalfLife = new float?(3000f)
			});
			base.AddExposureRule(new Disease.ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f)
			});
		}

		public override List<Descriptor> GetDiseaseSourceDescriptors()
		{
			return new List<Descriptor>
			{
				new Descriptor(string.Format(DUPLICANTS.DISEASES.SLIMELUNG.DISEASE_SOURCE_DESCRIPTOR, GameUtil.GetFormattedTime(20f), GameUtil.GetFormattedDiseaseAmount(1000), base.Name), string.Format(DUPLICANTS.DISEASES.SLIMELUNG.DISEASE_SOURCE_DESCRIPTOR_TOOLTIP, GameUtil.GetFormattedTime(20f), GameUtil.GetFormattedDiseaseAmount(1000), base.Name), Descriptor.DescriptorType.DiseaseSource, false)
			};
		}

		private const float COUGH_FREQUENCY = 20f;

		private const float COUGH_MASS = 0.1f;

		private const int DISEASE_AMOUNT = 1000;

		private const float DEATH_TIMER = 6000f;

		public const string ID = "SlimeLung";

		public class SlimeLungComponent : Disease.DiseaseComponent
		{
			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				SlimeLung.SlimeLungComponent.StatesInstance statesInstance = new SlimeLung.SlimeLungComponent.StatesInstance(diseaseInstance);
				statesInstance.StartSM();
				return statesInstance;
			}

			public override void OnCure(GameObject go, object instance_data)
			{
				SlimeLung.SlimeLungComponent.StatesInstance statesInstance = (SlimeLung.SlimeLungComponent.StatesInstance)instance_data;
				statesInstance.StopSM("Cured");
			}

			public override List<Descriptor> GetSymptoms()
			{
				return new List<Descriptor>
				{
					new Descriptor(DUPLICANTS.DISEASES.SLIMELUNG.COUGH_SYMPTOM, DUPLICANTS.DISEASES.SLIMELUNG.COUGH_SYMPTOM_TOOLTIP, Descriptor.DescriptorType.SymptomAidable, false)
				};
			}

			public class StatesInstance : GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.GameInstance
			{
				public StatesInstance(DiseaseInstance master)
					: base(master)
				{
				}

				public Reactable GetReactable()
				{
					return new SelfEmoteReactable(base.master.gameObject, Db.Get().ChoreTypes.Cough, "anim_sneeze_kanim").AddStep(new EmoteReactable.EmoteStep
					{
						anim = "sneeze",
						finishcb = new Action<GameObject>(this.ProduceSlime)
					}).AddStep(new EmoteReactable.EmoteStep
					{
						anim = "sneeze_pst"
					}).AddStep(new EmoteReactable.EmoteStep
					{
						startcb = new Action<GameObject>(this.FinishedCoughing)
					});
				}

				private void ProduceSlime(GameObject cougher)
				{
					AmountInstance amountInstance = Db.Get().Amounts.Temperature.Lookup(cougher);
					int num = Grid.PosToCell(cougher);
					SimMessages.AddRemoveSubstance(num, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.Cough, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex("SlimeLung"), 1000, -1);
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, base.master.modifier.Name, 1000), cougher.transform, 1.5f, false);
				}

				private void FinishedCoughing(GameObject cougher)
				{
					base.sm.coughFinished.Trigger(this);
				}
			}

			public class States : GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance>
			{
				public override void InitializeStates(out StateMachine.BaseState default_state)
				{
					default_state = this.breathing;
					this.breathing.DefaultState(this.breathing.normal).TagTransition(GameTags.NoOxygen, this.notbreathing, false).ToggleSchedulePeriodic("Cough", 20f, delegate(SlimeLung.SlimeLungComponent.StatesInstance smi)
					{
						if (!smi.master.IsDoctored)
						{
							smi.GoTo(this.breathing.cough);
						}
					});
					this.breathing.cough.ToggleReactable((SlimeLung.SlimeLungComponent.StatesInstance smi) => smi.GetReactable()).OnSignal(this.coughFinished, this.breathing.normal);
					this.notbreathing.TagTransition(new Tag[] { GameTags.NoOxygen }, this.breathing, true);
				}

				public StateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.Signal coughFinished;

				public SlimeLung.SlimeLungComponent.States.BreathingStates breathing;

				public GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.State notbreathing;

				public class BreathingStates : GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.State
				{
					public GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.State normal;

					public GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.State cough;
				}
			}
		}
	}
}
